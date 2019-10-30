using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Unity.PerformanceTesting.Runtime;
using UnityEngine;

namespace Unity.PerformanceTesting.Editor
{
    public class TestResultXmlParser
    {
        public PerformanceTestRun GetPerformanceTestRunFromXml(string resultXmlFileName)
        {
            ValidateInput(resultXmlFileName);
            var xmlDocument = TryLoadResultXmlFile(resultXmlFileName);
            var performanceTestRun = TryParseXmlToPerformanceTestRun(xmlDocument);
            return performanceTestRun;
        }

        private void ValidateInput(string resultXmlFileName)
        {
            if (string.IsNullOrEmpty(resultXmlFileName))
            {
                Debug.LogWarning($"Test results path is null or empty.");
            }

            if (!File.Exists(resultXmlFileName))
            {
                Debug.LogWarning($"Test results file does not exists at path: {resultXmlFileName}");
            }
        }

        private XDocument TryLoadResultXmlFile(string resultXmlFileName)
        {
            try
            {
                return XDocument.Load(resultXmlFileName);
            }
            catch (Exception e)
            {
                var errMsg = $"Failed to load xml result file: {resultXmlFileName}";
                Debug.LogWarning($"{errMsg}\r\nException: {e.Message}\r\n{e.StackTrace}");
            }

            return null;
        }

        private PerformanceTestRun TryParseXmlToPerformanceTestRun(XContainer xmlDocument)
        {
            var output = xmlDocument.Descendants("output").ToArray();
            if (!output.Any())
            {
                return null;
            }

            var run = new PerformanceTestRun();
            DeserializeTestResults(output, run);
            DeserializeMetadata(output, run);
            
            return run;
        }

        private void DeserializeTestResults(IEnumerable<XElement> output, PerformanceTestRun run)
        {
            foreach (var element in output)
            {
                foreach (var line in element.Value.Split('\n'))
                {
                    var json = GetJsonFromHashtag("performancetestresult", line);
                    if (json == null)
                    {
                        continue;
                    }

                    var result = TryDeserializePerformanceTestResultJsonObject(json);
                    if (result != null)
                    {
                        run.Results.Add(result);
                    }                    
                }
            }
        }

        private void DeserializeMetadata(IEnumerable<XElement> output, PerformanceTestRun run)
        {
            foreach (var element in output)
            {
                var elements = element.Value.Split('\n');
                if (!elements.Any(e => e.Length > 0 && e.Substring(0, 2).Equals("##"))) continue;
                {
                    var line = elements.First(e => e.Length > 0 && e.Substring(0, 2).Equals("##"));

                    var json = GetJsonFromHashtag("performancetestruninfo", line);

                    // This is the happy case where we have a performancetestruninfo json object
                    if (json != null)
                    {
                        var result = TryDeserializePerformanceTestRunJsonObject(json);
                        if (result == null) continue;
                        run.TestSuite = result.TestSuite;
                        run.EditorVersion = result.EditorVersion;
                        run.QualitySettings = result.QualitySettings;
                        run.ScreenSettings = result.ScreenSettings;
                        run.BuildSettings = result.BuildSettings;
                        run.PlayerSettings = result.PlayerSettings;
                        run.PlayerSystemInfo = result.PlayerSystemInfo;
                        run.StartTime = result.StartTime;
                        run.EndTime = Utils.DateToInt(DateTime.Now);
                    }
                    // Unhappy case where we couldn't find a performancetestruninfo object
                    // This could be because we have missing metadata for the test run
                    // In this case, we try to look for a performancetestresult json object
                    // We should have at least startime metadata  that we can use to correctly
                    // display the test results on the x-axis of the chart
                    else
                    {
                        json = GetJsonFromHashtag("performancetestresult", line);
                        if (json != null)
                        {
                            var result = TryDeserializePerformanceTestRunJsonObject(json);
                            run.StartTime = result.StartTime;
                            run.EndTime = Utils.DateToInt(DateTime.Now);
                        }
                    }
                }
            }
        }

        private PerformanceTest TryDeserializePerformanceTestResultJsonObject(string json)
        {
            try
            {
                return JsonUtility.FromJson<PerformanceTest>(json);
            }
            catch (Exception e)
            {
                var errMsg = $"Exception thrown while deserializing json string to PerformanceTestResult: {json}";
                Debug.LogWarning($"{errMsg}\r\nException: {e.Message}\r\n{e.StackTrace}");
            }

            return null;
        }
        
        private PerformanceTestRun TryDeserializePerformanceTestRunJsonObject(string json)
        {
            try
            {
                return JsonUtility.FromJson<PerformanceTestRun>(json);
            }
            catch (Exception e)
            {
                var errMsg = $"Exception thrown while deserializing json string to PerformanceTestRun: {json}";
                Debug.LogWarning($"{errMsg}\r\nException: {e.Message}\r\n{e.StackTrace}");
            }

            return null;
        }

        private string GetJsonFromHashtag(string tag, string line)
        {
            if (!line.Contains($"##{tag}:")) return null;
            var jsonStart = line.IndexOf('{');
            var openBrackets = 0;
            var stringIndex = jsonStart;
            while (openBrackets > 0 || stringIndex == jsonStart)
            {
                var character = line[stringIndex];
                switch (character)
                {
                    case '{':
                        openBrackets++;
                        break;
                    case '}':
                        openBrackets--;
                        break;
                }

                stringIndex++;
            }
            var jsonEnd = stringIndex;
            return line.Substring(jsonStart, jsonEnd - jsonStart);
        }
    }
}
