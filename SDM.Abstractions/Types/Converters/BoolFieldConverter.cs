namespace Skyline.DataMiner.SDM.Types.Converters
{
	using System;

	internal class BoolFieldConverter : IFieldValueConverter
	{
		public Type FieldType { get => typeof(bool); }

		public object Convert(object value)
		{
			if (value is bool boolValue)
			{
				return boolValue;
			}

			return System.Convert.ToBoolean(value);
		}
	}
}
