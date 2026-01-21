namespace Skyline.DataMiner.SDM
{
	using System;

	/// <summary>
	/// Instructs the SDM source generator to exclude a property from code generation.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Apply this attribute to properties that should be excluded from SDM source generator processing.
	/// The source generator will skip properties marked with this attribute when generating SDM-related code.
	/// </para>
	/// <para>
	/// This attribute cannot be inherited and can only be applied once per property.
	/// </para>
	/// </remarks>
	[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class SdmIgnoreAttribute : Attribute
	{
	}
}
