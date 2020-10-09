using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class TeamUpdateSystem : SystemBase
{
	const int k_MinHeatConsideredFire = 25;
	
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
			.ForEach((in WaterTag water, in Intensity volume, in LocalToWorld world) =>
			{
				waterLocations[waterIndex] = world.Position;
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
			float3 pickup = team.PickupLocation;
			if (TryFindNearestAndSetSeekTarget(pickup, waterLocations, waterIsAvailable, true, out var targetPickup))
			{
				team.PickupLocation = targetPickup;
			}
			
			float3 dropOff = team.DropOffLocation;

			bool stillValidFirePosition = false;
			if (BoardHelper.TryGet2DArrayIndex((int)dropOff.x, (int)dropOff.z, heatMap.SizeX, heatMap.SizeZ, out var currentIndex))
			{
				if (heatMapBuffer[currentIndex].Value >= k_MinHeatConsideredFire)
				{
					stillValidFirePosition = true;
				}
			}

			if (!stillValidFirePosition)
			{
				// Find nearest next fire between the dropOff and pickup locations
				int posX = (int)(dropOff.x * 0.5f + team.PickupLocation.x * 0.5f);
				int posZ = (int)(dropOff.z * 0.5f + team.PickupLocation.z * 0.5f);
				
				if (TryFindNearestFire(posX, posZ, heatMap.SizeX, heatMap.SizeZ, heatMapBuffer, out var targetDropOff))
				{
					team.DropOffLocation = targetDropOff;
				}	
			}
			
			Debug.DrawLine(team.DropOffLocation, team.PickupLocation, UnityEngine.Color.red);

		}).Schedule();

		waterLocations.Dispose(Dependency);
		waterIsAvailable.Dispose(Dependency);

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
		for (int i = 0; i < heatMap.Length * 2; i++)
		{
			float posX = x;
			float posZ = z;
			BoardHelper.ApplySpiralOffset(i, ref posX, ref posZ);

			if (BoardHelper.TryGet2DArrayIndex((int)posX, (int)posZ, sizeX, sizeZ, out var index))
			{
				if (heatMap[index].Value > k_MinHeatConsideredFire)
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
