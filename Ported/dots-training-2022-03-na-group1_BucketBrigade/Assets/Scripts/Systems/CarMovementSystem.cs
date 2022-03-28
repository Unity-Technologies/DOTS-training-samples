using Unity.Entities;
using Unity.Transforms;

namespace Tutorial
{
    #region Tutorial
    public partial class CarMovementSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var time = Time.ElapsedTime;
/*
            Entities
                .ForEach((ref Translation translation, in CarMovement movement) =>
                {
                    translation.Value.x = (float) ((time + movement.Offset) % 100) - 50f;
                }).ScheduleParallel();
                */
        }
    }
    #endregion
}