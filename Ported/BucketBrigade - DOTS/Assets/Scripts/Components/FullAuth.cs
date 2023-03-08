namespace Components
{
    using Unity.Entities;


    class FullAuth : UnityEngine.MonoBehaviour
    {
        class FullBaker : Baker<FullAuth>
        {
            public override void Bake(FullAuth authoring)
            {
                AddComponent<Full>();
            }
        }
   
    }


    struct Full : IComponentData, IEnableableComponent
    {
    }

}