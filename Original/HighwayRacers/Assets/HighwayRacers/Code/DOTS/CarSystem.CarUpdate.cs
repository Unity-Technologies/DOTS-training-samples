using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

partial class CarSystem
{
    [BurstCompile]
    // TOOD: Proper grouping of the properties based on access type: Read-write or Read-only.
    private struct CarUpdateJob : IJobForEachWithEntity<CarBasicState, CarCrossVelocity, CarLogicState, CarReadOnlyProperties>
    {
        const float MIN_DIST_BETWEEN_CARS = .7f;

        public float Dt;
        public float HighwayLen;
        public float Acceleration;
        public float BrakeDeceleration;

        [ReadOnly] public CarQueryStructure QueryStructure;

        public void Execute(Entity entity, int index, ref CarBasicState carBasicState, ref CarCrossVelocity carCrossVelocity, ref CarLogicState carLogicState,
            [ReadOnly] ref CarReadOnlyProperties carReadOnlyProperties)
        {
            var targetSpeed = carBasicState.Speed;
            var hasCarInFront = QueryStructure.GetCarInFront(index, carBasicState, HighwayLen, out var carInFront, out var distToCarInFront);

            switch (carLogicState.State)
            {
                case VehicleState.NORMAL:
                    targetSpeed = carReadOnlyProperties.DefaultSpeed;
                    carCrossVelocity.CrossLaneVel = 0;

                    // if won't merge, match car in front's speed
                    if (distToCarInFront < carReadOnlyProperties.MergeDistance)
                        targetSpeed = math.min(targetSpeed, carInFront.Speed);

                    break;
                case VehicleState.MERGE_LEFT:
                case VehicleState.MERGE_RIGHT:
                case VehicleState.OVERTAKING:
                    break;
            }

            // increase to speed
            if (targetSpeed > carBasicState.Speed)
                carBasicState.Speed = math.min(targetSpeed, carBasicState.Speed + Acceleration * Dt);
            else
                carBasicState.Speed = math.max(targetSpeed, carBasicState.Speed - BrakeDeceleration * Dt);

            // crash prevention failsafe
            if (hasCarInFront && Dt > 0)
            {
                float maxDistanceDiff = math.max(0, distToCarInFront - MIN_DIST_BETWEEN_CARS);
                carBasicState.Speed = math.min(carBasicState.Speed, maxDistanceDiff / Dt);
            }

            carBasicState.Position += carBasicState.Speed * Dt;
            var newLane = carBasicState.Lane + carCrossVelocity.CrossLaneVel * Dt;
            carBasicState.Position = Utilities.ConvertPositionToLane(carBasicState.Position, carBasicState.Lane, newLane, HighwayLen);
        }
    }
}
