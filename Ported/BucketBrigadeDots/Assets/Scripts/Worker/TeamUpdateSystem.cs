using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[UpdateAfter(typeof(WaterAndFireLocatorSystem))]
public partial struct TeamUpdateSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var nextPositions = SystemAPI.GetComponentLookup<NextPosition>();
        
        foreach (var (teamData, teamState, teamMembers) in SystemAPI.Query<
                     RefRO<TeamData>,
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
    
    void RepositionTeam(DynamicBuffer<TeamMember> teamMembers, ComponentLookup<NextPosition> nextPositions, RefRO<TeamData> teamData)
    {
        var waterPosition = teamData.ValueRO.WaterPosition;
        var firePosition = teamData.ValueRO.FirePosition;
        var direction = math.normalize(firePosition - waterPosition);
        var perpendicular = new float2(direction.y, -direction.x);
        
        var halfTeamSize = teamMembers.Length / 2;
        var quarterTeamSize = halfTeamSize / 2;
        for (var i = 0; i < teamMembers.Length; ++i)
        {
            var halfTeamId = i % halfTeamSize;
            var position = math.lerp(waterPosition, firePosition, halfTeamId / (float)halfTeamSize);
            
            var perpendicularOffset = math.lerp(1f, 0f, math.abs(halfTeamId - quarterTeamSize) / (float)quarterTeamSize);
            var isFirstHalf = i < halfTeamSize;
            if (isFirstHalf)
                position += perpendicular * perpendicularOffset;
            else
                position -= perpendicular * perpendicularOffset;
            
            var workerEntity = teamMembers[i].Value;
            nextPositions[workerEntity] = new NextPosition()
            {
                Value = position
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