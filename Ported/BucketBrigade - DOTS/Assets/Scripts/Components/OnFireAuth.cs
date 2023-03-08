using Unity.Entities;


class OnFireAuth : UnityEngine.MonoBehaviour
{
    class OnFireBaker : Baker<OnFireAuth>
    {
        public override void Bake(OnFireAuth authoring)
        {
            AddComponent<OnFire>();
        }
    }
   
}


struct OnFire : IComponentData, IEnableableComponent
{
}