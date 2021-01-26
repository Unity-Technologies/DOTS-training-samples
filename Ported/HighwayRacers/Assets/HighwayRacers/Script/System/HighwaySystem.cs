using System;
using HighwayRacersOldCode;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using Unity.Mathematics;
public class HighwaySystem : SystemBase
{
    public const int NUM_LANES = 4;
    public const float LANE_SPACING = 1.9f;
    public const float MID_RADIUS = 31.46f;
    public const float CURVE_LANE0_RADIUS = MID_RADIUS - LANE_SPACING * (NUM_LANES - 1) / 2f;
    public const float MIN_HIGHWAY_LANE0_LENGTH = CURVE_LANE0_RADIUS * 4;
    public const float MIN_DIST_BETWEEN_CARS = .7f;


    protected override void OnCreate()
    {
        RequireSingletonForUpdate<TrackInfo>();
    }

    protected override void OnUpdate()
    {
        
        
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // The PRNG (pseudorandom number generator) from Unity.Mathematics is a struct
        // and can be used in jobs. For simplicity and debuggability in development,
        // we'll initialize it with a constant. (In release, we'd want a seed that
        // randomly varies, such as the time from the user's system clock.)
        var random = new Random(1234);


        var tInfo = GetSingletonEntity<TrackInfo>();
        var tInfoComp = GetComponent<TrackInfo>(tInfo);
        
        // collect the track info
        float straightPieceLength = tInfoComp.SegmentLength;
        float cornerRadius = tInfoComp.CornerRadius;
        float trackSize = tInfoComp.TrackSize;
        
        // deleter the TrackInfo so this only runs at init
        ecb.DestroyEntity(tInfo);

        // length between the corner radii
        float straightLen = trackSize - (2 * cornerRadius);
         
        
        // layout the straight segments as 4 lines of instances
        Entities
            .ForEach((Entity entity, in HighwayPrefabs highway) =>
            {
                // this probably wants to go if we need to rebuild this on the fly
                ecb.DestroyEntity(entity);

                float halfOffset = trackSize * 0.5f;
                float cornerOffset = cornerRadius;
                int segmentCount = Mathf.RoundToInt(straightLen / straightPieceLength);
                float stretch = straightLen / (segmentCount * straightPieceLength);

                // layout straight segments
                for (int i = 0; i < segmentCount; i++)
                {
                    var sp = ecb.Instantiate(highway.StraightPiecePrefab);
                    var trans = new Translation
                    {
                        Value = new float3(halfOffset, 0, straightPieceLength * stretch * i - halfOffset + cornerOffset)
                    };

                    var scl = new NonUniformScale{
                            Value = new float3(1.0f, 1.0f, stretch)
                    };
                    ecb.SetComponent(sp, trans);
;                   ecb.AddComponent(sp, scl);

                    var sp2 = ecb.Instantiate(highway.StraightPiecePrefab);
                    var trans2 = new Translation
                    {
                        Value = new float3(halfOffset * -1, 0, straightPieceLength * stretch * i - halfOffset + cornerOffset)
                    };
                    ecb.SetComponent(sp2, trans2);
                    ecb.AddComponent(sp2, scl);

                    var rot = new Rotation {
                        Value = Quaternion.AngleAxis(90, Vector3.up)
                    };
                    
                    var sp3 = ecb.Instantiate(highway.StraightPiecePrefab);
                    var trans3 = new Translation
                    {
                        Value = new float3( straightPieceLength * stretch * i -halfOffset + cornerOffset, 0, halfOffset * -1)
                    };
                    ecb.SetComponent(sp3, trans3);
                    ecb.SetComponent(sp3, rot);
                    ecb.AddComponent(sp3, scl);
                    
                    
                    var sp4 = ecb.Instantiate(highway.StraightPiecePrefab);
                    var trans4 = new Translation
                    {
                        Value = new float3( straightPieceLength * stretch * i - halfOffset +cornerOffset, 0, halfOffset)
                    };
                    ecb.SetComponent(sp4, trans4);
                    ecb.SetComponent(sp4, rot);
                    ecb.AddComponent(sp4, scl);

                }
                
                // corners.  This would be nicer if done with a proper pivot offset
                var c1 = ecb.Instantiate(highway.CurvePiecePrefab);
                var c1t = new Translation
                {
                    Value = new float3(-1 * halfOffset + cornerRadius, 0, -1 * halfOffset)
                };
                ecb.SetComponent(c1, c1t);
                var c1r = new Rotation
                {
                    Value = Quaternion.AngleAxis(-90, Vector3.up)
                };
                ecb.SetComponent(c1, c1r);
                
                var c2 = ecb.Instantiate(highway.CurvePiecePrefab);
                var c2t = new Translation
                {
                    Value = new float3(-1 * halfOffset, 0, 1 * halfOffset -cornerRadius)
                };
                ecb.SetComponent(c2, c2t);
   
                var c3 = ecb.Instantiate(highway.CurvePiecePrefab);
                var c3t = new Translation
                {
                    Value = new float3(1 * halfOffset - cornerRadius, 0, 1 * halfOffset )
                };
                
                ecb.SetComponent(c3, c3t);
                var c3r = new Rotation
                {
                    Value = Quaternion.AngleAxis(90, Vector3.up)
                };
                ecb.SetComponent(c3, c3r);

                var c4 = ecb.Instantiate(highway.CurvePiecePrefab);
                var c4t = new Translation
                {
                    Value = new float3(1* halfOffset, 0, -1 * halfOffset + cornerRadius)
                };
                ecb.SetComponent(c4, c4t);
                var c4r = new Rotation
                {
                    Value = Quaternion.AngleAxis(180, Vector3.up)
                };
                ecb.SetComponent(c4, c4r);
                
            }).WithoutBurst().Run();

        ecb.Playback(EntityManager);
    }
    
    
    
}
