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

		var heatMapEntity = GetSingletonEntity<HeatMap>();
		var heatMap = EntityManager.GetComponentData<HeatMap>(heatMapEntity);
		var heatMapBuffer = EntityManager.GetBuffer<HeatMapElement>(heatMapEntity).AsNativeArray();

		Entities
			.ForEach((Entity entity, ref Translation translation, ref NonUniformScale scale, ref Color color, in CellInfo cell, in RootTranslation rootTranslation) =>
			{
				BoardHelper.TryGet2DArrayIndex(cell.X, cell.Z, heatMap.SizeX, heatMap.SizeZ, out var index);
				float heatValue = heatMapBuffer[index].Value;
				
				float t = math.clamp((heatValue - cellDisplay.CoolValue) / cellDisplayRange, 0.0f, 1.0f);
				float top = math.lerp(cellDisplay.CoolHeight, cellDisplay.FireHeight, t) + rootTranslation.Value.y;
				float bottom = -1.0f;
				translation.Value = new float3(rootTranslation.Value.x, (top + bottom) / 2.0f, rootTranslation.Value.z);
				scale.Value = new float3(scale.Value.x, top - bottom, scale.Value.z);
				color.Value = (cellDisplay.CoolColor * (1.0f - t)) + (cellDisplay.FireColor * t);
			})
			.ScheduleParallel();
	}
}
