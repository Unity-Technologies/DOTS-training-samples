using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Collections;
using Unity.NetCode;
using Unity.Rendering;
using Unity.Transforms;

public struct OverlayGhostSerializer : IGhostSerializer<OverlaySnapshotData>
{
    private ComponentType componentTypeOverlayComponentTag;
    private ComponentType componentTypeOverlayPlacementTickComponent;
    private ComponentType componentTypePerInstanceCullingTag;
    private ComponentType componentTypeRenderBounds;
    private ComponentType componentTypeRenderMesh;
    private ComponentType componentTypeLocalToWorld;
    private ComponentType componentTypeNonUniformScale;
    private ComponentType componentTypeRotation;
    private ComponentType componentTypeTranslation;
    // FIXME: These disable safety since all serializers have an instance of the same type - causing aliasing. Should be fixed in a cleaner way
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<Rotation> ghostRotationType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<Translation> ghostTranslationType;


    public int CalculateImportance(ArchetypeChunk chunk)
    {
        return 1;
    }

    public bool WantsPredictionDelta => true;

    public int SnapshotSize => UnsafeUtility.SizeOf<OverlaySnapshotData>();
    public void BeginSerialize(ComponentSystemBase system)
    {
        componentTypeOverlayComponentTag = ComponentType.ReadWrite<OverlayComponentTag>();
        componentTypeOverlayPlacementTickComponent = ComponentType.ReadWrite<OverlayPlacementTickComponent>();
        componentTypePerInstanceCullingTag = ComponentType.ReadWrite<PerInstanceCullingTag>();
        componentTypeRenderBounds = ComponentType.ReadWrite<RenderBounds>();
        componentTypeRenderMesh = ComponentType.ReadWrite<RenderMesh>();
        componentTypeLocalToWorld = ComponentType.ReadWrite<LocalToWorld>();
        componentTypeNonUniformScale = ComponentType.ReadWrite<NonUniformScale>();
        componentTypeRotation = ComponentType.ReadWrite<Rotation>();
        componentTypeTranslation = ComponentType.ReadWrite<Translation>();
        ghostRotationType = system.GetArchetypeChunkComponentType<Rotation>(true);
        ghostTranslationType = system.GetArchetypeChunkComponentType<Translation>(true);
    }

    public bool CanSerialize(EntityArchetype arch)
    {
        var components = arch.GetComponentTypes();
        int matches = 0;
        for (int i = 0; i < components.Length; ++i)
        {
            if (components[i] == componentTypeOverlayComponentTag)
                ++matches;
            if (components[i] == componentTypeOverlayPlacementTickComponent)
                ++matches;
            if (components[i] == componentTypePerInstanceCullingTag)
                ++matches;
            if (components[i] == componentTypeRenderBounds)
                ++matches;
            if (components[i] == componentTypeRenderMesh)
                ++matches;
            if (components[i] == componentTypeLocalToWorld)
                ++matches;
            if (components[i] == componentTypeNonUniformScale)
                ++matches;
            if (components[i] == componentTypeRotation)
                ++matches;
            if (components[i] == componentTypeTranslation)
                ++matches;
        }
        return (matches == 9);
    }

    public void CopyToSnapshot(ArchetypeChunk chunk, int ent, uint tick, ref OverlaySnapshotData snapshot, GhostSerializerState serializerState)
    {
        snapshot.tick = tick;
        var chunkDataRotation = chunk.GetNativeArray(ghostRotationType);
        var chunkDataTranslation = chunk.GetNativeArray(ghostTranslationType);
        snapshot.SetRotationValue(chunkDataRotation[ent].Value, serializerState);
        snapshot.SetTranslationValue(chunkDataTranslation[ent].Value, serializerState);
    }
}
