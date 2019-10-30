using System;
using System.IO;
using System.Linq;
using System.Text;
using Mono.Cecil;
using NUnit.Framework;
using Unity.Entities.CodeGen;

namespace Unity.Entities.CodeGen.Tests.PropertyBags.Infrastructure
{
    [TestFixture]
    public abstract class PropertyBagsIntegrationTest : IntegrationTest
    {
        protected override string ExpectedPath
        {
            get { return "Packages/com.unity.entities/Unity.Entities.CodeGen.Tests/PropertyBags/IntegrationTests"; }
        }

        protected void RunTest(Type type)
        {
            var assemblyDefinition = AssemblyDefinitionFor(type);
            var typeReference = assemblyDefinition.MainModule.GetType(type.FullName);
            var assemblyResolver = assemblyDefinition.MainModule.AssemblyResolver;
            using (var scratchAssembly = AssemblyDefinition.CreateAssembly(
                new AssemblyNameDefinition("PropertyBagIntegrationTest", new Version(0, 0)),
                "PropertyBagIntegrationTest.dll", new ModuleParameters()
                {
                   AssemblyResolver = assemblyResolver,
                   Kind = ModuleKind.Dll
                }))
            {
                var generatedPropertyBag = PropertyBagPostProcessor.GeneratePropertyBag(scratchAssembly.MainModule, typeReference);
                scratchAssembly.MainModule.Types.Add(generatedPropertyBag.GeneratedType);
                
                RunTest(scratchAssembly.MainModule.ImportReference(generatedPropertyBag.GeneratedType));
            }
        }
    }
}