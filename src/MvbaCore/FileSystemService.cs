using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Web.Hosting;

using JetBrains.Annotations;

namespace MvbaCore
{
	public interface IFileSystemService
	{
		void AppendAllText([NotNull] string filePath, [NotNull] string textToAppend);

		[NotNull]
		DirectoryInfo CreateDirectory([NotNull] string directoryPath);

		[NotNull]
		StreamWriter CreateFile([NotNull] string filePath);

		[NotNull]
		StreamWriter CreateTextFile([NotNull] string filePath);

		void DeleteDirectoryContents([NotNull] string dirPath);

		void DeleteDirectoryRecursive([NotNull] string dirPath);
		bool DeleteFile([NotNull] string filePath);
		bool DirectoryExists([NotNull] string directoryPath);
		bool FileExists([NotNull] string filePath);

		[NotNull]
		string GetCurrentWebApplicationPath();

		[NotNull]
		DirectoryInfo GetDirectoryInfo([NotNull] string directoryPath);

		string[] GetFiles(string filePath, string searchPattern);

		[NotNull]
		IEnumerable<string> GetNamesOfFilesInDirectory([NotNull] string directoryPath);

		void MoveFile([NotNull] string oldFilePath, [NotNull] string newfilePath);

		[NotNull]
		string[] ReadAllLines([NotNull] string filePath);

		[NotNull]
		string ReadAllText([NotNull] string filePath);

		void Writefile(string filePath, Stream data);

		void Writefile(string filePath, string data);
	}

	public class FileSystemService : IFileSystemService
	{
		[NotNull]
		public string GetCurrentWebApplicationPath()
		{
			string env = HostingEnvironment.ApplicationPhysicalPath;
			return env;
		}

		[NotNull]
		public DirectoryInfo GetDirectoryInfo([NotNull] string directoryPath)
		{
			return new DirectoryInfo(directoryPath);
		}

		[NotNull]
		public IEnumerable<string> GetNamesOfFilesInDirectory([NotNull] string directoryPath)
		{
			return Directory.GetFiles(directoryPath);
		}

		public bool DirectoryExists([NotNull] string directoryPath)
		{
			return Directory.Exists(directoryPath);
		}

		[NotNull]
		public DirectoryInfo CreateDirectory([NotNull] string directoryPath)
		{
			if (!Directory.Exists(directoryPath))
			{
				return Directory.CreateDirectory(directoryPath);
			}
			return new DirectoryInfo(directoryPath);
		}

		[NotNull]
		public StreamWriter CreateTextFile([NotNull] string filePath)
		{
			return File.CreateText(filePath);
		}

		public bool FileExists([NotNull] string filePath)
		{
			return File.Exists(filePath);
		}

		public void Writefile(string filePath, Stream data)
		{
			File.WriteAllBytes(filePath, data.ReadAllBytes());
		}

		public void Writefile(string filePath, string data)
		{
			File.WriteAllText(filePath, data);
		}

		[NotNull]
		public StreamWriter CreateFile([NotNull] string filePath)
		{
			try
			{
				return new StreamWriter(new FileStream(filePath, FileMode.Create, FileAccess.Write));
			}
			catch (DirectoryNotFoundException exception)
			{
				throw new EnvironmentException("Output Directory Not Found", exception);
			}
			catch (SecurityException exception)
			{
				throw new EnvironmentException("Unable to Open Or Create output file", exception);
			}
			catch (UnauthorizedAccessException exception)
			{
				throw new EnvironmentException("Unauthorized to Open or Create output file", exception);
			}
			catch (PathTooLongException exception)
			{
				throw new EnvironmentException("Output Directory is > 248 characters or output file is > 260 characters", exception);
			}
		}

		public void MoveFile([NotNull] string oldFilePath, [NotNull] string newFilePath)
		{
			try
			{
				File.Move(oldFilePath, newFilePath);
			}
			catch (Exception ex)
			{
				throw new ApplicationException("unable to move file " + oldFilePath + " to " + newFilePath, ex);
			}
		}

		public void AppendAllText([NotNull] string filePath, [NotNull] string textToAppend)
		{
			File.AppendAllText(filePath, textToAppend);
		}

		[NotNull]
		public string[] ReadAllLines([NotNull] string filePath)
		{
			return File.ReadAllLines(filePath);
		}

		[NotNull]
		public string ReadAllText([NotNull] string filePath)
		{
			return File.ReadAllText(filePath);
		}

		public bool DeleteFile([NotNull] string filePath)
		{
			if (!FileExists(filePath))
			{
				return true;
			}
			try
			{
				File.Delete(filePath);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public void DeleteDirectoryContents([NotNull] string dirPath)
		{
			if (!Directory.Exists(dirPath))
			{
				return;
			}
			foreach (string directory in Directory.GetDirectories(dirPath))
			{
				DeleteDirectoryRecursive(directory);
			}
			foreach (string file in Directory.GetFiles(dirPath))
			{
				File.Delete(file);
			}
		}

		public void DeleteDirectoryRecursive([NotNull] string dirPath)
		{
			if (!Directory.Exists(dirPath) || dirPath.Split(Path.DirectorySeparatorChar).Last() == ".svn")
			{
				return;
			}

			DeleteDirectoryContents(dirPath);

			Directory.Delete(dirPath);
		}

		public string[] GetFiles(string filePath, string searchPattern)
		{
			return Directory.GetFiles(filePath, searchPattern);
		}
	}
}