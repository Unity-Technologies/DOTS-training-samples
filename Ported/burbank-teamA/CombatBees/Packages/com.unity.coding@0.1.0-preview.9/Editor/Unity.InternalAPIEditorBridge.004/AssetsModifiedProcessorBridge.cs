using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor.Experimental;
using Unity.Coding.Editor;
using Unity.Coding.Format;
using Unity.Coding.Utils;

namespace CodingToolsInternal.Bridge
{
    class CodingToolsAssetsModifiedProcessor : AssetsModifiedProcessor
    {
        protected override void OnAssetsModified(string[] changedAssets, string[] addedAssets, string[] deletedAssets, AssetMoveInfo[] movedAssets)
        {
            var formatContext = Utility.CreateFormatContext(FormatTriggerKind.Automatic);

            var pathsToFormat = changedAssets.Concat(addedAssets)
                .Where(p => !Path.IsPathRooted(p))
                .Select(p => new NPath(p))
                .Where(p => Utility.ShouldFormat(formatContext, p))
                .UnDefer();

            var formatted = Utility.ProcessWithProgressBar(pathsToFormat, formatContext);
            Utility.lastFormattedAssets = formatted.AddRangeOverride(Utility.lastFormattedAssets);

            foreach (var path in formatted.Keys)
            {
                ReportAssetChanged(path);
            }
        }
    }
}
