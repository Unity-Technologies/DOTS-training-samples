using Unity.Entities;
using UnityEngine;

namespace Systems
{
    public partial class InputSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                World.GetExistingSystem<GenerationSystem>().Reset();
            }
        }
    }
}