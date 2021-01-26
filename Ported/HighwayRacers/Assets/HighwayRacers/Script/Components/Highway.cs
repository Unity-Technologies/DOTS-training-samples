using Unity.Entities;
using UnityEngine;

public class Highway : IComponentData
{
    public Entity StraightPiecePrefab;
    public Entity CurvePiecePrefab;
}
