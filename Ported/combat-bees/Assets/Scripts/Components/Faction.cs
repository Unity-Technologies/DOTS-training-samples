using Unity.Entities;

public enum Factions
{
    None,
    Team1,
    Team2
}

public struct Faction : IComponentData
{
    public int Value;
}