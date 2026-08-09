# Change Tracking — Proposal

## Problem Statement

When working with SDM domain objects in repositories, there is currently no built-in way to know
**what changed** on an entity between the time it was read and the time it is written back. This leads
to several recurring pain points:

- **Blind updates** — every `Update` call sends the full entity regardless of whether anything changed,
  placing unnecessary load on the underlying system.
- **No audit trail** — there is no easy way to record which fields changed, by how much, and when.
- **Manual diffing** — developers write ad-hoc comparison code that is fragile, untested, and scattered
  across the codebase.
- **Missed optimisations** — partial updates (e.g. patching only dirty properties) are impossible without
  knowing which properties are dirty.

These problems compound as the number of entities and update paths grows.

> **Comments / conclusions:**
> 

---

## Goal

Provide a **first-class, type-safe change tracking facility** for SDM domain objects that:

- Requires **no reflection** at runtime
- Produces **no boxing** for value types
- Is **refactor-safe** — renaming a property is caught at compile time
- Is approachable to use — the end-user API should be intuitive and consistent
- Can be **incrementally adopted** — teams opt in, nothing breaks existing code

> **Comments / conclusions:**
> 

---

## Background — What Already Exists

The source generator already emits **Exposers** for every entity annotated with `[GenerateExposers]`.
An Exposer is a typed, cached accessor:

```csharp
// Generated
public static class PersonExposers
{
    public static readonly Exposer<Person, string> Name;
    public static readonly Exposer<Person, int>    Age;
}

// Usage — no reflection, no strings, refactor-safe
PersonExposers.Name.internalFunc(person);
```

Change tracking is a natural next step: if we can already get properties in a typed way,
we can snapshot them and compare them later — entirely at compile time.

> **Comments / conclusions:**
> 

---

## Proposed Approach

The core idea is to generate a small set of companion types alongside the existing Exposers.
For an entity `Person`, the generator would emit:

| Type | Responsibility |
|---|---|
| `PersonSnapshot` | Immutable point-in-time copy of a `Person` |
| `PersonDiffer` | Stateless, pure comparison — `Diff(original, current)` |
| `PersonChangeSet` | The result of a diff — per-property original/current values + `HasChanges` |
| `PersonTracker` | Stateful wrapper — snapshot taken on `Track()`, exposes current diff at any time |

These types could rely on a small set of **shared non-generated infrastructure** that we could place in the abstractions:

```csharp
TrackedProperty<TEntity, TValue>          // OriginalValue, CurrentValue, IsModified
CollectionChangeSet<TEntity, TChangeSet>  // Added, Removed (by id), Modified
```

The sections below describe the key design decisions that are still open for discussion.

> **Comments / conclusions:**
> 

---

## End-User API

This is the most important part. Regardless of which options are chosen, the API should feel easy and intuitive.

To match the proposed approach, the API is split per generated type.

### 1) `PersonSnapshot` API options

Represents a point-in-time immutable copy of an entity.

#### A) Static factory on snapshot type *(current lean)*

```csharp
PersonSnapshot snapshot = PersonSnapshot.Capture(person);
```

**Pros:** Discoverable, explicit, easy to search for.  
**Cons:** Slightly more verbose at call sites.

#### B) Extension method on entity

```csharp
PersonSnapshot snapshot = person.ToSnapshot();
```

**Pros:** Fluent and short. Reads naturally in pipelines.  
**Cons:** Less obvious where the method comes from.

#### C) Snapshot via tracker baseline

```csharp
PersonTracker tracked = person.Track();
PersonSnapshot snapshot = tracked.Original;
```

**Pros:** One entry point for stateful flows.  
**Cons:** Indirect for stateless scenarios (tests, migrations, replay).

### 2) `PersonDiffer` API options

Stateless comparison logic. Suitable for auditing, testing, and middleware.

#### A) Static `Diff` overloads *(current lean)*

```csharp
PersonChangeSet c1 = PersonDiffer.Diff(snapshot, person);
PersonChangeSet c2 = PersonDiffer.Diff(personA, personB);
PersonChangeSet c3 = PersonDiffer.Diff(snapshotA, snapshotB);
```

