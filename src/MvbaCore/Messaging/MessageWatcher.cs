//   * **************************************************************************
//   * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//   * This source code is subject to terms and conditions of the MIT License.
//   * A copy of the license can be found in the License.txt file
//   * at the root of this distribution.
//   * By using this source code in any fashion, you are agreeing to be bound by
//   * the terms of the MIT License.
//   * You must not remove this notice from this software.
//   * **************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

using JetBrains.Annotations;

using MvbaCore.Json;
using MvbaCore.Logging;
using MvbaCore.Services;

namespace MvbaCore.Messaging
{
	public static class Constants
	{
		public static readonly string MessageDataFileExtension = ".data";
		public static readonly string MessageHeaderFileExtension = ".request";
	}

	public interface IMessageHandler
	{
		bool CanHandle(MessageRequest messageRequest);
		bool Handle(MessageRequest header, string dataFilePath);
		void Quiesce();
	}

	[UsedImplicitly]
	public class MessageRequest
	{
		public string CreatedBy { get; set; }
		public int Priority { get; set; }
		public int SourceSystem { get; set; }
		public int TaskType { get; set; }
		public DateTime TimeStamp { get; set; }
		public string TypeOfData { get; set; }
	}

	public class MessageWrapper
	{
		public string File { get; set; }
		public DateTime FileDate { get; set; }
		public MessageRequest Header { get; set; }
		public bool Processed { get; set; }
	}

	public class MessageWatcher
	{
		private readonly string _archiveDirectory = "Archive";
		private readonly string _errorMessageDirectory = "Errors";
		private readonly IFileSystemService _fileSystemService;
		private readonly string _messageDir;
		private readonly IList<IMessageHandler> _messageHandlers;
		private readonly Func<MessageWrapper, bool> _processMessage;
		private readonly Func<string, bool> _processMessageFileNamed;

		private readonly TimeSpan _sleepTimeout = TimeSpan.FromSeconds(2);
		private readonly Stopwatch _timer;

		private bool _directoryChanged;
		private FileSystemWatcher _fileSystemWatcher;
		private List<MessageWrapper> _messages;
		private volatile bool _running;
		private Thread _thread;

		/// <summary>
		///     Initializes the message watcher
		/// </summary>
		public MessageWatcher(string messageDir,
			IFileSystemService fileSystemService,
			params IMessageHandler[] messageHandlers)
			:
				this(messageDir, fileSystemService, x => true, x => true, messageHandlers)
		{
		}

		/// <summary>
		///     Initializes the message watcher
		/// </summary>
		/// <param name="messageDir"></param>
		/// <param name="fileSystemService"></param>
		/// <param name="processMessage">a function to filter out messages by header info, defaults to x=&gt; true</param>
		/// <param name="messageHandlers"></param>
		public MessageWatcher(string messageDir,
			IFileSystemService fileSystemService,
			Func<MessageWrapper, bool> processMessage,
			params IMessageHandler[] messageHandlers)
			: this(messageDir, fileSystemService, x => true, processMessage, messageHandlers)
		{
		}

