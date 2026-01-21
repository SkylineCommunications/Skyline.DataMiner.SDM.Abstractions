namespace Skyline.DataMiner.SDM
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>
	/// Represents errors that occur during CRUD operations for a specific <see cref="SdmObject{T}"/>.
	/// </summary>
	/// <typeparam name="T">The type of the SDM object associated with the exception.</typeparam>
	[Serializable]
	public class SdmCrudException<T> : SdmException
		where T : class, ISdmObject
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SdmCrudException{T}"/> class with the specified object.
		/// </summary>
		/// <param name="item">The SDM object associated with the exception.</param>
		public SdmCrudException(T item) : this(item, "An error occurred trying to perform a CRUD operation on the object.")
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SdmCrudException{T}"/> class with the specified object and error message.
		/// </summary>
		/// <param name="item">The SDM object associated with the exception.</param>
		/// <param name="message">The message that describes the error.</param>
		public SdmCrudException(T item, string message) : this(item, message, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SdmCrudException{T}"/> class with the specified object, error message, and inner exception.
		/// </summary>
		/// <param name="item">The SDM object associated with the exception.</param>
		/// <param name="message">The message that describes the error.</param>
		/// <param name="innerException">The exception that is the cause of the current exception.</param>
		public SdmCrudException(T item, string message, Exception innerException) : base(message, innerException)
		{
			Object = item;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SdmCrudException{T}"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		protected SdmCrudException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			Object = info.GetValue(nameof(Object), typeof(T)) as T;
		}

		/// <summary>
		/// Gets the SDM object associated with the exception.
		/// </summary>
		public T Object { get; }
	}
}
