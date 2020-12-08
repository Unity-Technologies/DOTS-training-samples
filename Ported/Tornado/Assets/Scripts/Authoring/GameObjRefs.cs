using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public class GameObjRefs : IComponentData
{
    public Camera Camera;
}
