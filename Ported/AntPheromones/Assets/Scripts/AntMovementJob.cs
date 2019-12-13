
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public struct AntMovementJob : IJobForEach<Translation, Rotation, AntComponent, AntSteeringComponent>
{
	[ReadOnly] public float TimeDelta;
	[ReadOnly] public AntSettings Settings;
	[ReadOnly] public float2 ColonyPosition;
	[ReadOnly] public float2 ResourcePosition;
	[ReadOnly] public float TargetRadius;

	public void Execute(ref Translation translation, ref Rotation rotation, ref AntComponent ant, [ReadOnly] ref AntSteeringComponent steering)
	{
		float targetSpeed = Settings.antSpeed;
		float2 position = translation.Value.xy;
		float angularVelocity = 0.0f;
		
		// Update the steering...
		angularVelocity += steering.PheromoneSteering * Settings.pheromoneSteerStrength;
		angularVelocity += steering.WallSteering * Settings.wallSteerStrength;
		angularVelocity += steering.TargetSteering;

		targetSpeed *= 1f - (math.abs(steering.PheromoneSteering) + math.abs(steering.WallSteering)) / 3f;

		// Update the rotation/position...
		ant.facingAngle += steering.RandomDirection + angularVelocity;	//* TimeDelta;
		math.sincos(ant.facingAngle, out float sin, out float cos);
		ant.speed += (targetSpeed - ant.speed) * Settings.antAccel;

		float2 velocity = (ant.velocity + ant.speed * new float2(cos, sin)) * TimeDelta;

		// Clamp to the edge of the map...
		if (position.x + velocity.x < 0f || position.x + velocity.x >= 1)
		{
			velocity.x = -velocity.x;
		}
		else
		{
			position.x += velocity.x;
		}
		if (position.y + velocity.y < 0f || position.y + velocity.y >= 1)
		{
			velocity.y = -velocity.y;
		}
		else
		{
			position.y += velocity.y;
		}
		// Output the modified values...
		ant.facingAngle = math.atan2(velocity.y, velocity.x);
		translation.Value.xy = position;
		rotation.Value = quaternion.Euler(0.0f, 0.0f, ant.facingAngle);

		var targetPos = ant.state == 0 ? ResourcePosition : ColonyPosition;

		if (math.lengthsq(translation.Value.xy - targetPos) < TargetRadius * TargetRadius)
		{
			ant.state = 1 - ant.state;
			ant.stateSwitch = true;
			ant.facingAngle = math.fmod(ant.facingAngle + math.PI, math.PI * 2);
		}
	}
}
