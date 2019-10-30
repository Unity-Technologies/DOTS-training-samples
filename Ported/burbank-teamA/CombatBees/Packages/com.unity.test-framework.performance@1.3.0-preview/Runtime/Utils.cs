using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.PerformanceTesting.Exceptions;
using UnityEngine;

namespace Unity.PerformanceTesting.Runtime
{
    public static class Utils
    {
        public static string ResourcesPath => Path.Combine(Application.dataPath, "Resources");
        public const string TestRunPath = "Assets/Resources/"+TestRunInfo;
        public const string TestRunInfo = "PerformanceTestRunInfo.json";
        public const string PlayerPrefKeyRunJSON = "PT_Run";

        public static double DateToInt(DateTime date)
        {
            return date.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                .TotalMilliseconds;
        }

        public static double ConvertSample(SampleUnit from, SampleUnit to, double value)
        {
            double f = RelativeSampleUnit(@from);
            double t = RelativeSampleUnit(to);

            return value * (t / f);
        }

        public static double RelativeSampleUnit(SampleUnit unit)
        {
            switch (unit)
            {
                case SampleUnit.Nanosecond:
                    return 1000000;
                case SampleUnit.Microsecond:
                    return 1000;
                case SampleUnit.Millisecond:
                    return 1;
                case SampleUnit.Second:
                    return 0.001;
                default:
                    throw new PerformanceTestException(
                        "Wrong SampleUnit type used. Are you trying to convert between time and size units?");
            }
        }

        public static int GetZeroValueCount(List<double> samples)
        {
            var zeroValues = 0;
            foreach (var sample in samples)
            {
                if (Math.Abs(sample) < .0001f)
                {
                    zeroValues++;
                }
            }

            return zeroValues;
        }

        public static double GetMedianValue(List<double> samples)
        {
            var samplesClone = new List<double>(samples);
            samplesClone.Sort();

            var middleIdx = samplesClone.Count / 2;
            return samplesClone[middleIdx];
        }

        public static double GetPercentile(List<double> samples, double percentile)
        {
            if (percentile < 0.00001D)
                return percentile;

            var samplesClone = new List<double>(samples);
            samplesClone.Sort();

            if (samplesClone.Count == 1)
            {
                return samplesClone[0];
            }

            var rank = percentile * (samplesClone.Count + 1);
            var integral = (int) rank;
            var fractional = rank % 1;
            return samplesClone[integral - 1] + fractional * (samplesClone[integral] - samplesClone[integral - 1]);
        }

        public static double GetStandardDeviation(List<double> samples, double average)
        {
            double sumOfSquaresOfDifferences = 0.0D;
            foreach (var sample in samples)
            {
                sumOfSquaresOfDifferences += (sample - average) * (sample - average);
            }

            return Math.Sqrt(sumOfSquaresOfDifferences / samples.Count);
        }

        public static double Min(List<double> samples)
        {
            double min = Mathf.Infinity;
            foreach (var sample in samples)
            {
                if (sample < min) min = sample;
            }

            return min;
        }

        public static double Max(List<double> samples)
        {
            double max = Mathf.NegativeInfinity;
            foreach (var sample in samples)
            {
                if (sample > max) max = sample;
            }

            return max;
        }

        public static double Average(List<double> samples)
        {
            return Sum(samples) / samples.Count;
        }

        public static double Sum(List<double> samples)
        {
            double sum = 0.0D;
            foreach (var sample in samples)
            {
                sum += sample;
            }

            return sum;
        }

        public static string RemoveIllegalCharacters(string path)
        {
            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

            foreach (char c in invalid)
            {
                path = path.Replace(c.ToString(), "");
            }

            return path;
        }

        public static string GetArg(string name)
        {
            var args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == name && args.Length > i + 1)
                {
                    return args[i + 1];
                }
            }

            return null;
        }

        public static bool VerifyTestRunMetadata(PerformanceTestRun run)
        {
            List<String> errors = new List<string>();

            if (run.TestSuite != "Playmode" && run.TestSuite != "Editmode") errors.Add("TestSuite");
            if (run.StartTime < 1.0D) errors.Add("StartTime");
            if (run.EndTime < 1.0D) errors.Add("EndTime");

            if (run.BuildSettings == null) errors.Add("BuildSettings");
            else
            {
                if (run.BuildSettings.BuildTarget.Length == 0) errors.Add("BuildSettings.BuildTarget");
                if (run.BuildSettings.Platform.Length == 0) errors.Add("BuildSettings.Platform");
            }

            if (run.EditorVersion == null) errors.Add("EditorVersion");
            else
            {
                if (run.EditorVersion.DateSeconds == 0) errors.Add("EditorVersion.DateSeconds");
                if (run.EditorVersion.FullVersion.Length == 0) errors.Add("EditorVersion.FullVersion");
                if (run.EditorVersion.Branch.Length == 0) errors.Add("EditorVersion.Branch");
                if (run.EditorVersion.RevisionValue == 0) errors.Add("EditorVersion.RevisionValue");
            }

            if (run.PlayerSettings == null) errors.Add("Performance test has its build settings unassigned.");
            else
            {
                if (run.PlayerSettings.ScriptingBackend.Length == 0) errors.Add("PlayerSettings.ScriptingBackend");
                if (run.PlayerSettings.GraphicsApi.Length == 0) errors.Add("PlayerSettings.GraphicsAp");
                if (run.PlayerSettings.Batchmode.Length == 0) errors.Add("PlayerSettings.Batchmode");
            }

            if (run.PlayerSystemInfo == null) errors.Add("Performance test has its build settings unassigned.");
            else
            {
                if (run.PlayerSystemInfo.ProcessorCount == 0) errors.Add("PlayerSystemInfo.ProcessorCount");
                if (run.PlayerSystemInfo.OperatingSystem.Length == 0) errors.Add("PlayerSystemInfo.OperatingSystem");
                if (run.PlayerSystemInfo.ProcessorType.Length == 0) errors.Add("PlayerSystemInfo.ProcessorType");
                if (run.PlayerSystemInfo.GraphicsDeviceName.Length == 0)errors.Add("PlayerSystemInfo.GraphicsDeviceName");
                if (run.PlayerSystemInfo.SystemMemorySize == 0) errors.Add("PlayerSystemInfo.SystemMemorySize");
            }

            if (run.QualitySettings == null) errors.Add("Performance test has its build settings unassigned.");
            else
            {
                if (run.QualitySettings.ColorSpace.Length == 0) errors.Add("QualitySettings.ColorSpace");
                if (run.QualitySettings.BlendWeights.Length == 0) errors.Add("QualitySettings.BlendWeights");
                if (run.QualitySettings.AnisotropicFiltering.Length == 0) errors.Add("QualitySettings.AnisotropicFiltering");
            }

            if (run.ScreenSettings == null) errors.Add("ScreenSettings");

            if (errors.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var error in errors)
                {
                    sb.Append(error + ", ");
                }
                
                Debug.LogError("Performance run has missing metadata. Please report this as a bug on #devs-performance. The following fields have not been set: " + sb);
                return false;
            }

            return true;
        }
    }
}