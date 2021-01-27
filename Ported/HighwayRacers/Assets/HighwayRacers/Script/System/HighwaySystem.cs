using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using Unity.Mathematics;
using UnityEngine.UI;

[AlwaysUpdateSystem]
public class HighwaySystem : SystemBase
{
    private Slider TerrainSlider;
    private float LastTrackSize = -1f;
    private float straightPieceLength = -1f;
    private float cornerRadius = -1f;
    private EntityQuery ToBeDeleted;
    
    protected override void OnCreate()
    {
        var trackUIGO = GameObject.FindWithTag("TrackUI");
        TerrainSlider = trackUIGO.GetComponent<Slider>();
        // tracks previously generated peices for regeneration
        ToBeDeleted = GetEntityQuery(typeof(HighwayPiece));
    }
    
    protected override void OnUpdate()
    {

        if (LastTrackSize == TerrainSlider.value)
        {
            return;
        }
        else
        {
            LastTrackSize = TerrainSlider.value;
            Debug.Log("Track Size Changed");
        }

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // lazy man's sentinel value
        // in unset (= first time) grab the setup data from the
        // trackInfo
        if (straightPieceLength < 0)
        {
            var tInfo = GetSingletonEntity<TrackInfo>();
            var tInfoComp = GetComponent<TrackInfo>(tInfo);
            
            // these could really be constants but it's good
            // to make them data driven
            straightPieceLength = tInfoComp.SegmentLength;
            cornerRadius = tInfoComp.CornerRadius;
        }

        // construction params
        float trackSize = LastTrackSize;
        float straightLen = trackSize - (2 * cornerRadius);
        float halfOffset = trackSize * 0.5f;
        float cornerOffset = cornerRadius;
        int segmentCount = Mathf.RoundToInt(straightLen / straightPieceLength);
        float stretch = straightLen / (segmentCount * straightPieceLength);
        
        // Delete the pre-existing track
        EntityManager.DestroyEntity(ToBeDeleted);
        
        // layout the straight segments as 4 lines of instances
        Entities
            .ForEach((Entity entity, in HighwayPrefabs highway) =>
            {
                var hp = new HighwayPiece(); 
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
                    ecb.AddComponent(sp, scl);
                    ecb.AddComponent(sp, hp);

                    var sp2 = ecb.Instantiate(highway.StraightPiecePrefab);
                    var trans2 = new Translation
                    {
                        Value = new float3(halfOffset * -1, 0, straightPieceLength * stretch * i - halfOffset + cornerOffset)
                    };
                    ecb.SetComponent(sp2, trans2);
                    ecb.AddComponent(sp2, scl);
                    ecb.AddComponent(sp2, hp);


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
                    ecb.AddComponent(sp3, hp);

                    
                    var sp4 = ecb.Instantiate(highway.StraightPiecePrefab);
                    var trans4 = new Translation
                    {
                        Value = new float3( straightPieceLength * stretch * i - halfOffset +cornerOffset, 0, halfOffset)
                    };
                    ecb.SetComponent(sp4, trans4);
                    ecb.SetComponent(sp4, rot);
                    ecb.AddComponent(sp4, scl);
                    ecb.AddComponent(sp4, hp);

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
                ecb.AddComponent(c1, hp);

                
                var c2 = ecb.Instantiate(highway.CurvePiecePrefab);
                var c2t = new Translation
                {
                    Value = new float3(-1 * halfOffset, 0, 1 * halfOffset -cornerRadius)
                };
                ecb.SetComponent(c2, c2t);
                ecb.AddComponent(c2, hp);

   
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
                ecb.AddComponent(c3, hp);


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
                ecb.AddComponent(c4, hp);

                
            }).WithoutBurst().Run();

        
        ecb.Playback(EntityManager);
    }
    
    
    
}
