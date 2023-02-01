using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
// IJobEntity relies on source generation to implicitly define a query from the signature of the Execute function.
partial struct CarTranslationJob : IJobEntity
{
    public float InverseLaneLength;
    public float LaneLength;
    
    [BurstCompile]
    void Execute( ref TransformAspect transform, in CarPositionInLane positionInLane)
    {
        float3 positionBefore = transform.LocalPosition;
        float lanePosition = positionInLane.LanePosition;
        float position = positionInLane.Position * InverseLaneLength * math.PI * 2;

        //Add radius setting here
        float radius = LaneLength / (math.PI * 2) - lanePosition;
        
        float x = math.sin(position) * radius;
        float y = math.cos(position) * radius;
        float3 targetPosition = new float3(x, 0.0f, y);
        transform.LocalPosition = targetPosition;
        transform.LookAt(targetPosition + math.cross(transform.Up, targetPosition));

    }
}

[BurstCompile]
[UpdateAfter(typeof(CarDrivingSystem))]
partial struct CarTranslationSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var globalSettings = SystemAPI.GetSingleton<GlobalSettings>();
        
        var carTranslationJob = new CarTranslationJob
        {
            InverseLaneLength = 1 / globalSettings.LengthLanes,
            LaneLength = globalSettings.LengthLanes
        };
        carTranslationJob.ScheduleParallel();
    }
}