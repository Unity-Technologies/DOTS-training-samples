using JetBrains.Annotations;
using Unity.Mathematics;
using Unity.Properties;
using Unity.Properties.UI;
using Unity.Transforms;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Entities.Editor.Inspectors
{
    abstract class Float4x4ValueInspector<T> : Inspector<T>
    {
        static Float4x4ValueInspector()
        {
            TypeConversion.Register<float4, Vector4>(v => v);
            TypeConversion.Register<Vector4, float4>(v => v);
        }

        public override VisualElement Build()
        {
            var root = new BindableElement
            {
                bindingPath = "Value"
            };

            for (var i = 0; i < 4; ++i)
            {
                var column = new Vector4Field { bindingPath = "c" + i };
                column.Query<FloatField>().ForEach(field => field.formatString = "0.###");
                root.Add(column);
            }

            return root;
        }
    }

    [UsedImplicitly]
    sealed class LocalToParentInspector : Float4x4ValueInspector<LocalToParent>
    {
    }

    [UsedImplicitly]
    sealed class LocalToWorldInspector : Float4x4ValueInspector<LocalToWorld>
    {
    }

    [UsedImplicitly]
    sealed class CompositeRotationInspector : Float4x4ValueInspector<CompositeRotation>
    {
    }

    [UsedImplicitly]
    sealed class CompositeScaleInspector : Float4x4ValueInspector<CompositeScale>
    {
    }

    [UsedImplicitly]
    sealed class ParentScaleInverseInspector : Float4x4ValueInspector<ParentScaleInverse>
    {
    }

    [UsedImplicitly]
    sealed class WorldToLocalInspector : Float4x4ValueInspector<WorldToLocal>
    {
    }

    abstract class Float3ValueInspector<T> : Inspector<T>
    {
        static Float3ValueInspector()
        {
            TypeConversion.Register<float3, Vector3>(v => v);
            TypeConversion.Register<Vector3, float3>(v => v);
        }

        public override VisualElement Build()
        {
            var valueField = new Vector3Field { bindingPath = "Value" };
            valueField.Query<FloatField>().ForEach(field => field.formatString = "0.###");
            return valueField;
        }
    }

    [UsedImplicitly]
    sealed class TranslationInspector : Float3ValueInspector<Translation>
    {
    }

    [UsedImplicitly]
    sealed class NonUniformScaleInspector : Float4x4ValueInspector<NonUniformScale>
    {
    }

    [UsedImplicitly]
    sealed class RotationEulerXYZInspector : Float4x4ValueInspector<RotationEulerXYZ>
    {
    }

    [UsedImplicitly]
    sealed class RotationEulerXZYInspector : Float4x4ValueInspector<RotationEulerXZY>
    {
    }

    [UsedImplicitly]
    sealed class RotationEulerYXZInspector : Float4x4ValueInspector<RotationEulerYXZ>
    {
    }

    [UsedImplicitly]
    sealed class RotationEulerYZXInspector : Float4x4ValueInspector<RotationEulerYZX>
    {
    }

    [UsedImplicitly]
    sealed class RotationEulerZXYInspector : Float4x4ValueInspector<RotationEulerZXY>
    {
    }

    [UsedImplicitly]
    sealed class RotationEulerZYXInspector : Float4x4ValueInspector<RotationEulerZYX>
    {
    }

    [UsedImplicitly]
    sealed class PostRotationEulerXYZInspector : Float4x4ValueInspector<PostRotationEulerXYZ>
    {
    }

    [UsedImplicitly]
    sealed class PostRotationEulerXZYInspector : Float4x4ValueInspector<PostRotationEulerXZY>
    {
    }

    [UsedImplicitly]
    sealed class PostRotationEulerYXZInspector : Float4x4ValueInspector<PostRotationEulerYXZ>
    {
    }

    [UsedImplicitly]
    sealed class PostRotationEulerYZXInspector : Float4x4ValueInspector<PostRotationEulerYZX>
    {
    }

    [UsedImplicitly]
    sealed class PostRotationEulerZXYInspector : Float4x4ValueInspector<PostRotationEulerZXY>
    {
    }

    [UsedImplicitly]
    sealed class PostRotationEulerZYXInspector : Float4x4ValueInspector<PostRotationEulerZYX>
    {
    }

    [UsedImplicitly]
    sealed class RotationPivotInspector : Float4x4ValueInspector<RotationPivot>
    {
    }

    [UsedImplicitly]
    sealed class RotationPivotTranslationInspector : Float4x4ValueInspector<RotationPivotTranslation>
    {
    }

    [UsedImplicitly]
    sealed class ScalePivotInspector : Float4x4ValueInspector<ScalePivot>
    {
    }

    [UsedImplicitly]
    sealed class ScalePivotTranslationInspector : Float4x4ValueInspector<ScalePivotTranslation>
    {
    }

    abstract class QuaternionValueInspector<T> : Inspector<T>
    {
        static QuaternionValueInspector()
        {
            TypeConversion.Register<quaternion, Vector4>(v => v.value);
            TypeConversion.Register<Vector4, quaternion>(v => new quaternion { value = v });
        }

        public override VisualElement Build()
        {
            var valueField = new Vector4Field { bindingPath = "Value" };
            valueField.Query<FloatField>().ForEach(field => field.formatString = "0.###");
            return valueField;
        }
    }

    [UsedImplicitly]
    sealed class RotationInspector : QuaternionValueInspector<Rotation>
    {
    }

    [UsedImplicitly]
    sealed class PostRotationInspector : QuaternionValueInspector<PostRotation>
    {
    }

    abstract class DefaultValueInspector<T> : Inspector<T>
    {
        public override VisualElement Build()
        {
            var root = new VisualElement();
            DoDefaultGui(root, "Value");
            return root;
        }
    }

    [UsedImplicitly]
    class ParentInspector : DefaultValueInspector<Parent>
    {
    }

    [UsedImplicitly]
    class PreviousParentInspector : DefaultValueInspector<PreviousParent>
    {
    }

    [UsedImplicitly]
    class ChildInspector : DefaultValueInspector<Child>
    {
    }
}
