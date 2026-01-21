namespace Skyline.DataMiner.SDM.Types.Converters
{
	using System;

	internal class ByteFieldConverter : IFieldValueConverter
	{
		public Type FieldType { get => typeof(byte); }

		public object Convert(object value)
		{
			if (value is byte byteValue)
			{
				return byteValue;
			}

			return System.Convert.ToByte(value);
		}
	}
}
