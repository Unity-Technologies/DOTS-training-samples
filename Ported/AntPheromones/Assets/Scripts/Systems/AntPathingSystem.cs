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
		
		_random = new Unity.Mathematics.Random(1234);
	}

	protected override void OnUpdate()
	{
		var time = Time.DeltaTime;

		Tuning tuning = this.GetSingleton<Tuning>();

		Entity pheromoneEntity = GetSingletonEntity<PheromoneStrength>();
		DynamicBuffer<PheromoneStrength> pheromoneBuffer = GetBuffer<PheromoneStrength>(pheromoneEntity);
		var pheromoneRenderingRef = this.GetSingleton<GameObjectRefs>().PheromoneRenderingRef;

		float seekAhead = 1f;
		float speed = tuning.Speed * Time.DeltaTime;
		float angleRange = tuning.AntAngleRange;
		Unity.Mathematics.Random random = new Unity.Mathematics.Random(_random.NextUInt());

		// no line of sight, pick a direction
		Entities.
			WithAll<AntPathing>().
			WithNone<AntLineOfSight>().
			ForEach((ref AntHeading heading, ref AntTarget target, ref Rotation rotation, in Translation translation) =>
		{
			int xIndex, yIndex, gridIndex;
			float pValue;

			float headingOffset = random.NextFloat() * (angleRange);

			// calculate all targets left, right, fwd with weighting
			float degreesLeft = heading.Degrees - headingOffset;
			float radsLeft = Mathf.Deg2Rad * degreesLeft;
			float2 seekLeft = new float2(translation.Value.x + seekAhead * Mathf.Sin(radsLeft), translation.Value.y + seekAhead * Mathf.Cos(radsLeft));
			xIndex = (int)math.floor(((seekLeft.x / tuning.WorldSize) + tuning.WorldOffset.x));
			yIndex = (int)math.ceil(((seekLeft.y / tuning.WorldSize) + tuning.WorldOffset.y));
			gridIndex = (int)math.clamp((yIndex * tuning.Resolution) + xIndex, 0, (tuning.Resolution * tuning.Resolution) - 1);
			pValue = (float)(pheromoneBuffer[gridIndex]/255f);
			float weightLeft = tuning.MinAngleWeight + tuning.PheromoneWeighting * pValue;

			float degreesRight = heading.Degrees + headingOffset;
			float radsRight = Mathf.Deg2Rad * degreesRight;
			float2 seekRight = new float2(translation.Value.x + seekAhead * Mathf.Sin(radsRight), translation.Value.y + seekAhead * Mathf.Cos(radsRight));
			xIndex = (int)math.floor(((seekRight.x / tuning.WorldSize) + tuning.WorldOffset.x));
			yIndex = (int)math.ceil(((seekRight.y / tuning.WorldSize) + tuning.WorldOffset.y));
			gridIndex = (int)math.clamp((yIndex * tuning.Resolution) + xIndex, 0, (tuning.Resolution * tuning.Resolution) - 1);
			pValue = (float)(pheromoneBuffer[gridIndex] / 255f);
			float weightRight = tuning.MinAngleWeight + tuning.PheromoneWeighting * pValue;

			float degreesFwd = heading.Degrees;
			float radsFwd = Mathf.Deg2Rad * degreesFwd;
			float2 seekFwd = new float2(translation.Value.x + seekAhead * Mathf.Sin(radsFwd), translation.Value.y + seekAhead * Mathf.Cos(radsFwd));
			xIndex = (int)math.floor(((seekFwd.x / tuning.WorldSize) + tuning.WorldOffset.x));
			yIndex = (int)math.ceil(((seekFwd.y / tuning.WorldSize) + tuning.WorldOffset.y));
			gridIndex = (int)math.clamp((yIndex * tuning.Resolution) + xIndex, 0, (tuning.Resolution * tuning.Resolution) - 1);
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

			rotation.Value = quaternion.EulerXYZ(0, 0, -rads);

		}).Run();

		// has line of sight, straight path to goal
		Entities.
			WithAll<AntLineOfSight>().
			ForEach((ref Translation translation, ref Rotation rotation, in AntLineOfSight antLos) =>
			{
				float rads = Mathf.Deg2Rad * antLos.DegreesToGoal;

				translation.Value.x = translation.Value.x + speed * Mathf.Sin(rads);
				translation.Value.y = translation.Value.y + speed * Mathf.Cos(rads);

				rotation.Value = quaternion.EulerXYZ(0, 0, -rads);
			}).Run();

		// TODO wall collision tests - only if no line of sight
		/*
		Entities.
			WithAll<AntPathing>().
			WithNone<AntLineOfSight>().
			ForEach((ref Translation translation, in AntTarget target) =>
			{
				translation.Value.x = target.Target.x;
				translation.Value.y = target.Target.y;
			}).ScheduleParallel();
		*/

		// copy target into translation.  we have already done this if we have LineOfSight because we ignore collision tests
		Entities.
			WithAll<AntPathing>().
			WithNone<AntLineOfSight>().
			ForEach((ref Translation translation, in AntTarget target)=>
		{
			translation.Value.x = target.Target.x;
			translation.Value.y = target.Target.y;
		}).Run();
	}
}
