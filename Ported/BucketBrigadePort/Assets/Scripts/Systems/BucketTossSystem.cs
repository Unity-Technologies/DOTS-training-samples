using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

public class BucketTossSystem : SystemBase
{
    protected override void OnUpdate()
    {
        /*
        Entities
        .WithName("bucket_fill_lakes")
        .WithAll<WaterAmount>()
        .WithAll<WaterRefill>()
        .ForEach((int entityInQueryIndex, Entity ballEntity) =>
        {
            UnityEngine.Debug.LogWarning("dead");
            ecb.DestroyEntity(entityInQueryIndex, ballEntity);
        }).Schedule();

        Entities
            .WithName("ball_movement")
            .ForEach((int entityInQueryIndex, Entity ballEntity, ref Translation2D translation, ref Velocity2D velocity, in Scale2D scale2D) =>
            {
                var aabb = screenBounds;
                aabb.Extents -= scale2D.Value / 2;
                translation.Value += velocity.Value * deltaTime;
                if (velocity.Value.x > 0 && translation.Value.x > aabb.Max.x)
                {
                    velocity.Value.x *= -1;
                }
                if (velocity.Value.x < 0 && translation.Value.x < aabb.Min.x)
                {
                    velocity.Value.x *= -1;
                }
                if (velocity.Value.y > 0 && translation.Value.y > aabb.Max.y)
                {
                    velocity.Value.y *= -1;
                }
                if (velocity.Value.y < 0 && translation.Value.y < aabb.Min.y)
                {
                    //velocity.Value.y *= -1;
                    //UnityEngine.Debug.Log("dead");
                    ecb.AddComponent<DeadBall>(entityInQueryIndex, ballEntity);
                }
            }
        ).ScheduleParallel();
        
        m_ECBSystem.AddJobHandleForProducer(Dependency);
        */
    }
}
