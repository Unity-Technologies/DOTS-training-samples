using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct MovingTag : IComponentData { }

public struct FallingTag : IComponentData { }

public struct ArmComponent : IComponentData
{
    public float3 HandTargetPosition;
    public float3 HandForward;
    public float3 HandUp;
    public float3 HandRight;
    public Matrix4x4 HandMatrix;
}

public struct InverseKinematicsDataTag : IComponentData
{
}

public struct GrabbedTag : IComponentData
{
}

public struct Diameter : IComponentData
{
    public float Value;
}

public struct Velocity : IComponentData
{
    public float3 Value;
}

public struct AngularVelocity : IComponentData
{
    public float3 Value;
}

public struct BoneMatrixBuffer : IBufferElementData
{
    public Matrix4x4 Value;

    public static implicit operator BoneMatrixBuffer(Matrix4x4 matrix) => 
        new BoneMatrixBuffer {Value = matrix};

    public static implicit operator Matrix4x4(BoneMatrixBuffer matrixBuffer) => matrixBuffer.Value; 
}

public struct ArmJointPositionBuffer : IBufferElementData
{
    public float3 Value;
	
    public static implicit operator ArmJointPositionBuffer(float3 matrix) =>
        new ArmJointPositionBuffer {Value = matrix};

    public static implicit operator float3(ArmJointPositionBuffer buffer) => buffer.Value; 
}

public struct FingerJointPositionBuffer : IBufferElementData
{
    public float3 Value;
	
    public static implicit operator FingerJointPositionBuffer(float3 matrix) =>
        new FingerJointPositionBuffer {Value = matrix};

    public static implicit operator float3(FingerJointPositionBuffer buffer) => buffer.Value; 
}

public struct ThumbJointPositionBuffer : IBufferElementData
{
    public float3 Value;
	
    public static implicit operator ThumbJointPositionBuffer(float3 matrix) =>
        new ThumbJointPositionBuffer {Value = matrix};

    public static implicit operator float3(ThumbJointPositionBuffer buffer) => buffer.Value; 
}

public struct TinCanRendering : ISharedComponentData, IEquatable<TinCanRendering>
{
    public Material Material;
    public Mesh Mesh;
    public float Height;
    public float Diameter;

    public bool Equals(TinCanRendering other)
    {
        return Equals(Material, other.Material)
               && Equals(Mesh, other.Mesh)
               && Height.Equals(other.Height) 
               && Diameter.Equals(other.Diameter);
    }

    public override bool Equals(object obj)
    {
        return obj is TinCanRendering other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Material != null ? Material.GetHashCode() : 0;
            hashCode = (hashCode * 397) ^ (Mesh != null ? Mesh.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ Height.GetHashCode();
            hashCode = (hashCode * 397) ^ Diameter.GetHashCode();
            return hashCode;
        }
    }
}