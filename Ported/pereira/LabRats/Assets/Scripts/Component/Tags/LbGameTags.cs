using Unity.Entities;

/// <summary>
/// Used to show the intro cinematic
/// </summary>
public struct LbGameStart : IComponentData
{
}

/// <summary>
/// Wait for spawning all dynamic entities (cat/rat spawners and player curosrs)
/// </summary>
public struct LbGameWaitForSpawn : IComponentData
{
    public float Value;
}

public struct LbGameSpawnAll : IComponentData
{
}

/// <summary>
/// Gameplay timer
/// </summary>
public struct LbGameTimer : IComponentData
{
    public float Value;
}

/// <summary>
/// Timer to restart the game
/// </summary>
public struct LbGameRestarter : IComponentData
{
    public float Value;
}