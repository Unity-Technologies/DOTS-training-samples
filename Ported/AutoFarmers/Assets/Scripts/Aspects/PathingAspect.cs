using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

readonly partial struct PathingAspect : IAspect<PathingAspect>
{
    public readonly DynamicBuffer<Waypoint> Waypoints;
    public readonly MovementAspect Movement;
}
