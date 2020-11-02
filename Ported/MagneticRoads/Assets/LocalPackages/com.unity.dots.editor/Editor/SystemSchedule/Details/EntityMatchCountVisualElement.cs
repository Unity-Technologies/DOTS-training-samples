using UnityEngine.UIElements;

namespace Unity.Entities.Editor
{
    class EntityMatchCountVisualElement : BindableElement, IBinding
    {
        EntityQuery m_Query;
        readonly Label m_EntityMatchLabel;

        public EntityQuery Query
        {
            get => m_Query;
            set
            {
                if (m_Query == value)
                    return;

                m_Query = value;

                Update();
            }
        }

        public World CurrentWorld { get; set; }

        public EntityMatchCountVisualElement()
        {
            binding = this;
            m_EntityMatchLabel = new Label();
            Add(m_EntityMatchLabel);
        }

        static void SetText(Label label, string text)
        {
            if (null != label && label.text != text)
                label.text = text;
        }

        public void Update()
        {
            if (Query == default || CurrentWorld == null || !CurrentWorld.IsCreated
                || !CurrentWorld.EntityManager.IsQueryValid(Query))
                return;

            SetText(m_EntityMatchLabel, Query.CalculateEntityCount().ToString());
        }

        public void PreUpdate()
        {
        }

        public void Release()
        {
        }
    }
}
