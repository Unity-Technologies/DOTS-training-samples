using System;

namespace Unity.Properties
{
    public struct PropertyContainerConstructOptions
    {
        public string TypeIdentifierKey;
    }

    public static partial class PropertyContainer
    {
        struct ConstructAbstractType<TSrcContainer> : IContainerTypeCallback
        {
            public PropertyContainerConstructOptions Options;
            public VisitResult Result;
            public TSrcContainer SrcContainer;
            public object DstContainerBoxed;

            public void Invoke<TDstContainer>()
            {
                var visitor = new TypeConstructionVisitor<TDstContainer>((TDstContainer) DstContainerBoxed, Result, Options);
                Visit(ref SrcContainer, ref visitor);
                DstContainerBoxed = visitor.Target;
            }
        }

        public static VisitResult Construct<TDstContainer, TSrcContainer>(ref TDstContainer dstContainer, ref TSrcContainer srcContainer, PropertyContainerConstructOptions options = default)
        {
            if (!RuntimeTypeInfoCache<TSrcContainer>.IsValueType() && srcContainer == null)
            {
                throw new ArgumentNullException(nameof(srcContainer));
            }
            
            if (!RuntimeTypeInfoCache<TDstContainer>.IsValueType() && dstContainer == null)
            {
                if (typeof(UnityEngine.Object).IsAssignableFrom(typeof(TDstContainer)))
                {
                    throw new ArgumentNullException(nameof(dstContainer));
                }
                
                if (!TypeConstruction.TryConstruct(srcContainer.GetType(), out dstContainer))
                {
                    throw new ArgumentNullException(nameof(dstContainer));
                }
            }
            
            var result = VisitResult.GetPooled();
            Construct(ref dstContainer, ref srcContainer, result, options);
            return result;
        }

        internal static void Construct<TDstContainer, TSrcContainer>(
            ref TDstContainer dstContainer,
            ref TSrcContainer srcContainer,
            VisitResult result,
            PropertyContainerConstructOptions options = default)
        {
            if (RuntimeTypeInfoCache<TDstContainer>.IsAbstractOrInterface() || typeof(TDstContainer) != dstContainer.GetType())
            {
                var propertyBag = PropertyBagResolver.Resolve(dstContainer.GetType());
                var action = new ConstructAbstractType<TSrcContainer>
                {
                    Options = options,
                    Result = result,
                    SrcContainer = srcContainer,
                    DstContainerBoxed = dstContainer
                };
                propertyBag.Cast(ref action);
                dstContainer = (TDstContainer) action.DstContainerBoxed;
            }
            else
            {
                var visitor = new TypeConstructionVisitor<TDstContainer>(dstContainer, result, options);
                Visit(ref srcContainer, ref visitor);
                dstContainer = visitor.Target;
            }
        }
    }
    
    struct TypeConstructionVisitor<TDstContainer> : IPropertyVisitor
    {
        TDstContainer m_DstContainer;
        readonly PropertyContainerConstructOptions m_Options;
        readonly VisitResult Result;
        readonly IPropertyBag<TDstContainer> m_DstPropertyBag;
        public TDstContainer Target => m_DstContainer;
        
        public TypeConstructionVisitor(TDstContainer dstContainer, VisitResult result, PropertyContainerConstructOptions options)
        {
            m_Options = options;
            Result = result;
            m_DstContainer = dstContainer;
            m_DstPropertyBag = PropertyBagResolver.Resolve<TDstContainer>();

            if (null == m_DstPropertyBag)
                throw new ArgumentException($"No property bag exists for the given Type=[{typeof(TDstContainer)}]");
        }
        
        public VisitStatus VisitProperty<TSrcProperty, TSrcContainer, TSrcValue>(
            TSrcProperty srcProperty,
            ref TSrcContainer srcContainer,
            ref ChangeTracker changeTracker)
            where TSrcProperty : IProperty<TSrcContainer, TSrcValue>
        {
            var action = new ConstructContainer<TSrcValue> 
            {
                Options = m_Options,
                Result = Result,
                SrcValue = srcProperty.GetValue(ref srcContainer)
            };
            
            m_DstPropertyBag.FindProperty(
                srcProperty.GetName(),
                ref m_DstContainer,
                ref changeTracker,
                ref action);

            return VisitStatus.Handled;
        }