**Pros:** No allocations for differ instance, simple mental model.  
**Cons:** Harder to inject custom options later.

#### B) Instance differ with optional settings

```csharp
var differ = new PersonDiffer(new PersonDiffOptions
{
  IgnoreCaseForStrings = true,
});

PersonChangeSet changes = differ.Diff(original, current);
```

**Pros:** Extensible for future knobs without overload explosion.  
**Cons:** More ceremony for the common case.

#### C) Try-pattern for low-allocation call sites

```csharp
if (PersonDiffer.TryDiff(original, current, out PersonChangeSet changes))
{
  // Only enters when there are actual changes.
}
```

**Pros:** Fast path for unchanged entities, useful in hot middleware loops.  
**Cons:** Additional API surface and branch-style usage.

### 3) `PersonChangeSet` API options

Represents diff results with per-property details and aggregate state.

Example legend used below:

- `changes`: the full diff result (`PersonChangeSet`) between two `Person` instances.
- `changes.Name`: the diff node for the `Name` property (`TrackedProperty<Person, string>`).
- `change`: one item from an enumerable of modified properties (`IPropertyChange`).

#### A) Generated strongly typed property bag *(current lean)*

Direct property access on the generated change set.

```csharp
PersonChangeSet changes = PersonDiffer.Diff(original, current);

if (changes.HasChanges)
{
  Console.WriteLine(changes.Name.OriginalValue);
  Console.WriteLine(changes.Name.CurrentValue);
  Console.WriteLine(changes.Name.IsModified);
}
```

Use when business logic checks known fields.

**Pros:** Maximum type safety and IntelliSense.  
**Cons:** Can feel verbose for generic logging.

#### B) Typed + enumerable view

Keep typed access and add a generic iterator for infrastructure scenarios.

```csharp
PersonChangeSet changes = PersonDiffer.Diff(original, current);

foreach (IPropertyChange change in changes.GetModifiedProperties())
{
  Console.WriteLine($"{change.PropertyName}: {change.OriginalValue} -> {change.CurrentValue}");
}
```

Use when you need both domain code and generic logging/audit processing.

**Pros:** Great for logging/audit sinks and generic middleware.  
**Cons:** Interface/object view may introduce boxing for value types unless carefully designed.

#### C) Tiered shape: `HasChanges` + lazy property materialization

Two-phase usage: cheap check first, detailed nodes only when requested.

```csharp
PersonChangeSet changes = PersonDiffer.Diff(original, current);

if (!changes.HasChanges)
{
  return;
}

TrackedProperty<Person, string> name = changes.Name;
```

Use when no-change is common and you want the fastest early-exit path.

**Pros:** Keeps hot path cheap while preserving full detail on demand.  
**Cons:** More complex implementation and generated code paths.

#### Practical recommendation

If we optimize for clarity first, option A is the simplest API to teach and document.
If we know audit/middleware scenarios are first-class from day one, option B gives the best balance.
Option C is mostly a performance optimization and is best introduced only if profiling shows the need.

Collection properties would still expose recursive diffs when tracked:

```csharp
CollectionChangeSet<Address, AddressChangeSet> addressChanges = changes.Addresses;

foreach (Address added in addressChanges.Added) { }
foreach (string removedId in addressChanges.Removed) { }
foreach (ItemChange<Address, AddressChangeSet> modified in addressChanges.Modified)
{
  Console.WriteLine(modified.Changes.Street.OriginalValue);
  Console.WriteLine(modified.Changes.Street.CurrentValue);
}
```

### 4) `PersonTracker` API options

Stateful helper that captures original state and computes current changes.

#### A) Entity-centric tracker *(current lean)*

```csharp
PersonTracker tracked = person.Track();

tracked.Entity.Name = "Alice";

PersonChangeSet changes = tracked.GetChanges();
bool dirty = tracked.HasChanges;
```

**Pros:** Intuitive workflow for update pipelines.  
**Cons:** Requires users to work through `tracked.Entity`.

