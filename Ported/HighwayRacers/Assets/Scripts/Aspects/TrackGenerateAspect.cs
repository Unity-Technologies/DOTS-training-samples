using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

readonly partial struct TrackGenerateAspect : IAspect<TrackGenerateAspect>
{
    private readonly RefRO<TrackSectionPrefabs> m_Sections;
    private readonly RefRO<TrackNeedsGeneration> m_NeedsGeneration;

    public TrackSectionPrefabs Sections => m_Sections.ValueRO;
}