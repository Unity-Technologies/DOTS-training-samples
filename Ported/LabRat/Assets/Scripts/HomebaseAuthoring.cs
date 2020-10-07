using Unity.Entities;

[GenerateAuthoringComponent]
public struct HomeBase : IComponentData
{
    // this may not be the best place for these but its a good holding place for now.
    public byte playerIndex;
    public int playerScore;
}