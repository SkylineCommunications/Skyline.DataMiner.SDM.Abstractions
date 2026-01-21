namespace Skyline.DataMiner.SDM
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	using SLDataGateway.API.Types.Querying;

	public interface IReadableRepository<T> : IRepositoryMarker<T>
		where T : class
	{
		IEnumerable<T> Read(FilterElement<T> filter);

		IEnumerable<T> Read(IQuery<T> query);
	}

	public interface IReadableMiddleware<T> : IMiddlewareMarker<T>
		where T : class
	{
		IEnumerable<T> OnRead(FilterElement<T> filter, Func<FilterElement<T>, IEnumerable<T>> next);

		IEnumerable<T> OnRead(IQuery<T> query, Func<IQuery<T>, IEnumerable<T>> next);
	}
}