namespace Skyline.DataMiner.SDM
{
#pragma warning disable S4023 // Interfaces should not be empty
#pragma warning disable S2326 // Unused type parameters should be removed

	public interface IRepositoryMarker<T>
		where T : class
	{
	}

	public interface IMiddlewareMarker<T>
		where T : class
	{
	}

#pragma warning restore S2326 // Unused type parameters should be removed
#pragma warning restore S4023 // Interfaces should not be empty
}
