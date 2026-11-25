namespace Skyline.DataMiner.SDM
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	using SLDataGateway.API.Types.Querying;

	public sealed class PartialQuery<T> : IPartialQuery<T>
		where T : class
	{
		public PartialQuery(IQuery<T> query)
			: this(query, new ExposerCollection<T>())
		{
		}

		internal PartialQuery(IQuery<T> query, IReadOnlyExposerCollection<T> fields)
		{
			Query = query ?? throw new ArgumentNullException(nameof(query));
			Fields = fields is null
				? throw new ArgumentNullException(nameof(fields))
				: new ExposerCollection<T>(fields);
		}

		public IQuery<T> Query { get; }

		public IReadOnlyExposerCollection<T> Fields { get; }

		public IPartialQuery<T> WithFields(IReadOnlyExposerCollection<T> fields)
		{
			return new PartialQuery<T>(Query, fields);
		}

		public IPartialQuery<T> WithQuery(IQuery<T> query)
		{
			return new PartialQuery<T>(query, Fields);
		}

		public IPartialQuery<T> WithFilter(FilterElement<T> filter)
		{
			var query = Query.WithFilter(filter);
			return new PartialQuery<T>(query, Fields);
		}

		public IPartialQuery<T> WithOrder(IOrderBy order)
		{
			var query = Query.WithOrder(order);
			return new PartialQuery<T>(query, Fields);
		}

		public IPartialQuery<T> WithLimit(ILimitBy limit)
		{
			var query = Query.WithLimit(limit);
			return new PartialQuery<T>(query, Fields);
		}

		public IEnumerable<PartialObject<T>> ExecuteInMemory(IEnumerable<T> items)
		{
			if (items is null)
			{
				throw new ArgumentNullException(nameof(items));
			}

			foreach (var item in Query.ExecuteInMemory(items))
			{
				yield return new PartialObject<T>(item, Fields);
			}
		}
	}
}
