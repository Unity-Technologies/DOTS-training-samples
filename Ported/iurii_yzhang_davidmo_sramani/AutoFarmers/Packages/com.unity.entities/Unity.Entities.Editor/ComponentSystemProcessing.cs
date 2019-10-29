using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.UnityLinker;

namespace Unity.Entities.IL2CPPProcessing
{
    public class ComponentSystemProcessing : IUnityLinkerProcessor
    {
        public int callbackOrder => 0;

        public string GenerateAdditionalLinkXmlFile(BuildReport report, UnityLinkerBuildPipelineData data)
        {
            // Let's build a dictionary of Assemblies as key and all their ComponentSystem as value (in a list)
            var typesByAssemblies = new Dictionary<Assembly, List<Type>>();

            var csb = typeof(ComponentSystemBase);

            void ExtractComponentSystemTypes(Type[] inputTypes, Assembly hostingAssembly)
            {
                var types = inputTypes.Where(type => type != null && type.IsSubclassOf(csb)).ToList();

                if (types.Count > 0)
                {
                    typesByAssemblies.Add(hostingAssembly, types);
                }
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!TypeManager.IsAssemblyReferencingEntities(assembly))
                    continue;

                try
                {
                    var allTypes = assembly.GetTypes();
                    ExtractComponentSystemTypes(allTypes, assembly);
                }
                catch (ReflectionTypeLoadException ex)
                {
                    ExtractComponentSystemTypes(ex.Types, assembly);
                    Debug.LogWarning($"Couldn't load types from assembly: {assembly.FullName}");
                }
            }

            // Create the XML file
            var sb = new StringBuilder();

            // Header
            sb.AppendLine("<linker>");

            // For each assembly, add an <assembly> element that will contains <type> elements nested for all the type to include
            foreach (var assembly in typesByAssemblies.Keys)
            {
                // Add the assembly element
                sb.AppendLine($"  <assembly fullname=\"{assembly.GetName().Name}\">");

                // Add the type element
                var types = typesByAssemblies[assembly];
                foreach (var type in types)
                {
                    sb.AppendLine($"    <type fullname=\"{FormatForXml(ToCecilName(type.FullName))}\" preserve=\"all\"/>");
                }

                // Close assembly element
                sb.AppendLine("  </assembly>");
            }

            // Close linker element
            sb.AppendLine("</linker>");

            // Create a file with the content
            var filePathName = Path.Combine(data.inputDirectory, "DotsStripping.xml");
            File.WriteAllText(filePathName, sb.ToString());

            // Return the file path & name
            return filePathName;
        }

        static string ToCecilName (string fullTypeName)
        {
            return fullTypeName.Replace ('+', '/');
        }

        static string FormatForXml(string value)
        {
            return value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
        }

        public void OnBeforeRun(BuildReport report, UnityLinkerBuildPipelineData data)
        {
        }

        public void OnAfterRun(BuildReport report, UnityLinkerBuildPipelineData data)
        {
        }
    }
}
