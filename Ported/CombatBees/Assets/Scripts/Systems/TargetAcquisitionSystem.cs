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
        NativeArray<ArchetypeChunk> foodChunks = foodQuery.CreateArchetypeChunkArray(Allocator.TempJob);
        
        EntityQuery teamRedQuery = GetEntityQuery(typeof(TeamRed));
        NativeArray<ArchetypeChunk> teamRedChunks = teamRedQuery.CreateArchetypeChunkArray(Allocator.TempJob);
        
        EntityQuery teamBlueQuery = GetEntityQuery(typeof(TeamBlue));
        NativeArray<ArchetypeChunk> teamBlueChunks = teamBlueQuery.CreateArchetypeChunkArray(Allocator.TempJob);        
        
        EntityTypeHandle entityHandles = GetEntityTypeHandle();

        Entities
            .WithReadOnly(foodChunks)
            .WithReadOnly(teamBlueChunks)
            .WithReadOnly(entityHandles)
            .WithDisposeOnCompletion(teamBlueChunks)
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
            .WithDisposeOnCompletion(foodChunks)
            .WithDisposeOnCompletion(teamRedChunks)
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
        
        // alternate way to dispose: foodChunks.Dispose(Dependency);
    }
}