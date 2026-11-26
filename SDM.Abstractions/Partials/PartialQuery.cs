namespace Skyline.DataMiner.SDM
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	using SLDataGateway.API.Types.Querying;

	public sealed class PartialQuery<T> : IPartialQuery<T>
		where T : class
	{
		public PartialQuery(IQuery<T> query)
			: this(query, new ExposerCollection<T>())
		{
		}

		internal PartialQuery(IQuery<T> query, IReadOnlyExposerCollection<T> fields)
		{
			Query = query ?? throw new ArgumentNullException(nameof(query));
			Fields = fields is null
				? throw new ArgumentNullException(nameof(fields))
				: new ExposerCollection<T>(fields);
		}

		public IQuery<T> Query { get; }

		public IReadOnlyExposerCollection<T> Fields { get; }

		public IPartialQuery<T> WithFields(IReadOnlyExposerCollection<T> fields)
		{
			return new PartialQuery<T>(Query, fields);
		}

		public IPartialQuery<T> WithQuery(IQuery<T> query)
		{
			return new PartialQuery<T>(query, Fields);
		}

		public IPartialQuery<T> WithFilter(FilterElement<T> filter)
		{
			var query = Query.WithFilter(filter);
			return new PartialQuery<T>(query, Fields);
		}

		public IPartialQuery<T> WithOrder(IOrderBy order)
		{
			var query = Query.WithOrder(order);
			return new PartialQuery<T>(query, Fields);
		}

		public IPartialQuery<T> WithLimit(ILimitBy limit)
		{
			var query = Query.WithLimit(limit);
			return new PartialQuery<T>(query, Fields);
		}

		public IEnumerable<PartialObject<T>> ExecuteInMemory(IEnumerable<T> items)
		{
			if (items is null)
			{
				throw new ArgumentNullException(nameof(items));
			}

			foreach (var item in Query.ExecuteInMemory(items))
			{
				var builder = PartialObject<T>.CreateBuilder();
				foreach (var field in Fields)
				{
					AddFieldValue(builder, item, field);
				}

				yield return builder.Build();
			}
		}

		private static void AddFieldValue(PartialObject<T>.Builder builder, T item, FieldExposer field)
		{
			var exposerType = field.GetType();
			while (exposerType != null &&
				(!exposerType.IsGenericType ||
				 exposerType.GetGenericTypeDefinition() != typeof(Exposer<,>)))
			{
				exposerType = exposerType.BaseType;
			}

			if (exposerType is null)
			{
				throw new InvalidOperationException("The field is not a supported exposer.");
			}

			var valueType = exposerType.GetGenericArguments()[1];
			var method = typeof(PartialQuery<T>).GetMethod(
				nameof(AddTypedFieldValue),
				System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
			var genericMethod = method.MakeGenericMethod(valueType);
			genericMethod.Invoke(null, new object[] { builder, item, field });
		}

		private static void AddTypedFieldValue<TField>(PartialObject<T>.Builder builder, T item, FieldExposer field)
		{
			builder.Include((Exposer<T, TField>)field, ((Exposer<T, TField>)field).internalFunc(item));
		}
	}
}
