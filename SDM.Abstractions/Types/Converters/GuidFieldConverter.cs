namespace Skyline.DataMiner.SDM.Types.Converters
{
	using System;

	internal class GuidFieldConverter : IFieldValueConverter
	{
		public Type FieldType { get => typeof(Guid); }

		public object Convert(object value)
		{
			if (value is Guid intValue)
			{
				return intValue;
			}

			var converted = System.Convert.ToString(value);
			return Guid.Parse(converted);
		}
	}
}
