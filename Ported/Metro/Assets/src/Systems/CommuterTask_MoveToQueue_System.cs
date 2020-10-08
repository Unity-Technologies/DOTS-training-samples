using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class CommuterTask_MoveToQueue_System : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();
    }

    protected override void OnUpdate()
    {
        //Entities
        //    .WithName("commuter_task_movetoqueue")
        //    .WithStructuralChanges()
        //    .WithAll<CommuterTask_MoveToQueue>()
        //    .WithNone<TargetPoint>()
        //    .ForEach((Entity commuter) =>
        //    {
        //        EntityManager.RemoveComponent<CommuterTask_MoveToQueue>(commuter);
        //        EntityManager.AddComponent<CommuterTask_Idle>(commuter);
        //    }).Run();
    }
}
