using Unity.Entities;

public class BoxesAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.Color minHeightColour;

    public UnityEngine.Color maxHeightColour;

    public UnityEngine.MeshRenderer renderMesh;

    public UnityEngine.GameObject boxSpawn;

    public UnityEngine.GameObject boxPrefab;

    public float boxHeight;

    public int boxHeightDamage;
}

[BakingType]
struct ChildrenWithRenderer : IBufferElementData
{
    public Entity value;
}

class BoxesBaker : Baker<BoxesAuthoring>
{
    public override void Bake(BoxesAuthoring authoring)
    {
        //AddComponent(new Boxes
        //{
        //    boxPrefab = GetEntity(authoring.boxPrefab),
        //    boxSpawn = GetEntity(authoring.boxSpawn),
        //    //minHeightColour = authoring.minHeightColour,
        //    //maxHeightColour = authoring.maxHeightColour,
        //    //renderMesh = authoring.renderMesh,
        //    spacing = authoring.spacing,
        //    boxHeight = authoring.boxHeight,
        //    boxHeightDamage = authoring.boxHeightDamage,
        //});

        var buffer = AddBuffer<ChildrenWithRenderer>().Reinterpret<Entity>();

        AddComponent<Boxes>();

        foreach (var renderer in GetComponentsInChildren<UnityEngine.MeshRenderer>())
        {
            buffer.Add(GetEntity(renderer));
        }
    }
}