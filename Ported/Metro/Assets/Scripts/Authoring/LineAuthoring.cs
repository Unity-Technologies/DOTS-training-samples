using Unity.Entities;
using Unity.Transforms;

using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityGizmos = UnityEngine.Gizmos;
using UnityGUI = UnityEngine.GUI;
using UnityTransform = UnityEngine.Transform;
using UnityVector3 = UnityEngine.Vector3;
using UnityColor = UnityEngine.Color;

public class LineAuthoring : UnityMonoBehaviour, IConvertGameObjectToEntity
{
    public int LineID;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var children = gameObject.GetComponentsInChildren<LineMarkerAuthoring>();
        
        dstManager.AddComponent<LineMarkerBufferElement>(entity);

        var dynamicBuffer = dstManager.GetBuffer<LineMarkerBufferElement>(entity);
        foreach (var childMarker in children)
        {
            dynamicBuffer.Add(new LineMarkerBufferElement
            {
                IsPlatform = childMarker.MarkerType == LineMarkerType.Platform,
                Position = childMarker.gameObject.transform.position
            });
        }

        dstManager.AddSharedComponentData<LineIDComponent>(entity, new LineIDComponent{Value = LineID});
    }
}
