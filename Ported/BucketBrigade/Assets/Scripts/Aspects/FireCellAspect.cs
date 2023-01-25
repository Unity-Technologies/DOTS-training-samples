

using Unity.Entities;

readonly partial struct FireCellAspect : IAspect
{
    // An Entity field in an aspect provides access to the entity itself.
    // This is required for registering commands in an EntityCommandBuffer for example.
    public readonly Entity Self;

    public readonly RefRO<CellInfo> CellInfo;
    public readonly RefRW<DisplayHeight> DisplayHeight;


}
