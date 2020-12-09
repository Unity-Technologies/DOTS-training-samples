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
        Generate();

        Enabled = false;
    }

    private void Generate()
    {
        List<Point> pointsList = new List<Point>();
        List<Bar> barsList = new List<Bar>();
        
        var buildingEntity = EntityManager.CreateEntity(typeof(Building));
        var constraintBuffer = EntityManager.AddBuffer<Constraint>(buildingEntity);

        // buildings
        for (int i = 0; i < 1; i++)
        {
            int height = Random.Range(4, 12);
            Vector3 pos = new Vector3(Random.Range(-45f, 45f), 0f, Random.Range(-45f, 45f));
            float spacing = 2f;
            for (int j = 0; j < height; j++)
            {
                Point point = new Point();
                point.x = pos.x + spacing;
                point.y = j * spacing;
                point.z = pos.z - spacing;
                point.oldX = point.x;
                point.oldY = point.y;
                point.oldZ = point.z;
                if (j == 0)
                {
                    point.anchor = true;
                }
                pointsList.Add(point);
                point = new Point();
                point.x = pos.x - spacing;
                point.y = j * spacing;
                point.z = pos.z - spacing;
                point.oldX = point.x;
                point.oldY = point.y;
                point.oldZ = point.z;
                if (j == 0)
                {
                    point.anchor = true;
                }
                pointsList.Add(point);
                point = new Point();
                point.x = pos.x + 0f;
                point.y = j * spacing;
                point.z = pos.z + spacing;
                point.oldX = point.x;
                point.oldY = point.y;
                point.oldZ = point.z;
                if (j == 0)
                {
                    point.anchor = true;
                }
                pointsList.Add(point);
            }
        }

        // ground details
        for (int i = 0; i < 6; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-55f, 55f), 0f, Random.Range(-55f, 55f));
            Point point = new Point();
            point.x = pos.x + Random.Range(-.2f, -.1f);
            point.y = pos.y + Random.Range(0f, 3f);
            point.z = pos.z + Random.Range(.1f, .2f);
            point.oldX = point.x;
            point.oldY = point.y;
            point.oldZ = point.z;
            pointsList.Add(point);

            point = new Point();
            point.x = pos.x + Random.Range(.2f, .1f);
            point.y = pos.y + Random.Range(0f, .2f);
            point.z = pos.z + Random.Range(-.1f, -.2f);
            point.oldX = point.x;
            point.oldY = point.y;
            point.oldZ = point.z;
            if (Random.value < .1f)
            {
                point.anchor = true;
            }
            pointsList.Add(point);
        }

        for (int i = 0; i < pointsList.Count; i++)
        {
            for (int j = i + 1; j < pointsList.Count; j++)
            {
                Bar bar = new Bar();
                bar.AssignPoints(pointsList[i], pointsList[j]);
                if (bar.length < 5f && bar.length > .2f)
                {
                    bar.point1.neighborCount++;
                    bar.point2.neighborCount++;

                    barsList.Add(bar);
                }
            }
        }

        var points = new Point[barsList.Count * 2];
        var pointCount = 0;
        for (int i = 0; i < pointsList.Count; i++)
        {
            if (pointsList[i].neighborCount > 0)
            {
                points[pointCount] = pointsList[i];
                pointCount++;
            }
        }
        
        for (int i = 0; i < barsList.Count; i++)
        {
            var curBar = barsList[i];
            float3 pointA, pointB;
            pointA = new float3(curBar.point1.x, curBar.point1.y, barsList[i].point1.z);
            pointB = new float3(curBar.point2.x, curBar.point2.y, barsList[i].point2.z);
            
            var pointAEntity = CreatePointEntity(pointA, curBar.point1.anchor, curBar.point1.neighborCount);
            var pointBEntity = CreatePointEntity(pointB, curBar.point2.anchor, curBar.point2.neighborCount);

            var constraint = new Constraint()
            {
                pointA = pointAEntity,
                pointB = pointBEntity,
                distance = Vector3.Distance(pointA, pointB)
            };

            constraintBuffer = EntityManager.GetBuffer<Constraint>(buildingEntity);
            constraintBuffer.Add(constraint);
        }

        System.GC.Collect();
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