namespace Skyline.DataMiner.SDM.Types.Converters
{
	using System;

	internal class ULongFieldConverter : IFieldValueConverter
	{
		public Type FieldType { get => typeof(ulong); }

		public object Convert(object value)
		{
			if (value is ulong ulongValue)
			{
				return ulongValue;
			}

			return System.Convert.ToUInt64(value);
		}
	}
}
