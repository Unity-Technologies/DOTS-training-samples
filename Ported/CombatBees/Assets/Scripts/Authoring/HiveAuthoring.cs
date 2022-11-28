using Unity.Entities;
using UnityEngine;

class HiveAuthoring : MonoBehaviour
{
    public GameObject beePrefab;
    public int startBeeCount;
    public Color beeColor;
    public int team;
}

class HiveBaker : Baker<HiveAuthoring>
{
    public override void Bake(HiveAuthoring authoring)
    {
        var bounds = authoring.GetComponent<Renderer>().bounds;
        AddComponent(new Hive
        {
            beePrefab = GetEntity(authoring.beePrefab),
            startBeeCount = authoring.startBeeCount,
            color = (Vector4)authoring.beeColor,
            boundsPosition = bounds.center,
            boundsExtents = bounds.extents
        });
        AddSharedComponent(new Team
        {
            number = authoring.team
        });
    }
}