        public VisitStatus VisitCollectionProperty<TSrcProperty, TSrcContainer, TSrcValue>(
            TSrcProperty srcProperty,
            ref TSrcContainer srcContainer,
            ref ChangeTracker changeTracker)
            where TSrcProperty : ICollectionProperty<TSrcContainer, TSrcValue>
        {
            var action = new ConstructCollection<TSrcProperty, TSrcContainer, TSrcValue> 
            {
                Options = m_Options,
                Result = Result,
                SrcProperty = srcProperty,
                SrcContainer = srcContainer,
                SrcValue = srcProperty.GetValue(ref srcContainer)
            };
            
            m_DstPropertyBag.FindProperty(
                srcProperty.GetName(),
                ref m_DstContainer,
                ref changeTracker,
                ref action);

            return VisitStatus.Handled;
        }
        
        struct ConstructContainer<TSrcValue> : IPropertyGetter<TDstContainer>
        {
            public PropertyContainerConstructOptions Options;
            public VisitResult Result;
            public TSrcValue SrcValue;

            public void VisitProperty<TDstProperty, TDstValue>(
                TDstProperty dstProperty,
                ref TDstContainer dstContainer,
                ref ChangeTracker changeTracker)
                where TDstProperty : IProperty<TDstContainer, TDstValue>
            {
                if (!dstProperty.IsContainer)
                {
                    return;
                }
                
                if (!RuntimeTypeInfoCache<TSrcValue>.IsValueType() && null == SrcValue)
                {
                    dstProperty.SetValue(ref dstContainer, default);
                    return;
                }
                
                var dstValue = dstProperty.GetValue(ref dstContainer);

                if (!RuntimeTypeInfoCache<TDstValue>.IsValueType() && null == dstValue || SrcValue is TDstValue && dstValue.GetType() != SrcValue.GetType())
                {
                    if (!TypeConstructionUtility.TryConstructFromData(ref SrcValue, Options.TypeIdentifierKey, Result, out dstValue))
                    {
                        return;
                    }
                }

                PropertyContainer.Construct(ref dstValue, ref SrcValue, Result, Options);

                dstProperty.SetValue(ref dstContainer, dstValue);
            }

            public void VisitCollectionProperty<TDstProperty, TDstValue>(
                TDstProperty dstProperty,
                ref TDstContainer dstContainer,
                ref ChangeTracker changeTracker)
                where TDstProperty : ICollectionProperty<TDstContainer, TDstValue>
            {
                Result.AddException(new InvalidOperationException($"PropertyContainer.Construct ContainerType=[{typeof(TDstContainer)}] PropertyName=[{dstProperty.GetName()}] expected container type but was collection type."));
            }
        }

