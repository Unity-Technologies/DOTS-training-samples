//using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class WaterVisualStateSystem: SystemBase
{
	private const float k_emptyAlpha = 0.25f;
	private const float k_fullAlpha = 1.0f;

	protected override void OnUpdate()
	{
		Entities
			.ForEach((ref Color col, in Intensity waterVolume, in Water water) =>
			{
				float fullness = math.clamp(waterVolume.Value / water.MaxVolume, 0.0f, 1.0f);
				float alpha = math.lerp(k_emptyAlpha, k_fullAlpha, fullness);
				col.Value = new float4(col.Value.x, col.Value.y, col.Value.z, alpha);
			})
			.ScheduleParallel();
	}
}
