using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

// Contrarily to ISystem, SystemBase systems are classes.
// They are not Burst compiled, and can use managed code.
partial struct PlayerMovementSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MazeConfig>();
        state.RequireForUpdate<TileBufferElement>();
        

    }

    public void OnDestroy(ref SystemState state)
    {
        
    }

    public void OnUpdate(ref SystemState state)
    {

        MazeConfig mazeConfig = SystemAPI.GetSingleton<MazeConfig>();
        DynamicBuffer<TileBufferElement> tiles = SystemAPI.GetSingletonBuffer<TileBufferElement>();
        float dt = state.Time.DeltaTime;
        float3 startIndex;
        foreach (var character in SystemAPI.Query<CharacterAspect>())
        {
            float3 tempPos = new float3(
                Input.GetAxis("Horizontal"),
                0,
                Input.GetAxis("Vertical")) * character.speed * dt;

            character.position += tempPos;
        }
       
        
    }
}

