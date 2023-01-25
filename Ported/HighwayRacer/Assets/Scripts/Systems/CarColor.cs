using Aspects;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

[BurstCompile]
public partial struct CarColor : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    public void OnUpdate(ref SystemState state)
    {
        var random = new Random(1234);

        foreach (var carAspect in SystemAPI.Query<CarAspect>())
        {
            float4 color = new float4(0.5f, 0.5f, 0.5f, 1f);
            if (carAspect.Acceleration > 0.001f)
            {
                color = new float4(0, 1, 0, 1);
            }else if (carAspect.Acceleration < -0.001f)
            {
                color = new float4(1, 0, 0, 1);
            }

            state.EntityManager.SetComponentData(carAspect.Self,
            new URPMaterialPropertyBaseColor
            {
                Value = color
            });
        }
    }
}
