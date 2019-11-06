using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Collections;
using Unity.NetCode;
using Unity.Transforms;
using Unity.Rendering;

public struct CatGhostSerializer : IGhostSerializer<CatSnapshotData>
{
    private ComponentType componentTypeEaterComponentTag;
    private ComponentType componentTypeLastPositionComponent;
    private ComponentType componentTypeLocalToWorld;
    private ComponentType componentTypeRotation;
    private ComponentType componentTypeTranslation;
    private ComponentType componentTypeWalkComponent;
    // FIXME: These disable safety since all serializers have an instance of the same type - causing aliasing. Should be fixed in a cleaner way
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<Rotation> ghostRotationType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<Translation> ghostTranslationType;


    public int CalculateImportance(ArchetypeChunk chunk)
    {
        return 1;
    }

    public bool WantsPredictionDelta => true;

    public int SnapshotSize => UnsafeUtility.SizeOf<CatSnapshotData>();
    public void BeginSerialize(ComponentSystemBase system)
    {
        componentTypeEaterComponentTag = ComponentType.ReadWrite<EaterComponentTag>();
        componentTypeLastPositionComponent = ComponentType.ReadWrite<LastPositionComponent>();
        componentTypeLocalToWorld = ComponentType.ReadWrite<LocalToWorld>();
        componentTypeRotation = ComponentType.ReadWrite<Rotation>();
        componentTypeTranslation = ComponentType.ReadWrite<Translation>();
        componentTypeWalkComponent = ComponentType.ReadWrite<WalkComponent>();
        ghostRotationType = system.GetArchetypeChunkComponentType<Rotation>(true);
        ghostTranslationType = system.GetArchetypeChunkComponentType<Translation>(true);
    }

    public bool CanSerialize(EntityArchetype arch)
    {
        var components = arch.GetComponentTypes();
        int matches = 0;
        for (int i = 0; i < components.Length; ++i)
        {
            if (components[i] == componentTypeEaterComponentTag)
                ++matches;
            if (components[i] == componentTypeLastPositionComponent)
                ++matches;
            if (components[i] == componentTypeLocalToWorld)
                ++matches;
            if (components[i] == componentTypeRotation)
                ++matches;
            if (components[i] == componentTypeTranslation)
                ++matches;
            if (components[i] == componentTypeWalkComponent)
                ++matches;
        }
        return (matches == 6);
    }

    public void CopyToSnapshot(ArchetypeChunk chunk, int ent, uint tick, ref CatSnapshotData snapshot, GhostSerializerState serializerState)
    {
        snapshot.tick = tick;
        var chunkDataRotation = chunk.GetNativeArray(ghostRotationType);
        var chunkDataTranslation = chunk.GetNativeArray(ghostTranslationType);
        snapshot.SetRotationValue(chunkDataRotation[ent].Value, serializerState);
        snapshot.SetTranslationValue(chunkDataTranslation[ent].Value, serializerState);
    }
}
