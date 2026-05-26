# SDM Abstractions

> **Do not deploy manually in production**
> This package is deployed automatically by the **SDM Registration solution**. Manual deployment should only be done when targeting a pre-release or development version.

## About

`SDM Abstractions` is the **official DataMiner install package** for the **Standard Data Model (SDM)** abstractions DevPack. It deploys the foundational contracts, interfaces, and utilities needed to build SDM-compliant solutions onto a DataMiner System, enabling consistent, reusable data access patterns across your DataMiner environment.

It is the primary delivery mechanism used by the **SDM Registration solution** to bring the SDM Abstractions DevPack to a DataMiner Agent. For stable releases, deployment is handled automatically. Manual installation is only needed when working with pre-release or development builds.

## What Gets Installed?

This package installs the **`Skyline.DataMiner.Dev.Utils.SDM.Abstractions`** DevPack DLL onto the DataMiner Agent, providing the following building blocks.

### Repository Interfaces

Granular, composable interfaces that let you expose only the capabilities your repository actually supports:

| Interface | Description |
|---|---|
| `IRepository<T>` | Full CRUD composite: create, read, update, delete, count, page |
| `ICreateableRepository<T>` | Single-entity create |
| `IBulkCreateableRepository<T>` | Bulk create |
| `IReadableRepository<T>` | Read by identifier |
| `IBulkReadableRepository<T>` | Bulk read by identifiers |
| `IUpdatableRepository<T>` | Single-entity update |
| `IBulkUpdatableRepository<T>` | Bulk update |
| `IDeletableRepository<T>` | Single-entity delete |
| `IBulkDeletableRepository<T>` | Bulk delete |
| `ICountableRepository<T>` | Count entities matching a `FilterElement<T>` |
| `IPageableRepository<T>` | Paginated reads returning `PagedResult<T>` |
| `IQueryableRepository<T>` | LINQ-style `IQueryable<T>` access |

### SDM Object Model

| Type | Description |
|---|---|
| `ISdmObject` | Base interface; exposes the `Identifier` string property |
| `ISdmObject<T>` | Strongly-typed variant for use in generic constraints |

### Middleware Pipeline

`MiddlewareRepository<T>` wraps any inner repository and delegates each operation through a single middleware layer. Middleware interfaces mirror the repository interfaces:

| Middleware Interface | Intercepted Operation |
|---|---|
| `ICreatableMiddleware<T>` | `OnCreate` (single) |
| `IBulkCreatableMiddleware<T>` | `OnCreate` (bulk) |
| `IUpdatableMiddleware<T>` | `OnUpdate` (single) |
| `IBulkUpdatableMiddleware<T>` | `OnUpdate` (bulk) |
| `IDeletableMiddleware<T>` | `OnDelete` (single) |
| `IBulkDeletableMiddleware<T>` | `OnDelete` (bulk) |
| `IQueryableMiddleware<T>` | `OnQuery` |

Each `On*` method receives a `next` delegate, allowing full pre/post processing around the inner repository call.

### Paging

| Type | Description |
|---|---|
| `IPageResult<T>` | Page result contract: items, page number, count, has-next flag |
| `PagedResult<T>` | Concrete implementation with a static `From(list, page, pageSize)` factory |

### Factory Utilities

| Type | Description |
|---|---|
| `FilterElementFactory` | Creates a `FilterElement<T>` from a `FieldExposer`, a `Comparer`, and a value, with built-in type conversion and comparer validation |
| `OrderByElementFactory` | Creates an `IOrderByElement` from a `FieldExposer`, a `SortOrder`, and an optional natural-sort flag |

### Field Type System

The `Types` namespace provides the infrastructure that `FilterElementFactory` relies on to translate CLR values into SLDataGateway-compatible filter values:

- **Converters** - `IFieldValueConverter` implementations for `bool`, `byte`, `DateTime`, `decimal`, `double`, `Enum`, `float`, `Guid`, `int`, `long`, `sbyte`, `short`, `string`, `TimeSpan`, `uint`, `ulong`, `ushort`
- **Shape handlers** - `IFieldShapeHandler` implementations for scalar, string, nullable, collection, and `SdmObject` reference field shapes

## Prerequisites

- DataMiner **10.4.0.0 - 14003** or higher

## Related Resources

- [Skyline.DataMiner.Dev.Utils.SDM.Abstractions NuGet](https://www.nuget.org/packages/Skyline.DataMiner.Dev.Utils.SDM.Abstractions) - Reference this in your own SDM solutions.
- [Full Documentation & Examples](https://github.com/SkylineCommunications/Skyline.DataMiner.SDM.Abstractions/blob/main/README.md)
- [Source Code](https://github.com/SkylineCommunications/Skyline.DataMiner.SDM.Abstractions)

## About DataMiner

DataMiner is a transformational platform that provides vendor-independent control and monitoring of devices and services. Out of the box and by design, it addresses key challenges such as security, complexity, multi-cloud, and much more. It has a pronounced open architecture and powerful capabilities enabling users to evolve easily and continuously.

> **Note:** See also: [About DataMiner](https://aka.dataminer.services/about-dataminer).

## About Skyline Communications

At Skyline Communications, we deal in world-class solutions that are deployed by leading companies around the globe. Check out [our proven track record](https://aka.dataminer.services/about-skyline) and see how we make our customers' lives easier by empowering them to take their operations to the next level.