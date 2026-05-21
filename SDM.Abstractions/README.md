# Skyline.DataMiner.Dev.Utils.SDM.Abstractions

A NuGet package providing the core abstractions, interfaces, and middleware building blocks for Standard Data Model (SDM) repository implementations in DataMiner.

Use this package to build typed repositories with consistent CRUD operations, filtering, paging, and optional middleware for concerns like validation, logging, and authorization.

## Getting Started

> **Prerequisites:** A DataMiner-compatible solution targeting .NET Framework 4.8, with access to the DataMiner packages required by your repository implementation.

Define your SDM model by inheriting from `SdmObject<T>`.

```csharp
using Skyline.DataMiner.SDM;

public class Product : SdmObject<Product>
{
	public string Name { get; set; }
	public decimal Price { get; set; }
}
```

Implement a repository interface such as `IRepository<T>`.

```csharp
public class ProductRepository : IRepository<Product>
{
	public Product Create(Product entity) { /* ... */ }
	public Product Read(string id) { /* ... */ }
	public Product Update(Product entity) { /* ... */ }
	public void Delete(Product entity) { /* ... */ }
	public PagedResult<Product> ReadPage(PageDescriptor pageDescriptor) { /* ... */ }
	public long Count(FilterElement<Product> filter) { /* ... */ }
}
```

Use strongly typed filters and paging in your application flow.

```csharp
var filter = ProductExposers.Price.GreaterThanOrEqual(10m);
var total = repository.Count(filter);

foreach (var page in repository.ReadPaged(filter, 100))
{
	Console.WriteLine($"Page {page.PageNumber}: {page.Count} item(s)");
}
```

For complete guidance and additional examples, see the [full documentation](https://github.com/SkylineCommunications/Skyline.DataMiner.SDM.Abstractions/blob/main/README.md).

## About

Contains the base contracts and utility types for SDM object modeling and repository development.

### About DataMiner

DataMiner is a transformational platform that provides vendor-independent control and monitoring of devices and services. Out of the box and by design, it addresses key challenges such as security, complexity, multi-cloud, and much more. It has a pronounced open architecture and powerful capabilities enabling users to evolve easily and continuously.

The foundation of DataMiner is its powerful and versatile data acquisition and control layer. With DataMiner, there are no restrictions to what data users can access. Data sources may reside on premises, in the cloud, or in a hybrid setup.

A unique catalog of 7000+ connectors already exists. In addition, you can leverage DataMiner Development Packages to build your own connectors (also known as "protocols" or "drivers").

> **Note**
> See also: [About DataMiner](https://aka.dataminer.services/about-dataminer).

### About Skyline Communications

At Skyline Communications, we deal in world-class solutions that are deployed by leading companies around the globe. Check out [our proven track record](https://aka.dataminer.services/about-skyline) and see how we make our customers' lives easier by empowering them to take their operations to the next level.
