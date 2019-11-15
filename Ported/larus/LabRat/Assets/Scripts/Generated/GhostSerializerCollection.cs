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
            "OverlayColorGhostSerializer",
            "HomebaseGhostSerializer",
            "CatGhostSerializer",
            "SpawnedWallGhostSerializer",
        };
        return arr;
    }

    public int Length => 7;
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
        if (typeof(T) == typeof(OverlayColorSnapshotData))
            return 3;
        if (typeof(T) == typeof(HomebaseSnapshotData))
            return 4;
        if (typeof(T) == typeof(CatSnapshotData))
            return 5;
        if (typeof(T) == typeof(SpawnedWallSnapshotData))
            return 6;
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
        if (m_OverlayColorGhostSerializer.CanSerialize(arch))
            return 3;
        if (m_HomebaseGhostSerializer.CanSerialize(arch))
            return 4;
        if (m_CatGhostSerializer.CanSerialize(arch))
            return 5;
        if (m_SpawnedWallGhostSerializer.CanSerialize(arch))
            return 6;
        throw new ArgumentException("Invalid serializer type");
    }

    public void BeginSerialize(ComponentSystemBase system)
    {
        m_OverlayGhostSerializer.BeginSerialize(system);
        m_MouseGhostSerializer.BeginSerialize(system);
        m_PlayerGhostSerializer.BeginSerialize(system);
        m_OverlayColorGhostSerializer.BeginSerialize(system);
        m_HomebaseGhostSerializer.BeginSerialize(system);
        m_CatGhostSerializer.BeginSerialize(system);
        m_SpawnedWallGhostSerializer.BeginSerialize(system);
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
            case 3:
                return m_OverlayColorGhostSerializer.CalculateImportance(chunk);
            case 4:
                return m_HomebaseGhostSerializer.CalculateImportance(chunk);
            case 5:
                return m_CatGhostSerializer.CalculateImportance(chunk);
            case 6:
                return m_SpawnedWallGhostSerializer.CalculateImportance(chunk);
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
            case 3:
                return m_OverlayColorGhostSerializer.WantsPredictionDelta;
            case 4:
                return m_HomebaseGhostSerializer.WantsPredictionDelta;
            case 5:
                return m_CatGhostSerializer.WantsPredictionDelta;
            case 6:
                return m_SpawnedWallGhostSerializer.WantsPredictionDelta;
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
            case 3:
                return m_OverlayColorGhostSerializer.SnapshotSize;
            case 4:
                return m_HomebaseGhostSerializer.SnapshotSize;
            case 5:
                return m_CatGhostSerializer.SnapshotSize;
            case 6:
                return m_SpawnedWallGhostSerializer.SnapshotSize;
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
            case 3:
            {
                return GhostSendSystem<LabRatGhostSerializerCollection>.InvokeSerialize<OverlayColorGhostSerializer, OverlayColorSnapshotData>(m_OverlayColorGhostSerializer, data);
            }
            case 4:
            {
                return GhostSendSystem<LabRatGhostSerializerCollection>.InvokeSerialize<HomebaseGhostSerializer, HomebaseSnapshotData>(m_HomebaseGhostSerializer, data);
            }
            case 5:
            {
                return GhostSendSystem<LabRatGhostSerializerCollection>.InvokeSerialize<CatGhostSerializer, CatSnapshotData>(m_CatGhostSerializer, data);
            }
            case 6:
            {
                return GhostSendSystem<LabRatGhostSerializerCollection>.InvokeSerialize<SpawnedWallGhostSerializer, SpawnedWallSnapshotData>(m_SpawnedWallGhostSerializer, data);
            }
            default:
                throw new ArgumentException("Invalid serializer type");
        }
    }
    private OverlayGhostSerializer m_OverlayGhostSerializer;
    private MouseGhostSerializer m_MouseGhostSerializer;
    private PlayerGhostSerializer m_PlayerGhostSerializer;
    private OverlayColorGhostSerializer m_OverlayColorGhostSerializer;
    private HomebaseGhostSerializer m_HomebaseGhostSerializer;
    private CatGhostSerializer m_CatGhostSerializer;
    private SpawnedWallGhostSerializer m_SpawnedWallGhostSerializer;
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
