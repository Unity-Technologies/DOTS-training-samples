using Unity.Collections;
using Unity.Entities;

public partial class RoadConverterSystem : SystemBase
{
    protected override void OnUpdate()
    {
        if (!RoadGenerator.bullshit || RoadGenerator.trackSplines.Count == 0)
            return;
        
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var entityBuffer = new Entity[RoadGenerator.trackSplines.Count*4];
        //TODO use a job for each spline
        foreach (var spline in RoadGenerator.trackSplines)
        {
            foreach (var splineDef in RoadGenerator.splineToMultiSpline[spline.splineId])
            {
                entityBuffer[splineDef.splineId] = CreateRoadEntity(ecb, spline, splineDef);
            }
        }

        for (int i = 0; i < entityBuffer.Length; i++)
        {
            var entity = entityBuffer[i];
            var buffer = ecb.AddBuffer<RoadNeighbors>(entity);
            foreach (var link in RoadGenerator.weirdoWeirdSplineLinks[i])
            {
                buffer.Add(new RoadNeighbors {Value = entityBuffer[link]});
            }
        }
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
        
        Enabled = false;

        World.GetOrCreateSystem<CarSpawnerSystem>().Enabled = true;
    }
    
    private Entity CreateRoadEntity(EntityCommandBuffer ecb, TrackSpline originSpline, SplineDef splineDef)
    {
        var entity = ecb.CreateEntity();

        ecb.AddComponent(entity, splineDef);
        ecb.AddComponent<CarQueue>(entity);
        ecb.AddComponent(entity, new RoadLength
        {
            roadLength = originSpline.measuredLength
        });
        return entity;
    }
}

