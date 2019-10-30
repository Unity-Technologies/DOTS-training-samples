---
uid: ecs-system-state-component-data
---
# System State Components

The purpose of `SystemStateComponentData` is to allow you to track resources internal to a system and have the opportunity to appropriately create and destroy those resources as needed without relying on individual callbacks.

`SystemStateComponentData` and `SystemStateSharedComponentData` are exactly like `ComponentData` and `SharedComponentData`, respectively, except in one important respect:

1. `SystemStateComponentData` is not deleted when an entity is destroyed.

`DestroyEntity` is shorthand for

1. Find all components which reference this particular entity ID.
2. Delete those components.
3. Recycle the entity id for reuse.

However, if `SystemStateComponentData` is present, it is not removed. This gives a system the opportunity to cleanup any resources or state associated with an entity ID. The entity ID will only be reused once all `SystemStateComponentData` has been removed.

## Motivation

- Systems may need to keep an internal state based on `ComponentData`. For instance, resources may be allocated. 
- Systems need to be able to manage that state as values and state changes are made by other systems. For example, when values in components change, or when relevant components are added or deleted.
- "No callbacks" is an important element of the ECS design rules.

## Concept

The general use of  `SystemStateComponentData` is expected to mirror a user component, providing the internal state.

For instance, given:
- FooComponent (`ComponentData`, user assigned)
- FooStateComponent (`SystemComponentData`, system assigned)

### Detecting Component Add

When user adds FooComponent, FooStateComponent does not exist. The FooSystem update queries for FooComponent without FooStateComponent and can infer that they have been added. At that point, the FooSystem will add the FooStateComponent and any needed internal state. 

### Detecting Component Remove

When user removes FooComponent, FooStateComponent still exists. The FooSystem update queries for FooStateComponent without FooComponent and can infer that they have been removed. At that point, the FooSystem will remove the FooStateComponent and fix up any needed internal state. 

### Detecting Destroy Entity

`DestroyEntity` is actually a shorthand utility for:

- Find components which reference given entity ID.
- Delete components found.
- Recycle entity ID.

However, `SystemStateComponentData` are not removed on `DestroyEntity` and the entity ID is not recycled until the last component is deleted. This gives the system the opportunity to clean up the internal state in the exact same way as with component removal.

## SystemStateComponent

A `SystemStateComponentData` is analogous to a `ComponentData` and used similarly.

```
struct FooStateComponent : ISystemStateComponentData
{
}
```

Visibility of a `SystemStateComponentData` is also controlled in the same way as a component (using `private`, `public`, `internal`) However, it's expected, as a general rule, that a `SystemStateComponentData` will be `ReadOnly` outside the system that creates it.

## SystemStateSharedComponent

A `SystemStateSharedComponentData` is analogous to a `SharedComponentData` and used similarly.

```
struct FooStateSharedComponent : ISystemStateSharedComponentData
{
  public int Value;
}
```

## Example system using state components

The following example shows a simplified system that illustrates how to manage entities with system state components. The example defines a general-purpose IComponentData instance and a system state, ISystemStateComponentData instance. It also defines three queries based on those entities:

* m_newEntities selects entities that have the general-purpose, but not the system state component. This query finds new entities that the system has not seen before. The system runs a job using the new entities query that adds the system state component.
* m_activeEntities selects entities that have both the general-purpose and the system state component. In a real application, other systems could be the ones that process or destroy the entities.
* m_destroyedEntities selects entities that have the system state, but not the general-purpose component. Since the system state component is never added to an entity by itself, the entities selected by this query must have been deleted, either by this system or another system. The system runs a job using the destroyed entities query to remove the system state component from the entities, which allows the ECS code to recycle the Entity identifier. 

Note that this simplified example does not maintain any state within the system. One purpose for system state components is to track when persistent resources need to be allocated or cleaned up.

