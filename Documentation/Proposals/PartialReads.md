# Proposal: Partial Read Support

## Status
**Draft** - Under Discussion

## Direction from Latest Discussion

The current preferences are:

- Slight preference for representing field selection as `IPartialQuery<T>` rather than introducing a separately constructed field-selection object.
- Slight preference for returning `PartialObject<T>`, with typed accessors for scalar and collection fields.
- Slight preference for exposing regular and paged partial reads through separate capability interfaces, using `ReadPartial` and `ReadPagedPartial`.

These are preferences, not final API decisions. The alternatives below are retained as context.

## Summary
This proposal adds the ability to read only selected properties from objects instead of loading entire objects, reducing data transfer and improving performance when only a subset of fields is needed.

## Motivation

Users often need only a few properties from objects but are forced to load complete objects:

```csharp
// Current: Load entire objects even when only needing Name and Age
var people = repository.Read(PersonExposers.Status.Equal(Status.Active));
foreach (var person in people)
{
    Console.WriteLine($"{person.Name}, {person.Age}"); // Only using 2 properties
}
```

This results in:
- Unnecessary data transfer from database/storage
- Increased memory usage
- Slower query performance
- Wasted bandwidth

## Use Cases

1. **List views**: Display Name and Status only, not entire object
2. **Reports**: Extract specific fields for aggregation/export
3. **Large collections**: Read subset of fields from thousands of objects
4. **Performance optimization**: Reduce query time by fetching only what's needed

## Key Design Questions

### 1. How to Specify Selected Fields?

**Option A: Exposer-based selection (type-safe)**
```csharp
var selection = FieldSelection.For<Person>()
    .Add(PersonExposers.Name)
    .Add(PersonExposers.Age)
    .Add(PersonExposers.Status);
```

