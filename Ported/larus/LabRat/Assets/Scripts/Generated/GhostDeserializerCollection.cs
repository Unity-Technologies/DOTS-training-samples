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
            "OverlayColorGhostSerializer",
            "HomebaseGhostSerializer",
            "CatGhostSerializer",
            "SpawnedWallGhostSerializer",
        };
        return arr;
    }

    public int Length => 7;
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
        var curOverlayColorGhostSpawnSystem = world.GetOrCreateSystem<OverlayColorGhostSpawnSystem>();
        m_OverlayColorSnapshotDataNewGhostIds = curOverlayColorGhostSpawnSystem.NewGhostIds;
        m_OverlayColorSnapshotDataNewGhosts = curOverlayColorGhostSpawnSystem.NewGhosts;
        curOverlayColorGhostSpawnSystem.GhostType = 3;
        var curHomebaseGhostSpawnSystem = world.GetOrCreateSystem<HomebaseGhostSpawnSystem>();
        m_HomebaseSnapshotDataNewGhostIds = curHomebaseGhostSpawnSystem.NewGhostIds;
        m_HomebaseSnapshotDataNewGhosts = curHomebaseGhostSpawnSystem.NewGhosts;
        curHomebaseGhostSpawnSystem.GhostType = 4;
        var curCatGhostSpawnSystem = world.GetOrCreateSystem<CatGhostSpawnSystem>();
        m_CatSnapshotDataNewGhostIds = curCatGhostSpawnSystem.NewGhostIds;
        m_CatSnapshotDataNewGhosts = curCatGhostSpawnSystem.NewGhosts;
        curCatGhostSpawnSystem.GhostType = 5;
        var curSpawnedWallGhostSpawnSystem = world.GetOrCreateSystem<SpawnedWallGhostSpawnSystem>();
        m_SpawnedWallSnapshotDataNewGhostIds = curSpawnedWallGhostSpawnSystem.NewGhostIds;
        m_SpawnedWallSnapshotDataNewGhosts = curSpawnedWallGhostSpawnSystem.NewGhosts;
        curSpawnedWallGhostSpawnSystem.GhostType = 6;
    }

    public void BeginDeserialize(JobComponentSystem system)
    {
        m_OverlaySnapshotDataFromEntity = system.GetBufferFromEntity<OverlaySnapshotData>();
        m_MouseSnapshotDataFromEntity = system.GetBufferFromEntity<MouseSnapshotData>();
        m_PlayerSnapshotDataFromEntity = system.GetBufferFromEntity<PlayerSnapshotData>();
        m_OverlayColorSnapshotDataFromEntity = system.GetBufferFromEntity<OverlayColorSnapshotData>();
        m_HomebaseSnapshotDataFromEntity = system.GetBufferFromEntity<HomebaseSnapshotData>();
        m_CatSnapshotDataFromEntity = system.GetBufferFromEntity<CatSnapshotData>();
        m_SpawnedWallSnapshotDataFromEntity = system.GetBufferFromEntity<SpawnedWallSnapshotData>();
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
            case 3:
                return GhostReceiveSystem<LabRatGhostDeserializerCollection>.InvokeDeserialize(m_OverlayColorSnapshotDataFromEntity, entity, snapshot, baseline, baseline2,
                baseline3, reader, ref ctx, compressionModel);
            case 4:
                return GhostReceiveSystem<LabRatGhostDeserializerCollection>.InvokeDeserialize(m_HomebaseSnapshotDataFromEntity, entity, snapshot, baseline, baseline2,
                baseline3, reader, ref ctx, compressionModel);
            case 5:
                return GhostReceiveSystem<LabRatGhostDeserializerCollection>.InvokeDeserialize(m_CatSnapshotDataFromEntity, entity, snapshot, baseline, baseline2,
                baseline3, reader, ref ctx, compressionModel);
            case 6:
                return GhostReceiveSystem<LabRatGhostDeserializerCollection>.InvokeDeserialize(m_SpawnedWallSnapshotDataFromEntity, entity, snapshot, baseline, baseline2,
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
            case 3:
                m_OverlayColorSnapshotDataNewGhostIds.Add(ghostId);
                m_OverlayColorSnapshotDataNewGhosts.Add(GhostReceiveSystem<LabRatGhostDeserializerCollection>.InvokeSpawn<OverlayColorSnapshotData>(snapshot, reader, ref ctx, compressionModel));
                break;
            case 4:
                m_HomebaseSnapshotDataNewGhostIds.Add(ghostId);
                m_HomebaseSnapshotDataNewGhosts.Add(GhostReceiveSystem<LabRatGhostDeserializerCollection>.InvokeSpawn<HomebaseSnapshotData>(snapshot, reader, ref ctx, compressionModel));
                break;
            case 5:
                m_CatSnapshotDataNewGhostIds.Add(ghostId);
                m_CatSnapshotDataNewGhosts.Add(GhostReceiveSystem<LabRatGhostDeserializerCollection>.InvokeSpawn<CatSnapshotData>(snapshot, reader, ref ctx, compressionModel));
                break;
            case 6:
                m_SpawnedWallSnapshotDataNewGhostIds.Add(ghostId);
                m_SpawnedWallSnapshotDataNewGhosts.Add(GhostReceiveSystem<LabRatGhostDeserializerCollection>.InvokeSpawn<SpawnedWallSnapshotData>(snapshot, reader, ref ctx, compressionModel));
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
    private BufferFromEntity<OverlayColorSnapshotData> m_OverlayColorSnapshotDataFromEntity;
    private NativeList<int> m_OverlayColorSnapshotDataNewGhostIds;
    private NativeList<OverlayColorSnapshotData> m_OverlayColorSnapshotDataNewGhosts;
    private BufferFromEntity<HomebaseSnapshotData> m_HomebaseSnapshotDataFromEntity;
    private NativeList<int> m_HomebaseSnapshotDataNewGhostIds;
    private NativeList<HomebaseSnapshotData> m_HomebaseSnapshotDataNewGhosts;
    private BufferFromEntity<CatSnapshotData> m_CatSnapshotDataFromEntity;
    private NativeList<int> m_CatSnapshotDataNewGhostIds;
    private NativeList<CatSnapshotData> m_CatSnapshotDataNewGhosts;
    private BufferFromEntity<SpawnedWallSnapshotData> m_SpawnedWallSnapshotDataFromEntity;
    private NativeList<int> m_SpawnedWallSnapshotDataNewGhostIds;
    private NativeList<SpawnedWallSnapshotData> m_SpawnedWallSnapshotDataNewGhosts;
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
