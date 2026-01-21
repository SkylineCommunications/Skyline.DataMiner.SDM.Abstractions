namespace SDM.AbstractionsTests.Exposers
{
	using System;
	using System.Linq;

	using FluentAssertions;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.SDM;

	using SLDataGateway.API.Querying;

	using SDM.AbstractionsTests.Shared;

	[TestClass]
	public class BasicExposerTypeTests
	{
		[TestMethod]
		public void StringFilter_Equal()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var filter = TestClassExposers.Name.Equal("Alice");

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().NotBeNull();
			result.Should().HaveCount(1);
			result.First().Name.Should().Be("Alice");
		}

		[TestMethod]
		public void StringFilter_NotEqual()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var filter = TestClassExposers.Name.NotEqual("Alice");

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().NotBeNull();
			result.Should().AllSatisfy(t => t.Name.Should().NotBe("Alice"));
		}

		[TestMethod]
		public void StringFilter_Contains()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var filter = TestClassExposers.Name.Contains("ice");

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().NotBeNull();
			result.Should().HaveCount(1);
			result.First().Name.Should().Contain("ice");
		}

		[TestMethod]
		public void StringFilter_NotContains()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var filter = TestClassExposers.Name.NotContains("ice");

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().NotBeNull();
			result.Should().AllSatisfy(t => t.Name.Should().NotContain("ice"));
		}

		[TestMethod]
		public void StringFilter_Regex()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var filter = TestClassExposers.Name.Matches("^A"); // matches strings that start with A

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().NotBeNull();
			result.Should().AllSatisfy(t => t.Name.Should().StartWith("A"));
		}

		[TestMethod]
		public void StringFilter_NotRegex()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var filter = TestClassExposers.Name.NotMatches("^A"); // regex for strings that start with A

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().NotBeNull();
			result.Should().AllSatisfy(t => t.Name.Should().NotStartWith("A"));
		}

		[TestMethod]
		public void IntFilter_Equal()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var filter = TestClassExposers.Age.Equal(25);

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().NotBeNull();
			result.Should().HaveCount(1);
			result.First().Age.Should().Be(25);
		}

		[TestMethod]
		public void IntFilter_NotEqual()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var filter = TestClassExposers.Age.NotEqual(25);

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().NotBeNull();
			result.Should().AllSatisfy(t => t.Age.Should().NotBe(25));
		}

		[TestMethod]
		public void IntFilter_GreaterThan()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var filter = TestClassExposers.Age.GreaterThan(25);

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().NotBeNull();
			result.Should().AllSatisfy(t => t.Age.Should().BeGreaterThan(25));
		}

		[TestMethod]
		public void IntFilter_GreaterThanOrEqual()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var filter = TestClassExposers.Age.GreaterThan(25);

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().NotBeNull();
			result.Should().AllSatisfy(t => t.Age.Should().BeGreaterThanOrEqualTo(25));
		}

		[TestMethod]
		public void IntFilter_LessThan()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var filter = TestClassExposers.Age.LessThan(30);

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().NotBeNull();
			result.Should().AllSatisfy(t => t.Age.Should().BeLessThan(30));
		}

		[TestMethod]
		public void IntFilter_LessThanOrEqual()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var filter = TestClassExposers.Age.LessThanOrEqual(30);

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().NotBeNull();
			result.Should().AllSatisfy(t => t.Age.Should().BeLessThanOrEqualTo(30));
		}

		[TestMethod]
		public void EnumFilter_Equal()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var filter = TestClassExposers.Status.Equal(Status.Active);

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().NotBeNull();
			result.Should().AllSatisfy(t => t.Status.Should().Be(Status.Active));
		}

		[TestMethod]
		public void EnumFilter_NotEqual()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var filter = TestClassExposers.Status.NotEqual(Status.Active);

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().NotBeNull();
			result.Should().AllSatisfy(t => t.Status.Should().NotBe(Status.Active));
		}

		[TestMethod]
		public void EnumFilter_LessThan()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var filter = TestClassExposers.Status.LessThan(Status.Active);

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().NotBeNull();
			result.Should().AllSatisfy(t => ((int)t.Status).Should().BeLessThan((int)Status.Active));
		}

		[TestMethod]
		public void EnumFilter_LessThanOrEqual()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var filter = TestClassExposers.Status.LessThanOrEqual(Status.Active);

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().NotBeNull();
			result.Should().AllSatisfy(t => ((int)t.Status).Should().BeLessThanOrEqualTo((int)Status.Active));
		}

		[TestMethod]
		public void EnumFilter_GreaterThan()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var filter = TestClassExposers.Status.GreaterThan(Status.Active);

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().NotBeNull();
			result.Should().AllSatisfy(t => ((int)t.Status).Should().BeGreaterThan((int)Status.Active));
		}

		[TestMethod]
		public void EnumFilter_GreaterThanOrEqual()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var filter = TestClassExposers.Status.GreaterThanOrEqual(Status.Active);

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().NotBeNull();
			result.Should().AllSatisfy(t => ((int)t.Status).Should().BeGreaterThanOrEqualTo((int)Status.Active));
		}

		[TestMethod]
		public void DateTimeFilter_Equal()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var date = new DateTime(2018, 3, 10);
			var filter = TestClassExposers.CreatedAt.Equal(date);

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().NotBeNull();
			result.Should().AllSatisfy(t => t.CreatedAt.Should().Be(date));
		}

		[TestMethod]
		public void DateTimeFilter_NotEqual()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var date = new DateTime(2018, 3, 10);
			var filter = TestClassExposers.CreatedAt.NotEqual(date);

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().NotBeNull();
			result.Should().AllSatisfy(t => t.CreatedAt.Should().NotBe(date));
		}

		[TestMethod]
		public void DateTimeFilter_LessThan()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var date = new DateTime(2019, 12, 31);
			var filter = TestClassExposers.CreatedAt.LessThan(date);

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().NotBeNull();
			result.Should().AllSatisfy(t => t.CreatedAt.Should().BeBefore(date));
		}

		[TestMethod]
		public void DateTimeFilter_LessThanOrEqual()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var date = new DateTime(2019, 12, 31);
			var filter = TestClassExposers.CreatedAt.LessThanOrEqual(date);

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().NotBeNull();
			result.Should().AllSatisfy(t => t.CreatedAt.Should().BeOnOrBefore(date));
		}

		[TestMethod]
		public void DateTimeFilter_GreaterThan()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var date = new DateTime(2019, 12, 31);
			var filter = TestClassExposers.CreatedAt.GreaterThan(date);

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().NotBeNull();
			result.Should().AllSatisfy(t => t.CreatedAt.Should().BeAfter(date));
		}

		[TestMethod]
		public void DateTimeFilter_GreaterThanOrEqual()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var date = new DateTime(2019, 12, 31);
			var filter = TestClassExposers.CreatedAt.GreaterThanOrEqual(date);

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().NotBeNull();
			result.Should().AllSatisfy(t => t.CreatedAt.Should().BeOnOrAfter(date));
		}

		[TestMethod]
		public void TimeSpanFilter_Equal()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var span = TimeSpan.FromHours(1.5);
			var filter = TestClassExposers.Range.Equal(span);

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().NotBeNull();
			result.Should().AllSatisfy(t => t.Range.Should().Be(span));
		}

		[TestMethod]
		public void TimeSpanFilter_NotEqual()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var span = TimeSpan.FromHours(1.5);
			var filter = TestClassExposers.Range.NotEqual(span);

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().NotBeNull();
			result.Should().AllSatisfy(t => t.Range.Should().NotBe(span));
		}

		[TestMethod]
		public void TimeSpanFilter_LessThan()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var span = TimeSpan.FromHours(1.5);
			var filter = TestClassExposers.Range.LessThan(span);

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().NotBeNull();
			result.Should().AllSatisfy(t => t.Range.Should().BeLessThan(span));
		}

		[TestMethod]
		public void TimeSpanFilter_LessThanOrEqual()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var span = TimeSpan.FromHours(1.5);
			var filter = TestClassExposers.Range.LessThanOrEqual(span);

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().NotBeNull();
			result.Should().AllSatisfy(t => t.Range.Should().BeLessThanOrEqualTo(span));
		}

		[TestMethod]
		public void TimeSpanFilter_GreaterThan()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var span = TimeSpan.FromHours(1.5);
			var filter = TestClassExposers.Range.GreaterThan(span);

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().NotBeNull();
			result.Should().AllSatisfy(t => t.Range.Should().BeGreaterThan(span));
		}

		[TestMethod]
		public void TimeSpanFilter_GreaterThanOrEqual()
		{
			// Arrange
			var data = DummyData.GetDummyData();
			var span = TimeSpan.FromHours(1.5);
			var filter = TestClassExposers.Range.GreaterThanOrEqual(span);

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().NotBeNull();
			result.Should().AllSatisfy(t => t.Range.Should().BeGreaterThanOrEqualTo(span));
		}
	}
}
