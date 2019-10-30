---
uid: ecs-components
---
# Components

Components are one of the three principle elements of an Entity Component System architecture. They represent the data of your game or program. [Entities](ecs_entities.md) are essentially identifiers that index your collections of components. [Systems](ecs_systems.md) provide the behavior. 

Concretely, a component in ECS is a struct with one of the following "marker interfaces":

* [IComponentData](xref:Unity.Entities.IComponentData) — use for [general purpose](xref:ecs-component-data) and [chunk components](xref:ecs-chunk-component-data).
* [IBufferElementData](xref:Unity.Entities.IBufferelementData) — use for associating  [dynamic buffers](xref:ecs-dynamic-buffers) with an entity.
* [ISharedComponentData](xref:Unity.Entities.ISharedComponentData) — use to categorize or group entities by value within an archetype. See [Shared Component Data](xref:ecs-shared-component-data).
* [ISystemStateComponentData](xref:Unity.Entities.ISystemStateComponentData) — use for associating system-specific state with an entity and for detecting when individual entities are created or destroyed. See [System State Components](xref:ecs-system-state-component-data).
* [ISharedSystemStateComponentData](xref:Unity.Entities.ISharedSystemStateComponentData) — a combination of shared and system state data. See [System State Components](xref:ecs-system-state-component-data).
* [Blob assets](xref:ecs-blob-asset-data) – while not technically a "component," you can use blob assets to store data. Blob assets can be referenced by one or more components using a [BlobAssetReference](xref:Unity.Entities.BlobAssetReference) and are immutable. Blob assets allow you to share data between assets and access that data in C# Jobs.
 
The EntityManager organizes unique combinations of components appearing on your entities into **Archetypes**. It stores the components of all entities with the same archetype together in blocks of memory called **Chunks**. The entities in a given Chunk all have the same component archetype.

![](images/ArchetypeChunkDiagram.png)

This diagram illustrates how component data is stored in chunks by archetype. Shared components and chunk components are exceptions because they are stored outside the chunk; a single instance of these component types apply to all the entities in the applicable chunks. In addition, you can optionally store dynamic buffers outside the chunk. Even though these types of components are not stored inside the chunk, you generally treat them the same as other component types when querying for entities.

