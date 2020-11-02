using JetBrains.Annotations;
using Unity.Mathematics;
using Unity.Properties;
using UnityEditor.UIElements;
using UnityEngine;

namespace Unity.Entities.Editor.Inspectors
{
    [UsedImplicitly]
    class Float2Inspector : BaseFieldInspector<Vector2Field, Vector2, float2>
    {
        static Float2Inspector()
        {
            TypeConversion.Register<float2, Vector2>(v => v);
            TypeConversion.Register<Vector2, float2>(v => v);
        }
    }

    [UsedImplicitly]
    class Float3Inspector : BaseFieldInspector<Vector3Field, Vector3, float3>
    {
        static Float3Inspector()
        {
            TypeConversion.Register<float3, Vector3>(v => v);
            TypeConversion.Register<Vector3, float3>(v => v);
        }
    }

    [UsedImplicitly]
    class Float4Inspector : BaseFieldInspector<Vector4Field, Vector4, float4>
    {
        static Float4Inspector()
        {
            TypeConversion.Register<float4, Vector4>(v => v);
            TypeConversion.Register<Vector4, float4>(v => v);
        }
    }

    [UsedImplicitly]
    class QuaternionInspector : BaseFieldInspector<Vector4Field, Vector4, quaternion>
    {
        static QuaternionInspector()
        {
            TypeConversion.Register<quaternion, Vector4>(v => v.value);
            TypeConversion.Register<Vector4, quaternion>(v => new quaternion { value = v });
        }
    }
}
