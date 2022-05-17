

using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
partial struct BucketVisualUpdateSystem : ISystem {
    //This system will handle reading the fill level and using that to
    //calculate the bucket's size and color

    public void OnCreate(ref SystemState state) {
        
    }

    public void OnDestroy(ref SystemState state) {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        float scale = 1;
        foreach (var bucket in SystemAPI.Query<BucketAspect>()) {
            bucket.Scale = 0.5f + (bucket.FillLevel * 2);
            bucket.Color = Color.Lerp(Color.cyan, Color.blue, bucket.FillLevel);
            //Adjust this to use math.lerp, using the float4 representations of the colors.
        }
    }
}