namespace Skyline.DataMiner.SDM
{
	using System;

	/// <summary>
	/// Marks a class as allowing SDM middleware to be applied.
	/// </summary>
	/// <remarks>
	/// This attribute should be applied to classes that support SDM middleware processing.
	/// The attribute cannot be inherited and can only be applied once per class.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
	public sealed class AllowSdmMiddlewareAttribute : Attribute
	{
	}
}
