using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEditor.SceneManagement;
using UnityInput = UnityEngine.Input;
using UnityKeyCode = UnityEngine.KeyCode;
using UnityDebug = UnityEngine.Debug;

public class PlayerInput : SystemBase
{
    protected override void OnUpdate()
    {
        var simSpeedEntity = GetSingletonEntity<SimulationSpeed>();
        
        // Loop through numeric keycodes and assign Simulation Speed upon match
        for( int i = (int) UnityKeyCode.Alpha1 ; i <= (int) UnityKeyCode.Alpha9 ; ++i )
        {
            if(UnityInput.GetKeyDown((UnityKeyCode)i))
            {
                SimulationSpeed newSimSpeed = new SimulationSpeed
                {
                    Value = i - (int)UnityKeyCode.Alpha0
                };
                SetComponent(simSpeedEntity, newSimSpeed);
            }
        }
        
        // Upon 'Reset' input, find all 'Wall' and 'Ant' entities and
        // destroy them using the EntityCommandBuffer
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        if (UnityInput.GetKeyDown(UnityKeyCode.R))
        {
            // Destroy all Walls and Ants
            Entities
                .WithAll<Wall>()
                .ForEach((Entity entity) =>
                {
                    ecb.DestroyEntity(entity);
                }).Run();
            
            Entities
                .WithAll<Ant>()
                .ForEach((Entity entity) =>
                {
                    ecb.DestroyEntity(entity);
                }).Run();

            Entities
                .WithAll<FoodSource>()
                .ForEach((Entity entity) =>
                {
                    ecb.DestroyEntity(entity);
                }).Run();
            
            Entities
                .WithAny<Wall, WallSegment>()
                .ForEach((Entity entity) =>
                {
                    ecb.DestroyEntity(entity);
                }).Run();
            
            // Add Respawn component to AntSpawner
            Entities
                .WithAny<AntSpawner, WallSpawner, PheromoneMap>()
                .ForEach((Entity entity) =>
                {
                    ecb.AddComponent<Respawn>(entity);
                }).Run();

            Entities
                .WithAny<FoodSpawner, PheromoneMap>()
                .ForEach((Entity entity) =>
                {
                    ecb.AddComponent<Respawn>(entity);
                }).Run();
        }
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}