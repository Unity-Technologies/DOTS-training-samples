using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace Dots
{
    public struct AnchorList : IComponentData
    {
        public Entity p1;
        public Entity p2;
    }
}

