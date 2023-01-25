using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

public class CarAuthoring : UnityEngine.MonoBehaviour
{
    public float Distance;
    public float Length;
    public float Speed;
    public float Acceleration;
    public float TrackLength;
    public int LaneNumber;
    public float LaneChangeClearance;
    public int SegmentNumber;

    class CarBaker : Baker<CarAuthoring>
    {
        public override void Bake(CarAuthoring authoring)
        {
            AddComponent(new Car
            {
                Distance = authoring.Distance,
                Length = authoring.Length,
                Speed = authoring.Speed,
                Acceleration = authoring.Acceleration,
                TrackLength = authoring.TrackLength,
                LaneNumber = authoring.LaneNumber,
                LaneChangeClearance = authoring.LaneChangeClearance,
                SegmentNumber = authoring.SegmentNumber
            });
        }
    }
}

struct Car : IComponentData
{
    public float Distance;
    public float Length;
    public float Speed;
    public float Acceleration;
    public float TrackLength;
    public int LaneNumber;
    public float LaneChangeClearance;
    public int SegmentNumber;
}
