using HighwayRacers.Script.Components;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.Rendering.VirtualTexturing;
using NotImplementedException = System.NotImplementedException;

[UpdateAfter(typeof(CarMovementSystem))]
public class MergeStateSystem : SystemBase
{
    public readonly uint TilesPerLane = 64;
    public readonly uint LaneCount = 4;
    
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        var lanes = GetEntityQuery(typeof(LaneOccupancy)).ToEntityArray(Allocator.TempJob);
        var buffers = new NativeArray<DynamicBuffer<LaneOccupancy>>((int)LaneCount, Allocator.Temp);
        
        
        for (int i = 0; i < lanes.Length; i++)
        {
            var buffer = EntityManager.GetBuffer<LaneOccupancy>(lanes[i]);
            buffers[i] = buffer;
        }

        uint tilesPerLane = TilesPerLane;
        var laneCount = LaneCount;
        
        var normalJob = Entities
            .ForEach((Entity vehicle, ref CarMovement movement, in NormalState state) =>
            {
                float trackPos = movement.Offset;
                int myTile = (int)(trackPos * tilesPerLane);
                var laneBuffer = buffers[(int)movement.Lane];

                // TODO : set to normal speed
                // movement.Velocity = normalSpeed
                
                
                bool shouldMergeLeft = false;

                // 3 tiles ahead?
                for (int i = myTile + 1; i < myTile + 4; i++)
                {
                    shouldMergeLeft = laneBuffer[i].Occupied;
                    if (shouldMergeLeft)
                    {
                        // TODO : Set speed to car in front
                        // movement.Velocity
                        
                        ecb.RemoveComponent<NormalState>(vehicle);
                        ecb.AddComponent<MergeLeftState>(vehicle);
                        return;
                    }
                }
            })
            .ScheduleParallel(Dependency);

        var mergeLeftJob = Entities
            .ForEach((Entity vehicle, ref CarMovement movement, in MergeLeftState state) =>
            {
                float trackPos = movement.Offset;
                int myTile = (int)(trackPos * tilesPerLane);
                var laneBuffer = buffers[(int)movement.Lane];

                bool canMergeLeft = false;
                if (movement.Lane + 1 < laneCount)
                {
                    // check in left lane
                    var leftLaneBuffer = buffers[(int)movement.Lane + 1];
                    canMergeLeft = !leftLaneBuffer[myTile].Occupied;
                    if (canMergeLeft)
                    {
                        ecb.RemoveComponent<MergeLeftState>(vehicle);
                        ecb.AddComponent<OvertakeState>(vehicle);
                        
                        // change to left lane
                        var current = laneBuffer[myTile]; 
                        current.Occupied = false;
                        laneBuffer[myTile] = current;
                        
                        var left = leftLaneBuffer[myTile];
                        left.Occupied = true;
                        leftLaneBuffer[myTile] = left;
                    }
                }
            })
            .ScheduleParallel(normalJob);

        var overtakeJob = Entities
            .ForEach((Entity vehicle, ref CarMovement movement, in OvertakeState state) =>
            {
                float trackPos = movement.Offset;
                int myTile = (int)(trackPos * tilesPerLane);
                var laneBuffer = buffers[(int)movement.Lane];

                // check if can merge right
                bool canMergeRight = false;
                if ((int)movement.Lane - 1 >= 0)
                {
                    // check in right lane
                    var rightLaneBuffer = buffers[(int)movement.Lane - 1];
                    canMergeRight = !rightLaneBuffer[myTile].Occupied;
                    if (canMergeRight)
                    {
                        ecb.RemoveComponent<OvertakeState>(vehicle);
                        ecb.AddComponent<MergeRightState>(vehicle);
                        
                        // change to right lane
                        var current = laneBuffer[myTile]; 
                        current.Occupied = false;
                        laneBuffer[myTile] = current;
                        
                        var right = rightLaneBuffer[myTile];
                        right.Occupied = true;
                        rightLaneBuffer[myTile] = right;
                    }
                }
                
                // if not wait or merge left again?
                
                
                
            })
            .ScheduleParallel(mergeLeftJob);
        
        var mergeRightJob = Entities
            .ForEach((Entity vehicle, ref CarMovement movement, in MergeRightState state) =>
            {
                // float trackPos = movement.Offset;
                // int myTile = (int)(trackPos * tilesPerLane);
                // var laneBuffer = buffers[(int)movement.Lane];

                ecb.RemoveComponent<MergeRightState>(vehicle);
                ecb.AddComponent<NormalState>(vehicle);
            })
            // .WithDisposeOnCompletion(lanes)
            // .WithDisposeOnCompletion(buffers)
            .ScheduleParallel(overtakeJob);
    }
}
