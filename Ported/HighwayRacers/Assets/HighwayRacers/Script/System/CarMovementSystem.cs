using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;
using Random = Unity.Mathematics.Random;

// Any type inheriting from SystemBase will be registered as a system and will start
// updating every frame.
public class CarMovementSystem : SystemBase
{
// todo Burst compiler complains if this is not readonly
    public  static float TrackRadius = 91.0f;
    public readonly static float LaneWidth = 5;
    public readonly static float RoundedCorner = 0.29f;
    private uint Frame = 0;
    
        
    private TrackOccupancySystem m_TrackOccupancySystem;
    Random Random;
    

    protected override void OnCreate()
    {
        m_TrackOccupancySystem = World.GetExistingSystem<TrackOccupancySystem>();
        Random = new Random(1234);

    
    }

    protected override void OnUpdate()
    {
        Frame++;
        float deltaTime = Time.DeltaTime;
        float laneWidth = LaneWidth;
        uint tilesPerLane = TrackOccupancySystem.TilesPerLane;
        uint laneCount = m_TrackOccupancySystem.LaneCount;
        uint theFrame = Frame;
        var random = Random;
        
      
        
        Entities
            .ForEach((ref Translation translation, ref Rotation rotation, ref CarMovement movement) =>
            {
// todo DOTS? access array directly, because we don't know how to do this in DOTS
// IBufferElementData was recommended, but we couldn't get it to work (see older
// version of TrackOccupancySystem)

                // Limit cars from switching lanes to frequently
                movement.LaneSwitchCounter -= deltaTime;

                // Get occupancy of nearby tiles
                int myTile = TrackOccupancySystem.GetMyTile(movement.Offset);
                int nextTile = (int) ((myTile+1) % tilesPerLane);
                int prevTile = (int) (math.max(myTile-1, 0) % tilesPerLane);
                bool nextIsOccupied = TrackOccupancySystem.ReadOccupancy[movement.Lane, nextTile];

                // If car is an European driver and it is blocking a car behind it, the driver
                // will attempt to switch to a more inner lane.
                // If the driver is American, it will exercise its constitutional rights and stay in its lane.
                bool favorInnerLane = false;
                if (movement.Profile == DriverProfile.European)
                {
                    favorInnerLane = TrackOccupancySystem.ReadOccupancy[movement.Lane, prevTile];
                }

                // Make a random decision to switch lanes when blocked
                bool randomlySwitchLanes = random.NextInt(0, 100) > 33;

                // Decide to switch lanes
                if ((nextIsOccupied || favorInnerLane) && movement.LaneSwitchCounter <= 0 && randomlySwitchLanes)
                {
                    // To avoid having two cars merge into the same lane, we allow
                    // mergers to the right at even frames and merges to the left at odd frames.
                    int sideLane = (int) movement.Lane;
                    bool isEven = theFrame % 2 == 0;
                    if (isEven && !favorInnerLane)
                    {
                        sideLane = sideLane+1 < laneCount ? sideLane+1 : (int)movement.Lane;
                    }
                    else
                    {
                        sideLane = sideLane-1 >= 0 ? sideLane-1 : (int)movement.Lane;
                    }
                    
                    if (sideLane != movement.Lane)
                    {
                        // Require 3 un-occupied slots for switching lanes
                        bool sideIsOccupied = TrackOccupancySystem.ReadOccupancy[sideLane, myTile];
                        bool nextSideIsOccupied = TrackOccupancySystem.ReadOccupancy[sideLane, nextTile];
                        bool prevSideIsOccupied = TrackOccupancySystem.ReadOccupancy[sideLane, prevTile];
                        
                        if (!sideIsOccupied && !nextSideIsOccupied && !prevSideIsOccupied)
                        {
                            movement.Lane = (uint) sideLane;
                            movement.LaneSwitchCounter = random.NextFloat(5, 10);
                        }
                    }
                }

                // All cars move at the minimum required speed on the highway.
                // (We don't want cars to stop if the tile in front is occupied)
                float v = nextIsOccupied ? SpawnerSystem.MinimumVelocity : movement.Velocity;

                // LaneOffset is the physical lane position of the car while Lane
                // is the lane the car wants to be in. Let's make progress to
                // merge towards that lane

                movement.LaneOffset = movement.LaneOffset + ((float)movement.Lane - movement.LaneOffset) * deltaTime * 2.0f;

                // Map car's 'Offset' in lane to XZ coords on track's rounded-rect
                //float laneRadius = (trackRadius + (movement.LaneOffset * laneWidth));
                float3 transXZA;
                float transYaw;
                RoundRect.InterpolateRoundRectRealDist(movement.Offset * 50, out transXZA, out transYaw);
    
                quaternion yawQuat = quaternion.AxisAngle(Vector3.up, transYaw);
                float cx = math.cos(transYaw + 1);
                float sx = Mathf.Sin(transYaw + 1);
                
                // eyeballed...
                float laneAdjust = ((movement.LaneOffset  / laneCount - 1) + 0.625f ) * LaneWidth;
                float3 laneOffset = new float3(cx + sx, 0, cx - sx) * laneAdjust;
                translation.Value = transXZA + laneOffset;
                
                // Move car forward on its track
                movement.Offset += v * deltaTime;
                //movement.Offset = movement.Offset % 1.0f;

                // Rotate based on where it is on the rounded rect
                rotation.Value = quaternion.AxisAngle(Vector3.up, transYaw);

            }).ScheduleParallel();
    }
}