		/// <summary>
		///     Initializes the message watcher
		/// </summary>
		/// <param name="messageDir"></param>
		/// <param name="fileSystemService"></param>
		/// <param name="processMessageFileNamed">a function to filter out messages by file name, defaults to x=&gt; true</param>
		/// <param name="processMessage">a function to filter out messages by header info, defaults to x=&gt; true</param>
		/// <param name="messageHandlers"></param>
		public MessageWatcher(string messageDir,
			IFileSystemService fileSystemService,
			Func<string, bool> processMessageFileNamed,
			Func<MessageWrapper, bool> processMessage,
			params IMessageHandler[] messageHandlers)
		{
			_messages = new List<MessageWrapper>();
			_messageDir = messageDir;
			_fileSystemService = fileSystemService;
			_processMessageFileNamed = processMessageFileNamed;
			_processMessage = processMessage;
			_messageHandlers = messageHandlers;

			_timer = new Stopwatch();
			_timer.Start();
			_errorMessageDirectory = Path.Combine(_messageDir, _errorMessageDirectory);
			_directoryChanged = true;

			if (!_fileSystemService.DirectoryExists(_errorMessageDirectory))
			{
				Logger.Log(NotificationSeverity.Info, "Creating Error message directory ");
				try
				{
					var directory = _fileSystemService.CreateDirectory(_errorMessageDirectory);
					Logger.Log(NotificationSeverity.Info, "Created Error message directory " + directory.FullName);
				}
				catch (Exception e)
				{
					Logger.Log(NotificationSeverity.Error, "Unable to create Error message directory: " + e);
				}
			}

			_archiveDirectory = Path.Combine(_messageDir, _archiveDirectory);
			if (!_fileSystemService.DirectoryExists(_archiveDirectory))
			{
				Logger.Log(NotificationSeverity.Info, "Creating Archive message directory ");
				try
				{
					var directory = _fileSystemService.CreateDirectory(_archiveDirectory);
					Logger.Log(NotificationSeverity.Info, "Created Archive message directory " + directory.FullName);
				}
				catch (Exception e)
				{
					Logger.Log(NotificationSeverity.Error, "Unable to create Archive message directory: " + e);
				}
			}
		}

		private void LoadMessage(ICollection<MessageWrapper> messages, FileInfo file)
		{
			try
			{
				var message = new MessageWrapper
				              {
					              File = file.FullName,
					              FileDate = file.LastWriteTime,
					              Header = JsonUtility.DeserializeFromJsonFile<MessageRequest>(file.FullName)
				              };
				if (_processMessage(message))
				{
					messages.Add(message);
				}
			}
			catch (IOException ioException)
			{
				_directoryChanged = true;
				if (ioException.Message.Contains("Could not find file"))
				{
					_directoryChanged = true;
				}
				else
				{
					Console.WriteLine("---------------------------------------------------------------- file contention");
					Logger.Log(NotificationSeverity.Warning, "=> file " + file + " is in use... ", ioException);
				}
			}
			catch (Exception deserializeFileException)
			{
				MoveMessageRequestToErrorDirectory(file.FullName);
				Logger.Log(NotificationSeverity.Error, "=> Bad Message in file " + file, deserializeFileException);
			}
		}

		public IEnumerable<MessageWrapper> LoadMessages()
		{
			if (_directoryChanged && _timer.Elapsed.TotalSeconds > 10 || !_messages.Any() || _messages.All(x => x.Processed))
			{
				_directoryChanged = false;
				try
				{
					var files = _fileSystemService
						.GetDirectoryInfo(_messageDir)
						.GetFiles("*" + Constants.MessageHeaderFileExtension)
						.Where(x => _processMessageFileNamed(x.FullName));
					var currentMessages = _messages.Select(x => x.File).ToHashSet();
					var messages = new List<MessageWrapper>();
					foreach (var file in files.Where(x => !currentMessages.Contains(x.FullName)))
					{
						LoadMessage(messages, file);
					}
					if (messages.Any())
					{
						_messages = _messages
							.Where(x => !x.Processed)
							.Concat(messages)
							.OrderBy(x => x.Header.Priority)
							.ThenBy(x => x.Header.TimeStamp)
							.ToList();
					}
					else
					{
						_messages = _messages.Where(x => !x.Processed).ToList();
					}
				}
				catch (IOException)
				{
					Console.WriteLine("**************************************************************** file contention");
					_directoryChanged = true;
				}

				_timer.Restart();
			}
			return _messages;
		}

		private void MoveMessageRequestToErrorDirectory(string file)
		{
			try
			{
				// ReSharper disable AssignNullToNotNullAttribute
				TryMoveFileUntilSuccessful(file, Path.Combine(_errorMessageDirectory, Path.GetFileName(file)));
				// ReSharper restore AssignNullToNotNullAttribute
			}
			catch (Exception moveFileException)
			{
				Logger.Log(NotificationSeverity.Error, "=> Unable to move bad message file " + file, moveFileException);
			}
		}

