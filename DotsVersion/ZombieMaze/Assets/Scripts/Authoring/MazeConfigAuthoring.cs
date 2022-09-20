using Unity.Entities;
using UnityEngine;

public class MazeConfigAuthoring : MonoBehaviour
{
    public int Width = 100;
    public int Height = 100;
    public int PillsToSpawn = 20;
    public int ZombiesToSpawn;
    public int OpenStrips = 4;
    public int MazeStrips = 4;
    public int MovingWallsToSpawn = 15;
    public int MovingWallSize = 10;
}

class MazeConfigBaker : Baker<MazeConfigAuthoring>
{
    public override void Bake(MazeConfigAuthoring authoring)
    {
        AddComponent(new MazeConfig
        {
            Width = authoring.Width,
            Height = authoring.Height,
            PillsToSpawn = authoring.PillsToSpawn,
            ZombiesToSpawn = authoring.ZombiesToSpawn,
            OpenStrips = authoring.OpenStrips,
            MazeStrips = authoring.MazeStrips,
            MovingWallsToSpawn = authoring.MovingWallsToSpawn,
            MovingWallSize = authoring.MovingWallSize
        });
    }
}