namespace SDM.AbstractionsTests.Middleware
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.SDM;
	using Skyline.DataMiner.SDM.Middleware;

	using SLDataGateway.API.Querying;
	using SLDataGateway.API.Types.Querying;

	[TestClass]
	public class SecurityMiddlewareTest
	{
		[TestMethod]
		public void Middleware_SecurityMiddleware()
		{
			// Arrange
			var middleware = new SecurityMiddleware(false);
			var repository = Mocked.CreateExampleProvider(middleware);

			var noPermissionCount = repository.Read(new TRUEFilterElement<ExampleObject>().ToQuery()).ToList();
			Assert.AreEqual(5, noPermissionCount.Count);

			middleware.HasPermission = true;
			var permissionCount = repository.Read(new TRUEFilterElement<ExampleObject>().ToQuery()).ToList();
			Assert.AreEqual(6, permissionCount.Count);
		}
	}

	internal class SecurityMiddleware : IReadableMiddleware<ExampleObject>
	{
		public SecurityMiddleware(bool hasPermission)
		{
			HasPermission = hasPermission;
		}

		public bool HasPermission { get; set; }

		public IEnumerable<ExampleObject> OnRead(FilterElement<ExampleObject> filter, Func<FilterElement<ExampleObject>, IEnumerable<ExampleObject>> next)
		{
			if(!HasPermission)
			{
				return next(filter.AND(ExampleObjectExposers.Name.Contains("SECURITY: ")));
			}

			return next(filter);
		}

		public IEnumerable<ExampleObject> OnRead(IQuery<ExampleObject> query, Func<IQuery<ExampleObject>, IEnumerable<ExampleObject>> next)
		{
			if (!HasPermission)
			{
				return next(query.WithFilter(query.Filter.AND(ExampleObjectExposers.Name.NotContains("SECURITY: "))));
			}

			return next(query);
		}
	}
}
