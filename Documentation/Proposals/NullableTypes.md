# Proposal: Nullable Type Support for Exposers

## Status
**Draft** - Under Discussion

## Summary
This proposal defines nullable value type support (`T?`) for `Exposer<TFilter, TField>` filter extension methods, enabling filtering on optional properties.

## Motivation

Users need to filter on optional properties:
- Check if a nullable field has no value (`null`)
- Check if a nullable field has any value (not `null`)
- Compare nullable fields to specific values

```csharp
// Example: Filter devices by optional temperature
var data = GetDevices();

// Check for null explicitly
var filterHasTemp = DeviceExposers.Temperature.HasValue();
var devicesWithTemp = filterHasTemp.ToQuery().ExecuteInMemory(data).ToArray();

var filterNoTemp = DeviceExposers.Temperature.HasNoValue();
var devicesWithoutTemp = filterNoTemp.ToQuery().ExecuteInMemory(data).ToArray();

// Compare nullable fields (NULLs naturally excluded)
var filterHighTemp = DeviceExposers.Temperature.GreaterThan(25);
var hotDevices = filterHighTemp.ToQuery().ExecuteInMemory(data).ToArray();
```

**Current State**: FilterExtensions only supports Enum types and collections. No nullable struct support exists.

## Key Design Questions

### Null-Check API Design

**Question:** Should we provide dedicated methods (`HasValue()`, `HasNoValue()`), allow `Equal(null)`, or both?

**Option A: Dedicated methods only**
- Pro: Explicit and unambiguous
- Con: Cannot enforce for strings (SLDataGateway already allows `Equal(null)`)
- Con: Inconsistent between string and nullable struct exposers

**Option B: Allow `Equal(null)` only**
- Pro: Smaller API surface, familiar pattern
- Con: Less discoverable, less self-documenting

**Option C: Hybrid approach (provide both)**
- Pro: Clarity (`HasValue()`) and flexibility (`Equal(null)`)
- Pro: Consistent with SLDataGateway patterns
- Con: Larger API surface

### Comparison Operator Behavior

**Note:** Comparison operators (`LessThan`, `GreaterThan`, etc.) will naturally exclude NULL values. C# nullable operators return false when comparing null (e.g., `null < 5` → `false`), which matches SQL WHERE behavior.

## Proposed Approach

Provide **both** explicit null checks and nullable comparisons (Option C):

1. **Explicit Null Checks:**
   - `HasValue()` - Returns items where the nullable field is not null
   - `HasNoValue()` - Returns items where the nullable field is null

2. **Nullable Comparisons:**
   - `Equal(TField value)` - Compare against a non-nullable value
   - `Equal(TField? value)` - Accepts nullable values, including `null`
   - `NotEqual(TField? value)` - Not-equals comparison with nullable support
   - `LessThan`, `GreaterThan`, `LessThanOrEqual`, `GreaterThanOrEqual` with nullable values

### Usage Examples

```csharp
var data = GetDevices();

// Null checks
var hasTemp = DeviceExposers.Temperature.HasValue();
var noTemp = DeviceExposers.Temperature.HasNoValue();

// Comparisons (NULLs naturally excluded)
var lowTemp = DeviceExposers.Temperature.LessThan(0);
var result = lowTemp.ToQuery().ExecuteInMemory(data).ToArray();
// Returns only devices with Temperature < 0 (no NULLs)

// To intentionally include NULLs (if desired)
var lowOrNull = DeviceExposers.Temperature.LessThan(0)
    .OR(DeviceExposers.Temperature.HasNoValue());
var resultWithNulls = lowOrNull.ToQuery().ExecuteInMemory(data).ToArray();

// Equal with null
var nullFilter = DeviceExposers.Temperature.Equal(null);
var devicesNoTemp = nullFilter.ToQuery().ExecuteInMemory(data).ToArray();
```

## Proposed API

All methods return `ManagedFilter<TFilter, TField?>` and require `where TField : struct`.

### Null Check Methods

```csharp
HasValue<TFilter, TField>(this Exposer<TFilter, TField?> exposer)
HasNoValue<TFilter, TField>(this Exposer<TFilter, TField?> exposer)
```

### Equality Comparisons

```csharp
Equal<TFilter, TField>(this Exposer<TFilter, TField?> exposer, TField value)
Equal<TFilter, TField>(this Exposer<TFilter, TField?> exposer, TField? value)
NotEqual<TFilter, TField>(this Exposer<TFilter, TField?> exposer, TField? value)
```

### Comparison Operators

```csharp
LessThan<TFilter, TField>(this Exposer<TFilter, TField?> exposer, TField? value)
LessThanOrEqual<TFilter, TField>(this Exposer<TFilter, TField?> exposer, TField? value)
GreaterThan<TFilter, TField>(this Exposer<TFilter, TField?> exposer, TField? value)
GreaterThanOrEqual<TFilter, TField>(this Exposer<TFilter, TField?> exposer, TField? value)
```

**Behavior:** Comparison operators naturally exclude NULL values from results (e.g., `null < 5` evaluates to `false`).

## Discussion Points

1. **Null-Check API**: Should we provide both `HasValue()`/`HasNoValue()` AND `Equal(null)`, or choose one approach?
   - Hybrid approach (both) offers flexibility but larger API
   - Cannot enforce single approach due to SLDataGateway string behavior

1. **Comparison Operator Parameter Type**: Should comparison operators (`LessThan`, `GreaterThan`, etc.) accept nullable values?
   - **Option A**: Accept `TField?` (nullable) - as shown in current implementation
     - Pro: Consistent signature across all methods
     - Con: Allows meaningless comparisons like `LessThan(null)` (always false)
   - **Option B**: Accept only `TField` (non-nullable)
     - Pro: Prevents meaningless null comparisons at compile time
     - Pro: Forces users to be explicit about null checks
     - Con: Slightly less consistent with `Equal` method signatures

1. **Documentation Strategy**: If we provide both methods, which should we emphasize in examples and documentation?

## Open Questions

1. Add Roslyn analyzer to suggest `HasValue()` over `Equal(null)` for discoverability?
1. Should `CollectionExposer<TFilter, TField?>` support nullable element types?
