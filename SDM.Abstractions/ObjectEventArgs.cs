namespace Skyline.DataMiner.SDM
{
	using System;

	/// <summary>
	/// Provides data for events related to SdmObject operations.
	/// </summary>
	/// <typeparam name="T">The type of the SdmObject.</typeparam>
	public class ObjectEventArgs<T> : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectEventArgs{T}"/> class.
		/// </summary>
		/// <param name="sdmObject">The SdmObject associated with the event.</param>
		public ObjectEventArgs(T sdmObject)
		{
			if (sdmObject == null)
			{
				throw new ArgumentNullException(nameof(sdmObject));
			}

			Object = sdmObject;
		}

		/// <summary>
		/// Gets the SdmObject associated with the event.
		/// </summary>
		public T Object { get; }
	}
}
