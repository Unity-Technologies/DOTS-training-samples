using System;
using System.Linq;
using GameAI;
using NUnit.Framework;
using Pathfinding;
using Unity.Entities.Tests;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Tests
{
    public class DistanceFieldSystemTests : ECSTestsFixture
    {
        [Test]
        public void DistanceFieldEmpty()
        {
            var creator = World.GetOrCreateSystem<WorldCreatorSystem>();
            creator.WorldSize = new int2(2, 3);
            
            var pq = m_Manager.CreateEntityQuery(typeof(PlantPositionRequest));
            var pblock = m_Manager.CreateEntityQuery(typeof(StonePositionRequest));

            var distanceField = new DistanceField(creator.WorldSize, pq, pblock);
            
            var handle2 = distanceField.SchedulePlantField();
            
            handle2.Complete();

            for (int y = creator.WorldSize.y - 1; y >= 0; y--)
            {
                var str = "";
                for (int x = 0; x < creator.WorldSize.x; x++)
                {
                    var val = distanceField.PlantDistFieldRead[y * creator.WorldSize.x + x];
                    var valStr = val == int.MaxValue ? "X" : val.ToString();
                        
                    str += valStr + " ";
                }
                Debug.Log(str);
            }
            

            foreach (var val in distanceField.PlantDistFieldRead)
            {
                Assert.AreEqual(int.MaxValue, val);
            }
        }
        
        [Test]
        public void DistanceFieldOne()
        {
            var creator = World.GetOrCreateSystem<WorldCreatorSystem>();
            creator.WorldSize = new int2(2, 3);

            var e = m_Manager.CreateEntity(typeof(PlantPositionRequest));
            m_Manager.SetComponentData(e, new PlantPositionRequest {position = new int2(1,2)});
            
            var pq = m_Manager.CreateEntityQuery(typeof(PlantPositionRequest));
            var pblock = m_Manager.CreateEntityQuery(typeof(StonePositionRequest));
            var distanceField = new DistanceField(creator.WorldSize, pq, pblock);
            
            var handle2 = distanceField.SchedulePlantField();
            handle2.Complete();

            for (int y = creator.WorldSize.y - 1; y >= 0; y--)
            {
                var str = "";
                for (int x = 0; x < creator.WorldSize.x; x++)
                {
                    var val = distanceField.PlantDistFieldRead[y * creator.WorldSize.x + x];
                    var valStr = val == int.MaxValue ? "X" : val.ToString();
                        
                    str += valStr + " ";
                }
                Debug.Log(str);
            }
            
            // TODO: check
        }


        [Test]
        public void DistanceFieldComplex1()
        {
            var df = DistanceFieldComplex(@"
            XO..........................
            X.........................X.
            .........XXXXX..............
            ........X.....X.....X.......
            .........X...X......X.O.....
            ..........XXX...............
            .x..........................");

            foreach (var startPos in new [] {new int2(3, 1), 
                new int2(15, 6),
                new int2(12, 3), 
            })
            {
                Debug.Log($"Starting from {startPos}");
                
                var reached = false;
                var currentPosition = startPos;

                for (;;)
                {
                    var prevVal = df.GetDistanceFieldValue(currentPosition);
                    Debug.Log($"pos = {currentPosition} val = {prevVal} reached {reached}");
                    currentPosition = df.PathToPlant(currentPosition, out reached);
                    var newVal = df.GetDistanceFieldValue(currentPosition);

                    if (reached)
                        Assert.IsTrue(newVal == 0, $"Reached == true, but {newVal} != 0");

                    if (newVal == prevVal)
                        break;

                    Assert.IsTrue(newVal < prevVal, $"new {newVal} shoud be < prev {prevVal}");
                }
            }
        }

        [Test]
        public void DistanceFieldComplex2x2()
        {
            var df = DistanceFieldComplex(@"
            XO
            ..");

            var data = df.PlantDistFieldRead;
            Assert.AreEqual(int.MaxValue, data[0]);
            Assert.AreEqual(0, data[1]);
            Assert.AreEqual(2, data[2]);
            Assert.AreEqual(1, data[3]);

            var reached = false;
            var currentPosition = new int2(0, 1);

            Debug.Log($"pos = {currentPosition} reached {reached}");
            
            currentPosition = df.PathToPlant(currentPosition, out reached);
            Assert.IsFalse(reached);
            Assert.AreEqual(new int2(1, 1), currentPosition);
            
            Debug.Log($"pos = {currentPosition} reached {reached}");

            currentPosition = df.PathToPlant(currentPosition, out reached);
            Assert.IsTrue(reached);
            Assert.AreEqual(new int2(1, 0), currentPosition);
            
            Debug.Log($"pos = {currentPosition} reached {reached}");
        }

        public DistanceField DistanceFieldComplex(string map)
        {
            map = map.Trim().Replace(" ", "");
            
            Debug.Log(map);
            
            string[] lines = map.Split(
                new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None
            );

            int sizeY = lines.Length;
            int sizeX = lines[0].Length;

            Assert.AreEqual(0, lines.Count(s => s.Length != sizeX), "Fix your map pls"); 
            
            var creator = World.GetOrCreateSystem<WorldCreatorSystem>();
            creator.WorldSize = new int2(sizeX, sizeY);

            for (int y = sizeY - 1; y >= 0; y--)
            {
                for (int x = 0; x < sizeX; ++x)
                {
                    if (lines[y][x] == 'X')
                    {
                        var e = m_Manager.CreateEntity(typeof(StonePositionRequest));
                        m_Manager.SetComponentData(e, new StonePositionRequest {position = new int2(x,y), size = new int2(1,1)});            
                    }
                    else if (lines[y][x] == 'O')
                    {
                        var e = m_Manager.CreateEntity(typeof(PlantPositionRequest));
                        m_Manager.SetComponentData(e, new PlantPositionRequest {position = new int2(x,y)});
                    }
                }
            }
            
            
            var pq = m_Manager.CreateEntityQuery(typeof(PlantPositionRequest));
            var pblock = m_Manager.CreateEntityQuery(typeof(StonePositionRequest));
            var distanceField = new DistanceField(creator.WorldSize, pq, pblock);
            
            var handle2 = distanceField.SchedulePlantField();
            handle2.Complete();

            for (int y = 0; y < creator.WorldSize.y; y++)
            {
                var str = "";
                for (int x = 0; x < creator.WorldSize.x; x++)
                {
                    var val = distanceField.PlantDistFieldRead[y * creator.WorldSize.x + x];
                    var valStr = val == int.MaxValue ? "X" : val.ToString();
                        
                    str += valStr + " ";
                }
                Debug.Log(str);
            }

            return distanceField;
        }

        
        [Test]
        public void DistanceFieldSwapTest()
        {
            const int sx = 20;
            const int sy = 4;
            
            var creator = World.GetOrCreateSystem<WorldCreatorSystem>();
            creator.WorldSize = new int2(sx, sy);

            var a = new int[sx * sy];
            var c = new int[sx * sy];
            var d = new int[sx * sy];

            var e = m_Manager.CreateEntity(typeof(PlantPositionRequest));
            m_Manager.SetComponentData(e, new PlantPositionRequest {position = new int2(1,2)});
            
            var e2 = m_Manager.CreateEntity(typeof(PlantPositionRequest));
            m_Manager.SetComponentData(e2, new PlantPositionRequest {position = new int2(1,2)});

            var pq = m_Manager.CreateEntityQuery(typeof(PlantPositionRequest));
            var pblock = m_Manager.CreateEntityQuery(typeof(StonePositionRequest));

            var distanceField = new DistanceField(creator.WorldSize, pq, pblock);
            
            distanceField.PlantDistFieldRead.CopyTo(a);

            var handle1 = distanceField.SchedulePlantField();
            
            m_Manager.SetComponentData(e2, new PlantPositionRequest {position = new int2(2,2)});
            
            handle1.Complete();
            Assert.IsTrue(handle1.IsCompleted);

            distanceField.PlantDistFieldRead.CopyTo(c);

            var handle2 = distanceField.SchedulePlantField();
            Assert.IsFalse(handle2.IsCompleted);
            
            handle2.Complete();

            distanceField.PlantDistFieldRead.CopyTo(d);

            // c has (1,2)
            // d has (1,2) + (2,2)

            // a != c
            // c != d
            
            //Assert.IsTrue(IsArrayEqual(a, b));
            Assert.IsFalse(IsArrayEqual(a, c));
            Assert.IsFalse(IsArrayEqual(c, d));
        }

        private bool IsArrayEqual(int[] a, int[] b)
        {
            var n = a.Length;
            if (n != b.Length)
                return false;
            for (int i = 0; i < n; i++)
            {
                if (a[i] != b[i])
                    return false;
            }

            return true;
        }
    }
}