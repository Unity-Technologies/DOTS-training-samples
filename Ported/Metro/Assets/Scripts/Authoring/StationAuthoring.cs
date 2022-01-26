using Unity.Entities;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

public class StationAuthoring : UnityMonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager
        , GameObjectConversionSystem conversionSystem)
    {
        StationPlacementHelper helper = GetComponent<StationPlacementHelper>();
        /*if (helper != null) {
            UnityEngine.Debug.Log(helper.distanceFromSplineOrigin);
        }*/
        dstManager.AddComponentData(entity, new Station { trackDistance = helper != null ? helper.distanceFromSplineOrigin : 0.0f });
    }
}