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
                // De las arrowas
                var instance = EntityManager.Instantiate(prefab);
                EntityManager.SetComponentData(instance, new Translation{Value = new float3(0,10,0)});

                // Cell stamp color
                instance = EntityManager.Instantiate(colorPrefab);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                EntityManager.SetName(instance, "OverlayColor");
#endif
                EntityManager.SetComponentData(instance, new Translation{Value = new float3(0,10,0)});
                var colorIndex = (byte) (math.floor(i / PlayerConstants.MaxArrows) - 1);
                EntityManager.SetComponentData(instance, new OverlayColorComponent{Color = colorIndex});
                Debug.Log("Set index " + i + " entity " + instance + " as color " + colorIndex);
            }
            EntityManager.AddComponent<InitializedOverlaySpawner>(entity);
        });
    }
}


[UpdateInGroup(typeof(ClientSimulationSystemGroup))]
public class ApplyOverlayColors : ComponentSystem
{
    public Material[] PlayerMats;
    private Color[] m_Colors = new[] {Color.black, Color.blue, Color.red, Color.green};

    public struct InitializedColorTag : IComponentData
    {}

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<LocalPlayerComponent>();
    }

    protected override void OnUpdate()
    {
        var playerComponent = GetSingleton<LocalPlayerComponent>();
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
                    mat.color = m_Colors[colorIndex];
                    PlayerMats[colorIndex] = mat;
                }
                //Debug.Log("Client set index " + colorIndex + " entity " + entity + " as color " + PlayerMats[colorIndex].color);
            }

            Debug.Log("Client set index " + colorIndex + " entity " + entity + " as color " + PlayerMats[colorIndex].color);
            sharedMesh.material = PlayerMats[colorIndex];
            EntityManager.SetSharedComponentData(entity, sharedMesh);

            EntityManager.AddComponent<InitializedColorTag>(entity);
        });
    }
}