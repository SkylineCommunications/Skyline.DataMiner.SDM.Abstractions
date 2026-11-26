namespace Skyline.DataMiner.SDM
{
	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	/// <summary>
	/// Provides access to, and allows modification of, the fields selected for a partial query.
	/// </summary>
	/// <typeparam name="T">The type being queried.</typeparam>
	public interface IFieldSelection<T> : IReadOnlyFieldSelection<T>
		where T : class
	{
		/// <summary>
		/// Adds a field to the selection when it is not already contained.
		/// </summary>
		/// <typeparam name="TField">The field type.</typeparam>
		/// <param name="exposer">The exposer to add.</param>
		/// <returns><see langword="true"/> when the exposer was added; otherwise, <see langword="false"/>.</returns>
		bool Add<TField>(Exposer<T, TField> exposer);

		/// <summary>
		/// Adds a dynamic list field to the selection when it is not already contained.
		/// </summary>
		/// <typeparam name="TField">The element type of the dynamic list.</typeparam>
		/// <param name="exposer">The dynamic list exposer to add.</param>
		/// <returns><see langword="true"/> when the exposer was added; otherwise, <see langword="false"/>.</returns>
		bool Add<TField>(DynamicListExposer<T, TField> exposer);
	}
}
