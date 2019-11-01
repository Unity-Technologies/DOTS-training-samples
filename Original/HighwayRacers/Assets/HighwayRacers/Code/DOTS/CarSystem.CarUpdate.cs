using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

partial class CarSystem
{
    [BurstCompile]
    private struct CarUpdateJob : IJobForEachWithEntity<CarBasicState, CarLogicState, CarReadOnlyProperties>
    {
        const float MIN_DIST_BETWEEN_CARS = .7f;

        public float Dt;

        public float Acceleration;
        public float BrakeDeceleration;
        public float LaneSwitchSpeed;
        public float MaxOvertakeTime;

        [ReadOnly] public CarQueryStructure QueryStructure;

        public void Execute(Entity entity, int index, ref CarBasicState carBasicState, ref CarLogicState carLogicState,
            [ReadOnly] ref CarReadOnlyProperties carReadOnlyProperties)
        {
            var highwayLen = QueryStructure.HighwayLen;
            var hasCarInFront = QueryStructure.GetCarInFront(index, carBasicState, out var carInFront, out var carInFrontIndex, out var distToCarInFront);
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

            if (carLogicState.OvertakingCarIndex >= 0)
            {
                carLogicState.OvertakeRemainingTime -= Dt;
                // detect overtaking car taking too long
                if (carLogicState.OvertakeRemainingTime <= 0)
                {
                    carLogicState.OvertakingCarIndex = -1;
                    carLogicState.OvertakeRemainingTime = 0.0f;
                }
            }

            // detect merging
            if (!carLogicState.State.IsMerging())
            {
                // detect merging to left lane
                if (carBasicState.Lane + 1 < 4 // left lane exists
                    && distToCarInFront < carReadOnlyProperties.MergeDistance // close enough to car in front
                    && carReadOnlyProperties.OvertakeEagerness > carInFront.Speed / carReadOnlyProperties.DefaultSpeed) // car in front is slow enough
                {
                    if (QueryStructure.CanMergeToLane(carBasicState, carBasicState.Lane + 1))
                    {
                        // if space is available
                        // start merge to left
                        carLogicState.State = VehicleState.MERGE_LEFT;
                        carLogicState.TargetLane = math.round(carBasicState.Lane + 1);
                        carLogicState.OvertakingCarIndex = carInFrontIndex;
                        carLogicState.OvertakeRemainingTime = MaxOvertakeTime;
                    }
                }

                // detect merging to right lane
                bool tryMergeRight = false;
                if (carLogicState.OvertakingCarIndex < 0)
                {
                    // if overtake car got destroyed
                    tryMergeRight = true;
                }
                else
                {
                    // if passed overtake car
                    var overtakeCarState = QueryStructure.GetCarState(carLogicState.OvertakingCarIndex);
                    if (Utilities.DistanceTo(carBasicState.Position, carBasicState.Lane, overtakeCarState.Position, overtakeCarState.Lane, highwayLen)
                        > Utilities.LaneLength(carBasicState.Lane, highwayLen) / 2)
                    {
                        tryMergeRight = true;
                    }
                }

                if (carBasicState.Lane - 1 < 0)
                {
                    // right lane must exist
                    tryMergeRight = false;
                }

                if (tryMergeRight)
                {
                    // don't merge if just going to merge back
                    var hasRightCarInFront = QueryStructure.GetCarInFront(
                        Utilities.ConvertPositionToLane(carBasicState.Position, carBasicState.Lane, carBasicState.Lane - 1, highwayLen),
                        carBasicState.Lane - 1, out var rightCarInFront, out var rightCarInFrontIndex, out var distToRightCarInFront);
                    // condition for merging to left lane
                    if (distToRightCarInFront < carReadOnlyProperties.MergeDistance // close enough to car in front
                        && carReadOnlyProperties.OvertakeEagerness > rightCarInFront.Speed / carReadOnlyProperties.DefaultSpeed) // car in front is slow enough
                    {
                        tryMergeRight = false;
                    }
                }

                if (!carLogicState.State.IsMerging() // not currently merging
                    && tryMergeRight) // overtook target car (or overtake car doesn't exist)
                {
                    if (QueryStructure.CanMergeToLane(carBasicState, carBasicState.Lane - 1))
                    {
                        // if space is available
                        // start merge to right
                        carLogicState.OvertakingCarIndex = -1;
                        carLogicState.State = VehicleState.MERGE_RIGHT;
                        carLogicState.TargetLane = math.round(carBasicState.Lane - 1);
                    }
                }
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

            carBasicState.Position = Utilities.ConvertPositionToLane(carBasicState.Position, carBasicState.Lane, newLane, highwayLen);
            carBasicState.Lane = newLane;
        }
    }
}
