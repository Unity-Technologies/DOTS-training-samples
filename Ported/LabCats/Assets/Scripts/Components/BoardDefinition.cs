using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public struct BoardDefinition : IComponentData
{
    public float CellSize;
    public int NumberColumns;
    public int NumberRows;
    public GridPosition GoalPlayer1;
    public GridPosition GoalPlayer2;
    public GridPosition GoalPlayer3;
    public GridPosition GoalPlayer4;
}
