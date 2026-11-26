namespace SDM.AbstractionsTests
{
	using System;
	using System.Collections;
	using System.Linq;

	using FluentAssertions;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using SDM.AbstractionsTests.Shared;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.SDM;

	using SLDataGateway.API.Querying;

	[TestClass]
	public class PartialQueryTests
	{
		private static readonly DynamicListExposer<TestClass, string> DynamicTags =
			DynamicListExposer<TestClass, string>.CreateFromListExposer(
				new Exposer<TestClass, IEnumerable>(item => item.Tags, nameof(TestClass.Tags)));

		[TestMethod]
		public void ExecuteInMemory_AppliesFilterOrderAndLimitAndProjectsSelectedFields()
		{
			// Arrange
			var query = new PartialQuery<TestClass>(new TRUEFilterElement<TestClass>().ToQuery())
				.Include(TestClassExposers.Name)
				.Include(TestClassExposers.Age)
				.Include(TestClassExposers.Tags)
				.WithFilter(TestClassExposers.Age.GreaterThan(25))
				.OrderByDescending(TestClassExposers.Age)
				.Limit(2);

			// Act
			var result = query.ExecuteInMemory(DummyData.GetDummyData()).ToList();

			// Assert
			result.Should().HaveCount(2);
			result.Select(item => item.GetValue(TestClassExposers.Age)).Should().Equal(35, 30);
			result.Should().AllSatisfy(item =>
			{
				item.HasField(TestClassExposers.Name).Should().BeTrue();
				item.HasField(TestClassExposers.Age).Should().BeTrue();
				item.HasField(TestClassExposers.Tags).Should().BeTrue();
				item.HasField(TestClassExposers.NickName).Should().BeFalse();
			});
			result[0].GetValues(TestClassExposers.Tags).Should().Contain(new[] { "tag1", "tag4" });
		}

		[TestMethod]
		public void WithFields_ReplacesSelectedFields_WhileOtherCompositionPreservesThem()
		{
			// Arrange
			var fields = new FieldSelection<TestClass>();
			fields.Add(TestClassExposers.Name);
			var partial = new PartialQuery<TestClass>(new TRUEFilterElement<TestClass>().ToQuery())
				.Include(TestClassExposers.Age);

			// Act
			var result = partial
				.WithFields(fields)
				.WithFilter(TestClassExposers.Age.GreaterThan(25))
				.ExecuteInMemory(DummyData.GetDummyData())
				.ToList();

			// Assert
			result.Should().HaveCount(3);
			result.Should().AllSatisfy(item =>
			{
				item.HasField(TestClassExposers.Name).Should().BeTrue();
				item.HasField(TestClassExposers.Age).Should().BeFalse();
			});
		}

		[TestMethod]
		public void WithQuery_ReplacesUnderlyingQuery_WhilePreservingSelectedFields()
		{
			// Arrange
			var partial = new PartialQuery<TestClass>(new TRUEFilterElement<TestClass>().ToQuery())
				.Include(TestClassExposers.Name);
			var replacementQuery = new TRUEFilterElement<TestClass>()
				.ToQuery()
				.WithFilter(TestClassExposers.Age.GreaterThan(30));

			// Act
			var result = partial
				.WithQuery(replacementQuery)
				.ExecuteInMemory(DummyData.GetDummyData())
				.ToList();

			// Assert
			result.Should().ContainSingle();
			result[0].HasField(TestClassExposers.Name).Should().BeTrue();
			result[0].GetValue(TestClassExposers.Name).Should().Be("Charlie");
		}

		[TestMethod]
		public void ExecuteInMemory_WithNoSelectedFields_ReturnsEmptyPartialObjects()
		{
			var partial = new PartialQuery<TestClass>(new TRUEFilterElement<TestClass>().ToQuery());

			var result = partial.ExecuteInMemory(DummyData.GetDummyData()).ToList();

			result.Should().NotBeEmpty();
			result.Should().AllSatisfy(item =>
			{
				item.HasField(TestClassExposers.Name).Should().BeFalse();
				item.TryGetValue(TestClassExposers.Name, out _).Should().BeFalse();
			});
		}

		[TestMethod]
		public void ConstructorAndExecution_RejectNullArguments()
		{
			// Arrange
			var partial = new PartialQuery<TestClass>(new TRUEFilterElement<TestClass>().ToQuery());

			// Act
			Action createWithNullQuery = () => new PartialQuery<TestClass>(null);
			Action executeWithNullItems = () => partial.ExecuteInMemory(null).ToList();
			Action withNullFields = () => partial.WithFields(null);
			Action withNullQuery = () => partial.WithQuery(null);

			// Assert
			createWithNullQuery.Should().Throw<ArgumentNullException>();
			executeWithNullItems.Should().Throw<ArgumentNullException>();
			withNullFields.Should().Throw<ArgumentNullException>();
			withNullQuery.Should().Throw<ArgumentNullException>();
		}

		[TestMethod]
		public void Include_DeduplicatesFields_AndRejectsNullArguments()
		{
			// Arrange
			var partial = new PartialQuery<TestClass>(new TRUEFilterElement<TestClass>().ToQuery());

			// Act
			var result = partial
				.Include(TestClassExposers.Name)
				.Include(TestClassExposers.Name);
			Action includeOnNull = () => PartialQueryExtensions.Include<TestClass, string>(null, TestClassExposers.Name);
			Action includeNullExposer = () => PartialQueryExtensions.Include<TestClass, string>(
				partial,
				(Exposer<TestClass, string>)null);

			// Assert
			result.Fields.Should().ContainSingle();
			result.Fields.Select(field => field.Exposer).Should().Contain(TestClassExposers.Name);
			includeOnNull.Should().Throw<ArgumentNullException>();
			includeNullExposer.Should().Throw<ArgumentNullException>();
		}

		[TestMethod]
		public void Fields_ReturnsCopy_AndDoesNotMutateTheQuery()
		{
			// Arrange
			var fields = new FieldSelection<TestClass>();
			fields.Add(TestClassExposers.Name);
			var partial = new PartialQuery<TestClass>(new TRUEFilterElement<TestClass>().ToQuery())
				.WithFields(fields);

			// Act
			fields.Add(TestClassExposers.Age);
			var result = partial.ExecuteInMemory(DummyData.GetDummyData()).ToList();
			var updatedResult = partial.WithFields(fields)
				.ExecuteInMemory(DummyData.GetDummyData())
				.ToList();

			// Assert
			result.Should().AllSatisfy(item =>
			{
				item.HasField(TestClassExposers.Name).Should().BeTrue();
				item.HasField(TestClassExposers.Age).Should().BeFalse();
			});
			updatedResult.Should().AllSatisfy(item =>
			{
				item.HasField(TestClassExposers.Name).Should().BeTrue();
				item.HasField(TestClassExposers.Age).Should().BeTrue();
			});
		}

		[TestMethod]
		public void ExecuteInMemory_ProjectsDynamicListExposer()
		{
			// Arrange
			var item = new TestClass { Tags = new System.Collections.Generic.List<string> { "one", "two" } };
			var query = new PartialQuery<TestClass>(new TRUEFilterElement<TestClass>().ToQuery())
				.Include(DynamicTags);

			// Act
			var result = query.ExecuteInMemory(new[] { item }).Single();
			Action includeNullExposer = () => PartialQueryExtensions.Include<TestClass, string>(
				query,
				(DynamicListExposer<TestClass, string>)null);

			// Assert
			query.Fields.Should().ContainSingle();
			result.HasField(DynamicTags).Should().BeTrue();
			result.GetValues(DynamicTags).Should().Equal("one", "two");
			includeNullExposer.Should().Throw<ArgumentNullException>();
		}

		[TestMethod]
		public void ExecuteInMemory_PreservesNullDynamicListExposerValue()
		{
			// Arrange
			var item = new TestClass { Tags = null };
			var query = new PartialQuery<TestClass>(new TRUEFilterElement<TestClass>().ToQuery())
				.Include(DynamicTags);

			// Act
			var result = query.ExecuteInMemory(new[] { item }).Single();

			// Assert
			result.TryGetValues(DynamicTags, out var values).Should().BeTrue();
			values.Should().BeNull();
		}
	}
}
