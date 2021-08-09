using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;

[AlwaysUpdateSystem]
[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
public partial class ServerListenSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Ensure the AntSimulation singleton is setup:
        if (!TryGetSingleton<AntSimulationParams>(out var simParams)) 
            return;
        
        if (!TryGetSingleton(out AntSimulationRuntimeData simRuntimeData))
        {
            Debug.Log("Server spawning replicated AntSimulationRuntimeParams...");
            simRuntimeData.colonyPos = new float2(simParams.mapSize * .5f);
            var resourceAngle = UnityEngine.Random.value * 2f * math.PI;
            simRuntimeData.foodPosition = simRuntimeData.colonyPos + (new float2(math.cos(resourceAngle), math.sin(resourceAngle)) * simParams.mapSize * .475f);
            var simRuntimeDataEntity = EntityManager.Instantiate(simParams.antSimulationRuntimeDataPrefab);
            EntityManager.SetComponentData(simRuntimeDataEntity, simRuntimeData);

            EntityManager.Instantiate(simParams.foodPheromonesPrefab);
            EntityManager.Instantiate(simParams.colonyPheromonesPrefab);
        }
        
        // Server world automatically listen for connections from any host:
        var network = World.GetExistingSystem<NetworkStreamReceiveSystem>();
        if (!network.Driver.Listening)
        {
            var ep = NetworkEndPoint.AnyIpv4;
            ep.Port = 7979;
            network.Listen(ep);
            Debug.Log($"Server in '{World.Name}' starting listening at address: '{ep.Address}'!");
        }

        var antSimulationParams = GetSingleton<AntSimulationParams>();
        var playerAntPrefab = antSimulationParams.playerAntPrefab;
        var prefabName = new FixedString64($"{playerAntPrefab}:{EntityManager.GetName(playerAntPrefab)}");
        
        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        var networkIdFromEntity = GetComponentDataFromEntity<NetworkIdComponent>(true);
        Entities.WithReadOnly(networkIdFromEntity).ForEach((Entity reqEnt, in GoInGameRequest req, in ReceiveRpcCommandRequestComponent reqSrc) =>
        {
            commandBuffer.AddComponent<NetworkStreamInGame>(reqSrc.SourceConnection);
            var networkId = networkIdFromEntity[reqSrc.SourceConnection];
            
            Debug.Log($"Ant MP: Server setting NetworkConnection {networkId.Value} to in game, so spawning '{prefabName}' for player!");

            var player = commandBuffer.Instantiate(playerAntPrefab);
            commandBuffer.SetComponent(player, new GhostOwnerComponent { NetworkId = networkId.Value});

            commandBuffer.AddBuffer<AntInput>(player);
            commandBuffer.SetComponent(reqSrc.SourceConnection, new CommandTargetComponent {targetEntity = player});

            commandBuffer.DestroyEntity(reqEnt);
        }).Run();
        commandBuffer.Playback(EntityManager);
    }
}
