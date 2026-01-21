namespace Skyline.DataMiner.SDM
{
	using System;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	using SLDataGateway.API.Types.Querying;

	public interface ICountableRepository<T> : IRepositoryMarker<T>
		where T : class
	{
		long Count(FilterElement<T> filter);

		long Count(IQuery<T> query);
	}

	public interface ICountableMiddleware<T> : IMiddlewareMarker<T>
		where T : class
	{
		long OnCount(FilterElement<T> filter, Func<FilterElement<T>, long> next);

		long OnCount(IQuery<T> query, Func<IQuery<T>, long> next);
	}
}
