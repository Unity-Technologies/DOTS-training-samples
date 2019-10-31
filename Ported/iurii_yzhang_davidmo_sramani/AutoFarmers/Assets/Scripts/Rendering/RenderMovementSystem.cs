using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

namespace GameAI
{
    // Defaults to simulation group
    public class RenderMovementSystem : JobComponentSystem
    {
        float timeToCompleteGroundTravel = 0.25f;
        
        // drone speed per second in meters
        float droneSpeed = 25.0f;

        int2 worldHalfSize;

        protected override void OnCreate()
        {
            worldHalfSize = World.GetOrCreateSystem<WorldCreatorSystem>().WorldSizeHalf;
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            var ecbSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            var ecb1 = ecbSystem.CreateCommandBuffer().ToConcurrent();

            var deltaT = Time.deltaTime;
            var worldHalfSizeLoc = worldHalfSize;
            var timeToCompletion = timeToCompleteGroundTravel;

            var movementSystemJobHandle = Entities
                .WithAll<FarmerAITag>()
                .ForEach(
                (int nativeThreadIndex, Entity e, ref RenderingAnimationComponent animationComponent, in TilePositionable tilePositionableComponent, in HasTarget hasTargetComponent) =>
                {
                    var startPos = RenderingUnity.Tile2WorldPosition(tilePositionableComponent.Position, worldHalfSizeLoc);
                    var endPosition = RenderingUnity.Tile2WorldPosition(hasTargetComponent.TargetPosition, worldHalfSizeLoc);
                    var currentPosition = new float3(animationComponent.currentPosition, startPos.z);
                    var totalDistanceSqrd = lengthsq(endPosition - startPos);
                    var traveledDistanceSqrd = lengthsq(endPosition - currentPosition);
                    var currentAnimTime = (traveledDistanceSqrd / totalDistanceSqrd) * timeToCompletion + deltaT;
                    
                    animationComponent.currentPosition = lerp(startPos, endPosition, currentAnimTime ).xy;

                    if (lengthsq(animationComponent.currentPosition - startPos.xy) > totalDistanceSqrd)
                    {
                        // Add Animation Complete Tag
                        ecb1.AddComponent<AnimationCompleteTag>(nativeThreadIndex, e);
                    }
                }).Schedule(inputDependencies);

                // TODO DRONES
            // var movementSystemJobHandle = Entities
            //     .WithNone<FarmerAITag>()
            //     .ForEach().Schedule();

            return movementSystemJobHandle;
        }
    }
}
#if USE_BASIC_BITCH_CLASS
public class RenderMovementSystem : JobComponentSystem
{
    // This declares a new kind of job, which is a unit of work to do.
    // The job is declared as an IJobForEach<Translation, Rotation>,
    // meaning it will process all entities in the world that have both
    // Translation and Rotation components. Change it to process the component
    // types you want.
    //
    // The job is also tagged with the BurstCompile attribute, which means
    // that the Burst compiler will optimize it for the best performance.
    [BurstCompile]
    struct RenderMovementSystemJob : IJobForEach<Translation, Rotation>
    {
        // Add fields here that your job needs to do its work.
        // For example,
        //    public float deltaTime;
        
        
        
        public void Execute(ref Translation translation, [ReadOnly] ref Rotation rotation)
        {
            // Implement the work to perform for each entity here.
            // You should only access data that is local or that is a
            // field on this job. Note that the 'rotation' parameter is
            // marked as [ReadOnly], which means it cannot be modified,
            // but allows this job to run in parallel with other jobs
            // that want to read Rotation component data.
            // For example,
            //     translation.Value += mul(rotation.Value, new float3(0, 0, 1)) * deltaTime;
            
            
        }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new RenderMovementSystemJob();
        
        // Assign values to the fields on your job here, so that it has
        // everything it needs to do its work when it runs later.
        // For example,
        //     job.deltaTime = UnityEngine.Time.deltaTime;
        
        
        
        // Now that the job is set up, schedule it to be run. 
        return job.Schedule(this, inputDependencies);
    }
}
#endif