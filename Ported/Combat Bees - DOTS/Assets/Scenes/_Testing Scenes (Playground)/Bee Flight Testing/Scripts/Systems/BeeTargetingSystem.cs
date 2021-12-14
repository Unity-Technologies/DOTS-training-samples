using Unity.Entities;

namespace CombatBees.Testing.BeeFlight
{
    public partial class BeeTargetingSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<SingeltonBeeMovement>();
        }

        protected override void OnUpdate()
        {

        }
    }
}