#if !UNITY_DOTSPLAYER
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Unity.Properties;
using UnityEngine;
using FieldAttributes = Mono.Cecil.FieldAttributes;
using MethodAttributes = Mono.Cecil.MethodAttributes;
using ParameterAttributes = Mono.Cecil.ParameterAttributes;
using TypeAttributes = Mono.Cecil.TypeAttributes;

[assembly: InternalsVisibleTo("Unity.Entities.CodeGen.Tests")]
namespace Unity.Entities.CodeGen
{
    /// <summary>
    /// The PropertyBag post-processor's role is to generate Unity.Properties.PropertyBag<T> classes for all component
    /// types which "Unity.Entities may visit". Additionally, the post-processor must ensure the generated PropertyBags
    /// will be registered in the static Unity.PropertyBagResolver before any runtime/editor code may try visiting
    /// those component types.
    ///
    /// For example for the type:
    /// 
    /// namespace Some.Namespace
    /// {
    ///     public class MyComponent : IComponentData
    ///     {
    ///         public int MyInt;
    ///         public List<string> MyList;
    ///         public SomeClass[] MyArray;
    ///     }
    /// }
    ///
    /// We will generate the following 
    /// namespace Unity.Properties.CodeGeneratedPropertyBags
    /// {
    ///     public class Some_Namespace_MyComponentPropertyBag : PropertyBag<Some.Namespace.MyComponent>
    ///     {
    ///     	public static readonly Property<Some.Namespace.MyComponent, int> MyInt = new Property<Some.Namespace.MyComponent, int>("MyInt", MyIntGetter, MyIntSetter);
    ///     	public static readonly ListProperty<Some.Namespace.MyComponent, string> MyList = new ListProperty<Some.Namespace.MyComponent, string>("MyList", MyListGetter, MyListSetter);
    ///         public static readonly ArrayProperty<Some.Namespace.MyComponent, SomeClass> MyArray = new ListProperty<Some.Namespace.MyComponent, SomeClass>("MyArray", MyArrayGetter, MyArraySetter);
    ///     
    ///     	public static int MyIntGetter(ref Some.Namespace.MyComponent container) { return container.MyInt; }
    ///         public static void MyIntSetter(ref Some.Namespace.MyComponent container, int val) { container.MyInt = val; }
    ///     
    ///         public static IList<string> MyListGetter(ref Some.Namespace.MyComponent container) { return container.MyInt; }
    ///         public static void MyListSetter(ref Some.Namespace.MyComponent container, IList<string> val) { container.MyList = (List<string>) val; }
    ///     
    ///         public static SomeClass[] MyArrayGetter(ref Some.Namespace.MyComponent container) { return container.MyArray; }
    ///         public static void MyArraySetter(ref Some.Namespace.MyComponent container, SomeClass[] val) { container.MyInt = val; }
    ///     
    ///     	public override void Accept<TVisitor>(ref Some.Namespace.MyComponent container, ref TVisitor visitor, ref ChangeTracker changeTracker)
    ///     	{
    ///     		visitor.VisitProperty<Property<Some.Namespace.MyComponent, int>, Some.Namespace.MyComponent, int>(MyInt, ref container, ref changeTracker);
    ///     		visitor.VisitCollectionProperty<ListProperty<Some.Namespace.MyComponent, string>, Some.Namespace.MyComponent, IList<string>>(MyList, ref container, ref changeTracker);
    ///     		visitor.VisitCollectionProperty<ArrayProperty<Some.Namespace.MyComponent, SomeClass>, Some.Namespace.MyComponent, SomeClass>(MyArray, ref container, ref changeTracker);
    ///     	}
    ///     
    ///     	public override bool FindProperty<TAction>(string name, ref Some.Namespace.MyComponent container, ref ChangeTracker changeTracker, ref TAction action)
    ///     	{
    ///     		if (string.Equals(name, "MyInt"))
    ///     		{
    ///     			action.VisitProperty<Property<Some.Namespace.MyComponent, int>, int>(MyInt, ref container, ref changeTracker);
    ///     			return true;
    ///     		}
    ///     		if (string.Equals(name, "MyList"))
    ///     		{
    ///     			action.VisitCollectionProperty<ListProperty<Some.Namespace.MyComponent, string>, IList<string>>(MyList, ref container, ref changeTracker);
    ///     			return true;
    ///     		}
    ///     		if (string.Equals(name, "MyArray"))
    ///     		{
    ///     			action.VisitCollectionProperty<ArrayProperty<Some.Namespace.MyComponent, SomeClass>, SomeClass>(MyArray, ref container, ref changeTracker);
    ///     			return true;
    ///     		}
    ///     		return false;
    ///     	}
    ///     }
    /// }
    ///
    /// ## What are the types "Unity.Entities may visit", you ask? ##
    /// We currently use Unity.Properties to generically inspect fields for all managed IComponentData types as well as
    /// ISharedComponentData (managed or not). Importantly, we may attempt to visit these types close to boot, since
    /// we use Unity.Properties for deserializing ISharedComponentData and managed IComponentData, thus we must ensure
    /// that Unity.Properties is ready to handle these types as soon as possible once we boot.
    ///
    /// ## Why code-gen? ##
    /// Unity.Properties provides a ReflectionPropertyBag which allows for visiting type fields without codegen, however
    /// there are a few reasons why this won't work for all use-cases and leaves us to resort to code-gen:
    /// - Reflection may not be available
    /// -- For DOTS Runtime, the System.Reflection API is simply not available -- codegen is the only route
    /// - Reflection is slower
    /// -- Less of a concern since the reflection codepaths are only hit the first time a type is seen, but it is a
    ///    potential risk for a performance spike
    /// - IL2CPP stripping
    /// -- When an IL2CPP player build is made, the linker may strip out symbols not used explicitly by the runtime
    ///    which can mean it will inadvertently strip out the type information / codepaths the ReflectionPropertyBag
    ///    requires for building the appropriate PropertyBag<T> types, and visit them.
    ///
    /// ## What is the cost of this code-gen approach? ##
    /// TLDR; Any assembly with a type "Unity.Entities may visit" will generate new classes, making assemblies fatter 
    /// than they would be otherwise. Compared to normal code footprint size this should be relatively small however.
    ///     ILPostProcessors operate per assembly and are forbidden from modifying other assemblies than the
    /// ICompiledAssembly passed to the Process() method. So for a given assembly we need to generate a PropertyBag<T>
    /// for all types _and all fields_ in said types which may be visited per assembly. While we are confident all
    /// PropertyBag<MySpecialComponent>'s will be only generated once across _all_ assemblies, since we need to generate
    /// PropertyBags for 'MySpecialComponent's fields, we are likely to see PropertyBags for field types duplicated
    /// across all assemblies. For example, if MySpecialComponent is defined in AssemblyA and contains an Entity field,
    /// AssemblyA will contains a generated PropertyBag<MySpecialComponent> and PropertyBag<Entity>. If AssemblyB
    /// contains defines a MyOtherSpecialComponent which also has an Entity field, AssemblyB will then contain a
    /// PropertyBag<MyOtherSpecialComponent> _as well as_ a PropertyBag<Entity> which is identical to the one defined in
    /// AssemblyA.
    ///
    /// ## How are we ensuring types are registered early? ##
    /// The postprocessor will generate a PropertyBagRegistry for each assembly which contains a static function
    /// 'RegisterAllPropertyBags()' which registers all generated property bag types. In order to make sure this
    /// this function is called very early we do a few things:
    /// - For Hybrid player builds, we put the
    /// [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded))] attribute on the method to
    /// have the UnityLinker inject our method into Unity generated setup code just after assemblies have been loaded.
    /// - For Hybrid editor builds, we put the [InitializeOnLoadMethod] attribute on the method to allow our function
    /// to be invoked after a domain reload. NOTE: since we currently don't have the ability to see script defines
    /// in an ILPostProcessor, editor builds actually uses Reflection in the TypeManager.Initialize() to find all
    /// PropertyBagRegistry types and then manually invokes RegisterAllPropertyBags(). This can be removed in 2019.3.0b9
    ///  - Theoretically for DOTS Runtime (once there is DOTSRuntime support for Unity.Properties and Module Initializer
    /// support in IL2CPP, we inject the RegisterAllPropertyBags method into the module intializer of the assembly we
    /// postprocess. IL2CPP will ensure all module initializers are invoked at boot for all assemblies. 
    /// </summary>
    internal class PropertyBagPostProcessor : EntitiesILPostProcessor
    {
        public static readonly string CodeGenNamespace = "Unity.Properties.CodeGeneratedPropertyBagRegistry";
        
