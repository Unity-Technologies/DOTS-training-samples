using Unity.Entities;
using Unity.Mathematics;

public struct WalkwayPoints : IComponentData
{
    public float3 WalkwayFrontBottom;
    public float3 WalkwayFrontTop;
    public float3 WalkwayBackBottom;
    public float3 WalkwayBackTop;
}

public class WalkwayPointsAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
{
    public UnityEngine.GameObject WalkwayFrontBottom;
    public UnityEngine.GameObject WalkwayFrontTop;
    public UnityEngine.GameObject WalkwayBackBottom;
    public UnityEngine.GameObject WalkwayBackTop;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new WalkwayPoints
        {
            WalkwayFrontBottom = new float3(WalkwayFrontBottom.transform.position.x, WalkwayFrontBottom.transform.position.y, WalkwayFrontBottom.transform.position.z),
            WalkwayFrontTop = new float3(WalkwayFrontTop.transform.position.x, WalkwayFrontTop.transform.position.y, WalkwayFrontTop.transform.position.z),
            WalkwayBackBottom = new float3(WalkwayBackBottom.transform.position.x, WalkwayBackBottom.transform.position.y, WalkwayBackBottom.transform.position.z),
            WalkwayBackTop = new float3(WalkwayBackTop.transform.position.x, WalkwayBackTop.transform.position.y, WalkwayBackTop.transform.position.z),
        });
    }
}
