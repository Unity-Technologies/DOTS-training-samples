using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;

[UpdateBefore(typeof(BeeMovementSystem))]
public partial class TargetAcquisitionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        TargetCandidates targets = GetSingleton<TargetCandidates>();

        var random = new Random(1234); 
        EntityQuery query = GetEntityQuery(typeof(Food));
        var entityChunks = query.CreateArchetypeChunkArray(Allocator.Temp);
        var randomChunk = entityChunks [random.NextInt(0, entityChunks.Length)];

        EntityTypeHandle entityHandles = GetEntityTypeHandle();
        var entityContainerFinally = randomChunk.GetNativeArray(entityHandles);
        targets.Food = entityContainerFinally[random.NextInt(0, entityContainerFinally.Length)];


        EntityQuery teamQuery = GetEntityQuery(typeof(Team));
        var teamChunks = teamQuery.CreateArchetypeChunkArray(Allocator.Temp);
        var randomTeamChunk = teamChunks [random.NextInt(0, teamChunks.Length)];
        var teamEntityContainerFinally = randomTeamChunk.GetNativeArray(entityHandles);
        targets.TeamBlue = teamEntityContainerFinally[random.NextInt(0, teamEntityContainerFinally.Length)];
        targets.TeamRed = teamEntityContainerFinally[random.NextInt(0, teamEntityContainerFinally.Length)];
        SetSingleton<TargetCandidates>(targets);
    }
}