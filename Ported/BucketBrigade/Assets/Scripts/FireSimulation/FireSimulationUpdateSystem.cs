using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class FireSimulationUpdateSystem : SystemBase
{
    Random m_Random;

    protected override void OnCreate()
    {
        m_Random = new Random(0x98209104);
    }

    protected override void OnUpdate()
    {
        FireSimulation fireSimulation = GetSingleton<FireSimulation>();
        float flameHeight = fireSimulation.maxFlameHeight;
        float time = (float)Time.ElapsedTime;
        float flickerRate = 0.4f;
        float flickerRange = 0.5f;

    Entities.
            ForEach((Entity fireCellEntity, ref Temperature temperature, ref Translation translation, ref BaseColor baseColor, in CellIndex cellIndex) =>
            {
                // TODO: Run only on cells on fire
                if (temperature.Value >= fireSimulation.flashpoint)
                {
                    float3 currentPos = translation.Value;
                    currentPos.y = (-flameHeight * 0.5f + (temperature.Value * flameHeight)) - flickerRange;
                    currentPos.y += (flickerRange * 0.5f) + UnityEngine.Mathf.PerlinNoise((time - cellIndex.Value) * flickerRate - temperature.Value, temperature.Value) * flickerRange;
                    translation.Value = currentPos;

                    UnityEngine.Color color = UnityEngine.Color.Lerp(fireSimulation.fireCellColorCool, fireSimulation.fireCellColorHot, temperature.Value);
                    baseColor.Value = new float4(color.r, color.g, color.b, color.a);
                }
            }).ScheduleParallel();
    }
}
