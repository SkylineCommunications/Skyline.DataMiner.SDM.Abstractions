# Skyline.DataMiner.Dev.Utils.Abstractions

A library providing abstractions, interfaces, and utilities for building Standard Data Model (SDM) repositories in DataMiner. This package simplifies CRUD operations, enables middleware patterns, and provides extensibility through attributes and exposers.

## Table of Contents

- [Installation](#installation)
- [Core Concepts](#core-concepts)
- [Getting Started](#getting-started)
- [Repository Interfaces](#repository-interfaces)
- [Working with SDM Objects](#working-with-sdm-objects)
- [Middleware Pattern](#middleware-pattern)
- [Exception Handling](#exception-handling)
- [Examples](#examples)

## Installation

Install the package via NuGet:

```bash
Install-Package Skyline.DataMiner.Dev.Utils.SDM.Abstractions
```

Or via .NET CLI:

```bash
dotnet add package Skyline.DataMiner.Dev.Utils.SDM.Abstractions
```

## Core Concepts

**SDM (Standard Data Model)** provides a structured approach to managing domain objects in DataMiner. This library offers:

- **Repository Pattern**: Standard interfaces for CRUD operations
- **Middleware Support**: Extensible pipeline for cross-cutting concerns (validation, logging, security)
- **Type-Safe Filtering**: Strongly-typed query capabilities
- **Bulk Operations**: Efficient handling of multiple entities
- **Paging Support**: Built-in pagination for large datasets

## Getting Started

### 1. Define Your SDM Object

Create a class that inherits from `SdmObject<T>`:

```csharp
using Skyline.DataMiner.SDM;

public class Product : SdmObject<Product>
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    
    public Product() : base() { }
    
    public Product(string identifier) : base(identifier) { }
}
```

### 2. Implement a Repository

Implement one or more repository interfaces:

```csharp
public class ProductRepository : IRepository<Product>
{
    public Product Create(Product entity) { /* Implementation */ }
    public Product Read(string id) { /* Implementation */ }
    public Product Update(Product entity) { /* Implementation */ }
    public void Delete(Product entity) { /* Implementation */ }
    public PagedResult<Product> ReadPage(PageDescriptor pageDescriptor) { /* Implementation */ }
    public long Count(FilterElement<Product> filter) { /* Implementation */ }
}
```

### 3. Use the Repository

```csharp
var repository = new ProductRepository();

// Create
var product = new Product { Name = "Widget", Price = 29.99m, Stock = 100 };
var created = repository.Create(product);

// Read
var retrieved = repository
    .Read(ProductExposers.Identifier.Equal(created.Identifier))
    .First();

// Update
retrieved.Price = 24.99m;
var updated = repository.Update(retrieved);

// Delete
repository.Delete(updated);
```

## Repository Interfaces

The library provides granular interfaces for different operations:

### Basic Operations

- **`ICreatableRepository<T>`**: Create single entities
- **`IReadableRepository<T>`**: Read entities by identifier
- **`IUpdatableRepository<T>`**: Update existing entities
- **`IDeletableRepository<T>`**: Delete entities by identifier
- **`ICountableRepository<T>`**: Count entities with filters

### Advanced Operations

- **`IPageableRepository<T>`**: Paginated reads
- **`IQueryableRepository<T>`**: Advanced query support
- **`IBulkRepository<T>`**: Bulk create, update, delete operations

### Composite Interface

**`IRepository<T>`** combines all basic CRUD operations plus paging and counting.

```csharp
public interface IRepository<T> :
    ICreatableRepository<T>,
    IPageableRepository<T>,
    IUpdatableRepository<T>,
    IDeletableRepository<T>,
    ICountableRepository<T>
    where T : class
{
}
```

## Working with SDM Objects

### SdmObject Base Class

All SDM objects should inherit from `SdmObject<T>`:

```csharp
public abstract class SdmObject<T> : ISdmObject<T>, IEquatable<SdmObject<T>>
    where T : SdmObject<T>
{
    public virtual string Identifier { get; set; }
}
```

**Benefits**:
- Automatic identifier management
- Built-in equality comparison based on `Identifier`
- Type-safe self-reference pattern

### SdmObjectReference

For referencing other SDM objects without loading them:

```csharp
public class Order : SdmObject<Order>
{
    public SdmObjectReference<Product> ProductReference { get; set; }
}
```

## Middleware Pattern

The `MiddlewareRepository<T>` enables you to wrap repositories with cross-cutting concerns.

### Creating Middleware

Implement middleware interfaces:

```csharp
public class ValidationMiddleware<T> : ICreatableMiddleware<T> where T : class
{
    public T OnCreate(T entity, Func<T, T> next)
    {
        // Validate before creating
        if (!IsValid(entity))
            throw new ValidationException("Invalid entity");
            
        return next(entity); // Continue to next middleware or repository
    }
}
```

### Applying Middleware

```csharp
var repository = new ProductRepository();
var middleware = new ValidationMiddleware<Product>();
var wrappedRepo = new MiddlewareRepository<Product>(repository, middleware);

// ValidationMiddleware will execute before repository.Create()
var product = wrappedRepo.Create(new Product { Name = "Test" });
```

### Middleware Interfaces

- `ICreatableMiddleware<T>`
- `IUpdatableMiddleware<T>`
- `IDeletableMiddleware<T>`
- `IPageableMiddleware<T>`
- `ICountableMiddleware<T>`
- `IBulkCreatableMiddleware<T>`
- `IBulkUpdatableMiddleware<T>`
- `IBulkDeletableMiddleware<T>`

### PagedResult

Contains both the data and paging metadata:

```csharp
public interface IPageResult<T> : IReadOnlyList<T>
    where T : class
{
    int PageNumber { get; }
    bool HasNextPage { get; }
}
```

## Exception Handling

The library provides specific exception types:

- **`SdmException`**: Base exception for all SDM operations
- **`SdmCrudException`**: Errors during CRUD operations
- **`SdmBulkCrudException`**: Errors during bulk operations (includes partial results)

```csharp
try
{
    repository.Create(product);
}
catch (SdmCrudException ex)
{
    Console.WriteLine($"CRUD failed: {ex.Message}");
}
catch (SdmException ex)
{
    Console.WriteLine($"SDM error: {ex.Message}");
}
```

## Examples

### Full Repository with Middleware

```csharp
// Define your entity
public class Product : SdmObject<Product>
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}

// Create middleware for logging
public class LoggingMiddleware<T> : IRepositoryMiddleware<T> where T : class
{
    public T OnCreate(T entity, Func<T, T> next)
    {
        Console.WriteLine($"Creating {typeof(T).Name}...");
        var result = next(entity);
        Console.WriteLine($"Created with ID: {(result as ISdmObject)?.Identifier}");
        return result;
    }
    
    // Implement other middleware methods...
}

// Usage
var baseRepo = new ProductDomRepository();
var middleware = new LoggingMiddleware<Product>();
var repository = new MiddlewareRepository<Product>(baseRepo, middleware);

var product = repository.Create(new Product 
{ 
    Name = "Premium Widget", 
    Price = 99.99m 
});
```

### Bulk Operations

```csharp
var products = new List<Product>
{
    new Product { Name = "Item 1", Price = 10m },
    new Product { Name = "Item 2", Price = 20m },
    new Product { Name = "Item 3", Price = 30m }
};

var bulkRepo = (IBulkRepository<Product>)repository;
var created = bulkRepo.Create(products);
```

### Advanced Querying

```csharp
var filter = new ANDFilterElement<Product>(
    ProductExposers.Price.GreaterThanOrEqual(10),
    ProductExposers.Price.LessThanOrEqual(100),
);
var count = repository.Count(filter);

foreach(var page in repository.ReadPaged(filter, 100))
{
    Console.WriteLine($"Showing page {page.PageNumber} with {page.Count} of 100 products");
}
```

## About DataMiner

DataMiner is a transformational platform that provides vendor-independent control and monitoring of devices and services. Out of the box and by design, it addresses key challenges such as security, complexity, multi-cloud, and much more. It has a pronounced open architecture and powerful capabilities enabling users to evolve easily and continuously.

The foundation of DataMiner is its powerful and versatile data acquisition and control layer. With DataMiner, there are no restrictions to what data users can access. Data sources may reside on premises, in the cloud, or in a hybrid setup.

A unique catalog of 7000+ connectors already exists. In addition, you can leverage DataMiner Development Packages to build your own connectors (also known as "protocols" or "drivers").

> **Note**
> See also: [About DataMiner](https://aka.dataminer.services/about-dataminer).

## About Skyline Communications

At Skyline Communications, we deal in world-class solutions that are deployed by leading companies around the globe. Check out [our proven track record](https://aka.dataminer.services/about-skyline) and see how we make our customers' lives easier by empowering them to take their operations to the next level.
