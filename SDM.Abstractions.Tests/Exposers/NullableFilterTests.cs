namespace SDM.AbstractionsTests.Exposers
{
	using System.Linq;

	using FluentAssertions;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using SDM.AbstractionsTests.Shared;

	using Skyline.DataMiner.SDM;

	using SLDataGateway.API.Querying;

	[TestClass]
	public class NullableFilterTests
	{
		[TestMethod]
		public void NullableIntFilter_HasValue()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var filter = TestClassExposers.OptionalAge.HasValue();

			// Act
			var linqResult = data.Where(t => t.OptionalAge.HasValue).ToArray();
			var filterResult = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			linqResult.Should().Equal(filterResult);
			filterResult.Should().NotBeNull();
			filterResult.Should().AllSatisfy(t => t.OptionalAge.Should().NotBeNull());
		}

		[TestMethod]
		public void NullableIntFilter_HasNoValue()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var filter = TestClassExposers.OptionalAge.HasNoValue();

			// Act
			var linqResult = data.Where(t => !t.OptionalAge.HasValue).ToArray();
			var filterResult = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			linqResult.Should().Equal(filterResult);
			filterResult.Should().NotBeNull();
			filterResult.Should().AllSatisfy(t => t.OptionalAge.Should().BeNull());
		}

		[TestMethod]
		public void StringFilter_HasValue()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var filter = TestClassExposers.NickName.HasValue();

			// Act
			var linqResult = data.Where(t => t.NickName != null).ToArray();
			var filterResult = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			linqResult.Should().Equal(filterResult);
			filterResult.Should().NotBeNull();
			filterResult.Should().AllSatisfy(t => t.NickName.Should().NotBeNull());
		}

		[TestMethod]
		public void StringFilter_HasNoValue()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var filter = TestClassExposers.NickName.HasNoValue();

			// Act
			var linqResult = data.Where(t => t.NickName == null).ToArray();
			var filterResult = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			linqResult.Should().Equal(filterResult);
			filterResult.Should().NotBeNull();
			filterResult.Should().AllSatisfy(t => t.NickName.Should().BeNull());
		}

		[TestMethod]
		public void NullableIntFilter_LessThan()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var filter = TestClassExposers.OptionalAge.LessThan(30);

			// Act
			var linqResult = data.Where(t => t.OptionalAge < 30).ToArray();
			var filterResult = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			linqResult.Should().Equal(filterResult);
			filterResult.Should().NotBeNull();
			filterResult.Should().HaveCount(1);
			filterResult.Should().AllSatisfy(t => t.OptionalAge.Should().BeLessThan(30));
		}

		[TestMethod]
		public void NullableIntFilter_LessThanOrEqual()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var filter = TestClassExposers.OptionalAge.LessThanOrEqual(25);

			// Act
			var linqResult = data.Where(t => t.OptionalAge <= 25).ToArray();
			var filterResult = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			linqResult.Should().Equal(filterResult);
			filterResult.Should().NotBeNull();
			filterResult.Should().HaveCount(1);
			filterResult.Should().AllSatisfy(t => t.OptionalAge.Should().BeLessThanOrEqualTo(25));
		}

		[TestMethod]
		public void NullableIntFilter_GreaterThan()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var filter = TestClassExposers.OptionalAge.GreaterThan(25);

			// Act
			var linqResult = data.Where(t => t.OptionalAge > 25).ToArray();
			var filterResult = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			linqResult.Should().Equal(filterResult);
			filterResult.Should().NotBeNull();
			filterResult.Should().HaveCount(1);
			filterResult.Should().AllSatisfy(t => t.OptionalAge.Should().BeGreaterThan(25));
		}

		[TestMethod]
		public void NullableIntFilter_GreaterThanOrEqual()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var filter = TestClassExposers.OptionalAge.GreaterThanOrEqual(35);

			// Act
			var linqResult = data.Where(t => t.OptionalAge >= 35).ToArray();
			var filterResult = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			linqResult.Should().Equal(filterResult);
			filterResult.Should().NotBeNull();
			filterResult.Should().HaveCount(1);
			filterResult.Should().AllSatisfy(t => t.OptionalAge.Should().BeGreaterThanOrEqualTo(35));
		}
	}
}
