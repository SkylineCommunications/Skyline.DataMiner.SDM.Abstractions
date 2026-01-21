namespace Skyline.DataMiner.SDM.Types.Converters
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	internal class EnumFieldConverter : IFieldValueConverter
	{
		private readonly bool _isFlags;
		private readonly Dictionary<string, object> _nameLookup;

		public EnumFieldConverter(Type enumType)
		{
			FieldType = enumType ?? throw new ArgumentNullException(nameof(enumType));

			if(!enumType.IsEnum)
			{
				throw new ArgumentException("The provided type is not an enum.", nameof(enumType));
			}

			_isFlags = enumType.IsDefined(typeof(FlagsAttribute), inherit: false);
			_nameLookup = Enum.GetNames(enumType)
				.ToDictionary(name => name, name => Enum.Parse(enumType, name));
		}

		public Type FieldType { get; }

		public object Convert(object value)
		{
			if(value is null)
			{
				throw new ArgumentNullException(nameof(value), "Cannot convert null to non-nullable enum.");
			}

			if (FieldType.IsInstanceOfType(value))
			{
				return value;
			}

			if(value is string s)
			{
				return ConvertFromString(s);
			}

			if(IsNumeric(value))
			{
				return Enum.ToObject(FieldType, value);
			}

			throw new InvalidCastException($"Cannot convert value '{value}' to enum {FieldType}");
		}

		private object ConvertFromString(string input)
		{
			if (_isFlags && input.Contains(','))
			{
				var parts = input.Split(',').Where(s => !String.IsNullOrEmpty(s)).ToArray();

				ulong result = 0;

				foreach (var part in parts)
				{
					if (!_nameLookup.TryGetValue(part.Trim(), out var flag))
					{
						throw new ArgumentException($"'{part}' is not a valid value for enum {FieldType}");
					}

					result |= System.Convert.ToUInt64(flag);
				}

				return Enum.ToObject(FieldType, result);
			}

			if (_nameLookup.TryGetValue(input.Trim(), out var value))
			{
				return value;
			}

			if (ulong.TryParse(input, out var numeric))
			{
				return Enum.ToObject(FieldType, numeric);
			}

			throw new ArgumentException($"'{input}' is not a valid value for enum {FieldType.Name}");
		}

#pragma warning disable SA1204 // Static elements should appear before instance elements
#pragma warning disable S1067 // Expressions should not be too complex
		private static bool IsNumeric(object value)
		{
			return value is byte ||
				value is sbyte ||
				value is short ||
				value is ushort ||
				value is int ||
				value is uint ||
				value is long ||
				value is ulong;
		}
#pragma warning restore S1067 // Expressions should not be too complex
#pragma warning restore SA1204 // Static elements should appear before instance elements
	}
}
