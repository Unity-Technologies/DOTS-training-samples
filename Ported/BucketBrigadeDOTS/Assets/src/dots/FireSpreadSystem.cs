using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class FireSpreadSystem : SystemBase
{
    protected override void OnCreate()
    {
    }

    protected override void OnUpdate()
    {
        var fireGridEntity = GetSingletonEntity<FireGridSettings>();
        var fireGridSetting = GetComponent<FireGridSettings>(fireGridEntity);
        var bufferPrev = EntityManager.GetBuffer<FireCellHistory>(fireGridEntity);
        float dt = Time.DeltaTime;

        Entities
        .WithReadOnly(bufferPrev)
        .ForEach((DynamicBuffer<FireCell> buffer) =>
        {
            for (int i = 0; i < buffer.Length; ++i)
            {
                int x = i % (int)fireGridSetting.FireGridResolution.x;
                int y = i / (int)fireGridSetting.FireGridResolution.x;

                int kernelRadius = 1;
                int2 rangeX = math.clamp(new int2(x - kernelRadius, x + kernelRadius), 0, (int)fireGridSetting.FireGridResolution.x - 1);
                int2 rangeY = math.clamp(new int2(y - kernelRadius, y + kernelRadius), 0, (int)fireGridSetting.FireGridResolution.y - 1);

                FireCell cell = buffer[i];
                cell.FireTemperature = bufferPrev[i].FireTemperaturePrev;
                for (int v = rangeY.x; v <= rangeY.y; ++v)
                    for (int u = rangeX.x; u <= rangeX.y; ++u)
                    {
                        float targetHistoryTemp = bufferPrev[v * (int)fireGridSetting.FireGridResolution.x + u].FireTemperaturePrev;
                        cell.FireTemperature += targetHistoryTemp > 0.2f ? targetHistoryTemp * 0.03f * dt : 0.0f;
                    }

                cell.FireTemperature = math.min(cell.FireTemperature, 1.0f);
                buffer[i] = cell;
            }
        }).Schedule();
    }
}
