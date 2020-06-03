using Unity.Entities;
using Unity.Mathematics;

namespace DefaultNamespace
{
    public class Passer : SystemBase
    {
        protected override void OnCreate()
        {
        }

        protected override void OnUpdate()
        {
            var chainComponent = GetComponentDataFromEntity<Chain>();
            Entities.WithNone<ScooperState, ThrowerState>().ForEach((ref TargetPosition position, in Agent agent)
                =>
            {
                var chain = chainComponent[agent.MyChain];
                
                // direction & perp can be computed once when the chain changes 
                var direction = math.normalize(chain.ChainEndPosition - chain.ChainStartPosition);
                var perpendicular = new float3(direction.z, 0f, -direction.x);

                position.Target = ChainInit.CalculateChainPosition(agent, chain, perpendicular);
            }).WithReadOnly(chainComponent).ScheduleParallel();
        }
    }
}