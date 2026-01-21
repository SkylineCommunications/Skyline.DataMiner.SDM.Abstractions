namespace Skyline.DataMiner.SDM.Types.Converters
{
	using System;

	internal interface IFieldValueConverter
	{
		Type FieldType { get; }

		object Convert(object value);
	}
}
