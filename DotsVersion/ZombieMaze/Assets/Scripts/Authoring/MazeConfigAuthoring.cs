using Unity.Entities;
using UnityEngine;

public class MazeConfigAuthoring : MonoBehaviour
{
    public int Width = 100;
    public int Height = 100;
}

class MazeConfigBaker : Baker<MazeConfigAuthoring>
{
    public override void Bake(MazeConfigAuthoring authoring)
    {
        AddComponent(new MazeConfig
        {
            Width = authoring.Width,
            Height = authoring.Height
        });
    }
}