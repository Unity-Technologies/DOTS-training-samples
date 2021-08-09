using System;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;

[AlwaysUpdateSystem]
[UpdateInGroup(typeof(ClientSimulationSystemGroup))]
public partial class ClientJoinSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Client worlds automatically connect to localhost
        var network = World.GetExistingSystem<NetworkStreamReceiveSystem>();
        if (!HasSingleton<CommandTargetComponent>())
        {
            var ep = NetworkEndPoint.LoopbackIpv4;
            ep.Port = 7979;
            if (Application.isEditor)
                ep = NetworkEndPoint.Parse(ClientServerBootstrap.RequestedAutoConnect, 7979);
            network.Connect(ep);
            Debug.Log($"Client in '{World.Name}' connecting to address '{ep.Address}'!");
        }

        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        Entities.WithNone<NetworkStreamInGame>().ForEach((Entity ent, in NetworkIdComponent id) =>
        {
            Debug.Log($"Adding NetworkStreamInGame to NetworkId {id} and requesting GoInGameRequest!");
            commandBuffer.AddComponent<NetworkStreamInGame>(ent);
            commandBuffer.SetName(ent, nameof(NetworkStreamInGame));
            
            var req = commandBuffer.CreateEntity();
            commandBuffer.SetName(req, nameof(GoInGameRequest));
            commandBuffer.AddComponent<GoInGameRequest>(req);
            commandBuffer.AddComponent(req, new SendRpcCommandRequestComponent { TargetConnection = ent });
        }).Run();
        commandBuffer.Playback(EntityManager);
    }
}
