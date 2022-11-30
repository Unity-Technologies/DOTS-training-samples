using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

struct HasResource : IComponentData
{
    public bool Value;
    public bool Trigger;
}
