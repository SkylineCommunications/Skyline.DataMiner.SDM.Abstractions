namespace Skyline.DataMiner.SDM
{
	using System;

	/// <summary>
	/// Represents a reference to an SDM object of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of SDM object being referenced.</typeparam>
	public readonly struct SdmObjectReference<T> : IEquatable<SdmObjectReference<T>> where T : SdmObject<T>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SdmObjectReference{T}"/> struct.
		/// </summary>
		/// <param name="id">The identifier of the SDM object.</param>
		public SdmObjectReference(string id)
		{
			Identifier = id;
		}

		/// <summary>
		/// Gets the identifier of the referenced SDM object.
		/// </summary>
		public string Identifier { get; }

		/// <summary>
		/// Implicitly converts an <see cref="SdmObjectReference{T}"/> to a string.
		/// </summary>
		/// <param name="id">The SDM object reference to convert.</param>
		public static implicit operator string(SdmObjectReference<T> id)
		{
			return id.Identifier;
		}

		/// <summary>
		/// Explicitly converts a string to an <see cref="SdmObjectReference{T}"/>.
		/// </summary>
		/// <param name="id">The identifier string to convert.</param>
		/// <returns>A new <see cref="SdmObjectReference{T}"/> if the identifier is not null or empty; otherwise, the default value.</returns>
		public static explicit operator SdmObjectReference<T>(string id)
		{
			if (String.IsNullOrEmpty(id))
			{
				return default;
			}

			return new SdmObjectReference<T>(id);
		}

		/// <summary>
		/// Implicitly converts an <see cref="SdmObject{T}"/> to an <see cref="SdmObjectReference{T}"/>.
		/// </summary>
		/// <param name="sdmObject">The SDM object to convert.</param>
		/// <returns>A new <see cref="SdmObjectReference{T}"/> if the object and its identifier are not null; otherwise, the default value.</returns>
		public static implicit operator SdmObjectReference<T>(SdmObject<T> sdmObject)
		{
			if (sdmObject?.Identifier is null)
			{
				return default;
			}

			return new SdmObjectReference<T>(sdmObject.Identifier);
		}

		/// <summary>
		/// Determines whether two <see cref="SdmObjectReference{T}"/> instances are equal.
		/// </summary>
		/// <param name="left">The first reference to compare.</param>
		/// <param name="right">The second reference to compare.</param>
		/// <returns><c>true</c> if the references are equal; otherwise, <c>false</c>.</returns>
		public static bool operator ==(SdmObjectReference<T> left, SdmObjectReference<T> right)
		{
			return left.Equals(right);
		}

		/// <summary>
		/// Determines whether two <see cref="SdmObjectReference{T}"/> instances are not equal.
		/// </summary>
		/// <param name="left">The first reference to compare.</param>
		/// <param name="right">The second reference to compare.</param>
		/// <returns><c>true</c> if the references are not equal; otherwise, <c>false</c>.</returns>
		public static bool operator !=(SdmObjectReference<T> left, SdmObjectReference<T> right)
		{
			return !(left == right);
		}

		/// <summary>
		/// Converts an object to an <see cref="SdmObjectReference{T}"/>.
		/// </summary>
		/// <param name="obj">The object to convert. Can be an <see cref="SdmObjectReference{T}"/>, <see cref="ISdmObject{T}"/>, or string.</param>
		/// <returns>An <see cref="SdmObjectReference{T}"/> representing the object.</returns>
		/// <exception cref="InvalidOperationException">Thrown when the object cannot be converted to an <see cref="SdmObjectReference{T}"/>.</exception>
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

		/// <summary>
		/// Determines whether the specified object is equal to the current <see cref="SdmObjectReference{T}"/>.
		/// </summary>
		/// <param name="obj">The object to compare with the current instance.</param>
		/// <returns><c>true</c> if the specified object is equal to the current instance; otherwise, <c>false</c>.</returns>
		public override bool Equals(object obj)
		{
			return obj is SdmObjectReference<T> reference && Equals(reference);
		}

		/// <summary>
		/// Determines whether the specified <see cref="SdmObjectReference{T}"/> is equal to the current instance.
		/// </summary>
		/// <param name="other">The reference to compare with the current instance.</param>
		/// <returns><c>true</c> if the specified reference is equal to the current instance; otherwise, <c>false</c>.</returns>
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

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return Identifier.GetHashCode();
		}

		/// <summary>
		/// Returns a string representation of the <see cref="SdmObjectReference{T}"/>.
		/// </summary>
		/// <returns>A string that represents the current reference.</returns>
		public override string ToString()
		{
			return $"Ref {typeof(T).Name} [{Identifier}]";
		}
	}
}
