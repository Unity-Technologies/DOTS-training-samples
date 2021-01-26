using System.Collections;
using System.Collections.Generic;
using Unity.Entities;

public struct InitializationSpawner : IComponentData
{
    public int NumberOfBees;
    public Entity BeePrefab;
}
