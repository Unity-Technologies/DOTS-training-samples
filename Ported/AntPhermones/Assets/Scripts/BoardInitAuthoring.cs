using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct BoardInitAuthoring : IComponentData
{
    public int NumberOfRings;
    
    public float SpaceBetweenTheRings;
    public float FoodRadius;
    
    public float MinRingWidth;
    public float MaxRingWidth;

    public float DualRingThreshold;
    
    public Entity wallPrefab;
}
