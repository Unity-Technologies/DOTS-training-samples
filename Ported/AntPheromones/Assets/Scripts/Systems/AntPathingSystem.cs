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
			float headingOffset = -(angleRange / 2) + (random.NextFloat() * (angleRange));

			heading.Degrees += headingOffset;
			float rads = Mathf.Deg2Rad * heading.Degrees;

			target.Target.x = translation.Value.x + speed * Mathf.Sin(rads);
			target.Target.y = translation.Value.y + speed * Mathf.Cos(rads);

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
