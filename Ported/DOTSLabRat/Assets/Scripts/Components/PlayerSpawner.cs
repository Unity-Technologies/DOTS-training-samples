using Unity.Entities;

[GenerateAuthoringComponent]
public struct PlayerSpawner : IComponentData
{
    public Entity playerPrefab;
    public Entity arrowPrefab;
    public int maxArrows;
    public UnityEngine.Color player1Color;
    public UnityEngine.Color player2Color;
    public UnityEngine.Color player3Color;
    public UnityEngine.Color player4Color;
}