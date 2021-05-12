using Unity.Entities;

public class SetupArrowColorSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity e) =>
        {
            
        }).ScheduleParallel();
    }
}
