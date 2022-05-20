using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
partial struct BucketMovementSystem : ISystem {
    //This system will handle reading the fill level and using that to
    //calculate the bucket's size and color

    private float4 cyan;
    private float4 blue;

    public void OnCreate(ref SystemState state) {
        
    }

    public void OnDestroy(ref SystemState state) {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        foreach (var bucket in SystemAPI.Query<BucketAspect>()) {
            bucket.Scale = 0.5f + (bucket.FillLevel * 2);
            bucket.Color = math.lerp(cyan, blue, bucket.FillLevel);
        }
    }
}