using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using Unity.Mathematics;

[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
public class OverlaySpawnerSystem : ComponentSystem
{
    public struct InitializedOverlay : IComponentData
    {}

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<GhostPrefabCollectionComponent>();
    }

    protected override void OnUpdate()
    {
        Entities.WithNone<InitializedOverlay>().ForEach((Entity entity, ref GhostPrefabCollectionComponent collection) =>
        {
            var serverPrefabs = EntityManager.GetBuffer<GhostPrefabBuffer>(collection.serverPrefabs);
            var prefab = serverPrefabs[LabRatGhostSerializerCollection.FindGhostType<OverlaySnapshotData>()].Value;
            for (int i = 0; i < 4 * 3; ++i)
            {
                var instance = EntityManager.Instantiate(prefab);
                EntityManager.SetComponentData(instance, new Translation{Value = new float3(0,10,0)});
            }
            PostUpdateCommands.AddComponent<InitializedOverlay>(entity);
        });
    }
}
