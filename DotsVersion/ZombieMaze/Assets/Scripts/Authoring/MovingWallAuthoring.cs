using Unity.Entities;
using UnityEngine;

public class MovingWallAuthoring : UnityEngine.MonoBehaviour
{

}

class MovingWallComponentBaker : Baker<MovingWallAuthoring>
{
    public override void Bake(MovingWallAuthoring authoring)
    {
        AddComponent(new MovingWall
        {
        });
    }
}
