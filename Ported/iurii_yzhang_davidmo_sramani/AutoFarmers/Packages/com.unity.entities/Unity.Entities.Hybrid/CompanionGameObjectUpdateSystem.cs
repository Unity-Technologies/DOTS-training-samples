#if !UNITY_DISABLE_MANAGED_COMPONENTS

using Unity.Entities;
using UnityEngine;

// Needs to be a system state because instantiation will always create disabled GameObjects
struct CompanionGameObjectActiveSystemState : ISystemStateComponentData { }

class CompanionGameObjectUpdateSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithNone<CompanionLinkSystemState>().ForEach((Entity entity, CompanionLink link) =>
        {
            EntityManager.AddComponentObject(entity, new CompanionLinkSystemState { Link = link.gameObject });
        });

        var toActivate = Entities.WithNone<CompanionGameObjectActiveSystemState, Disabled, Prefab>().WithAll<CompanionLink>();
        toActivate.ForEach((CompanionLink link) => link.gameObject.SetActive(true));
        EntityManager.AddComponent<CompanionGameObjectActiveSystemState>(toActivate.ToEntityQuery());

        var toDeactivate = Entities.WithAny<Disabled, Prefab>().WithAll<CompanionGameObjectActiveSystemState>();
        toDeactivate.ForEach((CompanionLink link) => link.gameObject.SetActive(false));
        EntityManager.RemoveComponent<CompanionGameObjectActiveSystemState>(toDeactivate.ToEntityQuery());

        Entities.WithNone<CompanionLink>().ForEach((Entity entity, CompanionLinkSystemState link) =>
        {
            GameObject.DestroyImmediate(link.Link);
            EntityManager.RemoveComponent<CompanionLinkSystemState>(entity);
        });

        var activeSystemStateCleanup = Entities.WithNone<CompanionLink>().WithAll<CompanionGameObjectActiveSystemState>().ToEntityQuery();
        EntityManager.RemoveComponent<CompanionGameObjectActiveSystemState>(activeSystemStateCleanup);
    }
}

#endif
