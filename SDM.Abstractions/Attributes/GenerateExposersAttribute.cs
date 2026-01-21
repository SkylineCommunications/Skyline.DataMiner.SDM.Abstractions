namespace Skyline.DataMiner.SDM
{
	using System;

	/// <summary>
	/// Indicates that exposer methods should be automatically generated for the decorated class.
	/// </summary>
	/// <remarks>
	/// This attribute is used by source generators to create exposer methods that provide controlled access to class members.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class GenerateExposersAttribute : Attribute
	{
	}
}
