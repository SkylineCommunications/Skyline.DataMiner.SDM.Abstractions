namespace Skyline.DataMiner.SDM.Types.Converters
{
	using System;

	internal class DoubleFieldConverter : IFieldValueConverter
	{
		public Type FieldType { get => typeof(double); }

		public object Convert(object value)
		{
			if (value is double doubleValue)
			{
				return doubleValue;
			}

			return System.Convert.ToDouble(value);
		}
	}
}
