using Unity.Entities;

public enum Factions
{
    Team1,
    Team2
}

public struct Faction : IComponentData
{
    public int Value;
}