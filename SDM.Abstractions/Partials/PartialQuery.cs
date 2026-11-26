namespace Skyline.DataMiner.SDM
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	using SLDataGateway.API.Types.Querying;

	/// <summary>
	/// Represents an immutable query that retrieves a selected set of fields.
	/// </summary>
	/// <typeparam name="T">The type being queried.</typeparam>
	public sealed class PartialQuery<T> : IPartialQuery<T>
		where T : class
	{
		private readonly ReadOnlyFieldSelection<T> _fields;

		/// <summary>
		/// Initializes a new instance of the <see cref="PartialQuery{T}"/> class with no selected fields.
		/// </summary>
		/// <param name="query">The underlying query.</param>
		/// <exception cref="ArgumentNullException"><paramref name="query"/> is <see langword="null"/>.</exception>
		public PartialQuery(IQuery<T> query)
			: this(query, new FieldSelection<T>())
		{
		}

		internal PartialQuery(IQuery<T> query, IReadOnlyFieldSelection<T> fields)
		{
			Query = query ?? throw new ArgumentNullException(nameof(query));
			if (fields is null)
			{
				throw new ArgumentNullException(nameof(fields));
			}

			_fields = new ReadOnlyFieldSelection<T>(fields);
		}

		/// <inheritdoc />
		public IQuery<T> Query { get; }

		/// <inheritdoc />
		public IReadOnlyFieldSelection<T> Fields => _fields;

		/// <inheritdoc />
		public IPartialQuery<T> WithFields(IReadOnlyFieldSelection<T> fields)
		{
			return new PartialQuery<T>(Query, fields);
		}

		/// <inheritdoc />
		public IPartialQuery<T> WithQuery(IQuery<T> query)
		{
			return new PartialQuery<T>(query, _fields);
		}

		/// <inheritdoc />
		public IPartialQuery<T> WithFilter(FilterElement<T> filter)
		{
			var query = Query.WithFilter(filter);
			return new PartialQuery<T>(query, _fields);
		}

		/// <inheritdoc />
		public IPartialQuery<T> WithOrder(IOrderBy order)
		{
			var query = Query.WithOrder(order);
			return new PartialQuery<T>(query, _fields);
		}

		/// <inheritdoc />
		public IPartialQuery<T> WithLimit(ILimitBy limit)
		{
			var query = Query.WithLimit(limit);
			return new PartialQuery<T>(query, _fields);
		}

		/// <inheritdoc />
		public IEnumerable<PartialObject<T>> ExecuteInMemory(IEnumerable<T> items)
		{
			if (items is null)
			{
				throw new ArgumentNullException(nameof(items));
			}

			foreach (var item in Query.ExecuteInMemory(items))
			{
				var builder = PartialObject<T>.CreateBuilder();
				foreach (var projection in _fields)
				{
					projection.Populate(builder, item);
				}

				yield return builder.Build();
			}
		}
	}
}
