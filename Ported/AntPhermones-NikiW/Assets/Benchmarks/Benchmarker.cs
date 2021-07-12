using System;
using System.IO;
using System.Linq;
using Unity.Collections;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
///     NW: Little benchmark framework. Outputs to the console and to a file.
/// </summary>
public class Benchmarker
{
    public delegate (double elapsed, NativeArray<float> results) TestDel(uint seed, double writePerc, int length);
        
    public readonly string Name;
    public readonly TestDel Action;
    public double[] Elapsed;
    public NativeList<float> AllResults;

    public Benchmarker(string name, TestDel action)
    {
        Name = name;
        Action = action;
        AllResults = new NativeList<float>(1024 * 8, Allocator.Persistent);
    }

    /// <summary>
    ///     Returns the ms time cost of counting values.
    ///     Only useful to compare atomic vs counter.
    ///     Do not use to test performance of the job itself, as we perform arbitrary work.
    /// </summary>
    public static void RunTestInnerLoop(Benchmarker[] tests, int[] numIterations, double writePerc, int testId)
    {
        var numInnerLoopIterations = numIterations[testId];
        
        // Run test:
        var seed = (uint)(Random.value * ushort.MaxValue);

        // Random order:
        tests = tests.OrderBy(x => Random.value).ToArray();
        foreach (var test in tests)
        {
            var (elapsed, result) = test.Action(seed, writePerc, numInnerLoopIterations);
            test.Elapsed[testId] += elapsed;

            test.AllResults.AddRange(result);
            result.Dispose();
        }
    }

    public static void RunAndOutputTests(string testName, int[] numIterationsForTestId, Benchmarker[] tests)
    {
        var fullTestStart = Time.realtimeSinceStartupAsDouble;
        var quickResultsString = "QuickResults";
        var tableString = tests.Aggregate("Write%", (c, n) => $"{c},{n.Name}");
        for (var testId = 0; testId < numIterationsForTestId.Length; testId++)
        {
            foreach (var test in tests) test.Elapsed = new double[numIterationsForTestId.Length];

            for (int writePercentageInt = -10; writePercentageInt <= 100; writePercentageInt++)
            {
                double writePercentage = writePercentageInt * 0.01f;

                // NW: First 10 tests are "warmup" and should be disregarded.
                var isWarmup = writePercentage < -0.005f; // NW: Note that writePercentage will never be exactly 0.
                RunTestInnerLoop(tests, numIterationsForTestId, writePercentage, testId);

                if (!isWarmup)
                    tableString += tests.Aggregate($"\n{writePercentage:0.00}", (c, n) => $"{c},{n.Elapsed:000.000000}");
            }

            quickResultsString += tests.Aggregate("", (c, n) => $"{c}\n{n.Name}: Elapsed: {n.Elapsed:000.000000}, Results Length: {n.AllResults.Length}");
            foreach (var test in tests)
            {
                test.AllResults.Clear();
                for (var i = 0; i < test.Elapsed.Length; i++) test.Elapsed[i] = 0;
            }
        }

        foreach (var test in tests) test.AllResults.Dispose();
        var fullTestDuration = (Time.realtimeSinceStartupAsDouble - fullTestStart);
        var fullResultString = $"RESULTS (test duration: {fullTestDuration:0.00}s, iterating pattern: [{string.Join(", ", numIterationsForTestId)}], job worker batch count of {DeterministicNativeQueueTest.innerloopBatchCount}, max job workers: {JobsUtility.MaxJobThreadCount}):\n{tableString}\n\n{quickResultsString}";

        Debug.Log(fullResultString);
        var filePath = Application.persistentDataPath + $"/{testName}_{DateTime.Now:yyyy-MM-dd_hh-mm-ss}.csv";
        Debug.Log(filePath);
        File.WriteAllText(filePath, fullResultString);
    }
}
