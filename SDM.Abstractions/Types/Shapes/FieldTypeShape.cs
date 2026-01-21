namespace Skyline.DataMiner.SDM.Types.Shapes
{
	using System;
	using System.Linq;

	internal sealed class FieldTypeShape
	{
		private FieldTypeShape(Type originalType, Type elementType, bool isNullable, bool isCollection)
		{
			OriginalType = originalType;
			ElementType = elementType;
			IsNullable = isNullable;
			IsCollection = isCollection;
		}

		public Type OriginalType { get; }

		public Type ElementType { get; }

		public bool IsNullable { get; }

		public bool IsCollection { get; }

		public static FieldTypeShape Analyze(Type fieldType)
		{
			var isNullable =
				fieldType.IsGenericType &&
				fieldType.GetGenericTypeDefinition() == typeof(Nullable<>);

			var nonNullableType = isNullable
				? fieldType.GetGenericArguments()[0]
				: fieldType;

			var isCollection =
				nonNullableType != typeof(string) &&
				typeof(System.Collections.IEnumerable).IsAssignableFrom(nonNullableType);
			if (!isCollection)
			{
				return new FieldTypeShape(
					fieldType,
					nonNullableType,
					isNullable,
					isCollection);
			}

			var elementType = nonNullableType;
			if (nonNullableType.IsArray)
			{
				// Arrays
				elementType = nonNullableType.GetElementType();
			}
			else
			{
				// IEnumerable<T>
				elementType = nonNullableType.GetGenericArguments().First();
			}

			return new FieldTypeShape(
				fieldType,
				elementType,
				isNullable,
				isCollection);
		}
	}
}