**Option B: Lambda projection (most C#-idiomatic)**
```csharp
Expression<Func<Person, PersonListItemDto>> selector =
    p => new PersonListItemDto
    {
        Name = p.Name,
        Age = p.Age,
        Status = p.Status,
    };
```

**Option C: Fluent query extension (`WithFields`)**
```csharp
var partialQuery = query.WithFields(
    PersonExposers.Name,
    PersonExposers.Age,
    PersonExposers.Status);
```
- Similar developer experience as `WithFilter`, `WithOrder`, and `WithLimit`
- Can be modeled as an extension method without modifying `IQuery<T>`
- Could return a wrapper such as `IPartialQuery<T>` carrying selected exposers

### 2. What Should Be Returned?

**Option A: Partial object with accessor methods**
```csharp
PartialObject<Person> result = ...;
string name = result.GetValue(PersonExposers.Name);
IEnumerable<string> tags = result.GetValues(PersonExposers.Tags);
```

**Option B: Dynamic projection**
```csharp
var result = new { Name = "John", Age = 30, Status = Status.Active };
```

**Option C: Dictionary-like access**
```csharp
PartialObject<Person> result = ...;
string name = result["Name"];
// Issues: String-based, not type-safe
```

**Option D: Strongly-typed DTO projection**
```csharp
IEnumerable<PersonListItemDto> results = repository.Read(
    filter,
    p => new PersonListItemDto { Name = p.Name, Age = p.Age, Status = p.Status });
```
- Pro: Very familiar C# usage and strong typing
- Pro: No extra PartialObject wrapper for consumers
- Con: Requires expression parsing/translation for remote execution
- Con: Potentially stricter limits on supported expression constructs

### 3. API Integration

**Constraint:** `FilterElement<T>` and `IQuery<T>` are managed by Platform and cannot be modified.

**Option A: Add overloads to IReadableRepository**
```csharp
IEnumerable<PartialObject<T>> Read(FilterElement<T> filter, FieldSelection<T> fields);
IEnumerable<PartialObject<T>> Read(IQuery<T> query, FieldSelection<T> fields);
```
- Pro: Clean API for consumers
- Pro: We control IReadableRepository
- Con: Breaking for third-party implementations of IReadableRepository
- Con: Separate method signatures for partial vs full reads

**Option A2: New capability interface (non-breaking for existing implementations)**
```csharp
public interface IPartialReadableRepository<T> : IReadableRepository<T> where T : class
{
    IEnumerable<PartialObject<T>> ReadPartial(FilterElement<T> filter, FieldSelection<T> fields);
    IEnumerable<PartialObject<T>> ReadPartial(IQuery<T> query, FieldSelection<T> fields);
}
```
- Pro: Existing IReadableRepository implementations remain source-compatible
- Pro: Clear capability opt-in
- Con: Consumers may need type checks/casts to use partial reads

**Option B: Extension methods on IQuery**
```csharp
// Extension method (not modifying IQuery interface)
public static IPartialQuery<T> WithFields<T>(this IQuery<T> query, params FieldExposer[] selectedFields)
{
    return new PartialQuery<T>(query, selectedFields);
}

// New interface for partial queries
public interface IPartialQuery<T>
{
    IQuery<T> BaseQuery { get; }
    FieldExposer[] Fields { get; }
}
```
- Pro: Fluent chaining: `query.WithFields(selection)`
- Con: Need repository to recognize IPartialQuery type
- Con: More complex API surface

**Option C: Builder pattern wrapping IQuery**
```csharp
public class PartialReadBuilder<T>
{
    public PartialReadBuilder<T> WithQuery(IQuery<T> query);
    public PartialReadBuilder<T> WithFields(FieldSelection<T> fields);
    public IEnumerable<PartialObject<T>> Execute(IReadableRepository<T> repository);
}
```
- Pro: Flexible, chainable
- Con: Different execution pattern than standard Read methods

## Candidate API Shapes

### Candidate A: Exposer Selection + PartialObject

```csharp
// Fluent builder for selecting fields
public class FieldSelection<T> where T : class
{
    public FieldSelection<T> Add<TField>(Exposer<T, TField> exposer);
    public bool Contains<TField>(Exposer<T, TField> exposer);
    public IReadOnlyList<FieldExposer> SelectedFields { get; }
}

// Factory method
FieldSelection.For<Person>()
    .Add(PersonExposers.Name)
    .Add(PersonExposers.Age);
```

### Partial Object Result

```csharp
public class PartialObject<T> where T : class
{
    public F GetValue<F>(FieldExposer<T, F> exposer);
    public IEnumerable<F> GetValues<F>(FieldExposer<T, IEnumerable<F>> collectionExposer);
    public bool TryGetValue<F>(FieldExposer<T, F> exposer, out F value);
    public bool TryGetValues<F>(
        FieldExposer<T, IEnumerable<F>> collectionExposer,
        out IEnumerable<F> values);
    public bool Contains<F>(FieldExposer<T, F> exposer);
}
```

### Candidate B: Lambda Projection + DTO/Anonymous Type

```csharp
public interface IPartialReadableRepository<T> : IReadableRepository<T> where T : class
{
    IEnumerable<TResult> ReadPartial<TResult>(
        FilterElement<T> filter,
        Expression<Func<T, TResult>> selector);

    IEnumerable<TResult> ReadPartial<TResult>(
        IQuery<T> query,
        Expression<Func<T, TResult>> selector);
}
```

### Candidate C: IPartialQuery<T> Wrapper

```csharp
public interface IPartialQuery<T>
{
    IQuery<T> BaseQuery { get; }
    IReadOnlyList<FieldExposer> SelectedFields { get; }
}

public static class PartialQueryExtensions
{
    public static IPartialQuery<T> WithFields<T>(
        this IQuery<T> query,
        params FieldExposer[] selectedFields)
    {
        return new PartialQuery<T>(query, selectedFields);
    }
}

public interface IPartialReadableRepository<T> : IReadableRepository<T> where T : class
{
    IEnumerable<PartialObject<T>> ReadPartial(IPartialQuery<T> query);
}

public interface IPartialPageableRepository<T> : IPageableRepository<T> where T : class
{
    IEnumerable<IPagedResult<PartialObject<T>>> ReadPagedPartial(IPartialQuery<T> query);
    IEnumerable<IPagedResult<PartialObject<T>>> ReadPagedPartial(IPartialQuery<T> query, int pageSize);
}
```
- Pro: Keeps `IQuery<T>` unchanged while supporting fluent query composition
- Pro: Reuses exposers end-to-end and aligns with `WithFilter` style usage
- Con: Introduces extra wrapper types/interfaces
- Con: Requires explicit support in repository and middleware layers
- Current preference: Slightly preferred over a separately constructed field-selection object.

### Repository Integration Options

The current preference is to keep the regular and paged partial-read capabilities in separate interfaces. The following illustrates the direction; exact inheritance and overloads remain open.

```csharp
public interface IPartialReadableRepository<T> : IReadableRepository<T> where T : class
{
    IEnumerable<PartialObject<T>> ReadPartial(IPartialQuery<T> query);
}
```

```csharp
public interface IPartialPageableRepository<T> : IPageableRepository<T> where T : class
{
    IEnumerable<IPagedResult<PartialObject<T>>> ReadPagedPartial(IPartialQuery<T> query);
    IEnumerable<IPagedResult<PartialObject<T>>> ReadPagedPartial(IPartialQuery<T> query, int pageSize);
}
```

The separate capability interfaces avoid adding partial-read members to existing repository interfaces. Whether the paged interface should also inherit `IPartialReadableRepository<T>` remains to be decided.

### Usage Example

```csharp
// Apply filter as usual
var filter = PersonExposers.Status.Equal(Status.Active)
    .AND(PersonExposers.Age.GreaterThan(18));

var query = filter.ToQuery()
    .OrderBy(PersonExposers.Name)
    .Limit(100);

var partialQuery = query.WithFields(
    PersonExposers.Name,
    PersonExposers.Age,
    PersonExposers.Status);

var results = partialRepository.ReadPartial(partialQuery);
foreach (var person in results)
{
    var name = person.GetValue(PersonExposers.Name);
    var age = person.GetValue(PersonExposers.Age);
    var status = person.GetValue(PersonExposers.Status);

    if (person.TryGetValues(PersonExposers.Tags, out var tags))
    {
        // The collection field was selected; tags may still be null.
    }

    bool hasEmail = person.Contains(PersonExposers.Email);
}

var pages = partialPageableRepository.ReadPagedPartial(partialQuery, pageSize: 100);
```

## Discussion Points

1. **Field Selection Syntax**: The current slight preference is an `IPartialQuery<T>` created through `WithFields(...)`; the alternatives are a separate field-selection object and lambda projection.

1. **Return Type**: The current slight preference is `PartialObject<T>` rather than projecting directly to a DTO.

1. **API Integration**: The current slight preference is for separate interfaces for regular and paged partial reads, with `ReadPartial` and `ReadPagedPartial` method names.

1. **Accessor Semantics**: Confirm how `GetValue`/`GetValues` behave when a field was not selected. `TryGetValue`/`TryGetValues` and `Contains` provide ways to test selection without retrieving the value. A selected field whose value is null must remain distinguishable from a field that was not selected.

1. **Middleware Support**: How should `IReadableMiddleware<T>` handle partial reads?
   - New OnRead overload with FieldSelection parameter?
   - Middleware sees PartialObject instead of T?

1. **Collection Fields**: The proposed `GetValues`/`TryGetValues` methods support collection exposers such as `PersonExposers.Tags`. Their retrieval and performance semantics still need to be specified.

1. **Projection Limits**: If lambda projection is supported, which expression patterns are guaranteed?
    - Simple member projections only?
    - Constructor calls and object initializers?
    - Method calls and computed values?

## Implementation Considerations

1. **In-Memory Execution**: `IQuery.ExecuteInMemory` needs to handle field selection
1. **Validation**: Check at compile-time or runtime that selected fields exist?

## Open Questions

1. Should we support selecting nested properties (e.g., `Person.Address.City`)?
1. Should `IPartialPageableRepository<T>` also inherit `IPartialReadableRepository<T>`?
1. Should `IPartialQuery<T>` support construction from a filter directly, or should filters be converted to `IQuery<T>`?
1. How to handle versioning if additional Exposer metadata is needed?

## References

- Current IReadableRepository interface
- DomInstance SelectedFields implementation (other system)
- [C# Select Projection](https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.select)
- Entity Framework Select patterns

## Conclusion

The latest discussion establishes a direction, not a finalized API: prefer `IPartialQuery<T>` for carrying the query and selected fields, `PartialObject<T>` for results (including typed scalar and collection accessors), and two distinct repository capabilities for `ReadPartial` and `ReadPagedPartial`. Detailed signatures and edge-case behavior remain open.
