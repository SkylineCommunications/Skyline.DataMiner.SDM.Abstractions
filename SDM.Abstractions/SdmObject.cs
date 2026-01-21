namespace Skyline.DataMiner.SDM
{
	using System;

	/// <summary>
	/// Represents an abstract base class for SDM objects with a unique identifier.
	/// </summary>
	/// <typeparam name="T">The derived type that inherits from <see cref="SdmObject{T}"/>.</typeparam>
	public abstract class SdmObject<T> : ISdmObject<T>, IEquatable<SdmObject<T>>
		where T : SdmObject<T>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SdmObject{T}"/> class with a new unique identifier.
		/// </summary>
		protected SdmObject()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SdmObject{T}"/> class with the specified identifier.
		/// </summary>
		/// <param name="identifier">The unique identifier for this object.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="identifier"/> is <see langword="null"/>.</exception>
		protected SdmObject(string identifier)
		{
			Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
		}

		/// <summary>
		/// Gets or sets the unique identifier for this object.
		/// </summary>
		public virtual string Identifier { get; set; } = String.Empty;

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return Identifier.GetHashCode();
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns><see langword="true"/> if the specified object is equal to the current object; otherwise, <see langword="false"/>.</returns>
		public override bool Equals(object obj)
		{
			return Equals(obj as SdmObject<T>);
		}

		/// <summary>
		/// Determines whether the specified <see cref="SdmObject{T}"/> is equal to the current object.
		/// </summary>
		/// <param name="other">The object to compare with the current object.</param>
		/// <returns><see langword="true"/> if the specified object is equal to the current object; otherwise, <see langword="false"/>.</returns>
		public virtual bool Equals(SdmObject<T> other)
		{
			if (other is null)
			{
				return false;
			}

			return Identifier.Equals(other.Identifier);
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string containing the type name and identifier.</returns>
		public override string ToString()
		{
			return $"{typeof(T).Name} [{Identifier}]";
		}
	}
}
