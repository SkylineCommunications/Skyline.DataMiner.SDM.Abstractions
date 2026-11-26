namespace Skyline.DataMiner.SDM
{
	using System;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	/// <summary>
	/// Describes how a typed field is projected into a partial object.
	/// </summary>
	/// <typeparam name="T">The type being queried.</typeparam>
	/// <typeparam name="TField">The type of the selected field.</typeparam>
	public sealed class FieldProjection<T, TField> : IFieldProjection<T>
		where T : class
	{
		private readonly Exposer<T, TField> exposer;

		/// <summary>
		/// Initializes a new instance of the <see cref="FieldProjection{T, TField}"/> class.
		/// </summary>
		/// <param name="exposer">The exposer for the field to project.</param>
		/// <exception cref="ArgumentNullException"><paramref name="exposer"/> is <see langword="null"/>.</exception>
		public FieldProjection(Exposer<T, TField> exposer)
		{
			this.exposer = exposer ?? throw new ArgumentNullException(nameof(exposer));
		}

		/// <inheritdoc />
		public FieldExposer Exposer => exposer;

		/// <inheritdoc />
		public void Populate(PartialObject<T>.Builder builder, T item)
		{
			builder.Include(exposer, exposer.internalFunc(item));
		}
	}
}
