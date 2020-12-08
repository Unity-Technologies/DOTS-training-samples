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


        float3 debugPoint1, debugPoint2, debugPoint3;

        var buildingEntity = EntityManager.CreateEntity(typeof(Building), typeof(BuildingNeedsBars));
        var constraintBuffer = EntityManager.AddBuffer<Constraint>(buildingEntity);

        // buildings
        for (int i = 0; i < 35; i++)
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
        for (int i = 0; i < 600; i++)
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

        List<Entity> finalNodes = new List<Entity>();
        List<float3> finalPositions = new List<float3>();

        var spawner = GetSingleton<BarSpawner>();

        for (int i = 0; i < barsList.Count; i++)
        {
            var curBar = barsList[i];
            float3 pointA, pointB;
            pointA = new float3(curBar.point1.x, curBar.point1.y, barsList[i].point1.z);
            pointB = new float3(curBar.point2.x, curBar.point2.y, barsList[i].point2.z);

            Debug.DrawLine(pointA, pointB, Color.cyan, 50);

            //var barEntity = EntityManager.CreateEntity(typeof(Constraint));

            var pointAEntity = CreatePointEntity(pointA, curBar.point1.anchor);
            var pointBEntity = CreatePointEntity(pointB, curBar.point2.anchor);

            finalNodes.Add(pointAEntity);
            finalNodes.Add(pointBEntity);
            finalPositions.Add(pointA);
            finalPositions.Add(pointB);

            var constraint = new Constraint()
            {
                pointA = pointAEntity,
                pointB = pointBEntity,
                distance = Vector3.Distance(pointA, pointB)
            };

            /*
            var instance = EntityManager.Instantiate(spawner.barPrefab);
            var translation = new Translation();
            var rotation = new Rotation();
            var scale = new NonUniformScale();

            translation.Value = (pointA + pointB) * 0.5f;
            rotation.Value = Quaternion.LookRotation(((Vector3)(pointA - pointB)).normalized);
            scale.Value = new float3(0.2f, 0.2f, Vector3.Distance(pointA, pointB));

            EntityManager.SetComponentData(instance, rotation);
            EntityManager.SetComponentData(instance, translation);
            EntityManager.AddComponentData(instance, scale);
            */

            constraintBuffer = EntityManager.GetBuffer<Constraint>(buildingEntity);
            constraintBuffer.Add(constraint);
        }

        /*
        constraintBuffer = EntityManager.GetBuffer<Constraint>(buildingEntity);
        for (int i=0; i<finalNodes.Count; i++)
        {
            for (int j=i+1; j<finalNodes.Count; j++)
            {
                if (Vector3.Distance(finalPositions[j], finalPositions[i]) < 0.0001f)
                {
                    constraintBuffer.Add(new Constraint() {
                        pointA = finalNodes[i],
                        pointB = finalNodes[j],
                        distance = 0
                    });

                    Debug.DrawRay(finalPositions[i], Vector3.left * 0.5f, Color.red, 50f);
                }
            }
        }
        */

        Debug.Log(pointCount + " points, room for " + points.Length + " (" + barsList.Count + " bars)");
        
        System.GC.Collect();
    }

    Entity CreatePointEntity( float3 position, bool anchor)
    {
        var pointEntity = EntityManager.CreateEntity(typeof(Translation), typeof(Node));

        var translation = EntityManager.GetComponentData<Translation>(pointEntity);
        translation.Value = new float3(position);
        EntityManager.SetComponentData<Translation>(pointEntity, translation);

        EntityManager.SetComponentData<Node>(pointEntity, new Node() { anchor = anchor });

        return pointEntity;
    }
}