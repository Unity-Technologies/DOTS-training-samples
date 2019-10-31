using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

partial class CarSystem
{
    [BurstCompile]
    // TOOD: Proper grouping of the properties based on access type: Read-write or Read-only.
    private struct CarUpdateJob : IJobForEachWithEntity<CarBasicState, CarLogicState, CarReadOnlyProperties>
    {
        const float MIN_DIST_BETWEEN_CARS = .7f;

        public float Dt;

        public float HighwayLen;
        public float Acceleration;
        public float BrakeDeceleration;
        public float LaneSwitchSpeed;

        [ReadOnly] public CarQueryStructure QueryStructure;

        public void Execute(Entity entity, int index, ref CarBasicState carBasicState, ref CarLogicState carLogicState,
            [ReadOnly] ref CarReadOnlyProperties carReadOnlyProperties)
        {
            var hasCarInFront = QueryStructure.GetCarInFront(index, carBasicState, HighwayLen, out var carInFront, out var carInFrontIndex, out var distToCarInFront);
            var targetSpeed = carReadOnlyProperties.DefaultSpeed;
            float crossLaneSpeed = 0.0f;

            switch (carLogicState.State)
            {
                case VehicleState.NORMAL:
                    // if won't merge, match car in front's speed
                    if (distToCarInFront < carReadOnlyProperties.MergeDistance)
                        targetSpeed = math.min(targetSpeed, carInFront.Speed);
                    break;

                case VehicleState.MERGE_LEFT:
                    crossLaneSpeed = LaneSwitchSpeed;
                    // detect ending merge
                    if (carBasicState.Lane + crossLaneSpeed * Dt >= carLogicState.TargetLane)
                    {
                        // set veloicty to not overshoot lane
                        crossLaneSpeed = (carLogicState.TargetLane - carBasicState.Lane) / Dt;
                        if (carBasicState.Lane >= carLogicState.TargetLane)
                        { // end when frame started in the target lane
                            carLogicState.State = VehicleState.OVERTAKING;
                        }
                    }
                    break;

                case VehicleState.OVERTAKING:
                    targetSpeed = carReadOnlyProperties.MaxSpeed;
                    break;

                case VehicleState.MERGE_RIGHT:
                    crossLaneSpeed = -LaneSwitchSpeed;
                    // detect ending merge
                    if (carBasicState.Lane + crossLaneSpeed * Dt <= carLogicState.TargetLane)
                    {
                        // set veloicty to not overshoot lane
                        crossLaneSpeed = (carLogicState.TargetLane - carBasicState.Lane) / Dt;
                        if (carBasicState.Lane <= carLogicState.TargetLane)
                        { // end when frame started in the target lane
                            carLogicState.State = VehicleState.NORMAL;
                        }
                    }
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

            var newLane = carBasicState.Lane + crossLaneSpeed * Dt;
            float roundLane = math.round(newLane);
            if (math.abs(roundLane - newLane) < .0001f)
                newLane = roundLane;

            carBasicState.Position = Utilities.ConvertPositionToLane(carBasicState.Position, carBasicState.Lane, newLane, HighwayLen);
        }
    }
}
