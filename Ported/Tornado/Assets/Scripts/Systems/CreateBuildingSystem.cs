using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class CreateBuildingSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<BarSpawner>();
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        Random random = new Random(1234);

        // buildings
        for (int i = 0; i < 45; i++)
        {
            var newBuildingEntity = ecb.CreateEntity();
            var newBuildingConstructionData = new BuildingConstructionData();
            newBuildingConstructionData.height = random.NextInt(4, 12);
            newBuildingConstructionData.position = new float3(random.NextFloat(-45f, 45f), 0f, random.NextFloat(-45f, 45f));
            newBuildingConstructionData.spacing = 2f;

            ecb.AddComponent(newBuildingEntity, newBuildingConstructionData);
            ecb.AddComponent<Building>(newBuildingEntity);
        }

        ecb.Playback(EntityManager);

        ecb.Dispose();

        ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities.ForEach((Entity entity, in BuildingConstructionData buildingConstructionData) =>
        {
            var height = buildingConstructionData.height;
            var pos = buildingConstructionData.position;
            var spacing = buildingConstructionData.spacing;

            float3 pointPosition = new float3(0);

            var nodesList = ecb.AddBuffer<NodeBuildingData>(entity);

            for (int j = 0; j < height; j++)
            {
                var nodeData = new NodeBuildingData();

                var point = new Node();
                var trans = new Translation();
                var nodeEntity = ecb.CreateEntity();

                trans.Value.x = pos.x + spacing;
                trans.Value.y = j * spacing;
                trans.Value.z = pos.z - spacing;
                point.oldPosition = trans.Value;
                if (j == 0)
                {
                    point.anchor = true;
                }

                ecb.AddComponent(nodeEntity, point);
                ecb.AddComponent(nodeEntity, trans);
                nodeData.nodeEntity = nodeEntity;
                nodeData.node = point;
                nodeData.translation = trans;
                nodesList.Add(nodeData);

                nodeData = new NodeBuildingData();
                nodeEntity = ecb.CreateEntity();
                trans.Value.x = pos.x - spacing;
                trans.Value.y = j * spacing;
                trans.Value.z = pos.z - spacing;
                point.oldPosition = trans.Value;
                if (j == 0)
                {
                    point.anchor = true;
                }

                ecb.AddComponent(nodeEntity, point);
                ecb.AddComponent(nodeEntity, trans);
                nodeData.nodeEntity = nodeEntity;
                nodeData.node = point;
                nodeData.translation = trans;
                nodesList.Add(nodeData);

                nodeData = new NodeBuildingData();
                nodeEntity = ecb.CreateEntity();
                trans.Value.x = pos.x;
                trans.Value.y = j * spacing;
                trans.Value.z = pos.z + spacing;
                point.oldPosition = trans.Value;
                if (j == 0)
                {
                    point.anchor = true;
                }

                ecb.AddComponent(nodeEntity, point);
                ecb.AddComponent(nodeEntity, trans);
                nodeData.nodeEntity = nodeEntity;
                nodeData.node = point;
                nodeData.translation = trans;
                nodesList.Add(nodeData);
            }

            // ground details
            for (int i = 0; i < 0; i++)
            {
                float3 pos2 = new float3(random.NextFloat(-55f, 55f), 0f, random.NextFloat(-55f, 55f)) + pos;

                var nodeData = new NodeBuildingData();
                var point = new Node();
                var trans = new Translation();
                var nodeEntity = ecb.CreateEntity();

                trans.Value.x = pos2.x + random.NextFloat(-.2f, -.1f);
                trans.Value.y = pos2.y + random.NextFloat(0f, 3f);
                trans.Value.z = pos2.z + random.NextFloat(.1f, .2f);
                point.oldPosition = trans.Value;

                ecb.AddComponent(nodeEntity, point);
                ecb.AddComponent(nodeEntity, trans);
                nodeData.nodeEntity = nodeEntity;
                nodeData.node = point;
                nodeData.translation = trans;
                nodesList.Add(nodeData);

                nodeData = new NodeBuildingData();
                nodeEntity = ecb.CreateEntity();

                trans.Value.x = pos2.x + random.NextFloat(.2f, .1f);
                trans.Value.y = pos2.y + random.NextFloat(0f, 3f);
                trans.Value.z = pos2.z + random.NextFloat(-.1f, -.2f);
                point.oldPosition = trans.Value;
                if (random.NextFloat(0f, 1f) < .1f)
                {
                    point.anchor = true;
                }

                ecb.AddComponent(nodeEntity, point);
                ecb.AddComponent(nodeEntity, trans);
                nodeData.nodeEntity = nodeEntity;
                nodeData.node = point;
                nodeData.translation = trans;
                nodesList.Add(nodeData);
            }

            var constraintsList = ecb.AddBuffer<Constraint>(entity);

            for (int i = 0; i < nodesList.Length; i++)
            {
                for (int j = i + 1; j < nodesList.Length; j++)
                {
                    var bar = new Constraint();

                    var nodeA = nodesList[i].node;
                    var nodeB = nodesList[j].node;
                    var positionA = nodesList[i].translation.Value;
                    var positionB = nodesList[j].translation.Value;

                    bar.AssignPoints(nodesList[i].nodeEntity, nodesList[j].nodeEntity, positionA, positionB);
                    if (bar.distance < 5f && bar.distance > .2f)
                    {
                        nodeA.neighborCount++;
                        ecb.SetComponent(nodesList[i].nodeEntity, nodeA);

                        nodeB.neighborCount++;
                        ecb.SetComponent(nodesList[j].nodeEntity, nodeB);

                        constraintsList.Add(bar);
                    }
                }
            }

            ecb.RemoveComponent<BuildingConstructionData>(entity);
        }).Run();

        ecb.Playback(EntityManager);

        ecb.Dispose();


        ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities.WithAll<Building>().ForEach((in DynamicBuffer<NodeBuildingData> nodesList) =>
        {
            for (int i = 0; i < nodesList.Length; i++)
            {
                var node = GetComponent<Node>(nodesList[i].nodeEntity);

                if (node.neighborCount <= 0)
                {
                    ecb.DestroyEntity(nodesList[i].nodeEntity);
                }
            }
        }).Run();

        ecb.Playback(EntityManager);

        ecb.Dispose();

        System.GC.Collect();

        Enabled = false;
    }
}