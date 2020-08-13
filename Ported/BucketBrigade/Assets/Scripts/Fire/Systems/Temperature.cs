using Unity.Entities;

// Current temperature on a cell  
[GenerateAuthoringComponent]
public struct Temperature : IComponentData
{
    public float Value;
    public int FireGridIndex;
}

// Added temperature each frame on a single cell  
public struct AddedTemperature : IComponentData
{
    public float Value;
}
