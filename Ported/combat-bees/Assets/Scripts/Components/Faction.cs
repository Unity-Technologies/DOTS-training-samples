using Unity.Entities;
using UnityEngine;

public enum Factions
{
    None,
    Team1,
    Team2,
    NumFactions
}

public struct Faction : ISharedComponentData
{
    public int Value;
    public Color Color;
}