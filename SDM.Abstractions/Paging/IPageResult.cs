namespace Skyline.DataMiner.SDM
{
	using System.Collections.Generic;

	/// <summary>
	/// Represents a paged result set.
	/// </summary>
	/// <typeparam name="T">The type of the elements in the page.</typeparam>
	public interface IPagedResult<out T> : IReadOnlyList<T>
		where T : class
	{
		/// <summary>
		/// Gets the current page number (0-based).
		/// </summary>
		int PageNumber { get; }

		/// <summary>
		/// Gets a value indicating whether there is a next page.
		/// </summary>
		bool HasNextPage { get; }
	}
}
