using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;

namespace HighwayRacers
{

    public class CarRenderSystem : MonoBehaviour
    {
        public Mesh ToDrawMesh;
        public Material ToDrawMaterial;
        public MaterialPropertyBlock MatBlock;
        public Matrix4x4[] PoseArray = null;
        public Vector4[] ColorArray = null;
        public Transform ReferenceCenter;

        public Mesh UnitCubeMesh;
        public Material UnitCubeMat;
        public Matrix4x4[] UnitPoseArray = null;

        public static CarRenderSystem instance = null;

        private void Start()
        {
            MatBlock = new MaterialPropertyBlock();
            instance = this;
        }
    }

    public class CarRenderJobSystem : JobComponentSystem
    {
        private EntityQuery CachedMainQuery;

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var inst = CarRenderSystem.instance;
            if ((!inst) || (inst.MatBlock==null)) return inputDeps;

            var carQuery = CachedMainQuery ?? this.GetEntityQuery(ComponentType.ReadOnly<CarRenderData>());
            this.CachedMainQuery = carQuery;
            var carData = carQuery.ToComponentDataArray<CarRenderData>(Unity.Collections.Allocator.TempJob);

            if ((inst.PoseArray==null) || (inst.PoseArray.Length != carData.Length))
            {
                inst.PoseArray = new Matrix4x4[carData.Length];
                inst.ColorArray = new Vector4[carData.Length];
                inst.MatBlock = new MaterialPropertyBlock();
            }
            Matrix4x4[] carPoses = inst.PoseArray;
            for (var i=0; i<carData.Length; i++)
            {
                inst.PoseArray[i] = carData[i].Matrix;
                inst.ColorArray[i] = carData[i].Color;
            }
            inst.MatBlock.SetVectorArray("_Color", inst.ColorArray);

            Graphics.DrawMeshInstanced(inst.ToDrawMesh, 0, inst.ToDrawMaterial, carPoses, carPoses.Length, inst.MatBlock);

            carData.Dispose();

            return inputDeps;
        }

        public static JobHandle DrawNearbyStuff(JobHandle inputDeps, NativeArray<CarsNearbyData> links)
        {
            var inst = CarRenderSystem.instance;
            if ((!inst) || (inst.MatBlock == null)) return inputDeps;

            var maxLinks = links.Length * CarsNearbyData.MAX_COUNT;
            if ((inst.UnitPoseArray==null) || (inst.UnitPoseArray.Length != maxLinks))
            {
                inst.UnitPoseArray = new Matrix4x4[maxLinks];
            }

            var numLinks = 0;
            var offset = Vector3.up * 1.0f;
            for (var i = 0; i < links.Length; i++)
            {
                var enty = links[i];
                for (var j = 0; j < enty.Refs.Count; j++)
                {
                    var lnk = enty.Refs.Get(j);
                    var index = numLinks;
                    numLinks++;

                    var delta = (Vector3)(lnk.Position - enty.MyPosition);

                    var mtx = Matrix4x4.TRS(
                        ((Vector3)(enty.MyPosition + lnk.Position) * 0.5f) + offset,
                        Quaternion.LookRotation(delta, Vector3.up),
                        new Vector3(0.15f, 0.15f, delta.magnitude * 0.95f));
                    inst.UnitPoseArray[index] = mtx;

                    if (numLinks == 1000)
                    {
                        Graphics.DrawMeshInstanced(inst.UnitCubeMesh, 0, inst.UnitCubeMat, inst.UnitPoseArray, numLinks);
                        numLinks = 0;
                    }
                }
            }

            Graphics.DrawMeshInstanced(inst.UnitCubeMesh, 0, inst.UnitCubeMat, inst.UnitPoseArray, numLinks);

            return inputDeps;
        }
    }

}