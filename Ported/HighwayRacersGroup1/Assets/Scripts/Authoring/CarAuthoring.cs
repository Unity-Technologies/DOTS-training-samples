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
        }
    }
}

// todo: rename to "Car"
struct CarData : IComponentData
{
    public int SegmentID;
    public float SegmentDistance;
    public int Lane;
    public float Speed;
    public float DefaultSpeed;
    public float OvertakeSpeed;
    public float LerpDistance;

    public float DistanceToCarInFront;
    public float CarInFrontSpeed;
}
