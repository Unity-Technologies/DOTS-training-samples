using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class TestRayCastSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<TestRayCastRequirement>();
        TestRayCast.EntityManager = EntityManager;
    }

    protected override void OnUpdate()
    {

    }
}
