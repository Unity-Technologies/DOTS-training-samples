using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityCamera = UnityEngine.Camera;
using UnityGameObject = UnityEngine.GameObject;
using UnityInput = UnityEngine.Input;
using UnityKeyCode = UnityEngine.KeyCode;
using UnityMeshRenderer = UnityEngine.MeshRenderer;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityRangeAttribute = UnityEngine.RangeAttribute;


public class FieldAuthoring : UnityMonoBehaviour, IConvertGameObjectToEntity
{

    public void Convert(Entity entity, EntityManager dstManager
        , GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<Bounds>(entity);
        dstManager.AddComponent<IsArena>(entity);

        var translation = transform.position;
        var extents = transform.localScale/2;
        extents = math.abs(extents);

        dstManager.AddComponentData(entity, new Bounds
        {
            Value = new AABB { Center = translation, Extents = extents }
        });

    }
}
