
# Proposal: Subscription Mechanism for Repository Change Notifications

**Status:** Draft

## Summary
This proposal describes a subscription mechanism for repository change notifications, enabling consumers to react to entity changes (add, update, delete) in a flexible, thread-safe, and repository-agnostic manner. The approach is opt-in, supports filtering, and abstracts away the underlying transport (e.g., SLNet, NATS), allowing for future extensibility and consistent usage across repositories.

## Requirements & Constraints
- Must be compatible with .NET Framework 4.8 and C# 7.3.
- Subscription mechanism is opt-in per repository (not forced globally).
- Must be thread-safe for concurrent use.
- Must support correct disposal and lifetime management.
- Should be extensible for new notification types or transports.

## Motivation
Many solutions require the ability to react to changes in repository data, such as when a new person is added or an existing entity is updated. A robust subscription mechanism enables event-driven workflows, improves responsiveness, and decouples consumers from the underlying data source. By abstracting the transport, consumers can subscribe without knowledge of whether changes originate from SLNet, NATS, or other mechanisms.

## Use Cases
- Notify when a new entity (e.g., Person) is added to a repository.
- React to property changes (e.g., a Person's name or status changes).
- Subscribe to specific state transitions (e.g., Person status changes from Active to Deprecated).
- Support multiple, concurrent subscriptions with different filters.
- Enable opt-in subscriptions for repositories that support change notifications.

## Decision Points
1. **Subscription API Shape**
   - Option A: Introduce a `.Subscribe(FilterElement<T> filter)` method on repositories via a new interface.
     - *Pros:* Familiar, explicit opt-in, easy to document.
     - *Cons:* Requires interface changes, may not fit all repository types.
   - Option B: Provide a standalone subscription manager/service.
     - *Pros:* Decouples subscription logic from repositories, reusable.
     - *Cons:* More indirection, may complicate usage.

2. **Filter Specification**
   - Option A: Use `FilterElement<T>` to specify interest.
     - *Pros:* Reuses existing filtering logic, flexible.
     - *Cons:* May be too generic or complex for some scenarios.
   - Option B: Define a dedicated subscription filter type.
     - *Pros:* Tailored to subscription needs, can evolve independently.
     - *Cons:* More code to maintain, potential duplication.

3. **Notification Granularity**
   - Option A: Notify on any change (add/update/delete).
     - *Pros:* Simple, covers all cases.
     - *Cons:* May generate too many notifications.
   - Option B: Allow filtering by change type or state transition.
     - *Pros:* Reduces noise, more targeted.
     - *Cons:* More complex to implement and configure.

4. **Change Source Abstraction**
   - Option A: Abstract over transport (SLNet, NATS, etc.) so consumers are unaware of the source.
     - *Pros:* Decouples consumers, easier to swap backends.
     - *Cons:* May limit access to source-specific features.

5. **Subscription Lifetime Management**
   - Option A: Subscriptions are disposable and must be explicitly disposed.
     - *Pros:* Predictable resource management.
     - *Cons:* Risk of leaks if not disposed.
   - Option B: Support both short-lived and long-lived subscriptions, possibly with auto-expiry or weak references.
     - *Pros:* Flexible, safer for long-running apps.
     - *Cons:* More complex lifecycle management.

6. **Thread Safety**
   - Option A: Require all subscription APIs to be thread-safe.
     - *Pros:* Safe for concurrent use.
     - *Cons:* May add implementation overhead.


7. **Extensibility**
   - Option A: Design for future notification types (e.g., batch changes, custom events, or user-defined subscription objects).
     - *Pros:* Future-proof, adaptable, empowers advanced scenarios.
     - *Cons:* May require more abstraction upfront.

8. **Custom Subscription Objects**
   - Option A: Allow users to define their own subscription types (not just filters), e.g., subscribing to specific domain events or complex object relationships.
     - *Pros:* Enables advanced, domain-specific scenarios (e.g., state transitions, child object changes).
     - *Cons:* Increases API surface and complexity.


## Usage Examples
### Option 3: User-Defined Subscription Object
```csharp
// Example: Custom subscription for state transitions
public class StateTransitionSubscription : ISubscription<Person>
{
  public Status From { get; }
  public Status To { get; }
  public Action<Person, Status, Status> Callback { get; }

  public StateTransitionSubscription(Status from, Status to, Action<Person, Status, Status> callback)
  {
    From = from;
    To = to;
    Callback = callback;
  }

  public bool ShouldNotify(Person person, Status oldStatus, Status newStatus)
  {
    return oldStatus == From && newStatus == To;
  }
}

// Usage:
var stateTransitionSubscription = repository.Subscribe(
  new StateTransitionSubscription(Status.Active, Status.Deprecated, (person, oldStatus, newStatus) =>
  {
    // Custom logic for when a person transitions from Active to Deprecated
  }));

// Example: Custom subscription for when a node is added to a job
public class NodeAddedToJobSubscription : ISubscription<Job>
{
  public Action<Job, Node> Callback { get; }

  public NodeAddedToJobSubscription(Action<Job, Node> callback)
  {
    Callback = callback;
  }

  public bool ShouldNotify(Job job, Node addedNode)
  {
    // Custom logic to determine if notification should be sent
    return true;
  }
}

// Usage:
var nodeAddedSubscription = repository.Subscribe(
  new NodeAddedToJobSubscription((job, node) =>
  {
    // Custom logic for when a node is added to a job
  }));
```

### Option 1: Repository-Based Subscription (Interface Method)
```csharp
// Directly on the repository via a new interface
var subscription = repository.Subscribe(
  new FilterElement<Person>(p => p.Status == Status.Active),
  OnPersonChanged);
```

### Option 2: Central Subscription Service
```csharp
// Using a standalone subscription manager/service
var subscription = subscriptionService.Subscribe(
  repository,
  new FilterElement<Person>(p => p.Status == Status.Active),
  OnPersonChanged);
```

// Both approaches return a disposable subscription object:
subscription.Dispose();

// Additional example: Filtering by change type
```csharp
// Only interested in added or deleted persons
var addDeleteSubscription = repository.Subscribe(
  new ChangeTypeFilter(ChangeType.Added | ChangeType.Deleted),
  OnPersonAddedOrDeleted);
```

// Example: Subscribing to a property change
```csharp
var nameChangeSubscription = repository.Subscribe(
  new PropertyChangedFilter<Person>(nameof(Person.Name)),
  OnPersonNameChanged);
```

## Discussion Points
- Should the API support user-defined subscription objects for advanced scenarios?
- How granular should the filtering and notification mechanism be (e.g., property-level, state transitions)?
- Should subscriptions be managed per repository, or via a central service?
- How to ensure correct disposal and avoid resource leaks?
- What is the minimum thread-safety guarantee?
- How to support future extensibility for new notification types?
