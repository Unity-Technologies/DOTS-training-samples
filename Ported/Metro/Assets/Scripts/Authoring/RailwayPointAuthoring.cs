using Unity.Entities;
using UnityEngine;

public class RailwayPointAuthoring : MonoBehaviour
{
    public RailwayPointType RailwayPointType;
    public int StationId;

    [HideInInspector] public RailwayPointAuthoring NextPoint;
    [HideInInspector] public RailwayPointAuthoring LastPoint;
}

class RailwayPointBaker : Baker<RailwayPointAuthoring>
{
    public override void Bake(RailwayPointAuthoring authoring)
    {
        AddComponent(new RailwayPoint
        {
            RailwayPointType = authoring.RailwayPointType,
            NextPoint = authoring.NextPoint.transform.position,
            PreviousPoint = authoring.LastPoint.transform.position
        });
    }
}