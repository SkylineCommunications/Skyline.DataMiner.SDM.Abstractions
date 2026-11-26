namespace Skyline.DataMiner.SDM
{
	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	/// <summary>
	/// Describes how a selected field is populated in a partial object.
	/// </summary>
	/// <typeparam name="T">The type being queried.</typeparam>
	public interface IFieldProjection<T>
		where T : class
	{
		/// <summary>
		/// Gets the exposer for the selected field.
		/// </summary>
		FieldExposer Exposer { get; }

		/// <summary>
		/// Populates the selected field from an item into a partial object builder.
		/// </summary>
		/// <param name="builder">The builder to populate.</param>
		/// <param name="item">The item from which to read the field.</param>
		void Populate(PartialObject<T>.Builder builder, T item);
	}
}
