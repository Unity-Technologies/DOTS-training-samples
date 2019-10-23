using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class PathfindingTests
{
    List<GameObject> m_Objs;
    Platform pl0, pl1, pl2, pl3, pl4, pl5;

        [SetUp]
    public void Setup()
    {
        m_Objs = new List<GameObject>();
        m_Objs.Add(new GameObject());
        m_Objs.Add(new GameObject());
        m_Objs.Add(new GameObject());
        m_Objs.Add(new GameObject());
        m_Objs.Add(new GameObject());
        m_Objs.Add(new GameObject());

        pl0 = m_Objs[0].AddComponent<Platform>();
        pl0.platformIndex = 0;
        pl1 = m_Objs[1].AddComponent<Platform>();
        pl1.platformIndex = 1;
        pl2 = m_Objs[2].AddComponent<Platform>();
        pl2.platformIndex = 2;
        pl3 = m_Objs[3].AddComponent<Platform>();
        pl3.platformIndex = 3;
        pl4 = m_Objs[4].AddComponent<Platform>();
        pl4.platformIndex = 4;
        pl5 = m_Objs[5].AddComponent<Platform>();
        pl5.platformIndex = 5;
    }

    [TearDown]
    public void TearDown()
    {
        foreach (var obj in m_Objs)
            Object.Destroy(obj);
    }

    [Test]
    public void TwoStationsWithTwoPlatforms()
    {
        SetupPlatform(pl0, pl1, pl2, new List<Platform>());
        SetupPlatform(pl1, pl0, pl3, new List<Platform>());
        SetupPlatform(pl2, pl3, pl0, new List<Platform>());
        SetupPlatform(pl3, pl2, pl1, new List<Platform>());

        Pathfinding.GeneratePathFindingData(new []
        {
           pl0, pl1, pl2, pl3
        });

        var path0To1 = Pathfinding.GetConnections(0,1);
        ValidateSingleConnection(path0To1, Method.Walk, 1);

        var path0To2 = Pathfinding.GetConnections(0, 2);
        ValidateSingleConnection(path0To2, Method.Train, 2);

        var path0To3 = Pathfinding.GetConnections(0,3);
        ValidateMultipleConnection(path0To3, new []{Method.Walk, Method.Train}, new []{1, 3});

        var path1To0 = Pathfinding.GetConnections(1, 0);
        ValidateSingleConnection(path1To0, Method.Walk, 0);

        var path1To2 = Pathfinding.GetConnections(1,2);
        ValidateMultipleConnection(path1To2, new []{Method.Walk, Method.Train}, new []{0,2});

        var path1To3 = Pathfinding.GetConnections(1, 3);
        ValidateSingleConnection(path1To3, Method.Train, 3);

        var path2To0 = Pathfinding.GetConnections(2, 0);
        ValidateSingleConnection(path2To0, Method.Train, 0);

        var path2To1 = Pathfinding.GetConnections(2,1);
        ValidateMultipleConnection(path2To1, new []{Method.Walk, Method.Train}, new []{3,1});

        var path2To3 = Pathfinding.GetConnections(2, 3);
        ValidateSingleConnection(path2To3, Method.Walk, 3);

        var path3To0 = Pathfinding.GetConnections(3, 0);
        ValidateMultipleConnection(path3To0, new []{Method.Walk, Method.Train}, new []{2,0});

        var path3To1 = Pathfinding.GetConnections(3, 1);
        ValidateSingleConnection(path3To1, Method.Train, 1);

        var path3To2 = Pathfinding.GetConnections(3, 2);
        ValidateSingleConnection(path3To2, Method.Walk, 2);
    }

    [Test]
    public void ThreeStationsWithTwoPlatforms()
    {
        SetupPlatform(pl0, pl1, pl2, new List<Platform>());
        SetupPlatform(pl1, pl0, pl5, new List<Platform>());
        SetupPlatform(pl2, pl3, pl4, new List<Platform>());
        SetupPlatform(pl3, pl2, pl1, new List<Platform>());
        SetupPlatform(pl4, pl5, pl0, new List<Platform>());
        SetupPlatform(pl5, pl4, pl3, new List<Platform>());

        Pathfinding.GeneratePathFindingData(new []
        {
            pl0, pl1, pl2, pl3, pl4, pl5
        });

        //0 to 3
        var path0To3 = Pathfinding.GetConnections(0, 3);
        ValidateMultipleConnection(path0To3, new []{Method.Train, Method.Walk}, new []{2, 3});

        //0 to 2
        var path0To2 = Pathfinding.GetConnections(0, 2);
        ValidateSingleConnection(path0To2, Method.Train, 2);

        //5 to 0
        var path5To0 = Pathfinding.GetConnections(5, 0);
        ValidateMultipleConnection(path5To0, new []{Method.Walk, Method.Train}, new []{4, 0});
    }

    static void ValidateSingleConnection(Connection[] connections, Method expectedMethod, int expectedDestinationId)
    {
        Assert.That(connections.Length, Is.EqualTo(1));
        Assert.That(connections.First().method, Is.EqualTo(expectedMethod));
        Assert.That(connections.First().destinationPlatformId, Is.EqualTo(expectedDestinationId));
    }

    static void ValidateMultipleConnection(Connection[] connections, Method[] methods, int[] expectedDestinationIds)
    {
        for (var i = 0; i< connections.Length; i++)
        {
            Assert.That(connections[i].method, Is.EqualTo(methods[i]));
            Assert.That(connections[i].destinationPlatformId, Is.EqualTo(expectedDestinationIds[i]));
        }
    }

    static void SetupPlatform(Platform platform, Platform opposite, Platform next, List<Platform> adjacent)
    {
        platform.oppositePlatform = opposite;
        platform.nextPlatform = next;
        platform.adjacentPlatforms = adjacent;
    }
}
