namespace Skyline.DataMiner.SDM
{
	using System;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	using SLDataGateway.API.Querying;
	using SLDataGateway.API.Types.Querying;

	/// <summary>
	/// Provides extension methods for building and modifying <see cref="IQuery{T}"/> instances.
	/// </summary>
	public static class QueryExtensions
	{
		/// <summary>
		/// Orders the query results in ascending order by the specified field exposer.
		/// </summary>
		/// <typeparam name="T">The type of the query result.</typeparam>
		/// <param name="query">The query to order.</param>
		/// <param name="exposer">The field exposer to order by.</param>
		/// <returns>A new query with the specified ordering applied.</returns>
		public static IQuery<T> OrderBy<T>(this IQuery<T> query, FieldExposer exposer)
		{
			return query.WithOrder(SLDataGateway.API.Querying.OrderBy.Default.SingleConcat(
				OrderByElement.Default
					.WithFieldExposer(exposer)
					.WithSortOrder(SortOrder.Ascending)));
		}

		/// <summary>
		/// Orders the query results in ascending order by the specified exposer, with optional natural sort.
		/// </summary>
		/// <typeparam name="T">The type of the query result.</typeparam>
		/// <param name="query">The query to order.</param>
		/// <param name="exposer">The exposer to order by.</param>
		/// <param name="naturalSort">Indicates whether to use natural sorting.</param>
		/// <returns>A new query with the specified ordering applied.</returns>
		public static IQuery<T> OrderBy<T>(this IQuery<T> query, Exposer<T, string> exposer, bool naturalSort)
		{
			return query.WithOrder(SLDataGateway.API.Querying.OrderBy.Default.SingleConcat(
				OrderByElement.Default
					.WithFieldExposer(exposer)
					.WithSortOrder(SortOrder.Ascending)
					.WithNaturalSort(naturalSort)));
		}

		/// <summary>
		/// Orders the query results in descending order by the specified field exposer.
		/// </summary>
		/// <typeparam name="T">The type of the query result.</typeparam>
		/// <param name="query">The query to order.</param>
		/// <param name="exposer">The field exposer to order by.</param>
		/// <returns>A new query with the specified ordering applied.</returns>
		public static IQuery<T> OrderByDescending<T>(this IQuery<T> query, FieldExposer exposer)
		{
			return query.WithOrder(SLDataGateway.API.Querying.OrderBy.Default.SingleConcat(
				OrderByElement.Default
					.WithFieldExposer(exposer)
					.WithSortOrder(SortOrder.Descending)));
		}

		/// <summary>
		/// Orders the query results in descending order by the specified exposer, with optional natural sort.
		/// </summary>
		/// <typeparam name="T">The type of the query result.</typeparam>
		/// <param name="query">The query to order.</param>
		/// <param name="exposer">The exposer to order by.</param>
		/// <param name="naturalSort">Indicates whether to use natural sorting.</param>
		/// <returns>A new query with the specified ordering applied.</returns>
		public static IQuery<T> OrderByDescending<T>(this IQuery<T> query, Exposer<T, string> exposer, bool naturalSort)
		{
			return query.WithOrder(SLDataGateway.API.Querying.OrderBy.Default.SingleConcat(
				OrderByElement.Default
					.WithFieldExposer(exposer)
					.WithSortOrder(SortOrder.Ascending)
					.WithNaturalSort(naturalSort)));
		}

		/// <summary>
		/// Adds a secondary ascending order by the specified field exposer to the query.
		/// </summary>
		/// <typeparam name="T">The type of the query result.</typeparam>
		/// <param name="query">The query to order.</param>
		/// <param name="exposer">The field exposer to order by.</param>
		/// <returns>A new query with the additional ordering applied.</returns>
		public static IQuery<T> ThenBy<T>(this IQuery<T> query, FieldExposer exposer)
		{
			return query.WithOrder(query.Order.SingleConcat(
				OrderByElement.Default
					.WithFieldExposer(exposer)
					.WithSortOrder(SortOrder.Ascending)));
		}

		/// <summary>
		/// Adds a secondary ascending order by the specified exposer to the query, with optional natural sort.
		/// </summary>
		/// <typeparam name="T">The type of the query result.</typeparam>
		/// <param name="query">The query to order.</param>
		/// <param name="exposer">The exposer to order by.</param>
		/// <param name="naturalSort">Indicates whether to use natural sorting.</param>
		/// <returns>A new query with the additional ordering applied.</returns>
		public static IQuery<T> ThenBy<T>(this IQuery<T> query, Exposer<T, string> exposer, bool naturalSort)
		{
			return query.WithOrder(query.Order.SingleConcat(
				OrderByElement.Default
					.WithFieldExposer(exposer)
					.WithSortOrder(SortOrder.Ascending)
					.WithNaturalSort(naturalSort)));
		}

		/// <summary>
		/// Adds a secondary descending order by the specified field exposer to the query.
		/// </summary>
		/// <typeparam name="T">The type of the query result.</typeparam>
		/// <param name="query">The query to order.</param>
		/// <param name="exposer">The field exposer to order by.</param>
		/// <returns>A new query with the additional ordering applied.</returns>
		public static IQuery<T> ThenByDescending<T>(this IQuery<T> query, FieldExposer exposer)
		{
			return query.WithOrder(query.Order.SingleConcat(
				OrderByElement.Default
					.WithFieldExposer(exposer)
					.WithSortOrder(SortOrder.Descending)));
		}

		/// <summary>
		/// Adds a secondary descending order by the specified exposer to the query, with optional natural sort.
		/// </summary>
		/// <typeparam name="T">The type of the query result.</typeparam>
		/// <param name="query">The query to order.</param>
		/// <param name="exposer">The exposer to order by.</param>
		/// <param name="naturalSort">Indicates whether to use natural sorting.</param>
		/// <returns>A new query with the additional ordering applied.</returns>
		public static IQuery<T> ThenByDescending<T>(this IQuery<T> query, Exposer<T, string> exposer, bool naturalSort)
		{
			return query.WithOrder(query.Order.SingleConcat(
				OrderByElement.Default
					.WithFieldExposer(exposer)
					.WithSortOrder(SortOrder.Descending)
					.WithNaturalSort(naturalSort)));
		}

		/// <summary>
		/// Limits the number of results returned by the query.
		/// </summary>
		/// <typeparam name="T">The type of the query result.</typeparam>
		/// <param name="query">The query to limit.</param>
		/// <param name="limit">The maximum number of results to return.</param>
		/// <returns>A new query with the specified limit applied.</returns>
		/// <exception cref="ArgumentException">Thrown if the query already contains a limit.</exception>
		public static IQuery<T> Limit<T>(this IQuery<T> query, int limit)
		{
			if (!query.Limit.Equals(LimitBy.Default))
			{
				throw new ArgumentException("Query already contains a limit. Can only have one.", "query");
			}

			return query.WithLimit(LimitBy.Default.WithLimit(limit));
		}
	}
}