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
                    str += distanceField.PlantDistFieldRead[y * creator.WorldSize.x + x] + " ";
                }
                Debug.Log(str);
            }
            

            foreach (var val in distanceField.PlantDistFieldRead)
            {
                Assert.AreEqual(-1, val);
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
                    str += distanceField.PlantDistFieldRead[y * creator.WorldSize.x + x] + " ";
                }
                Debug.Log(str);
            }
            
            // TODO: check
        }


        [Test]
        public void DistanceFieldComplex1()
        {
            DistanceFieldComplex(@"
            XO..........................
            X.........................X.
            .........XXXXX..............
            ........X..O..X.....X.......
            .........X...X......X.O.....
            ..........XXX...............
            ............................");
        }

        [Test]
        public void DistanceFieldComplex2x2()
        {
            var df = DistanceFieldComplex(@"
            XO
            ..");

            var data = df.PlantDistFieldRead;
            Assert.AreEqual(-1, data[0]);
            Assert.AreEqual(0, data[1]);
            Assert.AreEqual(2, data[2]);
            Assert.AreEqual(1, data[3]);
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
                    str += distanceField.PlantDistFieldRead[y * creator.WorldSize.x + x] + " ";
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