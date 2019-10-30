namespace Unity.Properties
{
    /// <summary>
    /// Implement this interface to intercept the visitation for a specific <see cref="TContainer"/> and <see cref="TValue"/> pair.
    /// </summary>
    public interface IVisitAdapter<TContainer, TValue>
    {
        /// <summary>
        /// Invoked when the visitor encounters specific a <see cref="TContainer"/> and <see cref="TValue"/> pair.
        /// </summary>
        VisitStatus Visit<TProperty>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, TValue>;
    }

    /// <summary>
    /// Implement this interface to intercept the visitation for a specific <see cref="TValue"/> type.
    /// </summary>
    public interface IVisitAdapter<TValue>
    {
        /// <summary>
        /// Invoked when the visitor encounters specific <see cref="TValue"/> type with any container.
        /// </summary>
        VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, TValue>;
    }

    /// <summary>
    /// Implement this interface to intercept the visitation for all unhandled leaf nodes.
    /// </summary>
    public interface IVisitAdapter
    {
        /// <summary>
        /// Invoked when the visitor encounters any leaf node.
        /// </summary>
        VisitStatus Visit<TProperty, TContainer, TValue>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, TValue>;
    }

    /// <summary>
    /// Implement this interface to intercept container visitation for a specific <see cref="TContainer"/> and <see cref="TValue"/> pair.
    /// </summary>
    public interface IVisitContainerAdapter<TContainer, TValue>
    {
        VisitStatus BeginContainer<TProperty>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, TValue>;

        void EndContainer<TProperty>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, TValue>;
    }

    public interface IVisitContainerAdapterC<TContainer>
    {
        VisitStatus BeginContainer<TProperty, TValue>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, TValue>;

        void EndContainer<TProperty, TValue>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, TValue>;
    }

    public interface IVisitContainerAdapter<TValue>
    {
        VisitStatus BeginContainer<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, TValue>;

        void EndContainer<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, TValue>;
    }

    /// <summary>
    /// Implement this interface to intercept the visitation for all unhandled container nodes.
    /// </summary>
    public interface IVisitContainerAdapter
    {
        /// <summary>
        /// Invoked when the visitor encounters any container node.
        /// </summary>
        VisitStatus BeginContainer<TProperty, TValue, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, TValue>;

        void EndContainer<TProperty, TValue, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, TValue>;
    }

    public interface IVisitCollectionAdapter<TContainer, TValue>
    {
        VisitStatus BeginCollection<TProperty>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            where TProperty : ICollectionProperty<TContainer, TValue>;

        void EndCollection<TProperty>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            where TProperty : ICollectionProperty<TContainer, TValue>;
    }

    public interface IVisitCollectionAdapter<TValue>
    {
        VisitStatus BeginCollection<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            where TProperty : ICollectionProperty<TContainer, TValue>;

        void EndCollection<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            where TProperty : ICollectionProperty<TContainer, TValue>;
    }

    public interface IVisitCollectionAdapterC<TContainer>
    {
        VisitStatus BeginCollection<TProperty, TValue>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            where TProperty : ICollectionProperty<TContainer, TValue>;

        void EndCollection<TProperty, TValue>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            where TProperty : ICollectionProperty<TContainer, TValue>;
    }

    public interface IVisitCollectionAdapter
    {
        VisitStatus BeginCollection<TProperty, TContainer, TValue>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            where TProperty : ICollectionProperty<TContainer, TValue>;

        void EndCollection<TProperty, TContainer, TValue>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            where TProperty : ICollectionProperty<TContainer, TValue>;
    }

    public interface IVisitAdapterPrimitives :
        IVisitAdapter<sbyte>,
        IVisitAdapter<short>,
        IVisitAdapter<int>,
        IVisitAdapter<long>,
        IVisitAdapter<byte>,
        IVisitAdapter<ushort>,
        IVisitAdapter<uint>,
        IVisitAdapter<ulong>,
        IVisitAdapter<float>,
        IVisitAdapter<double>,
        IVisitAdapter<bool>,
        IVisitAdapter<char>
    {
    }

    public interface IPropertyVisitorAdapter
    {

    }

    public static class PropertyVisitorAdapterExtensions
    {
        public static VisitStatus TryVisitValue<TProperty, TContainer, TValue>(this IPropertyVisitorAdapter self, IPropertyVisitor visitor, TProperty property, ref TContainer container,
            ref TValue value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, TValue>
        {
            VisitStatus status;

            if (self is IVisitAdapter<TContainer, TValue> visitAdapterTypedContainerValue)
            {
                if ((status = visitAdapterTypedContainerValue.Visit(visitor, property, ref container, ref value, ref changeTracker)) != VisitStatus.Unhandled)
                {
                    return status;
                }
            }

            if (self is IVisitAdapter<TValue> visitAdapterTypedValue)
            {
                if ((status = visitAdapterTypedValue.Visit(visitor, property, ref container, ref value, ref changeTracker)) != VisitStatus.Unhandled)
                {
                    return status;
                }
            }

            if (self is IVisitAdapter visitAdapter)
            {
                if ((status = visitAdapter.Visit(visitor, property, ref container, ref value, ref changeTracker)) != VisitStatus.Unhandled)
                {
                    return status;
                }
            }

            return VisitStatus.Unhandled;
        }

        public static VisitStatus TryVisitContainer<TProperty, TContainer, TValue>(this IPropertyVisitorAdapter self, IPropertyVisitor visitor, TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, TValue>
        {
            VisitStatus status;

            if (self is IVisitAdapter<TContainer, TValue> visitAdapterTypedContainerValue)
            {
                if ((status = visitAdapterTypedContainerValue.Visit(visitor, property, ref container, ref value, ref changeTracker)) != VisitStatus.Unhandled)
                {
                    return status;
                }
            }

            if (self is IVisitAdapter<TValue> visitAdapterTypedValue)
            {
                if ((status = visitAdapterTypedValue.Visit(visitor, property, ref container, ref value, ref changeTracker)) != VisitStatus.Unhandled)
                {
                    return status;
                }
            }

            if (self is IVisitContainerAdapter<TContainer, TValue> visitContainerAdapterTypedContainerValue)
            {
                if ((status = visitContainerAdapterTypedContainerValue.BeginContainer(visitor, property, ref container, ref value, ref changeTracker)) == VisitStatus.Handled)
                {
                    PropertyContainer.Visit(ref value, visitor, ref changeTracker);
                }

                visitContainerAdapterTypedContainerValue.EndContainer(visitor, property, ref container, ref value, ref changeTracker);

                if (status != VisitStatus.Unhandled)
                {
                    return status;
                }
            }

            if (self is IVisitContainerAdapterC<TContainer> visitContainerAdapterTypedContainer)
            {
                if ((status = visitContainerAdapterTypedContainer.BeginContainer(visitor, property, ref container, ref value, ref changeTracker)) == VisitStatus.Handled)
                {
                    PropertyContainer.Visit(ref value, visitor, ref changeTracker);
                }

                visitContainerAdapterTypedContainer.EndContainer(visitor, property, ref container, ref value, ref changeTracker);

                if (status != VisitStatus.Unhandled)
                {
                    return status;
                }
            }

            if (self is IVisitContainerAdapter<TValue> visitContainerAdapterTypedValue)
            {
                if ((status = visitContainerAdapterTypedValue.BeginContainer(visitor, property, ref container, ref value, ref changeTracker)) == VisitStatus.Handled)
                {
                    PropertyContainer.Visit(ref value, visitor, ref changeTracker);
                }

                visitContainerAdapterTypedValue.EndContainer(visitor, property, ref container, ref value, ref changeTracker);

                if (status != VisitStatus.Unhandled)
                {
                    return status;
                }
            }

            if (self is IVisitContainerAdapter visitContainerAdapter)
            {
                if ((status = visitContainerAdapter.BeginContainer(visitor, property, ref container, ref value, ref changeTracker)) == VisitStatus.Handled)
                {
                    PropertyContainer.Visit(ref value, visitor, ref changeTracker);
                }

                visitContainerAdapter.EndContainer(visitor, property, ref container, ref value, ref changeTracker);

                if (status != VisitStatus.Unhandled)
                {
                    return status;
                }
            }

            return VisitStatus.Unhandled;
        }

        static void VisitCollectionElements<TProperty, TContainer, TValue>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ChangeTracker changeTracker)
            where TProperty : ICollectionProperty<TContainer, TValue>
        {
            for (int i = 0, count = property.GetCount(ref container); i < count; i++)
            {
                var elementChangeTracker = new ChangeTracker(changeTracker.VersionStorage);
                var callback = new VisitCollectionElementCallback<TContainer>(visitor);

                property.GetPropertyAtIndex(ref container, i, ref elementChangeTracker, ref callback);

                if (elementChangeTracker.IsChanged())
                {
                    changeTracker.IncrementVersion<TProperty, TContainer, TValue>(property, ref container);
                }
            }
        }

        public static VisitStatus TryVisitCollection<TProperty, TContainer, TValue>(this IPropertyVisitorAdapter self, IPropertyVisitor visitor, TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            where TProperty : ICollectionProperty<TContainer, TValue>
        {
            VisitStatus status;

            if (self is IVisitAdapter<TContainer, TValue> visitAdapterTypedContainerValue)
            {
                if ((status = visitAdapterTypedContainerValue.Visit(visitor, property, ref container, ref value, ref changeTracker)) != VisitStatus.Unhandled)
                {
                    return status;
                }
            }

            if (self is IVisitAdapter<TValue> visitAdapterTypedValue)
            {
                if ((status = visitAdapterTypedValue.Visit(visitor, property, ref container, ref value, ref changeTracker)) != VisitStatus.Unhandled)
                {
                    return status;
                }
            }

            if (self is IVisitCollectionAdapter<TContainer, TValue> visitCollectionAdapterTypedContainerValue)
            {
                if ((status = visitCollectionAdapterTypedContainerValue.BeginCollection(visitor, property, ref container, ref value, ref changeTracker)) == VisitStatus.Handled)
                {
                    VisitCollectionElements<TProperty, TContainer, TValue>(visitor, property, ref container, changeTracker);
                }

                visitCollectionAdapterTypedContainerValue.EndCollection(visitor, property, ref container, ref value, ref changeTracker);

                if (status != VisitStatus.Unhandled)
                {
                    return status;
                }
            }

            if (self is IVisitCollectionAdapterC<TContainer> visitCollectionAdapterTypedContainer)
            {
                if ((status = visitCollectionAdapterTypedContainer.BeginCollection(visitor, property, ref container, ref value, ref changeTracker)) == VisitStatus.Handled)
                {
                    VisitCollectionElements<TProperty, TContainer, TValue>(visitor, property, ref container, changeTracker);
                }

                visitCollectionAdapterTypedContainer.EndCollection(visitor, property, ref container, ref value, ref changeTracker);

                if (status != VisitStatus.Unhandled)
                {
                    return status;
                }
            }

            if (self is IVisitCollectionAdapter<TValue> visitCollectionAdapterTypedValue)
            {
                if ((status = visitCollectionAdapterTypedValue.BeginCollection(visitor, property, ref container, ref value, ref changeTracker)) == VisitStatus.Handled)
                {
                    VisitCollectionElements<TProperty, TContainer, TValue>(visitor, property, ref container, changeTracker);
                }

                visitCollectionAdapterTypedValue.EndCollection(visitor, property, ref container, ref value, ref changeTracker);

                if (status != VisitStatus.Unhandled)
                {
                    return status;
                }
            }

            if (self is IVisitCollectionAdapter visitCollectionAdapter)
            {
                if ((status = visitCollectionAdapter.BeginCollection(visitor, property, ref container, ref value, ref changeTracker)) == VisitStatus.Handled)
                {
                    VisitCollectionElements<TProperty, TContainer, TValue>(visitor, property, ref container, changeTracker);
                }

                visitCollectionAdapter.EndCollection(visitor, property, ref container, ref value, ref changeTracker);

                if (status != VisitStatus.Unhandled)
                {
                    return status;
                }
            }

            return VisitStatus.Unhandled;
        }
    }
}
