namespace Components
{
    using Unity.Entities;


    class BucketAuth : UnityEngine.MonoBehaviour
    {
        public float Capacity;
        
        class BucketAuthBaker : Baker<BucketAuth>
        {
            public override void Bake(BucketAuth authoring)
            {
                AddComponent(new Bucket
                {
                    Capacity = authoring.Capacity 
                });
            }
        }

    }

    //Capacity used to subtract from water source(Maybe not needed if the capacity is predefined)
    struct Bucket : IComponentData
    {
        
        public float Capacity;
    }
}
