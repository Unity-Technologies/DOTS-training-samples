using Unity.Entities;
using UnityEngine;

namespace AntPheromones_ECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class AdjustTimescaleSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Time.timeScale = 1f;
            } 
            else if (Input.GetKeyDown(KeyCode.Alpha2)) 
            {
                Time.timeScale = 2f;
            } 
            else if (Input.GetKeyDown(KeyCode.Alpha3)) 
            {
                Time.timeScale = 3f;
            } 
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                Time.timeScale = 4f;
            } 
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                Time.timeScale = 5f;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                Time.timeScale = 6f;
            } 
            else if (Input.GetKeyDown(KeyCode.Alpha7)) 
            {
                Time.timeScale = 7f;
            } 
            else if (Input.GetKeyDown(KeyCode.Alpha8)) 
            {
                Time.timeScale = 8f;
            } 
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                Time.timeScale = 9f;
            }
        }
    }
}