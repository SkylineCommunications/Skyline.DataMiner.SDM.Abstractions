namespace Skyline.DataMiner.SDM.Types.Converters
{
	using System;

	internal class SByteFieldConverter : IFieldValueConverter
	{
		public Type FieldType { get => typeof(sbyte); }

		public object Convert(object value)
		{
			if (value is sbyte sbyteValue)
			{
				return sbyteValue;
			}

			return System.Convert.ToSByte(value);
		}
	}
}
