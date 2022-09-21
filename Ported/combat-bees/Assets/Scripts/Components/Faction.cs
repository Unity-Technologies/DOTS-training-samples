using Unity.Entities;

public enum Factions
{
    Food,
    Team1,
    Team2,
    NumFactions
}

public struct Faction : ISharedComponentData
{
    public int Value;
}