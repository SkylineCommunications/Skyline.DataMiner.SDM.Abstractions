namespace SDM.AbstractionsTests
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Net.Helper;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.SDM;

	using SLDataGateway.API.Querying;
	using SLDataGateway.API.Types.Querying;

	public class ExampleObject : SdmObject<ExampleObject>
	{
		public override string Identifier { get; set; } = Guid.NewGuid().ToString();

		public string Name { get; set; }

		public Info Info { get; set; }
	}

	public class Info
	{
		public int IntProperty { get; set; }
	}

	public static class ExampleObjectExposers
	{
		public static readonly Exposer<ExampleObject, string> Identifier = new Exposer<ExampleObject, string>((obj) => obj.Identifier, nameof(ExampleObject.Identifier));
		public static readonly Exposer<ExampleObject, string> Name = new Exposer<ExampleObject, string>((obj) => obj.Name, nameof(ExampleObject.Name));

		public static class Info
		{
			public static readonly Exposer<ExampleObject, int> IntProperty = new Exposer<ExampleObject, int>((obj) => obj.Info.IntProperty, String.Join(".", nameof(Info), nameof(IntProperty)));
		}

		public static FilterElement<ExampleObject> CreateFilter(string fieldName, Skyline.DataMiner.Net.Messages.SLDataGateway.Comparer comparer, object value)
		{
			switch (fieldName)
			{
				case "Identifier":
					return FilterElementFactory.Create<ExampleObject>(ExampleObjectExposers.Identifier, comparer, (string)value);
				case nameof(ExampleObject.Name):
					return FilterElementFactory.Create<ExampleObject>(Name, comparer, (string)value);
				case "Info.IntProperty":
					return FilterElementFactory.Create<ExampleObject>(Info.IntProperty, comparer, (int)value);
				default:
					throw new NotImplementedException();
			}
		}
	}

	public class ExampleStorageProvider : IRepository<ExampleObject>
	{
		private readonly List<ExampleObject> _objects;

		public ExampleStorageProvider(IEnumerable<ExampleObject> objects)
		{
			_objects = objects.ToList() ?? throw new ArgumentNullException(nameof(objects));
		}

		public IEnumerable<ExampleObject> Read(FilterElement<ExampleObject> filter)
		{
			return Read(filter.ToQuery());
		}

		public IEnumerable<ExampleObject> Read(IQuery<ExampleObject> query)
		{
			var result = query.ExecuteInMemory(_objects);
			return result;
		}

		public long Count(FilterElement<ExampleObject> filter)
		{
			var result = filter.ToQuery().ExecuteInMemory(_objects);
			return result.LongCount();
		}

		public long Count(IQuery<ExampleObject> query)
		{
			var result = query.ExecuteInMemory(_objects);
			return result.LongCount();
		}

		public ExampleObject Create(ExampleObject oToCreate)
		{
			_objects.Add(oToCreate);
			return oToCreate;
		}

		public ExampleObject Update(ExampleObject oToUpdate)
		{
			var index = _objects.FindIndex(o => o.Identifier == oToUpdate.Identifier);
			_objects[index] = oToUpdate;
			return oToUpdate;
		}

		public void Delete(ExampleObject oToDelete)
		{
			var index = _objects.FindIndex(o => o.Identifier == oToDelete.Identifier);
			_objects.RemoveAt(index);
		}

		public IEnumerable<IPagedResult<ExampleObject>> ReadPaged(FilterElement<ExampleObject> filter)
		{
			return ReadPaged(filter, 30);
		}

		public IEnumerable<IPagedResult<ExampleObject>> ReadPaged(IQuery<ExampleObject> query)
		{
			return ReadPaged(query, 30);
		}

		public IEnumerable<IPagedResult<ExampleObject>> ReadPaged(FilterElement<ExampleObject> filter, int pageSize)
		{
			return ReadPaged(filter.ToQuery(), pageSize);
		}

		public IEnumerable<IPagedResult<ExampleObject>> ReadPaged(IQuery<ExampleObject> query, int pageSize)
		{
			var i = 0;
			var enumerator = query.ExecuteInMemory(_objects).Batch(pageSize).GetEnumerator();
			var hasNext = enumerator.MoveNext();
			while (hasNext)
			{
				var page = enumerator.Current;
				hasNext = enumerator.MoveNext();
				yield return new PagedResult<ExampleObject>(page, i, pageSize, hasNext);
			}
		}
	}
}
