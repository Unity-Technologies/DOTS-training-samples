using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[BurstCompile]
partial struct StationSpawningSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var stations = CollectionHelper.CreateNativeArray<Entity>(config.StationCount, Allocator.Temp);
        ecb.Instantiate(config.StationPrefab, stations);

        LocalTransform first = new LocalTransform();
        LocalTransform last = new LocalTransform();

        for (int i = 0; i < stations.Length; i++)
        {
            var transform = LocalTransform.FromPosition(i * 90, 0, 0);
          //  transform.Rotation = quaternion.RotateY(math.radians(90));
            ecb.SetComponent(stations[i], transform);
            if (i == 0)
            {
                first = transform;
            }

            if (i == stations.Length -1)
            {
                last = transform;
            }
        }
        
        /*foreach (var transform in SystemAPI.Query<TransformAspect>().WithAll<Turret>())
        {
            transform.RotateWorld(rotation);
        }*/
        
        
        
        // This system should only run once at startup. So it disables itself after one update.
        var railSpawn = ecb.Instantiate(config.RailPrefab);
        
        // need var distance /2 between first and last station in x value (for now)

        var railTransform = LocalTransform.FromPosition(first.Position.x + (last.Position.x - first.Position.x)/2, 0, 0);
        
        // need var distance to calculate scale value

        ecb.SetComponent(railSpawn, new PostTransformScale{Value = float3x3.Scale(last.Position.x - first.Position.x,1,1)});

        ecb.SetComponent(railSpawn, railTransform);
        
        state.Enabled = false;
    }
}
