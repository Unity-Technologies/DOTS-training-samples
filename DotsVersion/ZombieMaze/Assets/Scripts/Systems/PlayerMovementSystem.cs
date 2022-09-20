using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

// Contrarily to ISystem, SystemBase systems are classes.
// They are not Burst compiled, and can use managed code.
partial struct PlayerMovementSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        
    }

    public void OnDestroy(ref SystemState state)
    {
        
    }

    public void OnUpdate(ref SystemState state)
    {
        var dt = Time.deltaTime;


        PlayerData playerMovement;
        PlayerInput playerInput;



        playerMovement.position = new float2(0, 0);
        playerMovement.direction = new float2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));
        playerMovement.speed = 15;
        //var playerInput = GetSingleton<PlayerData>();

        foreach (var transform in SystemAPI.Query<TransformAspect>().WithAll<PlayerData>())
        {
            Debug.Log(Input.mouseScrollDelta.x);
            transform.Position += new float3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) *playerMovement.speed*dt;
            playerMovement.position = new float2(transform.Position.x, transform.Position.z);
            //transform.position += transform.direction * transform.speed * dt;
        }
    }
}

