namespace SDM.AbstractionsTests.Shared
{
	using System;
	using System.Collections.Generic;

	public static class DummyData
	{
		public static ICollection<TestClass> GetDummyData()
		{
			var first = new TestClass
			{
				Identifier = "efc9bcfe-c111-40ba-8102-29185502a833",
				Age = 25,
				CreatedAt = new DateTime(2020, 1, 1),
				IsActive = true,
				Name = "Alice",
				Range = TimeSpan.FromHours(1),
				Rating = 4.5f,
				Score = 95.5m,
				Status = Status.Active,
				Tags = new List<string> { "tag1", "tag2" },
				Statuses = new List<Status> { Status.Active, Status.Draft },
				Sub = new SubClass { Description = "First", Size = 10, Guid = new Guid("0ef82ddf-357c-41fe-bdc6-5ecf217641eb") },
				SubClasses = new List<SubClass>
				{
					new SubClass { Description = "SubFirst-1", Size = 5, Guid = new Guid("1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d") },
					new SubClass { Description = "SubFirst-2", Size = 8, Guid = new Guid("2b3c4d5e-6f7a-8b9c-0d1e-2f3a4b5c6d7e") },
				},
			};

			return new[]
			{
				first,
				new TestClass
				{
					Identifier = "6c4e9203-cdc9-4355-b428-d2f56627f68e",
					Age = 30,
					CreatedAt = new DateTime(2019, 6, 15),
					IsActive = false,
					Name = "Bob",
					Range = TimeSpan.FromHours(2),
					Rating = 3.8f,
					Score = 88.0m,
					Status = Status.Draft,
					Tags = new List<string> { "tag3" },
					Statuses = new List<Status> { Status.Draft },
					Sub = new SubClass { Description = "Second", Size = 20, Guid = new Guid("ef3c5b2d-2d1c-4d9c-bf94-0ab9eb2cc3e7") },
					SubClasses = new List<SubClass>
					{
						new SubClass { Description = "SubSecond-1", Size = 12, Guid = new Guid("3c4d5e6f-7a8b-9c0d-1e2f-3a4b5c6d7e8f") },
						new SubClass { Description = "SubSecond-2", Size = 18, Reference = first, Guid = new Guid("4d5e6f7a-8b9c-0d1e-2f3a-4b5c6d7e8f9a") },
						new SubClass { Description = "SubSecond-3", Size = 22, Guid = new Guid("5e6f7a8b-9c0d-1e2f-3a4b-5c6d7e8f9a0b") },
					},
				},
				new TestClass
				{
					Identifier = "5a6289e3-c8bf-4c97-af44-11e040d5001d",
					Age = 35,
					CreatedAt = new DateTime(2018, 3, 10),
					IsActive = true,
					Name = "Charlie",
					Range = TimeSpan.FromHours(3),
					Rating = 4.2f,
					Score = 92.3m,
					Status = Status.Deprecated,
					Tags = new List<string> { "tag1", "tag4" },
					Statuses = new List<Status> { Status.Active, Status.Deprecated },
					Sub = new SubClass { Description = "Third", Size = 15, Reference = first, Guid = new Guid("51ab8ecb-cfa5-48f8-b7ac-653d693cd582") },
					SubClasses = new List<SubClass>
					{
						new SubClass { Description = "SubThird-1", Size = 7, Guid = new Guid("6f7a8b9c-0d1e-2f3a-4b5c-6d7e8f9a0b1c") },
					},
				},
				new TestClass
				{
					Identifier = "ca0753d8-4407-4db5-a33f-6adab1de13e4",
					Age = 28,
					CreatedAt = new DateTime(2021, 11, 5),
					IsActive = false,
					Name = "Diana",
					Range = TimeSpan.FromHours(1.5),
					Rating = 4.9f,
					Score = 97.7m,
					SubClasses = new List<SubClass>
					{
						new SubClass { Description = "SubFourth-1", Size = 30, Guid = new Guid("7a8b9c0d-1e2f-3a4b-5c6d-7e8f9a0b1c2d") },
						new SubClass { Description = "SubFourth-2", Size = 28, Guid = new Guid("8b9c0d1e-2f3a-4b5c-6d7e-8f9a0b1c2d3e") },
						new SubClass { Description = "SubFourth-3", Size = 35, Reference = first, Guid = new Guid("9c0d1e2f-3a4b-5c6d-7e8f-9a0b1c2d3e4f") },
						new SubClass { Description = "SubFourth-4", Size = 20, Guid = new Guid("0d1e2f3a-4b5c-6d7e-8f9a-0b1c2d3e4f5a") },
					},
					Status = Status.Deleted,
					Tags = new List<string> { "tag2", "tag5" },
					Statuses = new List<Status> { Status.Deleted },
					Sub = new SubClass { Description = "Fourth", Size = 25, Guid = new Guid("ee8a75a9-2962-4e9f-acfe-257d23270fb2") },
				},
			};
		}
	}
}
