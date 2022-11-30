using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

class AgentAuthoring : MonoBehaviour
{
    public Transform InitialTarget;
}
 
class AgentBaker : Baker<AgentAuthoring>
{
    public override void Bake(AgentAuthoring authoring)
    {
        AddComponent(new Agent()
        {
            CurrentWaypoint = GetEntity(authoring.InitialTarget)
        });
    }
}