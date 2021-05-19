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
    public int TeamId;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<Bounds>(entity);
        dstManager.AddComponent<Team>(entity);

        dstManager.AddComponentData(entity, new Team
        {
            Id = TeamId
        });

        if (TeamId == 0)
        {
            dstManager.AddComponent<YellowBase>(entity);
        }
        else
        {
            dstManager.AddComponent<BlueBase>(entity);
        }

        var translation = transform.position;
        var extents = transform.localScale / 2;
        extents = math.abs(extents);

        dstManager.AddComponentData(entity, new Bounds
        {
            Value = new AABB
            {
                Center = translation,
                Extents = extents
            }
        });
    }
}