        protected override bool PostProcessImpl()
        {
            var propertyBags = GeneratePropertyBags();
            if (propertyBags.Count == 0)
                return false;
            
            GeneratePropertyBagRegistry(propertyBags);

            return true;
        }

        void GeneratePropertyBagRegistry(List<GeneratedPropertyBag> propertyBags)
        {
            var propertBagRegistryDef = new TypeDefinition(CodeGenNamespace,
                "PropertyBagRegistry", TypeAttributes.Class | TypeAttributes.NotPublic,
                AssemblyDefinition.MainModule.ImportReference(typeof(object)));
            AssemblyDefinition.MainModule.Types.Add(propertBagRegistryDef);

            // Declares: "static CodeGeneratedPropertyBagRegistry() { }" (i.e. the static ctor / .cctor)
            var registerPropertyBagsFn = new MethodDefinition("RegisterAllPropertyBags",
                MethodAttributes.Static | MethodAttributes.Public,
                AssemblyDefinition.MainModule.ImportReference(typeof(void)));

            // We need our registration to be triggered as soon as the assemblies are loaded so we do so with the following
            // custom attributes in hybrid. DOTS Player will solve this elsewhere (in TypeRegGen)
#if !UNITY_DOTSPLAYER
            var initializeOnLoadAttribute = GetAttributeToForceEarlyMethodInvocation();
            registerPropertyBagsFn.CustomAttributes.Add(initializeOnLoadAttribute);
#endif

            propertBagRegistryDef.Methods.Add(registerPropertyBagsFn);
            propertBagRegistryDef.IsBeforeFieldInit = true;
            registerPropertyBagsFn.Body.InitLocals = true;
            var il = registerPropertyBagsFn.Body.GetILProcessor();

            var getTypeFromHandle = AssemblyDefinition.MainModule.ImportReference(typeof(Type).GetMethod("GetTypeFromHandle"));
            var registerFn = AssemblyDefinition.MainModule.ImportReference(typeof(PropertyBagResolver).GetMethod("Register", new Type[]{typeof(Type), typeof(IPropertyBag)}));
            
            foreach (var propertyBag in propertyBags)
            {
                AssemblyDefinition.MainModule.Types.Add(propertyBag.GeneratedType);
                var propertyBagCtor = AssemblyDefinition.MainModule.ImportReference(propertyBag.GeneratedType.GetConstructors().First());
                
                // Load the runtime handle for our type so we can convert it to a `Type` in the getTypeFromHandle call
                il.Emit(OpCodes.Ldtoken, AssemblyDefinition.MainModule.ImportReference(propertyBag.ReferenceType));
                il.Emit(OpCodes.Call, getTypeFromHandle);
                il.Emit(OpCodes.Newobj, propertyBagCtor);
                il.Emit(OpCodes.Call, registerFn);
            }

            il.Emit(OpCodes.Ret);
        }
        
