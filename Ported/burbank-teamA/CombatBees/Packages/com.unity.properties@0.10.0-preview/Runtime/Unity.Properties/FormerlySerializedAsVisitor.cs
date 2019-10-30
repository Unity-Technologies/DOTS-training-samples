namespace Unity.Properties
{
    struct FormerlySerializedAsVisitor : IPropertyVisitor
    {
        readonly string m_SerializedName;
        public string CurrentName;
        
        public FormerlySerializedAsVisitor(string serializedName)
        {
            m_SerializedName = serializedName;
            CurrentName = string.Empty;
        }
        
        public VisitStatus VisitProperty<TProperty, TContainer, TValue>(
            TProperty property,
            ref TContainer container,
            ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, TValue>
            => Visit<TProperty, TContainer, TValue>(property);

        public VisitStatus VisitCollectionProperty<TProperty, TContainer, TValue>(
            TProperty property,
            ref TContainer container,
            ref ChangeTracker changeTracker)
            where TProperty : ICollectionProperty<TContainer, TValue>
            => Visit<TProperty, TContainer, TValue>(property);
        
        VisitStatus Visit<TProperty, TContainer, TValue>(
            TProperty property)
            where TProperty : IProperty<TContainer, TValue>
        {
            var attributes = property.Attributes?.GetAttributes<UnityEngine.Serialization.FormerlySerializedAsAttribute>();
            if (null == attributes)
            {
                return VisitStatus.Override;
            }

            foreach (var formerlySerializedAs in attributes)
            {
                if (formerlySerializedAs.oldName != m_SerializedName)
                {
                    continue;
                }
                CurrentName = property.GetName();
                break;
            }

            return VisitStatus.Override;
        }
    }
}