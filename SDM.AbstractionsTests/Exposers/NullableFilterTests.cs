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
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().NotBeNull();
			result.Should().AllSatisfy(t => t.OptionalAge.Should().NotBeNull());
		}

		[TestMethod]
		public void NullableIntFilter_HasNoValue()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var filter = TestClassExposers.OptionalAge.HasNoValue();

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().NotBeNull();
			result.Should().AllSatisfy(t => t.OptionalAge.Should().BeNull());
		}

		[TestMethod]
		public void StringFilter_HasValue()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var filter = TestClassExposers.NickName.HasValue();

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().NotBeNull();
			result.Should().AllSatisfy(t => t.NickName.Should().NotBeNull());
		}

		[TestMethod]
		public void StringFilter_HasNoValue()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var filter = TestClassExposers.NickName.HasNoValue();

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().NotBeNull();
			result.Should().AllSatisfy(t => t.NickName.Should().BeNull());
		}
	}
}
