using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class CellDisplaySystem : SystemBase
{
	protected override void OnStartRunning()
	{
		Entities.WithAll<CellDisplay>()
			.WithoutBurst()
			.ForEach((ref CellDisplay cellDisplay) =>
			{
				SetSingleton<CellDisplay>(cellDisplay);
			})
			.Run();
	}

	protected override void OnUpdate()
	{
		CellDisplay cellDisplay = GetSingleton<CellDisplay>();
		float cellDisplayRange = cellDisplay.FireValue - cellDisplay.CoolValue;

		Entities
			.ForEach((ref Translation translation, ref NonUniformScale scale, ref Color color, in CellTag cell, in Intensity intensity, in RootTranslation rootTranslation) =>
			{
				float t = math.clamp((intensity.Value - cellDisplay.CoolValue) / cellDisplayRange, 0.0f, 1.0f);
				float top = math.lerp(cellDisplay.CoolHeight, cellDisplay.FireHeight, t) + rootTranslation.Value.y;
				float bottom = -1.0f;
				translation.Value = new float3(rootTranslation.Value.x, (top + bottom) / 2.0f, rootTranslation.Value.z);
				scale.Value = new float3(scale.Value.x, top - bottom, scale.Value.z);
				color.Value = (cellDisplay.CoolColor * (1.0f - t)) + (cellDisplay.FireColor * t);
			})
			.ScheduleParallel();
	}
}
