namespace Skyline.DataMiner.SDM.Types.Shapes
{
	using System;

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
			var isNullable = true;
			var nonNullableType = Nullable.GetUnderlyingType(fieldType);
			if (nonNullableType is null)
			{
				isNullable = false;
				nonNullableType = fieldType;
			}

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

			var elementType = default(Type);
			if (nonNullableType.IsArray)
			{
				// Arrays
				elementType = nonNullableType.GetElementType();
			}
			else
			{
				// IEnumerable<T>
				elementType = nonNullableType.GetGenericArguments()[0];
			}

			return new FieldTypeShape(
				fieldType,
				elementType,
				isNullable,
				isCollection);
		}
	}
}
