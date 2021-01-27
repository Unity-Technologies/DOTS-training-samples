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
		RequireSingletonForUpdate<Tuning>();
		_random = new Unity.Mathematics.Random(1234);
	}

	protected override void OnUpdate()
	{
		var time = Time.DeltaTime;

		Tuning tuning = this.GetSingleton<Tuning>();
		float speed = tuning.Speed * Time.DeltaTime;
		float angleRange = tuning.AntAngleRange;
		Unity.Mathematics.Random random = new Unity.Mathematics.Random(_random.NextUInt());

		// no line of sight, pick a direction
		Entities.
			WithAll<AntPathing>().
			WithNone<AntLineOfSight>().
			ForEach((ref AntHeading heading, ref AntTarget target, ref Rotation rotation, in Translation translation) =>
		{
			float headingOffset = random.NextFloat() * (angleRange);

			// calculate all targets left, right, fwd with weighting
			float degreesLeft = heading.Degrees - headingOffset;
			float radsLeft = Mathf.Deg2Rad * degreesLeft;
			float2 targetLeft = new float2(translation.Value.x + speed * Mathf.Sin(radsLeft), translation.Value.y + speed * Mathf.Cos(radsLeft));
			float weightLeft = 1f;

			float degreesRight = heading.Degrees + headingOffset;
			float radsRight = Mathf.Deg2Rad * degreesRight;
			float2 targetRight = new float2(translation.Value.x + speed * Mathf.Sin(radsRight), translation.Value.y + speed * Mathf.Cos(radsRight));
			float weightRight = 1f;

			float degreesFwd = heading.Degrees;
			float radsFwd = Mathf.Deg2Rad * degreesFwd;
			float2 targetFwd = new float2(translation.Value.x + speed * Mathf.Sin(radsFwd), translation.Value.y + speed * Mathf.Cos(radsFwd));
			float weightFwd = 1f * tuning.AntFwdWeighting;

			float totalWeight = weightLeft + weightRight + weightFwd;
			float randomWeight = random.NextFloat() * totalWeight;

			// select random weighted target
			float rads = 0;
			if (randomWeight < weightLeft)
			{
				target.Target = targetLeft;
				heading.Degrees = degreesLeft;
				rads = radsLeft;
			} 
			else if (randomWeight < weightLeft + weightRight)
			{
				target.Target = targetRight;
				heading.Degrees = degreesRight;
				rads = radsRight;
			}
			else
			{
				target.Target = targetFwd;
				heading.Degrees = degreesFwd;
				rads = radsFwd;
			}

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
		}).ScheduleParallel();
	}
}
