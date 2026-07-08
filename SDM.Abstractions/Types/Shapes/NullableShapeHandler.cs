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
			var filterType = typeof(ManagedFilter<,>).MakeGenericType(typeof(T), shape.OriginalType);
			var createMethod = filterType.GetMethod(
				"Create",
				System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static,
				null,
				new[]
				{
					exposer.GetType(),
					typeof(Comparer),
					shape.ElementType,
				},
				null);
			if (createMethod is null)
			{
				throw new InvalidOperationException($"Create method not found on {filterType}");
			}

			return (FilterElement<T>)createMethod.Invoke(null, new object[] { exposer, comparer, value });
		}
	}
}
