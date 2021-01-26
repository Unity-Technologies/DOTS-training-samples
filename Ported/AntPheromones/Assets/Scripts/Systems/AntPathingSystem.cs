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

		Entities.
			WithAll<AntPathing>().
			ForEach((ref AntHeading heading, ref Translation translation, ref Rotation rotation) =>
		{
			float headingOffset = -(angleRange / 2) + (random.NextFloat() * (angleRange));

			heading.Degrees += headingOffset;
			float rads = Mathf.Deg2Rad * heading.Degrees;

			translation.Value.x += speed * Mathf.Sin(rads);
			translation.Value.y += speed * Mathf.Cos(rads);
			
			rotation.Value = quaternion.EulerXYZ(0, 0, -rads);

		}).ScheduleParallel();
	}
}
