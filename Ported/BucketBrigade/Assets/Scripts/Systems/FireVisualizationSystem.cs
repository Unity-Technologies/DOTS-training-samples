using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(FireSimSystem))]
[BurstCompile]
partial struct FireVisualizationSystem : ISystem
{
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Debug.Log("fire vis system update :')");

        var config = SystemAPI.GetSingleton<Config>();
        var defaultTempColor = config.defaultTemperatureColour;
        var lowTempColor = config.lowTemperatureColour;
        var highTempColor = config.highTemperatureColour;
        
        var speed = 1;
        var offsetMultiplier = 0.2f;
        var timeAdjusted = ((float) SystemAPI.Time.ElapsedTime * speed);
        var oscillatingTime = Mathf.Sin(timeAdjusted);
        var color = new float4(5, 0, 0, 1);

        foreach (var flameCellAspect in SystemAPI.Query<FireCellAspect>())
        {
            var flameCell = flameCellAspect.Self;
            var displayHeight = flameCellAspect.DisplayHeight.ValueRO.height;
            var lerpedColor = Color.Lerp(lowTempColor, highTempColor, displayHeight);
            
            
            if (displayHeight > 0)
            {
                state.EntityManager.SetComponentData(flameCell, new URPMaterialPropertyBaseColor(){Value = (UnityEngine.Vector4)lerpedColor});
            }

            var location = state.EntityManager.GetAspect<TransformAspect>(flameCell).LocalPosition;
            
            




            //flameCell.ValueRW = new URPMaterialPropertyBaseColor(){Value = color};

            //state.EntityManager.SetComponentData(flameCell.Self, new URPMaterialPropertyBaseColor(){Value = color});
            //Debug.Log(state.EntityManager.GetComponentData<URPMaterialPropertyBaseColor>(flameCell.Self).Value);

            //ecb.SetComponent(flameCell.Self, new URPMaterialPropertyBaseColor {Value = color});

            // URPMaterialPropertyBaseColor baseColor = new URPMaterialPropertyBaseColor();
            // baseColor.Value = new float4(fireColor.r, fireColor.g, fireColor.b, fireColor.a);
            // state.WorldUnmanaged.EntityManager.SetComponentData(flameCell.Self, baseColor);
        }


        /*var updateVisualsJob = new UpdateVisuals()
        {
            adjustedTime = oscillatingTime,
            offsetMult = offsetMultiplier
        };
        updateVisualsJob.Schedule();*/
    }
}

/*[UpdateAfter(typeof(FireSimSystem))]
[WithAll(typeof(OnFireTag))]
[BurstCompile]
partial struct UpdateVisuals : IJobEntity
{
    public float adjustedTime;
    public float offsetMult;

    void Execute(ref TransformAspect transformAspect, ref URPMaterialPropertyBaseColor color, Entity entity, in DisplayHeight displayHeight)
    {
        //var offset = displayHeight.height + offsetMult * adjustedTime;
        //var transformY = transformAspect.LocalPosition.y + offset;
        //
        //transformAspect.LocalPosition = new float3(0,transformY,0);
        
        color.Value = new float4(5, 0, 0, 1);
        
        Debug.Log(color);
    }
}
}*/




