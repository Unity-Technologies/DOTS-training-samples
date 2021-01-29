using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

//[UpdateBefore(typeof(AntPheromoneDropSystem))]
public class AntPathingSystem : SystemBase
{
	Unity.Mathematics.Random _random;

	protected override void OnCreate()
	{
		base.OnCreate();
		
		RequireSingletonForUpdate<PheromoneStrength>();
		RequireSingletonForUpdate<Tuning>();
		RequireSingletonForUpdate<RingElement>();
		RequireSingletonForUpdate<GameTime>();
		RequireSingletonForUpdate<ObstacleBuilder>();

		_random = new Unity.Mathematics.Random(1234);
	}

	protected override void OnUpdate()
	{
		var time = GetSingleton<GameTime>().DeltaTime;

		Tuning tuning = this.GetSingleton<Tuning>();

		Entity pheromoneEntity = GetSingletonEntity<PheromoneStrength>();
		DynamicBuffer<PheromoneStrength> pheromoneBuffer = GetBuffer<PheromoneStrength>(pheromoneEntity);

		ObstacleBuilder map = this.GetSingleton<ObstacleBuilder>();
		float2 mapSize = new float2(map.dimensions.x / 2, map.dimensions.y / 2);

		float seekAhead = 2f;
		float speed = tuning.Speed * time;
		float angleRange = tuning.AntAngleRange;
		Random random = new Random(_random.NextUInt());

		
		//Debug.Log($"angleRange {angleRange}");
		//Debug.Log($"seekAhead {seekAhead}");
		// Debug.Log($"speed {speed}");
		
		float headingOffset = random.NextFloat() * (angleRange);
		Debug.Log($"headingOffset {headingOffset}");
		var jobHandleA = new JobHandle();
		var jobHandleB = new JobHandle();
		var jobHandleC = new JobHandle();
		
		//Left
		Entities.
			WithAll<AntPathing>().
			WithNone<AntLineOfSight>().
			WithNativeDisableContainerSafetyRestriction(pheromoneBuffer).
			ForEach((ref WeightLeft weight, in Translation translation,in AntHeading heading) =>
		{
			weight.Degrees = heading.Degrees - headingOffset;
			weight.Rads = math.radians(weight.Degrees);
			var seek = new float2(translation.Value.x + seekAhead * math.sin(weight.Rads), translation.Value.y + seekAhead * math.cos(weight.Rads));
			int grid = MapCoordinateSystem.PositionToIndex(seek, tuning);
			var pValue = (pheromoneBuffer[grid])/255f;
			weight.Weight = tuning.MinAngleWeight + tuning.PheromoneWeighting * pValue;
		
			//Debug.Log($"1 angleRange {angleRange}");
			Debug.Log($"inside headingOffset {headingOffset}");
			// Debug.Log($"1headingOffset {headingOffset}");
			// Debug.Log($"1weight.Degrees {weight.Degrees}");
			// Debug.Log($"1weight.Rads {weight.Rads}");
			// Debug.Log($"1seek {seek}");
			// Debug.Log($"1grid {grid}");
			// Debug.Log($"1pValue {pValue}");
			// Debug.Log($"1weight.Weight {weight.Weight}");
		}).ScheduleParallel();

		//Right
		Entities.
			WithAll<AntPathing>().
			WithNone<AntLineOfSight>().
			WithReadOnly(pheromoneBuffer).
			ForEach((ref WeightRight weight, in Translation translation,in AntHeading heading) =>
			{
				weight.Degrees = heading.Degrees + headingOffset;
				weight.Rads = math.radians(weight.Degrees);
				var seek = new float2(translation.Value.x + seekAhead * math.sin(weight.Rads), translation.Value.y + seekAhead * math.cos(weight.Rads));
				int grid = MapCoordinateSystem.PositionToIndex(seek, tuning);
				var pValue = (pheromoneBuffer[grid])/255f;
				weight.Weight = tuning.MinAngleWeight + tuning.PheromoneWeighting * pValue;
				//
				// Debug.Log($"2angleRange {angleRange}");
				// Debug.Log($"2seekAhead {seekAhead}");
				// Debug.Log($"2headingOffset {headingOffset}");
				// Debug.Log($"2weight.Degrees {weight.Degrees}");
				// Debug.Log($"2weight.Rads {weight.Rads}");
				// Debug.Log($"2seek {seek}");
				// Debug.Log($"2grid {grid}");
				// Debug.Log($"2pValue {pValue}");
				// Debug.Log($"2weight.Weight {weight.Weight}");
			}).ScheduleParallel();

		// Forward
		Entities.
			WithAll<AntPathing>().
			WithNone<AntLineOfSight>().
			WithReadOnly(pheromoneBuffer).
			ForEach((ref WeightForward weight, in Translation translation,in AntHeading heading) =>
			{
				weight.Degrees = heading.Degrees;
				weight.Rads = math.radians(heading.Degrees);
				var seek = new float2(translation.Value.x + seekAhead * math.sin(weight.Rads), translation.Value.y + seekAhead * math.cos(weight.Rads));
				int grid = MapCoordinateSystem.PositionToIndex(seek, tuning);
				var pValue = (pheromoneBuffer[grid])/255f;
				weight.Weight = (tuning.MinAngleWeight + tuning.PheromoneWeighting * pValue) * 3;
				
				
				// Debug.Log($"3angleRange {angleRange}");
				// Debug.Log($"3seekAhead {seekAhead}");
				// Debug.Log($"3weight.Degrees {weight.Degrees}");
				// Debug.Log($"3weight.Rads {weight.Rads}");
				// Debug.Log($"3seek {seek}");
				// Debug.Log($"3grid {grid}");
				// Debug.Log($"3pValue {pValue}");
				// Debug.Log($"3weight.Weight {weight.Weight}");
			}).ScheduleParallel();

		//Dependency = JobHandle.CombineDependencies(jobHandleA, jobHandleB, jobHandleC);
		
		Entities.
			WithAll<AntPathing>().
			WithNone<AntLineOfSight>().
			ForEach((ref AntHeading heading, ref AntTarget target, ref Rotation rotation,
				in WeightForward forward, in WeightLeft left, in WeightRight right, in Translation translation) =>
		{

			float totalWeight = left.Weight + right.Weight + forward.Weight;
			float randomWeight = random.NextFloat() * totalWeight;
			
			//Debug.Log($"Left = {left.Weight} right = {right.Weight}, forward = {forward.Weight}" );
			//Debug.Log($"Weights = {myGrid} / {gridLeft} {gridFwd} {gridRight} / {weightLeft} {weightFwd} {weightRight} - random = {randomWeight}");				

			// select random weighted target, reproject ahead the move distance
			float rads = 0;
			if (randomWeight <  left.Weight)
			{
				target.Target = new float2(translation.Value.x + speed * math.sin(left.Rads), translation.Value.y + speed * math.cos(left.Rads));
				heading.Degrees = left.Degrees;
				rads = left.Rads;
				//Debug.Log("LEFT");
			} 
			else if (randomWeight <  left.Weight + right.Weight)
			{
				target.Target = new float2(translation.Value.x + speed * math.sin(right.Rads), translation.Value.y + speed * math.cos(right.Rads));
				heading.Degrees = right.Degrees;
				rads = right.Rads;
				//Debug.Log("RIGHT");
			}
			else
			{
				target.Target = new float2(translation.Value.x + speed * math.sin(forward.Rads), translation.Value.y + speed * math.cos(forward.Rads));
				heading.Degrees = forward.Degrees;
				rads = forward.Rads;
				//Debug.Log("FWD");
			}

			heading.Degrees = heading.Degrees % 360;

			rotation.Value = quaternion.EulerXYZ(0, 0, -rads);

		}).Schedule();
		
		// has line of sight, straight path to goal
		Entities.
			WithAll<AntLineOfSight>().
			WithAll<AntHeading>().
			ForEach((ref Translation translation, ref Rotation rotation, in AntHeading antHeading) =>
			{
				float rads = math.radians(antHeading.Degrees);
		
				translation.Value.x = translation.Value.x + speed * math.sin(rads);
				translation.Value.y = translation.Value.y + speed * math.cos(rads);
		
				rotation.Value = quaternion.EulerXYZ(0, 0, -rads);
			}).Schedule();
		
		var ringEntity = GetSingletonEntity<RingElement>();
		DynamicBuffer<RingElement> rings = GetBuffer<RingElement>(ringEntity);
		
		// wall collision tests - only if no line of sight
		Entities.
			WithAll<AntPathing>().
			WithNone<AntLineOfSight>().
			WithReadOnly(rings).
			ForEach((ref AntTarget target, ref AntHeading heading, in Translation translation) =>
			{
				bool didCollide = false;
				bool reflectNormal = false;
				float2 normal = float2.zero;
				float2 outVector = float2.zero;
		
				// test if we're outside the map
				if (target.Target.x >= mapSize.x)
				{
					didCollide = true;
					outVector = new float2(-(target.Target.x - translation.Value.x), target.Target.y - translation.Value.y);
				}
				else if (target.Target.x <= -mapSize.x)
				{
					didCollide = true;
					outVector = new float2(-(target.Target.x - translation.Value.x), target.Target.y - translation.Value.y);
				}
				else if (target.Target.y >= mapSize.y)
				{
					didCollide = true;
					outVector = new float2(target.Target.x - translation.Value.x, -(target.Target.y - translation.Value.y));
				}
				else if (target.Target.y <= -mapSize.y)
				{
					didCollide = true;
					outVector = new float2(target.Target.x - translation.Value.x, -(target.Target.y - translation.Value.y));
				}
				else
				{
					// test if we're colliding with any rings
					for (int i = 0; i < rings.Length; i++)
					{
						RingElement ring = rings[i];
		
						float2 startPos = new float2(translation.Value.x, translation.Value.y);
		
						if (WorldResetSystem.DoesPathCollideWithRing(ring, startPos, target.Target, out float2 collisionPos, out bool outWards))
						{
							didCollide = true;
							reflectNormal = true;
		
							if (outWards)
							{
								normal = -math.normalize(collisionPos);
							}
							else
							{
								normal = math.normalize(collisionPos);
							}
		
							break;
						}
					}
				}
		
				// handle collision
				if (didCollide)
				{
					if (reflectNormal)
					{
						float2 vIn = new float2(target.Target.x - translation.Value.x, target.Target.y - translation.Value.y);
						vIn = math.normalize(vIn);
		
						outVector = vIn - 2 * math.dot(vIn, normal) * normal;
					}
		
					heading.Degrees = math.radians(math.atan2(outVector.x, outVector.y));
		
					// reset target to previous position
					target.Target.x = translation.Value.x;
					target.Target.y = translation.Value.y;
				}
			}).Schedule();
		
		// copy target into translation.  we have already done this if we have LineOfSight because we ignore collision tests
		Entities.
			WithAll<AntPathing>().
			WithNone<AntLineOfSight>().
			ForEach((ref Translation translation, in AntTarget target)=>
		{
			translation.Value.x = target.Target.x;
			translation.Value.y = target.Target.y;
		}).Schedule();
	}
	
