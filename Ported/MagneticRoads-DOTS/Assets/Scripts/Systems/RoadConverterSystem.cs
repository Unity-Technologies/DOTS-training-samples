using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public partial class RoadConverterSystem : SystemBase
{
    protected override void OnUpdate()
    {
        if (!RoadGenerator.bullshit || RoadGenerator.trackSplines.Count == 0)
            return;
        
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        //TODO use a job for each spline
        foreach (var spline in RoadGenerator.trackSplines)
        {
            CreateRoadEntity(ecb, spline, false, true);
            CreateRoadEntity(ecb, spline, true, true);
            CreateRoadEntity(ecb, spline, false, false);
            CreateRoadEntity(ecb, spline, true, false);
        }
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
        
        Enabled = false;

        World.GetOrCreateSystem<CarSpawnerSystem>().Enabled = true;
    }
    
    private void CreateRoadEntity(EntityCommandBuffer ecb, TrackSpline spline, bool reversed, bool isTop)
    {
        var entity = ecb.CreateEntity();
        var def = new SplineDef
        {
            startPoint = reversed ? spline.endPoint : spline.startPoint,
            anchor1 = reversed ? spline.anchor2 : spline.anchor1,
            anchor2 = reversed ? spline.anchor1 : spline.anchor2,
            endPoint = reversed ? spline.startPoint : spline.endPoint,
            startNormal = reversed ? spline.endNormal.ToInt3() : spline.startNormal.ToInt3(),
            endNormal = reversed ? spline.startNormal.ToInt3() : spline.endNormal.ToInt3(),
            startTangent = reversed ? spline.endTangent.ToInt3() : spline.startTangent.ToInt3(),
            endTangent = reversed ? spline.startTangent.ToInt3() : spline.endTangent.ToInt3(),
            twistMode = spline.twistMode,
            offset = new float2(- RoadGenerator.trackRadius * 0.5f, (isTop ? 1f : -1f) * RoadGenerator.trackThickness* 1.75f)
        };
        
        ecb.AddComponent(entity, def);
        ecb.AddComponent<CarQueue>(entity);
        ecb.AddComponent(entity, new RoadLength
        {
            roadLength = spline.measuredLength
        });
    }
}

