using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[BurstCompile]
partial struct PickAttractionJob : IJobEntity {
    [ReadOnly] public NativeArray<Entity> allies;

    void Execute([EntityInQueryIndex] int entityIndex, ref AttractionComponent attractionComponent) {
        var random = Random.CreateFromIndex((uint) entityIndex);
        attractionComponent.attracted = allies[random.NextInt(allies.Length)];
        attractionComponent.repelled = allies[random.NextInt(allies.Length)];
    }
}

[BurstCompile]
partial struct AttractionJob : IJobEntity {
    
    public float teamAttraction;
    public float teamRepulsion;
    public float deltaTime;
    
    [ReadOnly] public ComponentLookup<Position> positionLookup;

    void Execute(in Position position, ref Velocity velocity, in AttractionComponent attractionComponent) {
        var attractiveFriend = attractionComponent.attracted;
        if (positionLookup.TryGetComponent(attractiveFriend, out var attractiveFriendPosition)) {
            float3 delta = attractiveFriendPosition.Value - position.Value;
            float dist = Mathf.Sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
            if (dist > 0f) {
                velocity.Value += delta * (teamAttraction * deltaTime / dist);
            }
        }

        var repellentFriend = attractionComponent.repelled;
        if (positionLookup.TryGetComponent(repellentFriend, out var repellentFriendPosition)) {
            float3 delta = repellentFriendPosition.Value - position.Value;
            float dist = Mathf.Sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
            if (dist > 0f) {
                velocity.Value -= delta * (teamRepulsion * deltaTime / dist);
            }
        }
    }
}