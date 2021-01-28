using Unity.Entities;

[GenerateAuthoringComponent]
public struct GameTime : IComponentData
{
    //public int[] testInts;
    public int CurrentStep;
    public float DeltaTime;
}
