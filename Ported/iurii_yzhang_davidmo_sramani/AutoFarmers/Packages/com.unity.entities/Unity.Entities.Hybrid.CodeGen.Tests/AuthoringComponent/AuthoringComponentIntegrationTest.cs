using System;
using System.IO;
using System.Linq;
using System.Text;
using Mono.Cecil;
using NUnit.Framework;
using Unity.Entities.CodeGen.Tests;
using Unity.Entities.Hybrid.CodeGen;
using UnityEngine;

namespace Unity.Entities.Hybrid.CodeGen.Tests
{
    [TestFixture]
    public abstract class AuthoringComponentIntegrationTest : IntegrationTest
    {
        protected override string ExpectedPath
        {
            get { return "Packages/com.unity.entities/Unity.Entities.Hybrid.CodeGen.Tests/AuthoringComponent/IntegrationTests"; }
        }

        protected void RunTest(Type type)
        {
            var componentTypeDefinition = TypeDefinitionFor(type);
            var authoringType = AuthoringComponentPostProcessor.CreateAuthoringType(componentTypeDefinition);
            
            RunTest(authoringType);
        }
    }
}