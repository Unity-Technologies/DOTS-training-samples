using Unity.Properties;

namespace Unity.Serialization.Json
{
    class JsonVisitorAdapterUnityEditor : JsonVisitorAdapter
#if UNITY_EDITOR
        , IVisitAdapter<UnityEditor.GUID>
        , IVisitAdapter<UnityEditor.GlobalObjectId>
#endif
    {
        public JsonVisitorAdapterUnityEditor(JsonVisitor visitor) : base(visitor) { }

        public static void RegisterTypes()
        {
#if UNITY_EDITOR
            TypeConversion.Register<SerializedStringView, UnityEditor.GUID>(view => UnityEditor.GUID.TryParse(view.ToString(), out var guid) ? guid : default);
            TypeConversion.Register<SerializedStringView, UnityEditor.GlobalObjectId>(view => UnityEditor.GlobalObjectId.TryParse(view.ToString(), out var id) ? id : default);
#endif
        }

#if UNITY_EDITOR
        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref UnityEditor.GUID value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, UnityEditor.GUID>
        {
            AppendJsonString(property, value.ToString());
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref UnityEditor.GlobalObjectId value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, UnityEditor.GlobalObjectId>
        {
            AppendJsonString(property, value.ToString());
            return VisitStatus.Override;
        }
#endif
    }
}
