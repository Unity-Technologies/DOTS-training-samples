using System;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class CellDisplaySystem : SystemBase
{
	const float k_FlickerRange = 0.4f;
	const float k_FlickerRate = 2f;
	const float k_DefaultCellHeight = 1.0f;
	
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
		var elapsedTime = (float)Time.ElapsedTime;
		
		CellDisplay cellDisplay = GetSingleton<CellDisplay>();
		float cellDisplayRange = cellDisplay.FireValue - cellDisplay.CoolValue;

		var heatMapEntity = GetSingletonEntity<HeatMap>();
		var heatMap = EntityManager.GetComponentData<HeatMap>(heatMapEntity);
		var heatMapBuffer = EntityManager.GetBuffer<HeatMapElement>(heatMapEntity).AsNativeArray();

		Entities
			.ForEach((Entity entity, ref Translation translation, ref NonUniformScale scale, ref Color color, in CellInfo cell, in RootTranslation rootTranslation) =>
			{
				BoardHelper.TryGet2DArrayIndex(cell.X, cell.Z, heatMap.SizeX, heatMap.SizeZ, out var index);
				float heatValue = heatMapBuffer[index].Value;

				float height;
				if (heatValue > cellDisplay.OnFireThreshold)
				{
					float t = math.clamp((heatValue - cellDisplay.CoolValue) / cellDisplayRange, 0.0f, 1.0f);
					float top = math.lerp(cellDisplay.CoolHeight, cellDisplay.FireHeight, t) + rootTranslation.Value.y;
					// Animate cell
					top += (k_FlickerRange * 0.5f) + Mathf.PerlinNoise((elapsedTime - index) * k_FlickerRate - heatValue / 100f, heatValue / 100f) * k_FlickerRange;
					height = top + k_DefaultCellHeight;
					color.Value = (cellDisplay.CoolColor * (1.0f - t)) + (cellDisplay.FireColor * t);
				}
				else
				{
					height = rootTranslation.Value.y + k_DefaultCellHeight;
					color.Value = cellDisplay.NeutralColor;
				}
				
				scale.Value = new float3(scale.Value.x,height, scale.Value.z);
				translation.Value = new float3(rootTranslation.Value.x, (height / 2.0f) - k_DefaultCellHeight, rootTranslation.Value.z);
			})
			.ScheduleParallel();
	}
}
