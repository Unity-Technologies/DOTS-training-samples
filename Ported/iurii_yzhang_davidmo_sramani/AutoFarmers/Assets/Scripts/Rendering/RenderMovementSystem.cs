using Rendering;
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
        const float maxDroneOffset = 0.1f; // In meters

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
                .ForEach(
                (int nativeThreadIndex, Entity e, ref Translation translationComponent, ref RenderingAnimationComponent animationComponent, in MovementSpeedComponent speedComponent, in TilePositionable tilePositionableComponent, in HasTarget hasTargetComponent) =>
                {
                    var endPos = RenderingUnity.Tile2WorldPosition(hasTargetComponent.TargetPosition, worldHalfSizeLoc);
                    animationComponent.targetPosition = RenderingUnity.Tile2WorldPosition(hasTargetComponent.TargetPosition, worldHalfSizeLoc).xz;
                    animationComponent.currentPosition += (normalizesafe(endPos.xz - animationComponent.currentPosition) * speedComponent.speedInMeters * deltaT);
                    if (lengthsq(endPos.xz - animationComponent.currentPosition) < 0.05)
                    {
                        // Add Animation Complete Tag
                        ecb.AddComponent<AnimationCompleteTag>(nativeThreadIndex, e);
                        animationComponent.currentPosition = endPos.xz;
                    }
                    translationComponent.Value.xz = animationComponent.currentPosition;
                }).Schedule(inputDependencies);
            
            ecbSystem.AddJobHandleForProducer(movementSystemJobHandle);

            var droneOffsetLoc = maxDroneOffset;

            var droneTweakJobHandle = Entities
                .ForEach((int nativeThreadIndex, Entity e, ref Translation translationComponent, ref RenderingAnimationDroneFlyComponent flyRenderComponent) =>
                {
                    flyRenderComponent.offset += deltaT;
                    translationComponent.Value.y = (float)sin(flyRenderComponent.offset) * droneOffsetLoc;
                }).Schedule(movementSystemJobHandle);

            return droneTweakJobHandle;
        }
    }
}
