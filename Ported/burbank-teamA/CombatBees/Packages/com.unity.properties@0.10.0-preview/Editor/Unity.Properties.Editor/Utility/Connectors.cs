using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Properties.Editor
{
    public struct Connector<TFieldValue, TValue>
    {
        public delegate TValue ToValueDelegate(TFieldValue fieldValue);
        public delegate TFieldValue ToFieldDelegate(TValue value);

        public ToValueDelegate ToValue;
        public ToFieldDelegate ToField;
    }
    
    interface IDataConnector
    {
        bool MatchesExactly<T>(VisualElement element, T value);
        bool MatchesUpdaterType<T>(VisualElement element, T value);
        void SetData<T>(VisualElement element, T value);
        void RegisterCallback<T>(VisualElement element, T value);
        void UnregisterCallback<T>(VisualElement element, T value);
    }

    static class Connectors
    {
        public static TValue Identity<TValue>(TValue value) => value;
        public static TValue Cast<TBase, TValue>(TBase value) where TValue : TBase => (TValue) value;
        public static TValue UnsafeCast<TBase, TValue>(TBase value) => (TValue) (object) value;
        public static TValue NoOp<TBase, TValue>(TBase value) => default;
        
        class Entry
        {
            public Type m_Type;
            public IDataConnector m_Connector;
        }

        static readonly List<Entry> s_Entries = new List<Entry>();

        public static void Register(Type type, IDataConnector connector)
        {
            s_Entries.Add(new Entry {m_Type = type, m_Connector = connector});
        }

        public static void SetData<TValueType>(VisualElement element, TValueType value)
        {
            GetConnector(element, value)?.m_Connector.SetData(element, value);
        }

        public static void SetCollectionSize(IntegerField element, int size)
        {
            if (element?.focusController?.focusedElement != element)
            {
                element.isDelayed = false;
                element.SetValueWithoutNotify(size);
                element.isDelayed = true;
            }
        }
        
        public static void RegisterCallback<TValueType>(VisualElement element, TValueType value)
        {
            GetConnector(element, value)?.m_Connector.RegisterCallback(element, value);
        }

        public static void UnregisterCallback<TValueType>(VisualElement element, TValueType value)
        {
            GetConnector(element, value)?.m_Connector.UnregisterCallback(element, value);
        }

        static Entry GetConnector<TValueType>(VisualElement element, TValueType value)
        {
            var candidates = s_Entries.Where(e => e.m_Connector.MatchesUpdaterType(element, value)).ToArray();
            var bestCandidate = candidates.FirstOrDefault(e => e.m_Connector.MatchesExactly(element, value));
            if (null != bestCandidate)
            {
                return bestCandidate;
            }

            return candidates.Length > 0 
                ? candidates[0] 
                : null;
        }
    }

    public static class ConnectorFactory<TValue>
    {
        static readonly List<IDataConnector> m_AvailableTranslators = new List<IDataConnector>();

        class DataConnector<TElement, TFieldType> : IDataConnector
            where TElement : BaseField<TFieldType>, INotifyValueChanged<TFieldType>
        {
            internal readonly Connector<TFieldType, TValue> m_Connector;

            public DataConnector(Connector<TFieldType, TValue> connector)
            {
                m_Connector = connector;
            }

            public bool MatchesExactly<T>(VisualElement element, T value)
            {
                // We use `TValue` here because we want to match the underlying data.
                return element.GetType() == typeof(TElement) && typeof(T) == typeof(TValue);
            }

            public bool MatchesUpdaterType<T>(VisualElement element, T value)
            {
                if (element is BaseField<TFieldType> || (value?.GetType()?.IsAssignableFrom(typeof(TFieldType)) ?? false))
                {
                    return true;
                }

                return element.GetType().IsAssignableFrom(typeof(TElement)) &&
                       value is TValue;
            }

            public void SetData<T>(VisualElement element, T value)
            {
                if (!(element is BaseField<TFieldType> field))
                {
                    return;
                }

                if (!TypeConversion.TryConvert(value, out TValue data))
                {
                    data = (TValue) (object) value;
                }
                SetDataGeneric(field, data);
            }

            public void RegisterCallback<T>(VisualElement element, T value)
            {
                if (!(element is BaseField<TFieldType> field))
                {
                    return;
                }
                if (!TypeConversion.TryConvert(value, out TValue data))
                {
                    data = (TValue) (object) value;
                }
                RegisterGenericCallback(field, data);
            }

            public void UnregisterCallback<T>(VisualElement element, T value)
            {
                if (!(element is BaseField<TFieldType> field))
                {
                    return;
                }
                if (!TypeConversion.TryConvert(value, out TValue data))
                {
                    data = (TValue) (object) value;
                }
                UnregisterGenericCallback(field, data);
            }

            void SetDataGeneric(BaseField<TFieldType> field, TValue value)
            {
                if (field?.focusController?.focusedElement != field)
                {
                    var delayed = GetDelayed(field);
                    SetDelayed(field, false);
                    field.SetValueWithoutNotify(m_Connector.ToField(value));
                    SetDelayed(field, delayed);
                }
            }

            bool GetDelayed(VisualElement element)
            {
                if (element is TextInputBaseField<TFieldType> input)
                {
                    return input.isDelayed;
                }

                return false;
            }
            
            void SetDelayed(VisualElement element, bool delayed)
            {
                if (element is TextInputBaseField<TFieldType> input)
                {
                    input.isDelayed = delayed;
                }
            }

            void RegisterGenericCallback(BaseField<TFieldType> field, TValue value)
            {
                field.RegisterValueChangedCallback(Changed);

                if (field is ObjectField objectField && objectField.objectType == null)
                {
                    objectField.objectType = typeof(TValue);
                }
            }

            void UnregisterGenericCallback(BaseField<TFieldType> field, TValue value)
            {
                field.UnregisterValueChangedCallback(Changed);
            }
            
            void Changed(ChangeEvent<TFieldType> evt)
            {
                var field = evt.target as BaseField<TFieldType>;
                var value = evt.newValue;
                var context = field?.GetFirstAncestorOfType<PropertyElement>();
                if (null == context)
                {
                    return;
                }

                var newValue = context.SetValue(field, m_Connector.ToValue(value));
                field.SetValueWithoutNotify(m_Connector.ToField(newValue));
            }
        }
        
        public static void Register<TElement>()
            where TElement : BaseField<TValue>, INotifyValueChanged<TValue>
        {
            var updater = new DataConnector<TElement, TValue>(new Connector<TValue, TValue>()
            {
                ToValue = Connectors.Identity,
                ToField = Connectors.Identity
            });
            m_AvailableTranslators.Add(updater);
            Connectors.Register(typeof(TValue), updater);
        }

        public static void Register<TElement, TFieldType>(Connector<TFieldType, TValue> connector)
            where TElement : BaseField<TFieldType>, INotifyValueChanged<TFieldType>
        {
            var updater = new DataConnector<TElement, TFieldType>(connector);
            m_AvailableTranslators.Add(updater);
            Connectors.Register(typeof(TValue), updater);
        }
        
        internal static Connector<TFieldType, TValue> GetExact<TElement, TFieldType>()
            where TElement : BaseField<TFieldType>, INotifyValueChanged<TFieldType>
        {
            return m_AvailableTranslators
                .OfType<DataConnector<TElement, TFieldType>>()
                .FirstOrDefault()?.m_Connector ?? default;
        }
    }

    [InitializeOnLoad]
    public static class ConnectorFactory
    {
        static ConnectorFactory()
        {
            ConnectorFactory<string>.Register<BaseField<string>>();
            ConnectorFactory<bool>.Register<BaseField<bool>>();
            ConnectorFactory<int>.Register<BaseField<int>>();
            ConnectorFactory<long>.Register<BaseField<long>>();
            ConnectorFactory<float>.Register<BaseField<float>>();
            ConnectorFactory<double>.Register<BaseField<double>>();
            ConnectorFactory<UnityEngine.Object>.Register<ObjectField>();
            ConnectorFactory<Enum>.Register<BaseField<Enum>>();
            ConnectorFactory<Color>.Register<BaseField<Color>>();
            ConnectorFactory<Rect>.Register<BaseField<Rect>>();
            ConnectorFactory<Gradient>.Register<BaseField<Gradient>>();

            ConnectorFactory<sbyte>.Register<BaseField<int>, int>(new Connector<int, sbyte>
            {
                ToValue = v => (sbyte) Mathf.Clamp(v, sbyte.MinValue, sbyte.MaxValue),
                ToField = v => (int) v
            });

            ConnectorFactory<byte>.Register<BaseField<int>, int>(new Connector<int, byte>
            {
                ToValue = v => (byte) Mathf.Clamp(v, byte.MinValue, byte.MaxValue),
                ToField = v => (int) v
            });

            ConnectorFactory<ushort>.Register<BaseField<int>, int>(new Connector<int, ushort>
            {
                ToValue = v => (ushort) Mathf.Clamp(v, ushort.MinValue, ushort.MaxValue),
                ToField = v => (int) v
            });

            ConnectorFactory<short>.Register<BaseField<int>, int>(new Connector<int, short>
            {
                ToValue = v => (short) Mathf.Clamp(v, short.MinValue, short.MaxValue),
                ToField = v => (int) v
            });

            ConnectorFactory<uint>.Register<BaseField<long>, long>(new Connector<long, uint>
            {
                ToValue = v => (uint) Mathf.Clamp(v, uint.MinValue, uint.MaxValue),
                ToField = v => (long) v
            });

            ConnectorFactory<ulong>.Register<BaseField<string>, string>(new Connector<string, ulong>
            {
                ToValue = v =>
                {
                    ulong.TryParse(v, out var num);
                    return num;
                }, 
                ToField = v => v.ToString()
            });

            ConnectorFactory<int>.Register<BaseField<float>, float>(new Connector<float, int>
            {
                ToValue = v => (int) v,
                ToField = v => (float) v
            });

            ConnectorFactory<char>.Register<BaseField<string>, string>(new Connector<string, char>
            {
                ToValue = v =>
                {
                    if (string.IsNullOrEmpty(v))
                    {
                        return '\0';
                    }

                    return v[0];
                },
                ToField = v => v.ToString()
            });
        }
    }
}
