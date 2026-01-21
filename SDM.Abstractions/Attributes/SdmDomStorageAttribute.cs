namespace Skyline.DataMiner.SDM
{
	using System;

	/// <summary>
	/// Specifies the DataMiner Object Model (DOM) module identifier for storing instances of the decorated class
	/// and instructs the source generator to generate a full repository implementation for the decorated type.
	/// </summary>
	/// <remarks>
	/// When applied to a class, this attribute triggers automatic generation of a repository implementation
	/// that handles CRUD operations for DOM instances associated with the specified module.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class SdmDomStorageAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SdmDomStorageAttribute"/> class.
		/// </summary>
		/// <param name="moduleId">The DOM module identifier where instances of the decorated class will be stored.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="moduleId"/> is <see langword="null"/>.</exception>
		public SdmDomStorageAttribute(string moduleId)
		{
			ModuleId = moduleId ?? throw new ArgumentNullException(nameof(moduleId));
		}

		/// <summary>
		/// Gets the DOM module identifier.
		/// </summary>
		/// <value>
		/// The DOM module identifier where instances of the decorated class will be stored.
		/// This value is used by the source generator to configure the generated repository implementation.
		/// </value>
		public string ModuleId { get; }
	}
}
