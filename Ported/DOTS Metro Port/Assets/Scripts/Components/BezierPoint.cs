using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Mathematics;

struct BezierPoint : IBufferElementData
{
    public float3 location;
    public float distanceAlongPath;
}

