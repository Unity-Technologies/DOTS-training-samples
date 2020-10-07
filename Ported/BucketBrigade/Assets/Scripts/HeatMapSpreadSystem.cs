using System;
using Unity.Entities;

public class HeatMapSpreadSystem : SystemBase
{
	protected override void OnUpdate()
	{
		FireSimulationSettings settings = GetSingleton<FireSimulationSettings>();
		var deltaTime = UnityEngine.Time.deltaTime;
	
		Entities.ForEach((Entity entity, ref HeatMap map, ref DynamicBuffer<HeatMapElement> heatMapBuffer) =>
		{
			for (int i = 0; i < heatMapBuffer.Length; i++)
			{
				float newHeatMapValue = heatMapBuffer[i].Value;
				
				if (newHeatMapValue >= BoardHelper.MaxFireValue)
					continue;
				
				int x = i % map.SizeX;
				int z = i / map.SizeX;

				if (newHeatMapValue > settings.HeatIncreaseThreshold)
					newHeatMapValue += settings.HeatIncrease * deltaTime;

				for (int offsetX = -settings.PropagationRange; offsetX <= settings.PropagationRange; offsetX++)
				{
					for (int offsetZ = -settings.PropagationRange; offsetZ <= settings.PropagationRange; offsetZ++)
					{
						if (offsetX == 0 && offsetZ == 0)
							continue;

						if (BoardHelper.TryGet2DArrayIndex(x + offsetX, z + offsetZ, map.SizeX, map.SizeZ, out var otherIndex))
						{
							if (heatMapBuffer[otherIndex].Value > settings.PropagationThreshold)
								newHeatMapValue += heatMapBuffer[otherIndex].Value * settings.PropagationTransfer * deltaTime;
						}
					}
				}

				heatMapBuffer[i] = new HeatMapElement { Value = Math.Min(BoardHelper.MaxFireValue, newHeatMapValue) };
			}

		}).Schedule();
	}
}
