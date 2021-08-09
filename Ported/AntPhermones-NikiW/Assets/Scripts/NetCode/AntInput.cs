
using System;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;

public struct AntInput : ICommandData
{
    public uint Tick {get; set;}

    [GhostField]
    byte m_RawInput;

    public int Horizontal
    {
        get => (m_RawInput & 2) != 0 ? -1 : (m_RawInput & 1);
        set
        {
            m_RawInput = (byte)(m_RawInput & ~3);
            switch (value)
            {
                case -1: m_RawInput |= 2; break;
                case 0: break;
                case 1: m_RawInput |= 1; break;
                default: throw new ArgumentOutOfRangeException(nameof(value), value, $"Cannot set Horizontal Axis to {value}!");
            }
            Debug.Assert(value == Horizontal, $"Error in Horizontal Axis read/write. {value} became {Horizontal}!");
        }
    }

    public int Vertical
    {
        get => (m_RawInput & 4) != 0 ? -1 : (m_RawInput & 3) != 0 ? 0 : 1;
        set
        {
            m_RawInput = (byte)(m_RawInput & ~7);
            switch (value)
            { 
                case -1: m_RawInput |= 1 << 3; break;
                case 0: break;
                case 1: m_RawInput |= 1 << 4; break; 
                default: throw new ArgumentOutOfRangeException(nameof(value), value, $"Cannot set Vertical Axis to {value}!");
            } 
            Debug.Assert(value == Vertical, $"Error in Vertical Axis read/write. {value} became {Vertical}!");
        }
    }
}

[UpdateInGroup(typeof(GhostInputSystemGroup))]
[AlwaysSynchronizeSystem]
public partial class SampleAntInput : SystemBase
{
    ClientSimulationSystemGroup m_ClientSimulationSystemGroup;
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<NetworkIdComponent>();
        RequireSingletonForUpdate<CommandTargetComponent>();
        m_ClientSimulationSystemGroup = World.GetExistingSystem<ClientSimulationSystemGroup>();
    }

    protected override void OnUpdate()
    {
        var localInput = GetSingleton<CommandTargetComponent>().targetEntity;
        if (localInput == Entity.Null)
        {
            var localPlayerId = GetSingleton<NetworkIdComponent>().Value;
            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
            var commandTargetEntity = GetSingletonEntity<CommandTargetComponent>();
            Entities.WithNone<AntInput>().ForEach((Entity ent, in GhostOwnerComponent ghostOwner) =>
            {
                if (ghostOwner.NetworkId == localPlayerId)
                {
                    commandBuffer.AddBuffer<AntInput>(ent);
                    commandBuffer.SetComponent(commandTargetEntity, new CommandTargetComponent {targetEntity = ent});
                }
            }).Run();
            commandBuffer.Playback(EntityManager);
            return;
        }
        
        var input = default(AntInput);
        input.Tick = m_ClientSimulationSystemGroup.ServerTick;
        if (Input.GetKey("a"))
            input.Horizontal -= 1;
        if (Input.GetKey("d"))
            input.Horizontal += 1;
        if (Input.GetKey("s"))
            input.Vertical -= 1;
        if (Input.GetKey("w"))
            input.Vertical += 1;
        var inputBuffer = EntityManager.GetBuffer<AntInput>(localInput);
        inputBuffer.AddCommandData(input);
    }
}
