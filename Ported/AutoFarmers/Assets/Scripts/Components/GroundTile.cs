using Unity.Collections;
using Unity.Entities;

public enum GroundTileState : byte
{
    Open=0,
    Unpassable,
    Tilled,
    Planted,
    Silo,
    Claimed, // Tile is in-processe of being modified (to another state) so consider it inactive.
}

public struct GroundTile : IBufferElementData
{
    public GroundTileState tileState;

    public Entity rockEntityByTile;
    public Entity plantEntityByTile;
    public Entity siloEntityByTile;
}
