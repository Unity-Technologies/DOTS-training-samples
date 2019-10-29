using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Unity.Entities.CodeGen;

namespace Unity.Entities.CodeGen.Tests
{
    public class PostProcessorTestBase
    {
        protected AssemblyDefinition AssemblyDefinitionFor(Type type)
        {
            var assemblyLocation = type.Assembly.Location;

            var resolver = new OnDemandResolver();
            
            var ad = AssemblyDefinition.ReadAssembly(new MemoryStream(File.ReadAllBytes(assemblyLocation)), 
                new ReaderParameters(ReadingMode.Immediate)
                {
                    ReadSymbols = true,
                    ThrowIfSymbolsAreNotMatching = true,
                    SymbolReaderProvider = new PortablePdbReaderProvider(),
                    AssemblyResolver = resolver,
                    SymbolStream = PdbStreamFor(assemblyLocation)
                }
            );

            if (!ad.MainModule.HasSymbols)
                throw new Exception("NoSymbols");
            return ad;
        }
        
        protected TypeDefinition TypeDefinitionFor(Type type)
        {
            var ad = AssemblyDefinitionFor(type);
            var fullName = type.FullName.Replace("+", "/");
            return ad.MainModule.GetType(fullName).Resolve();
        }
        
        protected TypeDefinition TypeDefinitionFor(string typeName, Type nextToType)
        {
            var ad = AssemblyDefinitionFor(nextToType);
            var fullName = nextToType.FullName.Replace("+", "/");
            fullName = fullName.Replace(nextToType.Name, typeName);
            return ad.MainModule.GetType(fullName).Resolve();
        }
        
        protected MethodDefinition MethodDefinitionForOnlyMethodOf(Type type)
        {
            return MethodDefinitionForOnlyMethodOfDefinition(TypeDefinitionFor(type));
        }
        
        protected MethodDefinition MethodDefinitionForOnlyMethodOfDefinition(TypeDefinition typeDefinition)
        {
            var a = typeDefinition.GetMethods().Where(m => !m.IsConstructor && !m.IsStatic).ToList();
            return a.Count == 1 ? a.Single() : a.Single(m=>m.Name == "Test");
        }

        private static MemoryStream PdbStreamFor(string assemblyLocation)
        {
            var file = Path.ChangeExtension(assemblyLocation, ".pdb");
            if (!File.Exists(file))
                return null;
            return new MemoryStream(File.ReadAllBytes(file));
        }

        class OnDemandResolver : IAssemblyResolver
        {
            public void Dispose()
            {
            }

            public AssemblyDefinition Resolve(AssemblyNameReference name)
            {
                return Resolve(name, new ReaderParameters(ReadingMode.Deferred));
            }

            public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
            {
                var assembly = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == name.Name);
                var fileName = assembly.Location;
                parameters.AssemblyResolver = this;
                parameters.SymbolStream = PdbStreamFor(fileName);
                var bytes = File.ReadAllBytes(fileName);
                return AssemblyDefinition.ReadAssembly(new MemoryStream(bytes), parameters);
            }
        }

        protected void AssertProducesNoError(Type systemType)
        {
            Assert.DoesNotThrow(() =>
            {
                var assemblyDefinition = AssemblyDefinitionFor(systemType);
                var testSystemType = assemblyDefinition.MainModule.GetAllTypes()
                    .Where(TypeDefinitionExtensions.IsComponentSystem)
                    .Where(t => t.Name == systemType.Name).FirstOrDefault();

                foreach (var methodToAnalyze in testSystemType.Methods.ToList())
                {
                    foreach (var forEachDescriptionConstruction in LambdaJobDescriptionConstruction.FindIn(methodToAnalyze))
                    {
                        LambdaJobsPostProcessor.Rewrite(methodToAnalyze, forEachDescriptionConstruction);
                    }
                }

                // Write out assembly to memory stream
                // Missing ImportReference errors for types only happens here. 
                var pe = new MemoryStream();
                var pdb = new MemoryStream();
                var writerParameters = new WriterParameters
                {
                    SymbolWriterProvider = new PortablePdbWriterProvider(), SymbolStream = pdb, WriteSymbols = true
                };
                assemblyDefinition.Write(pe, writerParameters);
            });
        }

        protected void AssertProducesError(Type systemType, params string[] shouldContains)
        {
            var methodToAnalyze = MethodDefinitionForOnlyMethodOf(systemType);
            var userCodeException = Assert.Throws<FoundErrorInUserCodeException>(() =>
            {
                foreach (var forEachDescriptionConstruction in LambdaJobDescriptionConstruction.FindIn(methodToAnalyze))
                {
                    LambdaJobsPostProcessor.Rewrite(methodToAnalyze, forEachDescriptionConstruction);
                }
            });
            foreach(var s in shouldContains)
                StringAssert.Contains(s, userCodeException.ToString());

            if (!userCodeException.ToString().Contains(".cs"))
                Assert.Fail("Diagnostic message had no file info: "+userCodeException);

            var match = Regex.Match(userCodeException.ToString(), "\\.cs:?\\((?<line>.*?),(?<column>.*?)\\)");
            if (!match.Success)
                Assert.Fail("Diagnostic message had no line info: "+userCodeException);

            
            var line = int.Parse(match.Groups["line"].Value);
            if (line > 1000)
                Assert.Fail("Unreasonable line number in errormessage: " + userCodeException);
        }
    }
}