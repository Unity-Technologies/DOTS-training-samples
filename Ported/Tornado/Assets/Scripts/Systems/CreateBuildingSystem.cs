using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public class CreateBuildingSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<BarSpawner>();
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var buildingEntity = EntityManager.CreateEntity(typeof(Building));
        var constraintBuffer = EntityManager.AddBuffer<Constraint>(buildingEntity);

        // buildings
        for (int i = 0; i < 35; i++)
        {
            var newBuildingEntity = ecb.CreateEntity();
            var newBuildingConstructionData = new BuildingConstructionData();
            newBuildingConstructionData.height = Random.Range(4, 12);
            newBuildingConstructionData.position = new float3(Random.Range(-45f, 45f), 0f, Random.Range(-45f, 45f));
            newBuildingConstructionData.spacing = 2f;

            ecb.AddComponent(newBuildingEntity, newBuildingConstructionData);
        }

        ecb.Playback(EntityManager);

        ecb.Dispose();

        ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities.ForEach(( Entity entity, BuildingConstructionData buildingConstructionData) =>
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
               point.oldPosition.x = trans.Value.x;
               point.oldPosition.y = trans.Value.y;
               point.oldPosition.z = trans.Value.z;
               if (j == 0)
               {
                   point.anchor = true;
               }
               ecb.AddComponent(nodeEntity, point);
               ecb.AddComponent(nodeEntity, trans);
               nodeData.nodeEntity = nodeEntity;
               nodeData.node= point;
               nodeData.translation = trans;
               nodesList.Add(nodeData);

               nodeData = new NodeBuildingData();
               nodeEntity = ecb.CreateEntity();
               trans.Value.x = pos.x - spacing;
               trans.Value.y = j * spacing;
               trans.Value.z = pos.z - spacing;
               point.oldPosition.x = trans.Value.x;
               point.oldPosition.y = trans.Value.y;
               point.oldPosition.z = trans.Value.z;
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
               trans.Value.x = pos.x + spacing;
               trans.Value.y = j * spacing;
               trans.Value.z = pos.z + spacing;
               point.oldPosition.x = trans.Value.x;
               point.oldPosition.y = trans.Value.y;
               point.oldPosition.z = trans.Value.z;
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
           for (int i = 0; i < 50; i++)
           {
               float3 pos2 = new float3(Random.Range(-55f, 55f), 0f, Random.Range(-55f, 55f)) + pos;
               
               var nodeData = new NodeBuildingData();
               var point = new Node();
               var trans = new Translation();
               var nodeEntity = ecb.CreateEntity();

               trans.Value.x = pos2.x + Random.Range(-.2f, -.1f);
               trans.Value.y = pos2.y + Random.Range(0f, 3f);
               trans.Value.z = pos2.z + Random.Range(.1f, .2f);
               point.oldPosition.x = trans.Value.x;
               point.oldPosition.y = trans.Value.y;
               point.oldPosition.z = trans.Value.z;

               ecb.AddComponent(nodeEntity, point);
               ecb.AddComponent(nodeEntity, trans);
               nodeData.nodeEntity = nodeEntity;
               nodeData.node = point;
               nodeData.translation = trans;
               nodesList.Add(nodeData);

               nodeData = new NodeBuildingData();
               nodeEntity = ecb.CreateEntity();

               trans.Value.x = pos2.x + Random.Range(.2f, .1f);
               trans.Value.y = pos2.y + Random.Range(0f, 3f);
               trans.Value.z = pos2.z + Random.Range(-.1f, -.2f);
               point.oldPosition.x = trans.Value.x;
               point.oldPosition.y = trans.Value.y;
               point.oldPosition.z = trans.Value.z;
               if (Random.value < .1f)
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
                       ecb.SetComponent(nodesList[j].nodeEntity, nodeA);

                       constraintsList.Add(bar);
                   }
               }
           }

           for (int i = 0; i < nodesList.Length; i++)
           {
               var node = GetComponent<Node>(nodesList[i].nodeEntity);

               if (node.neighborCount <= 0)
               {
                   ecb.DestroyEntity(nodesList[i].nodeEntity);
               }
           }

           ecb.RemoveComponent<BuildingConstructionData>(entity);
       }).Run();

        ecb.Playback(EntityManager);

        

        System.GC.Collect();

        Enabled = false;
    }

    Entity CreatePointEntity( float3 position, bool anchor, int neighborCount)
    {
        var pointEntity = EntityManager.CreateEntity(typeof(Translation), typeof(Node));

        var translation = EntityManager.GetComponentData<Translation>(pointEntity);
        translation.Value = new float3(position);
        EntityManager.SetComponentData<Translation>(pointEntity, translation);

        EntityManager.SetComponentData<Node>(pointEntity, new Node() { anchor = anchor, neighborCount = neighborCount});

        return pointEntity;
    }
}