namespace Skyline.DataMiner.SDM
{
	using System;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.SDM.Types;
	using Skyline.DataMiner.SDM.Types.Shapes;

	/// <summary>
	/// Factory class for creating filter elements based on exposers and comparers.
	/// </summary>
	public static class FilterElementFactory
	{
		/// <summary>
		/// Creates a <see cref="FilterElement{T}"/> using the specified field exposer, comparer, and value.
		/// </summary>
		/// <typeparam name="T">The type of the object to filter.</typeparam>
		/// <param name="exposer">The field exposer used to extract the value from the object.</param>
		/// <param name="comparer">The comparer to use for filtering.</param>
		/// <param name="value">The value to compare against.</param>
		/// <returns>A filter element for the specified parameters.</returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="exposer"/> is <c>null</c>.</exception>
		/// <exception cref="NotSupportedException">
		/// Thrown when the field type of the exposer is not supported or when the specified comparer is not valid for the field type.
		/// </exception>
		public static FilterElement<T> Create<T>(FieldExposer exposer, Comparer comparer, object value)
		{
			if (exposer is null)
			{
				throw new ArgumentNullException(nameof(exposer));
			}

			var shape = FieldTypeShape.Analyze(exposer.FieldType);
			var handler = ShapeLocator.Locate(exposer, shape);
			if (handler is null)
			{
				throw new NotSupportedException($"Field type of the exposer is not supported. Unsupported type: {shape.ElementType}");
			}

			if (!handler.SupportsComparer(shape, comparer))
			{
				throw new NotSupportedException($"Comparer '{comparer}' is not valid for field '{exposer.fieldName}' ({shape.ElementType}).");
			}

			var convertedValue = handler.Convert(value, shape);
			var filter = handler.BuildFilter<T>(exposer, comparer, convertedValue, shape);

			return filter;
		}
	}
}
