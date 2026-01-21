namespace Skyline.DataMiner.SDM.Types.Converters
{
	using System;

	internal class UShortFieldConverter : IFieldValueConverter
	{
		public Type FieldType { get => typeof(ushort); }

		public object Convert(object value)
		{
			if (value is ushort ushortValue)
			{
				return ushortValue;
			}

			return System.Convert.ToUInt16(value);
		}
	}
}