		private void OnFileCreated(object sender, FileSystemEventArgs e)
		{
			_directoryChanged = true;
		}

		public void ProcessMessage(MessageWrapper messageWrapper)
		{
			try
			{
				var handler = _messageHandlers.FirstOrDefault(x => x.CanHandle(messageWrapper.Header));
				if (handler == null)
				{
					MoveMessageRequestToErrorDirectory(messageWrapper.File);
					Logger.Log(NotificationSeverity.Error, "=> Don't have a handler for " + messageWrapper.File);
					messageWrapper.Processed = true;
					return;
				}
				var dataFileName =
					// ReSharper disable AssignNullToNotNullAttribute
					Path.Combine(Path.GetDirectoryName(messageWrapper.File), Path.GetFileNameWithoutExtension(messageWrapper.File)) +
					// ReSharper restore AssignNullToNotNullAttribute
					Constants.MessageDataFileExtension;
				var deleteFile = handler.Handle(messageWrapper.Header, dataFileName);
				if (deleteFile)
				{
					// ReSharper disable AssignNullToNotNullAttribute
					var newFile = Path.Combine(_archiveDirectory, Path.GetFileName(messageWrapper.File));
					// ReSharper restore AssignNullToNotNullAttribute
					if (_fileSystemService.FileExists(newFile))
					{
						_fileSystemService.DeleteFile(newFile);
					}

					TryMoveFileUntilSuccessful(messageWrapper.File, newFile);

					// ReSharper disable AssignNullToNotNullAttribute
					newFile = Path.Combine(_archiveDirectory, Path.GetFileName(dataFileName));
					// ReSharper restore AssignNullToNotNullAttribute
					if (_fileSystemService.FileExists(newFile))
					{
						_fileSystemService.DeleteFile(newFile);
					}
					_fileSystemService.MoveFile(dataFileName, newFile);
				}
				else
				{
					MoveMessageRequestToErrorDirectory(messageWrapper.File);
				}
				messageWrapper.Processed = true;
			}
			catch (IOException e)
			{
				if (e.Message.StartsWith("Could not find file"))
				{
					MoveMessageRequestToErrorDirectory(messageWrapper.File);
					Logger.Log(NotificationSeverity.Error, "=> Error while processing " + messageWrapper.File, e);
					messageWrapper.Processed = true;
				}
				else
				{
					Console.WriteLine("================================================================ file contention for " + messageWrapper.File + " = " + e.Message);
				}
			}
			catch (InvalidOperationException e)
			{
				MoveMessageRequestToErrorDirectory(messageWrapper.File);
				Logger.Log(NotificationSeverity.Error, "=> Error while processing " + messageWrapper.File, e);
				messageWrapper.Processed = true;
			}
			catch (Exception e)
			{
				if (e.GetType().Name.Contains("LazyInitializationException"))
				{
					if (!e.Message.EndsWith("Could not initialize proxy - no Session."))
					{
						MoveMessageRequestToErrorDirectory(messageWrapper.File);
						Logger.Log(NotificationSeverity.Error, "=> Error while processing " + messageWrapper.File, e);
						messageWrapper.Processed = true;
					}
				}
				else
				{
					MoveMessageRequestToErrorDirectory(messageWrapper.File);
					Logger.Log(NotificationSeverity.Error, "=> Error while processing " + messageWrapper.File, e);
					messageWrapper.Processed = true;
				}
			}
		}

