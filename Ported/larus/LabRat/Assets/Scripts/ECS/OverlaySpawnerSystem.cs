using UnityEngine;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;

static class PlayerConstants
{
    public const int MaxArrows = 3;
    public const int MaxPlayers = 4;
}

[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
[UpdateBefore(typeof(StartGame))]
public class OverlaySpawnerSystem : ComponentSystem
{
    public struct InitializedOverlaySpawner : IComponentData
    {}

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<GhostPrefabCollectionComponent>();
    }

    protected override void OnUpdate()
    {
        Entities.WithNone<InitializedOverlaySpawner>().ForEach((Entity entity, ref GhostPrefabCollectionComponent collection) =>
        {

            var serverPrefabs = EntityManager.GetBuffer<GhostPrefabBuffer>(collection.serverPrefabs);
            var prefab = serverPrefabs[LabRatGhostSerializerCollection.FindGhostType<OverlaySnapshotData>()].Value;
            var colorPrefab = serverPrefabs[LabRatGhostSerializerCollection.FindGhostType<OverlayColorSnapshotData>()].Value;
            for (int i = 0; i < PlayerConstants.MaxPlayers * PlayerConstants.MaxArrows; ++i)
            {
                var instance = EntityManager.Instantiate(prefab);
                EntityManager.SetComponentData(instance, new Translation{Value = new float3(0,-10,-10)});

                instance = EntityManager.Instantiate(colorPrefab);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                EntityManager.SetName(instance, "OverlayColor");
#endif
                EntityManager.SetComponentData(instance, new Translation{Value = new float3(0,-10,-10)});
                var colorIndex = (byte) (math.floor(i / PlayerConstants.MaxArrows));
                EntityManager.SetComponentData(instance, new OverlayColorComponent{Color = colorIndex});
            }
            EntityManager.AddComponent<InitializedOverlaySpawner>(entity);
        });
    }
}


[UpdateInGroup(typeof(ClientSimulationSystemGroup))]
public class ApplyOverlayColors : ComponentSystem
{
    public Material[] PlayerMats;
    public Color[] Colors = new[] {Color.black, Color.blue, Color.red, Color.green};

    public struct InitializedColorTag : IComponentData
    {}

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<LocalPlayerComponent>();
    }

    protected override void OnUpdate()
    {
        Entities.WithNone<InitializedColorTag>().ForEach((Entity entity, ref OverlayColorComponent colorComponent) =>
        {
            var sharedMesh = EntityManager.GetSharedComponentData<RenderMesh>(entity);
            var colorIndex = colorComponent.Color;
            // Color has not yet been replicated
            if (colorIndex == byte.MaxValue)
                return;
            if (PlayerMats == null || (PlayerMats.Length >= PlayerConstants.MaxPlayers && PlayerMats[colorIndex] == null))
            {
                if (PlayerMats == null)
                    PlayerMats = new Material[PlayerConstants.MaxPlayers];

                if (PlayerMats[colorIndex] == null)
                {
                    var mat = new Material(sharedMesh.material);
                    mat.color = Colors[colorIndex];
                    PlayerMats[colorIndex] = mat;
                }
            }
            sharedMesh.material = PlayerMats[colorIndex];
            EntityManager.SetSharedComponentData(entity, sharedMesh);

            EntityManager.AddComponent<InitializedColorTag>(entity);
        });
    }
}
