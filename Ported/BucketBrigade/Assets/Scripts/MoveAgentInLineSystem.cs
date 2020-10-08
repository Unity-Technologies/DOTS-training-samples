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
				var headPosition = EntityManager.GetComponentData<Translation>(currentAgentEntity).Value;
				var tailAgentEntity = team.LineEmptyTail;
				var tailPosition = EntityManager.GetComponentData<Translation>(tailAgentEntity).Value;
				
				Debug.DrawLine(headPosition, tailPosition);
				
				int i = 0;
				while (currentAgentEntity != Entity.Null)
				{
					var currentAgent = EntityManager.GetComponentData<Agent>(currentAgentEntity);

					// If the agent is idle send him back in the line at his position
					if (currentAgent.ActionState == (byte)AgentAction.IDLE)
					{
						var newDestination = math.lerp(headPosition, tailPosition, i / (float)team.Length);
						
						EntityManager.SetComponentData(currentAgentEntity, new SeekPosition { TargetPos = newDestination, Velocity = 1f});
					}
					
					currentAgentEntity = currentAgent.NextAgent;
					i++;
				}
				
				// Full Line
				currentAgentEntity = team.LineFullHead;
				headPosition = EntityManager.GetComponentData<Translation>(currentAgentEntity).Value;
				tailAgentEntity = team.LineFullTail;
				tailPosition = EntityManager.GetComponentData<Translation>(tailAgentEntity).Value;
				
				i = 0;
				while (currentAgentEntity != Entity.Null)
				{
					var currentAgent = EntityManager.GetComponentData<Agent>(currentAgentEntity);

					if (currentAgent.ActionState == (byte)AgentAction.IDLE)
					{
						var newDestination = math.lerp(headPosition, tailPosition, i / (float)team.Length);
						
						EntityManager.SetComponentData(currentAgentEntity, new SeekPosition { TargetPos = newDestination, Velocity = 1f});
					}
					
					currentAgentEntity = currentAgent.NextAgent;
					i++;
				}

			}).Run();
	}
}