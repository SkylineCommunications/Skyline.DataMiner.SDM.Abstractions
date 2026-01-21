namespace Skyline.DataMiner.SDM.Types
{
	using System.Linq;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.SDM.Types.Shapes;

	internal class ShapeLocator
	{
		private static readonly IFieldShapeHandler[] _shapedHandlers = new IFieldShapeHandler[]
		{
			new CollectionShapeHandler(),
			new NullableShapeHandler(),
			new SdmObjectReferenceShapeHandler(),
			new StringShapeHandler(),
			new ScalarShapeHandler(),
		};

		public static IFieldShapeHandler Locate(FieldExposer exposer, FieldTypeShape shape)
		{
			if (exposer is DynamicListExposer)
			{
				return new CollectionShapeHandler();
			}

			var handler = _shapedHandlers.FirstOrDefault(h => h.CanHandle(shape));
			return handler;
		}
	}
}
