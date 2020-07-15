using FireBrigade.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace UnityTemplateProjects.FireBrigade.Systems
{
    public class MoveToGoalSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;
            float speed = 10f;
            Entities.ForEach((Entity EntityManager,
                ref Translation translation,
                in GoalPosition goalPosition) =>
            {
                if (math.distance(goalPosition.Value, translation.Value) < 0.1f) return;

                var vec = goalPosition.Value - translation.Value;
                vec = math.normalize(vec);
                translation.Value += vec * speed * deltaTime;
            }).ScheduleParallel();
        }
    }
}