using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering.Authoring;

public class FoodHomeCollisionSystem : SystemBase
{
    protected override void OnUpdate() {
        var foodEntity = GetSingletonEntity<FoodTag>();
        var foodPos = EntityManager.GetComponentData<Translation>(foodEntity);

        var homeEntity = GetSingletonEntity<HomeTag>();
        var homePos = EntityManager.GetComponentData<Translation>(homeEntity);

        Entities.ForEach((ref AntTag ant, ref AntColor color, ref Yaw yaw, ref Translation antPos) => {
            if (!ant.HasFood && math.length(antPos.Value - foodPos.Value) < (1 / 2f + AntTag.Size / 2f)) {
                ant.HasFood = true;
                yaw.CurrentYaw += math.PI;
            }

            if (ant.HasFood && math.length(antPos.Value - homePos.Value) < (1 / 2f + AntTag.Size / 2f)) {
                ant.HasFood = false;
                yaw.CurrentYaw += math.PI;
            }

            color.Value = ant.HasFood ? AntColorAuthoring.kFoodColor : AntColorAuthoring.kHungryColor;
            color.Value = math.lerp(color.Value, (ant.HasFood ? AntColorAuthoring.kSeeBaseColor : AntColorAuthoring.kSeeFoodColor), ant.GoalSeekAmount);
        }).Run();
    }
}
