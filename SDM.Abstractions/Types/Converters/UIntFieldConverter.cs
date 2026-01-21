namespace Skyline.DataMiner.SDM.Types.Converters
{
	using System;

	internal class UIntFieldConverter : IFieldValueConverter
	{
		public Type FieldType { get => typeof(uint); }

		public object Convert(object value)
		{
			if (value is uint uintValue)
			{
				return uintValue;
			}

			return System.Convert.ToUInt32(value);
		}
	}
}
