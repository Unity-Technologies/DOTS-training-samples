---
uid: ecs-component-data
---

# General purpose components

ComponentData in Unity (also known as a component in standard ECS terms) is a struct that contains only the instance data for an [entity](entities.md). ComponentData should not contain methods beyond utility functions for accessing the data in the struct. All game logic and behaviour should be implemented in systems. To put this in terms of the old Unity system, this is somewhat similar to an old Component class, but one that **only contains variables**.

The Unity ECS API provides an interface called [IComponentData](xref:Unity.Entities.IComponentData) that you can implement in your code to declare a general-purpose component type.

## IComponentData

Traditional Unity components (including `MonoBehaviour`) are [object-oriented](https://en.wikipedia.org/wiki/Object-oriented_programming) classes which contain data and methods for behavior. `IComponentData` is a pure ECS-style component, meaning that it defines no behavior, only data. `IComponentData` should be implemented as struct rather than a class, meaning that it is copied [by value instead of by reference](https://stackoverflow.com/questions/373419/whats-the-difference-between-passing-by-reference-vs-passing-by-value?answertab=votes#tab-top) by default. You will usually need to use the following pattern to modify data:

```c#
var transform = group.transform[index]; // Read

transform.heading = playerInput.move; // Modify
transform.position += deltaTime * playerInput.move * settings.playerMoveSpeed;

group.transform[index] = transform; // Write
```

`IComponentData` structs may not contain references to managed objects. Since `ComponentData` lives in simple non-garbage-collected tracked [chunk memory](chunk_iteration.md) allowing for many performance advantages.

### Managed IComponentData

To help porting existing code over to ECS in a piecemeal fashion, interoperating with managed data not suitable in `ISharedComponentData` or when first prototyping what your data layout will look like, it may be helpful to use a managed `IComponentData` (that is, `IComponentData` declared using a `class` rather than `struct`). These components are used the same way as value type `IComponentData` however internally are handled in a much different (and slower) way. Users who do not need managed component support should define `UNITY_DISABLE_MANAGED_COMPONENTS` in their Project Settings -> Player -> Scripting Define Symbols to prevent accidental usage.

By nature of being a managed type, managed `IComponentData` have the following performance drawbacks compared to valuetype `IComponentData`:
* Cannot be used with the Burst Compiler
* Cannot be used in Job structs 
* Cannot use [chunk memory](chunk_iteration.md) 
* Require garbage collection

Users should try to limit the number of managed components, preferring blittable types as much as possible. 

Managed `IComponentData` must implement the `IEquatable<T>` interface and override for `Object.GetHashCode()`. As well, for serialization purposes, managed components must be default constructible.

Since managed components are by their nature not blittable, these components are stored in a managed C# array known indirectly by each `ArchetypeChunk` and are indexed to by `Entity`. 

You must set the value of the component on the main thread using either the  `EntityManager` or `EntityCommandBuffer`. Being a reference type, you may change value of the component without moving entities across chunks
unlike [ISharedComponentData](xref:Unity.Entities.ISharedComponentData) and thus does not create a sync-point. However while logically stored separate from value-type components, 
managed components still contribute to an entity's `EntityArchetype` definition. As such, adding a new managed component to an entity will still cause a new archetype to be created (if a matching archetype doesn't exist already) and the entity to be moved to a new chunk.

See file: _/Packages/com.unity.entities/Unity.Entities/IComponentData.cs_.



