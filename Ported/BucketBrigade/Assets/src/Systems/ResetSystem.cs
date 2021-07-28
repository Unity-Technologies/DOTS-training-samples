using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

using UnityInput = UnityEngine.Input;
using UnityKeyCode = UnityEngine.KeyCode;

using src.Components;

namespace src.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class ResetSystem: SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            RequireSingletonForUpdate<TeamData>();
            RequireSingletonForUpdate<FireSimConfigValues>();
            RequireSingletonForUpdate<Temperature>();
        }

        protected override void OnUpdate()
        {
            if (UnityInput.GetKeyDown(UnityKeyCode.Space))
            {
                var configValues = GetSingleton<FireSimConfigValues>();

                // Reset temperatures
                var temperatureEntity = GetSingletonEntity<Temperature>();
                var temperatureBuffer = EntityManager.GetBuffer<Temperature>(temperatureEntity);
                var totalCells = configValues.Columns * configValues.Rows;
                for (int i = 0; i < totalCells; ++i)
                    temperatureBuffer[i] = default;

                // Reset team data
                var teamContainerEntity = GetSingletonEntity<TeamData>();
                var teamDatas = EntityManager.GetBuffer<TeamData>(teamContainerEntity);
                for (int i = 0; i < teamDatas.Length; ++i)
                    teamDatas = default;

                // TODO: reset water source levels
                // TODO: reset workers' positions and tags
                // TODO: unparent all buckets, set random position and reset tags
            }
        }
    }
}