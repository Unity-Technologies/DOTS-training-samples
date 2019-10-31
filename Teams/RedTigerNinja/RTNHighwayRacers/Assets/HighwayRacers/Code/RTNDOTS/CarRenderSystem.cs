using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;

namespace HighwayRacers
{

    public class CarRenderSystem : MonoBehaviour
    {
        public Mesh ToDrawMesh;
        public Material ToDrawMaterial;
        public MaterialPropertyBlock MatBlock;
        public Matrix4x4[] PoseArray = null;
        public Vector4[] ColorArray = null;

        public static CarRenderSystem instance = null;

        private void Start()
        {
            MatBlock = new MaterialPropertyBlock();
            instance = this;
        }
    }

    public class CarRenderJobSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var inst = CarRenderSystem.instance;
            if ((!inst) || (inst.MatBlock==null)) return inputDeps;

            var carQuery = this.GetEntityQuery(ComponentType.ReadOnly<CarRenderData>());
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
    }

}