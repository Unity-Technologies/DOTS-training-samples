using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityEditor;
using UnityEngine;

namespace Unity.Entities.CodeGen.Tests
{
    internal static class Decompiler
    {
        public static string DecompileIntoString(TypeReference typeReference)
        {
            var assemblyDefinition = typeReference.Module.Assembly;

            var tempFolder = Path.GetTempPath();
            var fileName = $@"{tempFolder}TestAssembly.dll";
            var fileNamePdb = $@"{tempFolder}TestAssembly.pdb";
            var peStream = new FileStream(fileName, FileMode.Create);
            var symbolStream = new FileStream(fileNamePdb, FileMode.Create);
      
            assemblyDefinition.Write(peStream,new WriterParameters() {SymbolStream = symbolStream, SymbolWriterProvider = new PortablePdbWriterProvider(), WriteSymbols = true});
            peStream.Close();
            symbolStream.Close();

            var sb = new StringBuilder();
            
            var processed = new HashSet<string>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(a=>!a.IsDynamic && !string.IsNullOrEmpty(a.Location)))
            {
                string path;
                try
                {
                    path = Path.GetDirectoryName(assembly.Location);
                }
                catch (ArgumentException)
                {
                    UnityEngine.Debug.Log("Unexpected path: " + assembly.Location);
                    continue;
                }

                if (processed.Contains(path))
                    continue;
                processed.Contains(path);
                sb.Append($"--referencepath \"{path}\" ");
            }


            var isWin = Environment.OSVersion.Platform == PlatformID.Win32Windows || Environment.OSVersion.Platform == PlatformID.Win32NT;
            var ilspycmd = Path.GetFullPath("Packages/com.unity.entities/Unity.Entities.CodeGen.Tests/.tools/ilspycmd.exe");
            if (isWin)
                ilspycmd = ilspycmd.Replace("/","\\");
            var psi = new ProcessStartInfo()
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                
                FileName = isWin ? ilspycmd : $"{EditorApplication.applicationPath}/Contents/MonoBleedingEdge/bin/mono",
                Arguments = $"{(isWin ? "" : ilspycmd)} \"{fileName}\" -t {typeReference.FullName.Replace("/","+")} {sb}",
                RedirectStandardOutput = true
            };

            var p = new Process() {StartInfo = psi};
            p.Start();
            return p.StandardOutput.ReadToEnd();
        }
    }
}