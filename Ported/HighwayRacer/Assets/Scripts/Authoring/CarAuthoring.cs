using Unity.Entities;
using Unity.Mathematics;

public class CarAuthoring : UnityEngine.MonoBehaviour
{
    public float Distance;
    public float Length;
    public float Speed;
    public float DesiredSpeed;
    public float Acceleration;
    public float TrackLength;
    public int LaneNumber;
    public float LaneChangeClearance;
    public float4 Color;
    public int SegmentNumber;
    public bool IsPassing;

    class CarBaker : Baker<CarAuthoring>
    {
        public override void Bake(CarAuthoring authoring)
        {
            AddComponent(new Car
            {
                Distance = authoring.Distance,
                Length = authoring.Length,
                Speed = authoring.Speed,
                DesiredSpeed = authoring.DesiredSpeed,
                Acceleration = authoring.Acceleration,
                TrackLength = authoring.TrackLength,
                LaneNumber = authoring.LaneNumber,
                NewLaneNumber = -1,
                LaneChangeProgress = -1.0f,
                LaneChangeClearance = authoring.LaneChangeClearance,
                Color = authoring.Color,
                SegmentNumber = authoring.SegmentNumber,
                IsPassing = authoring.IsPassing
            });

        }
    }
}

public struct Car : IComponentData
{
    public float Distance;
    public float Length;
    public float Speed;
    public float DesiredSpeed;
    public float Acceleration;
    public float TrackLength;
    public int LaneNumber;
    public int NewLaneNumber;
    public float LaneChangeProgress;
    public float LaneChangeClearance;
    public float4 Color;
    public int SegmentNumber;
    public int Index;
    public bool IsPassing;
    public float4x4 StartTransformation;
    public float4x4 EndTransformation;
}
