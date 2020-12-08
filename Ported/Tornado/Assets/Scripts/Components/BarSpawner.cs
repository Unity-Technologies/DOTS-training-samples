using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public class BarSpawner : IComponentData
{
    public Entity barPrefab;
}
