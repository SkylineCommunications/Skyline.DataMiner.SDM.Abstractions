namespace Skyline.DataMiner.SDM.Types.Converters
{
	using System;

	internal class DecimalFieldConverter : IFieldValueConverter
	{
		public Type FieldType { get => typeof(decimal); }

		public object Convert(object value)
		{
			if (value is decimal decimalValue)
			{
				return decimalValue;
			}

			return System.Convert.ToDecimal(value);
		}
	}
}
