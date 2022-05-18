using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

class TileGridConfigAuthoring : UnityEngine.MonoBehaviour
{
    public int Size;
    public float CellSize;
    public UnityEngine.GameObject TilePrefab;
    public Color GrassColor;
    public Color LightFireColor;
    public Color MediumFireColor;
    public Color IntenseFireColor;

    public int Spacing;
    public int OuterSize;
    public int NbOfWaterTiles;
    public Color LightWaterColor;
    public Color MediumWaterColor;
    public Color IntenseWaterColor;
}

class TileGridConfigBaker : Baker<TileGridConfigAuthoring>
{
    public override void Bake(TileGridConfigAuthoring authoring)
    {
        AddComponent(new TileGridConfig
        {
            Size = authoring.Size,
            CellSize = authoring.CellSize,
            TilePrefab = GetEntity(authoring.TilePrefab),
            GrassColor = new float4(authoring.GrassColor.r, authoring.GrassColor.g, authoring.GrassColor.b, authoring.GrassColor.a),
            LightFireColor = new float4(authoring.LightFireColor.r, authoring.LightFireColor.g, authoring.LightFireColor.b, authoring.LightFireColor.a),
            MediumFireColor = new float4(authoring.MediumFireColor.r, authoring.MediumFireColor.g, authoring.MediumFireColor.b, authoring.MediumFireColor.a),
            IntenseFireColor = new float4(authoring.IntenseFireColor.r, authoring.IntenseFireColor.g, authoring.IntenseFireColor.b, authoring.IntenseFireColor.a),
            
            Spacing = authoring.Spacing,
            OuterSize = authoring.OuterSize,
            NbOfWaterTiles = authoring.NbOfWaterTiles,
            
            LightWaterColor = new float4(authoring.LightWaterColor.r, authoring.LightWaterColor.g, authoring.LightWaterColor.b, authoring.LightWaterColor.a),
            MediumWaterColor = new float4(authoring.MediumWaterColor.r, authoring.MediumWaterColor.g, authoring.MediumWaterColor.b, authoring.MediumWaterColor.a),
            IntenseWaterColor = new float4(authoring.IntenseWaterColor.r, authoring.IntenseWaterColor.g, authoring.IntenseWaterColor.b, authoring.IntenseWaterColor.a),
        });
    }
}
