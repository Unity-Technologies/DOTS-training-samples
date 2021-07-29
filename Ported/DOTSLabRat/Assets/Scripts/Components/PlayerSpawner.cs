using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct PlayerSpawner : IComponentData
{
    public Entity playerPrefab;
    public Entity arrowPrefab;
    public Entity arrowPreviewPrefab;
    public int maxArrows;
    public Vector2 AIArrowPlaceDelayRange;
    public UnityEngine.Color player1Color;
    public UnityEngine.Color player2Color;
    public UnityEngine.Color player3Color;
    public UnityEngine.Color player4Color;
}