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
    // TODO : to use in CarMovementSystem
    public const int NUM_LANES = 4;
    public const float LANE_SPACING = 1.9f;
    public const float STRAIGHT_PIECE_LENGTH = 6.0f;
    public const float MID_RADIUS = 31.46f;
    public const float CURVE_LANE0_RADIUS = MID_RADIUS - LANE_SPACING * (NUM_LANES - 1) / 2f;
    public static float TrackSize;
    
    
    private Slider TerrainSlider;
    private float straightPieceLength = -1f;
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
        if (TrackSize == TerrainSlider.value)
        {
            return;
        }
        else
        {
            TrackSize = TerrainSlider.value;
            //Debug.Log("Track Size Changed");
        }

        var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        // Delete the pre-existing track
        EntityManager.DestroyEntity(ToBeDeleted);
        
        // layout the straight segments as 4 lines of instances
        Entities
            .ForEach((Entity entity, in HighwayPrefabs highway) =>
            {
                float lane0Length = TrackSize;
                float straightPieceLength = (lane0Length - CURVE_LANE0_RADIUS * 4) / 4;
                
                Vector3 pos = Vector3.zero;
                float rot = 0;

                for (int i = 0; i < 8; i++)
                {
                    if (i % 2 == 0)
                    {
                        // straight piece
                        var rotQ = Quaternion.Euler(0, rot * Mathf.Rad2Deg, 0);
                        var straight = ecb.Instantiate(highway.StraightPiecePrefab);
                        ecb.SetComponent(straight, new Translation()
                        {
                            Value = new float3(pos.x, pos.y, pos.z) 
                        });

                        ecb.SetComponent(straight, new Rotation()
                        {
                            Value = rotQ
                        });
                        
                        ecb.SetComponent(straight, new NonUniformScale()
                        {
                            Value = new float3(1, 1, straightPieceLength/STRAIGHT_PIECE_LENGTH)
                        });
                        
                        pos += rotQ * new Vector3(0, 0, straightPieceLength);
                    }
                    else
                    {
                        // curve piece
                        var curved = ecb.Instantiate(highway.CurvePiecePrefab);
                        ecb.SetComponent(curved, new Translation()
                        {
                            Value = pos 
                        });

                        var rotQ = Quaternion.Euler(0, rot * Mathf.Rad2Deg, 0);
                        ecb.SetComponent(curved, new Rotation()
                        {
                            Value = rotQ
                        });
                        
                        pos += rotQ * new Vector3(MID_RADIUS, 0, MID_RADIUS);
                        rot = Mathf.PI / 2 * (i / 2 + 1);
                    }
                }
                
            }).WithoutBurst().Run();
        
        ecb.Playback(EntityManager);
    }
}
