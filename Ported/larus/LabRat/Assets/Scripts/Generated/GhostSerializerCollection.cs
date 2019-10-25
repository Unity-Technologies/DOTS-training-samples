using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Networking.Transport;
using Unity.NetCode;

public struct LabRatGhostSerializerCollection : IGhostSerializerCollection
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public string[] CreateSerializerNameList()
    {
        var arr = new string[]
        {
            "OverlayGhostSerializer",
            "MouseGhostSerializer",
            "PlayerGhostSerializer",
        };
        return arr;
    }

    public int Length => 3;
#endif
    public static int FindGhostType<T>()
        where T : struct, ISnapshotData<T>
    {
        if (typeof(T) == typeof(OverlaySnapshotData))
            return 0;
        if (typeof(T) == typeof(MouseSnapshotData))
            return 1;
        if (typeof(T) == typeof(PlayerSnapshotData))
            return 2;
        return -1;
    }
    public int FindSerializer(EntityArchetype arch)
    {
        if (m_OverlayGhostSerializer.CanSerialize(arch))
            return 0;
        if (m_MouseGhostSerializer.CanSerialize(arch))
            return 1;
        if (m_PlayerGhostSerializer.CanSerialize(arch))
            return 2;
        throw new ArgumentException("Invalid serializer type");
    }

    public void BeginSerialize(ComponentSystemBase system)
    {
        m_OverlayGhostSerializer.BeginSerialize(system);
        m_MouseGhostSerializer.BeginSerialize(system);
        m_PlayerGhostSerializer.BeginSerialize(system);
    }

    public int CalculateImportance(int serializer, ArchetypeChunk chunk)
    {
        switch (serializer)
        {
            case 0:
                return m_OverlayGhostSerializer.CalculateImportance(chunk);
            case 1:
                return m_MouseGhostSerializer.CalculateImportance(chunk);
            case 2:
                return m_PlayerGhostSerializer.CalculateImportance(chunk);
        }

        throw new ArgumentException("Invalid serializer type");
    }

    public bool WantsPredictionDelta(int serializer)
    {
        switch (serializer)
        {
            case 0:
                return m_OverlayGhostSerializer.WantsPredictionDelta;
            case 1:
                return m_MouseGhostSerializer.WantsPredictionDelta;
            case 2:
                return m_PlayerGhostSerializer.WantsPredictionDelta;
        }

        throw new ArgumentException("Invalid serializer type");
    }

    public int GetSnapshotSize(int serializer)
    {
        switch (serializer)
        {
            case 0:
                return m_OverlayGhostSerializer.SnapshotSize;
            case 1:
                return m_MouseGhostSerializer.SnapshotSize;
            case 2:
                return m_PlayerGhostSerializer.SnapshotSize;
        }

        throw new ArgumentException("Invalid serializer type");
    }

    public int Serialize(SerializeData data)
    {
        switch (data.ghostType)
        {
            case 0:
            {
                return GhostSendSystem<LabRatGhostSerializerCollection>.InvokeSerialize<OverlayGhostSerializer, OverlaySnapshotData>(m_OverlayGhostSerializer, data);
            }
            case 1:
            {
                return GhostSendSystem<LabRatGhostSerializerCollection>.InvokeSerialize<MouseGhostSerializer, MouseSnapshotData>(m_MouseGhostSerializer, data);
            }
            case 2:
            {
                return GhostSendSystem<LabRatGhostSerializerCollection>.InvokeSerialize<PlayerGhostSerializer, PlayerSnapshotData>(m_PlayerGhostSerializer, data);
            }
            default:
                throw new ArgumentException("Invalid serializer type");
        }
    }
    private OverlayGhostSerializer m_OverlayGhostSerializer;
    private MouseGhostSerializer m_MouseGhostSerializer;
    private PlayerGhostSerializer m_PlayerGhostSerializer;
}

public struct EnableLabRatGhostSendSystemComponent : IComponentData
{}
public class LabRatGhostSendSystem : GhostSendSystem<LabRatGhostSerializerCollection>
{
    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<EnableLabRatGhostSendSystemComponent>();
    }
}
