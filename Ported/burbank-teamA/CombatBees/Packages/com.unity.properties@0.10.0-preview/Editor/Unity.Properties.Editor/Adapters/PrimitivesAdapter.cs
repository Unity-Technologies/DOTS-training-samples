namespace Unity.Properties.Editor
{
    sealed class PrimitivesAdapter<T> : InspectorAdapter<T>
        , IVisitAdapterPrimitives
        , IVisitAdapter<string>
        , IVisitAdapter
    {
        struct GetPropertyDrawerCallback : IContainerTypeCallback
        {
            readonly InspectorVisitor<T> m_Visitor;
            readonly PropertyPath m_PropertyPath;
            readonly string m_PropertyName;
            readonly InspectorVisitLevel m_VisitLevel;
            readonly IProperty m_Property;
            
            bool Visited { get; set; }

            GetPropertyDrawerCallback(InspectorVisitor<T> visitor, IProperty property, PropertyPath propertyPath, string propertyName,
                InspectorVisitLevel visitLevel)
            {
                m_Visitor = visitor;
                Visited = false;
                m_PropertyPath = propertyPath;
                m_PropertyName = propertyName;
                m_VisitLevel = visitLevel;
                m_Property = property;
            }

            public static bool Execute(object container, InspectorVisitor<T> visitor, IProperty property, PropertyPath path,
                string propertyName, InspectorVisitLevel visitLevel)
            {
                var action = new GetPropertyDrawerCallback(visitor, property, path, propertyName, visitLevel);
                PropertyBagResolver.Resolve(container.GetType()).Cast(ref action);
                return action.Visited;
            }

            public void Invoke<TValueType>()
            {
                Visited = TryGetDrawer<TValueType>(m_Visitor, m_Property, m_PropertyPath, m_PropertyName, m_VisitLevel);;
            }
        }
        
        public PrimitivesAdapter(InspectorVisitor<T> visitor) : base(visitor)
        {
        }

        delegate TElement DrawHandler<in TProperty, TContainer, TValue, out TElement>(
            TProperty property,
            ref TContainer container,
            ref TValue value,
            InspectorVisitorContext visitorContext)
            where TProperty : IProperty<TContainer, TValue>;
        
        VisitStatus IVisitAdapter<sbyte>.Visit<TProperty, TContainer>(
            IPropertyVisitor visitor,
            TProperty property,
            ref TContainer container,
            ref sbyte value,
            ref ChangeTracker changeTracker)
            => VisitPrimitive(property, ref container, ref value, GuiFactory.SByteField);

        VisitStatus IVisitAdapter<short>.Visit<TProperty, TContainer>(
            IPropertyVisitor visitor,
            TProperty property,
            ref TContainer container,
            ref short value, 
            ref ChangeTracker changeTracker)
            => VisitPrimitive(property, ref container, ref value, GuiFactory.ShortField);

        VisitStatus IVisitAdapter<int>.Visit<TProperty, TContainer>(
            IPropertyVisitor visitor,
            TProperty property,
            ref TContainer container,
            ref int value, 
            ref ChangeTracker changeTracker)
            => VisitPrimitive(property, ref container, ref value, GuiFactory.IntField);
        
        VisitStatus IVisitAdapter<long>.Visit<TProperty, TContainer>(
            IPropertyVisitor visitor,
            TProperty property,
            ref TContainer container,
            ref long value,
            ref ChangeTracker changeTracker)
            => VisitPrimitive(property, ref container, ref value, GuiFactory.LongField);
        
        VisitStatus IVisitAdapter<byte>.Visit<TProperty, TContainer>(
            IPropertyVisitor visitor,
            TProperty property,
            ref TContainer container,
            ref byte value,
            ref ChangeTracker changeTracker)
            => VisitPrimitive(property, ref container, ref value, GuiFactory.ByteField);
        
        VisitStatus IVisitAdapter<ushort>.Visit<TProperty, TContainer>(
            IPropertyVisitor visitor,
            TProperty property,
            ref TContainer container,
            ref ushort value,
            ref ChangeTracker changeTracker)
            => VisitPrimitive(property, ref container, ref value, GuiFactory.UShortField);
        
        VisitStatus IVisitAdapter<uint>.Visit<TProperty, TContainer>(
            IPropertyVisitor visitor,
            TProperty property,
            ref TContainer container,
            ref uint value,
            ref ChangeTracker changeTracker)
            => VisitPrimitive(property, ref container, ref value, GuiFactory.UIntField);
        
        VisitStatus IVisitAdapter<ulong>.Visit<TProperty, TContainer>(
            IPropertyVisitor visitor,
            TProperty property,
            ref TContainer container,
            ref ulong value,
            ref ChangeTracker changeTracker)
            => VisitPrimitive(property, ref container, ref value, GuiFactory.ULongField);

        VisitStatus IVisitAdapter<float>.Visit<TProperty, TContainer>(
            IPropertyVisitor visitor,
            TProperty property,
            ref TContainer container,
            ref float value, 
            ref ChangeTracker changeTracker)
            => VisitPrimitive(property, ref container, ref value, GuiFactory.FloatField);
        
        VisitStatus IVisitAdapter<double>.Visit<TProperty, TContainer>(
            IPropertyVisitor visitor,
            TProperty property,
            ref TContainer container,
            ref double value,
            ref ChangeTracker changeTracker)
            => VisitPrimitive(property, ref container, ref value, GuiFactory.DoubleField);
        
        VisitStatus IVisitAdapter<bool>.Visit<TProperty, TContainer>(
            IPropertyVisitor visitor,
            TProperty property,
            ref TContainer container,
            ref bool value,
            ref ChangeTracker changeTracker)
            => VisitPrimitive(property, ref container, ref value, GuiFactory.Toggle);
        
        VisitStatus IVisitAdapter<char>.Visit<TProperty, TContainer>(
            IPropertyVisitor visitor,
            TProperty property,
            ref TContainer container,
            ref char value,
            ref ChangeTracker changeTracker)
            => VisitPrimitive(property, ref container, ref value, GuiFactory.CharField);
        
        VisitStatus IVisitAdapter.Visit<TProperty, TContainer, TValue>(
            IPropertyVisitor visitor,
            TProperty property, 
            ref TContainer container,
            ref TValue value,
            ref ChangeTracker changeTracker)
        {
            if (!typeof(TValue).IsEnum)
            {
                return VisitStatus.Unhandled;
            }

            if (RuntimeTypeInfoCache<TValue>.IsFlagsEnum())
            {
                GuiFactory.FlagsField(property, ref container, ref value, VisitorContext);
            }
            else
            {
                GuiFactory.EnumField(property, ref container, ref value, VisitorContext);
            }
            return VisitStatus.Override;
        }

        VisitStatus IVisitAdapter<string>.Visit<TProperty, TContainer>(
            IPropertyVisitor visitor,
            TProperty property,
            ref TContainer container,
            ref string value,
            ref ChangeTracker changeTracker)
            => VisitPrimitive(property, ref container, ref value, GuiFactory.TextField);

        VisitStatus VisitPrimitive<TProperty, TContainer, TValue, TElement>(
            TProperty property,
            ref TContainer container,
            ref TValue value,
            DrawHandler<TProperty, TContainer, TValue, TElement> handler
        )
            where TProperty : IProperty<TContainer, TValue>
        {
            // Regular value
            var propName = property.GetName();
            var path = Visitor.GetCurrentPath();
            path.Push(propName);
            if (TryGetDrawer(ref value, property, path, propName,
                InspectorVisitLevel.Field))
            {
                return VisitStatus.Override;
            }

            handler(property, ref container, ref value, VisitorContext);
            return VisitStatus.Handled;
        }

        bool TryGetDrawer<TValue>(
            ref TValue value,
            IProperty property,
            PropertyPath path,
            string name,
            InspectorVisitLevel visitLevel)
            => GetPropertyDrawerCallback.Execute(value, Visitor, property, path, name, visitLevel)
               || TryGetDrawer<TValue>(Visitor, property, path, name, visitLevel);
        
        static bool TryGetDrawer<TValue>(
            InspectorVisitor<T> visitor,
            IProperty property,
            PropertyPath propertyPath,
            string propertyName,
            InspectorVisitLevel visitLevel)
        {
            var inspector = CustomInspectorDatabase.GetPropertyDrawer<TValue>(property);
            if (null == inspector)
            {
                return false;
            }
            var proxy = new InspectorContext<TValue>(visitor.VisitorContext.Binding, property, propertyPath, propertyName, visitLevel);
            var customInspector = new CustomInspectorElement<TValue>(inspector, proxy);
            visitor.VisitorContext.Parent.contentContainer.Add(customInspector);
            inspector.Update(proxy);
            return true;
        }
    }
}
