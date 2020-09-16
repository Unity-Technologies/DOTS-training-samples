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
                yaw.CurrentYaw *= -1;
            }

            if (ant.HasFood && math.length(antPos.Value - homePos.Value) < (1 / 2f + AntTag.Size / 2f)) {
                ant.HasFood = false;
                yaw.CurrentYaw *= -1;
            }

            color.Value = ant.HasFood ? AntColorAuthoring.kFoodColor : AntColorAuthoring.kHungryColor;
        }).Run();
    }
}
