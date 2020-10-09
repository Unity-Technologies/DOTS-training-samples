using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateBefore(typeof(FarmerMoveSystem))]
public class GameTimeSystem : SystemBase
{
    SpeedDisplayUI slider;
    
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<GameTime>();

        slider = GameObject.FindObjectOfType<SpeedDisplayUI>();
    }

    protected override void OnUpdate()
    {
        float speed = slider.slider.value;
        
        Entity gameStateEntity = GetSingletonEntity<GameState>();
        GameState gameState = GetSingleton<GameState>();
        GameTime gameTime = GetSingleton<GameTime>();
        
        gameState.SimulationSpeed = speed;

        float deltaTime = Time.DeltaTime * gameState.SimulationSpeed;

        EntityManager.SetComponentData(gameStateEntity, new GameTime
        {
            DeltaTime = deltaTime,
            ElapsedTime = gameTime.ElapsedTime + deltaTime
        });
    }

}
