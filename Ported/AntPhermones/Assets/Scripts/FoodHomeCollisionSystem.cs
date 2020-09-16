using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class FoodHomeCollisionSystem : SystemBase
{
    protected override void OnUpdate() {
        var foodEntity = GetSingletonEntity<FoodTag>();
        var foodPos = EntityManager.GetComponentData<Translation>(foodEntity);

        var homeEntity = GetSingletonEntity<FoodTag>();
        var homePos = EntityManager.GetComponentData<Translation>(homeEntity);

        Entities.ForEach((ref AntTag ant, ref Yaw yaw, ref Translation antPos) => {
            if (!ant.HasFood && math.length(antPos.Value - foodPos.Value) < (1 / 2f + AntTag.Size / 2f)) {
                ant.HasFood = true;
                yaw.CurrentYaw += (float)Math.PI;
                antPos.Value += antPos.Value - foodPos.Value;
            }

            if (ant.HasFood && math.length(antPos.Value - homePos.Value) < (1 / 2f + AntTag.Size / 2f)) {
                ant.HasFood = false;
                yaw.CurrentYaw += (float)Math.PI;
                antPos.Value += antPos.Value - homePos.Value;
            }
        }).Run();
    }
}
