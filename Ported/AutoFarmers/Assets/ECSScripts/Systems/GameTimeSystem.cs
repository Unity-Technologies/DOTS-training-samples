using Unity.Entities;
using Unity.Mathematics;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateBefore(typeof(FarmerMoveSystem))]
public class GameTimeSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<GameTime>();
    }

    protected override void OnUpdate()
    {
        Entity gameStateEntity = GetSingletonEntity<GameState>();
        GameState gameState = GetSingleton<GameState>();
        GameTime gameTime = GetSingleton<GameTime>();

        float deltaTime = Time.DeltaTime * gameState.SimulationSpeed;

        EntityManager.SetComponentData(gameStateEntity, new GameTime
        {
            DeltaTime = deltaTime,
            ElapsedTime = gameTime.ElapsedTime + deltaTime
        });
    }

}
