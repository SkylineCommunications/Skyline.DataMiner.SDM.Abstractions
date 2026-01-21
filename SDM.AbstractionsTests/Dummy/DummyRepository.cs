namespace SDM.AbstractionsTests.Shared
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Net.Helper;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.SDM;

	using SLDataGateway.API.Querying;
	using SLDataGateway.API.Types.Querying;

	internal class DummyRepository<T> : IRepository<T>
		where T : SdmObject<T>
	{
		private readonly List<T> _items = new List<T>();

		public DummyRepository()
		{
		}

		public DummyRepository(IEnumerable<T> initialData)
		{
			_items = initialData.ToList();
		}

		public long Count(FilterElement<T> filter)
		{
			return Read(filter).LongCount();
		}

		public long Count(IQuery<T> query)
		{
			return Read(query).LongCount();
		}

		public T Create(T oToCreate)
		{
			_items.Add(oToCreate);
			return oToCreate;
		}

		public T Update(T oToUpdate)
		{
			var index = _items.FindIndex(p => p.Identifier == oToUpdate.Identifier);
			if (index >= 0)
			{
				_items[index] = oToUpdate;
			}

			return oToUpdate;
		}

		public void Delete(T oToDelete)
		{
			_items.RemoveAll(item => item.Identifier == oToDelete.Identifier);
		}

		public IEnumerable<T> Read(FilterElement<T> filter)
		{
			return Read(filter.ToQuery());
		}

		public IEnumerable<T> Read(IQuery<T> query)
		{
			return query.ExecuteInMemory(_items);
		}

		public IEnumerable<IPagedResult<T>> ReadPaged(FilterElement<T> filter)
		{
			return ReadPaged(filter.ToQuery(), 30);
		}

		public IEnumerable<IPagedResult<T>> ReadPaged(IQuery<T> query)
		{
			return ReadPaged(query, 30);
		}

		public IEnumerable<IPagedResult<T>> ReadPaged(FilterElement<T> filter, int pageSize)
		{
			return ReadPaged(filter.ToQuery(), pageSize);
		}

		public IEnumerable<IPagedResult<T>> ReadPaged(IQuery<T> query, int pageSize)
		{
			var pageNumber = 0;
			var items = Read(query);
			var enumerator = items.Batch(pageSize).GetEnumerator();
			var hasNext = enumerator.MoveNext();

			while (hasNext)
			{
				var page = enumerator.Current;
				hasNext = enumerator.MoveNext();
				yield return new PagedResult<T>(page, pageNumber++, pageSize, hasNext);
			}
		}
	}
}
