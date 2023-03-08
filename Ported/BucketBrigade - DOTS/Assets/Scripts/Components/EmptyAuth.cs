namespace Components
{
    using Unity.Entities;


    class EmptyAuth : UnityEngine.MonoBehaviour
    {
        class EmptyBaker : Baker<EmptyAuth>
        {
            public override void Bake(EmptyAuth authoring)
            {
                AddComponent<Empty>();
            }
        }
   
    }


    struct Empty : IComponentData, IEnableableComponent
    {
    }

}