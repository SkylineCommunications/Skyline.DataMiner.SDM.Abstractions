namespace Skyline.DataMiner.SDM
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	/// <summary>
	/// Describes how a typed field is projected into a partial object.
	/// </summary>
	/// <typeparam name="T">The type being queried.</typeparam>
	/// <typeparam name="TField">The type of the selected field.</typeparam>
	public sealed class FieldProjection<T, TField> : IFieldProjection<T>
		where T : class
	{
		private readonly Exposer<T, TField> _exposer;
		private readonly DynamicListExposer<T, TField> _dynamicListExposer;

		/// <summary>
		/// Initializes a new instance of the <see cref="FieldProjection{T, TField}"/> class.
		/// </summary>
		/// <param name="exposer">The exposer for the field to project.</param>
		/// <exception cref="ArgumentNullException"><paramref name="exposer"/> is <see langword="null"/>.</exception>
		public FieldProjection(Exposer<T, TField> exposer)
		{
			_exposer = exposer ?? throw new ArgumentNullException(nameof(exposer));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FieldProjection{T, TField}"/> class.
		/// </summary>
		/// <param name="exposer">The dynamic list exposer for the field to project.</param>
		/// <exception cref="ArgumentNullException"><paramref name="exposer"/> is <see langword="null"/>.</exception>
		public FieldProjection(DynamicListExposer<T, TField> exposer)
		{
			_dynamicListExposer = exposer ?? throw new ArgumentNullException(nameof(exposer));
		}

		/// <inheritdoc />
		public FieldExposer Exposer
		{
			get
			{
				if (_exposer is null)
				{
					return _dynamicListExposer;
				}
				else
				{
					return _exposer;
				}
			}
		}

		/// <inheritdoc />
		public void Populate(PartialObject<T>.Builder builder, T item)
		{
			if (_exposer is null)
			{
				var enumerable = _dynamicListExposer.ListExposer.internalFunc(item);
				if (enumerable is null)
				{
					builder.Include(_dynamicListExposer, null);
					return;
				}

				var items = new List<TField>();
				foreach (var element in enumerable)
				{
					if (element is null && default(TField) is null)
					{
						items.Add(default);
						continue;
					}

					if (!(element is TField fieldValue))
					{
						throw new InvalidOperationException($"The dynamic list exposer '{_dynamicListExposer}' returned an element of type '{element.GetType()}', which cannot be cast to the expected type '{typeof(TField)}'.");
					}

					items.Add(fieldValue);
				}

				builder.Include(_dynamicListExposer, items);
			}
			else
			{
				builder.Include(_exposer, _exposer.internalFunc(item));
			}
		}
	}
}
