using Unity.Entities;
using Unity.Rendering;

// Probably not needed 
class OnFireAuth : UnityEngine.MonoBehaviour
{
    class OnFireBaker : Baker<OnFireAuth>
    {
        public override void Bake(OnFireAuth authoring)
        {
            AddComponent<OnFire>();

            AddComponent<URPMaterialPropertyBaseColor>();
        }
    }
   
}
