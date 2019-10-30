using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build.Content;
using UnityEditor.Build.Pipeline.Injector;
using UnityEditor.Build.Pipeline.Interfaces;

namespace UnityEditor.Build.Pipeline.Tasks
{
    public class StripUnusedSpriteSources : IBuildTask
    {
        public int Version { get { return 2; } }

#pragma warning disable 649
        [InjectContext]
        IDependencyData m_DependencyData;

        [InjectContext(ContextUsage.In, true)]
        IBuildSpriteData m_SpriteData;
#pragma warning restore 649

        public ReturnCode Run()
        {
            if (m_SpriteData == null || m_SpriteData.ImporterData.Count == 0)
                return ReturnCode.SuccessNotRun;

            if (EditorSettings.spritePackerMode == SpritePackerMode.Disabled)
                return ReturnCode.SuccessNotRun;

            var unusedSources = new HashSet<ObjectIdentifier>();
            var textures = m_SpriteData.ImporterData.Values.Where(x => x.PackedSprite).Select(x => x.SourceTexture);
            unusedSources.UnionWith(textures);

            // Count refs from assets
            var assetRefs = m_DependencyData.AssetInfo.SelectMany(x => x.Value.referencedObjects);
            foreach (ObjectIdentifier reference in assetRefs)
                unusedSources.Remove(reference);

            // Count refs from scenes
            var sceneRefs = m_DependencyData.SceneInfo.SelectMany(x => x.Value.referencedObjects);
            foreach (ObjectIdentifier reference in sceneRefs)
                unusedSources.Remove(reference);

            SetOutputInformation(unusedSources);
            return ReturnCode.Success;
        }

        public void SetOutputInformation(HashSet<ObjectIdentifier> unusedSources)
        {
            foreach (var source in unusedSources)
            {
                var assetInfo = m_DependencyData.AssetInfo[source.guid];
                assetInfo.includedObjects.RemoveAt(0);
            }
        }
    }
}
