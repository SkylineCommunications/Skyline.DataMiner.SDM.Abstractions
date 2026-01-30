namespace Skyline.DataMiner.SDM.Exposers
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	/// <summary>
	/// Represents a serializable wrapper for a <see cref="CollectionExposer{TFilter, TField}"/>.
	/// </summary>
	/// <typeparam name="TFilter">The type of the filter object.</typeparam>
	/// <typeparam name="TField">The type of the field elements in the collection.</typeparam>
	[Serializable]
	public class SerializableCollectionExposer<TFilter, TField> : ISerializable, ISerializableExposer
		where TFilter : class
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SerializableCollectionExposer{TFilter, TField}"/> class.
		/// </summary>
		/// <param name="exposer">The collection exposer to wrap.</param>
		public SerializableCollectionExposer(CollectionExposer<TFilter, TField> exposer)
		{
			Exposer = exposer;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SerializableCollectionExposer{TFilter, TField}"/> class from serialized data.
		/// </summary>
		/// <param name="info">The serialization info containing the serialized data.</param>
		/// <param name="context">The streaming context for the serialization.</param>
		protected SerializableCollectionExposer(SerializationInfo info, StreamingContext context)
		{
			Exposer<TFilter, IEnumerable<TField>> listExposer = Exposer<TFilter, IEnumerable<TField>>.findExposer(info.GetString("listExposerName"));
			Exposer = new CollectionExposer<TFilter, TField>(listExposer);
		}

		/// <summary>
		/// Gets or sets the collection exposer.
		/// </summary>
		public CollectionExposer<TFilter, TField> Exposer { get; set; }

		/// <summary>
		/// Gets the exposer as a <see cref="FieldExposer"/>.
		/// </summary>
		FieldExposer ISerializableExposer.Exposer => Exposer;

		/// <summary>
		/// Populates a <see cref="SerializationInfo"/> with the data needed to serialize the target object.
		/// </summary>
		/// <param name="info">The serialization info to populate with data.</param>
		/// <param name="context">The destination for this serialization.</param>
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("listExposerName", Exposer.fieldName, typeof(string));
		}
	}
}
