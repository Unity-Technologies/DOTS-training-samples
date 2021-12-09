using Unity.Entities;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityRange = UnityEngine.RangeAttribute;
public class BeeAuthoring : UnityMonoBehaviour//, IConvertGameObjectToEntity
{
    // [UnityRange(0.0f, 3.0f)] 
    // public float BeeSpeed = 1.0f;
    // public float TargetWithinReach = 0.1f;
    //
    // public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    // {
    //     // dstManager.AddComponent<BeeMovement>(entity);
    //     // dstManager.AddComponent<BeeTargets>(entity);
    //     //
    //     // dstManager.AddComponentData(entity, new BeeMovement
    //     // {
    //     //     Speed = BeeSpeed
    //     // });
    //     //
    //     // dstManager.AddComponentData(entity, new BeeTargets
    //     // {
    //     //     TargetWithinReach = TargetWithinReach
    //     // });
    // }
}
