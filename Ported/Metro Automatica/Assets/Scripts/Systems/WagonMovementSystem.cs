using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;

[BurstCompile]
partial struct WagonMovementSystem : ISystem
{
    private float startTime;
    public float speed;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        startTime = Time.time;
        speed = 0.01f;
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        //var entity = state.EntityManager.CreateEntity();


        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        foreach (var (wagon, transform, ent) in SystemAPI.Query<RefRW<Wagon>, RefRW<LocalTransform>>().WithEntityAccess())
        {
            //transform.LocalTransform = Vector3.Lerp(startMarker.position, endMarker.position, fractionOfJourney);
            //wagon.ValueRW.currentDestination
            //state.EntityManager.GetComponentData<LocalTransform>(wagon.Item2);
            //float3 finalDestination = SystemAPI.GetComponent<LocalTransform>(wagon.ValueRO.currentDestination).Position;
            float3 finalDestination = wagon.ValueRO.currentDestination;
            //Debug.Log(finalDestination.x);
            //SystemAPI.GetComponent<LocalTransform>(wagon.ValueRO.currentDestination);
            // Distance moved equals elapsed time times speed..
            float distCovered = (Time.time - startTime) * speed;

            // Fraction of journey completed equals current distance divided by total distance.
            float journeyLength = Vector3.Distance(transform.ValueRO.Position, finalDestination);
            float fractionOfJourney = distCovered / journeyLength;

            // Set our position as a fraction of the distance between the markers.
            //transform.ValueRW. = Vector3.Lerp(startMarker.position, endMarker.position, fractionOfJourney); 
            transform.ValueRW.Position = Vector3.Lerp(transform.ValueRO.Position, finalDestination, fractionOfJourney);
            //float3 startDestination
            //SystemAPI.GetComponent<LocalTransform>(wagon.ValueRO.currentDestination);
        }

        /*var railSpawn = ecb.Instantiate(config.RailPrefab);
        
        // need var distance /2 between first and last station in x value (for now)

        var transform = LocalTransform.FromPosition(45, 0, 0);
        
        // need var distance to calculate scale value

        ecb.SetComponent(railSpawn, new PostTransformScale{Value = float3x3.Scale(90,1,1)});

        ecb.SetComponent(railSpawn, transform); 
        
        // This system should only run once at startup. So it disables itself after one update.*/
        //state.Enabled = false;
    }
}
