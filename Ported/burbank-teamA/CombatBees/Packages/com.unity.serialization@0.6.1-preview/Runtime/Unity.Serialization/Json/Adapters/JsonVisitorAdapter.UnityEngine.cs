using Unity.Properties;

namespace Unity.Serialization.Json
{
    internal class JsonVisitorAdapterUnityEngine : JsonVisitorAdapter
        , IVisitAdapter<UnityEngine.Object>
        , IVisitContainerAdapter
    {
        public JsonVisitorAdapterUnityEngine(JsonVisitor visitor) : base(visitor) { }

        public static void RegisterTypes()
        {
            TypeConversion.Register<SerializedStringView, UnityEngine.Object>((view) =>
            {
#if UNITY_EDITOR
                if (UnityEditor.GlobalObjectId.TryParse(view.ToString(), out var id))
                {
                    return UnityEditor.GlobalObjectId.GlobalObjectIdentifierToObjectSlow(id);
                }
#endif
                return default;
            });
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref UnityEngine.Object value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, UnityEngine.Object>
        {
#if UNITY_EDITOR
            var obj = value as UnityEngine.Object;
            AppendJsonString(property, UnityEditor.GlobalObjectId.GetGlobalObjectIdSlow(obj).ToString());
#else
            AppendJsonString(property, null);
#endif
            return VisitStatus.Override;
        }

        public VisitStatus BeginContainer<TProperty, TValue, TContainer>(IPropertyVisitor visitor, TProperty property,
            ref TContainer container, ref TValue value, ref ChangeTracker changeTracker) where TProperty : IProperty<TContainer, TValue>
        {
            if (!typeof(UnityEngine.Object).IsAssignableFrom(typeof(TValue)))
            {
                return VisitStatus.Unhandled;
            }

#if UNITY_EDITOR
            var obj = value as UnityEngine.Object;
            AppendJsonString(property, UnityEditor.GlobalObjectId.GetGlobalObjectIdSlow(obj).ToString());
#else
            AppendJsonString(property, null);
#endif
            return VisitStatus.Override;
        }

        public void EndContainer<TProperty, TValue, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container,
            ref TValue value, ref ChangeTracker changeTracker) where TProperty : IProperty<TContainer, TValue>
        {
        }
    }
}
