
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public struct AntMovementJob : IJobForEach<Translation, Rotation, AntComponent>
{
	public float TimeDelta;

	public void Execute(ref Translation translation, ref Rotation rotation, ref AntComponent ant)
	{
		float velocityChange = ant.acceleration * TimeDelta;

		math.sincos(ant.facingAngle, out float sin, out float cos);
		ant.speed += velocityChange;

		float speedChange = ant.speed * TimeDelta;
		float3 velocity = new float3(speedChange * cos, speedChange * sin, 0.0f);
		translation.Value += velocity;
		
		rotation.Value = quaternion.Euler(0.0f, 0.0f, ant.facingAngle);
	}
}
