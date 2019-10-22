using Unity.Entities;
using UnityEngine;

namespace ECSExamples
{
    // singleton entity
    public struct GameConfigComponent : IComponentData
    {
        public float DieAtDepth;
        public float RotationSpeed;
        public float FallingSpeed;
        public Interval EaterSpeed;
        public Interval EatenSpeed;
        public bool DiminishesArrows;
    }

    public class GameConfig : MonoBehaviour, IConvertGameObjectToEntity
    {
        public float DieAtDepth = -5.0f;
        public float RotationSpeed = 0.5f;
        public float FallingSpeed = 2f;
        public Interval EaterSpeed = new Interval(0.9f, 1.2f);
        public Interval EatenSpeed = new Interval(0.9f, 1.2f);
        public bool DiminishesArrows = false;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.SetName(entity, "GameConfig");
            dstManager.AddComponentData(entity, new GameConfigComponent
            {
                DiminishesArrows = DiminishesArrows,
                FallingSpeed = FallingSpeed,
                RotationSpeed = RotationSpeed,
                EaterSpeed = EaterSpeed,
                EatenSpeed = EatenSpeed,
                DieAtDepth = DieAtDepth
            });
        }
    }
}