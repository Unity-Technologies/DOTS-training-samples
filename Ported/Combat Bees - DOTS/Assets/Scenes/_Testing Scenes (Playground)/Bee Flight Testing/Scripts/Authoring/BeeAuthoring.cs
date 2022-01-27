using Unity.Entities;
using UnityGameObject = UnityEngine.GameObject;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityRange = UnityEngine.RangeAttribute;
using Random = Unity.Mathematics.Random;

namespace CombatBees.Testing.BeeFlight
{
    public class BeeAuthoring : UnityMonoBehaviour, IConvertGameObjectToEntity
    {
        [UnityRange(0.0f, 100f)] public float BeeChaseForce = 50f;
        [UnityRange(0.0f, 1f)] public float BeeDamping = 0.1f;
        public float TargetWithinReach = 0.1f;
        public float TargetWithinAttackReach = 5f;
        public float attackForce = 5;
        public float RotationStiffness = 5f;
        public float FlightJitter = 3f;
        public float TeamAttraction = 5f;
        public bool TeamA = false;
        public UnityGameObject HomeMarker;
        public UnityGameObject bloodparticle;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            // Add Components
            dstManager.AddComponent<BeeMovement>(entity);
            dstManager.AddComponent<BeeTargets>(entity);
            dstManager.AddComponent<IsHoldingResource>(entity);
            dstManager.AddComponent<HeldResource>(entity);
            dstManager.AddComponent<Bee>(entity);
            dstManager.AddComponent<BloodSpawner>(entity);

            // Set the Data of Components
            dstManager.AddComponentData(entity, new Bee()
            {
                TeamA = TeamA,
                dead = false,
            });
            
            dstManager.AddComponentData(entity, new BeeMovement
            {
                ChaseForce = BeeChaseForce,
                Damping = BeeDamping,
                RotationStiffness = RotationStiffness,
                FlightJitter = FlightJitter,
                TeamAttraction = TeamAttraction,
            });

            Random new_random = new Random((uint) (entity.Index + 1));

            dstManager.AddComponentData(entity, new BeeTargets
            {
                ResourceTarget = Entity.Null,
                TargetReach = TargetWithinReach,
                HomePosition = HomeMarker.transform.position,
                random = new_random, // +1 because seed can't be 0
                
                AttackTarget = Entity.Null,
                Aggression = new_random.NextFloat()*1000,
                isAttacking = false,
                AttackReach = TargetWithinAttackReach,
                attackForce = attackForce
            });

            dstManager.AddComponentData(entity, new IsHoldingResource
            {
                Value = false,
                JustPickedUp = false,
                ReachedHome = false
            });
            
            dstManager.AddComponentData(entity, new HeldResource()
            {
                Value = Entity.Null
            });
            
            dstManager.AddComponentData(entity, new BloodSpawner()
            {
                  bloodEntity = conversionSystem.GetPrimaryEntity(bloodparticle),
                  random = new Random((uint) (entity.Index + 4)),
                  amountParticles = 10,
                  steps = 50,
                  animationStarted = false,
            });
        }
    }
}