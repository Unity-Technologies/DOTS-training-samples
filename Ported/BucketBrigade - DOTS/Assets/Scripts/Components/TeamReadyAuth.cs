using Unity.Entities;
using UnityEngine;

public class TeamReadyAuth : MonoBehaviour
{
    public class TeamReadyBaker : Baker<TeamReadyAuth>
    {
        public override void Bake(TeamReadyAuth authoring)
        {
            AddComponent<TeamReadyTag>();
        }
    }
}

public struct TeamReadyTag : IEnableableComponent,IComponentData
{
    
}
