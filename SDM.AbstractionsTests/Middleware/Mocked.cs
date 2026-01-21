namespace SDM.AbstractionsTests.Middleware
{
	using System.Collections.Generic;

	using Skyline.DataMiner.SDM;
	using Skyline.DataMiner.SDM.Middleware;

	internal static class Mocked
	{
		internal static IRepository<ExampleObject> CreateExampleProvider(params IMiddlewareMarker<ExampleObject>[] middlewares)
		{
			IRepository<ExampleObject> repository = new ExampleStorageProvider(new List<ExampleObject>
			{
				new ExampleObject
				{
					Name = "Test1",
				},
				new ExampleObject
				{
					Name = "Test2",
				},
				new ExampleObject
				{
					Name = "Test3",
				},
				new ExampleObject
				{
					Name = "Test4",
				},
				new ExampleObject
				{
					Name = "Test5",
				},
				new ExampleObject
				{
					Name = "SECURITY: Test6",
				},
			});

			foreach (var middleware in middlewares)
			{
				repository = new MiddlewareRepository<ExampleObject>(
					repository,
					middleware);
			}

			return repository;
		}
	}
}
