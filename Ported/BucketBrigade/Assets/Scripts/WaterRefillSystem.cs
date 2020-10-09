using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class WaterRefillSystem : SystemBase
{
	public const float RefillRate = 0.002f;

	protected override void OnUpdate()
	{
	    Entities
			.ForEach((ref Intensity volume, in Water water) =>
		    {
			    if (volume.Value < water.MaxVolume)
			    {
					volume.Value = math.min(volume.Value + RefillRate, water.MaxVolume);
			    }
		    })
		    .ScheduleParallel();
	}
}
