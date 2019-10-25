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
            "OverlayGhostSerializer",
            "MouseGhostSerializer",
            "PlayerGhostSerializer",
        };
        return arr;
    }

    public int Length => 3;
#endif
    public void Initialize(World world)
    {
        var curOverlayGhostSpawnSystem = world.GetOrCreateSystem<OverlayGhostSpawnSystem>();
        m_OverlaySnapshotDataNewGhostIds = curOverlayGhostSpawnSystem.NewGhostIds;
        m_OverlaySnapshotDataNewGhosts = curOverlayGhostSpawnSystem.NewGhosts;
        curOverlayGhostSpawnSystem.GhostType = 0;
        var curMouseGhostSpawnSystem = world.GetOrCreateSystem<MouseGhostSpawnSystem>();
        m_MouseSnapshotDataNewGhostIds = curMouseGhostSpawnSystem.NewGhostIds;
        m_MouseSnapshotDataNewGhosts = curMouseGhostSpawnSystem.NewGhosts;
        curMouseGhostSpawnSystem.GhostType = 1;
        var curPlayerGhostSpawnSystem = world.GetOrCreateSystem<PlayerGhostSpawnSystem>();
        m_PlayerSnapshotDataNewGhostIds = curPlayerGhostSpawnSystem.NewGhostIds;
        m_PlayerSnapshotDataNewGhosts = curPlayerGhostSpawnSystem.NewGhosts;
        curPlayerGhostSpawnSystem.GhostType = 2;
    }

    public void BeginDeserialize(JobComponentSystem system)
    {
        m_OverlaySnapshotDataFromEntity = system.GetBufferFromEntity<OverlaySnapshotData>();
        m_MouseSnapshotDataFromEntity = system.GetBufferFromEntity<MouseSnapshotData>();
        m_PlayerSnapshotDataFromEntity = system.GetBufferFromEntity<PlayerSnapshotData>();
    }
    public bool Deserialize(int serializer, Entity entity, uint snapshot, uint baseline, uint baseline2, uint baseline3,
        DataStreamReader reader,
        ref DataStreamReader.Context ctx, NetworkCompressionModel compressionModel)
    {
        switch (serializer)
        {
            case 0:
                return GhostReceiveSystem<LabRatGhostDeserializerCollection>.InvokeDeserialize(m_OverlaySnapshotDataFromEntity, entity, snapshot, baseline, baseline2,
                baseline3, reader, ref ctx, compressionModel);
            case 1:
                return GhostReceiveSystem<LabRatGhostDeserializerCollection>.InvokeDeserialize(m_MouseSnapshotDataFromEntity, entity, snapshot, baseline, baseline2,
                baseline3, reader, ref ctx, compressionModel);
            case 2:
                return GhostReceiveSystem<LabRatGhostDeserializerCollection>.InvokeDeserialize(m_PlayerSnapshotDataFromEntity, entity, snapshot, baseline, baseline2,
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
                m_OverlaySnapshotDataNewGhostIds.Add(ghostId);
                m_OverlaySnapshotDataNewGhosts.Add(GhostReceiveSystem<LabRatGhostDeserializerCollection>.InvokeSpawn<OverlaySnapshotData>(snapshot, reader, ref ctx, compressionModel));
                break;
            case 1:
                m_MouseSnapshotDataNewGhostIds.Add(ghostId);
                m_MouseSnapshotDataNewGhosts.Add(GhostReceiveSystem<LabRatGhostDeserializerCollection>.InvokeSpawn<MouseSnapshotData>(snapshot, reader, ref ctx, compressionModel));
                break;
            case 2:
                m_PlayerSnapshotDataNewGhostIds.Add(ghostId);
                m_PlayerSnapshotDataNewGhosts.Add(GhostReceiveSystem<LabRatGhostDeserializerCollection>.InvokeSpawn<PlayerSnapshotData>(snapshot, reader, ref ctx, compressionModel));
                break;
            default:
                throw new ArgumentException("Invalid serializer type");
        }
    }

    private BufferFromEntity<OverlaySnapshotData> m_OverlaySnapshotDataFromEntity;
    private NativeList<int> m_OverlaySnapshotDataNewGhostIds;
    private NativeList<OverlaySnapshotData> m_OverlaySnapshotDataNewGhosts;
    private BufferFromEntity<MouseSnapshotData> m_MouseSnapshotDataFromEntity;
    private NativeList<int> m_MouseSnapshotDataNewGhostIds;
    private NativeList<MouseSnapshotData> m_MouseSnapshotDataNewGhosts;
    private BufferFromEntity<PlayerSnapshotData> m_PlayerSnapshotDataFromEntity;
    private NativeList<int> m_PlayerSnapshotDataNewGhostIds;
    private NativeList<PlayerSnapshotData> m_PlayerSnapshotDataNewGhosts;
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