        struct ConstructCollection<TSrcProperty, TSrcContainer, TSrcValue> : IPropertyGetter<TDstContainer>
            where TSrcProperty : ICollectionProperty<TSrcContainer, TSrcValue>
        {
            public PropertyContainerConstructOptions Options;
            public VisitResult Result;
            public TSrcProperty SrcProperty;
            public TSrcContainer SrcContainer;
            public TSrcValue SrcValue;

            public void VisitProperty<TDstProperty, TDstValue>(
                TDstProperty dstProperty, 
                ref TDstContainer dstContainer, 
                ref ChangeTracker changeTracker) 
                where TDstProperty : IProperty<TDstContainer, TDstValue>
            {
                Result.AddException(new InvalidOperationException($"PropertyContainer.Construct ContainerType=[{typeof(TDstContainer)}] PropertyName=[{dstProperty.GetName()}] expected collection type but was container type."));
            }

            public void VisitCollectionProperty<TDstProperty, TDstValue>(
                TDstProperty dstProperty, 
                ref TDstContainer dstContainer, 
                ref ChangeTracker changeTracker) 
                where TDstProperty : ICollectionProperty<TDstContainer, TDstValue>
            {
                if (!RuntimeTypeInfoCache<TSrcValue>.IsValueType() && null == SrcValue)
                {
                    dstProperty.SetValue(ref dstContainer, default);
                    return;
                }
                
                var dstValue = dstProperty.GetValue(ref dstContainer);
                
                if (!RuntimeTypeInfoCache<TDstValue>.IsValueType() && null == dstValue)
                {
                    if (typeof(UnityEngine.Object).IsAssignableFrom(typeof(TDstValue)))
                    {
                        return;
                    }
                    
                    if (TypeConstruction.TryConstruct(SrcValue.GetType(), out dstValue))
                    {
                        dstProperty.SetValue(ref dstContainer, dstValue);
                    }
                    else if (TypeConstruction.TryConstruct(out dstValue))
                    {
                        dstProperty.SetValue(ref dstContainer, dstValue);
                    }
                    else if (typeof(TDstValue).IsArray)
                    {
                        dstValue = (TDstValue) Activator.CreateInstance(typeof(TDstValue), SrcProperty.GetCount(ref SrcContainer));
                        dstProperty.SetValue(ref dstContainer, dstValue);
                    }
                }
                
                var srcCount = SrcProperty.GetCount(ref SrcContainer);
                var dstCount = dstProperty.GetCount(ref dstContainer);
                
                if (srcCount != dstCount)
                {
                    dstProperty.SetCount(ref dstContainer, srcCount);
                }
                
                for (var i = 0; i < srcCount; i++)
                {
                    var action = new SrcCollectionElementGetter<TDstProperty, TDstValue>
                    {
                        Options = Options,
                        Result = Result,
                        DstProperty = dstProperty,
                        DstContainer = dstContainer,
                        Index = i
                    };
                    
                    SrcProperty.GetPropertyAtIndex(ref SrcContainer, i, ref changeTracker, ref action);

                    dstContainer = action.DstContainer;
                }
            }

            struct SrcCollectionElementGetter<TDstProperty, TDstValue> : ICollectionElementPropertyGetter<TSrcContainer>
                where TDstProperty : ICollectionProperty<TDstContainer, TDstValue>
            {
                public PropertyContainerConstructOptions Options;
                public VisitResult Result;
                public TDstProperty DstProperty;
                public TDstContainer DstContainer;
                public int Index;
                
                public void VisitProperty<TSrcElementProperty, TSrcElementValue>(
                    TSrcElementProperty srcElementProperty, 
                    ref TSrcContainer srcContainer, 
                    ref ChangeTracker changeTracker) 
                    where TSrcElementProperty : ICollectionElementProperty<TSrcContainer, TSrcElementValue>
                {
                    var action = new DstCollectionElementGetter<TSrcElementValue>
                    {
                        Options = Options,
                        Result = Result,
                        SrcElementValue = srcElementProperty.GetValue(ref srcContainer)
                    };
                    
                    DstProperty.GetPropertyAtIndex(ref DstContainer, Index, ref changeTracker, ref action);
                }

                public void VisitCollectionProperty<TSrcElementProperty, TSrcElementValue>(
                    TSrcElementProperty srcElementProperty, 
                    ref TSrcContainer srcContainer, 
                    ref ChangeTracker changeTracker) 
                    where TSrcElementProperty : ICollectionProperty<TSrcContainer, TSrcElementValue>, ICollectionElementProperty<TSrcContainer, TSrcElementValue>
                {
                    Result.AddException(new InvalidOperationException("PropertyContainer.Construct does not support arrays of arrays."));
                }
            }

            struct DstCollectionElementGetter<TSrcElementValue> : ICollectionElementPropertyGetter<TDstContainer>
            {
                public PropertyContainerConstructOptions Options;
                public VisitResult Result;
                public TSrcElementValue SrcElementValue;

                public void VisitProperty<TDstElementProperty, TDstElementValue>(
                    TDstElementProperty dstElementProperty, 
                    ref TDstContainer dstContainer, 
                    ref ChangeTracker changeTracker) 
                    where TDstElementProperty : ICollectionElementProperty<TDstContainer, TDstElementValue>
                {
                    if (!dstElementProperty.IsContainer)
                    {
                        return;
                    }
                
                    if (!RuntimeTypeInfoCache<TSrcElementValue>.IsValueType() && null == SrcElementValue)
                    {
                        dstElementProperty.SetValue(ref dstContainer, default);
                        return;
                    }
                
                    var dstValue = dstElementProperty.GetValue(ref dstContainer);

                    if (!RuntimeTypeInfoCache<TDstElementValue>.IsValueType() && null == dstValue || SrcElementValue is TDstElementValue && dstValue.GetType() != SrcElementValue.GetType())
                    {
                        if (!TypeConstructionUtility.TryConstructFromData(ref SrcElementValue, Options.TypeIdentifierKey, Result, out dstValue))
                        {
                            return;
                        }
                    }

                    PropertyContainer.Construct(ref dstValue, ref SrcElementValue, Result, Options);

                    dstElementProperty.SetValue(ref dstContainer, dstValue);
                }

                public void VisitCollectionProperty<TDstElementProperty, TDstElementValue>(
                    TDstElementProperty dstElementProperty, 
                    ref TDstContainer dstContainer, 
                    ref ChangeTracker changeTracker) 
                    where TDstElementProperty : ICollectionProperty<TDstContainer, TDstElementValue>, ICollectionElementProperty<TDstContainer, TDstElementValue>
                {
                    Result.AddException(new InvalidOperationException($"PropertyContainer.Construct ContainerType=[{typeof(TDstContainer)}] PropertyName=[{dstElementProperty.GetName()}] expected collection type but was container type."));
                }
            }
        }
    }

