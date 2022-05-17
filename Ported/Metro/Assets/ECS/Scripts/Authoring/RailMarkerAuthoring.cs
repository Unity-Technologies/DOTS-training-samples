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
            Index = authoring.transform.GetSiblingIndex(),
            MarkerType = authoring.MarkerType
        });
    }
}