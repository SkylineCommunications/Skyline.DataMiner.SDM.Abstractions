namespace SDM.AbstractionsTests
{
	using System;
	using System.Collections.Generic;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using SDM.AbstractionsTests.Middleware;

	using Skyline.DataMiner.SDM;

	[TestClass]
	public class ValidationMiddlewareTests
	{
		[TestMethod]
		public void Middleware_ValidationMiddleware()
		{
			// Arrange
			var repository = Mocked.CreateExampleProvider(new NameMiddleware());

			// Act Create
			var item = new ExampleObject
			{
				Info = new Info
				{
					IntProperty = 5,
				},
			};
			var create = () => repository.Create(item);

			// Assert Create
			Assert.ThrowsException<Exception>(create);
		}
	}

	internal class NameMiddleware : ICreatableMiddleware<ExampleObject>
	{
		public ExampleObject OnCreate(ExampleObject oToCreate, Func<ExampleObject, ExampleObject> next)
		{
			if (String.IsNullOrEmpty(oToCreate?.Name))
			{
				throw new Exception("Name cannot be empty");
			}

			return next(oToCreate);
		}
	}
}