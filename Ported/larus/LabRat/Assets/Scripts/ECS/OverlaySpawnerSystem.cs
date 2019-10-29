using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEditorInternal.VersionControl;

[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
public class OverlaySpawnerSystem : ComponentSystem
{
    public Material[] PlayerMats;
    public Color[] Colors;
    /*public Material Player1;
    public Material Player2;
    public Material Player3;
    public Material Player4;*/

    public struct InitializedOverlaySpawner : IComponentData
    {}

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<GhostPrefabCollectionComponent>();
        Colors = new[] {Color.black, Color.blue, Color.red, Color.green};
        /*var shader = Shader.Find("Standard");
        Player1 = new Material(shader);
        Player1.color = Color.black;
        Player2 = new Material(shader);
        Player2.color = Color.blue;
        Player3 = new Material(shader);
        Player3.color = Color.red;
        Player4 = new Material(shader);
        Player4.color = Color.green;*/
    }

    protected override void OnUpdate()
    {
        Entities.WithNone<InitializedOverlaySpawner>().ForEach((Entity entity, ref GhostPrefabCollectionComponent collection) =>
        {
            const int maxArrows = 3;
            const int maxPlayers = 4;

            var serverPrefabs = EntityManager.GetBuffer<GhostPrefabBuffer>(collection.serverPrefabs);
            var prefab = serverPrefabs[LabRatGhostSerializerCollection.FindGhostType<OverlaySnapshotData>()].Value;
            var colorPrefab = serverPrefabs[LabRatGhostSerializerCollection.FindGhostType<OverlayColorSnapshotData>()].Value;
            for (int i = 0; i < maxPlayers * maxArrows; ++i)
            {
                // De las arrowas
                var instance = EntityManager.Instantiate(prefab);
                EntityManager.SetComponentData(instance, new Translation{Value = new float3(0,10,0)});

                // Cell stamp color
                instance = EntityManager.Instantiate(colorPrefab);
                EntityManager.SetComponentData(instance, new Translation{Value = new float3(0,10,0)});
                var sharedMesh = EntityManager.GetSharedComponentData<RenderMesh>(instance);
                if (PlayerMats == null || (PlayerMats.Length >= i && PlayerMats[i] == null))
                {
                    if (PlayerMats == null)
                        PlayerMats = new Material[maxPlayers * maxArrows];

                    var mat = new Material(sharedMesh.material);
                    var colorIndex = (int) math.floor(i / maxArrows);
                    mat.color = Colors[colorIndex];
                    PlayerMats[i] = mat;
                    Debug.Log("Set index " + i + " entity " + instance + " as color " + mat.color + " colorIndex=" + colorIndex);
                }
                /*if (Player1 == null)
                {
                    Player1 = new Material(sharedMesh.material);
                    Player1.color = Color.black;
                    Player2 = new Material(sharedMesh.material);
                    Player2.color = Color.blue;
                    Player3 = new Material(sharedMesh.material);
                    Player3.color = Color.red;
                    Player4 = new Material(sharedMesh.material);
                    Player4.color = Color.green;
                }*/
                // Doesn't work
                //sharedMesh.material = Player1;
                //sharedMesh.material = new Material(PlayerMats[i]);
                // Works
                sharedMesh.material.color = Color.black;
                //Debug.Log("Setting color " + sharedMesh.material.color + " on " + instance);
                EntityManager.SetSharedComponentData(instance, sharedMesh);
            }
            EntityManager.AddComponent<InitializedOverlaySpawner>(entity);
        });
    }
}
