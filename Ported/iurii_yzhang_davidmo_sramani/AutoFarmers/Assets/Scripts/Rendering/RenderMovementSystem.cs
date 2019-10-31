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
        int2 worldHalfSize;

        protected override void OnCreate()
        {
            worldHalfSize = World.GetOrCreateSystem<WorldCreatorSystem>().WorldSizeHalf;
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            var ecbSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            var ecb = ecbSystem.CreateCommandBuffer().ToConcurrent();

            var deltaT = Time.deltaTime;
            var worldHalfSizeLoc = worldHalfSize;

            var movementSystemJobHandle = Entities
                .WithNone<AnimationCompleteTag>()
                // .WithoutBurst()
                .ForEach(
                (int nativeThreadIndex, Entity e, ref Translation translationComponent, ref RenderingAnimationComponent animationComponent, in MovementSpeedComponent speedComponent, in TilePositionable tilePositionableComponent, in HasTarget hasTargetComponent) =>
                {
                    var endPos = RenderingUnity.Tile2WorldPosition(hasTargetComponent.TargetPosition, worldHalfSizeLoc);
                    animationComponent.targetPosition = RenderingUnity.Tile2WorldPosition(hasTargetComponent.TargetPosition, worldHalfSizeLoc).xz;
                    animationComponent.currentPosition += (normalizesafe(endPos.xz - animationComponent.currentPosition) * speedComponent.speedInMeters * deltaT);
                    translationComponent.Value.xz = animationComponent.currentPosition;
                    if (lengthsq(endPos.xz - animationComponent.currentPosition) < 0.5)
                    {
                        // Add Animation Complete Tag
                        ecb.AddComponent<AnimationCompleteTag>(nativeThreadIndex, e);
                    }
                }).Schedule(inputDependencies);
            
            ecbSystem.AddJobHandleForProducer(movementSystemJobHandle);

            return movementSystemJobHandle;
        }
    }
}
