namespace Skyline.DataMiner.SDM.Types.Shapes
{
	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	internal interface IFieldShapeHandler
	{
		bool CanHandle(FieldTypeShape shape);

		bool SupportsComparer(FieldTypeShape shape, Comparer comparer);

		object Convert(object value, FieldTypeShape shape);

		FilterElement<T> BuildFilter<T>(FieldExposer exposer, Comparer comparer, object value, FieldTypeShape shape);
	}
}
