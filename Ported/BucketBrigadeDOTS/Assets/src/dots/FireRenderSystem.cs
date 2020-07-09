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
        var mipChainFlag = EntityManager.GetBuffer<FireCellFlag>(fireGridEntity);
        float t = (float)Time.ElapsedTime;

        Entities
        .WithAll<FireRendererTag>()
        .WithReadOnly(bufferRef)
        .WithReadOnly(mipChainFlag)
        .ForEach((int entityInQueryIndex, ref LocalToWorld transform, ref FireColor fireColor) =>
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

            float4x4 mat = transform.Value;
            float4 c1 = transform.Value.c1;
            float4 c3 = transform.Value.c3;
            c1.y = temperatureWithNoise;
            c3.y = temperatureWithNoise * 0.5f;
            mat.c1 = c1;
            mat.c3 = c3;
            transform.Value = mat;

            // Update its color
            fireColor.Value = visualTemperature < flashPoint ? new float4(0.0f, 1.0f, 0.0f, 1.0f) : math.lerp(new float4(1.0f, 1.0f, 0.0f, 1.0f), new float4(1.0f, 0.0f, 0.0f, 1.0f), (visualTemperature - flashPoint) / (1.0f - flashPoint));

            // To visualize our mips
            if (fireGridSetting.MipDebugIndex >= 0)
            {
                int mipIndex = fireGridSetting.MipDebugIndex;
                int mipOffset = 0;
                for (int moI = 0; moI < mipIndex; ++moI)
                {
                    int2 subResolution = (int2)fireGridSetting.FireGridResolution >> moI;
                    mipOffset += (subResolution.x * subResolution.y);
                }
                uint subX = x >> mipIndex;
                uint subY = y >> mipIndex;
                int halfResPosition = (int)(subX + subY * (fireGridSetting.FireGridResolution.x >> mipIndex));
                int mipChainIndex = halfResPosition + mipOffset;
                FireCellFlag currentCellFlag = mipChainFlag[mipChainIndex];
                fireColor.Value = currentCellFlag.OnFire ? new float4(1.0f, 0.0f, 1.0f, 1.0f) : new float4(0.0f, 0.0f, 1.0f, 1.0f);
            }

        }).ScheduleParallel();
    }
}
