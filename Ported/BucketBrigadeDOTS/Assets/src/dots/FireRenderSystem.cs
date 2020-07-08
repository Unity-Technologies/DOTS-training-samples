using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(FireExtinguishSystem))]
public class FireRenderSystem : SystemBase
{
    const float flashPoint = 0.2f;

    protected override void OnCreate()
    {
        GetEntityQuery(typeof(FireCell));
    }

    private static float Hash01(uint seed)
    {
        seed = ((seed >> 16) ^ seed) * 0x45d9f3b;
        seed = ((seed >> 16) ^ seed) * 0x45d9f3b;
        seed = (seed >> 16) ^ seed;
        return math.asfloat((seed >> 9) | 0x3f800000) - 1.0f;
    }

    private static float Wave(float t, float amplitude, float frequency, float phase)
    {
        return amplitude * math.sin(frequency * t + phase);
    }

    protected override void OnUpdate()
    {
        var fireGridEntity = GetSingletonEntity<FireGridSettings>();
        var fireGridSetting = GetComponent<FireGridSettings>(fireGridEntity);
        var bufferRef = EntityManager.GetBuffer<FireCell>(fireGridEntity);
        float t = (float)Time.ElapsedTime;

        Entities
        .WithAll<FireRendererTag>()
        .WithReadOnly(bufferRef)
        .ForEach((int entityInQueryIndex, ref NonUniformScale scale, ref Translation translation, ref FireColor fireColor) =>
        {
            // Grab the target cell this renderer should be matching
            FireCell currentCell = bufferRef[entityInQueryIndex];

            float visualTemperature = currentCell.FireTemperature < flashPoint ? 0.001f : math.min(currentCell.FireTemperature, 1.0f);

            uint x = (uint)entityInQueryIndex % fireGridSetting.FireGridResolution.x;
            uint y = (uint)entityInQueryIndex / fireGridSetting.FireGridResolution.x;

            float noise = 0.04f * Hash01((uint)entityInQueryIndex);
            if (visualTemperature >= flashPoint)
            {
                noise += Wave(t, 0.02f, 2.3f, x * 1.1f + y * 2.9f) +
                    Wave(t, 0.02f, 2.9f, x * 2.3f - y * 2.3f) +
                    Wave(t, 0.02f, 3.1f, -x * 2.3f - y * -1.9f) +
                    Wave(t, 0.02f, 3.7f, x * -2.7f - y * -1.1f);
            }
            float temperatureWithNoise = visualTemperature + noise;

            // Update its scale
            scale.Value.y = temperatureWithNoise;
            // Update its translation
            translation.Value.y = scale.Value.y * 0.5f;
            // Update its color
            fireColor.Value = visualTemperature < flashPoint ? new float4(0.0f, 1.0f, 0.0f, 1.0f) : math.lerp(new float4(1.0f, 1.0f, 0.0f, 1.0f), new float4(1.0f, 0.0f, 0.0f, 1.0f), (visualTemperature - flashPoint) / (1.0f - flashPoint));
        }).ScheduleParallel();
    }
}
