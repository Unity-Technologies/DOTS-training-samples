using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

using UnityInput = UnityEngine.Input;
using UnityKeyCode = UnityEngine.KeyCode;

using src.Components;

namespace src.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public class ResetSystem: SystemBase
    {
        protected override void OnUpdate()
        {
            if (UnityInput.GetKeyDown(UnityKeyCode.Space))
            {
                var sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                EntityManager.CompleteAllJobs(); 
                World.DisposeAllWorlds();
                DefaultWorldInitialization.Initialize("Default World", false);
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);                
            }              
        }
    }
}