﻿using Unity.Burst;
using Unity.Entities;

[BurstCompile]
partial struct MovementJob : IJobEntity {

    public float deltaTime;

    void Execute(ref Position position, in Velocity velocity) {
        position.Value += velocity.Value * deltaTime;
    }
}