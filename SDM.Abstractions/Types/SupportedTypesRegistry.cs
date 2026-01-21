namespace Skyline.DataMiner.SDM.Types
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;

	using Skyline.DataMiner.SDM.Types.Converters;

	internal class SupportedTypesRegistry
	{
		//// Create a registry of supported types. We can then use that in the FilterElementFactory to get a valid FilterElement
		//// We could maybe use them in other places too? to be checked

		private static readonly object RegistryLock = new object();
		private static readonly Dictionary<Type, IFieldValueConverter> _registeredTypes = new Dictionary<Type, IFieldValueConverter>
		{
			[typeof(bool)] = new BoolFieldConverter(),
			[typeof(sbyte)] = new SByteFieldConverter(),
			[typeof(byte)] = new ByteFieldConverter(),
			[typeof(short)] = new ShortFieldConverter(),
			[typeof(ushort)] = new UShortFieldConverter(),
			[typeof(int)] = new IntFieldConverter(),
			[typeof(uint)] = new UIntFieldConverter(),
			[typeof(long)] = new LongFieldConverter(),
			[typeof(ulong)] = new ULongFieldConverter(),
			[typeof(float)] = new FloatFieldConverter(),
			[typeof(double)] = new DoubleFieldConverter(),
			[typeof(decimal)] = new DecimalFieldConverter(),
			[typeof(string)] = new StringFieldConverter(),
			[typeof(DateTime)] = new DateTimeFieldConverter(),
			[typeof(TimeSpan)] = new TimeSpanFieldConverter(),
			[typeof(Guid)] = new GuidFieldConverter(),
		};

		private SupportedTypesRegistry()
		{
		}

		public static IFieldValueConverter GetConverter(Type type)
		{
			// This ensures that if the type registers its converter in a static constructor, they're available before the lookup
			RuntimeHelpers.RunClassConstructor(type.TypeHandle);
			lock (RegistryLock)
			{
				if (_registeredTypes.TryGetValue(type, out var converter))
				{
					return converter;
				}

				if (type.IsEnum)
				{
					return new EnumFieldConverter(type);
				}

				return null;
			}
		}

		public static bool TryGetConverter(Type type, out IFieldValueConverter converter)
		{
			converter = GetConverter(type);
			if (converter is null)
			{
				return false;
			}

			return true;
		}

		public static void RegisterConverter(IFieldValueConverter converter, Type type)
		{
			lock (RegistryLock)
			{
				if (!_registeredTypes.ContainsKey(type))
				{
					_registeredTypes.Add(type, converter);
				}
			}
		}
	}
}
