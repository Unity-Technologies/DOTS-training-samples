using Unity.Entities;
using Unity.Mathematics;

public struct Worker : IComponentData
{

}

public struct WorkerMoveTo : IComponentData
{
    public float2 Start, End;
}

public class WorkerMoveToSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .ForEach((Entity e, Worker w, ref WorkerMoveTo target) =>
        {

        }).Run();
    }
}