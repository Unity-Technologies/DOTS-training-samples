using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace Dots
{
    public struct BeamGroup : ISharedComponentData
    {
        public int groupId;
    }
}

