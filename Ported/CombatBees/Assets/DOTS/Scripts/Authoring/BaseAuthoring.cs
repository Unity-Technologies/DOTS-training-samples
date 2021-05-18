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


public class BaseAuthoring : UnityMonoBehaviour, IConvertGameObjectToEntity
{
    public int team;


    public void Convert(Entity entity, EntityManager dstManager
        , GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<Bounds>(entity);
        dstManager.AddComponent<Team>(entity);

        dstManager.AddComponentData(entity, new Team
        {
            Id = team
        });

        var translation = new float3(0, 0, 0);   //GetComponent<Translation>(entity);

        dstManager.AddComponentData(entity, new Bounds
        {
            Value = new AABB
            {
                Center = translation
            }
           
        }) ;

    }
}