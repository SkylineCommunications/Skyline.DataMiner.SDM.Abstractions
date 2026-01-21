namespace Skyline.DataMiner.SDM.Middleware
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.SDM.Telemetry;

	using SLDataGateway.API.Types.Querying;

	public class TracingMiddleware<T> : IBulkRepositoryMiddleware<T>
		where T : class, ISdmObject
	{
		public long OnCount(FilterElement<T> filter, Func<FilterElement<T>, long> next)
		{
			using (var activity = SdmActivitySource.ActivitySource.StartActivity($"count {typeof(T).Name}"))
			{
				activity?.SetTag("sdm.operation", "count");
				activity?.SetTag("sdm.filter.type", "filter");
				activity?.SetTag("sdm.filter.summary", filter?.ToString() ?? "No Filter");
				activity?.SetTag("sdm.entity", typeof(T).Name);
				activity?.SetTag("sdm.entity.full_name", typeof(T).FullName);
				try
				{
					var result = next(filter);
					activity?.SetTag("sdm.result.count", result);
					return result;
				}
				catch (Exception ex)
				{
					if (activity is null)
						throw;

					AddException(activity, ex);
					throw;
				}
			}
		}

		public long OnCount(IQuery<T> query, Func<IQuery<T>, long> next)
		{
			using (var activity = SdmActivitySource.ActivitySource.StartActivity($"count {typeof(T).Name}"))
			{
				activity?.SetTag("sdm.operation", "count");
				activity?.SetTag("sdm.filter.type", "query");
				activity?.SetTag("sdm.filter.summary", query?.ToString() ?? "No Query");
				activity?.SetTag("sdm.entity", typeof(T).Name);
				activity?.SetTag("sdm.entity.full_name", typeof(T).FullName);
				try
				{
					var result = next(query);
					activity?.SetTag("sdm.result.count", result);
					return result;
				}
				catch (Exception ex)
				{
					if (activity is null)
						throw;

					AddException(activity, ex);
					throw;
				}
			}
		}

		public T OnCreate(T oToCreate, Func<T, T> next)
		{
			using (var activity = SdmActivitySource.ActivitySource.StartActivity($"create {typeof(T).Name}"))
			{
				activity?.SetTag("sdm.operation", "create");
				activity?.SetTag("sdm.entity", typeof(T).Name);
				activity?.SetTag("sdm.entity.full_name", typeof(T).FullName);
				activity?.SetTag("sdm.entity.identifier", oToCreate.Identifier);

				try
				{
					return next(oToCreate);
				}
				catch (Exception ex)
				{
					if (activity is null)
						throw;

					AddException(activity, ex);
					throw;
				}
			}
		}

		public IReadOnlyCollection<T> OnCreate(IEnumerable<T> oToCreate, Func<IEnumerable<T>, IReadOnlyCollection<T>> next)
		{
			using (var activity = SdmActivitySource.ActivitySource.StartActivity($"bulk create {typeof(T).Name}"))
			{
				activity?.SetTag("sdm.operation", "create");
				activity?.SetTag("sdm.entity", typeof(T).Name);
				activity?.SetTag("sdm.entity.full_name", typeof(T).FullName);

				if (oToCreate is ICollection<T> collectionToCreate)
				{
					activity?.SetTag("sdm.item.count", collectionToCreate.Count);
				}
				else
				{
					activity?.SetTag("sdm.item.count", oToCreate.Count());
				}

				try
				{
					return next(oToCreate);
				}
				catch (Exception ex)
				{
					if (activity is null)
						throw;

					AddException(activity, ex);
					throw;
				}
			}
		}

		public IReadOnlyCollection<T> OnCreateOrUpdate(IEnumerable<T> oToCreateOrUpdate, Func<IEnumerable<T>, IReadOnlyCollection<T>> next)
		{
			using (var activity = SdmActivitySource.ActivitySource.StartActivity($"bulk upsert {typeof(T).Name}"))
			{
				activity?.SetTag("sdm.operation", "upsert");
				activity?.SetTag("sdm.entity", typeof(T).Name);
				activity?.SetTag("sdm.entity.full_name", typeof(T).FullName);

				if (oToCreateOrUpdate is ICollection<T> collectionToCreate)
				{
					activity?.SetTag("sdm.item.count", collectionToCreate.Count);
				}
				else
				{
					activity?.SetTag("sdm.item.count", oToCreateOrUpdate.Count());
				}

				try
				{
					return next(oToCreateOrUpdate);
				}
				catch (Exception ex)
				{
					if (activity is null)
						throw;

					AddException(activity, ex);
					throw;
				}
			}
		}

		public void OnDelete(T oToDelete, Action<T> next)
		{
			using (var activity = SdmActivitySource.ActivitySource.StartActivity($"delete {typeof(T).Name}"))
			{
				activity?.SetTag("sdm.operation", "delete");
				activity?.SetTag("sdm.entity", typeof(T).Name);
				activity?.SetTag("sdm.entity.full_name", typeof(T).FullName);
				activity?.SetTag("sdm.entity.identifier", oToDelete.Identifier);
				try
				{
					next(oToDelete);
				}
				catch (Exception ex)
				{
					if (activity is null)
						throw;

					AddException(activity, ex);
					throw;
				}
			}
		}

		public void OnDelete(IEnumerable<T> oToDelete, Action<IEnumerable<T>> next)
		{
			using (var activity = SdmActivitySource.ActivitySource.StartActivity($"bulk delete {typeof(T).Name}"))
			{
				activity?.SetTag("sdm.operation", "delete");
				activity?.SetTag("sdm.entity", typeof(T).Name);
				activity?.SetTag("sdm.entity.full_name", typeof(T).FullName);

				if (oToDelete is ICollection<T> collectionToDelete)
				{
					activity?.SetTag("sdm.item.count", collectionToDelete.Count);
				}
				else
				{
					activity?.SetTag("sdm.item.count", oToDelete.Count());
				}

				try
				{
					next(oToDelete);
				}
				catch (Exception ex)
				{
					if (activity is null)
						throw;

					AddException(activity, ex);
					throw;
				}
			}
		}

		public IEnumerable<T> OnRead(FilterElement<T> filter, Func<FilterElement<T>, IEnumerable<T>> next)
		{
			using (var activity = SdmActivitySource.ActivitySource.StartActivity($"read {typeof(T).Name}"))
			{
				activity?.SetTag("sdm.operation", "read");
				activity?.SetTag("sdm.filter.type", "filter");
				activity?.SetTag("sdm.filter.summary", filter?.ToString() ?? "No Filter");
				activity?.SetTag("sdm.entity", typeof(T).Name);
				activity?.SetTag("sdm.entity.full_name", typeof(T).FullName);
				try
				{
					var result = next(filter);
					activity?.SetTag("sdm.result.count", result?.Count() ?? 0);
					return result;
				}
				catch (Exception ex)
				{
					if (activity is null)
						throw;

					AddException(activity, ex);
					throw;
				}
			}
		}

		public IEnumerable<T> OnRead(IQuery<T> query, Func<IQuery<T>, IEnumerable<T>> next)
		{
			using (var activity = SdmActivitySource.ActivitySource.StartActivity($"read {typeof(T).Name}"))
			{
				activity?.SetTag("sdm.operation", "read");
				activity?.SetTag("sdm.filter.type", "query");
				activity?.SetTag("sdm.filter.summary", query?.ToString() ?? "No Query");
				activity?.SetTag("sdm.entity", typeof(T).Name);
				activity?.SetTag("sdm.entity.full_name", typeof(T).FullName);

				try
				{
					var result = next(query);
					activity?.SetTag("sdm.result.count", result?.Count() ?? 0);
					return result;
				}
				catch (Exception ex)
				{
					if (activity is null)
						throw;

					AddException(activity, ex);
					throw;
				}
			}
		}

		public IEnumerable<IPagedResult<T>> OnReadPaged(FilterElement<T> filter, Func<FilterElement<T>, IEnumerable<IPagedResult<T>>> next)
		{
			using (var activity = SdmActivitySource.ActivitySource.StartActivity($"read {typeof(T).Name}"))
			{
				activity?.SetTag("sdm.operation", "read");
				activity?.SetTag("sdm.filter.type", "filter");
				activity?.SetTag("sdm.filter.summary", filter?.ToString() ?? "No Filter");
				activity?.SetTag("sdm.entity", typeof(T).Name);
				activity?.SetTag("sdm.entity.full_name", typeof(T).FullName);
				activity?.SetTag("sdm.page.page_size", "default");

				var enumerator = next(filter).GetEnumerator();
				while (enumerator.MoveNext())
				{
					using (var pagedActivity = SdmActivitySource.ActivitySource.StartActivity($"read {typeof(T).Name} - page"))
					{
						pagedActivity?.SetTag("sdm.operation", "read");
						pagedActivity?.SetTag("sdm.filter.type", "filter");
						pagedActivity?.SetTag("sdm.filter.summary", filter?.ToString() ?? "No Filter");
						pagedActivity?.SetTag("sdm.entity", typeof(T).Name);
						pagedActivity?.SetTag("sdm.entity.full_name", typeof(T).FullName);
						pagedActivity?.SetTag("sdm.page.page_size", "default");
						pagedActivity?.SetTag("sdm.page.page_number", enumerator.Current.PageNumber);
						pagedActivity?.SetTag("sdm.page.count", enumerator.Current.Count);
						pagedActivity?.SetTag("sdm.page.has_next_page", enumerator.Current.HasNextPage);

						yield return enumerator.Current;
					}
				}
			}
		}

		public IEnumerable<IPagedResult<T>> OnReadPaged(IQuery<T> query, Func<IQuery<T>, IEnumerable<IPagedResult<T>>> next)
		{
			using (var activity = SdmActivitySource.ActivitySource.StartActivity($"read {typeof(T).Name}"))
			{
				activity?.SetTag("sdm.operation", "read");
				activity?.SetTag("sdm.filter.type", "query");
				activity?.SetTag("sdm.filter.summary", query?.ToString() ?? "No Query");
				activity?.SetTag("sdm.entity", typeof(T).Name);
				activity?.SetTag("sdm.entity.full_name", typeof(T).FullName);
				activity?.SetTag("sdm.page.page_size", "default");

				var enumerator = next(query).GetEnumerator();
				while (enumerator.MoveNext())
				{
					using (var pagedActivity = SdmActivitySource.ActivitySource.StartActivity($"SDM {typeof(T).Name} ReadPaged Operation - Page"))
					{
						pagedActivity?.SetTag("sdm.operation", "read");
						pagedActivity?.SetTag("sdm.filter.type", "query");
						pagedActivity?.SetTag("sdm.filter.summary", query?.ToString() ?? "No Query");
						pagedActivity?.SetTag("sdm.entity", typeof(T).Name);
						pagedActivity?.SetTag("sdm.entity.full_name", typeof(T).FullName);
						pagedActivity?.SetTag("sdm.page.page_size", "default");
						pagedActivity?.SetTag("sdm.page.page_number", enumerator.Current.PageNumber);
						pagedActivity?.SetTag("sdm.page.count", enumerator.Current.Count);
						pagedActivity?.SetTag("sdm.page.has_next_page", enumerator.Current.HasNextPage);

						yield return enumerator.Current;
					}
				}
			}
		}

		public IEnumerable<IPagedResult<T>> OnReadPaged(FilterElement<T> filter, int pageSize, Func<FilterElement<T>, int, IEnumerable<IPagedResult<T>>> next)
		{
			using (var activity = SdmActivitySource.ActivitySource.StartActivity($"read {typeof(T).Name}"))
			{
				activity?.SetTag("sdm.operation", "read");
				activity?.SetTag("sdm.filter.type", "filter");
				activity?.SetTag("sdm.filter.summary", filter?.ToString() ?? "No Filter");
				activity?.SetTag("sdm.entity", typeof(T).Name);
				activity?.SetTag("sdm.entity.full_name", typeof(T).FullName);
				activity?.SetTag("sdm.page.page_size", pageSize);

				var enumerator = next(filter, pageSize).GetEnumerator();
				while (enumerator.MoveNext())
				{
					using (var pagedActivity = SdmActivitySource.ActivitySource.StartActivity($"SDM {typeof(T).Name} ReadPaged Operation - Page"))
					{
						pagedActivity?.SetTag("sdm.operation", "read");
						pagedActivity?.SetTag("sdm.filter.type", "filter");
						pagedActivity?.SetTag("sdm.filter.summary", filter?.ToString() ?? "No Filter");
						pagedActivity?.SetTag("sdm.entity", typeof(T).Name);
						pagedActivity?.SetTag("sdm.entity.full_name", typeof(T).FullName);
						pagedActivity?.SetTag("sdm.page.page_size", pageSize);
						pagedActivity?.SetTag("sdm.page.page_number", enumerator.Current.PageNumber);
						pagedActivity?.SetTag("sdm.page.count", enumerator.Current.Count);
						pagedActivity?.SetTag("sdm.page.has_next_page", enumerator.Current.HasNextPage);

						yield return enumerator.Current;
					}
				}
			}
		}

		public IEnumerable<IPagedResult<T>> OnReadPaged(IQuery<T> query, int pageSize, Func<IQuery<T>, int, IEnumerable<IPagedResult<T>>> next)
		{
			using (var activity = SdmActivitySource.ActivitySource.StartActivity($"read {typeof(T).Name}"))
			{
				activity?.SetTag("sdm.operation", "read");
				activity?.SetTag("sdm.filter.type", "query");
				activity?.SetTag("sdm.filter.summary", query?.ToString() ?? "No Query");
				activity?.SetTag("sdm.entity", typeof(T).Name);
				activity?.SetTag("sdm.entity.full_name", typeof(T).FullName);
				activity?.SetTag("sdm.page.page_size", pageSize);

				var enumerator = next(query, pageSize).GetEnumerator();
				while (enumerator.MoveNext())
				{
					using (var pagedActivity = SdmActivitySource.ActivitySource.StartActivity($"SDM {typeof(T).Name} ReadPaged Operation - Page"))
					{
						pagedActivity?.SetTag("sdm.operation", "read");
						pagedActivity?.SetTag("sdm.filter.type", "query");
						pagedActivity?.SetTag("sdm.filter.summary", query?.ToString() ?? "No Query");
						pagedActivity?.SetTag("sdm.entity", typeof(T).Name);
						pagedActivity?.SetTag("sdm.entity.full_name", typeof(T).FullName);
						pagedActivity?.SetTag("sdm.page.page_size", pageSize);
						pagedActivity?.SetTag("sdm.page.page_number", enumerator.Current.PageNumber);
						pagedActivity?.SetTag("sdm.page.count", enumerator.Current.Count);
						pagedActivity?.SetTag("sdm.page.has_next_page", enumerator.Current.HasNextPage);

						yield return enumerator.Current;
					}
				}
			}
		}

		public T OnUpdate(T oToUpdate, Func<T, T> next)
		{
			using (var activity = SdmActivitySource.ActivitySource.StartActivity($"update {typeof(T).Name}"))
			{
				activity?.SetTag("sdm.operation", "update");
				activity?.SetTag("sdm.entity", typeof(T).Name);
				activity?.SetTag("sdm.entity.full_name", typeof(T).FullName);
				activity?.SetTag("sdm.entity.identifier", oToUpdate.Identifier);

				try
				{
					return next(oToUpdate);
				}
				catch (Exception ex)
				{
					if (activity is null)
						throw;

					AddException(activity, ex);
					throw;
				}
			}
		}

		public IReadOnlyCollection<T> OnUpdate(IEnumerable<T> oToUpdate, Func<IEnumerable<T>, IReadOnlyCollection<T>> next)
		{
			using (var activity = SdmActivitySource.ActivitySource.StartActivity($"bulk update {typeof(T).Name}"))
			{
				activity?.SetTag("sdm.operation", "delete");
				activity?.SetTag("sdm.entity", typeof(T).Name);
				activity?.SetTag("sdm.entity.full_name", typeof(T).FullName);

				if (oToUpdate is ICollection<T> collectionToUpdate)
				{
					activity?.SetTag("sdm.item.count", collectionToUpdate.Count);
				}
				else
				{
					activity?.SetTag("sdm.item.count", oToUpdate.Count());
				}

				try
				{
					return next(oToUpdate);
				}
				catch (Exception ex)
				{
					if (activity is null)
						throw;

					AddException(activity, ex);
					throw;
				}
			}
		}

		private static void AddException(Activity activity, Exception ex)
		{
			var tags = new ActivityTagsCollection
			{
				{ "exception.type", ex.GetType().FullName },
				{ "exception.message", ex.Message },
				{ "exception.stacktrace", ex.StackTrace },
			};

			var acitivityEvent = new ActivityEvent("exception", DateTimeOffset.Now, tags);
			activity.AddEvent(acitivityEvent);
		}
	}
}