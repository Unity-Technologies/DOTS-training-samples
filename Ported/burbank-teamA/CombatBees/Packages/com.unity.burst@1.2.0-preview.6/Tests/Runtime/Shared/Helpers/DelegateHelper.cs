#if BURST_TESTS_ONLY
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace Burst.Compiler.IL.Tests.Helpers
{
    internal static class DelegateHelper
    {
        private static readonly Type[] _DelegateCtorSignature = new Type[2]
        {
            typeof(object),
            typeof(IntPtr)
        };

        private static readonly Dictionary<DelegateKey, Type> DelegateTypes = new Dictionary<DelegateKey, Type>();

        public static Type NewDelegateType(Type ret, Type[] parameters)
        {
            var key = new DelegateKey(ret, (Type[])parameters.Clone());
            Type delegateType;
            if (!DelegateTypes.TryGetValue(key, out delegateType))
            {
                var assemblyName = Guid.NewGuid().ToString();

                var name = new AssemblyName(assemblyName);
                var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
                var moduleBuilder = assemblyBuilder.DefineDynamicModule(name.Name);
                assemblyBuilder.DefineVersionInfoResource();

                var typeBuilder = moduleBuilder.DefineType("CustomDelegate", System.Reflection.TypeAttributes.Public | System.Reflection.TypeAttributes.Sealed | System.Reflection.TypeAttributes.AutoClass, typeof(MulticastDelegate));
                var constructor = typeof(UnmanagedFunctionPointerAttribute).GetConstructors()[0];

                // Make sure that we setup the C calling convention on the unmanaged delegate
                var customAttribute = new CustomAttributeBuilder(constructor, new object[] { CallingConvention.Cdecl });
                typeBuilder.SetCustomAttribute(customAttribute);
                typeBuilder.DefineConstructor(System.Reflection.MethodAttributes.Public | System.Reflection.MethodAttributes.HideBySig | System.Reflection.MethodAttributes.RTSpecialName, CallingConventions.Standard, _DelegateCtorSignature).SetImplementationFlags(System.Reflection.MethodImplAttributes.CodeTypeMask);
                typeBuilder.DefineMethod("Invoke", System.Reflection.MethodAttributes.Public | System.Reflection.MethodAttributes.Virtual | System.Reflection.MethodAttributes.HideBySig | System.Reflection.MethodAttributes.VtableLayoutMask, ret, parameters).SetImplementationFlags(System.Reflection.MethodImplAttributes.CodeTypeMask);
                delegateType = typeBuilder.CreateType();

                DelegateTypes.Add(key, delegateType);
            }
            return delegateType;
        }

        private struct DelegateKey : IEquatable<DelegateKey>
        {
            public DelegateKey(Type returnType, Type[] arguments)
            {
                ReturnType = returnType;
                Arguments = arguments;
            }

            public readonly Type ReturnType;

            public readonly Type[] Arguments;

            public bool Equals(DelegateKey other)
            {
                if (ReturnType.Equals(other.ReturnType) && Arguments.Length == other.Arguments.Length)
                {
                    for (int i = 0; i < Arguments.Length; i++)
                    {
                        if (Arguments[i] != other.Arguments[i])
                        {
                            return false;
                        }
                    }
                    return true;
                }
                return false;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is DelegateKey && Equals((DelegateKey) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hashcode = (ReturnType.GetHashCode() * 397) ^ Arguments.Length.GetHashCode();
                    for (int i = 0; i < Arguments.Length; i++)
                    {
                        hashcode = (hashcode * 397) ^ Arguments[i].GetHashCode();
                    }
                    return hashcode;
                }
            }
        }
    }
}
#endif