namespace Skyline.DataMiner.SDM.Telemetry
{
	using System.Diagnostics;
	using System.Reflection;

	public static class SdmActivitySource
	{
		public static readonly string SourceName = "Skyline.DataMiner.SDM";
		public static readonly string SourceVersion = typeof(SdmActivitySource).Assembly.GetName().Version.ToString();

		internal static readonly AssemblyName AssemblyName = typeof(SdmActivitySource).Assembly.GetName();
		internal static readonly ActivitySource ActivitySource = new ActivitySource(SourceName, SourceVersion);
	}
}