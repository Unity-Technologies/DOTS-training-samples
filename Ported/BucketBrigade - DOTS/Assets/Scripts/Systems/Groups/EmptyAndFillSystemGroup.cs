using System.Collections;
using System.Collections.Generic;
using Systems;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(InitializeChainIndecies))]
public class EmptyAndFillSystemGroup : ComponentSystemGroup
{
}
