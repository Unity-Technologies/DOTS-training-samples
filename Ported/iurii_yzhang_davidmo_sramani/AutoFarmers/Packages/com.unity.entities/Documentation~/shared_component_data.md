---
uid: ecs-shared-component-data
---
# Shared Component Data

Shared components are a special kind of data component that you can use to subdivide entities based on the specific values in the shared component (in addition to their archetype). When you add a shared component to an entity, the EntityManager places all entities with the same shared data values into the same Chunk. Shared components allow your systems to process like entities  together. For example, the shared component `Rendering.RenderMesh`, which is part of the Hybrid.rendering package, defines several fields, including **mesh**, **material**, **receiveShadows**, etc. When rendering, it is most efficient to process all the 3D objects that all have the same value for those fields together. Because these properties are specified in a shared component the EntityManager places the matching entities together in memory so the rendering system can efficiently iterate over them. 

**Note:** Over using shared components can lead to poor Chunk utilization since it involves a combinatorial expansion of the number of memory Chunks required based on archetype and every unique value of each shared component field. Avoid adding unnecessary fields to a shared component Use the [Entity Debugger](ecs_debugging.md) to view the current Chunk utilization.
 
If you add or remove a component from an entity, or change the value of a SharedComponent, The EntityManager moves the entity to a different Chunk, creating a new Chunk if necessary.


[IComponentData](xref:Unity.Entities.IComponentData) is generally appropriate for data that varies between entities, such as storing a world position, agent hit points, particle time-to-live, etc. In contrast, [ISharedComponentData](xref:Unity.Entities.ISharedComponentData) is appropriate when many entities share something in common. For example in the `Boid` demo we instantiate many entities from the same [Prefab](https://docs.unity3d.com/Manual/Prefabs.html) and thus the `RenderMesh` between many `Boid` entities is exactly the same. 

```cs
[System.Serializable]
public struct RenderMesh : ISharedComponentData
{
    public Mesh                 mesh;
    public Material             material;

    public ShadowCastingMode    castShadows;
    public bool                 receiveShadows;
}
```

The great thing about `ISharedComponentData` is that there is literally zero memory cost on a per entity basis.

We use `ISharedComponentData` to group all entities using the same `InstanceRenderer` data together and then efficiently extract all matrices for rendering. The resulting code is simple & efficient because the data is laid out exactly as it is accessed.

- `RenderMeshSystemV2` (see file:  _Packages/com.unity.entities/Unity.Rendering.Hybrid/RenderMeshSystemV2.cs_)

## Some important notes about SharedComponentData:

- Entities with the same `SharedComponentData` are grouped together in the same [Chunks](chunk_iteration.md). The index to the `SharedComponentData` is stored once per `Chunk`, not per entity. As a result `SharedComponentData` have zero memory overhead on a per entity basis. 
- Using `EntityQuery` we can iterate over all entities with the same type.
- Additionally we can use [EntityQuery.SetFilter()](xref:Unity.Entities.EntityQuery.SetFilter*) to iterate specifically over entities that have a specific `SharedComponentData` value. Due to the data layout this iteration has low overhead.
- Using `EntityManager.GetAllUniqueSharedComponents` we can retrieve all unique `SharedComponentData` that is added to any alive entities.
- `SharedComponentData` are automatically [reference counted](https://en.wikipedia.org/wiki/Reference_counting).
- `SharedComponentData` should change rarely. Changing a `SharedComponentData` involves using [memcpy](https://msdn.microsoft.com/en-us/library/aa246468(v=vs.60).aspx) to copy all `ComponentData` for that entity into a different `Chunk`.

