namespace SDM.AbstractionsTests.API
{
	using System.Threading.Tasks;

	using PublicApiGenerator;

	[TestClass]
	[UsesVerify]
	public partial class ApiChanges
	{
		[TestMethod]
		public Task PublicChanges()
		{
			var assembly = typeof(Skyline.DataMiner.SDM.IRepository<>).Assembly;
			var publicApi = assembly.GeneratePublicApi();

			return Verify(publicApi)
				.UseFileName("SDM.Abstractions");
		}
	}
}
