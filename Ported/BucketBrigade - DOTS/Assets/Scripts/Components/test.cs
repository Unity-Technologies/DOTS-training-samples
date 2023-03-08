using Unity.Entities;


class test : UnityEngine.MonoBehaviour
{
    public float baseHealth;
    class BaseBaker : Baker<test>
    {
        public override void Bake(test authoring)
        {
            AddComponent(new Base
            {
                Value = authoring.baseHealth,
            });
        }
    }
   
}


struct Base : IComponentData
{
    public float Value; 
}
