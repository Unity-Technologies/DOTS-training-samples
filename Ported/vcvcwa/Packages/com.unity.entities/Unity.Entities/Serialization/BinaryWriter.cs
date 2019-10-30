#if !NET_DOTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Properties;

[assembly: InternalsVisibleTo("Unity.Scenes.Hybrid")]
namespace Unity.Entities.Serialization
{
    static class BoxedProperties
    {
        class ReadBoxedStructForwarder : IContainerTypeCallback
        {
            public object Container;
            public PropertiesBinaryReader reader;

            public void Invoke<T>()
            {
                var value = default(T);
                PropertyContainer.Visit(ref value, reader);
                Container = value;
            }
        }

        class ReadBoxedClassForwarder : IContainerTypeCallback
        {
            public object Container;
            public PropertiesBinaryReader reader;

            public void Invoke<T>()
            {
                var value = (T) Activator.CreateInstance(typeof(T));
                PropertyContainer.Visit(ref value, reader);
                Container = value;
            }
        }

        public static object ReadBoxedStruct(Type type, PropertiesBinaryReader reader)
        {
            var forwarder = new ReadBoxedStructForwarder {reader = reader, Container = null};
            var propertyBag = PropertyBagResolver.Resolve(type);

            propertyBag.Cast(ref forwarder);

            return forwarder.Container;
        }

        public static object ReadBoxedClass(Type type, PropertiesBinaryReader reader)
        {
            var forwarder = new ReadBoxedClassForwarder {reader = reader, Container = null};
            var propertyBag = PropertyBagResolver.Resolve(type);

            propertyBag.Cast(ref forwarder);

            return forwarder.Container;
        }

        public static void WriteBoxedType<TVisitor>(object container, TVisitor visitor)
            where TVisitor : IPropertyVisitor
        {
            var changeTracker = new ChangeTracker();
            var resolved = PropertyBagResolver.Resolve(container.GetType());
            if (resolved != null)
            {
                resolved.Accept(ref container, ref visitor, ref changeTracker);
            }
            else
                throw new ArgumentException("Not supported");
        }
    }

    internal struct DictionaryContainer<TKey, TValue>
    {
        public List<TKey> Keys;
        public List<TValue> Values;

        public void PopulateDictionary(Dictionary<TKey, TValue> dict)
        {
            Assertions.Assert.AreEqual(Keys.Count, Values.Count);

            for (int i = 0; i < Keys.Count; ++i)
            {
                dict.Add(Keys[i], Values[i]);
            }
        }
    }

    class BasePropertyVisitor : PropertyVisitor
    {
        internal static readonly uint kMagicNull = 0xDEADC0DE;
    }

    unsafe class PropertiesBinaryWriter : BasePropertyVisitor
    {
        protected BinaryPrimitiveWriterAdapter PrimitiveWriter { get; }
        public ref UnsafeAppendBuffer Buffer
        {
            get { return ref *PrimitiveWriter.m_Buffer; }
        }


        // This whole file is marked as !NET_DOTS but once Unity.Properties is supported in NET_DOTS
        // that top-level ifdef will be removed but this inner ifdef should not be as this code is for hybrid only
#if !NET_DOTS
        private List<UnityEngine.Object> _ObjectTable = new List<UnityEngine.Object>();
        private Dictionary<UnityEngine.Object, int> _ObjectTableMap = new Dictionary<UnityEngine.Object, int>();

        public UnityEngine.Object[] GetObjectTable()
        {
            return _ObjectTable.ToArray();
        }

        void AppendObject(UnityEngine.Object obj, ref UnsafeAppendBuffer stream)
        {
            int index = -1;
            if (obj != null)
            {
                if (!_ObjectTableMap.TryGetValue(obj, out index))
                {
                    index = _ObjectTable.Count;
                    _ObjectTableMap.Add(obj, index);
                    _ObjectTable.Add(obj);
                }
            }

            stream.Add(index);
        }
#endif

        public PropertiesBinaryWriter(UnsafeAppendBuffer* stream)
        {
            PrimitiveWriter = new BinaryPrimitiveWriterAdapter(stream);
            AddAdapter(PrimitiveWriter);
        }

        private void VisitDictionary<TKey, TValue>(ICollection<TKey> keys, ICollection<TValue> values)
        {
            var dictContainer = new DictionaryContainer<TKey, TValue>() { Keys = new List<TKey>(keys), Values = new List<TValue>(values) };
            PropertyContainer.Visit(ref dictContainer, this);
        }

