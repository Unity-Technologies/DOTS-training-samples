using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[BurstCompile]
partial struct SpawnerSystem : ISystem
{
    Config config;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // The default world should not be used because the targeted EntityManager
        // may not be part of it.
        // var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        config = SystemAPI.GetSingleton<Config>();

        //Reset all entities
        if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.H))
        {
            //destroy all entities and re-enable respawn
            config.Respawn = true;

            EntityQueryDescBuilder entityQueryBuilder = new EntityQueryDescBuilder(Allocator.Temp);
            entityQueryBuilder.AddAny(ComponentType.ReadWrite<Bee>());
            entityQueryBuilder.AddAny(ComponentType.ReadWrite<Blood>());
            entityQueryBuilder.AddAny(ComponentType.ReadWrite<FoodResource>());
            entityQueryBuilder.FinalizeQuery();

            state.EntityManager.DestroyEntity(state.GetEntityQuery(entityQueryBuilder));
        }

        if (!config.Respawn)
            return;

        //Instantiate our two bee teams...
        state.EntityManager.Instantiate(config.BlueBeePrefab, config.TeamBlueBeeCount, Allocator.Temp);
        foreach (var(transform, bee) in SystemAPI.Query<TransformAspect, RefRW<Bee>>().WithAny<BlueBee>())
        {
            transform.Position = new float3(-45, 10, 0);
            bee.ValueRW.SpawnPoint = transform.Position;
            bee.ValueRW.beeTeam = Team.BLUE;
        }

        state.EntityManager.Instantiate(config.YellowBeePrefab, config.TeamYellowBeeCount, Allocator.Temp);
        foreach (var (transform, bee) in SystemAPI.Query<TransformAspect, RefRW<Bee>>().WithAny<YellowBee>())
        {
            transform.Position = new float3(45, 10, 0);
            bee.ValueRW.SpawnPoint = transform.Position;
            bee.ValueRW.beeTeam = Team.YELLOW;
        }

        state.EntityManager.Instantiate(config.FoodResourcePrefab, config.FoodResourceCount, Allocator.Temp);
        // food resource field is 20 by 20, 10 in each direction
        // random number generator for both dimension from -10 to 10
        Random rand = new Random(123);
        foreach (var (foodResource, transform) in SystemAPI.Query< RefRW < FoodResource > , TransformAspect >().WithAll<FoodResource>())
        {
            foodResource.ValueRW.State = FoodState.SETTLED;

            var position = new float3(rand.NextInt(-10, 11), 0, rand.NextInt(-10, 11));
            transform.Position = position;
        }

        config.Respawn = false;
        SystemAPI.SetSingleton<Config>(config);
    }
}