using Unity.Entities;
using UnityEngine;

[AlwaysUpdateSystem, UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
public partial class GameSequenceSystem : SystemBase
{
    
    
    protected override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartGame();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            StopGame();
        }
    }

    private void StartGame()
    {
        if (!HasSingleton<GameRunning>())
            EntityManager.CreateEntity(ComponentType.ReadWrite<GameRunning>());
    }

    private void StopGame()
    {
        if (HasSingleton<GameRunning>())
            EntityManager.DestroyEntity(GetSingletonEntity<GameRunning>());
    }
}