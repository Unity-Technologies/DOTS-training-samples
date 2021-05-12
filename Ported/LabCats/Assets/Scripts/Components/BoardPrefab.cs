using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public struct BoardPrefab : IComponentData
{
    public Entity LightCellPrefab;
    public Entity DarkCellPrefab;
    public Entity CursorPrefab;
    public Entity PlayerCursorPrefab;
    public Entity ArrowPrefab;
    public Entity MousePrefab;
    public Entity CatPrefab;
    public Entity WallPrefab;
    public Entity GoalPrefab;
}
