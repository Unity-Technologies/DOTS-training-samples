using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Miscellaneous.StateChangeEnableable
{
#if COMPILE_THIS
    public partial struct CubeSpawnSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Config>();
            state.RequireForUpdate<Execute.StateChangeEnableable>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            var config = SystemAPI.GetSingleton<Config>();
            state.EntityManager.Instantiate(config.Prefab, (int)(config.Size * config.Size), Allocator.Temp);
            /*
            var center = (config.Size - 1) / 2f;
            int i = 0;
            foreach (var (trans, spinnerEnabled) in SystemAPI.Query<RefRW<LocalTransform>, EnabledRefRW<Spinner>>().WithAll<Cube>())
            {
                spinnerEnabled.ValueRW = false;
                trans.ValueRW.Scale = 1;
                trans.ValueRW.Position.x = (i % config.Size - center) * 1.5f;
                trans.ValueRW.Position.z = (i / config.Size - center) * 1.5f;
                i++;
            }
            */
            var pos = config.Origin;
            int i = 0;
            foreach (var (trans, burn) in SystemAPI.Query<RefRW<LocalTransform>, EnabledRefRW<Burner>>().WithAll<Cube>()) {
                //spinnerEnabled.ValueRW = false;
                trans.ValueRW.Scale = 1;
                //burn.ValueRW = 0.0f;
                trans.ValueRW.Position.x = pos.x + (i % config.Size) * 1.05f;
                trans.ValueRW.Position.y = pos.y - 2.0f;
                trans.ValueRW.Position.z = pos.z + (i / config.Size) * 1.05f;
                i++;
            }
        }
    }
#endif
}
