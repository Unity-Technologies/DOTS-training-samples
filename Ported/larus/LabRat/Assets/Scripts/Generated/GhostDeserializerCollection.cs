using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Networking.Transport;
using Unity.NetCode;

public struct LabRatGhostDeserializerCollection : IGhostDeserializerCollection
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
    public void Initialize(World world)
    {
        var curMouseGhostSpawnSystem = world.GetOrCreateSystem<MouseGhostSpawnSystem>();
        m_MouseSnapshotDataNewGhostIds = curMouseGhostSpawnSystem.NewGhostIds;
        m_MouseSnapshotDataNewGhosts = curMouseGhostSpawnSystem.NewGhosts;
        curMouseGhostSpawnSystem.GhostType = 0;
    }

    public void BeginDeserialize(JobComponentSystem system)
    {
        m_MouseSnapshotDataFromEntity = system.GetBufferFromEntity<MouseSnapshotData>();
    }
    public bool Deserialize(int serializer, Entity entity, uint snapshot, uint baseline, uint baseline2, uint baseline3,
        DataStreamReader reader,
        ref DataStreamReader.Context ctx, NetworkCompressionModel compressionModel)
    {
        switch (serializer)
        {
            case 0:
                return GhostReceiveSystem<LabRatGhostDeserializerCollection>.InvokeDeserialize(m_MouseSnapshotDataFromEntity, entity, snapshot, baseline, baseline2,
                baseline3, reader, ref ctx, compressionModel);
            default:
                throw new ArgumentException("Invalid serializer type");
        }
    }
    public void Spawn(int serializer, int ghostId, uint snapshot, DataStreamReader reader,
        ref DataStreamReader.Context ctx, NetworkCompressionModel compressionModel)
    {
        switch (serializer)
        {
            case 0:
                m_MouseSnapshotDataNewGhostIds.Add(ghostId);
                m_MouseSnapshotDataNewGhosts.Add(GhostReceiveSystem<LabRatGhostDeserializerCollection>.InvokeSpawn<MouseSnapshotData>(snapshot, reader, ref ctx, compressionModel));
                break;
            default:
                throw new ArgumentException("Invalid serializer type");
        }
    }

    private BufferFromEntity<MouseSnapshotData> m_MouseSnapshotDataFromEntity;
    private NativeList<int> m_MouseSnapshotDataNewGhostIds;
    private NativeList<MouseSnapshotData> m_MouseSnapshotDataNewGhosts;
}
public struct EnableLabRatGhostReceiveSystemComponent : IComponentData
{}
public class LabRatGhostReceiveSystem : GhostReceiveSystem<LabRatGhostDeserializerCollection>
{
    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<EnableLabRatGhostReceiveSystemComponent>();
    }
}
