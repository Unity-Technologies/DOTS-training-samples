using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace MetroECS.Trains
{
    public class TrainMovement : SystemBase
    {
        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;

            Entities.ForEach((ref Translation translation, ref Rotation rotation, in Carriage carriage) =>
            {
                // var train = GetComponent<Train>(carriage.Train);
                // var path = GetComponent<PathData>(train.Path);
            }).Schedule();
        }
    }
}