using HighwayRacersOldCode;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class HighwaySystem : SystemBase
{
    public const int NUM_LANES = 4;
    public const float LANE_SPACING = 1.9f;
    public const float MID_RADIUS = 31.46f;
    public const float CURVE_LANE0_RADIUS = MID_RADIUS - LANE_SPACING * (NUM_LANES - 1) / 2f;
    public const float MIN_HIGHWAY_LANE0_LENGTH = CURVE_LANE0_RADIUS * 4;
    public const float MIN_DIST_BETWEEN_CARS = .7f;
    
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // The PRNG (pseudorandom number generator) from Unity.Mathematics is a struct
        // and can be used in jobs. For simplicity and debuggability in development,
        // we'll initialize it with a constant. (In release, we'd want a seed that
        // randomly varies, such as the time from the user's system clock.)
        var random = new Random(1234);

        Entities
            .ForEach((Entity entity, in HighwayPrefabs highway) =>
            {
                ecb.DestroyEntity(entity);

                var lane0Length = 250;
                if (lane0Length < MIN_HIGHWAY_LANE0_LENGTH)
                {
                    Debug.LogError("Highway length must be longer than " + MIN_HIGHWAY_LANE0_LENGTH);
                    return;
                }
                
                float straightPieceLength = (lane0Length - CURVE_LANE0_RADIUS * 4) / 4;
    
                Vector3 pos = Vector3.zero;
                float rot = 0;
    
                for (int i = 0; i < 8; i++)
                {
                    if (i % 2 == 0)
                    {
                        // straight piece
                        var instance = ecb.Instantiate(highway.StraightPiecePrefab);
                        ecb.SetComponent(instance, new Translation() { Value = pos });

                        var pieceRot = Quaternion.Euler(0, rot * Mathf.Rad2Deg, 0);
                        ecb.SetComponent(instance, new Rotation() { Value = pieceRot });
                        ecb.SetComponent(instance, new HighwayPiece() { });
                        ecb.SetComponent(instance, new StraightPiece()
                        {
                            Length = straightPieceLength,
                            baseLength = 6,
                            baseScaleY = 6
                        });
                        
                        pos += pieceRot * new Vector3(0, 0, straightPieceLength);
                    }
                    else
                    {
                        // curve piece
                        var instance = ecb.Instantiate(highway.CurvePiecePrefab);
                        ecb.SetComponent(instance, new Translation() { Value = pos });
                        
                        var pieceRot = Quaternion.Euler(0, rot * Mathf.Rad2Deg, 0);
                        ecb.SetComponent(instance, new Rotation() { Value = pieceRot });
                        
                        ecb.SetComponent(instance, new HighwayPiece(){});
                        
                        //ecb.SetComponent();
                        
                        //TODO : depends on lane
                        // ecb.SetComponent(instance, new CurvePiece()
                        // {
                        //     Lengths = new []
                        //     {
                        //         
                        //     }
                        // });
                        
                        pos += pieceRot * new Vector3(MID_RADIUS, 0, MID_RADIUS);
                        rot = Mathf.PI / 2 * (i / 2 + 1);
                    }
                }
            }).WithoutBurst().Run();

        ecb.Playback(EntityManager);
    }
}
