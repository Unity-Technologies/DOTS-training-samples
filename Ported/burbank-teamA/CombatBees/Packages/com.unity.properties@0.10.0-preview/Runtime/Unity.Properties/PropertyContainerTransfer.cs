using System;

namespace Unity.Properties
{
    public static partial class PropertyContainer
    {
        public static VisitResult Transfer<TDstContainer, TSrcContainer>(TDstContainer destination, TSrcContainer source)
            where TDstContainer : class
        {
            return Transfer(ref destination, ref source);
        }
        
        public static VisitResult Transfer<TDstContainer, TSrcContainer>(ref TDstContainer dstContainer, ref TSrcContainer srcContainer, IVersionStorage versionStorage)
        {
            return Transfer(ref dstContainer, ref srcContainer);
        }
        
        public static VisitResult Transfer<TDstContainer, TSrcContainer>(ref TDstContainer dstContainer, ref TSrcContainer srcContainer, ref ChangeTracker changeTracker)
        {
            return Transfer(ref dstContainer, ref srcContainer);
        }

        public static VisitResult Transfer<TDstContainer, TSrcContainer>(ref TDstContainer dstContainer, ref TSrcContainer srcContainer)
        {
            if (!RuntimeTypeInfoCache<TDstContainer>.IsValueType() && dstContainer == null)
            {
                throw new ArgumentNullException(nameof(dstContainer));
            }

            var result = VisitResult.GetPooled();
            Transfer(ref dstContainer, ref srcContainer, result);
            return result;
        }

        static void Transfer<TDstContainer, TSrcContainer>(
            ref TDstContainer dstContainer,
            ref TSrcContainer srcContainer,
            VisitResult result)
        {
            if (RuntimeTypeInfoCache<TDstContainer>.IsAbstractOrInterface() || typeof(TDstContainer) != dstContainer.GetType())
            {
                var propertyBag = PropertyBagResolver.Resolve(dstContainer.GetType());
                var action = new TransferAbstractType<TSrcContainer>
                {
                    Result = result,
                    SrcContainer = srcContainer,
                    DstContainerBoxed = dstContainer
                };
                propertyBag.Cast(ref action);
                dstContainer = (TDstContainer) action.DstContainerBoxed;
            }
            else
            {
                var visitor = new TransferVisitor<TDstContainer>(dstContainer, result);
                Visit(ref srcContainer, ref visitor);
                dstContainer = visitor.Target;
            }
        }
        
        struct TransferAbstractType<TSrcContainer> : IContainerTypeCallback
        {
            public VisitResult Result;
            public TSrcContainer SrcContainer;
            public object DstContainerBoxed;

            public void Invoke<TDstContainer>()
            {
                var visitor = new TransferVisitor<TDstContainer>((TDstContainer) DstContainerBoxed, Result);
                Visit(ref SrcContainer, ref visitor);
                DstContainerBoxed = visitor.Target;
            }
        }

        struct TransferVisitor<TDstContainer> : IPropertyVisitor
        {
            TDstContainer m_DstContainer;
            VisitResult m_Result;
            readonly IPropertyBag<TDstContainer> m_DstPropertyBag;
            public TDstContainer Target => m_DstContainer;

            public TransferVisitor(TDstContainer dstContainer, VisitResult result)
            {
                m_Result = result;
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
                var action = new TransferContainer<TSrcValue>
                {
                    Result = m_Result,
                    SrcValue = srcProperty.GetValue(ref srcContainer)
                };

                var sourcePropertyName = srcProperty.GetName();
                if (m_DstPropertyBag.FindProperty(
                    sourcePropertyName,
                    ref m_DstContainer,
                    ref changeTracker,
                    ref action))
                {
                    return VisitStatus.Handled;
                }

                var visitor = new FormerlySerializedAsVisitor(sourcePropertyName);
                Visit(ref m_DstContainer, ref visitor);
                if (!string.IsNullOrEmpty(visitor.CurrentName))
                {
                    m_DstPropertyBag.FindProperty(
                        visitor.CurrentName,
                        ref m_DstContainer,
                        ref changeTracker,
                        ref action);
                }

                return VisitStatus.Handled;
            }

