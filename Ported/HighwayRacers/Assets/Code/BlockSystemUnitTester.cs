using UnityEngine;
using Unity.Entities;

public class BlockSystemUnitTester : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        //EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        //var componentTypes = new ComponentType[]
        //{
        //    typeof(LaneAssignment),
        //    typeof(PercentComplete),
        //    typeof(MinimumDistance),
        //    typeof(Speed),
        //    typeof(TargetSpeed)
        //};

        //var entityArchetype = entityManager.CreateArchetype(componentTypes);

        //var entity = entityManager.CreateEntity(entityArchetype);
        //entityManager.SetComponentData(entity, new LaneAssignment { Value = 0 });
        //entityManager.SetComponentData(entity, new PercentComplete { Value = 0 });
        //entityManager.SetComponentData(entity, new MinimumDistance { Value = 0.01f });
        //entityManager.SetComponentData(entity, new Speed { Value = 0.025f });
        //entityManager.SetComponentData(entity, new TargetSpeed { Value = 0.05f });
    }
}
