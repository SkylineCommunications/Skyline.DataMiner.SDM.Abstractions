namespace Skyline.DataMiner.SDM.Types.Shapes
{
	using System;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	internal class SdmObjectReferenceShapeHandler : IFieldShapeHandler
	{
		public bool CanHandle(FieldTypeShape shape)
		{
			return shape.ElementType.IsGenericType &&
				shape.ElementType.GetGenericTypeDefinition() == typeof(SdmObjectReference<>);
		}

		public bool SupportsComparer(FieldTypeShape shape, Comparer comparer)
		{
			return comparer is Comparer.Equals ||
				comparer is Comparer.NotEquals;
		}

		public object Convert(object value, FieldTypeShape shape)
		{
			// We accept String or SdmObjectReference<T> or ISdmObject<T>
			var convertMethod = shape.ElementType.GetMethod("Convert", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
			if (convertMethod is null)
			{
				throw new InvalidOperationException($"Type {shape.ElementType.FullName} does not have a static Convert method.");
			}

			return convertMethod.Invoke(null, new[] { value });
		}

		public FilterElement<T> BuildFilter<T>(FieldExposer exposer, Comparer comparer, object value, FieldTypeShape shape)
		{
			return new ScalarShapeHandler().BuildFilter<T>(exposer, comparer, value, shape);
		}
	}
}
