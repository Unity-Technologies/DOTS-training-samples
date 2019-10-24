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
            "MouseGhostSerializer",
        };
        return arr;
    }

    public int Length => 1;
#endif
    public static int FindGhostType<T>()
        where T : struct, ISnapshotData<T>
    {
        if (typeof(T) == typeof(MouseSnapshotData))
            return 0;
        return -1;
    }
    public int FindSerializer(EntityArchetype arch)
    {
        if (m_MouseGhostSerializer.CanSerialize(arch))
            return 0;
        throw new ArgumentException("Invalid serializer type");
    }

    public void BeginSerialize(ComponentSystemBase system)
    {
        m_MouseGhostSerializer.BeginSerialize(system);
    }

    public int CalculateImportance(int serializer, ArchetypeChunk chunk)
    {
        switch (serializer)
        {
            case 0:
                return m_MouseGhostSerializer.CalculateImportance(chunk);
        }

        throw new ArgumentException("Invalid serializer type");
    }

    public bool WantsPredictionDelta(int serializer)
    {
        switch (serializer)
        {
            case 0:
                return m_MouseGhostSerializer.WantsPredictionDelta;
        }

        throw new ArgumentException("Invalid serializer type");
    }

    public int GetSnapshotSize(int serializer)
    {
        switch (serializer)
        {
            case 0:
                return m_MouseGhostSerializer.SnapshotSize;
        }

        throw new ArgumentException("Invalid serializer type");
    }

    public int Serialize(SerializeData data)
    {
        switch (data.ghostType)
        {
            case 0:
            {
                return GhostSendSystem<LabRatGhostSerializerCollection>.InvokeSerialize<MouseGhostSerializer, MouseSnapshotData>(m_MouseGhostSerializer, data);
            }
            default:
                throw new ArgumentException("Invalid serializer type");
        }
    }
    private MouseGhostSerializer m_MouseGhostSerializer;
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
