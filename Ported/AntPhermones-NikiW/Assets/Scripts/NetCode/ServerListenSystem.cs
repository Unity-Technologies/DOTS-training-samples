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
    bool m_HasSetClientServerTickRate;
    
    protected override void OnUpdate()
    {
        if (!m_HasSetClientServerTickRate)
        {
            Debug.Log("Setting ClientServerTickRate!");
            TryGetSingleton(out ClientServerTickRate clientServerTickRate);
            clientServerTickRate.ResolveDefaults();
            clientServerTickRate.SimulationTickRate = 60;
            clientServerTickRate.NetworkTickRate = 15;
            clientServerTickRate.MaxSimulationStepsPerFrame = 8;
            SetSingleton(clientServerTickRate);
            m_HasSetClientServerTickRate = true;
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
        }
        
        // Ensure the AntSimulation singleton is setup:
        if (!TryGetSingleton<AntSimulationPrefabs>(out var prefabs)) 
            return;
        
        if (!TryGetSingleton(out AntSimulationRuntimeData simRuntimeData))
        {
            var simParams = EntityManager.GetComponentData<AntSimulationParams>(prefabs.antSimulationRuntimeDataPrefab);
            Debug.Log($"Server spawning replicated AntSimulationRuntimeParams: mapSize: {simParams.mapSize}, antCount: {simParams.antCount}");
            
            simRuntimeData.colonyPos = new float2(simParams.mapSize * .5f);
            var resourceAngle = UnityEngine.Random.value * 2f * math.PI;
            simRuntimeData.foodPosition = simRuntimeData.colonyPos + (new float2(math.cos(resourceAngle), math.sin(resourceAngle)) * simParams.mapSize * .475f);
            var simRuntimeDataEntity = EntityManager.Instantiate(prefabs.antSimulationRuntimeDataPrefab);
            EntityManager.SetComponentData(simRuntimeDataEntity, simRuntimeData);

            EntityManager.Instantiate(prefabs.foodPheromonesPrefab);
            EntityManager.Instantiate(prefabs.colonyPheromonesPrefab);
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
        
        var prefabName = new FixedString64($"{prefabs.playerAntPrefab}:{EntityManager.GetName(prefabs.playerAntPrefab)}");
        
        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        var networkIdFromEntity = GetComponentDataFromEntity<NetworkIdComponent>(true);
        Entities.WithReadOnly(networkIdFromEntity).ForEach((Entity reqEnt, in GoInGameRequest req, in ReceiveRpcCommandRequestComponent reqSrc) =>
        {
            commandBuffer.AddComponent<NetworkStreamInGame>(reqSrc.SourceConnection);
            var networkId = networkIdFromEntity[reqSrc.SourceConnection];
            
            Debug.Log($"Ant MP: Server setting NetworkConnection {networkId.Value} to in game, so spawning '{prefabName}' for player!");

            var player = commandBuffer.Instantiate(prefabs.playerAntPrefab);
            commandBuffer.SetComponent(player, new GhostOwnerComponent { NetworkId = networkId.Value});

            commandBuffer.AddBuffer<AntInput>(player);
            commandBuffer.SetComponent(reqSrc.SourceConnection, new CommandTargetComponent {targetEntity = player});

            commandBuffer.DestroyEntity(reqEnt);
        }).Run();
        commandBuffer.Playback(EntityManager);
    }
}
