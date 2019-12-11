
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class AntMovementSystem : JobComponentSystem
{
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		float timeDelta = Time.DeltaTime;
		var job = Entities.ForEach((ref Translation translation, ref AntComponent ant) =>
		{
			float velocityChange = ant.acceleration * timeDelta;
			
			math.sincos(ant.facingAngle, out float sin, out float cos);
			ant.speed += velocityChange;

			float speedChange = ant.speed * timeDelta;
			float3 velocity = new float3(speedChange * cos, speedChange * sin, 0.0f);
			translation.Value += velocity;

		}).Schedule(inputDeps);

		return job;
	}
}
