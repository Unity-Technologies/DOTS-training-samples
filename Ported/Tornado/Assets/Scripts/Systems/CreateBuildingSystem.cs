using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class CreateBuildingSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<BarSpawner>();
    }

    protected override void OnUpdate()
    {
        var random = new Random(1234);
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var spawner = GetSingleton<BarSpawner>();


        var minHeight = spawner.minHeight;
        var maxHeight = spawner.maxHeight;
        var maxBasePoints = spawner.maxBasePoints;
        var horizontalSpacingFactor = spawner.horizontalSpacingFactor;
        var buildingsSpacing = spawner.buildingsSpacing;

        // buildings

        for (int i = 0; i < spawner.buildingsCount; i++)
        {
            var newBuildingEntity = ecb.CreateEntity();
            var newBuildingConstructionData = new BuildingConstructionData();
            newBuildingConstructionData.height = random.NextInt(minHeight, maxHeight);
            float3 pos = new float3(0);
            pos.xz = GetHexPosition(i);
            pos.xz += random.NextFloat2Direction() * random.NextFloat() * -0.5f;
            pos.xz *= buildingsSpacing;

            newBuildingConstructionData.position = pos;
            newBuildingConstructionData.spacing = 2f;

            ecb.AddComponent(newBuildingEntity, newBuildingConstructionData);
            ecb.AddComponent<Building>(newBuildingEntity);
        }

        ecb.Playback(EntityManager);

        ecb.Dispose();

        ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities.ForEach((Entity entity, in BuildingConstructionData buildingConstructionData) =>
       {
           var height = random.NextInt(minHeight, maxHeight);
           var baseSize = random.NextInt(3, maxBasePoints);

           var sizeList = new NativeArray<int>(baseSize, Allocator.Temp);
           for (int i = 0; i < baseSize; i++)
           {
               if (i < 3)
               {
                   sizeList[i] = height;
               }
               sizeList[i] = (int)math.ceil(height / math.pow(2.0f, i-2));
           }

           var pos = buildingConstructionData.position;
           var spacing = buildingConstructionData.spacing;

    var nodesList = ecb.AddBuffer<NodeBuildingData>(entity);

           float3 pointPosition = new float3(0);

           for (int i=0; i<height; i++)
           {
               for (int j=0; j<baseSize; j++)
               {
                   if (i <= sizeList[j])
                   {
                       pointPosition = pos;
                       pointPosition.xz += GetHexPosition(j).xy * spacing* horizontalSpacingFactor;
                       pointPosition.y = i * spacing;

                       var point = new Node();
                       var trans = new Translation();
                       var nodeEntity = ecb.CreateEntity();
                       trans.Value = pointPosition;
                       point.oldPosition = pointPosition;
                       point.anchor = i==0;
                       ecb.AddComponent(nodeEntity, point);
                       ecb.AddComponent(nodeEntity, trans);

                       var nodeData = new NodeBuildingData();
                       nodeData.nodeEntity = nodeEntity;
                       nodeData.node = point;
                       nodeData.translation = trans;
                       nodesList.Add(nodeData);
                   }
               }
           }
       }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
        ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities.ForEach((Entity entity, ref DynamicBuffer<NodeBuildingData> nodesList, in BuildingConstructionData buildingConstructionData) =>
        {
            var constraintsList = ecb.AddBuffer<Constraint>(entity);
            for (int i = 0; i < nodesList.Length; i++)
            {
                for (int j = i + 1; j < nodesList.Length; j++)
                {
                    var bar = new Constraint();

                    var nodeA = GetComponent<Node>(nodesList[i].nodeEntity);
                    var nodeB = GetComponent<Node>(nodesList[j].nodeEntity);
                    var positionA = nodesList[i].translation.Value;
                    var positionB = nodesList[j].translation.Value;

                    bar.AssignPoints(nodesList[i].nodeEntity, nodesList[j].nodeEntity, positionA, positionB);
                    if (bar.distance < 5f && bar.distance > .2f)
                    {
                        nodeA.neighborCount++;
                        SetComponent(nodesList[i].nodeEntity, nodeA);

                        nodeB.neighborCount++;
                        SetComponent(nodesList[j].nodeEntity, nodeB);

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

    static float2 GetHexPosition( int i )
    {
        var o = new float2(0);

        if (i == 0) { return o; }

        int layer = (int)math.round(math.sqrt(i / 3.0f));

        int firstIdxInLayer = 3 * layer * (layer - 1) + 1;
        int side = (i - firstIdxInLayer) / layer; // note: this is integer division
        int idx = (i - firstIdxInLayer) % layer;
        o.x = layer * math.cos((side - 1) * math.PI / 3) + (idx + 1) * math.cos((side + 1) * math.PI / 3);
        o.y = -layer * math.sin((side - 1) * math.PI / 3) - (idx + 1) * math.sin((side + 1) * math.PI / 3);

        return o;
    }
}
