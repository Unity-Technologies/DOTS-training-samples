using Unity.Burst;
using Unity.Entities;

public partial struct TeamUpdateSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var nextPositions = SystemAPI.GetComponentLookup<NextPosition>();
        
        foreach (var (teamData, teamState, teamMembers) in SystemAPI.Query<
                     RefRO<Team>,
                     RefRW<TeamState>,
                     DynamicBuffer<TeamMember>>())
        {
            switch (teamState.ValueRO.Value)
            {
                case TeamStates.Idle:
                    RepositionTeam(teamMembers, nextPositions, teamData);
                    teamState.ValueRW.Value = TeamStates.Repositioning;
                    break;
                case TeamStates.Repositioning:
                    if (IsReadyToExtinguish(teamMembers, nextPositions))
                        teamState.ValueRW.Value = TeamStates.Extinguishing;
                    break;
                case TeamStates.Extinguishing:
                    UnityEngine.Debug.Log($"Ready to extinguish!");
                    break;
            }
        }
    }
    
    void RepositionTeam(DynamicBuffer<TeamMember> teamMembers, ComponentLookup<NextPosition> nextPositions, RefRO<Team> teamData)
    {
        for (var i = 0; i < teamMembers.Length; ++i)
        {
            var workerEntity = teamMembers[i].Value;
            nextPositions[workerEntity] = new NextPosition()
            {
                Value = teamData.ValueRO.FirePosition
            };
            nextPositions.SetComponentEnabled(workerEntity, true);
        }
    }

    bool IsReadyToExtinguish(DynamicBuffer<TeamMember> teamMembers, ComponentLookup<NextPosition> nextPositions)
    {
        for (var i = 0; i < teamMembers.Length; ++i)
        {
            if (nextPositions.IsComponentEnabled(teamMembers[i].Value))
                return false;
        }
        return true;
    }
}