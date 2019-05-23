using System;
using Unity.Entities;

namespace JumpTheGun
{
    // If an entity ever exists with this tag, the game is over and should be restarted during the next update loop.
    [Serializable]
    public struct NewGameTag : IComponentData {} 
    [UnityEngine.DisallowMultipleComponent]
    public class NewGameProxy : ComponentDataProxy<NewGameTag>
    {
    }
}
