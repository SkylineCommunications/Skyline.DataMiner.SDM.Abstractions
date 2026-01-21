namespace Skyline.DataMiner.SDM.Types.Shapes
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.SDM.Exposers;

	internal class CollectionShapeHandler : IFieldShapeHandler
	{
		public bool CanHandle(FieldTypeShape shape)
		{
			return shape.IsCollection && !shape.IsNullable;
		}

		public bool SupportsComparer(FieldTypeShape shape, Comparer comparer)
		{
			return true;
		}

		public object Convert(object value, FieldTypeShape shape)
		{
			if (!(value is System.Collections.IEnumerable) || value is string)
			{
				return new ScalarShapeHandler().Convert(value, FieldTypeShape.Analyze(shape.ElementType));
			}

			if (!(value is System.Collections.IEnumerable input))
			{
				throw new ArgumentException("Value is not enumerable");
			}

			var converter = SupportedTypesRegistry.GetConverter(shape.ElementType);

			var listType = typeof(List<>).MakeGenericType(shape.ElementType);
			var list = (System.Collections.IList)Activator.CreateInstance(listType);

			foreach (var item in input)
			{
				list.Add(converter.Convert(item));
			}

			return list;
		}

		public FilterElement<T> BuildFilter<T>(FieldExposer exposer, Comparer comparer, object value, FieldTypeShape shape)
		{
			Type filterType;
			if (exposer is DynamicListExposer)
			{
				filterType = typeof(DynamicManagedListFilter<,>).MakeGenericType(typeof(T), shape.ElementType);
			}
			else
			{
				filterType = typeof(ManagedCollectionFilter<,>).MakeGenericType(typeof(T), shape.ElementType);
			}

			var filter = Activator.CreateInstance(
				filterType,
				new object[]
				{
					exposer,
					comparer,
					value,
				});

			return (FilterElement<T>)filter;
		}
	}
}
