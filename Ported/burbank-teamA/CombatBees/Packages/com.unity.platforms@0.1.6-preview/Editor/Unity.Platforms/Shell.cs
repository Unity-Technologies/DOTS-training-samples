using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Assert = UnityEngine.Assertions.Assert;
using Debug = UnityEngine.Debug;

namespace Unity.Platforms
{
    public class ShellProcessOutput
    {
        public bool Succeeded { get; set; } = true;
        public string Command { get; set; }
        public StringBuilder CommandOutput { get; set; }
        public string FullOutput { get; set; }
        public string ErrorOutput { get; set; }
        public int ExitCode { get; set; }
    }

    public class ShellProcessArgs
    {
        public const int DefaultMaxIdleTimeInMilliseconds = 30000;

        public static readonly ShellProcessArgs Default = new ShellProcessArgs();

        public string Executable { get; set; }
        public IEnumerable<string> Arguments { get; set; }
        public DirectoryInfo WorkingDirectory { get; set; }
        public IReadOnlyDictionary<string, string> EnvironmentVariables { get; set; }
        public int MaxIdleTimeInMilliseconds { get; set; } = DefaultMaxIdleTimeInMilliseconds;
        public bool MaxIdleKillIsAnError { get; set; } = true;
        public DataReceivedEventHandler OutputDataReceived { get; set; }
        public DataReceivedEventHandler ErrorDataReceived { get; set; }
        public bool ThrowOnError { get; set; } = true;
    }

    public static class Shell
    {
        public static ShellProcessOutput Run(ShellProcessArgs shellArgs)
        {
            Assert.IsNotNull(shellArgs);
            Assert.IsFalse(string.IsNullOrEmpty(shellArgs.Executable));

            var runOutput = new ShellProcessOutput();
            var hasErrors = false;
            var output = new StringBuilder();
            var logOutput = new StringBuilder();
            var errorOutput = new StringBuilder();

            // Prepare data received handlers
            DataReceivedEventHandler outputReceived = (sender, e) =>
            {
                LogProcessData(e.Data, output);
                logOutput.AppendLine(e.Data);
            };
            DataReceivedEventHandler errorReceived = (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    errorOutput.AppendLine(e.Data);
                    hasErrors = true;
                }
                LogProcessData(e.Data, output);
                logOutput.AppendLine(e.Data);
            };

            // Run command in shell and wait for exit
            using (var process = StartProcess(new ShellProcessArgs
            {
                Executable = shellArgs.Executable,
                Arguments = shellArgs.Arguments.ToArray(),
                WorkingDirectory = shellArgs.WorkingDirectory,
                OutputDataReceived = outputReceived,
                ErrorDataReceived = errorReceived
            }))
            {
                var processUpdate = WaitForProcess(process, shellArgs.MaxIdleTimeInMilliseconds, shellArgs.MaxIdleKillIsAnError);
                while (processUpdate.MoveNext())
                {
                }

                var exitCode = process.ExitCode;
                if (processUpdate.Current == ProcessStatus.Killed)
                {
                    if (shellArgs.MaxIdleKillIsAnError)
                    {
                        exitCode = -1;
                    }
                    else
                    {
                        exitCode = 0;
                    }
                }

                runOutput.ExitCode = exitCode;
                runOutput.Command = shellArgs.Executable;
                runOutput.CommandOutput = output;
                runOutput.FullOutput = logOutput.ToString();
                runOutput.ErrorOutput = errorOutput.ToString();
                LogProcessData($"Process exited with code '{exitCode}'", logOutput);
                hasErrors |= (exitCode != 0);
            }

            if (hasErrors && shellArgs.ThrowOnError)
            {
                throw new Exception(errorOutput.ToString());
            }

            runOutput.Succeeded = !hasErrors;
            return runOutput;
        }

        private static void LogProcessData(string data, StringBuilder output)
        {
            if (string.IsNullOrEmpty(data))
            {
                return;
            }

            Console.WriteLine(data); // Editor.log
            output.AppendLine(data);
        }

        public static Process RunAsync(ShellProcessArgs args)
        {
            return StartProcess(args);
        }

        private static Process StartProcess(ShellProcessArgs args)
        {
            var startInfo = new ProcessStartInfo()
            {
                FileName = args.Executable,
                Arguments = string.Join(" ", args.Arguments),
                WorkingDirectory = args.WorkingDirectory?.FullName ?? new DirectoryInfo(".").FullName,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                StandardOutputEncoding = Encoding.UTF8,
                RedirectStandardError = true,
                StandardErrorEncoding = Encoding.UTF8,
                CreateNoWindow = true,
                UseShellExecute = false
            };

            if (args.EnvironmentVariables != null)
            {
                foreach (var pair in args.EnvironmentVariables)
                {
                    startInfo.EnvironmentVariables[pair.Key] = pair.Value;
                }
            }

            var process = new Process { StartInfo = startInfo };

            if (args.OutputDataReceived != null)
            {
                process.OutputDataReceived += args.OutputDataReceived;
            }

            if (args.ErrorDataReceived != null)
            {
                process.ErrorDataReceived += args.ErrorDataReceived;
            }

            process.Start();

            if (args.OutputDataReceived != null)
            {
                process.BeginOutputReadLine();
            }

            if (args.ErrorDataReceived != null)
            {
                process.BeginErrorReadLine();
            }

            return process;
        }

        public enum ProcessStatus
        {
            Running,
            Killed,
            Done
        }

        public static IEnumerator<ProcessStatus> WaitForProcess(Process process, int maxIdleTimeInMs, bool idleAsError, int yieldFrequencyInMs = 30)
        {
            var totalWaitInMs = 0;
            for (; ; )
            {
                if (process.WaitForExit(yieldFrequencyInMs))
                {
                    // WaitForExit with a timeout will not wait for async event handling operations to finish.
                    // To ensure that async event handling has been completed, call WaitForExit that takes no parameters.
                    // See remarks: https://msdn.microsoft.com/en-us/library/ty0d8k56(v=vs.110)
                    process.WaitForExit();
                    yield return ProcessStatus.Done;
                    break;
                }

                totalWaitInMs += yieldFrequencyInMs;

                if (totalWaitInMs < maxIdleTimeInMs)
                {
                    yield return ProcessStatus.Running;
                    continue;
                }

                // idle for too long with no output? -> kill
                // nb: testing the process threads WaitState doesn't work on OSX
                if (idleAsError)
                    Debug.LogError("Idle process detected. See console for more details.");
                process.Kill();
                process.WaitForExit();
                yield return ProcessStatus.Killed;
                break;
            }
        }
    }
}
