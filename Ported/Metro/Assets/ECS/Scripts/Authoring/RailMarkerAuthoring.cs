using Unity.Entities;
using UnityEngine;

public class RailMarkerAuthoring : MonoBehaviour
{
    public RailMarkerType MarkerType = RailMarkerType.ROUTE;
}

public class RailMarkerBaker : Baker<RailMarkerAuthoring>
{
    public override void Bake(RailMarkerAuthoring authoring)
    {
        AddComponent(new RailMarker
        {
            Index = authoring.transform.GetSiblingIndex()
        });

        switch (authoring.MarkerType)
        {
            case RailMarkerType.ROUTE:
                AddComponent(new RouteTag());
                break;

            case RailMarkerType.PLATFORM_START:
                AddComponent(new PlatformStartTag());
                break;

            case RailMarkerType.PLATFORM_END:
                AddComponent(new PlatformEndTag());
                break;

            default:
                Debug.LogError($"Unknown {nameof(RailMarkerType)}");
                break;
        }
    }
}