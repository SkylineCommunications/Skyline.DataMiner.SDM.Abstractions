namespace Skyline.DataMiner.SDM.Types.Converters
{
	using System;
	using System.Reflection;

	internal class SdmObjectReferenceFieldConverter : IFieldValueConverter
	{
		private readonly MethodInfo _convertMethod;

		public SdmObjectReferenceFieldConverter(Type fieldType)
		{
			_convertMethod = typeof(SdmObjectReference<>).MakeGenericType(fieldType).GetMethod("Convert", BindingFlags.Public | BindingFlags.Static);
			if (_convertMethod is null)
			{
				throw new InvalidOperationException($"Type SdmObjectReference<{fieldType.FullName}> does not have a static Convert method.");
			}

			FieldType = fieldType ?? throw new ArgumentNullException(nameof(fieldType));
		}

		public Type FieldType { get; }

		public object Convert(object value)
		{
			if (value is null)
			{
				throw new ArgumentNullException(nameof(value), "Cannot convert null to non-nullable SdmObjectReference.");
			}

			return _convertMethod.Invoke(null, new[] { value });
		}
	}
}
