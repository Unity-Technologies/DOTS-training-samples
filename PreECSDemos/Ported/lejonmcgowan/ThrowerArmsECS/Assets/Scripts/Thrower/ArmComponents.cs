﻿
using Unity.Entities;
using Unity.Mathematics;


public struct ArmRenderComponentData: IComponentData
{
    public Entity armEntity;
    public int jointIndex;
}

public struct ArmLastRockRecord: IComponentData
{
    public float size;
    public float3 pos;
}

public struct ArmIdleTarget: IComponentData
{
    public float3 value;
    
    public static implicit operator ArmIdleTarget(float3 up) => new ArmIdleTarget
    {
        value = up
    };
    
    public static implicit operator float3(ArmIdleTarget c) => c.value;
}

public struct ArmReservedRock : IComponentData
{
    public Entity value;
    
    public static implicit operator ArmReservedRock(Entity up) => new ArmReservedRock
    {
        value = up
    };
    
    public static implicit operator Entity(ArmReservedRock c) => c.value;
}

public struct ArmGrabbedTag : IComponentData
{
}

public struct ArmGrabTimer : IComponentData
{
    public float value;

    public static implicit operator ArmGrabTimer(float t) => new ArmGrabTimer()
    {
        value = t
    };

    public static implicit operator float(ArmGrabTimer c) => c.value;
}


public struct ArmIKTarget: IComponentData
{
    public float3 value;
    
    public static implicit operator ArmIKTarget(float3 up) => new ArmIKTarget
    {
        value = up
    };
    
    public static implicit operator float3(ArmIKTarget c) => c.value;
}

public struct ArmGrabTarget: IComponentData
{
    public float3 value;
    
    public static implicit operator ArmGrabTarget(float3 target) => new ArmGrabTarget
    {
        value = target
    };
    
    public static implicit operator float3(ArmGrabTarget c) => c.value;
}

public struct ArmBasesUp: IComponentData
{
    public float3 value;
    
    public static implicit operator ArmBasesUp(float3 up) => new ArmBasesUp
    {
        value = up
    };
    
    public static implicit operator float3(ArmBasesUp c) => c.value;
}

public struct ArmBasesRight: IComponentData
{
    public float3 value;
    
    public static implicit operator ArmBasesRight(float3 up) => new ArmBasesRight
    {
        value = up
    };
    
    public static implicit operator float3(ArmBasesRight c) => c.value;
}

public struct ArmBasesForward: IComponentData
{
    public float3 value;
    
    public static implicit operator ArmBasesForward(float3 up) => new ArmBasesForward
    {
        value = up
    };
    
    public static implicit operator float3(ArmBasesForward c) => c.value;
}

public struct ArmJointElementData: IBufferElementData
{
    public float3 value;
    
    public static implicit operator ArmJointElementData(float3 up) => new ArmJointElementData
    {
        value = up
    };
    
    public static implicit operator float3(ArmJointElementData c) => c.value;
    
}

public struct ArmAnchorPos: IComponentData
{
    public float3 value;
    
    public static implicit operator ArmAnchorPos(float3 up) => new ArmAnchorPos
    {
        value = up
    };
    
    public static implicit operator float3(ArmAnchorPos c) => c.value;
}

public struct IdleArmSeed: IComponentData
{
    public int value;
    
    public static implicit operator IdleArmSeed(int up) => new IdleArmSeed
    {
        value = up
    };
    
    public static implicit operator int(IdleArmSeed c) => c.value;
}

public struct Wrist : IComponentData
{
    public Entity value;
    
    public static implicit operator Wrist(Entity e) => new Wrist
    {
        value = e
    };
    
    public static implicit operator Entity(Wrist c) => c.value;
}