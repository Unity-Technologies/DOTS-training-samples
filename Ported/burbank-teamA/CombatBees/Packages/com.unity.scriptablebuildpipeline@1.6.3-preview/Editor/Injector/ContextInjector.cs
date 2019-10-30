using System;
using System.Reflection;
using UnityEditor.Build.Pipeline.Interfaces;

namespace UnityEditor.Build.Pipeline.Injector
{
    [AttributeUsage(AttributeTargets.Field)]
    public class InjectContextAttribute : Attribute
    {
        //public string Identifier { get; set; }
        public ContextUsage Usage { get; set; }
        public bool Optional { get; set; }

        public InjectContextAttribute(ContextUsage usage = ContextUsage.InOut, bool optional = false)
        {
            this.Usage = usage;
            Optional = optional;
        }
    }

    public enum ContextUsage
    {
        InOut,
        In,
        Out
    }

    class ContextInjector
    {
        public static void Inject(IBuildContext context, object obj)
        {
            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (FieldInfo field in fields)
            {
                object[] attrs = field.GetCustomAttributes(typeof(InjectContextAttribute), true);
                if (attrs.Length == 0)
                    continue;

                InjectContextAttribute attr = attrs[0] as InjectContextAttribute;
                if (attr == null || attr.Usage == ContextUsage.Out)
                    continue;

                object injectionObject;
                if (field.FieldType == typeof(IBuildContext))
                    injectionObject = context;
                else if (!attr.Optional)
                    injectionObject = context.GetContextObject(field.FieldType);
                else
                {
                    IContextObject contextObject;
                    context.TryGetContextObject(field.FieldType, out contextObject);
                    injectionObject = contextObject;
                }

                field.SetValue(obj, injectionObject);
            }
        }

        public static void Extract(IBuildContext context, object obj)
        {
            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (FieldInfo field in fields)
            {
                object[] attrs = field.GetCustomAttributes(typeof(InjectContextAttribute), true);
                if (attrs.Length == 0)
                    continue;

                InjectContextAttribute attr = attrs[0] as InjectContextAttribute;
                if (attr == null || attr.Usage == ContextUsage.In)
                    continue;

                if (field.FieldType == typeof(IBuildContext))
                    throw new InvalidOperationException("IBuildContext can only be used with the ContextUsage.In option.");

                IContextObject contextObject = field.GetValue(obj) as IContextObject;
                if (!attr.Optional)
                    context.SetContextObject(field.FieldType, contextObject);
                else if (contextObject != null)
                    context.SetContextObject(field.FieldType, contextObject);
            }
        }
    }
}
