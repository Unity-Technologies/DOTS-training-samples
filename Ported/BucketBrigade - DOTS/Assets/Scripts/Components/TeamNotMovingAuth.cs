using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class TeamNotMovingAuth : MonoBehaviour
{
    
    public class TeamMovingBaker : Baker<TeamNotMovingAuth>
    {
        public override void Bake(TeamNotMovingAuth authoring)
        {
            AddComponent<TeamNotMovingTag>();
        }
    }
}

struct TeamNotMovingTag : IComponentData
{
    
}

