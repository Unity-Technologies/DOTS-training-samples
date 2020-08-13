using System.Transactions;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

public class ColorUpdaterSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var fireSpreadSettings = GetSingleton<FireSpreadSettings>();
        var tileDisplaySettings = GetSingleton<TileDisplaySettings>();
        Entities
            .WithName("ColorUpdater")
            .ForEach((Entity entity, 
                ref Color color, ref Translation translation, 
                in Temperature temperature) =>
            {
                // ref NonUniformScale scale, 
                float maxHeight = tileDisplaySettings.FlameHeight;
                float3 position = translation.Value;
                // TODO: add a tag for OnFire so this system can modify only the cells on fire
                if (temperature.Value > fireSpreadSettings.flashpoint)
                {
                    color.Value = math.lerp(tileDisplaySettings.ColorFireCool, tileDisplaySettings.ColorFireHot,
                        temperature.Value);
                    position.y = maxHeight * (-0.5f + temperature.Value);
                }
                else
                {
                    color.Value = tileDisplaySettings.ColorFireNeutral;
                    position.y = maxHeight * (-0.5f);
                }

                translation.Value = position;
            }).ScheduleParallel();
    }
}