            public VisitStatus VisitCollectionProperty<TSrcProperty, TSrcContainer, TSrcValue>(
                TSrcProperty srcProperty,
                ref TSrcContainer srcContainer,
                ref ChangeTracker changeTracker)
                where TSrcProperty : ICollectionProperty<TSrcContainer, TSrcValue>
            {
                var action = new TransferCollection<TSrcProperty, TSrcContainer, TSrcValue>
                {
                    Result = m_Result,
                    SrcProperty = srcProperty,
                    SrcContainer = srcContainer,
                    SrcValue = srcProperty.GetValue(ref srcContainer)
                };

                var sourcePropertyName = srcProperty.GetName();
                if (m_DstPropertyBag.FindProperty(
                    sourcePropertyName,
                    ref m_DstContainer,
                    ref changeTracker,
                    ref action))
                {
                    return VisitStatus.Handled;
                }
                
                var visitor = new FormerlySerializedAsVisitor(sourcePropertyName);
                Visit(ref m_DstContainer, ref visitor);
                if (!string.IsNullOrEmpty(visitor.CurrentName))
                {
                    m_DstPropertyBag.FindProperty(
                        visitor.CurrentName,
                        ref m_DstContainer,
                        ref changeTracker,
                        ref action);
                }

                return VisitStatus.Handled;
            }

            struct TransferContainer<TSrcValue> : IPropertyGetter<TDstContainer>
            {
                public VisitResult Result;
                public TSrcValue SrcValue;

                public void VisitProperty<TDstProperty, TDstValue>(
                    TDstProperty dstProperty,
                    ref TDstContainer dstContainer,
                    ref ChangeTracker changeTracker)
                    where TDstProperty : IProperty<TDstContainer, TDstValue>
                {
                    if (!RuntimeTypeInfoCache<TSrcValue>.IsValueType() && null == SrcValue)
                    {
                        dstProperty.SetValue(ref dstContainer, default);
                    }
                    else if (TypeConversion.TryConvert<TSrcValue, TDstValue>(SrcValue, out var dstValue))
                    {
                        dstProperty.SetValue(ref dstContainer, dstValue);
                    }
                    else if (dstProperty.IsContainer)
                    {
                        dstValue = dstProperty.GetValue(ref dstContainer);
                        
                        if (RuntimeTypeInfoCache<TDstValue>.IsValueType() || null != dstValue)
                        {
                            Transfer(ref dstValue, ref SrcValue, Result);
                        }
                        
                        dstProperty.SetValue(ref dstContainer, dstValue);
                    }
                    else
                    {
                        Result.AddLog($"PropertyContainer.Transfer ContainerType=[{typeof(TDstContainer)}] PropertyName=[{dstProperty.GetName()}] could not be transferred.");
                    }
                }

                public void VisitCollectionProperty<TDstProperty, TDstValue>(
                    TDstProperty dstProperty,
                    ref TDstContainer dstContainer,
                    ref ChangeTracker changeTracker)
                    where TDstProperty : ICollectionProperty<TDstContainer, TDstValue>
                {
                    Result.AddException(new InvalidOperationException($"PropertyContainer.Transfer ContainerType=[{typeof(TDstContainer)}] PropertyName=[{dstProperty.GetName()}] expected container type but was collection type."));
                }
            }

            struct TransferCollection<TSrcProperty, TSrcContainer, TSrcValue> : IPropertyGetter<TDstContainer>
                where TSrcProperty : ICollectionProperty<TSrcContainer, TSrcValue>
            {
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
                    Result.AddException(new InvalidOperationException($"PropertyContainer.Transfer ContainerType=[{typeof(TDstContainer)}] PropertyName=[{dstProperty.GetName()}] expected collection type but was container type."));
                }

