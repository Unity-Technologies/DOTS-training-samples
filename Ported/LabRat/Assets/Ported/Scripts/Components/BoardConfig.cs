using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct BoardConfig : IComponentData
{
    public int height;
    public int width;
    
    public Entity whiteTileEntity;
    public Entity grayTileEntity;
    public Entity wallEntity;
}
