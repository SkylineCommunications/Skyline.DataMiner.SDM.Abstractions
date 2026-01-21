namespace Skyline.DataMiner.SDM
{
	using System;

	/// <summary>
	/// Marks a class to be an SDM DOM mapper class, containing the ids for all the properties.
	/// </summary>
	/// <remarks>
	/// This attribute is used to indicate that the class should be processed
	/// by the SourceGenerator as DOM mapping system.
	/// It can be applied to class to enable automatic mapping
	/// between SDM objects and DOM instances.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
	public sealed class SdmDomMapperAttribute : Attribute
	{
	}
}
