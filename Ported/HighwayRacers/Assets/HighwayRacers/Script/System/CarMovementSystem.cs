using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

// Any type inheriting from SystemBase will be registered as a system and will start
// updating every frame.
public class CarMovementSystem : SystemBase
{
// todo Burst compiler complains if this is not readonly
    public readonly static float TrackRadius = 91.0f;
    public readonly static float LaneWidth = 3.75f;
    public float3 TrackOrigin = new float3(0,0,0);
    private const float CircleRadians = 2*math.PI;
    public readonly static float RoundedCorner = 0.29f;
    private uint Frame = 0;

    private TrackOccupancySystem m_TrackOccupancySystem;
    Random Random;

    static float3 MapToRoundedCorners(float t, float radius)
    {
        float R = CarMovementSystem.RoundedCorner;
        float straight = 1.0f - 2.0f * R;
        float curved = (2.0f * math.PI * R) * 0.25f;
        float total = straight + curved;
        float tls = math.saturate(straight/total);
        float tlr = math.saturate(curved/total);

        int q = (int)(t * 4.0f);

        float x = 0;
        float y = 0;
        float a = 0;

        if(q == 0)
        {
            float n = t * 4.0f;
            x = R;
            y = math.lerp(R, 1.0f - R, math.saturate(n/tls));

            a = 0.5f * math.PI * math.saturate((n - tls)/tlr);
            x -= math.cos(a) * R;
            y += math.sin(a) * R;
        }
        else if(q == 1)
        {
            float n = (t - 0.25f) * 4.0f;
            y = 1.0f - R;
            x = math.lerp(R, 1.0f - R, math.saturate(n/tls));

            a = 0.5f * math.PI * math.saturate((n - tls)/tlr);
            y += math.cos(a) * R;
            x += math.sin(a) * R;
            a += math.PI/2.0f;
        }
        else if(q == 2)
        {
            float n = (t - 0.5f) * 4.0f;
            x = 1.0f - R;
            y = math.lerp(1.0f - R, R, math.saturate(n/tls));

            a = 0.5f * math.PI * math.saturate((n - tls)/tlr);
            x += math.cos(a) * R;
            y -= math.sin(a) * R;
            a -= math.PI;
        }
        else
        {
            float n = (t - 0.75f) * 4.0f;
            y = R;
            x = math.lerp(1.0f - R, R, math.saturate(n/tls));

            a = 0.5f * math.PI * math.saturate((n - tls)/tlr);
            y -= math.cos(a) * R;
            x -= math.sin(a) * R;
            a -= math.PI/2.0f;
        }

        x -= 0.5f;
        y -= 0.5f;
        x *= radius;
        y *= radius;
        return new float3(x,y,a);
    }

    protected override void OnCreate()
    {
        m_TrackOccupancySystem = World.GetExistingSystem<TrackOccupancySystem>();
        Random = new Random(1234);
    }

    protected override void OnUpdate()
    {
        Frame++;
        float deltaTime = Time.DeltaTime;

        float trackRadius = TrackRadius;
        float3 trackOrigin = TrackOrigin;
        float laneWidth = LaneWidth;
        uint tilesPerLane = TrackOccupancySystem.TilesPerLane;
        uint laneCount = m_TrackOccupancySystem.LaneCount;
        uint theFrame = Frame;
        var random = Random;

        Entities
            .ForEach((ref Translation translation, ref Rotation rotation, ref CarMovement movement) =>
            {
// todo access array directly, because we don't know how to do this in DOTS
                // Look one tile ahead, if it is occupied, stop moving
// todo myTile calc must match with what is in TrackOccupanySystem
                int myTile = (int)((movement.Offset * tilesPerLane) + 0.5f) % (int)tilesPerLane;
                int nextTile = (int) ((myTile+1) % tilesPerLane);
                int prevTile = (int) (math.max(myTile-1, 0) % tilesPerLane);
                bool nextIsOccupied = TrackOccupancySystem.ReadOccupancy[movement.Lane, nextTile];

                bool randomlySwitchLanes = random.NextInt(0, 100) > 33;
                if (nextIsOccupied && movement.LaneSwitchCounter <= 0 && randomlySwitchLanes)
                {
                    // To avoid having two cars merge into the same lane, we allow
                    // mergers to the right at even frames and merges to the left at odd frames.
                    int sideLane = (int) movement.Lane;
                    bool isEven = theFrame % 2 == 0;
                    if (isEven) 
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
                
                movement.LaneSwitchCounter -= deltaTime;

                float laneRadius = (trackRadius + (movement.Lane * laneWidth));
                float v = nextIsOccupied ? SpawnerSystem.MinimumVelocity : movement.Velocity;
                float3 transXZA = MapToRoundedCorners((movement.Offset), laneRadius);

                translation.Value.x = transXZA.x + (TrackRadius)/2.0f + 2.75f;
                translation.Value.y = trackOrigin.y;
                translation.Value.z = transXZA.y + (TrackRadius)/4.0f - 6.0f;

                movement.Offset += v * deltaTime;
                movement.Offset = movement.Offset % 1.0f;

                rotation.Value = quaternion.EulerYXZ(0, transXZA.z, 0);

            }).ScheduleParallel();
    }
}
