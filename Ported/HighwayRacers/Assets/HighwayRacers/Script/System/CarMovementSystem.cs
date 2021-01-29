using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;
using Random = Unity.Mathematics.Random;

public class CarMovementSystem : SystemBase
{
    public const float straightPieceLength = (HighwayRacers.Highway.LANE0_LENGTH - HighwayRacers.Highway.CURVE_LANE0_RADIUS * 4) / 4;
    public const float derrivedTrackRadius = straightPieceLength + HighwayRacers.Highway.MID_RADIUS * 2.0f;

// todo Burst compiler complains if this is not readonly
    public readonly static float TrackRadius = derrivedTrackRadius - HighwayRacers.Highway.LANE_SPACING * 3.0f;
    public readonly static float LaneWidth = HighwayRacers.Highway.LANE_SPACING * 2.0f;
    public float3 TrackOrigin = new float3(0,0,0);
    public readonly static float RoundedCorner = (1.0f - straightPieceLength/(straightPieceLength + HighwayRacers.Highway.MID_RADIUS * 2.0f)) * 0.5f;
    private uint _Frame = 0;

    private TrackOccupancySystem m_TrackOccupancySystem;
    Random Random;

    public uint Frame => _Frame;
    
    protected override void OnCreate()
    {
        m_TrackOccupancySystem = World.GetExistingSystem<TrackOccupancySystem>();
        Random = new Random(1234);

    
    }

    protected override void OnUpdate()
    {
        _Frame++;

        float deltaTime = Time.DeltaTime;
        float trackRadius = TrackRadius;
        float3 trackOrigin = TrackOrigin;
        float laneWidth = LaneWidth;
        uint tilesPerLane = TrackOccupancySystem.TilesPerLane;
        uint laneCount = m_TrackOccupancySystem.LaneCount;
        uint theFrame = _Frame;
        var random = Random;
        
        var readOccupancy = m_TrackOccupancySystem.GetReadBuffer(_Frame);

        float4 lowVelocityColor = SpawnerSystem.LowVelocityColor;
        float4 americanColors = SpawnerSystem.AmericanColors;
        float4 europeanColors = SpawnerSystem.EuropeanColors;

        Entities
            .WithoutBurst()
            .WithNativeDisableContainerSafetyRestriction(readOccupancy)
            .ForEach((ref Translation translation, ref Rotation rotation, ref CarMovement movement, ref URPMaterialPropertyBaseColor baseColor) =>
            {
                // Limit cars from switching lanes too frequently
                movement.LaneSwitchCounter -= deltaTime;

                // Get occupancy of nearby tiles
                int myTile = TrackOccupancySystem.GetMyTile(movement.Offset);
                int nextTile = (int) ((myTile+1) % tilesPerLane);
                int prevTile = (int) (math.max(myTile-1, 0) % tilesPerLane);
                bool nextIsOccupied = TrackOccupancySystem.IsTileOccupied(ref readOccupancy, (int)movement.Lane, nextTile);

                // If car is an European driver and it is blocking a car behind it, the driver
                // will attempt to switch to a more inner lane.
                // If the driver is American, it will exercise its constitutional rights and stay in its lane.
                bool favorInnerLane = false;
                if (movement.Profile == DriverProfile.European)
                {
                    favorInnerLane = TrackOccupancySystem.IsTileOccupied(ref readOccupancy, (int)movement.Lane, prevTile);
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
                        bool sideIsOccupied = TrackOccupancySystem.IsTileOccupied(ref readOccupancy, sideLane, myTile);
                        bool nextSideIsOccupied = TrackOccupancySystem.IsTileOccupied(ref readOccupancy, sideLane, nextTile);
                        bool prevSideIsOccupied = TrackOccupancySystem.IsTileOccupied(ref readOccupancy, sideLane, prevTile);
                        
                        //bool finishedChangingLane = math.abs(movement.LaneOffset - movement.Lane) < 0.05;
                        if (!sideIsOccupied && !nextSideIsOccupied && !prevSideIsOccupied)
                        {
                            movement.Lane = (uint) sideLane;
                            movement.LaneSwitchCounter = random.NextFloat(5, 10);
                        }
                    }
                }

                // All cars move at the minimum required speed on the highway.
                // (We don't want cars to stop if the tile in front is occupied)
                // We lerp velocity changes over time so it looks like the car accelerates.
                movement.CurrentVelocity = nextIsOccupied ? SpawnerSystem.MinimumVelocity : math.lerp(movement.CurrentVelocity, movement.Velocity, deltaTime*2);

                // Turn cars red the longer they cannot reach their desired speed
                // We lerp this over time so we don't flicker.
                if (movement.CurrentVelocity < movement.Velocity - 0.001f)
                    baseColor.Value = math.lerp(baseColor.Value, lowVelocityColor, deltaTime);
                else
                    baseColor.Value = baseColor.Value = math.lerp(baseColor.Value, movement.Profile == DriverProfile.American ? americanColors : europeanColors, deltaTime*2);

                // LaneOffset is the physical lane position of the car while Lane
                // is the lane the car wants to be in. Let's make progress to
                // merge towards that lane

                movement.LaneOffset = movement.LaneOffset + ((float)movement.Lane - movement.LaneOffset) * deltaTime * 5.0f;

                // Map car's 'Offset' in lane to XZ coords on track's rounded-rect
				
                float3 transXZA;
                float transYaw;
                RoundRect.InterpolateRoundRectRealDist(movement.Offset * 50, out transXZA, out transYaw);
    
                quaternion yawQuat = quaternion.AxisAngle(Vector3.up, transYaw);
                float cx = math.cos(transYaw + 1);
                float sx = Mathf.Sin(transYaw + 1);
                
                // eyeballed...
                float laneAdjust = ((movement.LaneOffset  / laneCount - 1) + 0.625f ) * laneWidth * 1.5f;
                float3 laneOffset = new float3(cx + sx, 0, cx - sx) * laneAdjust;
                translation.Value = transXZA + laneOffset;
                
                // Move car forward on its track
                movement.Offset += movement.CurrentVelocity * deltaTime;
                movement.Offset = movement.Offset % 1.0f;

                // Rotate based on where it is on the rounded rect
                rotation.Value = quaternion.AxisAngle(Vector3.up, transYaw);
                
            })
            .ScheduleParallel();
    }
}
