namespace Skyline.DataMiner.SDM
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>
	/// Represents errors that occur during SDM operations for a specific <see cref="SdmObject{T}"/>.
	/// </summary>
	[Serializable]
	public class SdmException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SdmException"/> class.
		/// </summary>
		public SdmException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SdmException"/> class with a specified error message.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public SdmException(string message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SdmException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		/// <param name="innerException">The exception that is the cause of the current exception.</param>
		public SdmException(string message, Exception innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SdmException"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		protected SdmException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
