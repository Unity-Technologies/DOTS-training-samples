using System;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class RailMarkerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField] private int trackID;
    [SerializeField] private RailMarkerType markerType;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new RailMarker {Index = transform.GetSiblingIndex(), MarkerType = (int) markerType});
        dstManager.AddSharedComponentData(entity, new ID {Value = trackID});
    }
}