		public void Start()
		{
			// try to re-run previous failures on startup
			try
			{
				var previousErrorHeaderFiles = _fileSystemService.GetFiles(_errorMessageDirectory, "*" + Constants.MessageHeaderFileExtension);
				foreach (var headerFile in previousErrorHeaderFiles)
				{
					if (_fileSystemService.FileExists(Path.Combine(_messageDir, Path.GetFileNameWithoutExtension(headerFile) + Constants.MessageDataFileExtension)))
					{
//// ReSharper disable AssignNullToNotNullAttribute
						_fileSystemService.MoveFile(headerFile, Path.Combine(_messageDir, Path.GetFileName(headerFile)));
//// ReSharper restore AssignNullToNotNullAttribute
					}
				}
			}
			catch
			{
			}

			_thread = new Thread(WatchForMessages)
			          {
				          IsBackground = true
			          };
			_running = true;
			_thread.Start();
		}

		public void Stop()
		{
			Logger.Log(NotificationSeverity.Info, "Stopping...");
			_running = false;
			try
			{
				_thread.Join(TimeSpan.FromSeconds(10));
			}
			catch
			{
			}

			for (var i = 0; i < Math.Max(_sleepTimeout.TotalSeconds, 20); i++)
			{
				if (_thread.IsAlive)
				{
					Thread.Sleep(TimeSpan.FromSeconds(2));
				}
			}

			try
			{
				if (_thread.IsAlive)
				{
					_thread.Abort();
				}
			}
			catch
			{
			}
			Logger.Log(NotificationSeverity.Info, "Stopped...");
		}

		private void TryMoveFileUntilSuccessful(string source, string dest)
		{
			while (_running)
			{
				try
				{
					_fileSystemService.MoveFile(source, dest);
					return;
				}
				catch (ApplicationException aException)
				{
					if (aException.InnerException is IOException)
					{
						var ioException = aException.InnerException as IOException;
						if (ioException.Message.StartsWith("Cannot create a file when that file already exists"))
						{
							try
							{
								_fileSystemService.DeleteFile(source);
								return;
							}
							catch (Exception exception)
							{
								Logger.Log(NotificationSeverity.Error, "=> Unable to delete message file that also exists in Error folder " + source, exception);
							}
						}
						else if (ioException.Message.StartsWith("The process cannot access the file because it is being used by another process."))
						{
							if (_running)
							{
								// tries forever
								Thread.Sleep(TimeSpan.FromSeconds(1));
								continue;
							}
							return;
						}
					}
					throw;
				}
			}
		}

		private void WatchForMessages()
		{
			_fileSystemWatcher = new FileSystemWatcher(_messageDir, "*" + Constants.MessageHeaderFileExtension);
			_fileSystemWatcher.Created += OnFileCreated;
			_fileSystemWatcher.EnableRaisingEvents = true;
			while (_running)
			{
				LoadMessages();

				var stopwatch = new Stopwatch();
				stopwatch.Start();

				var messageWrapper = _messages.FirstOrDefault(x => !x.Processed);
				if (messageWrapper != null)
				{
					do
					{
						Logger.Log(NotificationSeverity.Info, "=> Processing " + Path.GetFileName(messageWrapper.File));

						ProcessMessage(messageWrapper);

						messageWrapper = _messages.FirstOrDefault(x => !x.Processed);
//// ReSharper disable ConditionIsAlwaysTrueOrFalse
					} while (_running && messageWrapper != null && stopwatch.Elapsed.TotalSeconds < 10);
//// ReSharper restore ConditionIsAlwaysTrueOrFalse

//// ReSharper disable ConditionIsAlwaysTrueOrFalse
					if (!_running || messageWrapper == null)
//// ReSharper restore ConditionIsAlwaysTrueOrFalse
					{
						foreach (var messageHandler in _messageHandlers)
						{
							messageHandler.Quiesce();
						}
					}
				}
				else
				{
//// ReSharper disable ConditionIsAlwaysTrueOrFalse
					if (_running)
//// ReSharper restore ConditionIsAlwaysTrueOrFalse
					{
						Thread.Sleep(_sleepTimeout);
					}
				}
			}
		}
	}
}