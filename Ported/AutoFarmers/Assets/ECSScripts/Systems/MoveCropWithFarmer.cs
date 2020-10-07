using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class MoveCropWithFarmer : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithName("moving_crops")
            .ForEach((ref Translation translation, ref CropCarried cropCarried) =>
            {
                Position farmerPosition = GetComponent<Position>(cropCarried.FarmerOwner);
                translation.Value = new float3(farmerPosition.Value.x, translation.Value.y, farmerPosition.Value.y);
            }).ScheduleParallel();
    }
}
