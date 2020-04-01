
using Unity.Entities;
using Unity.Mathematics;

public struct ArmRenderComponentData: IComponentData
{
    public Entity armEntity;
    public int jointIndex;
}

public struct ArmIdleTargetComponentData: IComponentData
{
    public float3 value;
    
    public static implicit operator ArmIdleTargetComponentData(float3 up) => new ArmIdleTargetComponentData
    {
        value = up
    };
    
    public static implicit operator float3(ArmIdleTargetComponentData c) => c.value;
}


public struct ArmIKTargetComponentData: IComponentData
{
    public float3 value;
    
    public static implicit operator ArmIKTargetComponentData(float3 up) => new ArmIKTargetComponentData
    {
        value = up
    };
    
    public static implicit operator float3(ArmIKTargetComponentData c) => c.value;
}

public struct ArmUpComponentData: IComponentData
{
    public float3 value;
    
    public static implicit operator ArmUpComponentData(float3 up) => new ArmUpComponentData
    {
        value = up
    };
    
    public static implicit operator float3(ArmUpComponentData c) => c.value;
}

public struct ArmRightComponentData: IComponentData
{
    public float3 value;
    
    public static implicit operator ArmRightComponentData(float3 up) => new ArmRightComponentData
    {
        value = up
    };
    
    public static implicit operator float3(ArmRightComponentData c) => c.value;
}

public struct ArmForwardComponentData: IComponentData
{
    public float3 value;
    
    public static implicit operator ArmForwardComponentData(float3 up) => new ArmForwardComponentData
    {
        value = up
    };
    
    public static implicit operator float3(ArmForwardComponentData c) => c.value;
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

public struct AnchorPosComponentData: IComponentData
{
    public float3 value;
    
    public static implicit operator AnchorPosComponentData(float3 up) => new AnchorPosComponentData
    {
        value = up
    };
    
    public static implicit operator float3(AnchorPosComponentData c) => c.value;
}

public struct IdleArmSeedComponentData: IComponentData
{
    public int value;
    
    public static implicit operator IdleArmSeedComponentData(int up) => new IdleArmSeedComponentData
    {
        value = up
    };
    
    public static implicit operator int(IdleArmSeedComponentData c) => c.value;
}