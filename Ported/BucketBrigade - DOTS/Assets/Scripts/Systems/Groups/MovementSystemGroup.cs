using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]

[UpdateAfter(typeof(FireHandlingSystem))]
public class MovementSystemGroup : ComponentSystemGroup
{
}
