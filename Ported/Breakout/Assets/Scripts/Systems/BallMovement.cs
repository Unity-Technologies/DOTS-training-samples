using Aspects;
using Authoring;
using Unity.Entities;
using Unity.Mathematics;

public partial struct BallMovement : ISystem
{
	public void OnCreate(ref SystemState state)
	{
		state.RequireForUpdate<Config>();
	}

	public void OnDestroy(ref SystemState state)
	{
		
	}

	public void OnUpdate(ref SystemState state)
	{
		var config = SystemAPI.GetSingleton<Config>();
		foreach (var ball in SystemAPI.Query<BallAspect>())
		{
			var speedLength = math.length(ball.Speed);
			if (speedLength > 0f)
			{
				// Update ball position based on speed value
				ball.Position += ball.Speed * SystemAPI.Time.DeltaTime;
				// Decay ball speed
				if (speedLength <= 0.25f)
				{
					ball.Speed = float3.zero;
				}
				else
				{
					var length = speedLength - (config.BallSpeedDecay * SystemAPI.Time.DeltaTime);
					var normalized = math.normalize(ball.Speed);
					ball.Speed = normalized * length;
				}
			}
		}
	}
}
