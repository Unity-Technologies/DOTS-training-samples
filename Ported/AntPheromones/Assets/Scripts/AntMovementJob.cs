
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public struct AntMovementJob : IJobForEach<Translation, Rotation, AntComponent, AntSteeringComponent>
{
	[ReadOnly] public float TimeDelta;
	[ReadOnly] public float AntMaxSpeed;
	[ReadOnly] public float PheromoneSteeringStrength;
	[ReadOnly] public float WallSteeringStrength;

	public void Execute(ref Translation translation, ref Rotation rotation, ref AntComponent ant, [ReadOnly] ref AntSteeringComponent steering)
	{
		float velocityChange = ant.acceleration * TimeDelta;
		float3 position = translation.Value;
		float angularVelocity = 0.0f;
		
		// Update the steering...
		angularVelocity += steering.PheromoneSteering * PheromoneSteeringStrength;
		angularVelocity += steering.WallSteering * WallSteeringStrength;

		// Update the rotation/position...
		ant.facingAngle += steering.RandomDirection + angularVelocity;	//* TimeDelta;
		math.sincos(ant.facingAngle, out float sin, out float cos);
		ant.speed = math.clamp(ant.speed + velocityChange, 0.0f, AntMaxSpeed);

		float speedChange = ant.speed * TimeDelta;
		float3 velocity = new float3(speedChange * cos, speedChange * sin, 0.0f);
		position += velocity;

		// Clamp to the edge of the map...
		if((position.x < 0) || (position.x >= 1.0f))
		{
			velocity.x = -velocity.x;
			position = translation.Value;
		} 
		if((position.y < 0.0f) || (position.y >= 1.0f))
		{
			velocity.y = -velocity.y;
			position = translation.Value;
		}

		// Output the modified values...
		ant.facingAngle = math.atan2(velocity.y, velocity.x);
		translation.Value = position;
		rotation.Value = quaternion.Euler(0.0f, 0.0f, ant.facingAngle);
	}
}
