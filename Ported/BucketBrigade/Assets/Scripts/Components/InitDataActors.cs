using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct InitDataActors : IComponentData
{
    public int BrigadeCount;
    public Entity ActorPrefab;
    public int ActorCountPerBrigade;
}
