namespace Components
{
    using Unity.Entities;


    class FillingAuth : UnityEngine.MonoBehaviour
    {
        class FillingBaker : Baker<FillingAuth>
        {
            public override void Bake(FillingAuth authoring)
            {
                AddComponent<Filling>();
            }
        }
   
    }


    struct Filling : IComponentData, IEnableableComponent
    {
    }

}