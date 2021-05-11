using System.Collections.Generic;
using Unity.Entities;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityGameObject = UnityEngine.GameObject;
using UnityCamera = UnityEngine.Camera;
using UnityInput = UnityEngine.Input;
using UnityKeyCode = UnityEngine.KeyCode;
using UnityMeshRenderer = UnityEngine.MeshRenderer;
using UnityRangeAttribute = UnityEngine.RangeAttribute;

public class BulletAuthoring : UnityMonoBehaviour
    , IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    public UnityGameObject BulletPrefab;

    public void DeclareReferencedPrefabs(List<UnityEngine.GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(BulletPrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Bullet()
        {
            BulletPrefab = conversionSystem.GetPrimaryEntity(BulletPrefab)
        });
    }
}
