using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Unity.Rendering
{
    // NOTE: This is a code example material override component for setting RGBA color to hybrid renderer. 
    //       You should implement your own material property override components inside your own project.

    [Serializable]
    [MaterialProperty("_Color", MaterialPropertyFormat.Float4)]
    public struct MaterialColor : IComponentData
    {
        public float4 Value;
    }

    namespace Authoring
    {
        [DisallowMultipleComponent]
        [RequiresEntityConversion]
        public class MaterialColor : MonoBehaviour, IConvertGameObjectToEntity
        {
            public Color color;

            public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
            {
                var data = new Unity.Rendering.MaterialColor { Value = new float4(color.r, color.g, color.b, color.a) };
                dstManager.AddComponentData(entity, data);
            }
        }
    }
}
