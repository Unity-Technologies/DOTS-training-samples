using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Mathematics;

public struct Platform : IBufferElementData
{
    public float startPoint;
    public float endPoint;
    public float3 startWorldPosition;
    public float3 endWorldPosition;
}

