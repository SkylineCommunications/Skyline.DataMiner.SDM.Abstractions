# Proposal: Partial Read Support

## Status
**Draft** - Under Discussion

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
int age = result.GetValue(PersonExposers.Age);
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
    // Type-safe value retrieval
    public TField GetValue<TField>(Exposer<T, TField> exposer);
    public bool TryGetValue<TField>(Exposer<T, TField> exposer, out TField value);
    
    // Check if field was selected
    public bool HasField<TField>(Exposer<T, TField> exposer);
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
    IReadOnlyList<IExposer> SelectedFields { get; }
}

public static class PartialQueryExtensions
{
    public static IPartialQuery<T> WithFields<T>(
        this IQuery<T> query,
        params IExposer[] selectedFields)
    {
        return new PartialQuery<T>(query, selectedFields);
    }
}

public interface IPartialReadableRepository<T> : IReadableRepository<T> where T : class
{
    IEnumerable<PartialObject<T>> ReadPartial(IPartialQuery<T> query);
}
```
- Pro: Keeps `IQuery<T>` unchanged while supporting fluent query composition
- Pro: Reuses exposers end-to-end and aligns with `WithFilter` style usage
- Con: Introduces extra wrapper types/interfaces
- Con: Requires explicit support in repository and middleware layers

### Repository Integration Options

```csharp
// Option A: Add overloads on IReadableRepository
public interface IReadableRepository<T> where T : class
{
    IEnumerable<T> Read(FilterElement<T> filter);
    IEnumerable<T> Read(IQuery<T> query);
    
    // New overloads for partial reads
    IEnumerable<PartialObject<T>> Read(FilterElement<T> filter, FieldSelection<T> fields);
    IEnumerable<PartialObject<T>> Read(IQuery<T> query, FieldSelection<T> fields);
}
```

```csharp
// Option A2: Capability interface to avoid breaking existing implementations
public interface IPartialReadableRepository<T> : IReadableRepository<T> where T : class
{
    IEnumerable<PartialObject<T>> ReadPartial(FilterElement<T> filter, FieldSelection<T> fields);
    IEnumerable<PartialObject<T>> ReadPartial(IQuery<T> query, FieldSelection<T> fields);
}
```

### Usage Example

```csharp
// Candidate A: Exposer selection
var fields = FieldSelection.For<Person>()
    .Add(PersonExposers.Name)
    .Add(PersonExposers.Age)
    .Add(PersonExposers.Status);

// Apply filter as usual
var filter = PersonExposers.Status.Equal(Status.Active)
    .AND(PersonExposers.Age.GreaterThan(18));

// Read partial objects
var results = repository.Read(filter, fields);

foreach (var person in results)
{
    // Access selected fields
    var name = person.GetValue(PersonExposers.Name);
    var age = person.GetValue(PersonExposers.Age);
    var status = person.GetValue(PersonExposers.Status);
    
    // Trying to access non-selected field throws exception
    // var email = person.GetValue(PersonExposers.Email); // InvalidOperationException
}

// Or with query
var query = PersonExposers.Status.Equal(Status.Active)
    .ToQuery()
    .OrderBy(PersonExposers.Name)
    .Limit(100);

var results = repository.Read(query, fields);

// Candidate B: Lambda projection to DTO
var projected = partialRepository.ReadPartial(
    query,
    p => new PersonListItemDto
    {
        Name = p.Name,
        Age = p.Age,
        Status = p.Status,
    });

// Candidate C / Option C: IPartialQuery + WithFields
var partialQuery = query.WithFields(
    PersonExposers.Name,
    PersonExposers.Age,
    PersonExposers.Status);

var partialResults = partialRepository.ReadPartial(partialQuery);
```

## Discussion Points

1. **Field Selection Syntax**: Which approach feels most "C#-like" or "DataMiner-like"?
   - Exposer-based (type-safe, consistent with DOM API)
    - Lambda projection (very natural for end users, difficult to build. This could be included in the Linq package)
    - Fluent query extension (`WithFields(...)`)

1. **Return Type**: Should we return `PartialObject<T>` or something else?
   - Pro: Clear distinction from full objects, prevents accidental property access
   - Con: Additional API surface, different programming model
    - Alternative: Project directly to DTO using lambda for a more idiomatic consumer experience

1. **API Integration**: How should partial reads be exposed?
    - New overloads on IReadableRepository (clean for consumers, but breaking for implementers)
    - New capability interface such as IPartialReadableRepository (non-breaking for existing implementers)
    - IPartialQuery wrapper (keeps IQuery untouched, adds dedicated partial-query type)
   - Extension methods with wrapper types (flexible but more complex)
   - Builder pattern (different execution model)

1. **Error Handling**: What happens when accessing non-selected fields?
   - Throw exception (fail-fast, explicit)
   - Return default value (silent, could hide bugs)
   - Return Option<T> or nullable (explicit optionality)

1. **Middleware Support**: How should `IReadableMiddleware<T>` handle partial reads?
   - New OnRead overload with FieldSelection parameter?
   - Middleware sees PartialObject instead of T?

1. **Collection Fields**: Should we support selecting collection exposers?
   - `PersonExposers.Tags` (collection of strings)
   - Could be expensive, might defeat purpose of partial reads

1. **Projection Limits**: If lambda projection is supported, which expression patterns are guaranteed?
    - Simple member projections only?
    - Constructor calls and object initializers?
    - Method calls and computed values?

## Implementation Considerations

1. **In-Memory Execution**: `IQuery.ExecuteInMemory` needs to handle field selection
1. **Validation**: Check at compile-time or runtime that selected fields exist?

## Open Questions

1. Should `PartialObject<T>` be convertible to `T` if all fields are selected?
1. Should we support selecting nested properties (e.g., `Person.Address.City`)?
1. What's the behavior for fields that are null vs fields that weren't selected?
1. Should FieldSelection be immutable (functional style) or mutable (builder style)?
1. How to handle versioning if additional Exposer metadata is needed?
1. If lambda projection is chosen, should unsupported expressions fail at compile-time (analyzers) or runtime?

## References

- Current IReadableRepository interface
- DomInstance SelectedFields implementation (other system)
- [C# Select Projection](https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.select)
- Entity Framework Select patterns
