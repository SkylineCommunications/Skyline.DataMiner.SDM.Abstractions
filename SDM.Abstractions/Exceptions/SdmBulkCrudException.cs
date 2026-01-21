namespace Skyline.DataMiner.SDM
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;
	using System.Runtime.Serialization;
	using System.Text;

	/// <summary>
	/// Represents an exception that occurs during a bulk CRUD operation for SDM objects.
	/// </summary>
	/// <typeparam name="T">The type of the SDM object associated with the exception.</typeparam>
	[Serializable]
	public class SdmBulkCrudException<T> : SdmException
		where T : class, ISdmObject
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SdmBulkCrudException{T}"/> class with the specified successful and failed items.
		/// </summary>
		/// <param name="successful">The list of successfully processed items.</param>
		/// <param name="failed">The list of failed CRUD items.</param>
		public SdmBulkCrudException(List<T> successful, List<FailedCrudItem> failed)
			: base(BuildMessage(successful, failed))
		{
			SuccessfulItems = new ReadOnlyCollection<T>(successful);
			FailedItems = new ReadOnlyCollection<FailedCrudItem>(failed);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SdmBulkCrudException{T}"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		protected SdmBulkCrudException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			SuccessfulItems = info.GetValue(nameof(Object), typeof(IReadOnlyCollection<T>)) as IReadOnlyCollection<T>;
			FailedItems = info.GetValue(nameof(Object), typeof(IReadOnlyCollection<FailedCrudItem>)) as IReadOnlyCollection<FailedCrudItem>;
		}

		/// <summary>
		/// Gets a value indicating whether there are any failed items in the bulk operation.
		/// </summary>
		public bool HasFailures => FailedItems.Count > 0;

		/// <summary>
		/// Gets the collection of successfully processed items.
		/// </summary>
		public IReadOnlyCollection<T> SuccessfulItems { get; }

		/// <summary>
		/// Gets the collection of failed CRUD items.
		/// </summary>
		public IReadOnlyCollection<FailedCrudItem> FailedItems { get; }

		/// <summary>
		/// Builds a detailed exception message for the bulk CRUD operation.
		/// </summary>
		/// <param name="successfullItems">The list of successfully processed items.</param>
		/// <param name="failedItems">The list of failed CRUD items.</param>
		/// <returns>A string containing the detailed exception message.</returns>
		private static string BuildMessage(List<T> successfullItems, List<FailedCrudItem> failedItems)
		{
			var builder = new StringBuilder();
			builder.AppendLine($"Bulk CRUD operation: {successfullItems.Count} succeeded, {failedItems} failed");
			builder.Append(" - IDs of the successful items: ");

			foreach (var identifier in successfullItems.Select(item => item.Identifier).Take(10))
			{
				builder.Append(identifier);
				builder.Append(", ");
			}

			if (successfullItems.Count > 10)
			{
				builder.Append($", ... ({successfullItems.Count - 10} more)");
			}

			builder.AppendLine(" - Failures:");

			foreach (var failedItem in failedItems)
			{
				builder.Append($"  - {failedItem.Item}");

				if (failedItem.Exception is null)
				{
					continue;
				}
				else
				{
					builder.Append(":");
				}

				builder.AppendLine();

				var source = failedItem.Exception?.ToString().Split(
					new[]
					{
						Environment.NewLine,
						"\n",
					},
					StringSplitOptions.None) ?? Array.Empty<string>();

				foreach (var line in source)
				{
					builder.AppendLine("    " + line);
				}
			}

			return builder.ToString();
		}

		/// <summary>
		/// Represents a failed CRUD item and its associated exception.
		/// </summary>
		public class FailedCrudItem
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="FailedCrudItem"/> class.
			/// </summary>
			/// <param name="item">The SDM object that failed during the CRUD operation.</param>
			/// <param name="ex">The exception that occurred for the item.</param>
			public FailedCrudItem(T item, Exception ex)
			{
				Item = item;
				Exception = ex;
			}

			/// <summary>
			/// Gets the SDM object that failed during the CRUD operation.
			/// </summary>
			public T Item { get; }

			/// <summary>
			/// Gets the exception that occurred for the item.
			/// </summary>
			public Exception Exception { get; }
		}

		/// <summary>
		/// Builder class for constructing <see cref="SdmBulkCrudException{T}"/> instances.
		/// </summary>
		public class Builder
		{
			private readonly List<FailedCrudItem> _failedItems = new List<FailedCrudItem>();
			private readonly List<T> _successfullItems = new List<T>();

			/// <summary>
			/// Gets a value indicating whether any failures have been added.
			/// </summary>
			public bool HasFailure => _failedItems.Any();

			/// <summary>
			/// Adds a failed item and its exception to the builder.
			/// </summary>
			/// <param name="item">The SDM object that failed.</param>
			/// <param name="exception">The exception that occurred.</param>
			/// <returns>The current <see cref="Builder"/> instance.</returns>
			public Builder AddFailed(T item, Exception exception)
			{
				_failedItems.Add(new FailedCrudItem(item, exception));
				return this;
			}

			/// <summary>
			/// Adds a successfully processed item to the builder.
			/// </summary>
			/// <param name="item">The SDM object that was processed successfully.</param>
			/// <returns>The current <see cref="Builder"/> instance.</returns>
			public Builder AddSuccessful(T item)
			{
				_successfullItems.Add(item);
				return this;
			}

			/// <summary>
			/// Builds a new <see cref="SdmBulkCrudException{T}"/> instance with the collected successful and failed items.
			/// </summary>
			/// <returns>A new <see cref="SdmBulkCrudException{T}"/> instance.</returns>
			public SdmBulkCrudException<T> Build()
			{
				return new SdmBulkCrudException<T>(
					_successfullItems,
					_failedItems);
			}
		}
	}
}
