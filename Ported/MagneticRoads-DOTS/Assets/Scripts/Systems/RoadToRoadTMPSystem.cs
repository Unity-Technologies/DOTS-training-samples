using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public partial class RoadToRoadTMPSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Persistent);
        var random = new Random(1234);

        Entities
            .WithAll<SplinePosition, SplineDefForCar, RoadCompleted>()
            .ForEach((Entity entity, ref SplineDefForCar splineDefForCar, ref SplinePosition splinePosition, ref CurrentRoad currentRoad) =>
            {
                var neighbors = GetBuffer<RoadNeighbors>(currentRoad.currentRoad);
                
                currentRoad.currentRoad = neighbors[random.NextInt(0,neighbors.Length)].Value;
                var roadSpline = GetComponent<SplineDef>(currentRoad.currentRoad);
                splineDefForCar = new SplineDefForCar(roadSpline);
                
                splinePosition.position = 0f;
                ecb.RemoveComponent<RoadCompleted>(entity);
            }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