                public void VisitCollectionProperty<TDstProperty, TDstValue>(
                    TDstProperty dstProperty,
                    ref TDstContainer dstContainer,
                    ref ChangeTracker changeTracker)
                    where TDstProperty : ICollectionProperty<TDstContainer, TDstValue>
                {
                    var dstValue = dstProperty.GetValue(ref dstContainer);
                    
                    if (!RuntimeTypeInfoCache<TSrcValue>.IsValueType() && null == SrcValue)
                    {
                        dstProperty.SetValue(ref dstContainer, default);
                    }
                    else if (RuntimeTypeInfoCache<TSrcValue>.IsValueType() || null != dstValue)
                    {
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
                                Result = Result,
                                DstProperty = dstProperty,
                                DstContainer = dstContainer,
                                Index = i
                            };

                            SrcProperty.GetPropertyAtIndex(ref SrcContainer, i, ref changeTracker, ref action);

                            dstContainer = action.DstContainer;
                        }
                    }
                    else
                    {
                        Result.AddLog($"PropertyContainer.Transfer ContainerType=[{typeof(TDstContainer)}] PropertyName=[{dstProperty.GetName()}] could not be transferred.");
                    }
                }

                struct SrcCollectionElementGetter<TDstProperty, TDstValue> : ICollectionElementPropertyGetter<TSrcContainer>
                    where TDstProperty : ICollectionProperty<TDstContainer, TDstValue>
                {
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
                        Result.AddException(new InvalidOperationException("PropertyContainer.Transfer does not support arrays of arrays."));
                    }
                }

                struct DstCollectionElementGetter<TSrcElementValue> : ICollectionElementPropertyGetter<TDstContainer>
                {
                    public VisitResult Result; 
                    public TSrcElementValue SrcElementValue;

                    public void VisitProperty<TDstElementProperty, TDstElementValue>(
                        TDstElementProperty dstElementProperty,
                        ref TDstContainer dstContainer,
                        ref ChangeTracker changeTracker)
                        where TDstElementProperty : ICollectionElementProperty<TDstContainer, TDstElementValue>
                    {
                        if (!RuntimeTypeInfoCache<TSrcElementValue>.IsValueType() && null == SrcElementValue)
                        {
                            dstElementProperty.SetValue(ref dstContainer, default);
                        }
                        else if (TypeConversion.TryConvert<TSrcElementValue, TDstElementValue>(SrcElementValue, out var dstElementValue))
                        {
                            dstElementProperty.SetValue(ref dstContainer, dstElementValue);
                        }
                        else if (dstElementProperty.IsContainer)
                        {
                            dstElementValue = dstElementProperty.GetValue(ref dstContainer);
                        
                            if (RuntimeTypeInfoCache<TDstElementValue>.IsValueType() || null != dstElementValue)
                            {
                                Transfer(ref dstElementValue, ref SrcElementValue, Result);
                            }
                        
                            dstElementProperty.SetValue(ref dstContainer, dstElementValue);
                        }
                        else
                        {
                            Result.AddLog($"PropertyContainer.Transfer ContainerType=[{typeof(TDstContainer)}] PropertyName=[{dstElementProperty.GetName()}] could not be transferred.");
                        }
                    }

                    public void VisitCollectionProperty<TDstElementProperty, TDstElementValue>(
                        TDstElementProperty dstElementProperty,
                        ref TDstContainer dstContainer,
                        ref ChangeTracker changeTracker)
                        where TDstElementProperty : ICollectionProperty<TDstContainer, TDstElementValue>, ICollectionElementProperty<TDstContainer, TDstElementValue>
                    {
                        Result.AddException(new InvalidOperationException($"PropertyContainer.Transfer ContainerType=[{typeof(TDstContainer)}] PropertyName=[{dstElementProperty.GetName()}] expected collection type but was container type."));
                    }
                }
            }
        }
    }
}
