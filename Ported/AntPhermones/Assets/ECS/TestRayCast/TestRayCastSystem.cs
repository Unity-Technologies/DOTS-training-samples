using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class TestRayCastSystem : SystemBase
{
    protected override void OnCreate()
    {
        TestRayCast.EntityManager = EntityManager;
    }

    protected override void OnUpdate()
    {

    }
}
