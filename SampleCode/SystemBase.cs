using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public class System : ComponentSystem
{
	protected override void OnUpdate()
	{
		Entities.ForEach((ref ComponentBase comp, ref Rotation rotation) =>
		{
			var deltaTime = Time.deltaTime;
			comp.Value += deltaTime;
		});
	}
}