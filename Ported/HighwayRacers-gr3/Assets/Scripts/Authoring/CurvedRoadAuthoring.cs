using Unity.Entities;

class CurvedRoadAuthoring : UnityEngine.MonoBehaviour
{
    
}

class CurvedRoadBaker : Baker<CurvedRoadAuthoring>
{
    public override void Bake(CurvedRoadAuthoring authoring)
    {
        AddComponent<CurvedRoad>();
    }
}