
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;

public class FireSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var time = Time.ElapsedTime;
        byte flashPoint = 45;

        Entities
            .ForEach((ref Translation translation, ref URPMaterialPropertyBaseColor color, in FireCell fireCell) =>
            {
                var temperature = fireCell.Temperature;
                //temperature += (float)(Unity.Mathematics.math.sin(time)*0.1f);
                
                if (temperature < flashPoint)
                {
                    color.Value = new float4(Unity.Mathematics.math.lerp(new float3(125, 202, 117), new float3(255, 252, 131), temperature / 255.0f), 1.0f);
                    color.Value /= 255;

                } else
                {
                    color.Value = new float4(1.0f, 0.0f, 0.0f, 0.0f);
                    translation.Value.y = fireCell.Temperature / 20;
                }
            }).ScheduleParallel();
    }
}