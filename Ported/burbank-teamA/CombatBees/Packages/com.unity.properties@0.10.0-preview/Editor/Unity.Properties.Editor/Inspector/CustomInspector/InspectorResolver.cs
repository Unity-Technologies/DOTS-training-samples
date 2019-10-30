using System;
using System.Linq;

namespace Unity.Properties.Editor
{
    interface IInspectorResolver
    {
        bool Resolve(Type inspectorType);
    }
    
    readonly struct NoPropertyDrawers : IInspectorResolver
    {
        static readonly Type BaseType = typeof(IPropertyDrawer<>);
        public bool Resolve(Type inspectorType)
        {
            return !inspectorType.GetInterfaces().Any(x =>
                x.IsGenericType
                && x.GetGenericTypeDefinition() == BaseType);
        }
    }
    
    readonly struct WithPropertyDrawers : IInspectorResolver
    {
        static readonly Type BaseType = typeof(IPropertyDrawer<>);
        readonly Type Type;

        public WithPropertyDrawers(Type type)
        {
            Type = type;
        }
            
        public bool Resolve(Type inspectorType)
        {
            var type = Type;
            return inspectorType.GetInterfaces().Any(x =>
                x.IsGenericType
                && x.GetGenericTypeDefinition() == BaseType
                && x.GetGenericArguments()[0] == type);
        }
    }
}