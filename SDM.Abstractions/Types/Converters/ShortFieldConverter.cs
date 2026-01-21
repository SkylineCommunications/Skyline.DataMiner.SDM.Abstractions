namespace Skyline.DataMiner.SDM.Types.Converters
{
	using System;

	internal class ShortFieldConverter : IFieldValueConverter
	{
		public Type FieldType { get => typeof(short); }

		public object Convert(object value)
		{
			if (value is short shortValue)
			{
				return shortValue;
			}

			return System.Convert.ToInt16(value);
		}
	}
}
