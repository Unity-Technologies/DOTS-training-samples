using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Unity.CompilationPipeline.Common.Diagnostics;
using Unity.CompilationPipeline.Common.ILPostProcessing;

[assembly: InternalsVisibleTo("Unity.Entities.Hybrid.CodeGen")]
namespace Unity.Entities.CodeGen
{
    internal class EntitiesILPostProcessors : ILPostProcessor
    {
        static EntitiesILPostProcessor[] FindAllEntitiesILPostProcessors()
        {
            var processorTypes = new List<Type>();
            foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if(assembly.FullName.Contains(".CodeGen"))
                    processorTypes.AddRange(assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(EntitiesILPostProcessor)) && !t.IsAbstract));
            }
            
            return processorTypes.Select(t => (EntitiesILPostProcessor)Activator.CreateInstance(t)).ToArray();
        }

        public override ILPostProcessResult Process(ICompiledAssembly compiledAssembly)
        {
            if (!WillProcess(compiledAssembly))
                return null;
  
            var assemblyDefinition = AssemblyDefinitionFor(compiledAssembly);
            var postProcessors = FindAllEntitiesILPostProcessors();

            var diagnostics = new List<DiagnosticMessage>(); 
            bool madeAnyChange = false;
            foreach (var postProcessor in postProcessors)
            {
                diagnostics.AddRange(postProcessor.PostProcess(assemblyDefinition, out var madeChange));
                madeAnyChange |= madeChange;
            };

            if (!madeAnyChange || diagnostics.Any(d=>d.DiagnosticType == DiagnosticType.Error))
                return new ILPostProcessResult(null, diagnostics);
            
            var pe = new MemoryStream();
            var pdb = new MemoryStream();
            var writerParameters = new WriterParameters
            {
                SymbolWriterProvider = new PortablePdbWriterProvider(), SymbolStream = pdb, WriteSymbols = true
            };
            
            assemblyDefinition.Write(pe, writerParameters);
            return new ILPostProcessResult(new InMemoryAssembly(pe.ToArray(), pdb.ToArray()), diagnostics);
        }

        public override ILPostProcessor GetInstance()
        {
            return this;
        }
        
        public override bool WillProcess(ICompiledAssembly compiledAssembly)
        {
            if (compiledAssembly.Name == "Unity.Entities")
                return true;
            return compiledAssembly.References.Any(f => f.EndsWith("Unity.Entities.dll")) && 
                   !compiledAssembly.Name.Contains("CodeGen.Tests");
        }

        class PostProcessorAssemblyResolver : IAssemblyResolver
        {
            private readonly string[] _references;
            Dictionary<string, AssemblyDefinition> _cache = new Dictionary<string, AssemblyDefinition>();
            private ICompiledAssembly _compiledAssembly;
            private AssemblyDefinition _selfAssembly;

            public PostProcessorAssemblyResolver(ICompiledAssembly compiledAssembly)
            {
                _compiledAssembly = compiledAssembly;
                _references = compiledAssembly.References;
            }
            
            public void Dispose()
            {
            }

            public AssemblyDefinition Resolve(AssemblyNameReference name)
            {
                return Resolve(name, new ReaderParameters(ReadingMode.Deferred));
            }

            
            public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
            {
                lock (_cache)
                {
                    if (name.Name == _compiledAssembly.Name)
                        return _selfAssembly;
                    
                    var fileName = _references.FirstOrDefault(r => r.EndsWith(name.Name + ".dll"));
                    if (fileName == null)
                    {
                        // perhaps the type comes from an exe instead
                        fileName = _references.FirstOrDefault(r => r.EndsWith(name.Name + ".exe"));
                        if (fileName == null)
                            return null;
                    }

                    var lastWriteTime = File.GetLastWriteTime(fileName);

                    var cacheKey = fileName + lastWriteTime.ToString();

                    if (_cache.TryGetValue(cacheKey, out var result))
                        return result;

                    parameters.AssemblyResolver = this;

                    var ms = MemoryStreamFor(fileName);
                    
                    var pdb = fileName + ".pdb";
                    if (File.Exists(pdb))
                        parameters.SymbolStream = MemoryStreamFor(pdb);
                    
                    var assemblyDefinition = AssemblyDefinition.ReadAssembly(ms, parameters);
                    _cache.Add(cacheKey, assemblyDefinition);
                    return assemblyDefinition;
                }
            }
            

            static MemoryStream MemoryStreamFor(string fileName)
            {
                return Retry(10, TimeSpan.FromSeconds(1), () => {
                    byte[] byteArray;
                    using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        byteArray = new byte[fs.Length];
                        var readLength = fs.Read(byteArray, 0, (int) fs.Length);
                        if (readLength != fs.Length)
                            throw new InvalidOperationException("File read length is not full length of file.");
                    }

                    return new MemoryStream(byteArray);
                });
            }

            private static MemoryStream Retry(int retryCount, TimeSpan waitTime, Func<MemoryStream> func)
            {
                try
                {
                    return func();
                }
                catch (IOException)
                {
                    if (retryCount == 0)
                        throw;
                    Console.WriteLine($"Caught IO Exception, trying {retryCount} more times");
                    Thread.Sleep(waitTime);
                    return Retry(retryCount - 1, waitTime, func);
                }
            }

            public void AddAssemblyDefinitionBeingOperatedOn(AssemblyDefinition assemblyDefinition)
            {
                _selfAssembly = assemblyDefinition;
            }
        }

        private static AssemblyDefinition AssemblyDefinitionFor(ICompiledAssembly compiledAssembly)
        {
            var resolver = new PostProcessorAssemblyResolver(compiledAssembly);
            var readerParameters = new ReaderParameters
            {
                SymbolStream = new MemoryStream(compiledAssembly.InMemoryAssembly.PdbData.ToArray()),
                SymbolReaderProvider = new PortablePdbReaderProvider(),
                AssemblyResolver = resolver,
                ReadingMode = ReadingMode.Immediate
            };

            var peStream = new MemoryStream(compiledAssembly.InMemoryAssembly.PeData.ToArray());
            var assemblyDefinition = AssemblyDefinition.ReadAssembly(peStream, readerParameters);

            //apparently, it will happen that when we ask to resolve a type that lives inside Unity.Entities, and we
            //are also postprocessing Unity.Entities, type resolving will fail, because we do not actually try to resolve
            //inside the assembly we are processing. Let's make sure we do that, so that we can use postprocessor features inside
            //unity.entities itself as well.
            resolver.AddAssemblyDefinitionBeingOperatedOn(assemblyDefinition);
            
            return assemblyDefinition;
        }
    }
    
    abstract class EntitiesILPostProcessor
    {
        protected AssemblyDefinition AssemblyDefinition;

        private List<DiagnosticMessage> _diagnosticMessages = new List<DiagnosticMessage>();

        public IEnumerable<DiagnosticMessage> PostProcess(AssemblyDefinition assemblyDefinition, out bool madeAChange)
        {
            AssemblyDefinition = assemblyDefinition;
            try
            {
                madeAChange = PostProcessImpl();
            }
            catch (FoundErrorInUserCodeException e)
            {
                madeAChange = false;
                return e.DiagnosticMessages;
            }

            return _diagnosticMessages;
        }
        
        protected abstract bool PostProcessImpl();

        protected void AddDiagnostic(DiagnosticMessage diagnosticMessage)
        {
            _diagnosticMessages.Add(diagnosticMessage);
        }
    }
}
