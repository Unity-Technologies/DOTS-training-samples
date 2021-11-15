using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace Dots
{
    public struct Length : IComponentData
    {
        public float value;
    }
}