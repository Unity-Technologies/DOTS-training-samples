using Unity.Entities;
using UnityEngine;

public class RailwayPointAuthoring : MonoBehaviour
{
    public int MetroLineId;
    public RailwayPointType RailwayPointType;
}

class RailwayPointBaker : Baker<RailwayPointAuthoring>
{
    public override void Bake(RailwayPointAuthoring authoring)
    {
        AddComponent(new RailwayPoint
        {
            RailwayPointType = authoring.RailwayPointType
        });
        AddComponent(new MetroLineID
        {
            ID = authoring.MetroLineId
        });
    }
}