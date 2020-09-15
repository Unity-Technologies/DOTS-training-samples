using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public class Spawner : IComponentData
{
    public Entity Ant;
    public int NumberOfAnts;
}
