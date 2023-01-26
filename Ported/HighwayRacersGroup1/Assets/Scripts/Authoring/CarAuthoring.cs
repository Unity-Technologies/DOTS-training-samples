using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public class CarAuthoring : MonoBehaviour
{
    class CarBaker : Baker<CarAuthoring>
    {
        public override void Bake(CarAuthoring authoring)
        {
            AddComponent<CarData>();
            AddComponent<URPMaterialPropertyBaseColor>();
        }
    }
}

// todo: rename to "Car"
struct CarData : IComponentData
{
    public int SegmentID;
    public float SegmentDistance;
    public int Lane;
    public int TargetLane;
    public float Speed;
    public float DefaultSpeed;
    public float OvertakeSpeed;
    public float LerpDistance;
    public int inFrontCarIndex;
    public int leftNearestCarIndex;
    public int rightNearestCarIndex;
    
    public float DistanceToCarInFront;
    public float DistanceToCarLeft;
    public float DistanceToCarRight;
    
    public float CarInFrontSpeed;
    public float SafeDistanceInFront;
    public float SafeDistanceOnSide;
    
    public float4 Color;
    public Entity CarPrefab;
}
