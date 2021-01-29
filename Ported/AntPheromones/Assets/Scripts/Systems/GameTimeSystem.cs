using Unity.Entities;
using UnityEngine;

public class GameTimeSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        
        Entities
            .WithoutBurst()
            .ForEach((Entity entity, ref GameTime gameTime) =>
            {
                if (Input.GetKeyDown(KeyCode.Alpha1)) { gameTime.CurrentStep = 1; }
                if (Input.GetKeyDown(KeyCode.Alpha2)) { gameTime.CurrentStep = 5; }
                if (Input.GetKeyDown(KeyCode.Alpha3)) { gameTime.CurrentStep = 10; }
                if (Input.GetKeyDown(KeyCode.Alpha4)) { gameTime.CurrentStep = 50; }
                if (Input.GetKeyDown(KeyCode.Alpha5)) { gameTime.CurrentStep = 100; }
                if (Input.GetKeyDown(KeyCode.Alpha6)) { gameTime.CurrentStep = 500; }
                if (Input.GetKeyDown(KeyCode.Alpha7)) { gameTime.CurrentStep = 1000; }
                if (Input.GetKeyDown(KeyCode.Alpha8)) { gameTime.CurrentStep = 5000; }
                if (Input.GetKeyDown(KeyCode.Alpha9)) { gameTime.CurrentStep = 10000; }
                
                gameTime.DeltaTime =  deltaTime * gameTime.CurrentStep;
               // Shader.SetGlobalFloat("_OffsetSpeedMultiplier", gameTime.CurrentStep);
            }).Run();
    }
}
