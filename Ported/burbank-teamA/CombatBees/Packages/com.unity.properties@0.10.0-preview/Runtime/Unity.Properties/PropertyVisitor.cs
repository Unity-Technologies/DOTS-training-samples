using System.Collections.Generic;

namespace Unity.Properties
{
    /// <summary>
    /// Default visit collection item getter 
    /// </summary>
    /// <typeparam name="TContainer">Type of the container to visit.</typeparam>
    public struct VisitCollectionElementCallback<TContainer> : ICollectionElementPropertyGetter<TContainer>
    {
        readonly IPropertyVisitor m_Visitor;
        
        public VisitCollectionElementCallback(IPropertyVisitor visitor)
        {
            m_Visitor = visitor;
        }

        void ICollectionElementPropertyGetter<TContainer>.VisitProperty<TElementProperty, TElement>(TElementProperty property, ref TContainer container, ref ChangeTracker changeTracker)
            => m_Visitor.VisitProperty<TElementProperty, TContainer, TElement>(property, ref container, ref changeTracker);

        void ICollectionElementPropertyGetter<TContainer>.VisitCollectionProperty<TElementProperty, TElement>(TElementProperty property, ref TContainer container, ref ChangeTracker changeTracker)
            => m_Visitor.VisitCollectionProperty<TElementProperty, TContainer, TElement>(property, ref container, ref changeTracker);
    }

    /// <inheritdoc />
    /// <summary>
    /// Default base class for extending the visitation API.
    /// </summary>
    public class PropertyVisitor : IPropertyVisitor
    {
        List<IPropertyVisitorAdapter> m_Adapters;

        public void AddAdapter(IPropertyVisitorAdapter adapter)
        {
            if (null == m_Adapters)
            {
                m_Adapters = new List<IPropertyVisitorAdapter>();
            }

            m_Adapters.Add(adapter);
        }

