namespace Components
{
    using Unity.Entities;


    class BucketAuth : UnityEngine.MonoBehaviour
    {
        public float MaxCapacity;
        
        class BucketAuthBaker : Baker<BucketAuth>
        {
            public override void Bake(BucketAuth authoring)
            {
                AddComponent(new Bucket
                {
                    MaxCapacity = authoring.MaxCapacity,
                    CurrCapacity = 0
                });
                AddComponent<Filling>();
                AddComponent<Empty>();
                AddComponent<Full>();
            }
        }

    }
    
    struct Bucket : IComponentData
    {
        public float MaxCapacity;
        public float CurrCapacity;
    }
    
    struct Filling : IComponentData, IEnableableComponent
    {
    }
    
    struct Empty : IComponentData, IEnableableComponent
    {
    }
    
    struct Full : IComponentData, IEnableableComponent
    {
    }
}
