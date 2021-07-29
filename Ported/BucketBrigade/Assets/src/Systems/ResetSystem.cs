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
                UnityEngine.SceneManagement.SceneManager.CreateScene("__temp");

                Dependency.Complete();
                World.DisposeAllWorlds();
                ScriptBehaviourUpdateOrder.RemoveWorldFromCurrentPlayerLoop(World.DefaultGameObjectInjectionWorld);

                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
                DefaultWorldInitialization.Initialize("Default World", false);
            }              
        }
    }
}