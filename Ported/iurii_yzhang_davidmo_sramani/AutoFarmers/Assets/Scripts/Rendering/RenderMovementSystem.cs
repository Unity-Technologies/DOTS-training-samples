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
        float farmerSpeed = 2.1f;
        
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
            var farmerSpeedLoc = this.farmerSpeed;

            var movementSystemJobHandle = Entities
                .WithAll<FarmerAITag>()
                .WithNone<AnimationCompleteTag>()
                // .WithoutBurst()
                .ForEach(
                (int nativeThreadIndex, Entity e, ref Translation translationComponent, ref RenderingAnimationComponent animationComponent, in TilePositionable tilePositionableComponent, in HasTarget hasTargetComponent) =>
                { 
                    var startPos = RenderingUnity.Tile2WorldPosition(tilePositionableComponent.Position, worldHalfSizeLoc);
                    var endPos = RenderingUnity.Tile2WorldPosition(hasTargetComponent.TargetPosition, worldHalfSizeLoc);
                    animationComponent.targetPosision = RenderingUnity.Tile2WorldPosition(hasTargetComponent.TargetPosition, worldHalfSizeLoc).xz;
                    animationComponent.currentPosition += (normalizesafe(endPos.xz - animationComponent.currentPosition) * farmerSpeedLoc * deltaT);
                    translationComponent.Value.xz = animationComponent.currentPosition;
                    if (lengthsq(endPos.xz - animationComponent.currentPosition) < 0.05)
                    {
                        // Add Animation Complete Tag
                        ecb1.AddComponent<AnimationCompleteTag>(nativeThreadIndex, e);
                    }
                }).Schedule(inputDependencies);
            
            ecbSystem.AddJobHandleForProducer(movementSystemJobHandle);
            
                // TODO DRONES
            // var movementSystemJobHandle = Entities
            //     .WithNone<FarmerAITag>()
            //     .ForEach().Schedule();

            return movementSystemJobHandle;
        }
    }
}