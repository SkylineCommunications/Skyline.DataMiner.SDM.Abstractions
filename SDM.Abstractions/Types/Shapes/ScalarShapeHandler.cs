namespace Skyline.DataMiner.SDM.Types.Shapes
{
	using System;
	using System.Linq;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	internal sealed class ScalarShapeHandler : IFieldShapeHandler
	{
		public bool CanHandle(FieldTypeShape shape)
		{
			return !shape.IsCollection && !shape.IsNullable;
		}

		public bool SupportsComparer(FieldTypeShape shape, Comparer comparer)
		{
			var type = shape.ElementType;

			// Always valid
			if (comparer is Comparer.Equals ||
				comparer is Comparer.NotEquals)
				return true;

			// Ordering semantics (numbers, DateTime, etc.)
			if (IsOrdered(type))
			{
				return comparer is Comparer.LT ||
					comparer is Comparer.GT ||
					comparer is Comparer.LTE ||
					comparer is Comparer.GTE;
			}

			// Everything else is invalid
			return false;
		}

		public object Convert(object value, FieldTypeShape shape)
		{
			if (shape.ElementType == typeof(object))
			{
				return value;
			}

			var converter = SupportedTypesRegistry.GetConverter(shape.ElementType);
			return converter.Convert(value);
		}

		public FilterElement<T> BuildFilter<T>(FieldExposer exposer, Comparer comparer, object value, FieldTypeShape shape)
		{
			var filterType = typeof(ManagedFilter<,>).MakeGenericType(typeof(T), shape.ElementType);
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

		private static bool IsOrdered(Type type)
		{
			return typeof(IComparable).IsAssignableFrom(type) ||
				   type.GetInterfaces().Any(i =>
					   i.IsGenericType &&
					   i.GetGenericTypeDefinition() == typeof(IComparable<>));
		}
	}
}
