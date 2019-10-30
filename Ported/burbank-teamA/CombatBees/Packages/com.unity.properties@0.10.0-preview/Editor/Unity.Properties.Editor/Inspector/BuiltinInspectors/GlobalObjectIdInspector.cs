using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Unity.Properties.Editor
{
    [UsedImplicitly]
    class GlobalObjectIdInspector : IInspector<GlobalObjectId>
    {
        enum IdentifierType
        {
            [UsedImplicitly] Null,
            [UsedImplicitly] ImportedAsset,
            [UsedImplicitly] SceneObject,
            [UsedImplicitly] SourceAsset
        }

        EnumField m_IdentifierType;
        TextField m_Guid;
        TextField m_FileId;
        
        public VisualElement Build(InspectorContext<GlobalObjectId> context)
        {
            var root = new Foldout
            {
                text = context.PrettyName,
                tooltip = context.Tooltip
            };
            var id = context.Data;
            m_IdentifierType = new EnumField(ObjectNames.NicifyVariableName(nameof(GlobalObjectId.identifierType)), (IdentifierType)id.identifierType);
            m_IdentifierType.SetEnabled(false);
            root.contentContainer.Add(m_IdentifierType);
            m_Guid = new TextField(ObjectNames.NicifyVariableName(nameof(GlobalObjectId.assetGUID)));
            m_Guid.SetValueWithoutNotify(context.Data.assetGUID.ToString());
            m_Guid.SetEnabled(false);
            root.contentContainer.Add(m_Guid);
            
            m_FileId = new TextField("File Id");
            m_FileId.SetValueWithoutNotify(context.Data.targetObjectId.ToString());
            m_FileId.SetEnabled(false);
            root.contentContainer.Add(m_FileId);
            return root;
        }

        public void Update(InspectorContext<GlobalObjectId> context)
        {
        }
    }
}