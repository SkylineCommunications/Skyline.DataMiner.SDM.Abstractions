namespace Skyline.DataMiner.SDM.Exposers
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	[Serializable]
	public class SerializableCollectionExposer<TFilter, TField> : ISerializable, ISerializableExposer
		where TFilter : class
	{
		public SerializableCollectionExposer(CollectionExposer<TFilter, TField> exposer)
		{
			Exposer = exposer;
		}

		protected SerializableCollectionExposer(SerializationInfo info, StreamingContext context)
		{
			Exposer<TFilter, IEnumerable<TField>> listExposer = Exposer<TFilter, IEnumerable<TField>>.findExposer(info.GetString("listExposerName"));
			Exposer = new CollectionExposer<TFilter, TField>(listExposer);
		}

		public CollectionExposer<TFilter, TField> Exposer { get; set; }

		FieldExposer ISerializableExposer.Exposer => Exposer;

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("listExposerName", Exposer.fieldName, typeof(string));
		}
	}
}
