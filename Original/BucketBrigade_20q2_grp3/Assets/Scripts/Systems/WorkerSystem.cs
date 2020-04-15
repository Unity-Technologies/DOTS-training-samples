using Unity.Entities;
using Unity.Mathematics;

public class WorkerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .ForEach((Entity e, Worker w) =>
        {

        }).Run();
    }
}