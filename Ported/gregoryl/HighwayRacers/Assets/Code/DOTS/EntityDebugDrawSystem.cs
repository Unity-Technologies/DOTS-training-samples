using System.Collections;
using System.Collections.Generic;


using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Assertions;

namespace HighwayRacers
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public class EntityDebugDrawSystem : Unity.Entities.JobComponentSystem
    {
        private List<Vector3> Lines = new List<Vector3>();
        private EntityQuery QueryToDraw = null;

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            //(new DebugDrawJob()).Schedule(this);
            //return inputDeps; 

            var _isDoDraw = true;
            if (_isDoDraw) {
                if (QueryToDraw == null)
                {
                    this.QueryToDraw = this.World.EntityManager.CreateEntityQuery(typeof(LocalToWorld));
                    //LocalToWorld range = new LocalToWorld() { Value = Matrix4x4.identity; };
                    //this.QueryToDraw.SetFilter<LocalToWorld>(RangeInt)
                }
                var data = this.QueryToDraw.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);
                //Debug.Log("Data.Length=" + data.Length);
                for (int i=0; i<data.Length; i++)
                {
                    var ob = data[i];
                    Debug.DrawLine(ob.Position, ob.Position + ((ob.Forward + ob.Up) * 10.0f) );
                }
                Debug.DrawLine(Vector3.zero, Vector3.one, Color.red);
                data.Dispose();
            }

            return inputDeps;
        }

        public struct DebugDrawJob : IJobForEach<LocalToWorld>
        {
            public void Execute([ReadOnly] ref LocalToWorld ob)
            {
                Debug.DrawLine(ob.Position, ob.Position + ((ob.Forward + ob.Up) * 10.0f));
            }
        }
    }

}