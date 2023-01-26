using Unity.Burst;
using Unity.Entities;
using UnityEngine;


/*partial class FireVisualizationSystem : SystemBase
{
    
    protected override void OnUpdate()
    {
        var speed = 1;
        var offsetRange = 0.2f;
        var runningTime = ((float)SystemAPI.Time.ElapsedTime * speed) * offsetRange;
        
        Entities
            .WithAll<OnFireTag>()
            .ForEach((ref TransformAspect transform) =>
            {
                var pos = transform.LocalPosition;
                pos.y = Mathf.Sin(runningTime);
                transform.LocalPosition = pos;

            }).Run();

        // Entities
        //     .WithAll<DisplayHeight>()
        //     .ForEach((URPMaterialPropertyBaseColor color) =>
        //     {
        //         color = new URPMaterialPropertyBaseColor();
        //     })
        //     .ScheduleParallel();
    }
}*/


[BurstCompile]
partial struct FireVisualizationSystem : ISystem
{
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        state.RequireForUpdate<FireSimSystem>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var speed = 1;
        var offsetRange = 0.2f;
        var runningTime = ((float)SystemAPI.Time.ElapsedTime * speed) * offsetRange;

        foreach (var fireCell in SystemAPI.Query<FireCellAspect>().WithAll<OnFireTag>())
        {
            var currentHeight = fireCell.DisplayHeight.ValueRO.height;
            
        }
    }
}




