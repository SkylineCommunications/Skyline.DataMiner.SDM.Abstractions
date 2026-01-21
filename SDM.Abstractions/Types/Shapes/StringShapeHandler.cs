namespace Skyline.DataMiner.SDM.Types.Shapes
{
	using System;
	using System.Linq;
	using System.Text.RegularExpressions;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	internal sealed class StringShapeHandler : IFieldShapeHandler
	{
		public bool CanHandle(FieldTypeShape shape)
		{
			return !shape.IsCollection && !shape.IsNullable && shape.ElementType == typeof(string);
		}

		public bool SupportsComparer(FieldTypeShape shape, Comparer comparer)
		{
			return true;
		}

		public object Convert(object value, FieldTypeShape shape)
		{
			var converter = SupportedTypesRegistry.GetConverter(shape.ElementType);
			return converter.Convert(value);
		}

		public FilterElement<T> BuildFilter<T>(FieldExposer exposer, Comparer comparer, object value, FieldTypeShape shape)
		{
			if (comparer is Comparer.Regex ||
				comparer is Comparer.NotRegex)
			{
				return BuildRegex<T>(exposer, comparer, value, shape);
			}

			var filterType = typeof(StringManagedFilter<>).MakeGenericType(typeof(T));
			var filter = Activator.CreateInstance(
				filterType,
				new object[]
				{
					exposer,
					comparer,
					value,
					StringComparison.OrdinalIgnoreCase,
				});

			return (FilterElement<T>)filter;
		}

		public FilterElement<T> BuildRegex<T>(FieldExposer exposer, Comparer comparer, object value, FieldTypeShape shape)
		{
			var filterType = typeof(StringRegexManagedFilter<>).MakeGenericType(typeof(T));
			var filter = Activator.CreateInstance(
				filterType,
				new object[]
				{
					exposer,
					comparer,
					value,
					RegexOptions.None,
				});

			return (FilterElement<T>)filter;
		}
	}
}
