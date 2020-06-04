using Unity.Entities;

public enum GameMode
{
    Intro,
    GamePlay,
    GameOver
}

public struct GameModeComponent : IComponentData
{
    public GameMode Value;
}