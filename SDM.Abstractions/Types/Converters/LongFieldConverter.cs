namespace Skyline.DataMiner.SDM.Types.Converters
{
	using System;

	internal class LongFieldConverter : IFieldValueConverter
	{
		public Type FieldType { get => typeof(long); }

		public object Convert(object value)
		{
			if (value is long longValue)
			{
				return longValue;
			}

			return System.Convert.ToInt64(value);
		}
	}
}
