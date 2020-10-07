using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct SpawnerInfo : IComponentData
{
    public float catMax;
    public float catCount;
    public float mouseCount;
    public float catFrequency;
    public float mouseFrequency;
    public float catTimer;
    public float mouseTimer;
    public float catSpeed;
    public float mouseSpeed;
    public int maxMiceCount;
    public int minCatSpeed;
    public int maxCatSpeed;
    public int minMouseSpeed;
    public int maxMouseSpeed;
    public Entity mousePrefab;
    public Entity catPrefab;
}
