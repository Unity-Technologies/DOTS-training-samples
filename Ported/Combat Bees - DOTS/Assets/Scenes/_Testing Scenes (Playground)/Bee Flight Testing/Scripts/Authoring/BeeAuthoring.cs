using Unity.Entities;
using UnityEngine;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityRange = UnityEngine.RangeAttribute;

namespace CombatBees.Testing.BeeFlight
{
    public class BeeAuthoring : UnityMonoBehaviour, IConvertGameObjectToEntity
    {
        [UnityRange(0.0f, 15.0f)] public float BeeSpeed = 5.0f;
        [UnityRange(0.0f, 100f)] public float BeeChaseForce = 50f;
        [UnityRange(0.0f, 1f)] public float BeeDamping = 0.1f;
        public float TargetWithinReach = 0.1f;

        public GameObject ResourceTargetObject;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponent<BeeMovement>(entity);
            dstManager.AddComponent<BeeTargets>(entity);
            dstManager.AddComponent<Bee>(entity);

            dstManager.AddComponentData(entity, new BeeMovement
            {
                Speed = BeeSpeed,
                ChaseForce = BeeChaseForce,
                Damping = BeeDamping
            });

            dstManager.AddComponentData(entity, new BeeTargets
            {
                ResourceTarget = Entity.Null,
                TargetReach = TargetWithinReach
                // LeftTarget = LeftTargetObject.position,
                // RightTarget = RightTargetObject.position,
            });
        }
    }
}