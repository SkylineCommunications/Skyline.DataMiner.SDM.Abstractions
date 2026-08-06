namespace SDM.AbstractionsTests.FilterFactory
{
	using System;
	using System.Linq;

	using FluentAssertions;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using SDM.AbstractionsTests.Shared;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.SDM;

	using SLDataGateway.API.Querying;

	[TestClass]
	public class FilterElementFactoryTests
	{
		[TestMethod]
		public void Create_StringFilter_ReturnsMatchingResults()
		{
			AssertFilter(TestClassExposers.Name, Comparer.Equals, "Alice", t => t.Name == "Alice");
		}

		[TestMethod]
		public void Create_StringFilter_Contains_ReturnsMatchingResults()
		{
			AssertFilter(TestClassExposers.Name, Comparer.Contains, "lic", t => t.Name.Contains("lic"));
		}

		[TestMethod]
		public void Create_StringFilter_NotContains_ReturnsMatchingResults()
		{
			AssertFilter(TestClassExposers.Name, Comparer.NotContains, "lic", t => !t.Name.Contains("lic"));
		}

		[TestMethod]
		public void Create_StringFilter_Regex_ReturnsMatchingResults()
		{
			AssertFilter(TestClassExposers.Name, Comparer.Regex, "^A", t => t.Name.StartsWith("A"));
		}

		[TestMethod]
		public void Create_StringFilter_NotRegex_ReturnsMatchingResults()
		{
			AssertFilter(TestClassExposers.Name, Comparer.NotRegex, "^A", t => !t.Name.StartsWith("A"));
		}

		[TestMethod]
		public void Create_BoolFilter_ReturnsMatchingResults()
		{
			AssertFilter(TestClassExposers.IsActive, Comparer.Equals, true, t => t.IsActive);
		}

		[TestMethod]
		public void Create_BoolFilter_NotEqual_ReturnsMatchingResults()
		{
			AssertFilter(TestClassExposers.IsActive, Comparer.NotEquals, true, t => !t.IsActive);
		}

		[TestMethod]
		public void Create_SByteFilter_ReturnsMatchingResults()
		{
			AssertFilter(TestClassExposers.SignedByte, Comparer.Equals, (sbyte)-5, t => t.SignedByte == -5);
		}

		[TestMethod]
		public void Create_ByteFilter_ReturnsMatchingResults()
		{
			AssertFilter(TestClassExposers.UnsignedByte, Comparer.Equals, (byte)1, t => t.UnsignedByte == 1);
		}

		[TestMethod]
		public void Create_ShortFilter_ReturnsMatchingResults()
		{
			AssertFilter(TestClassExposers.ShortNumber, Comparer.Equals, (short)10, t => t.ShortNumber == 10);
		}

		[TestMethod]
		public void Create_UShortFilter_ReturnsMatchingResults()
		{
			AssertFilter(TestClassExposers.UnsignedShort, Comparer.Equals, (ushort)100, t => t.UnsignedShort == 100);
		}

		[TestMethod]
		public void Create_IntFilter_ReturnsMatchingResults()
		{
			AssertFilter(TestClassExposers.Age, Comparer.Equals, 25, t => t.Age == 25);
		}

		[TestMethod]
		public void Create_IntFilter_GreaterThan_ReturnsMatchingResults()
		{
			AssertFilter(TestClassExposers.Age, Comparer.GT, 25, t => t.Age > 25);
		}

		[TestMethod]
		public void Create_IntFilter_LessThan_ReturnsMatchingResults()
		{
			AssertFilter(TestClassExposers.Age, Comparer.LT, 35, t => t.Age < 35);
		}

		[TestMethod]
		public void Create_UIntFilter_ReturnsMatchingResults()
		{
			AssertFilter(TestClassExposers.UnsignedInt, Comparer.Equals, 1000u, t => t.UnsignedInt == 1000u);
		}

		[TestMethod]
		public void Create_LongFilter_ReturnsMatchingResults()
		{
			AssertFilter(TestClassExposers.LongNumber, Comparer.Equals, 10000L, t => t.LongNumber == 10000L);
		}

		[TestMethod]
		public void Create_ULongFilter_ReturnsMatchingResults()
		{
			AssertFilter(TestClassExposers.UnsignedLong, Comparer.Equals, 100000UL, t => t.UnsignedLong == 100000UL);
		}

		[TestMethod]
		public void Create_FloatFilter_ReturnsMatchingResults()
		{
			AssertFilter(TestClassExposers.Rating, Comparer.Equals, 4.5f, t => t.Rating == 4.5f);
		}

		[TestMethod]
		public void Create_DoubleFilter_ReturnsMatchingResults()
		{
			AssertFilter(TestClassExposers.Sub.Size, Comparer.Equals, 10.0, t => t.Sub.Size == 10.0);
		}

		[TestMethod]
		public void Create_DecimalFilter_ReturnsMatchingResults()
		{
			AssertFilter(TestClassExposers.Score, Comparer.Equals, 95.5m, t => t.Score == 95.5m);
		}

		[TestMethod]
		public void Create_DateTimeFilter_ReturnsMatchingResults()
		{
			AssertFilter(TestClassExposers.CreatedAt, Comparer.Equals, new DateTime(2020, 1, 1), t => t.CreatedAt == new DateTime(2020, 1, 1));
		}

		[TestMethod]
		public void Create_DateTimeFilter_LessThan_ReturnsMatchingResults()
		{
			AssertFilter(TestClassExposers.CreatedAt, Comparer.LT, new DateTime(2020, 1, 1), t => t.CreatedAt < new DateTime(2020, 1, 1));
		}

		[TestMethod]
		public void Create_TimeSpanFilter_ReturnsMatchingResults()
		{
			AssertFilter(TestClassExposers.Range, Comparer.Equals, TimeSpan.FromHours(1), t => t.Range == TimeSpan.FromHours(1));
		}

		[TestMethod]
		public void Create_EnumFilter_ReturnsMatchingResults()
		{
			AssertFilter(TestClassExposers.Status, Comparer.Equals, Status.Active, t => t.Status == Status.Active);
		}

		[TestMethod]
		public void Create_EnumFilter_NotEqual_ReturnsMatchingResults()
		{
			AssertFilter(TestClassExposers.Status, Comparer.NotEquals, Status.Active, t => t.Status != Status.Active);
		}

		[TestMethod]
		public void Create_GuidFilter_ReturnsMatchingResults()
		{
			AssertFilter(TestClassExposers.Sub.Guid, Comparer.Equals, new Guid("0ef82ddf-357c-41fe-bdc6-5ecf217641eb"), t => t.Sub.Guid == new Guid("0ef82ddf-357c-41fe-bdc6-5ecf217641eb"));
		}

		[TestMethod]
		public void Create_SdmObjectReferenceFilter_ReturnsMatchingResults()
		{
			var data = DummyData.GetDummyData();
			var referenceTarget = data.First();
			var filter = FilterElementFactory.Create<TestClass>(TestClassExposers.Sub.Reference, Comparer.Equals, referenceTarget);

			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			result.Should().Equal(data.Where(t => t.Sub.Reference == referenceTarget));
			result.Should().NotBeNull();
			result.Should().AllSatisfy(t => t.Sub.Reference.Identifier.Should().Be(referenceTarget.Identifier));
		}

		[TestMethod]
		public void Create_SdmObjectReferenceFilter_NotEqual_ReturnsMatchingResults()
		{
			var data = DummyData.GetDummyData();
			var referenceTarget = data.First();
			var filter = FilterElementFactory.Create<TestClass>(TestClassExposers.Sub.Reference, Comparer.NotEquals, referenceTarget);

			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			result.Should().Equal(data.Where(t => t.Sub.Reference != referenceTarget));
			result.Should().NotBeNull();
			result.Should().AllSatisfy(t => t.Sub.Reference.Identifier.Should().NotBe(referenceTarget.Identifier));
		}

		[TestMethod]
		public void Create_NullableBoolFilter_HasValue_ReturnsMatchingResults()
		{
			AssertFilter(TestClassExposers.OptionalIsActive, Comparer.Equals, true, t => t.OptionalIsActive == true);
		}

		[TestMethod]
		public void Create_NullableBoolFilter_HasNoValue_ReturnsMatchingResults()
		{
			var data = DummyData.GetDummyData();
			var filter = FilterElementFactory.Create<TestClass>(TestClassExposers.OptionalIsActive, Comparer.Equals, null);

			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			result.Should().Equal(data.Where(t => t.OptionalIsActive == null));
			result.Should().NotBeNull();
			result.Should().AllSatisfy(t => t.OptionalIsActive.Should().BeNull());
		}

		[TestMethod]
		public void Create_NullableIntFilter_ReturnsMatchingResults()
		{
			AssertFilter(TestClassExposers.OptionalAge, Comparer.Equals, 35, t => t.OptionalAge == 35);
		}

		[TestMethod]
		public void Create_GuidCollectionFilter_ReturnsMatchingResults()
		{
			AssertCollectionFilter(TestClassExposers.SubClasses.Guid, Comparer.Contains, new Guid("1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d"), t => t.SubClasses.Any(x => x.Guid == new Guid("1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d")));
		}

		[TestMethod]
		public void Create_SdmObjectReferenceCollectionFilter_ReturnsMatchingResults_SdmObjectReference()
		{
			AssertCollectionFilter(TestClassExposers.SubClasses.Reference, Comparer.Contains, new SdmObjectReference<TestClass>("1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d"), t => t.SubClasses.Any(x => x.Reference == "1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d"));
		}

		[TestMethod]
		public void Create_SdmObjectReferenceCollectionFilter_ReturnsMatchingResults_String()
		{
			AssertCollectionFilter(TestClassExposers.SubClasses.Reference, Comparer.Contains, "1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d", t => t.SubClasses.Any(x => x.Reference == "1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d"));
		}

		[TestMethod]
		public void Create_SdmObjectReferenceCollectionFilter_ReturnsMatchingResults_SdmObject()
		{
			var testClass = new TestClass { Identifier = "7a8b9c0d-1e2f-3a4b-5c6d-7e8f9a0b1c2d" };
			AssertCollectionFilter(TestClassExposers.SubClasses.Reference, Comparer.Contains, testClass, t => t.SubClasses.Any(x => x.Reference == testClass.Identifier));
		}

		[TestMethod]
		public void Create_StringCollectionFilter_NotContains_ReturnsMatchingResults()
		{
			AssertCollectionFilter(TestClassExposers.Tags, Comparer.NotContains, "tag1", t => !t.Tags.Contains("tag1"));
		}

		[TestMethod]
		public void Create_EnumCollectionFilter_NotContains_ReturnsMatchingResults()
		{
			AssertCollectionFilter(TestClassExposers.Statuses, Comparer.NotContains, Status.Active, t => !t.Statuses.Contains(Status.Active));
		}

		[TestMethod]
		public void Create_NullableStringFilter_ReturnsMatchingResults()
		{
			AssertFilter(TestClassExposers.NickName, Comparer.Equals, "Ali", t => t.NickName == "Ali");
		}

		[TestMethod]
		public void Create_StringCollectionFilter_ReturnsMatchingResults()
		{
			AssertCollectionFilter(TestClassExposers.Tags, Comparer.Contains, "tag1", t => t.Tags.Contains("tag1"));
		}

		[TestMethod]
		public void Create_EnumCollectionFilter_ReturnsMatchingResults()
		{
			AssertCollectionFilter(TestClassExposers.Statuses, Comparer.Contains, Status.Active, t => t.Statuses.Contains(Status.Active));
		}

		private static void AssertFilter(FieldExposer exposer, Comparer comparer, object value, Func<TestClass, bool> predicate)
		{
			var data = DummyData.GetDummyData();
			var filter = FilterElementFactory.Create<TestClass>(exposer, comparer, value);

			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			result.Should().Equal(data.Where(predicate));
			result.Should().NotBeNull();
		}

		private static void AssertCollectionFilter(FieldExposer exposer, Comparer comparer, object value, Func<TestClass, bool> predicate)
		{
			var data = DummyData.GetDummyData();
			var filter = FilterElementFactory.Create<TestClass>(exposer, comparer, value);

			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			result.Should().Equal(data.Where(predicate));
			result.Should().NotBeNull();
		}
	}
}
