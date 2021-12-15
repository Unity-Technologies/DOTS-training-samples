using Unity.Entities;
using UnityEngine;
using UnityCamera = UnityEngine.Camera;

namespace Combatbees.Testing.Mahmoud
{
    [GenerateAuthoringComponent]
    public class GameObjectRef : IComponentData
    {
        public UnityCamera Camera;
        public Material MarkerMaterial;
    }

}