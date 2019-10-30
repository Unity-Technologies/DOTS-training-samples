using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using Unity.Coding.Utils;

namespace Unity.Coding.Editor
{
    [PublicAPI]
    public static class ProcessUtility
    {
        public enum StdStream
        {
            Stdout,
            Stderr
        }

        public static int ExecuteCommandLine(
            string exePath, IEnumerable<object> processArgs, string workingDirectory,
            Action<string, StdStream> onLine, IEnumerable<string> stdinLines = null)
        {
            // getting weird behavior sometimes (exe not found) in unity if using relative path, so force it absolute here
            exePath = Path.GetFullPath(exePath);

            if (processArgs == null)
                processArgs = Enumerable.Empty<object>();

            var processArgsText = processArgs
                .Select(obj =>
            {
                var str = obj.ToString();
                if (str.IndexOf(' ') >= 0)
                    str = '"' + str + '"';
                return str;
            })
                .StringJoin(" ");

            using (var stdoutCompleted = new ManualResetEvent(false))
            using (var stderrCompleted = new ManualResetEvent(false))
            using (var process = new Process
               {
                   StartInfo = new ProcessStartInfo
                   {
                       // keep new process completely out of user view
                       UseShellExecute = false,
                       CreateNoWindow = true,
                       WindowStyle = ProcessWindowStyle.Hidden,
                       ErrorDialog = false,

                       WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory,
                       FileName = exePath,
                       Arguments = processArgsText,

                       RedirectStandardInput = stdinLines != null,
                       RedirectStandardOutput = true,
                       RedirectStandardError = true,
                   }
               })
            {
                // avoid caller needing to do this (and pretty much everybody will want it)
                var serializer = new object();

                // ReSharper disable AccessToDisposedClosure
                // ^ this is ok because we either kill or wait for process to stop before `using` will dispose the events
                process.OutputDataReceived += (_, line) =>
                {
                    if (line.Data == null)
                        stdoutCompleted.Set();
                    else
                    {
                        lock (serializer)
                            onLine(line.Data, StdStream.Stdout);
                    }
                };
                process.ErrorDataReceived += (_, line) =>
                {
                    if (line.Data == null)
                        stderrCompleted.Set();
                    else
                    {
                        lock (serializer)
                            onLine(line.Data, StdStream.Stderr);
                    }
                };

                // ReSharper restore AccessToDisposedClosure

                // start everything
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                // write if needed
                if (stdinLines != null)
                {
                    foreach (var line in stdinLines)
                        process.StandardInput.WriteLine(line);

                    process.StandardInput.Close();
                }

                // wait for proc and all reads to finish
                process.WaitForExit();
                stdoutCompleted.WaitOne();
                stderrCompleted.WaitOne();

                return process.ExitCode;
            }
        }

        public static int ExecuteCommandLine(
            string exePath, IEnumerable<object> processArgs, string workingDirectory,
            ICollection<string> stdout, ICollection<string> stderr, IEnumerable<string> stdin = null)
        {
            return ExecuteCommandLine(
                exePath, processArgs, workingDirectory,
                (line, stream) => (stream == StdStream.Stdout ? stdout : stderr)?.Add(line),
                stdin);
        }
    }
}
