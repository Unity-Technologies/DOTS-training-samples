using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public partial class TrainSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        Entities
            .ForEach((Entity entity, in TrainSpawner spawner, in Spline spline) =>
            {
                // Destroying the current entity is a classic ECS pattern,
                // when something should only be processed once then forgotten.

                for (int i = 0; i < spawner.NoOfTrains; ++i)
                {
                    var leaderInstance = ecb.Instantiate(spawner.LeaderPrefab);
                    var trackProgress = new TrackProgress{Value = spline.splinePath.Value.pathLength * i / spawner.NoOfTrains};
                    var splineFollower = new SplineFollower{track = entity};
                    ecb.SetComponent(leaderInstance, trackProgress);
                    ecb.SetComponent(leaderInstance, splineFollower);

                    for (int j = 0; j < spawner.NoOfCartPerTrain; ++j)
                    {
                        var followerInstance = ecb.Instantiate(spawner.FollowerPrefab);
                        var follower = new Follower{Leader = leaderInstance, TrackData = entity, CartIndexInTrain = j+1};
                        ecb.SetComponent(followerInstance, follower);
                    }
                }
                ecb.RemoveComponent<TrainSpawner>(entity);
            }).Run();
        
        Entities
            .ForEach((Entity entity, in URPMaterialPropertyBaseColor baseColor) =>
            {
                
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}