namespace Skyline.DataMiner.SDM
{
	using System.Collections.Generic;

	/// <summary>
	/// Provides read-only access to the fields selected for a partial query.
	/// </summary>
	/// <typeparam name="T">The type being queried.</typeparam>
	public interface IReadOnlyFieldSelection<T> : IReadOnlyCollection<IFieldProjection<T>>
		where T : class
	{
	}
}
