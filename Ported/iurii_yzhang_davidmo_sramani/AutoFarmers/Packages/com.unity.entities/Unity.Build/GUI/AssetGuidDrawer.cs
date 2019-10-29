using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.Assertions;
using UnityEngine.UIElements;
using Unity.Properties.Editor;
using UnityEngine;

namespace Unity.Build
{
    abstract class AssetGuidInspectorBase<T> : IInspector<T>, IPropertyDrawer<AssetGuidAttribute>
    {
        protected ObjectField m_Field;
        
        public VisualElement Build(InspectorContext<T> context)
        {
            m_Field = new ObjectField(context.PrettyName)
            {
                allowSceneObjects = false
            };
            
            var asset = context.Attributes.GetAttribute<AssetGuidAttribute>();
            Assert.IsTrue(typeof(UnityEngine.Object).IsAssignableFrom(asset.Type));
            m_Field.objectType = asset.Type;

            m_Field.RegisterValueChangedCallback(evt => OnChanged(context, evt));
            return m_Field;
        }

        public void Update(InspectorContext<T> context)
        {
            OnUpdate(context);
        }
        
        protected abstract void OnChanged(InspectorContext<T> context, ChangeEvent<UnityEngine.Object> evt);
        protected abstract void OnUpdate(InspectorContext<T> context);
    }
    
    [UsedImplicitly]
    sealed class GuidAssetInspector : AssetGuidInspectorBase<GUID>
    {
        protected override void OnChanged(InspectorContext<GUID> context, ChangeEvent<Object> evt)
        {
            if (null != evt.newValue && evt.newValue)
            {
                context.Data = new GUID(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(evt.newValue)));
            }
            else
            {
                context.Data = default;
            }
        }

        protected override void OnUpdate(InspectorContext<GUID> context)
        {
            m_Field.SetValueWithoutNotify(AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(context.Data.ToString())));
        }
    }
    
    [UsedImplicitly]
    sealed class GlobalObjectIdAssetInspector : AssetGuidInspectorBase<GlobalObjectId>
    {
        protected override void OnChanged(InspectorContext<GlobalObjectId> context, ChangeEvent<Object> evt)
        {
            if (null != evt.newValue && evt.newValue)
            {
                context.Data = GlobalObjectId.GetGlobalObjectIdSlow(evt.newValue);
            } 
            else
            {
                context.Data = default;
            }
        }

        protected override void OnUpdate(InspectorContext<GlobalObjectId> context)
        {
            var id = context.Data;
            var defaultId = new GlobalObjectId();
            m_Field.SetValueWithoutNotify(id.assetGUID == defaultId.assetGUID
                ? null
                : GlobalObjectId.GlobalObjectIdentifierToObjectSlow(context.Data));
        }
    }
}
