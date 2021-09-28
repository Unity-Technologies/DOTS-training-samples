using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;

[UpdateBefore(typeof(BeeMovementSystem))]
public partial class TargetAcquisitionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var random = new Random(1234);

        EntityQuery foodQuery = GetEntityQuery(typeof(Food));
        NativeArray<ArchetypeChunk> foodChunks = foodQuery.CreateArchetypeChunkArray(Allocator.Temp);
        
        EntityQuery teamRedQuery = GetEntityQuery(typeof(TeamRed));
        NativeArray<ArchetypeChunk> teamRedChunks = teamRedQuery.CreateArchetypeChunkArray(Allocator.Temp);
        
        EntityQuery teamBlueQuery = GetEntityQuery(typeof(TeamBlue));
        NativeArray<ArchetypeChunk> teamBlueChunks = teamBlueQuery.CreateArchetypeChunkArray(Allocator.Temp);        
        
        EntityTypeHandle entityHandles = GetEntityTypeHandle();

        Entities
            .WithReadOnly(foodChunks)
            .WithReadOnly(teamBlueChunks)
            .WithReadOnly(entityHandles)
            .WithAll<TeamRed>()
            .ForEach((Entity beeEntity, ref Target target) =>
            {
                if (target.Value == Entity.Null)
                {
                    if (random.NextBool()) // Food
                    {
                        ArchetypeChunk randomChunk = foodChunks[random.NextInt(0, foodChunks.Length)];
                        NativeArray<Entity> entities = randomChunk.GetNativeArray(entityHandles);
                        target.Value = entities[random.NextInt(0, entities.Length)];
                    }
                    else // Blue Bee
                    {
                        ArchetypeChunk randomChunk = teamBlueChunks[random.NextInt(0, teamBlueChunks.Length)];
                        NativeArray<Entity> entities = randomChunk.GetNativeArray(entityHandles);
                        target.Value = entities[random.NextInt(0, entities.Length)];               
                    }
                }
            }).ScheduleParallel();
        
        Entities
            .WithReadOnly(foodChunks)
            .WithReadOnly(teamRedChunks)
            .WithReadOnly(entityHandles)
            .WithAll<TeamBlue>()
            .ForEach((Entity beeEntity, ref Target target) =>
            {
                if (target.Value == Entity.Null)
                {
                    if (random.NextBool()) // Food
                    {
                        ArchetypeChunk randomChunk = foodChunks[random.NextInt(0, foodChunks.Length)];
                        NativeArray<Entity> entities = randomChunk.GetNativeArray(entityHandles);
                        target.Value = entities[random.NextInt(0, entities.Length)];
                    }
                    else // Red Bee
                    {
                        ArchetypeChunk randomChunk = teamRedChunks[random.NextInt(0, teamRedChunks.Length)];
                        NativeArray<Entity> entities = randomChunk.GetNativeArray(entityHandles);
                        target.Value = entities[random.NextInt(0, entities.Length)];               
                    }
                }
            }).ScheduleParallel();

        /*
        var random = new Random(1234);
        
        EntityQuery query = GetEntityQuery(typeof(Food));
        var entityChunks = query.CreateArchetypeChunkArray(Allocator.Temp);
        var randomChunk = entityChunks [random.NextInt(0, entityChunks.Length)];

        EntityTypeHandle entityHandles = GetEntityTypeHandle();
        var entityContainerFinally = randomChunk.GetNativeArray(entityHandles);
        targets.Food = entityContainerFinally[random.NextInt(0, entityContainerFinally.Length)];
        
        EntityQuery teamRedQuery = GetEntityQuery(typeof(TeamRed));
        var teamRedChunks = teamRedQuery.CreateArchetypeChunkArray(Allocator.Temp);
        var randomTeamRedChunk = teamRedChunks [random.NextInt(0, teamRedChunks.Length)];
        var teamRedEntityContainerFinally = randomTeamRedChunk.GetNativeArray(entityHandles);
        targets.TeamBlue = teamRedEntityContainerFinally[random.NextInt(0, teamRedEntityContainerFinally.Length)];
        
        EntityQuery teamBlueQuery = GetEntityQuery(typeof(TeamBlue));
        var teamBlueChunks = teamBlueQuery.CreateArchetypeChunkArray(Allocator.Temp);
        var randomTeamBlueChunk = teamBlueChunks [random.NextInt(0, teamBlueChunks.Length)];
        var teamBlueEntityContainerFinally = randomTeamBlueChunk.GetNativeArray(entityHandles);
        targets.TeamRed = teamBlueEntityContainerFinally[random.NextInt(0, teamBlueEntityContainerFinally.Length)];
        
        SetSingleton<TargetCandidates>(targets);
        */
    }
}