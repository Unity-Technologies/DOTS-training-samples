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

    public const float SegmentLenght = 6f;

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

        
        
         // if (lane0Length < MIN_HIGHWAY_LANE0_LENGTH)
         //            {
         //                Debug.LogError("Highway length must be longer than " + MIN_HIGHWAY_LANE0_LENGTH);
         //                return;
         //            }
         //
        	// 		int tempNumCars = numCars;
        	// 		if (lane0Length < this.lane0Length) {
        	// 			ClearCars();
        	// 		}
         //
         //            float straightPieceLength = (lane0Length - CURVE_LANE0_RADIUS * 4) / 4;
         //
         //            Vector3 pos = Vector3.zero;
         //            float rot = 0;
         //
         //            for (int i = 0; i < 8; i++)
         //            {
         //                if (i % 2 == 0)
         //                {
         //                    // straight piece
         //                    if (pieces[i] == null)
         //                    {
         //                        pieces[i] = Instantiate(straightPiecePrefab, transform).GetComponent<StraightPiece>();
         //                    }
         //                    StraightPiece straightPiece = pieces[i] as StraightPiece;
         //                    straightPiece.SetStartPosition(pos);
         //                    straightPiece.startRotation = rot;
         //                    straightPiece.SetLength(straightPieceLength);
         //
         //                    pos += straightPiece.startRotationQ * new Vector3(0, 0, straightPieceLength);
         //                }
         //                else
         //                {
         //                    // curve piece
         //                    if (pieces[i] == null)
         //                    {
         //                        pieces[i] = Instantiate(curvePiecePrefab, transform).GetComponent<CurvePiece>();
         //                    }
         //                    CurvePiece curvePiece = pieces[i] as CurvePiece;
         //                    curvePiece.SetStartPosition(pos);
         //                    curvePiece.startRotation = rot;
         //
         //                    pos += curvePiece.startRotationQ * new Vector3(MID_RADIUS, 0, MID_RADIUS);
         //                    rot = Mathf.PI / 2 * (i / 2 + 1);
         //                }
         //            }


        float straightPieceLength = 6.0f;//(trackSize - CURVE_LANE0_RADIUS * 4) / 4;
        
    
         
        Entities
            .ForEach((Entity entity, in HighwayPrefabs highway) =>
            {
                ecb.DestroyEntity(entity);
                var instance = ecb.Instantiate(highway.CurvePiecePrefab);

                // vertical sides


                float halfOffset = trackSize * 0.5f;
                
                int segmentCount = Mathf.RoundToInt(trackSize / SegmentLenght);
                float stretch = trackSize / (segmentCount * SegmentLenght);

                for (int i = 0; i < segmentCount; i++)
                {
                    var sp = ecb.Instantiate(highway.StraightPiecePrefab);
                    var trans = new Translation
                    {
                        Value = new float3(halfOffset, 0, straightPieceLength * stretch * i - halfOffset)
                    };

                    var scl = new NonUniformScale{
                            Value = new float3(1.0f, 1.0f, stretch)
                    };
                    ecb.SetComponent(sp, trans);
;                   ecb.AddComponent(sp, scl);

                    var sp2 = ecb.Instantiate(highway.StraightPiecePrefab);
                    var trans2 = new Translation
                    {
                        Value = new float3(halfOffset * -1, 0, straightPieceLength * stretch * i - halfOffset)
                    };
                    ecb.SetComponent(sp2, trans2);
                    ecb.AddComponent(sp2, scl);

                    var rot = new Rotation {
                        Value = Quaternion.AngleAxis(90, Vector3.up)
                    };
                    
                    var sp3 = ecb.Instantiate(highway.StraightPiecePrefab);
                    var trans3 = new Translation
                    {
                        Value = new float3( straightPieceLength * stretch * i -halfOffset, 0, halfOffset * -1)
                    };
                    ecb.SetComponent(sp3, trans3);
                    ecb.SetComponent(sp3, rot);
                    ecb.AddComponent(sp3, scl);
                    
                    
                    var sp4 = ecb.Instantiate(highway.StraightPiecePrefab);
                    var trans4 = new Translation
                    {
                        Value = new float3( straightPieceLength * stretch * i - halfOffset, 0, halfOffset)
                    };
                    ecb.SetComponent(sp4, trans4);
                    ecb.SetComponent(sp4, rot);
                    ecb.AddComponent(sp4, scl);

                    
// learning note: the Add is needed bacause there is no 
                    // scale component by defaul
                }
                
            }).WithoutBurst().Run();

        ecb.Playback(EntityManager);
    }
    
    
    
}
