using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class HeatMapDataAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [Tooltip("Size of One Side")]
    public int gridSize = 10;
    public float maxFlameHeight = 5f;
    public float heatPropagationSpeed = 0.003f;
    
    public UnityEngine.Color colorNeutral;
    public UnityEngine.Color colorCool;
    public UnityEngine.Color colorHot;
    
    public void Convert(Entity entity, EntityManager dstManager
        , GameObjectConversionSystem conversionSystem)
    {
        // GetPrimaryEntity fetches the entity that resulted from the conversion of
        // the given GameObject, but of course this GameObject needs to be part of
        // the conversion, that's why DeclareReferencedPrefabs is important here.
        dstManager.AddComponentData(entity, new HeatMapData()
        {
            mapSideLength = gridSize, 
            maxTileHeight = maxFlameHeight,
            heatPropagationSpeed = this.heatPropagationSpeed,
            colorNeutral = new float4(this.colorNeutral.r, this.colorNeutral.g, this.colorNeutral.b, 1f),// new float4(0.49f,0.8f,0.46f,1f),
            colorCool = new float4(this.colorCool.r, this.colorCool.g, this.colorCool.b, 1f),//new float4(1f,1f,0.5f,1f),
            colorHot = new float4(this.colorHot.r, this.colorHot.g, this.colorHot.b, 1f),//new float4(1f,0f,0f,1f)
        });
    }
}