        protected override VisitStatus BeginContainer<TProperty, TContainer, TValue>
            (TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
        {
            if (typeof(System.Collections.IDictionary).IsAssignableFrom(typeof(TValue)))
            {
                var dict = value as System.Collections.IDictionary;
                var keys = dict.Keys;
                var values = dict.Values;
                var tKey = typeof(TValue).GetGenericArguments()[0];
                var tValue = typeof(TValue).GetGenericArguments()[1];

                // Workaround to support Dictionaries since Unity.Properties doesn't contain a ReflectedDictionaryProperty nor is it currently setup 
                // to easily be extended to support Key-Value container types. As such we treat dictionaries as two list (keys and values) by 
                // making our own container type to visit which we populate with the two lists we care about
                var openMethod = typeof(PropertiesBinaryWriter).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Single(m => m.Name == "VisitDictionary");
                var closedMethod = openMethod.MakeGenericMethod(tKey, tValue);
                closedMethod.Invoke(this, new[] { keys, values });

                return VisitStatus.Override;
            }
#if !NET_DOTS
            else if (typeof(UnityEngine.Object).IsAssignableFrom(typeof(TValue)))
            {
                AppendObject(value as UnityEngine.Object, ref Buffer);
                return VisitStatus.Override;
            }
#endif
            else if(value == null)
            {
                Buffer.Add(kMagicNull);
                return VisitStatus.Override;
            }

            return base.BeginContainer(property, ref container, ref value, ref changeTracker);
        }

        protected override VisitStatus BeginCollection<TProperty, TContainer, TValue>
            (TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
        {
            // Write out size of list required for deserializing later
            var count = property.GetCount(ref container);
            Buffer.Add(count);

            return VisitStatus.Handled;
        }
    }

    class PropertiesBinaryReader : BasePropertyVisitor
    {
        protected BinaryPrimitiveReaderAdapter _PrimitiveReader;

#if !NET_DOTS
        private UnityEngine.Object[] _ObjectTable;

        unsafe UnityEngine.Object ReadObject(UnsafeAppendBuffer.Reader* stream)
        {
            stream->ReadNext(out int index);
            if (index != -1)
                return _ObjectTable[index];
            else
                return null;
        }

        unsafe public PropertiesBinaryReader(UnsafeAppendBuffer.Reader* stream, UnityEngine.Object[] objectTable)
        {
            _PrimitiveReader = new BinaryPrimitiveReaderAdapter(stream);
            _ObjectTable = objectTable;
            AddAdapter(_PrimitiveReader);
        }
#else
        unsafe public PropertiesBinaryReader(UnsafeAppendBuffer.Reader* stream)
        {
            _PrimitiveReader = new BinaryPrimitiveReaderAdapter(stream);
            AddAdapter(_PrimitiveReader);
        }
#endif


        private DictionaryContainer<TKey, TValue> VisitDictionary<TKey, TValue>(ICollection<TKey> keys, ICollection<TValue> values)
        {
            var dictContainer = new DictionaryContainer<TKey, TValue>() { Keys = new List<TKey>(keys), Values = new List<TValue>(values) };
            PropertyContainer.Visit(ref dictContainer, this);

            return dictContainer;
        }

        unsafe protected override VisitStatus BeginContainer<TProperty, TContainer, TValue>
            (TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
        {
            if (typeof(System.Collections.IDictionary).IsAssignableFrom(typeof(TValue)))
            {
                InitializeNullIfFieldNotNull(ref value);

                var dict = value as System.Collections.IDictionary;
                var keys = dict.Keys;
                var values = dict.Values;
                var tKey = typeof(TValue).GetGenericArguments()[0];
                var tValue = typeof(TValue).GetGenericArguments()[1];

                // Workaround to support Dictionaries since Unity.Properties doesn't contain a ReflectedDictionaryProperty nor is it currently setup 
                // to easily be extended to support Key-Value container types. As such we treat dictionaries as two list (keys and values) by 
                // making our own container type to visit which we populate with the two lists we care about
                var openVisitMethod = typeof(PropertiesBinaryReader).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Single(m => m.Name == "VisitDictionary");
                var closedVisitMethod = openVisitMethod.MakeGenericMethod(tKey, tValue);
                var dictContainer = closedVisitMethod.Invoke(this, new[] { keys, values });

                // now fill our actual dict with the read in kv lists
                var closedDictContainerType = typeof(DictionaryContainer<,>).MakeGenericType(tKey, tValue);
                var populateMethod = closedDictContainerType.GetMethods(BindingFlags.Public | BindingFlags.Instance).Single(m => m.Name == "PopulateDictionary");
                populateMethod.Invoke(dictContainer, new object[] { value });

                return VisitStatus.Override;
            }
#if !NET_DOTS
            else if (typeof(UnityEngine.Object).IsAssignableFrom(typeof(TValue)))
            {
                if (_ObjectTable == null)
                    throw new ArgumentException("We are reading a UnityEngine.Object however no ObjectTable was provided to the PropertiesBinaryReader.");

                var unityObject = ReadObject(_PrimitiveReader.Buffer);
                Unsafe.As<TValue, UnityEngine.Object>(ref value) = unityObject;


                //Debug.Log($"BeginContainer : {obj}" );
                return VisitStatus.Override;
            }
            else if (value == null)
            {
                if (InitializeNullIfFieldNotNull(ref value))
                {
                    // The value really was null so just continue
                    return VisitStatus.Override;
                }
                // the value isn't null so fallthrough so we can try visiting it
            }
#endif

            return base.BeginContainer(property, ref container, ref value, ref changeTracker);
        }

        unsafe protected override VisitStatus BeginCollection<TProperty, TContainer, TValue>
            (TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
        {
            // Unity.Properties doesn't really support class types so we need to workaround
            // this issue for now by prefilling our list with instances which will be later replaced.
            // Properties assumes a value type (non-null) value will already be there to write 
            // and if not will try to create a default value (which will be null for class types)
            _PrimitiveReader.Buffer->ReadNext(out int count);

            var type = typeof(TValue);
            if (type.IsArray)
            {
                var tValue = type.GetElementType();
                Array array;
                array = Array.CreateInstance(tValue, count);

                for (int i = 0; i < count; ++i)
                {
                    // Strings are immutable so we need to give a value when creating them
                    if (typeof(string) == tValue)
                        array.SetValue(Activator.CreateInstance(tValue, "".ToCharArray()), 1);
                    if (typeof(UnityEngine.Object).IsAssignableFrom(tValue))
                    {
                        // do nothing
                    }
                    else
                    {
                        if (!tValue.IsValueType &&
                            !tValue.GetConstructors().Any(c => c.GetParameters().Count() == 0))
                        {
                            throw new ArgumentException(
                                $"All class component types must be default constructable. '{tValue.FullName}' is missing a default constructor.");
                        }

                        array.SetValue(Activator.CreateInstance(tValue), i);
                    }
                }

                value = (TValue) (object) array;
            }
            else if(type.IsGenericType)
            {
                var tValue = type.GetGenericArguments()[0];
                System.Collections.IList list;
                if (null == value)
                {
                    list = (System.Collections.IList) Activator.CreateInstance(typeof(List<>).MakeGenericType(tValue));
                }
                else
                {
                    list = value as System.Collections.IList;
                }

                for (int i = 0; i < count; ++i)
                {
                    // Strings are immutable so we need to give a value when creating them
                    if (typeof(string) == tValue)
                        list.Add(Activator.CreateInstance(tValue, "".ToCharArray()));
                    else if (typeof(UnityEngine.Object).IsAssignableFrom(tValue))
                    {
                        list.Add(null);
                    }
                    else
                    {
                        if (!tValue.IsValueType &&
                            !tValue.GetConstructors().Any(c => c.GetParameters().Count() == 0))
                        {
                            throw new ArgumentException(
                                $"All class component types must be default constructable. '{tValue.FullName}' is missing a default constructor.");
                        }

                        list.Add(Activator.CreateInstance(tValue));
                    }
                }

                value = (TValue) list;
            }
            else
            {
                //we did nothing
            }

            property.SetValue(ref container, value);
            property.SetCount(ref container, count);

            return VisitStatus.Handled;
        }

        // As properties iterates over containers, if the container doesn't default construct it's fields but we 
        // have serialized out field data for those containers check for that data (absence of our null sentinel) 
        // and then return a default constructed container to be filled by said data
        unsafe bool InitializeNullIfFieldNotNull<TValue>(ref TValue value)
        {
            bool isActuallyNull = true;
            if(value == null)
            {
                var oldOffset = _PrimitiveReader.Buffer->Offset;
                var sentinel = _PrimitiveReader.Buffer->ReadNext<uint>();
                if (sentinel != kMagicNull)
                {
                    isActuallyNull = false;
                    _PrimitiveReader.Buffer->Offset = oldOffset;
                    value = Activator.CreateInstance<TValue>();
                }
            }

            return isActuallyNull;
        }
    }
}
#endif