        CustomAttribute GetAttributeToForceEarlyMethodInvocation()
        {
            // TODO: Remove the "&& false" in this condition once 2019.3.0b9 is available which will allow
            // us to receive script defines in our ILPP to apply the correct attributes
            CustomAttribute initializeOnLoadAttribute = null;
#if UNITY_EDITOR && false
            var attributeCtor =
                AssemblyDefinition.MainModule.ImportReference(
                    typeof(UnityEditor.InitializeOnLoadMethodAttribute).GetConstructor(Type.EmptyTypes));
            initializeOnLoadAttribute = new CustomAttribute(attributeCtor);
#else
            var attributeCtor =
                AssemblyDefinition.MainModule.ImportReference(
                    typeof(UnityEngine.RuntimeInitializeOnLoadMethodAttribute).GetConstructor(new Type[]
                        {typeof(UnityEngine.RuntimeInitializeLoadType)}));
            initializeOnLoadAttribute = new CustomAttribute(attributeCtor);
            initializeOnLoadAttribute.ConstructorArguments.Add(new CustomAttributeArgument(
                AssemblyDefinition.MainModule.ImportReference(typeof(UnityEngine.RuntimeInitializeLoadType)),
                UnityEngine.RuntimeInitializeLoadType.AfterAssembliesLoaded));
#endif
            return initializeOnLoadAttribute;
        }
        
        void GatherTypes(TypeReference type, HashSet<TypeReference> typeSet)
        {
            if (type.IsPrimitive || type.IsPointer)
                return;
            
            var resolvedType = type.Resolve();
            if (resolvedType == null || resolvedType.IsEnum)
                return;
            
            if (type.IsGenericInstance || type.IsGenericParameter || type.HasGenericParameters)
            {
                // This is a workaround for how Unity.Entities handles properties until Unity.Properties supports
                //  Dictionaries. In Entities we split Dictionaries into two lists, List<Keys>+List<Values> so in
                // order to traverse these types we need to ensure we generate the PropertyBag for these list types
                if (resolvedType.Interfaces.Any(i =>
                    i.InterfaceType.FullName.Contains("System.Collections.Generic.IDictionary`2")))
                {
                    var genericInstance = type as GenericInstanceType;
                    var keyType = genericInstance.GenericArguments[0];
                    var valueType = genericInstance.GenericArguments[1];
                    var openDictionaryContainer = AssemblyDefinition.MainModule.ImportReference(typeof(Serialization.DictionaryContainer<,>));
                    var closedDictionaryContainer = openDictionaryContainer.MakeGenericInstanceType(keyType, valueType);
                    typeSet.Add(closedDictionaryContainer);
                    return;
                }
                
                if(resolvedType.Interfaces.All(i => !i.InterfaceType.FullName.Contains("System.Collections.Generic.IList`1")))
                    return;
            }

            if (!typeSet.Add(type)) 
                return;
            
            foreach(var field in resolvedType.Fields)
            {
                if (field.IsStatic)
                    continue;
                
                GatherTypes(field.FieldType, typeSet); 
            }
        }

        // We will be gathering types from all over, so we want to collapse TypeReferences that refer to the same types
        // which a TypeDefinition does, however TypeReferences keep the generic specifications which we care about. As
        // such we provide name based comparer to allow us to have a unique set of TypeReferences
        class TypeReferenceComparer : IEqualityComparer<TypeReference>
        {
            public bool Equals(TypeReference lhs, TypeReference rhs)
            {
                return lhs.FullName.Equals(rhs.FullName);
            }

            public int GetHashCode(TypeReference obj)
            {
                return obj.FullName.GetHashCode();
            }
        }

        List<GeneratedPropertyBag> GeneratePropertyBags()
        {
            var propertyBags = new List<GeneratedPropertyBag>();
            var propertyBagTypes = new HashSet<TypeReference>(new TypeReferenceComparer());

            var types = AssemblyDefinition.MainModule.GetAllTypes().Where(t => 
                !t.IsInterface && ((t.IsValueType && t.TypeImplements(typeof(ISharedComponentData))) || (!t.IsValueType && t.TypeImplements(typeof(IComponentData)))));
            foreach (var type in types)
                GatherTypes(type, propertyBagTypes);

            foreach (var type in propertyBagTypes)
                propertyBags.Add(GeneratePropertyBag(type));

            return propertyBags;
        }

        internal static GeneratedPropertyBag GeneratePropertyBag(ModuleDefinition module, TypeReference typeRef)
        {
            var propertyBag = new GeneratedPropertyBag(typeRef, module);
            propertyBag.GenerateType();
            return propertyBag; 
        }
        
        GeneratedPropertyBag GeneratePropertyBag(TypeReference typeRef)
        {
            return GeneratePropertyBag(AssemblyDefinition.MainModule, typeRef);
        }

