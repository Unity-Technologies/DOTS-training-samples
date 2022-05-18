using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

class TileGridConfigAuthoring : UnityEngine.MonoBehaviour
{
    public int Size;
    public UnityEngine.GameObject TilePrefab;
    public Color GrassColor;
    public Color LightFireColor;
    public Color MediumFireColor;
    public Color IntenseFireColor;
}

class TileGridConfigBaker : Baker<TileGridConfigAuthoring>
{
    public override void Bake(TileGridConfigAuthoring authoring)
    {
        AddComponent(new TileGridConfig
        {
            Size = authoring.Size,
            TilePrefab = GetEntity(authoring.TilePrefab),
            GrassColor = new float4(authoring.GrassColor.r, authoring.GrassColor.g, authoring.GrassColor.b, authoring.GrassColor.a),
            LightFireColor = new float4(authoring.LightFireColor.r, authoring.LightFireColor.g, authoring.LightFireColor.b, authoring.LightFireColor.a),
            MediumFireColor = new float4(authoring.MediumFireColor.r, authoring.MediumFireColor.g, authoring.MediumFireColor.b, authoring.MediumFireColor.a),
            IntenseFireColor = new float4(authoring.IntenseFireColor.r, authoring.IntenseFireColor.g, authoring.IntenseFireColor.b, authoring.IntenseFireColor.a),
        });
    }
}
