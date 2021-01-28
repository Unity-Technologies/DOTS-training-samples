using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Properties;

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
		var pheromoneRenderingRef = this.GetSingleton<GameObjectRefs>().PheromoneRenderingRef;

		ObstacleBuilder map = this.GetSingleton<ObstacleBuilder>();
		float2 mapSize = new float2(map.dimensions.x / 2, map.dimensions.y / 2);

		float seekAhead = 1f;
		float speed = tuning.Speed * time;
		float angleRange = tuning.AntAngleRange;
		Unity.Mathematics.Random random = new Unity.Mathematics.Random(_random.NextUInt());

		// no line of sight, pick a direction
		Entities.
			WithAll<AntPathing>().
			WithNone<AntLineOfSight>().
			WithReadOnly(pheromoneBuffer).
			ForEach((ref AntHeading heading, ref AntTarget target, ref Rotation rotation, in Translation translation) =>
		{
			int xIndex, yIndex, gridIndex;
			float pValue;

			float headingOffset = random.NextFloat() * (angleRange);

			// calculate all targets left, right, fwd with weighting
			float degreesLeft = heading.Degrees - headingOffset;
			float radsLeft = Mathf.Deg2Rad * degreesLeft;
			float2 seekLeft = new float2(translation.Value.x + seekAhead * Mathf.Sin(radsLeft), translation.Value.y + seekAhead * Mathf.Cos(radsLeft));
			gridIndex = MapCoordinateSystem.PositionToIndex(seekLeft, tuning);
			pValue = (float)(pheromoneBuffer[gridIndex]/255f);
			float weightLeft = tuning.MinAngleWeight + tuning.PheromoneWeighting * pValue;

			float degreesRight = heading.Degrees + headingOffset;
			float radsRight = Mathf.Deg2Rad * degreesRight;
			float2 seekRight = new float2(translation.Value.x + seekAhead * Mathf.Sin(radsRight), translation.Value.y + seekAhead * Mathf.Cos(radsRight));
			gridIndex = MapCoordinateSystem.PositionToIndex(seekRight, tuning);
			pValue = (float)(pheromoneBuffer[gridIndex] / 255f);
			float weightRight = tuning.MinAngleWeight + tuning.PheromoneWeighting * pValue;

			float degreesFwd = heading.Degrees;
			float radsFwd = Mathf.Deg2Rad * degreesFwd;
			float2 seekFwd = new float2(translation.Value.x + seekAhead * Mathf.Sin(radsFwd), translation.Value.y + seekAhead * Mathf.Cos(radsFwd));
			gridIndex = MapCoordinateSystem.PositionToIndex(seekFwd, tuning);
			pValue = (float)(pheromoneBuffer[gridIndex] / 255f);
			float weightFwd = tuning.MinAngleWeight + tuning.AntFwdWeighting + tuning.PheromoneWeighting * pValue;

			float totalWeight = weightLeft + weightRight + weightFwd;
			float randomWeight = random.NextFloat() * totalWeight;

			// select random weighted target, reproject ahead the move distance
			float rads = 0;
			if (randomWeight < weightLeft)
			{
				target.Target = new float2(translation.Value.x + speed * Mathf.Sin(radsLeft), translation.Value.y + speed * Mathf.Cos(radsLeft));
				heading.Degrees = degreesLeft;
				rads = radsLeft;
			} 
			else if (randomWeight < weightLeft + weightRight)
			{
				target.Target = new float2(translation.Value.x + speed * Mathf.Sin(radsRight), translation.Value.y + speed * Mathf.Cos(radsRight));
				heading.Degrees = degreesRight;
				rads = radsRight;
			}
			else
			{
				target.Target = new float2(translation.Value.x + speed * Mathf.Sin(radsFwd), translation.Value.y + speed * Mathf.Cos(radsFwd));
				heading.Degrees = degreesFwd;
				rads = radsFwd;
			}

			heading.Degrees = heading.Degrees % 360;

			rotation.Value = quaternion.EulerXYZ(0, 0, -rads);

		}).ScheduleParallel();

		// has line of sight, straight path to goal
		Entities.
			WithAll<AntLineOfSight>().
			ForEach((ref Translation translation, ref Rotation rotation, in AntLineOfSight antLos) =>
			{
				float rads = Mathf.Deg2Rad * antLos.DegreesToGoal;

				translation.Value.x = translation.Value.x + speed * Mathf.Sin(rads);
				translation.Value.y = translation.Value.y + speed * Mathf.Cos(rads);

				rotation.Value = quaternion.EulerXYZ(0, 0, -rads);
			}).ScheduleParallel();

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

					heading.Degrees = Mathf.Rad2Deg * Mathf.Atan2(outVector.x, outVector.y);

					// reset target to previous position
					target.Target.x = translation.Value.x;
					target.Target.y = translation.Value.y;
				}
			}).ScheduleParallel();

		// copy target into translation.  we have already done this if we have LineOfSight because we ignore collision tests
		Entities.
			WithAll<AntPathing>().
			WithNone<AntLineOfSight>().
			ForEach((ref Translation translation, in AntTarget target)=>
		{
			translation.Value.x = target.Target.x;
			translation.Value.y = target.Target.y;
		}).ScheduleParallel();
	}
}