#### B) Tracker proxy exposing members directly

```csharp
PersonTracker tracked = person.Track();

tracked.Name = "Alice";
tracked.Age = 30;

bool isNameDirty = tracked.NameChange.IsModified;
```

**Pros:** Convenient call-site ergonomics.  
**Cons:** Larger generated surface and potential confusion between entity members and change metadata.

#### C) Hybrid tracker + middleware hand-off

```csharp
PersonTracker tracked = person.Track();

tracked.Entity.Name = "Bob";

repo.Update(tracked); // Middleware can consume tracker directly
```

**Pros:** Smooth integration in repository flows; avoids re-snapshotting at write time.  
**Cons:** Couples repository contracts to tracking concepts unless carefully abstracted.

Future middleware integration remains possible for all options:

```csharp
// The middleware snapshots entities on read automatically.
// On update it diffs, skips unchanged entities, and only forwards dirty ones.
var people = repo.ReadAll();

people[0].Name = "Bob";

repo.Update(people); // Only person[0] is actually sent downstream
```

> **Comments / conclusions:**
> 

---

## Design Options

The following are the key decisions to be made. Each option is open for discussion.

---

### Option 1 — What triggers generation?

#### A) `[GenerateExposers]` drives everything *(current lean)*

The existing attribute already marks an entity for code generation.
Change tracking artifacts are generated automatically alongside Exposers.

**Pros:** Zero extra ceremony. Every entity with Exposers gets tracking for free.  
**Cons:** Entities that don't need tracking still pay the cost of generated types. Less explicit intent.

#### B) Separate `[GenerateChangeTracking]` attribute

A second opt-in attribute is placed on entities that need change tracking.

**Pros:** Explicit. Only entities that need it get it. Easier to scope initial rollout.  
**Cons:** Two attributes to maintain. Risk of forgetting to add it.

#### C) Flag on existing attribute

```csharp
[GenerateExposers(ChangeTracking = true)]
```

**Pros:** Single attribute, opt-in per entity, discoverable.  
**Cons:** Slightly more verbose than option A. Requires attribute parameter support in the generator.

> **Comments / conclusions:**
> 

---

### Option 2 — Snapshot representation

The snapshot needs to capture all tracked property values at a point in time.

#### A) Generated sealed class *(current lean)*

```csharp
public sealed class PersonSnapshot
{
    private PersonSnapshot(Person entity) { ... }

    public static PersonSnapshot Capture(Person entity) => new PersonSnapshot(entity);

    public string Name { get; }
    public int    Age  { get; }
}
```

**Pros:** Works for any entity including those with collection properties. Simple deep copy.  
**Cons:** Heap allocation per snapshot.

#### B) Generated readonly struct (flat entities only)

```csharp
public readonly struct PersonSnapshot
{
    public string Name { get; }
    public int    Age  { get; }
}
```

**Pros:** Stack-allocated, zero GC pressure for simple entities.  
**Cons:** Cannot hold a deep-copied `List<T>` safely. Generator must detect whether the entity is "flat".
  Copying a struct snapshot by accident copies all fields — potentially surprising.

#### C) Let the generator decide

Emit `readonly struct` for flat entities, `sealed class` for entities with collections.

**Pros:** Optimal for each case.  
**Cons:** More generator complexity. Two different shapes for the same concept — may surprise users.

> **Comments / conclusions:**
> 

---

### Option 3 — Tracker and Differ: one or both?

#### A) Differ only

Only emit `PersonDiffer` and `PersonChangeSet`. The user manages snapshots manually.

**Pros:** Minimal surface area. Purely functional — easy to test and reason about.  
**Cons:** More ceremony for the stateful use case. User must hold a reference to the snapshot.

#### B) Tracker wrapper only

Only emit `PersonTracker`. The differ logic lives inside it.

**Pros:** One type to learn.  
**Cons:** Cannot diff two independently loaded instances without wrapping one. Cannot diff snapshots
  from audit logs or migrations.

#### C) Both, where Tracked delegates to Differ *(current lean)*

