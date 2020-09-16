using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Physics.Authoring;
using Unity.Physics;

public class BuildingCreate : SystemBase
{
    protected override void OnUpdate()
    {
        BuildingCreationData buildingData = GetSingleton<BuildingCreationData>();

        Entities.WithStructuralChanges().ForEach((Entity entity, in SpawnData spawnData) => {

            Entity previousEntity = Entity.Null;

            for(int i = 0; i < spawnData.height; i++)
            {
                float3 position = spawnData.position;

                RenderBounds bounds = EntityManager.GetComponentData<RenderBounds>(buildingData.prefab);
                float prefabHeight = bounds.Value.Size.y;

                position.y += 0.5f + i * prefabHeight;

                var instance = EntityManager.Instantiate(buildingData.prefab);
                EntityManager.SetComponentData(instance, new Translation
                {
                    Value = position
                });
                
                if(i > 0)
                {
                    PhysicsJoint joint = PhysicsJoint.CreateBallAndSocket(new float3(0.0f, 0.5f, 0.0f), new float3(0.0f, -0.5f, 0.0f));

                    Entity jointEntity = EntityManager.CreateEntity(new ComponentType[] { typeof(PhysicsConstrainedBodyPair), typeof(PhysicsJoint), typeof(JointBreakDistance) });

                    EntityManager.SetComponentData(jointEntity, new PhysicsConstrainedBodyPair(previousEntity, instance, true));
                    EntityManager.SetComponentData(jointEntity, joint);
                    EntityManager.SetComponentData(jointEntity, new JointBreakDistance { Value = buildingData.jointBreakDistance });
                }

                previousEntity = instance;

            }
            EntityManager.DestroyEntity(entity);
        }).Run();
        
    }
}
//for (int i = 0; i < 35; i++)
//{
//	int height = Random.Range(4, 12);
//	Vector3 pos = new Vector3(Random.Range(-45f, 45f), 0f, Random.Range(-45f, 45f));
//	float spacing = 2f;
//	for (int j = 0; j < height; j++)
//	{
//		Point point = new Point();
//		point.x = pos.x + spacing;
//		point.y = j * spacing;
//		point.z = pos.z - spacing;
//		point.oldX = point.x;
//		point.oldY = point.y;
//		point.oldZ = point.z;
//		if (j == 0)
//		{
//			point.anchor = true;
//		}
//		pointsList.Add(point);
//		point = new Point();
//		point.x = pos.x - spacing;
//		point.y = j * spacing;
//		point.z = pos.z - spacing;
//		point.oldX = point.x;
//		point.oldY = point.y;
//		point.oldZ = point.z;
//		if (j == 0)
//		{
//			point.anchor = true;
//		}
//		pointsList.Add(point);
//		point = new Point();
//		point.x = pos.x + 0f;
//		point.y = j * spacing;
//		point.z = pos.z + spacing;
//		point.oldX = point.x;
//		point.oldY = point.y;
//		point.oldZ = point.z;
//		if (j == 0)
//		{
//			point.anchor = true;
//		}
//		pointsList.Add(point);
//	}
//}