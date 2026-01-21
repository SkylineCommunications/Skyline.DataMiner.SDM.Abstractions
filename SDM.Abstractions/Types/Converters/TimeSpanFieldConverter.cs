namespace Skyline.DataMiner.SDM.Types.Converters
{
	using System;

	internal class TimeSpanFieldConverter : IFieldValueConverter
	{
		public Type FieldType { get => typeof(TimeSpan); }

		public object Convert(object value)
		{
			if (value is TimeSpan timeSpanValue)
			{
				return timeSpanValue;
			}

			// TimeSpan doesn't have a direct Convert method, so we parse from string
			var converted = System.Convert.ToString(value);
			return TimeSpan.Parse(converted);
		}
	}
}
