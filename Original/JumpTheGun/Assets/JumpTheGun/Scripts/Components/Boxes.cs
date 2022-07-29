using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Rendering;

// An empty component is called a "tag component".
public struct Boxes : IComponentData
{
    public int row;
    public int column;

    public MaterialColor minHeightColour;

    public MaterialColor maxHeightColour;

    //public RenderMesh renderMesh;

    public Entity boxSpawn;

    public Entity boxPrefab;

    public float spacing;

    public float boxHeight;

    public int boxHeightDamage;

    public float top;
}