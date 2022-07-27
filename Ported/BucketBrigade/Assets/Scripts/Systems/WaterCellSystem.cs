using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial class WaterCellSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var dt = Time.DeltaTime;
        // TODO : use a Query instead
        Entities
            .WithAll<Volume, Position>()
            .WithNone<BucketId>().ForEach((ref NonUniformScale scale, ref Volume volume) =>
        {
            volume.Value = math.min(volume.Value + dt * 10, 100); // TODO : use a filling value
            scale.Value.x = volume.Value / 100.0f;
            scale.Value.z = volume.Value / 100.0f;
        }).ScheduleParallel();
    }
}
