using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering.Authoring;

[UpdateAfter(typeof(BoardInitSystem))]
public class FoodHomeCollisionSystem : SystemBase
{

    protected override void OnCreate() {
        base.OnCreate();
        RequireSingletonForUpdate<FoodTag>();
        RequireSingletonForUpdate<HomeTag>();
    }

    protected override void OnUpdate() {
        var foodEntity = GetSingletonEntity<FoodTag>();
        var foodPos = EntityManager.GetComponentData<Translation>(foodEntity);

        var homeEntity = GetSingletonEntity<HomeTag>();
        var homePos = EntityManager.GetComponentData<Translation>(homeEntity);

        float collisionThresholdSquare = ((0.5f * AntTag.Size) + 0.5f);
        collisionThresholdSquare *= collisionThresholdSquare;

        Entities
            .WithName("HomeFoodCollision")
            .ForEach((ref AntTag ant, ref AntColor color, ref Yaw yaw, ref Translation antPos) =>
        {
            
            if (!ant.HasFood && math.lengthsq(antPos.Value - foodPos.Value) < collisionThresholdSquare) {
                ant.HasFood = true;
                yaw.CurrentYaw += math.PI;
            }
            else if (ant.HasFood && math.lengthsq(antPos.Value - homePos.Value) < collisionThresholdSquare) {
                ant.HasFood = false;
                yaw.CurrentYaw += math.PI;
            }

            color.Value = ant.HasFood ? AntColorAuthoring.kFoodColor : AntColorAuthoring.kHungryColor;
            color.Value = math.lerp(color.Value, (ant.HasFood ? AntColorAuthoring.kSeeBaseColor : AntColorAuthoring.kSeeFoodColor), ant.GoalSeekAmount);
        }).ScheduleParallel();
    }
}
