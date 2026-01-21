namespace Skyline.DataMiner.SDM
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	using SLDataGateway.API.Types.Querying;

	public interface IPageableRepository<T> : IReadableRepository<T>
		where T : class
	{
		IEnumerable<IPagedResult<T>> ReadPaged(FilterElement<T> filter);

		IEnumerable<IPagedResult<T>> ReadPaged(IQuery<T> query);

		IEnumerable<IPagedResult<T>> ReadPaged(FilterElement<T> filter, int pageSize);

		IEnumerable<IPagedResult<T>> ReadPaged(IQuery<T> query, int pageSize);
	}

	public interface IPageableMiddleware<T> : IReadableMiddleware<T>
		where T : class
	{
		IEnumerable<IPagedResult<T>> OnReadPaged(FilterElement<T> filter, Func<FilterElement<T>, IEnumerable<IPagedResult<T>>> next);

		IEnumerable<IPagedResult<T>> OnReadPaged(IQuery<T> query, Func<IQuery<T>, IEnumerable<IPagedResult<T>>> next);

		IEnumerable<IPagedResult<T>> OnReadPaged(FilterElement<T> filter, int pageSize, Func<FilterElement<T>, int, IEnumerable<IPagedResult<T>>> next);

		IEnumerable<IPagedResult<T>> OnReadPaged(IQuery<T> query, int pageSize, Func<IQuery<T>, int, IEnumerable<IPagedResult<T>>> next);
	}
}