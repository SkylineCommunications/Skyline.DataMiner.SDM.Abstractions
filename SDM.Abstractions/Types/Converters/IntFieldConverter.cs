namespace Skyline.DataMiner.SDM.Types.Converters
{
	using System;

	internal class IntFieldConverter : IFieldValueConverter
	{
		public Type FieldType { get => typeof(int); }

		public object Convert(object value)
		{
			if (value is int intValue)
			{
				return intValue;
			}

			return System.Convert.ToInt32(value);
		}
	}
}
