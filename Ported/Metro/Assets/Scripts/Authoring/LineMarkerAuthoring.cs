using Unity.Entities;
using Unity.Transforms;

using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityGizmos = UnityEngine.Gizmos;
using UnityGUI = UnityEngine.GUI;
using UnityTransform = UnityEngine.Transform;
using UnityVector3 = UnityEngine.Vector3;
using UnityColor = UnityEngine.Color;

#if UNITY_EDITOR
using UnityEditorHandles = UnityEditor.Handles;
#endif

public enum LineMarkerType
{
    Platform,
    Route
}

[UnityEngine.ExecuteAlways]
public class LineMarkerAuthoring : UnityMonoBehaviour, IConvertGameObjectToEntity
{
    public int LineID;
    public LineMarkerType MarkerType = LineMarkerType.Route;
    public int MarkerRouteIndex;
    
    #region Legacy Code
    private void Awake()
    {
        MarkerRouteIndex = GetSiblingIndex(transform, transform.parent);
        name = LineID + "_" + MarkerRouteIndex;
    }
    public void OnDrawGizmos()
    {
        UnityGizmos.color = UnityGUI.color = (MarkerType != LineMarkerType.Platform) ?  GetColorFromIndex( LineID ) : UnityColor.white;
		
        // Draw marker X
        float xSize = 0.5f;
        UnityGizmos.DrawLine(transform.position + new UnityVector3(-xSize, 0f, -xSize), transform.position + new UnityVector3(xSize, 0f, xSize));
        UnityGizmos.DrawLine(transform.position + new UnityVector3(xSize, 0f, -xSize), transform.position + new UnityVector3(-xSize, 0f, xSize));
		
        // connect to next in line (if found)
        if (MarkerRouteIndex != transform.parent.childCount-1)
        {
            UnityGizmos.DrawLine(transform.position, transform.parent.GetChild(MarkerRouteIndex+1).position);
        }
		
#if UNITY_EDITOR
        UnityEditorHandles.Label(transform.position + new UnityVector3(0f,1f,0f), LineID +"_"+MarkerRouteIndex + ((MarkerType == LineMarkerType.Platform) ? " **" : ""));
#endif
    }
	
    static int GetSiblingIndex(UnityTransform child, UnityTransform parent)
    {
        int result = 0;
        for (int i = 0; i < parent.childCount; ++i)
        {
            if (child == parent.GetChild(i))
				
                result = i;
        }
        return result;
    }

    static UnityColor GetColorFromIndex(int lineID)
    {
        switch (lineID % 3)
        {
            case 0:
                return UnityColor.magenta;
            case 1:
                return UnityColor.cyan;
            case 2:
                return UnityColor.yellow;
            default:
                return UnityColor.green;
        }
    }
    #endregion
    
    #region ECS Conversion

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddSharedComponentData<LineIDComponent>(entity, new LineIDComponent {Value = LineID});
        dstManager.AddComponentData<LineMarkerIndexComponent>(entity, new LineMarkerIndexComponent {Value = MarkerRouteIndex});

        if (MarkerType == LineMarkerType.Platform)
        {
            dstManager.AddComponent<PlatformComponent>(entity);
        }
    }
    
    #endregion
}
