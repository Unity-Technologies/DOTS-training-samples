using UnityEngine;
using Unity.Entities;

public class BoardConfigAuthoring : MonoBehaviour
{
    public int breadth = 13;
    public int width = 13;
    
    public UnityEngine.GameObject whiteTile, grayTile, wall;
}

class BoardConfigBaker : Baker<BoardConfigAuthoring>
{
    public override void Bake(BoardConfigAuthoring authoring)
    {
        AddComponent(new BoardConfig
        {
            height = authoring.breadth,
            width = authoring.width,
            whiteTileEntity = GetEntity(authoring.whiteTile),
            grayTileEntity = GetEntity(authoring.grayTile),
            wallEntity = GetEntity(authoring.wall),
        });
    }
}