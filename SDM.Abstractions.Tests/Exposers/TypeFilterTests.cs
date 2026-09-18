namespace SDM.AbstractionsTests.Exposers
{
	using System;
	using System.Linq;

	using FluentAssertions;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using SDM.AbstractionsTests.Shared;

	using Skyline.DataMiner.SDM;

	using SLDataGateway.API.Querying;

	[TestClass]
	public class TypeFilterTests
	{
		[TestMethod]
		public void TypeFilter_Equal_MatchesOnlyTheExactType()
		{
			// Arrange
			var data = GetTypeData();
			var filter = TestClassExposers.Type.Equal(typeof(string));

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().ContainSingle();
			result.Single().Name.Should().Be("String");
		}

		[TestMethod]
		public void TypeFilter_NotEqual_ExcludesTheExactTypeAndNullFields()
		{
			// Arrange
			var data = GetTypeData();
			var filter = TestClassExposers.Type.NotEqual(typeof(string));

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Select(item => item.Name).Should().BeEquivalentTo("Integer", "Object");
		}

		[TestMethod]
		public void TypeFilter_Equal_Null_MatchesNullFields()
		{
			// Arrange
			var data = GetTypeData();
			var filter = TestClassExposers.Type.Equal(null);

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Should().ContainSingle();
			result.Single().Name.Should().Be("Null");
		}

		[TestMethod]
		public void TypeFilter_NotEqual_Null_ExcludesNullFields()
		{
			// Arrange
			var data = GetTypeData();
			var filter = TestClassExposers.Type.NotEqual(null);

			// Act
			var result = filter.ToQuery().ExecuteInMemory(data).ToArray();

			// Assert
			result.Select(item => item.Name).Should().BeEquivalentTo("String", "Integer", "Object");
		}

		private static TestClass[] GetTypeData()
		{
			return new[]
			{
				new TestClass { Name = "String", Type = typeof(string) },
				new TestClass { Name = "Integer", Type = typeof(int) },
				new TestClass { Name = "Object", Type = typeof(object) },
				new TestClass { Name = "Null", Type = null },
			};
		}
	}
}
