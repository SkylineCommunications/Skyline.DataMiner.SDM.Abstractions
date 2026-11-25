namespace SDM.AbstractionsTests
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using FluentAssertions;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.SDM;
	using Skyline.DataMiner.SDM.Middleware;

	using SLDataGateway.API.Querying;

	[TestClass]
	public class PartialMiddlewareRepositoryTests
	{
		[TestMethod]
		public void ReadPartial_DelegatesToRepositoryAndMiddleware()
		{
			// Arrange
			var repository = new PartialRepository();
			var middleware = new PartialMiddleware();
			var wrapped = new MiddlewareRepository<ExampleObject>(repository, middleware);
			var query = CreateQuery();

			// Act
			var result = wrapped.ReadPartial(query).ToList();

			// Assert
			result.Should().ContainSingle();
			repository.ReadPartialCalls.Should().Be(1);
			repository.LastQuery.Should().BeSameAs(query);
			middleware.ReadPartialCalls.Should().Be(1);
			middleware.LastQuery.Should().BeSameAs(query);
		}

		[TestMethod]
		public void ReadPartialPaged_DelegatesBothOverloadsAndForwardsPageSize()
		{
			// Arrange
			var repository = new PartialRepository();
			var middleware = new PartialMiddleware();
			var wrapped = new MiddlewareRepository<ExampleObject>(repository, middleware);
			var query = CreateQuery();

			// Act
			var resultWithoutPageSize = wrapped.ReadPartialPaged(query).ToList();
			var resultWithPageSize = wrapped.ReadPartialPaged(query, 7).ToList();

			// Assert
			resultWithoutPageSize.Should().ContainSingle();
			resultWithPageSize.Should().ContainSingle();
			repository.ReadPartialPagedCalls.Should().Be(2);
			repository.LastPageSize.Should().Be(7);
			middleware.ReadPartialPagedCalls.Should().Be(1);
			middleware.ReadPartialPagedWithPageSizeCalls.Should().Be(1);
			middleware.LastPageSize.Should().Be(7);
		}

		[TestMethod]
		public void PartialOperations_ThrowWhenRepositoryDoesNotSupportThem()
		{
			// Arrange
			var wrapped = new MiddlewareRepository<ExampleObject>(new MarkerRepository());
			var query = CreateQuery();

			// Act
			Action read = () => wrapped.ReadPartial(query).ToList();
			Action readPaged = () => wrapped.ReadPartialPaged(query).ToList();
			Action readPagedWithPageSize = () => wrapped.ReadPartialPaged(query, 7).ToList();

			// Assert
			read.Should().Throw<NotSupportedException>();
			readPaged.Should().Throw<NotSupportedException>();
			readPagedWithPageSize.Should().Throw<NotSupportedException>();
		}

		private static IPartialQuery<ExampleObject> CreateQuery()
		{
			return new PartialQuery<ExampleObject>(new TRUEFilterElement<ExampleObject>().ToQuery())
				.Include(ExampleObjectExposers.Name);
		}

		private sealed class PartialRepository : IPartialPageableRepository<ExampleObject>
		{
			public int ReadPartialCalls { get; private set; }

			public int ReadPartialPagedCalls { get; private set; }

			public IPartialQuery<ExampleObject> LastQuery { get; private set; } = null!;

			public int LastPageSize { get; private set; }

			public IEnumerable<PartialObject<ExampleObject>> ReadPartial(IPartialQuery<ExampleObject> query)
			{
				ReadPartialCalls++;
				LastQuery = query;
				return new[]
				{
					new PartialObject<ExampleObject>(new ExampleObject { Name = "Partial" }, query.Fields),
				};
			}

			public IEnumerable<IPagedResult<PartialObject<ExampleObject>>> ReadPartialPaged(IPartialQuery<ExampleObject> query)
			{
				ReadPartialPagedCalls++;
				LastQuery = query;
				return PartialRepository.CreatePage(query, 30);
			}

			public IEnumerable<IPagedResult<PartialObject<ExampleObject>>> ReadPartialPaged(IPartialQuery<ExampleObject> query, int pageSize)
			{
				ReadPartialPagedCalls++;
				LastQuery = query;
				LastPageSize = pageSize;
				return PartialRepository.CreatePage(query, pageSize);
			}

			private static IEnumerable<IPagedResult<PartialObject<ExampleObject>>> CreatePage(IPartialQuery<ExampleObject> query, int pageSize)
			{
				var item = new PartialObject<ExampleObject>(new ExampleObject { Name = "Partial" }, query.Fields);
				return new[]
				{
					new PagedResult<PartialObject<ExampleObject>>(new[] { item }, 0, pageSize, false),
				};
			}
		}

		private sealed class PartialMiddleware : IPartialPageableMiddleware<ExampleObject>
		{
			public int ReadPartialCalls { get; private set; }

			public int ReadPartialPagedCalls { get; private set; }

			public int ReadPartialPagedWithPageSizeCalls { get; private set; }

			public IPartialQuery<ExampleObject> LastQuery { get; private set; } = null!;

			public int LastPageSize { get; private set; }

			public IEnumerable<PartialObject<ExampleObject>> OnReadPartial(
				IPartialQuery<ExampleObject> query,
				Func<IPartialQuery<ExampleObject>, IEnumerable<PartialObject<ExampleObject>>> next)
			{
				ReadPartialCalls++;
				LastQuery = query;
				return next(query);
			}

			public IEnumerable<IPagedResult<PartialObject<ExampleObject>>> OnReadPartialPaged(
				IPartialQuery<ExampleObject> query,
				Func<IPartialQuery<ExampleObject>, IEnumerable<IPagedResult<PartialObject<ExampleObject>>>> next)
			{
				ReadPartialPagedCalls++;
				LastQuery = query;
				return next(query);
			}

			public IEnumerable<IPagedResult<PartialObject<ExampleObject>>> OnReadPartialPaged(
				IPartialQuery<ExampleObject> query,
				int pageSize,
				Func<IPartialQuery<ExampleObject>, int, IEnumerable<IPagedResult<PartialObject<ExampleObject>>>> next)
			{
				ReadPartialPagedWithPageSizeCalls++;
				LastQuery = query;
				LastPageSize = pageSize;
				return next(query, pageSize);
			}
		}

		private sealed class MarkerRepository : IRepositoryMarker<ExampleObject>
		{
		}
	}
}
