using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Properties;

public class AntPathingSystem : SystemBase
{
	protected override void OnUpdate()
	{
		var time = Time.DeltaTime;

		float speed = this.GetSingleton<Tuning>().Speed;

		Entities.
			WithAll<AntPathing>().
			ForEach((ref AntHeading heading, ref Translation translation, ref Rotation rotation) =>
		{
			translation.Value.y += speed * time;

			heading.Degrees++;
			rotation.Value = quaternion.EulerXYZ(0, 0, Mathf.Deg2Rad * heading.Degrees);

		}).ScheduleParallel();
	}
}
