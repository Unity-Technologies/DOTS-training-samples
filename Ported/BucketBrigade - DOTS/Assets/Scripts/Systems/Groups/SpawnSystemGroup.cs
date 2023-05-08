using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(WaterSpawningSystem))]
[UpdateAfter(typeof(BucketSpawningSystem))]
public class SpawnSystemGroup : ComponentSystemGroup
{
}
