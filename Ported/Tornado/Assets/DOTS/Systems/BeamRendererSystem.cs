using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Dots
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class BeamRendererSystem : SystemBase
    {
        NativeArray<Matrix4x4> transforms;

        protected override void OnCreate()
        {
            transforms = new NativeArray<Matrix4x4>(2000, Allocator.Persistent);
        }

        protected override void OnUpdate()
        {
            Debug.Log("BeamRendererSystem.OnUpdate");
            var i = 0;
            Entities
            .WithoutBurst()
            .ForEach((in TransformMatrix t) =>
            {
                transforms[i++] = t.matrix;
            }).Run();
        }
    }
}