using Unity.Entities;
 
[GenerateAuthoringComponent]
public struct CurrentSegment : IComponentData
{
    Entity Value;
}
