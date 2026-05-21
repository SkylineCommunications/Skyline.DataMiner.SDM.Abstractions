# Proposal: Nullable Type Support for Exposers

## Status
**Draft** - Decisions Recorded

## Summary
This proposal defines nullable value type support (`T?`) for `Exposer<TFilter, TField>` filter extension methods, enabling filtering on optional properties. Based on the team discussion, the initial scope is limited to nullable structs and introduces explicit null-check methods instead of supporting `Equal(null)`.

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

## Meeting Outcome

The following decisions were made during the design review:

- Nullable structs will support two new filter extensions:
  - `HasValue()`
  - `HasNoValue()`
- `Equal(null)` will not be supported for nullable types.
- Reference types such as `string` will not receive `HasValue()` / `HasNoValue()` support in the initial implementation.
- Collection nullables are out of scope for the first phase.
- Filtering and sorting behavior for nullable types will follow the default .NET behavior.

## Rationale

### Why explicit `HasValue()` / `HasNoValue()`?

- The intent is explicit and easy to understand at the call site.
- It avoids ambiguity around what `Equal(null)` should mean.
- It avoids encouraging an API shape that is known to behave poorly for reference types.

### Comparison Operator Behavior

**Note:** Comparison operators (`LessThan`, `GreaterThan`, etc.) will naturally exclude NULL values. C# nullable operators return false when comparing null (e.g., `null < 5` → `false`), which matches SQL WHERE behavior.

### Why not support `Equal(null)`?

- For reference types such as `string`, in-memory filtering in the core platform can throw because of how string equality is implemented.
- That behavior is not currently planned to be fixed in the platform.
- In DOM, a string field with no value is typically represented as field-not-present rather than field-present-with-null, which creates different behavior between persisted data and in-memory evaluation.
- Not supporting `Equal(null)` for nullable types avoids setting a misleading expectation that null equality is broadly supported.

### Why no reference-type null support in the first phase?

- The technical issues above apply primarily to reference types.
- Supporting nullable structs already addresses the main use cases discussed in the meeting.
- A custom managed filter for reference types could be explored later if a concrete use case appears.

## Proposed Approach

Implement explicit null checks for nullable structs, together with the existing comparison operators for nullable values.

1. **Explicit Null Checks:**
   - `HasValue()` - Returns items where the nullable field is not null
   - `HasNoValue()` - Returns items where the nullable field is null

2. **Nullable Comparisons:**
   - `Equal(TField value)` - Compare against a non-nullable value
   - `NotEqual(TField value)` or `NotEqual(TField? value)` may still be supported depending on the final API shape, but null equality is not part of the intended usage model
   - `LessThan`, `GreaterThan`, `LessThanOrEqual`, `GreaterThanOrEqual` remain available for nullable structs

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
NotEqual<TFilter, TField>(this Exposer<TFilter, TField?> exposer, TField value)
```

### Comparison Operators

```csharp
LessThan<TFilter, TField>(this Exposer<TFilter, TField?> exposer, TField? value)
LessThanOrEqual<TFilter, TField>(this Exposer<TFilter, TField?> exposer, TField? value)
GreaterThan<TFilter, TField>(this Exposer<TFilter, TField?> exposer, TField? value)
GreaterThanOrEqual<TFilter, TField>(this Exposer<TFilter, TField?> exposer, TField? value)
```

**Behavior:** Comparison operators naturally exclude NULL values from results (e.g., `null < 5` evaluates to `false`).

## Scope Boundaries

- Included in first phase:
   - Nullable structs such as `int?`, `DateTime?`, `TimeSpan?`, etc.
   - `HasValue()` and `HasNoValue()` extensions for those nullable structs
   - Existing comparison operators for nullable structs

- Explicitly out of scope for first phase:
   - Reference-type null filters such as `string.HasValue()` / `string.HasNoValue()`
   - `Equal(null)` support for nullable types
   - Collection nullables such as `IEnumerable<int?>`

- For reference types in the initial implementation, null handling remains a manual concern in consumer code.

## Future Considerations

- A custom managed filter for reference types may be considered later if a concrete use case arises.
- Collection nullable support can be revisited in a later iteration if needed.

## Notes

- The chosen approach favors explicitness and predictable behavior over API flexibility.
- The team considered this the clearest way to support nullable filtering without introducing inconsistent behavior between storage and in-memory evaluation.
- The approach was also considered the safest way to add nullable support without introducing avoidable breaking changes in related APIs.
