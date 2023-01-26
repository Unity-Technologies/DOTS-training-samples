using Unity.Entities;
using Unity.Mathematics;

public class CarAuthoring : UnityEngine.MonoBehaviour
{
    public float Distance;
    public float Length;
    public float Speed;
    public float MaxSpeed;
    public float Acceleration;
    public float TrackLength;
    public int LaneNumber;
    public float LaneChangeClearance;
    public float4 Color;
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
                MaxSpeed = authoring.MaxSpeed,
                Acceleration = authoring.Acceleration,
                TrackLength = authoring.TrackLength,
                LaneNumber = authoring.LaneNumber,
                NewLaneNumber = -1,
                LaneChangeProgress = -1.0f,
                LaneChangeClearance = authoring.LaneChangeClearance,
                Color = authoring.Color,
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
    public float MaxSpeed;
    public float Acceleration;
    public float TrackLength;
    public int LaneNumber;
    public int NewLaneNumber;
    public float LaneChangeProgress;
    public float LaneChangeClearance;
    public float4 Color;
    public int SegmentNumber;
    public int Index;
}
