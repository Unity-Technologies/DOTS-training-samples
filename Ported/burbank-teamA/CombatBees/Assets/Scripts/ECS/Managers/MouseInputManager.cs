using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class MouseInputManager : MonoBehaviour
{
   
    public GameObject[] spawningTeamBees;
    public GameObject spawningResources;
    
    Transform marker;


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            Transform mouseRay = FindObjectOfType<MouseRaycaster>().marker;
            
            //TODO: change that in the future
            EntityManager entityManager = World.Active.EntityManager;
            Entity entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(spawningResources, World.Active);

            if (mouseRay.position.x < -40)
            {
                entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(spawningTeamBees[0], World.Active);
            }

            if(mouseRay.position.x > 40)
            {
                entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(spawningTeamBees[1], World.Active);
            }

            var instance = entityManager.Instantiate(entity);
            entityManager.SetComponentData(instance, new Translation() { Value = mouseRay.position });
        }
    }

}