	[BurstCompile]
	private struct GetLeftWeightJob : IJobParallelFor
	{
		public Random Random;
		public float AngleRange;
		public Tuning Tuning;
		public float SeekAhead;

		public NativeArray<WeightLeft> WeightLefts;
		public NativeArray<AntHeading> Headings;
		public NativeArray<Translation> Translations;
		
		private NativeArray<PheromoneStrength> PheromoneBuffer;
		
		public void Execute(int index)
		{
			WeightLeft weight = WeightLefts[index];
			AntHeading heading = Headings[index];
			Translation translation = Translations[index];
			
			float headingOffset = Random.NextFloat() * (AngleRange);
			weight.Degrees = heading.Degrees - headingOffset;
			weight.Rads = Mathf.Deg2Rad * weight.Degrees;
			var seek = new float2(translation.Value.x + SeekAhead * Mathf.Sin(weight.Rads), translation.Value.y + SeekAhead * Mathf.Cos(weight.Rads));
			int grid = MapCoordinateSystem.PositionToIndex(seek, Tuning);
			var pValue = (PheromoneBuffer[grid])/255f;
			weight.Weight = Tuning.MinAngleWeight + Tuning.PheromoneWeighting * pValue;

			WeightLefts[index] = weight;
			Headings[index] = heading;
			Translations[index] = translation;
		}
	}
}
