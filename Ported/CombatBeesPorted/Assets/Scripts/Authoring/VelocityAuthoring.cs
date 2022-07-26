﻿using Unity.Entities;

class VelocityAuthoring : UnityEngine.MonoBehaviour
{
    public float Value;
}

class VelocityBaker : Baker<VelocityAuthoring>
{
    public override void Bake(VelocityAuthoring authoring)
    {
        AddComponent(new Velocity
        {
            Value = authoring.Value
        });
    }
    
}