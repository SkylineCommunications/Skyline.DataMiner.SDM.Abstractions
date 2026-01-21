namespace Skyline.DataMiner.SDM.Types.Shapes
{
	using System;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	internal class NullableShapeHandler : IFieldShapeHandler
	{
		public bool CanHandle(FieldTypeShape shape)
		{
			return shape.IsNullable && !shape.IsCollection;
		}

		public bool SupportsComparer(FieldTypeShape shape, Comparer comparer)
		{
			if (comparer is Comparer.Equals || comparer is Comparer.NotEquals)
			{
				return true;
			}

			var inner = FieldTypeShape.Analyze(shape.ElementType);
			return new ScalarShapeHandler().SupportsComparer(inner, comparer);
		}

		public object Convert(object value, FieldTypeShape shape)
		{
			if (value is null)
			{
				return null;
			}

			if (value is String s && String.IsNullOrEmpty(s))
			{
				return null;
			}

			var converter = SupportedTypesRegistry.GetConverter(shape.ElementType);
			return converter.Convert(value);
		}

		public FilterElement<T> BuildFilter<T>(FieldExposer exposer, Comparer comparer, object value, FieldTypeShape shape)
		{
			return new ScalarShapeHandler().BuildFilter<T>(exposer, comparer, value, FieldTypeShape.Analyze(shape.ElementType));
		}
	}
}
