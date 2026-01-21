namespace Skyline.DataMiner.SDM.Types.Converters
{
	using System;

	internal class StringFieldConverter : IFieldValueConverter
	{
		public Type FieldType { get => typeof(string); }

		public object Convert(object value)
		{
			if (value is int intValue)
			{
				return intValue;
			}

			return System.Convert.ToString(value);
		}
	}
}
