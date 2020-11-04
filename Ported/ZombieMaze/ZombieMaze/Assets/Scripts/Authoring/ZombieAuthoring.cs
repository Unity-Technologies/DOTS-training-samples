using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

[DisallowMultipleComponent]
public class ZombieAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
#if UNITY_EDITOR
        dstManager.SetName(entity, "Zombie");
#endif
        dstManager.AddComponent<ZombieTag>(entity);
        dstManager.AddComponentData(entity, new Position());
        dstManager.AddComponentData(entity, new Direction());
        dstManager.AddComponentData(entity, new Speed(1));
        dstManager.AddComponentData(entity, new Random((uint)UnityEngine.Random.Range(0,int.MaxValue)));

        dstManager.AddComponentData(entity,
            new URPMaterialPropertyBaseColor {Value = new float4(0, UnityEngine.Random.value, 0, 1)});
    }
}
