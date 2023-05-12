using Unity.Entities;
using UnityEngine;

public partial struct PheromoneMouseDrawSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GlobalSettings>();
        state.RequireForUpdate<PheromoneBufferElement>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        if (Input.GetMouseButton(0))
        {
            Camera camera = Camera.main;
            Vector3 mousePos = camera.ScreenPointToRay(Input.mousePosition).origin;
            GlobalSettings globalSettings = SystemAPI.GetSingleton<GlobalSettings>();

            int index = PheromonesSystem.PheromoneIndex((int)mousePos.x, (int)mousePos.y, globalSettings.MapSizeX);
            var buffer = SystemAPI.GetSingletonBuffer<PheromoneBufferElement>();
            
            if (index > 0 && index < buffer.Length)
            {
                var value = buffer[index].Value;
                value.y = 1;
                buffer[index] = value;
            }
        }
        
    }
}

