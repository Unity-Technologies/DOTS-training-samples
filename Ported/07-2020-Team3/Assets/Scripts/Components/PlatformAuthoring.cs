using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct Platform : IComponentData
{
    public Entity HeadStairsBottom;
    public Entity HeadStairsTop;
    public Entity FootStairsBottom;
    public Entity FootStairsTop;
    public Entity PlatformCenter;
    public Entity Queue0;
    public Entity Queue1;
    public Entity Queue2;
    public Entity Queue3;
    public Entity Queue4;
    public Entity NextPlatform;
}
