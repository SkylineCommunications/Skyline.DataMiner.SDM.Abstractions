# SDM Abstractions


> [!IMPORTANT]
> **Do not deploy manually in production** — This package is deployed automatically by the **SDM Registration solution**. Manual deployment should only be done when targeting a pre-release or development version.

## About

`SDM Abstractions` is the **official DataMiner install package** for the **Standard Data Model (SDM)** abstractions DevPack. It deploys the foundational contracts, interfaces, and utilities needed to build SDM-compliant solutions onto a DataMiner System — enabling consistent, reusable data access patterns across your DataMiner environment.

## Key Features

- **Repository abstraction layer** — Standardized interfaces that decouple business logic from storage implementations, making solutions easier to test and maintain.
- **Base SDM object model** — A common base class ensuring consistent entity structure across all SDM-compliant solutions.
- **Composable middleware pipeline** — Building-blocks for adding cross-cutting concerns (e.g. validation, caching) without modifying core logic.
- **Type-safe filtering and paging** — Strongly-typed query support that simplifies data retrieval and reduces runtime errors.
- **Consistent error handling** — Built-in exception types that standardize how errors are surfaced across SDM solutions.

## Prerequisites

- DataMiner Feature Release **10.5.9** or higher
- DataMiner Main Release **10.6.0** or higher

## Technical Reference

- [Skyline.DataMiner.Dev.Utils.SDM.Abstractions NuGet](https://www.nuget.org/packages/Skyline.DataMiner.Dev.Utils.SDM.Abstractions) — The NuGet package to reference in your own SDM solutions.
- [Full Documentation & Examples](https://github.com/SkylineCommunications/Skyline.DataMiner.SDM.Abstractions/blob/main/README.md) — Covers repository patterns, middleware, filtering, and paging in depth.
- [Source Code](https://github.com/SkylineCommunications/Skyline.DataMiner.SDM.Abstractions)
