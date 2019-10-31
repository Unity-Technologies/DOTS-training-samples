using System;
using ECSExamples;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

// Control system updating in the default world
[UpdateInWorld(UpdateInWorld.TargetWorld.Default)]
public class Game : ComponentSystem
{
    // Singleton component to trigger connections once from a control system
    struct InitGameComponent : IComponentData
    {
    }
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<InitGameComponent>();
        // Create singleton, require singleton for update so system runs once
        EntityManager.CreateEntity(typeof(InitGameComponent));
    }

    protected override void OnUpdate()
    {
        // Destroy singleton to prevent system from running again
        EntityManager.DestroyEntity(GetSingletonEntity<InitGameComponent>());
        foreach (var world in World.AllWorlds)
        {
            var network = world.GetExistingSystem<NetworkStreamReceiveSystem>();
            if (world.GetExistingSystem<ClientSimulationSystemGroup>() != null)
            {
                // Client worlds automatically connect to localhost
                NetworkEndPoint ep = NetworkEndPoint.LoopbackIpv4;
                ep.Port = 7979;
                Debug.Log("Connecting to 127.0.0.1:7979");
                network.Connect(ep);
            }
            #if UNITY_EDITOR
            else if (world.GetExistingSystem<ServerSimulationSystemGroup>() != null)
            {
                // Server world automatically listen for connections from any host
                NetworkEndPoint ep = NetworkEndPoint.AnyIpv4;
                ep.Port = 7979;
                Debug.Log("Listening on 7979");
                network.Listen(ep);
            }
            #endif
        }
    }
}

[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
public class SpawnPlayerSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithNone<NetworkStreamInGame>().ForEach((Entity entity, ref NetworkIdComponent id) =>
        {
            // Set up a player entity
            PostUpdateCommands.AddComponent<NetworkStreamInGame>(entity);
            var prefabs = GetSingleton<GhostPrefabCollectionComponent>();
            var serverPrefabs = EntityManager.GetBuffer<GhostPrefabBuffer>(prefabs.serverPrefabs);
            var prefab = serverPrefabs[LabRatGhostSerializerCollection.FindGhostType<PlayerSnapshotData>()].Value;
            var player = EntityManager.Instantiate(prefab);
            PostUpdateCommands.AddComponent(player, new PlayerComponent{PlayerId = id.Value});
            PostUpdateCommands.AddBuffer<PlayerInput>(player);
            PostUpdateCommands.SetComponent(entity, new CommandTargetComponent{targetEntity = player});

            // Set up the players home base
            World.GetExistingSystem<BoardSystem>().SpawnHomebase(id.Value);
            Debug.Log("Server got new connection NetId=" + id.Value + " spawned player entity " + player);
        });
    }
}

[UpdateInGroup(typeof(ClientSimulationSystemGroup))]
public class ClientGoInGameSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithNone<NetworkStreamInGame>().ForEach((Entity ent, ref NetworkIdComponent id) =>
        {
            Debug.Log("Client connected with NetId=" + id.Value);
            PostUpdateCommands.AddComponent<NetworkStreamInGame>(ent);

            // Create dummy player for sending inputs to server, normal client will get it replicated
            if (HasSingleton<ThinClientComponent>())
            {
                var dummy = PostUpdateCommands.CreateEntity();
                PostUpdateCommands.AddComponent(dummy, new PlayerComponent{PlayerId = id.Value});
            }
        });
    }
}
