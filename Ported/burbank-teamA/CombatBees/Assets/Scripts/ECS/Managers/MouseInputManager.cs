using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class MouseInputManager : MonoBehaviour
{
   
    public GameObject[] spawningTeamBees;
    public GameObject spawningResources;
    public GameObject sprinkleParticles;
    
    Transform marker;

    Entity resources, team1, team2, sprikles;

    private void Start()
    {
        resources = GameObjectConversionUtility.ConvertGameObjectHierarchy(spawningResources, World.Active);
        team1 = GameObjectConversionUtility.ConvertGameObjectHierarchy(spawningTeamBees[0], World.Active);
        team2 = GameObjectConversionUtility.ConvertGameObjectHierarchy(spawningTeamBees[1], World.Active);
        sprikles = GameObjectConversionUtility.ConvertGameObjectHierarchy(sprinkleParticles, World.Active);

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            Transform mouseRay = FindObjectOfType<MouseRaycaster>().marker;
            
            //TODO: change that in the future
            EntityManager entityManager = World.Active.EntityManager;
            var entity = resources;
            if (mouseRay.position.x < -40)
            {
                entity = team1;
                }

            if (mouseRay.position.x > 40)
            {
                entity = team2;
            }

            var instance = entityManager.Instantiate(entity);
            entityManager.SetComponentData(instance, new Translation() { Value = mouseRay.position });

            if (Mathf.Abs(mouseRay.position.x) > 40)
            {
                entity = sprikles;
                instance = entityManager.Instantiate(entity);
                entityManager.SetComponentData(instance, new Translation() { Value = mouseRay.position });
            }
        }
    }

}
