using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Properties;

namespace Unity.Entities
{
    public interface IDynamicBufferContainer
    {
        Type ElementType { get; }
    }

    public struct DynamicBufferContainer<T> : IDynamicBufferContainer
    {
        static DynamicBufferContainer()
        {
            PropertyBagResolver.Register(new PropertyBag());
        }

        private unsafe uint Buffer => (uint)m_Buffer;
        private readonly unsafe byte* m_Buffer;
        private readonly int m_Length;
        private readonly int m_Size;
        private readonly bool m_IsReadOnly;

        public int Length => m_Length;

        public Type ElementType => typeof(T);

        public override int GetHashCode()
        {
            return (int)math.hash(new uint2(Buffer, (uint)m_Length));
        }

        public unsafe DynamicBufferContainer(void* buffer, int length, int size, bool isReadOnly)
        {
            m_Buffer = (byte*) buffer;
            m_Length = length;
            m_Size = size;
            m_IsReadOnly = isReadOnly;
        }

        private class PropertyBag : PropertyBag<DynamicBufferContainer<T>>
        {
            private struct ElementsProperty : ICollectionProperty<DynamicBufferContainer<T>, IEnumerable<T>>
            {
                private readonly bool m_IsReadOnly;
                
                public string GetName() => "Elements";
                public bool IsReadOnly => true;
                public bool IsContainer => false;
                public IPropertyAttributeCollection Attributes => null;

                public ElementsProperty(bool isReadOnly)
                {
                    m_IsReadOnly = isReadOnly;
                }
                
                public IEnumerable<T> GetValue(ref DynamicBufferContainer<T> container) => default;
                public void SetValue(ref DynamicBufferContainer<T> container, IEnumerable<T> value) => throw new InvalidOperationException("Property is ReadOnly");
                public int GetCount(ref DynamicBufferContainer<T> container) => container.Length;
                public void SetCount(ref DynamicBufferContainer<T> container, int count) => throw new InvalidOperationException("Property is ReadOnly");
                public void Clear(ref DynamicBufferContainer<T> container) => throw new InvalidOperationException("Property is ReadOnly");

                public void GetPropertyAtIndex<TGetter>(ref DynamicBufferContainer<T> container, int index, ref ChangeTracker changeTracker, ref TGetter getter) where TGetter : ICollectionElementPropertyGetter<DynamicBufferContainer<T>>
                {
                    getter.VisitProperty<BufferElementProperty, T>(new BufferElementProperty(index, m_IsReadOnly), ref container, ref changeTracker);
                }
            }

            private struct BufferElementProperty : ICollectionElementProperty<DynamicBufferContainer<T>, T>
            {
                private readonly int m_Index;
                private readonly bool m_IsReadOnly;
                
                public string GetName() => $"[{m_Index}]";

                public int Index => m_Index;
                public bool IsReadOnly => m_IsReadOnly;
                public bool IsContainer => true;
                public IPropertyAttributeCollection Attributes => null;

                public BufferElementProperty(int index, bool isReadOnly)
                {
                    m_Index = index;
                    m_IsReadOnly = isReadOnly;
                }
                
                public unsafe T GetValue(ref DynamicBufferContainer<T> container)
                {
                    var ptr = container.m_Buffer + container.m_Size * Index;
                    return System.Runtime.CompilerServices.Unsafe.AsRef<T>(ptr);
                }

                public unsafe void SetValue(ref DynamicBufferContainer<T> container, T value)
                {
                    if (m_IsReadOnly)
                    {
                        throw new NotSupportedException("Property is ReadOnly");
                    }
                    
                    var ptr = container.m_Buffer + container.m_Size * Index;
                    System.Runtime.CompilerServices.Unsafe.Copy(ptr, ref value);
                }
            }

            public override void Accept<TVisitor>(ref DynamicBufferContainer<T> container, ref TVisitor visitor, ref ChangeTracker changeTracker)
            {
                visitor.VisitCollectionProperty<ElementsProperty, DynamicBufferContainer<T>, IEnumerable<T>>(new ElementsProperty(container.m_IsReadOnly), ref container, ref changeTracker);
            }

            public override bool FindProperty<TAction>(string name, ref DynamicBufferContainer<T> container, ref ChangeTracker changeTracker, ref TAction action)
            {
                if (name.Equals("Elements"))
                {
                    action.VisitCollectionProperty<ElementsProperty, IEnumerable<T>>(new ElementsProperty(container.m_IsReadOnly), ref container, ref changeTracker);
                    return true;
                }

                return false;
            }
        }
    }
}
