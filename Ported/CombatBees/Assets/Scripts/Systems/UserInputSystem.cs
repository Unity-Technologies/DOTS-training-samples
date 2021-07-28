using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

namespace Systems
{
    public class UserInputSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                // spawn something at worldMousePosition

            }
        }
    }
}