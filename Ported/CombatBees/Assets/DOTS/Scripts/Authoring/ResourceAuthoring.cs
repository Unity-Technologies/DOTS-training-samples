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

public class ResourceAuthoring : UnityMonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<IsResource>(entity);
        dstManager.AddComponent<HasGravity>(entity);
        dstManager.AddComponent<Velocity>(entity);
    }
}