```
PersonSnapshot  ←  PersonTracker  →  PersonDiffer  →  PersonChangeSet
    (data)           (lifetime)         (logic)           (result)
```

**Pros:** Each type has a single responsibility. Differ is reusable standalone. Tracked is convenient
  for the common stateful case.  
**Cons:** Slightly larger generated surface area.

> **Comments / conclusions:**
> 

---

### Option 4 — Collection diffing

For entities with `List<T>` properties where `T` is also a tracked type:

#### A) Always diff collections recursively *(current lean)*

The differ traverses into child collections, matching items by their `Identifier`.

**Pros:** Full picture of what changed. Works naturally for nested domain models.  
**Cons:** More generated code. Items must have a stable key (`Identifier`).

#### B) Collections are excluded by default, opt-in per property

```csharp
[TrackCollection]
public List<Address> Addresses { get; set; }
```

**Pros:** Simple base case. Avoids surprises for large collections.  
**Cons:** More attributes. Risk of forgetting to opt in.

#### C) Collections are shallow — only detect whether the list reference changed

Report the collection as "changed" or "unchanged" as a whole, without per-item detail.

**Pros:** Very simple to generate. No key matching required.  
**Cons:** Not actionable — caller cannot tell what was added, removed or modified.

#### D) Custom differ via attribute per collection

Allow a collection property to point to a user-provided differ type.

```csharp
[CollectionDiffer(typeof(AddressCollectionDiffer))]
public List<Address> Addresses { get; set; }
```

The differ type would implement a known contract, for example:

```csharp
public interface ICollectionDiffer<TItem, TChangeSet>
{
  CollectionChangeSet<TItem, TChangeSet> Diff(IReadOnlyList<TItem> original, IReadOnlyList<TItem> current);
}
```

**Pros:** Supports domain-specific matching and change rules (composite keys, tolerance, reorder handling).  
**Cons:** More complexity in generator validation and API surface. Requires clear constraints on allowed differ types.

> **Comments / conclusions:**
> 

---

### Option 5 — How items are matched in a collection diff

When diffing two lists, we need to know which item in the original corresponds to which item in the current.

#### A) By `SdmObject<T>.Identifier` *(current lean)*

All SDM entities already inherit from `SdmObject<T>` and have a stable `Identifier`.

**Pros:** Zero configuration. Works out of the box for all SDM types.  
**Cons:** Assumes every tracked collection item is an SDM object. Does not cover plain POCOs.

#### B) Configurable key via attribute

```csharp
[CollectionKey(nameof(Address.PostalCode))]
public List<Address> Addresses { get; set; }
```

**Pros:** Flexible. Works for non-SDM types or when `Identifier` is not the right match key.  
**Cons:** More configuration. Generator must read and validate attribute arguments.

#### C) Custom key selector via attribute type

Allow a collection property to specify a user-provided key selector type.

```csharp
[CollectionKeySelector(typeof(AddressKeySelector))]
public List<Address> Addresses { get; set; }
```

The selector type would implement a known contract, for example:

```csharp
public interface ICollectionKeySelector<TItem, TKey>
{
  TKey GetKey(TItem item);
}
```

**Pros:** Supports advanced key strategies (composite keys, normalization, fallback keys) without hard-coding generator rules.  
**Cons:** Adds another extension point to validate and document. Requires stable equality semantics for `TKey`.

> **Comments / conclusions:**
> 

---

## Constraints

- Target framework: `net48`, C# 7.3
- No reflection, no expression compilation at runtime
- Must not break existing repository or filter/query code

---

## Future Possibilities

Once the foundation is in place, the same generator walk can produce:

- patch statements (only dirty columns)
- Audit log entries (who changed what, when)
- Domain events (raise an event only if a specific property changed)
- Validation metadata
- Mapping metadata

**Honourable mention — ETags:** a snapshot naturally contains everything needed to generate a short hash
representing the entity's state at a point in time. This could serve as a cheap dirty-check, a cache
invalidation token, or an optimistic concurrency token on repository writes — complementary to, not a
replacement for, the detailed diff.

> **Comments / conclusions:**
> 
