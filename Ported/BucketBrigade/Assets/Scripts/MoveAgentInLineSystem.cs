using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class MoveAgentInLineSystem : SystemBase
{
	protected override void OnUpdate()
	{
		Entities 
			.WithoutBurst()
			.ForEach((Entity e, int entityInQueryIndex, ref Team team) =>
			{
				// Empty Line
				var currentAgentEntity = team.LineEmptyHead;
				
				int i = 0;
				while (currentAgentEntity != Entity.Null)
				{
					var currentAgent = EntityManager.GetComponentData<Agent>(currentAgentEntity);

					// If the agent is idle send him back in the line at his position
					if (currentAgent.ActionState == (byte)AgentAction.IDLE)
					{
						var newDestination = math.lerp(team.PickupLocation, team.DropOffLocation, i / (float)team.Length) - GetCurvePosition(team.PickupLocation, team.DropOffLocation, i, team.Length);;
						
						EntityManager.SetComponentData(currentAgentEntity, new SeekPosition { TargetPos = newDestination, Velocity = 1f});
					}
					
					currentAgentEntity = currentAgent.NextAgent;
					i++;
				}
				
				// Full Line
				currentAgentEntity = team.LineFullHead;
				
				i = 0;
				while (currentAgentEntity != Entity.Null)
				{
					var currentAgent = EntityManager.GetComponentData<Agent>(currentAgentEntity);

					if (currentAgent.ActionState == (byte)AgentAction.IDLE)
					{
						var newDestination = math.lerp(team.PickupLocation, team.DropOffLocation, i / (float)team.Length) + GetCurvePosition(team.PickupLocation, team.DropOffLocation, i, team.Length);
						EntityManager.SetComponentData(currentAgentEntity, new SeekPosition { TargetPos = newDestination, Velocity = 1f});
					}
					
					currentAgentEntity = currentAgent.NextAgent;
					i++;
				}

			}).Run();
	}
	
	static float3 GetCurvePosition(float3 lineStart, float3 lineEnd, int teamIndex, int teamCount)
	{
		// get perpendicular
		float2 direction = math.normalize(new float2(lineEnd.x - lineStart.x, lineEnd.z - lineStart.z) );
		float3 perpendicular = new float3(direction.y, 0, -direction.x);

		float curveOffset = math.sin((float)teamIndex / (float) teamCount * math.PI);

		return perpendicular * curveOffset;
	}
}