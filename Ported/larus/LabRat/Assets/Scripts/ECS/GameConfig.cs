using Unity.Entities;
using UnityEngine;

namespace ECSExamples
{
    // singleton entity
    public struct GameConfigComponent : IComponentData
    {
        public float DIE_AT_DEPTH; // how far to fall before dying
        public float RotationSpeed;
        public float FallingSpeed;
        public Interval Speed;
        public bool DiminishesArrows;
    }

    public class GameConfig : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.SetName(entity, "GameConfig");
            dstManager.AddComponentData(entity, new GameConfigComponent
            {
                DiminishesArrows = false,
                FallingSpeed = 2f,
                RotationSpeed = 0.5f,
                Speed = new Interval(0.9f, 1.2f),
                DIE_AT_DEPTH = -5.0f
            });
        }
    }
}