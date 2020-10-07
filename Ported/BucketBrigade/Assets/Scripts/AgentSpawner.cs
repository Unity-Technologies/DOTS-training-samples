using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct AgentSpawner : IComponentData
{
    public Entity AgentPrefab;
    
    [Min(0)]
    public int TeamCount;

    [Min(1)]
    public int TeamScoopers;
    
    [Min(1)]
    public int TeamThrowers;

    [Min(1)]
    public int AgentLineLength; // the number of agents in the line. 2 extra agents will be added: one scooper, one thrower.
    
    [Range(0.0f, 1.0f)]
    public float MaxAgentVelocity;
}