<!-- DocCodeSamples.Tests.StatefulSystem.cs-->
```
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public struct GeneralPurposeComponentA : IComponentData
{
    public bool IsAlive;
}

public struct StateComponentB : ISystemStateComponentData
{
    public int State;
}

public class StatefulSystem : JobComponentSystem
{
    private EntityQuery m_newEntities;
    private EntityQuery m_activeEntities;
    private EntityQuery m_destroyedEntities;
    private EntityCommandBufferSystem m_ECBSource;

    protected override void OnCreate()
    {
        // Entities with GeneralPurposeComponentA but not StateComponentB
        m_newEntities = GetEntityQuery(new EntityQueryDesc()
        {
            All = new ComponentType[] {ComponentType.ReadOnly<GeneralPurposeComponentA>()},
            None = new ComponentType[] {ComponentType.ReadWrite<StateComponentB>()}
        });

        // Entities with both GeneralPurposeComponentA and StateComponentB
        m_activeEntities = GetEntityQuery(new EntityQueryDesc()
        {
            All = new ComponentType[]
            {
                ComponentType.ReadWrite<GeneralPurposeComponentA>(),
                ComponentType.ReadOnly<StateComponentB>()
            }
        });

        // Entities with StateComponentB but not GeneralPurposeComponentA
        m_destroyedEntities = GetEntityQuery(new EntityQueryDesc()
        {
            All = new ComponentType[] {ComponentType.ReadWrite<StateComponentB>()},
            None = new ComponentType[] {ComponentType.ReadOnly<GeneralPurposeComponentA>()}
        });

        m_ECBSource = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    struct NewEntityJob : IJobForEachWithEntity<GeneralPurposeComponentA>
    {
        public EntityCommandBuffer.Concurrent ConcurrentECB;

        public void Execute(Entity entity, int index, [ReadOnly] ref GeneralPurposeComponentA gpA)
        {
            // Add an ISystemStateComponentData instance
            ConcurrentECB.AddComponent<StateComponentB>(index, entity, new StateComponentB() {State = 1});
        }
    }

    struct ProcessEntityJob : IJobForEachWithEntity<GeneralPurposeComponentA>
    {
        public EntityCommandBuffer.Concurrent ConcurrentECB;

        public void Execute(Entity entity, int index, ref GeneralPurposeComponentA gpA)
        {
            // Process entity, possibly setting IsAlive false --
            // In which case, destroy the entity
            if (!gpA.IsAlive)
            {
                ConcurrentECB.DestroyEntity(index, entity);
            }
        }
    }

    struct CleanupEntityJob : IJobForEachWithEntity<StateComponentB>
    {
        public EntityCommandBuffer.Concurrent ConcurrentECB;

        public void Execute(Entity entity, int index, [ReadOnly] ref StateComponentB state)
        {
            // This system is responsible for removing any ISystemStateComponentData instances it adds
            // Otherwise, the entity is never truly destroyed.
            ConcurrentECB.RemoveComponent<StateComponentB>(index, entity);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var newEntityJob = new NewEntityJob()
        {
            ConcurrentECB = m_ECBSource.CreateCommandBuffer().ToConcurrent()
        };
        var newJobHandle = newEntityJob.ScheduleSingle(m_newEntities, inputDependencies);
        m_ECBSource.AddJobHandleForProducer(newJobHandle);

        var processEntityJob = new ProcessEntityJob()
            {ConcurrentECB = m_ECBSource.CreateCommandBuffer().ToConcurrent()};
        var processJobHandle = processEntityJob.Schedule(m_activeEntities, newJobHandle);
        m_ECBSource.AddJobHandleForProducer(processJobHandle);

        var cleanupEntityJob = new CleanupEntityJob()
        {
            ConcurrentECB = m_ECBSource.CreateCommandBuffer().ToConcurrent()
        };
        var cleanupJobHandle = cleanupEntityJob.ScheduleSingle(m_destroyedEntities, processJobHandle);
        m_ECBSource.AddJobHandleForProducer(cleanupJobHandle);

        return cleanupJobHandle;
    }

    protected override void OnDestroy()
    {
        // Implement OnDestroy to cleanup any resources allocated by this system.
        // (This simplified example does not allocate any resources.)
    }
}
```