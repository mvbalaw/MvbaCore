using System;
using System.Runtime.Serialization;

namespace MvbaCore
{
	[Serializable]
	public class EnvironmentException : Exception
	{
		/// <summary>
		///     Initializes a new instance of the <see cref = "EnvironmentException" /> class.
		/// </summary>
		public EnvironmentException()
			: base("An Environment Setup Exception Occurred.")
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref = "EnvironmentException" /> class.
		/// </summary>
		/// <param name = "message">The message that describes the error. </param>
		public EnvironmentException(string message)
			: base(message)
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref = "EnvironmentException" /> class.
		/// </summary>
		/// <param name = "message">The message that describes the error. </param>
		/// <param name = "innerException">
		///     The exception that is the cause of the current exception. If the <paramref name = "innerException" /> parameter 
		///     is not a null reference, the current exception is raised in a catch block that handles 
		///     the inner exception.
		/// </param>
		public EnvironmentException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref = "EnvironmentException" /> class 
		///     with serialized data.
		/// </summary>
		/// <param name = "info">
		///     The <see cref = "SerializationInfo" /> that holds the serialized object 
		///     data about the exception being thrown.
		/// </param>
		/// <param name = "context">
		///     The <see cref = "StreamingContext" /> that contains contextual information about the source or destination.
		/// </param>
		protected EnvironmentException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}