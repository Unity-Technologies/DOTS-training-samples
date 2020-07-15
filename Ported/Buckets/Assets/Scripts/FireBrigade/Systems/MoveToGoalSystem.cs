using FireBrigade.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace FireBrigade.Systems
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

                var movementVector = goalPosition.Value - translation.Value;
                movementVector = math.normalize(movementVector);
                translation.Value += movementVector * speed * deltaTime;
                
            }).ScheduleParallel();
        }
    }
}