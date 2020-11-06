using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[DisallowMultipleComponent]
public class MovingWallAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        
#if UNITY_EDITOR
        dstManager.SetName(entity, "Moving Wall");
#endif
        dstManager.AddComponentData(entity, new MovingWall());
        dstManager.AddComponentData(entity, new Position());
        dstManager.AddComponentData(entity, new Speed());
        dstManager.AddComponentData(entity, new Random((uint)UnityEngine.Random.Range(0, int.MaxValue)));

        //dstManager.AddComponentData(entity, new Direction());
        //        dstManager.AddComponentData(entity, new NonUniformScale());
    }
}
