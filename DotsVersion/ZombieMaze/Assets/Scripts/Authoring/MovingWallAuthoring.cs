using Unity.Entities;
using UnityEngine;

public class MovingWallAuthoring : UnityEngine.MonoBehaviour
{
    public float MinMoveSpeedInSeconds = 0.2f;
    public float MaxMoveSpeedInSeconds = 2.0f;
    public int MinTilesToMove = 3;
    public int MaxTilesToMove = 6;
}

class MovingWallComponentBaker : Baker<MovingWallAuthoring>
{
    public override void Bake(MovingWallAuthoring authoring)
    {
        AddComponent(new MovingWall
        {
            MoveSpeedInSeconds = Random.Range(authoring.MinMoveSpeedInSeconds, authoring.MinMoveSpeedInSeconds),
            NumberOfTilesToMove = Random.Range(authoring.MinTilesToMove, authoring.MaxTilesToMove)
        });
    }
}
