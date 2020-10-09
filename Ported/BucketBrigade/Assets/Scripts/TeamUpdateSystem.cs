using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class TeamUpdateSystem : SystemBase
{
	EntityQuery m_WaterQuery;
	
	protected override void OnUpdate()
	{
		int watersFoundLastUpdate = m_WaterQuery.CalculateEntityCount();

		int waterIndex = 0;
		NativeArray<float3> waterLocations = new NativeArray<float3>(watersFoundLastUpdate, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
		NativeArray<bool> waterIsAvailable = new NativeArray<bool>(watersFoundLastUpdate, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

		Entities
			.WithoutBurst()
			.WithStoreEntityQueryInField(ref m_WaterQuery)
			.ForEach((in WaterTag water, in Intensity volume, in Translation t) =>
			{
				waterLocations[waterIndex] = t.Value;
				waterIsAvailable[waterIndex] = (volume.Value > 0.0f);
				waterIndex++;
			})
			.Run();

		var heatMapEntity = GetSingletonEntity<HeatMap>();
		var heatMap = EntityManager.GetComponentData<HeatMap>(heatMapEntity);
		var heatMapBuffer = EntityManager.GetBuffer<HeatMapElement>(heatMapEntity).AsNativeArray();
		
		Entities.ForEach((Entity entity, ref Team team) =>
		{
			// Update dropOff and pickup location
			
			float3 dropOff = team.DropOffLocation;
			if (TryFindNearestFire((int)dropOff.x, (int)dropOff.z, heatMap.SizeX, heatMap.SizeZ, heatMapBuffer, out var targetDropOff))
			{
				team.DropOffLocation = targetDropOff;
			}
			
			float3 pickup = team.PickupLocation;
			if (TryFindNearestAndSetSeekTarget(pickup, waterLocations, waterIsAvailable, true, out var targetPickup))
			{
				team.PickupLocation = targetPickup;
			}
			
			Debug.DrawLine(team.DropOffLocation, team.PickupLocation, UnityEngine.Color.red);

		}).Schedule();
	}
	
	static bool TryFindNearestAndSetSeekTarget(float3 currentPos, NativeArray<float3> objectLocation, NativeArray<bool> objectFilter, bool filterMatch, out float3 target)
	{
		float nearestDistanceSquared = float.MaxValue;
		int nearestIndex = -1;
		for (int i = 0; i < objectLocation.Length; ++i)
		{
			if (objectFilter[i] == filterMatch)
			{
				float squareLen = math.lengthsq(currentPos - objectLocation[i]);

				if (squareLen < nearestDistanceSquared /*&& squareLen > 5.0f*/)
				{
					nearestDistanceSquared = squareLen;
					nearestIndex = i;
				}
			}
		}

		if (nearestIndex == -1)
		{
			target = float3.zero;
			return false;
		}

		float3 loc = objectLocation[nearestIndex];
		target = new float3(loc.x, loc.y, loc.z);
		return true;
	}
	
	static bool TryFindNearestFire(int x, int z, int sizeX, int sizeZ, NativeArray<HeatMapElement> heatMap, out float3 target)
	{
		for (int i = 0; i < heatMap.Length; i++)
		{
			float posX = x;
			float posZ = z;
			BoardHelper.ApplySpiralOffset(i, ref posX, ref posZ);

			if (BoardHelper.TryGet2DArrayIndex((int)posX, (int)posZ, sizeX, sizeZ, out var index))
			{
				if (heatMap[index].Value > 75)
				{
					target = new float3(posX, 0, posZ);
					return true;
				}
			}
		}
		
		target = float3.zero;
		return false;
	}
}
