using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class WallsAuthoring : MonoBehaviour
{
    public class Baker : Baker<WallsAuthoring>
    {
        public override void Bake(WallsAuthoring authoring)
        {
            AddComponent<Walls>();
        }
    }
}