        static string SanitizeName(string name)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var c in name)
            {
                switch (c)
                {
                    case '.':
                    case '+':
                    case '/':
                    case ',':
                    case '`':
                    case '<':
                    case '>':
                    case '[':
                    case ']':
                        stringBuilder.Append('_');
                        break;
                    default:
                        stringBuilder.Append(c);
                        break;
                }
            }
            return stringBuilder.ToString();
        }

        public static string GeneratePropertyBagNameForType(string typeFullName)
        {
            return SanitizeName(typeFullName) + "PropertyBag";
        }
        
        public static string GeneratePropertyBagNameForType(TypeReference typeRef)
        {
            return GeneratePropertyBagNameForType(typeRef.FullName);
        }
        
        internal class GeneratedPropertyBag
        {
            public TypeDefinition GeneratedType;
            public TypeReference ReferenceType;
            private ModuleDefinition Module;
            
            public GeneratedPropertyBag(TypeReference referenceType, ModuleDefinition module)
            {
                ReferenceType = module.ImportReference(referenceType);
                Module = module;
                
                var basePropertyBagOpenDef = module.ImportReference(typeof(PropertyBag<>)); 
                var basePropertyBagClosedDef = basePropertyBagOpenDef.MakeGenericInstanceType(ReferenceType);

                // TypeReference Full names have all kinds of characters in them that are not ok to be translated
                // into names that will need to be consumed by IL2CPP, so we sanitize the names here.
                var propertyBagName = GeneratePropertyBagNameForType(ReferenceType);
                GeneratedType = new TypeDefinition(CodeGenNamespace,
                    propertyBagName,
                    TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.BeforeFieldInit,
                    module.ImportReference(basePropertyBagClosedDef))
                {
                    Scope = ReferenceType.Scope 
                };
            }

            public void GenerateType()
            {
                GenerateCtor(); 
                GenerateStaticFields();
                GenerateAccept();
                GenerateFindProperty();
            }

            void GenerateCtor()
            {
                var propertyBag = Module.ImportReference(typeof(PropertyBag<>));
                var closedPropertyBag = Module.ImportReference(propertyBag.MakeGenericInstanceType(ReferenceType));
                // NOTE: We create our own method reference since this assembly may not reference Unity.Properties on it's own. Thus any attempt
                // to Resolve() a TypeReference from Properties will return null. So instead we create MethodReferences for methods we
                // know will exist ourselves and let the new assembly, which will now include a reference to Properties, resolve at runtime
                var closedPropertyBagCtor = new MethodReference(".ctor", Module.ImportReference(typeof(void)), closedPropertyBag)
                {
                    HasThis = true,
                    ExplicitThis = false,
                    CallingConvention = MethodCallingConvention.Default
                };

                var ctor = new MethodDefinition(".ctor",
                    MethodAttributes.Public | MethodAttributes.HideBySig |
                    MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                    Module.ImportReference((typeof(void))));
                GeneratedType.Methods.Add(ctor);
                
                var il = ctor.Body.GetILProcessor();
                
                // We are implementing from an abstract class so we need to ensure our ctor invokes the parent ctor
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Call, Module.ImportReference(closedPropertyBagCtor));
                il.Emit((OpCodes.Ret));
            }
            
            void GenerateStaticFields()
            {
                var cctor = new MethodDefinition(".cctor",
                    MethodAttributes.Static | MethodAttributes.Public | MethodAttributes.HideBySig |
                    MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                    Module.ImportReference((typeof(void))));
                GeneratedType.Methods.Add(cctor);

                var il = cctor.Body.GetILProcessor();
                
                var openPropertyDef = Module.ImportReference(typeof(Property<,>));
                var openPropertyCtor = Module.ImportReference(typeof(Property<,>).GetConstructors()[0]);
                var openPropertyGetter = Module.ImportReference(typeof(Property<,>.Getter));
                var openPropertySetter = Module.ImportReference(typeof(Property<,>.Setter));
                var openPropertyGetterCtor = Module.ImportReference(typeof(Property<,>.Getter).GetConstructor(new Type[]{typeof(object), typeof(IntPtr)}));
                var openPropertySetterCtor = Module.ImportReference(typeof(Property<,>.Setter).GetConstructor(new Type[]{typeof(object), typeof(IntPtr)}));
                
                var openArrayPropertyDef = Module.ImportReference(typeof(ArrayProperty<,>));
                var openArrayPropertyCtor = Module.ImportReference(typeof(ArrayProperty<,>).GetConstructors()[0]);
                var openArrayPropertyGetter = Module.ImportReference(typeof(ArrayProperty<,>.Getter));
                var openArrayPropertySetter = Module.ImportReference(typeof(ArrayProperty<,>.Setter));
                var openArrayPropertyGetterCtor = Module.ImportReference(typeof(ArrayProperty<,>.Getter).GetConstructor(new Type[]{typeof(object), typeof(IntPtr)}));
                var openArrayPropertySetterCtor = Module.ImportReference(typeof(ArrayProperty<,>.Setter).GetConstructor(new Type[]{typeof(object), typeof(IntPtr)})); 

                var openIList = Module.ImportReference(typeof(IList<>));
                var openListPropertyDef = Module.ImportReference(typeof(ListProperty<,>));
                var openListPropertyCtor = Module.ImportReference(typeof(ListProperty<,>).GetConstructors()[0]);
                var openListPropertyGetter = Module.ImportReference(typeof(ListProperty<,>.Getter));
                var openListPropertySetter = Module.ImportReference(typeof(ListProperty<,>.Setter));
                var openListPropertyGetterCtor = Module.ImportReference(typeof(ListProperty<,>.Getter).GetConstructor(new Type[]{typeof(object), typeof(IntPtr)}));
                var openListPropertySetterCtor = Module.ImportReference(typeof(ListProperty<,>.Setter).GetConstructor(new Type[]{typeof(object), typeof(IntPtr)})); 
                
                var referenceTypeByReferenceRef = Module.ImportReference(ReferenceType.MakeByReferenceType());
                var resolvedType = ReferenceType.Resolve();

                // We need to generate a static readonly Property<typeof(Type), typeof(FieldType)> for each field
                // So we define the fields and generate the initialization code at the same time 
                foreach (var fieldDef in resolvedType.Fields)
                {
                    // For now only support public fields 
                    if (!fieldDef.IsPublic || fieldDef.IsStatic)
                        continue;

                    var fieldRef = Module.ImportReference(fieldDef);
                    var resolvedFieldType = fieldDef.FieldType.Resolve();

                    MethodDefinition getter = null;
                    MethodDefinition setter = null;
                    TypeReference propertyFieldType = null;
                    MethodReference propertyGetterCtor = null;
                    MethodReference propertySetterCtor = null;
                    MethodReference propertyCtor = null;
                    FieldDefinition propertyField = null;
                    
                    // We must generate different Property types based on the field type in order to visit types correctly
                    // as well as for performance (in the case of UnmanagedProperties)
                    if (fieldDef.FieldType.IsArray)
                    {
                        TypeReference fieldType = Module.ImportReference(fieldDef.FieldType);
                        var fieldElementTypeRef = Module.ImportReference(fieldDef.FieldType.GetElementType());
                        
                        // Define the static readonly ArrayProperty<Type, FieldType> field;
                        propertyFieldType = Module.ImportReference(openArrayPropertyDef.MakeGenericInstanceType(ReferenceType, fieldElementTypeRef));
                        // Important, we name the field the same as the actual field so we can use this name later in FindProperty using fieldDef.Name
                        propertyField = new FieldDefinition(fieldDef.Name,FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.InitOnly, propertyFieldType);
                        GeneratedType.Fields.Add(propertyField);
                        (getter, setter) = GenerateGetterSetters(referenceTypeByReferenceRef, fieldDef, fieldType);
                        
                        // Property and its nested delegate types are all open types so we must close them before we invoke ctors
                        propertyCtor = Module.ImportReference(openArrayPropertyCtor.MakeGenericHostMethod(propertyFieldType));
                        var closedPropertyGetter = Module.ImportReference(openArrayPropertyGetter.MakeGenericInstanceType(ReferenceType, fieldElementTypeRef));
                        propertyGetterCtor = Module.ImportReference(openArrayPropertyGetterCtor.MakeGenericHostMethod(closedPropertyGetter));
                        var closedPropertySetter = Module.ImportReference(openArrayPropertySetter.MakeGenericInstanceType(ReferenceType, fieldElementTypeRef));
                        propertySetterCtor = Module.ImportReference(openArrayPropertySetterCtor.MakeGenericHostMethod(closedPropertySetter));
                    }
                    else if(resolvedFieldType != null && resolvedFieldType.Interfaces.Any(i => i.InterfaceType.FullName == "System.Collections.Generic.IList`1<T>"))
                    {
                        TypeReference fieldType = fieldDef.FieldType;
                        if (fieldDef.FieldType.ContainsGenericParameter)
                        {
                            var genericFieldType = fieldDef.FieldType as GenericInstanceType;
                            var genericDeclaringType = ReferenceType as GenericInstanceType;
                            if(genericDeclaringType == null)
                                throw new ArgumentException($"The declaring type for field " +
                                                            $"'{ReferenceType.FullName}' does not specify the generic " +
                                                            $"arguments for field '{fieldDef.FullName}'. We don't yet " +
                                                            $"support nested generic fields parameterized by ancestors.'");
                            
                            // Here we have the case where we are dealing with a generic field that has been specialized
                            // by its declaring type (e.g. struct Foo<int> { List<T> mList; }). In this case our TypeRef
                            // contains a GenericParameter ('T') rather than the GenericArguement ('int') that we really 
                            // care about. So we search through our fields GenericParameters and match up the indicies of
                            // the generic parameters in the 'TypeDefinition' of our declaring type (note it's the
                            // TypeDefinition as that is the only type that will also contain GenericParameters). 
                            // Once we find a match we can use that index to lookup the correct GenericArgument from the 
                            // DeclaringType's TypeReference and use that list to create a new GenericInstance type for
                            // our generic field. Phew.
                            var genericArgs = new List<TypeReference>(); 
                            foreach (var gp in genericFieldType.GenericArguments)
                            {
                                for (int i = 0; i < resolvedType.GenericParameters.Count; ++i)
                                {
                                    var dgp = resolvedType.GenericParameters[i];
                                    if (gp == dgp)
                                    {
                                        genericArgs.Add(genericDeclaringType.GenericArguments[i]);
                                    }
                                }
                            }

                            fieldType = Module.ImportReference(fieldDef.FieldType.Resolve().MakeGenericInstanceType(genericArgs.ToArray()));
                        }
                        
                        var genericField = fieldType as GenericInstanceType;
                        var listElement = Module.ImportReference(genericField.GenericArguments[0]);
                        
                        // Define the static readonly ListProperty<Type, FieldType> field;
                        propertyFieldType = Module.ImportReference(openListPropertyDef.MakeGenericInstanceType(ReferenceType, listElement));
                        // Important, we name the field the same as the actual field so we can use this name later in FindProperty using fieldDef.Name
                        propertyField = new FieldDefinition(fieldDef.Name,FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.InitOnly, propertyFieldType);
                        GeneratedType.Fields.Add(propertyField);

                        // Generate getter and setter functions
                        var closedIList = Module.ImportReference(openIList.MakeGenericInstanceType(listElement));
                        (getter, setter) = GenerateGetterSetters(referenceTypeByReferenceRef, fieldDef, closedIList, fieldType);
                        
                        // Property and its nested delegate types are all open types so we must close them before we invoke ctors
                        propertyCtor = Module.ImportReference(openListPropertyCtor.MakeGenericHostMethod(propertyFieldType));
                        var closedPropertyGetter = Module.ImportReference(openListPropertyGetter.MakeGenericInstanceType(ReferenceType, listElement));
                        propertyGetterCtor = Module.ImportReference(openListPropertyGetterCtor.MakeGenericHostMethod(closedPropertyGetter));
                        var closedPropertySetter = Module.ImportReference(openListPropertySetter.MakeGenericInstanceType(ReferenceType, listElement));
                        propertySetterCtor = Module.ImportReference(openListPropertySetterCtor.MakeGenericHostMethod(closedPropertySetter));
                    }
                    else // Just use a generic Property type
                    {
                        var fieldTypeRef = Module.ImportReference(fieldDef.FieldType);
                        // Define the static readonly Property<Type, FieldType> field;
                        propertyFieldType = Module.ImportReference(openPropertyDef.MakeGenericInstanceType(ReferenceType, fieldTypeRef));
                        // Important, we name the field the same as the actual field so we can use this name later in FindProperty using fieldDef.Name
                        propertyField = new FieldDefinition(fieldDef.Name,FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.InitOnly, propertyFieldType);
                        GeneratedType.Fields.Add(propertyField);
                        (getter, setter) = GenerateGetterSetters(referenceTypeByReferenceRef, fieldDef, fieldTypeRef);
                        
                        // Property and its nested delegate types are all open types so we must close them before we invoke ctors
                        propertyCtor = Module.ImportReference(openPropertyCtor.MakeGenericHostMethod(propertyFieldType));
                        var closedPropertyGetter = Module.ImportReference(openPropertyGetter.MakeGenericInstanceType(ReferenceType, fieldTypeRef));
                        propertyGetterCtor = Module.ImportReference(openPropertyGetterCtor.MakeGenericHostMethod(closedPropertyGetter));
                        var closedPropertySetter = Module.ImportReference(openPropertySetter.MakeGenericInstanceType(ReferenceType, fieldTypeRef));
                        propertySetterCtor = Module.ImportReference(openPropertySetterCtor.MakeGenericHostMethod(closedPropertySetter));
                    }
                    
                    // Initialize the static field in the cctor. Construct a Property and store it to our static field
                    // e.g.
                    // public static readonly Property<ContainerType, FieldType> =
                    //     new Property<ContainerType, FieldType>("FieldName",
                    //         GeneratedGetter, // <== This implicitly constructs a delegate, e.g. new Property<ContainerType, FieldType>.Getter(null, &GeneratedGetter)
                    //         GeneratedSetter, // <== This implicitly constructs a delegate, e.g. new Property<ContainerType, FieldType>.Setter(null, &GeneratedSetter)
                    //         null);
                    il.Emit(OpCodes.Ldstr, fieldDef.Name); // arg0, the Field Name
                    // Construct arg0, our delegate for the getter
                    il.Emit(OpCodes.Ldnull); // null context (it's a static func)
                    il.Emit(OpCodes.Ldftn, getter);
                    il.Emit(OpCodes.Newobj, propertyGetterCtor);
                    // Construct arg1, our delegate for the setter
                    il.Emit(OpCodes.Ldnull); // null context (it's a static func)
                    il.Emit(OpCodes.Ldftn, setter);
                    il.Emit(OpCodes.Newobj, propertySetterCtor);
                    // Push null for arg2, the IPropertiesAttributeCollection as we don't need it at the moment
                    il.Emit(OpCodes.Ldnull);
                    // Construct the Property and store it to our field
                    il.Emit(OpCodes.Newobj, propertyCtor);
                    il.Emit(OpCodes.Stsfld, propertyField);
                }

                il.Emit(OpCodes.Ret);
            }

            (MethodDefinition, MethodDefinition) GenerateGetterSetters(TypeReference containerTypeByReference, FieldDefinition fieldDef, TypeReference getSetType, TypeReference collectionFieldType = null)
            {
                var fieldRef = Module.ImportReference(fieldDef);
                var voidRef = Module.ImportReference(typeof(void));
                
                // Generate getter and setter functions
                var getter = new MethodDefinition(fieldDef.Name + "Getter",MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig, getSetType);
                {
                    getter.Parameters.Add(new ParameterDefinition("container", Mono.Cecil.ParameterAttributes.None, containerTypeByReference));
                    GeneratedType.Methods.Add((getter));

                    // Generates a simple getter
                    // e.g.
                    // public static ElementType Getter(ref ContainerType passedInContainer)
                    // {
                    //     return passedInContainer.containerField;
                    // }
                    var getterIL = getter.Body.GetILProcessor();
                    getterIL.Emit(OpCodes.Ldarg_0);
                    if (!ReferenceType.IsValueType)
                        getterIL.Emit(OpCodes.Ldind_Ref);
                    getterIL.Emit(OpCodes.Ldfld, fieldRef);
                    getterIL.Emit(OpCodes.Ret);
                }

                var setter = new MethodDefinition(fieldDef.Name + "Setter",MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig, voidRef);
                {
                    setter.Parameters.Add(new ParameterDefinition("container", Mono.Cecil.ParameterAttributes.None, containerTypeByReference));
                    setter.Parameters.Add(new ParameterDefinition("val", Mono.Cecil.ParameterAttributes.None, getSetType));
                    GeneratedType.Methods.Add((setter));

                    // Generates a simple setter
                    // e.g.
                    // public static void Getter(ref ContainerType passedInContainer, ElementType passedInVal)
                    // {
                    //     passedInContainer.containerField = passedInVal;
                    // }
                    // In the case of ListProperties, the Unity.Properties API only deals with IList<ElementType>
                    // so we need to explicitly down-cast that back to our field type before assigning 
                    // e.g.
                    // public static void Setter(ref ContainerType passedInContainer, IList<ElementType> passedInVal)
                    // {
                    //     passedInContainer.containerField = (ActualListType<ElementType>) passedInIListVal;
                    // }
                    var setterIL = setter.Body.GetILProcessor();
                    setterIL.Emit(OpCodes.Ldarg_0);
                    if (!ReferenceType.IsValueType)
                        setterIL.Emit(OpCodes.Ldind_Ref);
                    setterIL.Emit(OpCodes.Ldarg_1);
                    if(collectionFieldType != null)
                        setterIL.Emit(OpCodes.Castclass, Module.ImportReference(collectionFieldType));
                    setterIL.Emit(OpCodes.Stfld, fieldRef);
                    setterIL.Emit(OpCodes.Ret);
                }
                
                return (getter, setter);
            }
            
            void GenerateAccept()
            {
                // Generates public override void Accept<TVisitor>(ref TContainer container, ref TVisitor visitor, ref ChangeTracker changeTracker)
                var referenceTypeByReferenceRef = Module.ImportReference(ReferenceType.MakeByReferenceType());
                var acceptFn = new MethodDefinition("Accept", MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.HideBySig, Module.ImportReference(typeof(void)));
                var genericParam = new GenericParameter("TVisitor", acceptFn);
                genericParam.Constraints.Add(Module.ImportReference(typeof(IPropertyVisitor)));
                acceptFn.GenericParameters.Add(genericParam);
                acceptFn.Parameters.Add(new ParameterDefinition("container", ParameterAttributes.None, referenceTypeByReferenceRef));
                acceptFn.Parameters.Add(new ParameterDefinition("visitor", ParameterAttributes.None, genericParam.MakeByReferenceType()));
                ///////////////
                // Remove these lines when Properties removes ChangeTracker from the API 
                var changeTrackerRef = Module.ImportReference(typeof(ChangeTracker));
                acceptFn.Parameters.Add(new ParameterDefinition("changeTracker", ParameterAttributes.None, Module.ImportReference(changeTrackerRef.MakeByReferenceType())));
                ///////////////
                GeneratedType.Methods.Add(acceptFn);

                var openVisitProperty = Module.ImportReference(typeof(IPropertyVisitor).GetMethod("VisitProperty"));
                var openVisitCollectionProperty = Module.ImportReference(typeof(IPropertyVisitor).GetMethod("VisitCollectionProperty"));
                var il = acceptFn.Body.GetILProcessor();
                foreach (var field in GeneratedType.Fields)
                {
                    // We know we will always have these two generic args
                    var genericfield = field.FieldType as GenericInstanceType;
                    var containerType = genericfield.GenericArguments[0]; 
                    var fieldType = genericfield.GenericArguments[1];

                    MethodReference visitProperty = null;
                    if (field.FieldType.Name.Contains("ArrayProperty"))
                    {
                        visitProperty = Module.ImportReference(openVisitCollectionProperty.MakeGenericInstanceMethod(field.FieldType, containerType, Module.ImportReference(fieldType.MakeArrayType())));

                    }
                    else if (field.FieldType.Name.Contains("ListProperty"))
                    {
                        var ilist = Module.ImportReference(typeof(IList<>));
                        var closedIList = Module.ImportReference(ilist.MakeGenericInstanceType(fieldType));
                        visitProperty = Module.ImportReference(openVisitCollectionProperty.MakeGenericInstanceMethod(field.FieldType, containerType, closedIList));
                    }
                    else
                    {
                        visitProperty = Module.ImportReference(openVisitProperty.MakeGenericInstanceMethod(field.FieldType, containerType, fieldType));
                    }

                    // Load our visitor and invoke VisitProperty()
                    // e.g.
                    // public override void Accept<TVisitor>(ref SceneTag container, ref TVisitor visitor, ref ChangeTracker changeTracker)
                    // {
                    //      visitor.VisitProperty<Property<ContainerType, FieldType1>, ContainerType, FieldType1>(GeneratedPropertyForField1, ref container, ref changeTracker);
                    //      visitor.VisitProperty<Property<ContainerType, FieldType2>, ContainerType, FieldType2>(GeneratedPropertyForField2, ref container, ref changeTracker);
                    //      ...
                    //      visitor.VisitProperty<Property<ContainerType, FieldTypeN>, ContainerType, FieldTypeN>(GeneratedPropertyForFieldN, ref container, ref changeTracker);
                    // }
                    il.Emit(OpCodes.Ldarg_2);
                    il.Emit(OpCodes.Ldsfld, field);
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Ldarg_3);
                    il.Emit(OpCodes.Constrained, genericParam);
                    il.Emit(OpCodes.Callvirt, visitProperty);
                    il.Emit(OpCodes.Pop); // ditch the return value
                }

                il.Emit(OpCodes.Ret);
            }
            
            void GenerateFindProperty()
            {
                // Generates public override bool FindProperty<TAction>(string name, ref TContainer container, ref ChangeTracker changeTracker, ref TAction action)
                var referenceTypeByReferenceRef = Module.ImportReference(ReferenceType.MakeByReferenceType());
                var openIPropertyGetter = Module.ImportReference(typeof(IPropertyGetter<>));
                var closedIPropertyGetter = Module.ImportReference(openIPropertyGetter.MakeGenericInstanceType(Module.ImportReference(ReferenceType)));
                
                var findPropertyFn = new MethodDefinition("FindProperty", MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.HideBySig, Module.ImportReference(typeof(bool)));
                var genericParam = new GenericParameter("TAction", findPropertyFn);
                genericParam.Constraints.Add(closedIPropertyGetter);
                findPropertyFn.GenericParameters.Add(genericParam);
                findPropertyFn.Parameters.Add(new ParameterDefinition("name", ParameterAttributes.None, Module.ImportReference(typeof(string))));
                findPropertyFn.Parameters.Add(new ParameterDefinition("container", ParameterAttributes.None, referenceTypeByReferenceRef));
                ///////////////
                // Remove these lines when Properties removes ChangeTracker from the API
                var changeTrackerRef = Module.ImportReference(typeof(ChangeTracker));
                findPropertyFn.Parameters.Add(new ParameterDefinition("changeTracker", ParameterAttributes.None, Module.ImportReference(changeTrackerRef.MakeByReferenceType())));
                ///////////////
                var actionParam = new ParameterDefinition("action", ParameterAttributes.None,
                    genericParam.MakeByReferenceType());
                findPropertyFn.Parameters.Add(actionParam);
                GeneratedType.Methods.Add(findPropertyFn);

                var stringEquals = Module.ImportReference(typeof(string).GetMethod("Equals", new Type[]{typeof(string), typeof(string)}));
                var openVisitProperty = Module.ImportReference(typeof(IPropertyGetter<>).GetMethod("VisitProperty"));
                var openVisitCollectionProperty = Module.ImportReference(typeof(IPropertyGetter<>).GetMethod("VisitCollectionProperty"));
                var closedIPropertyGetterOpenVisitProperty = Module.ImportReference(openVisitProperty.MakeGenericHostMethod(closedIPropertyGetter));
                var closedIPropertyGetterOpenVisitCollectionProperty = Module.ImportReference(openVisitCollectionProperty.MakeGenericHostMethod(closedIPropertyGetter));
                
                var il = findPropertyFn.Body.GetILProcessor();
                Instruction label = null;
                foreach (var field in GeneratedType.Fields)
                {
                    // We know we will always have these two generic args
                    var genericfield = field.FieldType as GenericInstanceType;
                    var containerType = genericfield.GenericArguments[0]; 
                    var fieldType = genericfield.GenericArguments[1];
                    
                    
                    MethodReference visitProperty = null;
                    if (field.FieldType.Name.Contains("ArrayProperty"))
                    {
                        visitProperty = Module.ImportReference(closedIPropertyGetterOpenVisitCollectionProperty.MakeGenericInstanceMethod(field.FieldType, Module.ImportReference(fieldType.MakeArrayType())));

                    }
                    else if (field.FieldType.Name.Contains("ListProperty"))
                    {
                        var ilist = Module.ImportReference(typeof(IList<>));
                        var closedIList = Module.ImportReference(ilist.MakeGenericInstanceType(fieldType));
                        visitProperty = Module.ImportReference(closedIPropertyGetterOpenVisitCollectionProperty.MakeGenericInstanceMethod(field.FieldType, closedIList));
                    }
                    else
                    {
                        visitProperty = Module.ImportReference(closedIPropertyGetterOpenVisitProperty.MakeGenericInstanceMethod(field.FieldType, fieldType));
                    }

                    // We generate a simple name based lookup (we may want to swap to a switch table later)
                    // and then invoke the VisitProperty func for the found property
                    // e.g
                    // public override bool FindProperty<TAction>(string name, ref ContainerType container, ref ChangeTracker changeTracker, ref TAction action)
                    // {
                    //     if (string.Equals(name, GeneratedPropertyField1.GetName()))
                    //     {
                    //         action.VisitProperty<Property<ContainerType, FieldType1>, FieldType1>(GeneratedPropertyField1, ref container, ref changeTracker);
                    //         return true;
                    //     }
                    //     if (string.Equals(name, GeneratedPropertyField2.GetName()))
                    //     {
                    //         action.VisitProperty<Property<ContainerType, FieldType2>, FieldType2>(GeneratedPropertyField2, ref container, ref changeTracker);
                    //         return true;
                    //     }
                    //     return false;
                    // }
                    label = il.Create(OpCodes.Nop);
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Ldstr, field.Name);
                    il.Emit(OpCodes.Call, stringEquals);
                    il.Emit(OpCodes.Brfalse, label);

                    il.Emit(OpCodes.Ldarg, actionParam);
                    il.Emit(OpCodes.Ldsfld, field);
                    il.Emit(OpCodes.Ldarg_2);
                    il.Emit(OpCodes.Ldarg_3);
                    il.Emit(OpCodes.Constrained, genericParam);
                    il.Emit(OpCodes.Callvirt, visitProperty);
                    il.Emit(OpCodes.Ldc_I4_1);
                    il.Emit(OpCodes.Ret);
                    il.Append(label);
                }

                il.Emit(OpCodes.Ldc_I4_0);
                il.Emit(OpCodes.Ret);
            }
        }
    }
}
#endif