    static class TypeConstructionUtility
    {
        public static bool TryConstructFromData<TDstValue, TSrcValue>(ref TSrcValue srcValue, string typeIdentifierKey, VisitResult result, out TDstValue dstValue)
        {
            if (typeof(UnityEngine.Object).IsAssignableFrom(typeof(TDstValue)))
            {
                dstValue = default;
                return false;
            }
            
            // First we try to construct based on the source type.
            if (TypeConstruction.TryConstruct(srcValue.GetType(), out dstValue))
            {
                return true;
            }
            
            // If that fails we try to construct based on the destination type.
            if (TypeConstruction.TryConstruct(out dstValue))
            {
                return true;
            }

            if (string.IsNullOrEmpty(typeIdentifierKey))
            {
                // We have no meta data string to look for. We can not construct this type.
                return false;
            }

            // If that fails, we try to construct base on the meta data string.
            if (!PropertyContainer.TryGetValue(ref srcValue, typeIdentifierKey, out string assemblyQualifiedTypeName))
            {
                result.AddException(new InvalidOperationException($"PropertyContainer.Construct failed to construct DstType=[{typeof(TDstValue)}]. SrcValue Property=[{typeIdentifierKey}] was not found."));
                return false;
            }

            if (string.IsNullOrEmpty(assemblyQualifiedTypeName))
            {
                result.AddException(new InvalidOperationException($"PropertyContainer.Construct failed to construct DstType=[{typeof(TDstValue)}]. SrcValue Property=[{typeIdentifierKey}] contained null or empty type information."));
                return false;
            }

            var dstType = Type.GetType(assemblyQualifiedTypeName);
            
            if (null == dstType)
            {
                result.AddException(new InvalidOperationException($"PropertyContainer.Construct failed to construct DstType=[{typeof(TDstValue)}]. Could not resolve type from TypeName=[{assemblyQualifiedTypeName}]."));
                return false;
            }
            
            return TypeConstruction.TryConstruct(dstType, out dstValue);
        }
    }
}