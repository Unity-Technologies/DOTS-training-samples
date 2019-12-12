
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
	[ReadOnly] public float2 ColonyPosition;
	[ReadOnly] public float2 ResourcePosition;
	[ReadOnly] public float TargetRadius;

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
		float3 velocity = speedChange * new float3(cos, sin, 0.0f);
		position += velocity;

		// Clamp to the edge of the map...
		if((position.x < 0) || (position.x >= 1.0f))
		{
			velocity.x = -velocity.x;
		} 
		if((position.y < 0.0f) || (position.y >= 1.0f))
		{
			velocity.y = -velocity.y;
		}
		position = math.clamp(position, 0, 1);

		// Output the modified values...
		ant.facingAngle = math.atan2(velocity.y, velocity.x);
		translation.Value = position;
		rotation.Value = quaternion.Euler(0.0f, 0.0f, ant.facingAngle);

		var targetPos = ant.state == 0 ? ResourcePosition : ColonyPosition;

		if (math.lengthsq(translation.Value.xy - targetPos) < TargetRadius * TargetRadius)
		{
			ant.state = 1 - ant.state;
			ant.facingAngle = math.fmod(ant.facingAngle + math.PI, math.PI * 2);
		}
	}
}
