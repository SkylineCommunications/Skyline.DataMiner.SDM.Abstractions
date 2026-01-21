namespace Skyline.DataMiner.SDM.Types.Converters
{
	using System;

	internal class FloatFieldConverter : IFieldValueConverter
	{
		public Type FieldType { get => typeof(float); }

		public object Convert(object value)
		{
			if (value is float floatValue)
			{
				return floatValue;
			}

			return System.Convert.ToSingle(value);
		}
	}
}
