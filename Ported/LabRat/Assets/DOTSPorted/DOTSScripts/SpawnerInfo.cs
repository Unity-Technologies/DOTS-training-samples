using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct SpawnerInfo : IComponentData
{
    public float catMax;
    public float catCount;
    public float catFrequency;
    public float mouseFrequency;
    public float catTimer;
    public float mouseTimer;
    public Entity mousePrefab;
    public Entity catPrefab;
}
