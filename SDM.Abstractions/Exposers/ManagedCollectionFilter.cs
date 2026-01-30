namespace Skyline.DataMiner.SDM.Exposers
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Runtime.Serialization;
	using System.Text.RegularExpressions;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	/// <summary>
	/// Represents a filter for managed collections that applies comparison operations against collection elements.
	/// </summary>
	/// <typeparam name="TFilter">The type of object being filtered.</typeparam>
	/// <typeparam name="TField">The type of elements in the collection.</typeparam>
	[Serializable]
	public sealed class ManagedCollectionFilter<TFilter, TField> : ManagedFilter<TFilter, IEnumerable<TField>>, IEquatable<ManagedCollectionFilter<TFilter, TField>>, IDynamicManagedListFilter, ISerializable
		where TFilter : class
	{
		/// <summary>
		/// Maps comparers to their logical inversions for dynamic list filtering.
		/// </summary>
		private static readonly IReadOnlyDictionary<Comparer, Comparer> DynamicListInversions = new Dictionary<Comparer, Comparer>
		{
			{
				Comparer.Equals,
				Comparer.NotEquals
			},
			{
				Comparer.NotEquals,
				Comparer.Equals
			},
			{
				Comparer.GT,
				Comparer.LTE
			},
			{
				Comparer.GTE,
				Comparer.LT
			},
			{
				Comparer.LT,
				Comparer.GTE
			},
			{
				Comparer.LTE,
				Comparer.GT
			},
			{
				Comparer.Contains,
				Comparer.NotContains
			},
			{
				Comparer.NotContains,
				Comparer.Contains
			},
			{
				Comparer.Regex,
				Comparer.NotRegex
			},
			{
				Comparer.NotRegex,
				Comparer.Regex
			},
		};

		/// <summary>
		/// The value to compare against elements in the collection.
		/// </summary>
		private TField _listValue;

		/// <summary>
		/// Initializes a new instance of the <see cref="ManagedCollectionFilter{TFilter, TField}"/> class.
		/// </summary>
		/// <param name="exposer">The collection exposer that extracts the collection from objects.</param>
		/// <param name="comp">The comparison operator to apply.</param>
		/// <param name="value">The value to compare against collection elements.</param>
		public ManagedCollectionFilter(CollectionExposer<TFilter, TField> exposer, Comparer comp, TField value)
			: base(exposer, comp, (IEnumerable<TField>)null, (Func<TFilter, bool>)null)
		{
			_listValue = value;
			stitched = Stitched;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ManagedCollectionFilter{TFilter, TField}"/> class.
		/// </summary>
		/// <param name="exposer">The exposer that extracts the collection from objects.</param>
		/// <param name="comp">The comparison operator to apply.</param>
		/// <param name="value">The value to compare against collection elements.</param>
		public ManagedCollectionFilter(Exposer<TFilter, IEnumerable<TField>> exposer, Comparer comp, TField value)
			: base(exposer, comp, (IEnumerable<TField>)null, (Func<TFilter, bool>)null)
		{
			_listValue = value;
			stitched = Stitched;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ManagedCollectionFilter{TFilter, TField}"/> class from serialized data.
		/// </summary>
		/// <param name="info">The serialization information.</param>
		/// <param name="context">The streaming context.</param>
		private ManagedCollectionFilter(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			_listValue = (TField)info.GetValue("ListValue", typeof(TField));
			StringComparison = TryGetValue<StringComparison>(info, "StringComparison", out var stringComparison) ? stringComparison : StringComparison.OrdinalIgnoreCase;
			stitched = Stitched;
		}

		/// <summary>
		/// Gets the string comparison mode used when comparing string values.
		/// </summary>
		public StringComparison StringComparison { get; internal set; } = StringComparison.OrdinalIgnoreCase;

		/// <summary>
		/// Populates a <see cref="SerializationInfo"/> with the data needed to serialize the filter.
		/// </summary>
		/// <param name="info">The serialization information to populate.</param>
		/// <param name="context">The streaming context.</param>
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ListValue", _listValue, typeof(TField));
			info.AddValue("StringComparison", StringComparison, typeof(StringComparison));
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current filter.
		/// </summary>
		/// <param name="obj">The object to compare with the current filter.</param>
		/// <returns><c>true</c> if the specified object is equal to the current filter; otherwise, <c>false</c>.</returns>
		public override bool Equals(object obj)
		{
			return Equals(obj as ManagedCollectionFilter<TFilter, TField>);
		}

		/// <summary>
		/// Determines whether the specified filter is equal to the current filter.
		/// </summary>
		/// <param name="other">The filter to compare with the current filter.</param>
		/// <returns><c>true</c> if the specified filter is equal to the current filter; otherwise, <c>false</c>.</returns>
		public bool Equals(ManagedCollectionFilter<TFilter, TField> other)
		{
			if (other is null)
			{
				return false;
			}

			if (exposer.Equals(other.exposer) && Comp.Equals(other.Comp))
			{
				ref TField listValue = ref _listValue;
				object obj = other._listValue;
				return listValue.Equals(obj);
			}

			return false;
		}

		/// <summary>
		/// Returns the hash code for this filter.
		/// </summary>
		/// <returns>A hash code for the current filter.</returns>
		public override int GetHashCode()
		{
			return (((Comp.GetHashCode() * 397) ^ ((exposer != null) ? exposer.GetHashCode() : 0)) * 397) ^ (_listValue?.GetHashCode() ?? 0);
		}

		/// <summary>
		/// Returns a string representation of the filter.
		/// </summary>
		/// <returns>A string that represents the current filter.</returns>
		public override string ToString()
		{
			string fixedFieldName = GetFixedFieldName(exposer.fieldName);
			return "(" + typeof(TFilter).Name + "." + fixedFieldName + "[DynamicList<" + _listValue.GetType().Name + ">] " + ManagedFilterBase.comparerToNames[Comp] + Convert.ToString(_listValue, CultureInfo.InvariantCulture) + ")";
		}

		/// <summary>
		/// Creates an inverted version of this filter.
		/// </summary>
		/// <returns>A new filter with the inverse comparison operation.</returns>
		public override ManagedFilterIdentifier invert()
		{
			return new ManagedCollectionFilter<TFilter, TField>(exposer, DynamicListInversions[Comp], _listValue)
			{
				StringComparison = StringComparison,
			};
		}

		/// <summary>
		/// Gets the filter value as a string.
		/// </summary>
		/// <returns>The string representation of the filter value.</returns>
		public override string getValueString()
		{
			return _listValue.ToString();
		}

		/// <summary>
		/// Gets the filter value.
		/// </summary>
		/// <returns>The filter value as an object.</returns>
		public override object getValue()
		{
			return _listValue;
		}

		/// <summary>
		/// Creates the stitched filter function.
		/// </summary>
		/// <returns>A function that evaluates the filter condition.</returns>
		protected override Func<TFilter, bool> CreateStitched()
		{
			return Stitched;
		}

		/// <summary>
		/// Evaluates the filter condition against the specified object.
		/// </summary>
		/// <param name="obj">The object to evaluate.</param>
		/// <returns><c>true</c> if the object matches the filter condition; otherwise, <c>false</c>.</returns>
		/// <exception cref="InvalidOperationException">Thrown when the list or value is null.</exception>
		/// <exception cref="NotSupportedException">Thrown when an unsupported comparer is used.</exception>
		private bool Stitched(TFilter obj)
		{
			if (!(exposer.execute(obj) is IEnumerable<TField> list))
			{
				throw new InvalidOperationException("DynamicListFilter behavior on a null list is not supported.");
			}

			if (_listValue == null)
			{
				throw new InvalidOperationException("DynamicListFilter behavior on a null value is not supported.");
			}

			switch (Comp)
			{
				case Comparer.Contains:
					return DoesDynamicallyContain(_listValue, list);
				case Comparer.NotContains:
					return !DoesDynamicallyContain(_listValue, list);
				case Comparer.Equals:
					return DoesDynamicallyEqual(_listValue, list);
				case Comparer.NotEquals:
					return !DoesDynamicallyEqual(_listValue, list);
				case Comparer.Regex:
					if (_listValue is string regexString2)
					{
						return DoesDynamicallyMatchRegex(regexString2, list);
					}

					throw new NotSupportedException($"the comparer of type {Comp} requires a regex string to match on");
				case Comparer.NotRegex:
					if (_listValue is string regexString)
					{
						return !DoesDynamicallyMatchRegex(regexString, list);
					}

					throw new NotSupportedException($"the comparer of type {Comp} requires a regex string to match on");
				case Comparer.LT:
				case Comparer.GT:
				case Comparer.LTE:
				case Comparer.GTE:
					return DoesDynamicallyCompare((_listValue as IComparable) ?? throw new NotSupportedException($"the comparer of type {Comp} requires a value that is IComparable."), list, Comp);
				default:
					throw new NotSupportedException($"the comparer of type {Comp} is currently not supported by DynamicManagedListFilter.");
			}
		}

		/// <summary>
		/// Performs a dynamic comparison operation on collection elements.
		/// </summary>
		/// <param name="value">The comparable value to compare against.</param>
		/// <param name="list">The collection of elements to compare.</param>
		/// <param name="comp">The comparison operator.</param>
		/// <returns><c>true</c> if any element in the list satisfies the comparison; otherwise, <c>false</c>.</returns>
		/// <exception cref="NotSupportedException">Thrown when an unsupported comparer is used.</exception>
#pragma warning disable SA1204 // Static elements should appear before instance elements
		private static bool DoesDynamicallyCompare(IComparable value, IEnumerable<TField> list, Comparer comp)
#pragma warning restore SA1204 // Static elements should appear before instance elements
		{
			Func<IComparable, IComparable, bool> func = null;
			switch (comp)
			{
				case Comparer.GT:
					func = (IComparable a, IComparable b) => a.CompareTo(b) > 0;
					break;

				case Comparer.GTE:
					func = (IComparable a, IComparable b) => a.CompareTo(b) >= 0;
					break;
				case Comparer.LT:
					func = (IComparable a, IComparable b) => a.CompareTo(b) < 0;
					break;
				case Comparer.LTE:
					func = (IComparable a, IComparable b) => a.CompareTo(b) <= 0;
					break;
				default:
					throw new NotSupportedException($"the comparer of type {comp} is not supported for dynamic compares.");
			}

			foreach (object item in list)
			{
				if (func(item as IComparable, value))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Determines whether any element in the collection equals the specified value.
		/// </summary>
		/// <param name="value">The value to search for.</param>
		/// <param name="list">The collection to search.</param>
		/// <returns><c>true</c> if the collection contains an equal element; otherwise, <c>false</c>.</returns>
		private bool DoesDynamicallyEqual(TField value, IEnumerable<TField> list)
		{
			foreach (object item in list)
			{
				if (item is string text && value is string text2)
				{
					if (text.Equals(text2, StringComparison))
					{
						return true;
					}
				}
				else if (object.Equals(value, item))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Determines whether any element in the collection contains the specified value.
		/// </summary>
		/// <param name="value">The value to search for.</param>
		/// <param name="list">The collection to search.</param>
		/// <returns><c>true</c> if the collection contains an element that contains the value; otherwise, <c>false</c>.</returns>
		private bool DoesDynamicallyContain(TField value, IEnumerable<TField> list)
		{
			foreach (object item in list)
			{
				if (item is string text && value is string text2)
				{
					if (text.IndexOf(text2, StringComparison) >= 0)
					{
						return true;
					}
				}
				else if (object.Equals(value, item))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Determines whether any string element in the collection matches the specified regular expression.
		/// </summary>
		/// <param name="regexString">The regular expression pattern.</param>
		/// <param name="list">The collection to search.</param>
		/// <returns><c>true</c> if any string element matches the regex; otherwise, <c>false</c>.</returns>
		private static bool DoesDynamicallyMatchRegex(string regexString, IEnumerable<TField> list)
		{
			Regex regex = new Regex(regexString);
			foreach (object item in list)
			{
				if (item is string input && regex.IsMatch(input))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Attempts to retrieve a value from serialization information.
		/// </summary>
		/// <typeparam name="TValue">The type of value to retrieve.</typeparam>
		/// <param name="serializationInfo">The serialization information.</param>
		/// <param name="name">The name of the value to retrieve.</param>
		/// <param name="value">When this method returns, contains the retrieved value if successful; otherwise, the default value.</param>
		/// <returns><c>true</c> if the value was successfully retrieved; otherwise, <c>false</c>.</returns>
		private static bool TryGetValue<TValue>(SerializationInfo serializationInfo, string name, out TValue value)
		{
			try
			{
				value = (TValue)serializationInfo.GetValue(name, typeof(TValue));
				return true;
			}
			catch (SerializationException)
			{
				value = default(TValue);
				return false;
			}
		}
	}
}
