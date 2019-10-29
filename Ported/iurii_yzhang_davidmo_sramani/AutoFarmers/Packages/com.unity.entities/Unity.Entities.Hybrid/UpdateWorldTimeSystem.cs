using Unity.Core;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Unity.Entities
{
    [Preserve]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class UpdateWorldTimeSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            World.SetTime(new TimeData(
                elapsedTime: UnityEngine.Time.time,
                deltaTime: UnityEngine.Time.deltaTime
            ));
        }
    }
}
