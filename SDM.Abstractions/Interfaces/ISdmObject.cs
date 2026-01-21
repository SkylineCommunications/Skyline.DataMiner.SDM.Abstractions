namespace Skyline.DataMiner.SDM
{
	/// <summary>
	/// Represents the base interface for all SDM objects.
	/// </summary>
	public interface ISdmObject
	{
		/// <summary>
		/// Gets the unique identifier of the SDM object.
		/// </summary>
		string Identifier { get; }
	}

	/// <summary>
	/// Represents a strongly-typed SDM object interface.
	/// </summary>
	/// <typeparam name="T">The type of the SDM object implementation.</typeparam>
	public interface ISdmObject<T> : ISdmObject
		where T : class
	{
	}
}
