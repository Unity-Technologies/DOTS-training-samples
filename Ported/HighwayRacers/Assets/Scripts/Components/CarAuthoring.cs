using System;
using Unity.Entities;
using UnityEngine;

public class CarAuthoring : MonoBehaviour
{
    class CarBaker : Baker<CarAuthoring>
    {
        public override void Bake(CarAuthoring authoring)
        {
            AddComponent<CarIsOvertaking>(new CarIsOvertaking {IsOvertaking = false});
            AddComponent<CarVelocity>(new CarVelocity {VelX = 0, VelY = 0});
            AddComponent<CarDefaultValues>(new CarDefaultValues {DefaultVelY = 0});
            AddComponent<CarOvertakeState>(new CarOvertakeState {OvertakeStartTime = 0.0f, OriginalLane = 0, ChangingLane = false});
            AddComponent<CarPositionInLane>(new CarPositionInLane{Position = 0, LanePosition = 0, LaneIndex = 0});
            AddComponent<CarCollision>(new CarCollision {CollisionFlags = CollisionType.None});
            AddComponent<CarIndex>(new CarIndex {Index = 0});
        }
    }
}

struct CarIsOvertaking : IComponentData
{
    public bool IsOvertaking;

}

struct CarOvertakeState : IComponentData
{
    public float OvertakeStartTime;
    public int OriginalLane;
    public bool ChangingLane;
}

public partial struct CarPositionInLane : IComponentData
{
    public float Position;
    public float LanePosition;
    public int LaneIndex;
}

struct CarVelocity : IComponentData
{
    public float VelX, VelY;
}

struct CarDefaultValues : IComponentData
{
    public float DefaultVelY;
}


[Flags]
public enum CollisionType : byte
{
    None = 0,
    Front = 1 << 0,
    Left = 1 << 1,
    Right = 1 << 2,
    
}

struct CarCollision : IComponentData
{
    public CollisionType CollisionFlags;
    public float FrontVelocity;
    public float FrontDistance;
}

struct CarIndex : IComponentData
{
    public int Index;
}