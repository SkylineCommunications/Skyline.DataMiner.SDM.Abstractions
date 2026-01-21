namespace SDM.AbstractionsTests.Exposers
{
	using System;
	using System.Linq;

	using FluentAssertions;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using SDM.AbstractionsTests.Shared;

	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Net.Sections;
	using Skyline.DataMiner.SDM;

	using SLDataGateway.API.Querying;

	[TestClass]
	public class CollectionExposerTypeTests
	{
		[TestMethod]
		public void StringFilter_Contains()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var filter = TestClassExposers.Tags.Contains("tag1");

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().NotBeNull();
			result.Should().AllSatisfy(t => t.Tags.Should().Contain("tag1"));
		}

		[TestMethod]
		public void StringFilter_NotContains()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var filter = TestClassExposers.Tags.NotContains("tag1");

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().NotBeNull();
			result.Should().AllSatisfy(t => t.Tags.Should().NotContain("tag1"));
		}

		[TestMethod]
		public void EnumFilter_Contains()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var filter = TestClassExposers.Statuses.Contains(Status.Active);

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().NotBeNull();
			result.Should().AllSatisfy(t => t.Statuses.Should().Contain(Status.Active));
		}

		[TestMethod]
		public void EnumFilter_NotContains()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var filter = TestClassExposers.Statuses.NotContains(Status.Active);

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().NotBeNull();
			result.Should().AllSatisfy(t => t.Statuses.Should().NotContain(Status.Active));
		}

		[TestMethod]
		public void DomInstanceFieldFilter_NotContains()
		{
			// Arrange
			var exposer = DomInstanceExposers.FieldValues.DomInstanceField(new FieldDescriptorID());
			var temp1 = exposer.Contains("ABC");

			// Act
			var filter = default(FilterElement<DomInstance>);
			var act = () => filter = FilterElementFactory.Create<DomInstance>(exposer, Comparer.NotContains, "ABC");

			// Assert
			act.Should().NotThrow<Exception>();
			filter.Should().NotBeNull();
		}
	}
}
