namespace Skyline.DataMiner.SDM.Types.Converters
{
	using System;

	internal class TypeFieldConverter : IFieldValueConverter
	{
		public Type FieldType { get => typeof(Type); }

		public object Convert(object value)
		{
			if (value is null || value is Type)
			{
				return value;
			}

			throw new InvalidCastException($"Cannot convert value of type '{value.GetType().FullName}' to '{typeof(Type).FullName}'.");
		}
	}
}
