using System.Collections.Generic;
using UnityEngine;

namespace Unity.Coding.Editor.ApiScraping
{
    public static class ApiScraping
    {
        public static bool ValidateAllFilesScraped(List<string> failedFileList)
        {
            return ApiScrapingEditorIntegration.ValidateAll(failedFileList);
        }
    }
}
