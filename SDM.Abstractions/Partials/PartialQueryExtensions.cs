namespace Skyline.DataMiner.SDM
{
	using System;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	using SLDataGateway.API.Querying;

	/// <summary>
	/// Provides extension methods for building and composing <see cref="IPartialQuery{T}"/> instances.
	/// </summary>
	public static class PartialQueryExtensions
	{
		/// <summary>
		/// Includes the field exposed by the specified <paramref name="exposer"/> in the partial query result.
		/// </summary>
		/// <typeparam name="T">The type being queried.</typeparam>
		/// <typeparam name="TField">The type of the field being included.</typeparam>
		/// <param name="partial">The partial query to add the field to.</param>
		/// <param name="exposer">The exposer identifying the field to include.</param>
		/// <returns>A new <see cref="IPartialQuery{T}"/> that includes the specified field.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="partial"/> or <paramref name="exposer"/> is <see langword="null"/>.</exception>
		public static IPartialQuery<T> Include<T, TField>(this IPartialQuery<T> partial, Exposer<T, TField> exposer)
			where T : class
		{
			if (partial is null)
			{
				throw new ArgumentNullException(nameof(partial));
			}

			if (exposer is null)
			{
				throw new ArgumentNullException(nameof(exposer));
			}

			var fields = new ExposerCollection<T>(partial.Fields)
			{
				exposer,
			};

			return new PartialQuery<T>(partial.Query, fields);
		}

		/// <summary>
		/// Limits the number of results returned by the partial query.
		/// </summary>
		/// <typeparam name="T">The type being queried.</typeparam>
		/// <param name="partial">The partial query to limit.</param>
		/// <param name="limit">The maximum number of results to return.</param>
		/// <returns>A new <see cref="IPartialQuery{T}"/> with the specified limit applied.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="partial"/> is <see langword="null"/>.</exception>
		public static IPartialQuery<T> Limit<T>(this IPartialQuery<T> partial, int limit)
			where T : class
		{
			if (partial is null)
			{
				throw new ArgumentNullException(nameof(partial));
			}

			var query = partial.Query.WithLimit(LimitBy.Default.WithLimit(limit));
			return new PartialQuery<T>(query, partial.Fields);
		}

		/// <summary>
		/// Orders the partial query results in ascending order by the field exposed by the specified <paramref name="exposer"/>.
		/// </summary>
		/// <typeparam name="T">The type being queried.</typeparam>
		/// <param name="partial">The partial query to order.</param>
		/// <param name="exposer">The exposer identifying the field to order by.</param>
		/// <returns>A new <see cref="IPartialQuery{T}"/> ordered by the specified field.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="partial"/> or <paramref name="exposer"/> is <see langword="null"/>.</exception>
		public static IPartialQuery<T> OrderBy<T>(this IPartialQuery<T> partial, FieldExposer exposer)
			where T : class
		{
			if (partial is null)
			{
				throw new ArgumentNullException(nameof(partial));
			}

			if (exposer is null)
			{
				throw new ArgumentNullException(nameof(exposer));
			}

			var query = partial.Query.OrderBy(exposer);
			return new PartialQuery<T>(query, partial.Fields);
		}

		/// <summary>
		/// Orders the partial query results in ascending order by the string field exposed by the specified <paramref name="exposer"/>.
		/// </summary>
		/// <typeparam name="T">The type being queried.</typeparam>
		/// <param name="partial">The partial query to order.</param>
		/// <param name="exposer">The exposer identifying the string field to order by.</param>
		/// <param name="naturalSort">A value indicating whether natural sort order should be used.</param>
		/// <returns>A new <see cref="IPartialQuery{T}"/> ordered by the specified field.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="partial"/> or <paramref name="exposer"/> is <see langword="null"/>.</exception>
		public static IPartialQuery<T> OrderBy<T>(this IPartialQuery<T> partial, Exposer<T, string> exposer, bool naturalSort)
			where T : class
		{
			if (partial is null)
			{
				throw new ArgumentNullException(nameof(partial));
			}

			if (exposer is null)
			{
				throw new ArgumentNullException(nameof(exposer));
			}

			var query = partial.Query.OrderBy(exposer, naturalSort);
			return new PartialQuery<T>(query, partial.Fields);
		}

		/// <summary>
		/// Orders the partial query results in descending order by the field exposed by the specified <paramref name="exposer"/>.
		/// </summary>
		/// <typeparam name="T">The type being queried.</typeparam>
		/// <param name="partial">The partial query to order.</param>
		/// <param name="exposer">The exposer identifying the field to order by.</param>
		/// <returns>A new <see cref="IPartialQuery{T}"/> ordered by the specified field.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="partial"/> or <paramref name="exposer"/> is <see langword="null"/>.</exception>
		public static IPartialQuery<T> OrderByDescending<T>(this IPartialQuery<T> partial, FieldExposer exposer)
			where T : class
		{
			if (partial is null)
			{
				throw new ArgumentNullException(nameof(partial));
			}

			if (exposer is null)
			{
				throw new ArgumentNullException(nameof(exposer));
			}

			var query = partial.Query.OrderByDescending(exposer);
			return new PartialQuery<T>(query, partial.Fields);
		}

		/// <summary>
		/// Orders the partial query results in descending order by the string field exposed by the specified <paramref name="exposer"/>.
		/// </summary>
		/// <typeparam name="T">The type being queried.</typeparam>
		/// <param name="partial">The partial query to order.</param>
		/// <param name="exposer">The exposer identifying the string field to order by.</param>
		/// <param name="naturalSort">A value indicating whether natural sort order should be used.</param>
		/// <returns>A new <see cref="IPartialQuery{T}"/> ordered by the specified field.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="partial"/> or <paramref name="exposer"/> is <see langword="null"/>.</exception>
		public static IPartialQuery<T> OrderByDescending<T>(this IPartialQuery<T> partial, Exposer<T, string> exposer, bool naturalSort)
			where T : class
		{
			if (partial is null)
			{
				throw new ArgumentNullException(nameof(partial));
			}

			if (exposer is null)
			{
				throw new ArgumentNullException(nameof(exposer));
			}

			var query = partial.Query.OrderByDescending(exposer, naturalSort);
			return new PartialQuery<T>(query, partial.Fields);
		}

		/// <summary>
		/// Performs a subsequent ascending ordering of the partial query results by the field exposed by the specified <paramref name="exposer"/>.
		/// </summary>
		/// <typeparam name="T">The type being queried.</typeparam>
		/// <param name="partial">The partial query to order.</param>
		/// <param name="exposer">The exposer identifying the field to order by.</param>
		/// <returns>A new <see cref="IPartialQuery{T}"/> with the additional ordering applied.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="partial"/> or <paramref name="exposer"/> is <see langword="null"/>.</exception>
		public static IPartialQuery<T> ThenBy<T>(this IPartialQuery<T> partial, FieldExposer exposer)
			where T : class
		{
			if (partial is null)
			{
				throw new ArgumentNullException(nameof(partial));
			}

			if (exposer is null)
			{
				throw new ArgumentNullException(nameof(exposer));
			}

			var query = partial.Query.ThenBy(exposer);
			return new PartialQuery<T>(query, partial.Fields);
		}

		/// <summary>
		/// Performs a subsequent ascending ordering of the partial query results by the string field exposed by the specified <paramref name="exposer"/>.
		/// </summary>
		/// <typeparam name="T">The type being queried.</typeparam>
		/// <param name="partial">The partial query to order.</param>
		/// <param name="exposer">The exposer identifying the string field to order by.</param>
		/// <param name="naturalSort">A value indicating whether natural sort order should be used.</param>
		/// <returns>A new <see cref="IPartialQuery{T}"/> with the additional ordering applied.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="partial"/> or <paramref name="exposer"/> is <see langword="null"/>.</exception>
		public static IPartialQuery<T> ThenBy<T>(this IPartialQuery<T> partial, Exposer<T, string> exposer, bool naturalSort)
			where T : class
		{
			if (partial is null)
			{
				throw new ArgumentNullException(nameof(partial));
			}

			if (exposer is null)
			{
				throw new ArgumentNullException(nameof(exposer));
			}

			var query = partial.Query.ThenBy(exposer, naturalSort);
			return new PartialQuery<T>(query, partial.Fields);
		}

		/// <summary>
		/// Performs a subsequent descending ordering of the partial query results by the field exposed by the specified <paramref name="exposer"/>.
		/// </summary>
		/// <typeparam name="T">The type being queried.</typeparam>
		/// <param name="partial">The partial query to order.</param>
		/// <param name="exposer">The exposer identifying the field to order by.</param>
		/// <returns>A new <see cref="IPartialQuery{T}"/> with the additional ordering applied.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="partial"/> or <paramref name="exposer"/> is <see langword="null"/>.</exception>
		public static IPartialQuery<T> ThenByDescending<T>(this IPartialQuery<T> partial, FieldExposer exposer)
			where T : class
		{
			if (partial is null)
			{
				throw new ArgumentNullException(nameof(partial));
			}

			if (exposer is null)
			{
				throw new ArgumentNullException(nameof(exposer));
			}

			var query = partial.Query.ThenByDescending(exposer);
			return new PartialQuery<T>(query, partial.Fields);
		}

		/// <summary>
		/// Performs a subsequent descending ordering of the partial query results by the string field exposed by the specified <paramref name="exposer"/>.
		/// </summary>
		/// <typeparam name="T">The type being queried.</typeparam>
		/// <param name="partial">The partial query to order.</param>
		/// <param name="exposer">The exposer identifying the string field to order by.</param>
		/// <param name="naturalSort">A value indicating whether natural sort order should be used.</param>
		/// <returns>A new <see cref="IPartialQuery{T}"/> with the additional ordering applied.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="partial"/> or <paramref name="exposer"/> is <see langword="null"/>.</exception>
		public static IPartialQuery<T> ThenByDescending<T>(this IPartialQuery<T> partial, Exposer<T, string> exposer, bool naturalSort)
			where T : class
		{
			if (partial is null)
			{
				throw new ArgumentNullException(nameof(partial));
			}

			if (exposer is null)
			{
				throw new ArgumentNullException(nameof(exposer));
			}

			var query = partial.Query.ThenByDescending(exposer, naturalSort);
			return new PartialQuery<T>(query, partial.Fields);
		}
	}
}
