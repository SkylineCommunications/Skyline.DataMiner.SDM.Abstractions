namespace Skyline.DataMiner.SDM.Types.Converters
{
	using System;

	internal class DateTimeFieldConverter : IFieldValueConverter
	{
		public Type FieldType { get => typeof(DateTime); }

		public object Convert(object value)
		{
			if (value is DateTime dateTimeValue)
			{
				return dateTimeValue;
			}

			return System.Convert.ToDateTime(value);
		}
	}
}
