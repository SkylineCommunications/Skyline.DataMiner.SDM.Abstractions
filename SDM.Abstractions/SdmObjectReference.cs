namespace Skyline.DataMiner.SDM
{
	using System;

	public readonly struct SdmObjectReference<T> : IEquatable<SdmObjectReference<T>> where T : SdmObject<T>
	{
		public SdmObjectReference(string id)
		{
			Identifier = id;
		}

		public string Identifier { get; }

		public static implicit operator string(SdmObjectReference<T> id)
		{
			return id.Identifier;
		}

		public static explicit operator SdmObjectReference<T>(string id)
		{
			if (String.IsNullOrEmpty(id))
			{
				return default;
			}

			return new SdmObjectReference<T>(id);
		}

		public static implicit operator SdmObjectReference<T>(SdmObject<T> sdmObject)
		{
			if (sdmObject?.Identifier is null)
			{
				return default;
			}

			return new SdmObjectReference<T>(sdmObject.Identifier);
		}

		public static bool operator ==(SdmObjectReference<T> left, SdmObjectReference<T> right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(SdmObjectReference<T> left, SdmObjectReference<T> right)
		{
			return !(left == right);
		}

		public static SdmObjectReference<T> Convert(object obj)
		{
			switch (obj)
			{
				case SdmObjectReference<T> refValue:
					return refValue;
				case ISdmObject<T> sdmObj:
					return new SdmObjectReference<T>(sdmObj.Identifier);
				case string id:
					return new SdmObjectReference<T>(id);
				default:
					throw new InvalidOperationException($"Cannot convert {obj?.GetType().Name} to {typeof(SdmObjectReference<T>).Name}");
			}
		}

		public override bool Equals(object obj)
		{
			return obj is SdmObjectReference<T> reference && Equals(reference);
		}

		public bool Equals(SdmObjectReference<T> other)
		{
			if (String.IsNullOrEmpty(Identifier) && String.IsNullOrEmpty(other.Identifier))
			{
				return true;
			}

			if (String.IsNullOrEmpty(Identifier) || String.IsNullOrEmpty(other.Identifier))
			{
				return false;
			}

			return Identifier.Equals(other.Identifier);
		}

		public override int GetHashCode()
		{
			return Identifier.GetHashCode();
		}

		public override string ToString()
		{
			return $"Ref {typeof(T).Name} [{Identifier}]";
		}
	}
}
