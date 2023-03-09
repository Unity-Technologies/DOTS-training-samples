using Unity.Entities;
using UnityEngine;

public class HomeAuthoring : MonoBehaviour
{
    public GameObject homePrefab;
    public class Baker : Baker<HomeAuthoring>
    {
        public override void Bake(HomeAuthoring authoring)
        {
            AddComponent(new Home
            {
                homePrefab = GetEntity(authoring.homePrefab)
            });
        }
    }
}

public struct Home : IComponentData
{
    public Entity homePrefab;
}