        public VisitStatus VisitProperty<TProperty, TContainer, TValue>(TProperty property, ref TContainer container, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, TValue>
        {
            // Give users a chance to filter based on the data.
            if (IsExcluded<TProperty, TContainer, TValue>(property, ref container))
            {
                return VisitStatus.Handled;
            }

            var value = property.GetValue(ref container);
            var valueChangeTracker = new ChangeTracker(changeTracker.VersionStorage);

            var status = property.IsContainer
                ? TryVisitContainerWithAdapters(property, ref container, ref value, ref valueChangeTracker)
                : TryVisitValueWithAdapters(property, ref container, ref value, ref valueChangeTracker);

            if (property.IsReadOnly)
            {
                return status;
            }

            property.SetValue(ref container, value);

            if (valueChangeTracker.IsChanged())
            {
                changeTracker.IncrementVersion<TProperty, TContainer, TValue>(property, ref container);
            }

            return status;
        }

        public VisitStatus VisitCollectionProperty<TProperty, TContainer, TValue>(TProperty property, ref TContainer container, ref ChangeTracker changeTracker)
            where TProperty : ICollectionProperty<TContainer, TValue>
        {
            // Give users a chance to filter based on the data.
            if (IsExcluded<TProperty, TContainer, TValue>(property, ref container))
            {
                return VisitStatus.Handled;
            }

            var value = property.GetValue(ref container);

            return TryVisitCollectionWithAdapters(property, ref container, ref value, ref changeTracker);
        }

        VisitStatus TryVisitValueWithAdapters<TProperty, TContainer, TValue>(TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, TValue>
        {
            if (null != m_Adapters)
            {
                for (var i = 0; i < m_Adapters.Count; i++)
                {
                    VisitStatus status;

                    if ((status = m_Adapters[i].TryVisitValue(this, property, ref container, ref value, ref changeTracker)) != VisitStatus.Unhandled)
                    {
                        return status;
                    }
                }
            }

            return Visit(property, ref container, ref value, ref changeTracker);
        }

        VisitStatus TryVisitContainerWithAdapters<TProperty, TContainer, TValue>(TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, TValue>
        {
            VisitStatus status;

            if (null != m_Adapters)
            {
                for (var i = 0; i < m_Adapters.Count; i++)
                {
                    if ((status = m_Adapters[i].TryVisitContainer(this, property, ref container, ref value, ref changeTracker)) != VisitStatus.Unhandled)
                    {
                        return status;
                    }
                }
            }

            if ((status = BeginContainer(property, ref container, ref value, ref changeTracker)) == VisitStatus.Handled)
            {
                PropertyContainer.Visit(ref value, this, ref changeTracker);
                EndContainer(property, ref container, ref value, ref changeTracker);
            }

            return status;
        }

        VisitStatus TryVisitCollectionWithAdapters<TProperty, TContainer, TValue>(TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            where TProperty : ICollectionProperty<TContainer, TValue>
        {
            VisitStatus status;

            if (null != m_Adapters)
            {
                for (var i = 0; i < m_Adapters.Count; i++)
                {
                    if ((status = m_Adapters[i].TryVisitCollection(this, property, ref container, ref value, ref changeTracker)) != VisitStatus.Unhandled)
                    {
                        return status;
                    }
                }
            }

            if ((status = BeginCollection(property, ref container, ref value, ref changeTracker)) != VisitStatus.Unhandled)
            {
                if (status == VisitStatus.Handled)
                {
                    for (int i = 0, count = property.GetCount(ref container); i < count; i++)
                    {
                        var callback = new VisitCollectionElementCallback<TContainer>(this);
                        var elementChangeTracker = new ChangeTracker(changeTracker.VersionStorage);

                        property.GetPropertyAtIndex(ref container, i, ref elementChangeTracker, ref callback);

                        if (elementChangeTracker.IsChanged())
                        {
                            changeTracker.IncrementVersion<TProperty, TContainer, TValue>(property, ref container);
                        }
                    }
                }

                EndCollection(property, ref container, ref value, ref changeTracker);
            }

            return status;
        }

        /// <summary>
        /// Invoked before entering into any node.
        /// </summary>
        /// <returns>True if the visit event should be skipped.</returns>
        public virtual bool IsExcluded<TProperty, TContainer, TValue>(TProperty property, ref TContainer container)
            where TProperty : IProperty<TContainer, TValue>
        {
            return false;
        }

        /// <summary>
        /// Invoked when entering any value leaf node.
        /// </summary>
        protected virtual VisitStatus Visit<TProperty, TContainer, TValue>(TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, TValue>
        {
            return VisitStatus.Handled;
        }

        /// <summary>
        /// Invoked before entering into a container node.
        ///
        /// If false is returned, the container should NOT be visited and <see cref="EndContainer{TProperty, TContainer,TValue}"/> should NOT be called.
        /// </summary>
        /// <returns>True if the visit event was consumed.</returns>
        protected virtual VisitStatus BeginContainer<TProperty, TContainer, TValue>(TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, TValue>
        {
            return VisitStatus.Handled;
        }

        /// <summary>
        /// Invoked after completing the node. Only if <see cref="BeginContainer{TProperty,TContainer,TValue}"/> returned true.
        /// </summary>
        protected virtual void EndContainer<TProperty, TContainer, TValue>(TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, TValue>
        {

        }

        /// <summary>
        /// Invoked before entering into a collection node.
        ///
        /// If false is returned, the collection should NOT be visited and <see cref="EndCollection{TProperty,TContainer,TValue}"/> should NOT be called.
        /// </summary>
        /// <returns>True if the visit event was consumed.</returns>
        protected virtual VisitStatus BeginCollection<TProperty, TContainer, TValue>(TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            where TProperty : ICollectionProperty<TContainer, TValue>
        {
            return VisitStatus.Handled;
        }

        /// <summary>
        /// Invoked after completing a collection node. Only if <see cref="BeginCollection{TProperty,TContainer,TValue}"/> returned true.
        /// </summary>
        protected virtual void EndCollection<TProperty, TContainer, TValue>(TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            where TProperty : ICollectionProperty<TContainer, TValue>
        {

        }
    }
}
