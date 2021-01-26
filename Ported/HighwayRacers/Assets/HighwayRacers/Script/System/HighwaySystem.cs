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
        float trackSize = GetComponent<TrackInfo>(tInfo).TrackSize;
        
        // deleter the TrackInfo so this only runs at init
        ecb.DestroyEntity(tInfo);

        
        Entities
            .ForEach((Entity entity, in HighwayPrefabs highway) =>
            {
                ecb.DestroyEntity(entity);
                var instance = ecb.Instantiate(highway.CurvePiecePrefab);
                
                for (int i = 0; i < 100; i++)
                {
                    var inst = ecb.Instantiate(highway.StraightPiecePrefab);

                    var tranlation = new Translation
                    {
                        Value = new float3(i * trackSize , 0, 0)
                    };
                    ecb.SetComponent(inst,tranlation);
                }
            }).WithoutBurst().Run();

        ecb.Playback(EntityManager);
    }
    
    
    
}
