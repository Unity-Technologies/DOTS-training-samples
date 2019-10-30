using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using DocoptNet;
using Unity.Coding.Format;
using Unity.Coding.Utils;

[assembly: InternalsVisibleTo("AppTests")]

namespace Unity.Coding.Cli
{
    public static class App
    {
        const string k_Name = "Unity Coding CLI";
        const string k_Version = "0.1.0.dev1";

        const string k_Usage = k_Name + @"
    Usage:
      coding.exe format [-n|--dry-run] [--package-root ROOT] PATH ...
      coding.exe (-h|--help)
      coding.exe --version

      PATH can be only be a filename currently.

    Options:
      -h --help            Show this screen.
      --version            Show version.
      -n --dry-run         Do the command. Don't (over)write any final files.
      --package-root ROOT  Specify the com.unity.coding root (default is to autodetect).
    ";
        // ^ important to keep at least two spaces between a) the end of the option spec and b) its description text

        internal static void Execute(ICollection<string> args, ILogger logger)
        {
            try
            {
                var docopt = new Docopt();
                var parseOk = true;
                docopt.PrintExit += (_, exitArgs) =>
                {
                    logger.Error(exitArgs.Message);
                    parseOk = false;
                };

                var parsed = docopt.Apply(k_Usage, args, version: $"{k_Name} {k_Version}", exit: true);
                if (!parseOk)
                    return;

                if (parsed["format"].IsTrue)
                {
                    var paths = parsed["PATH"].AsList.Cast<ValueObject>();

                    var context = new FormatContext { Logger = logger };
                    if (parsed["--dry-run"].IsTrue)
                        context.Options |= FormatOptions.DryRun;

                    var packageRoot = ((string)parsed["--package-root"]?.Value)?.ToNPath();
                    if (packageRoot != null)
                    {
                        try
                        {
                            context.ThisPackageRoot = packageRoot;
                        }
                        catch (Exception x)
                        {
                            logger.Error($"Invalid package root; {x.Message}");
                            return;
                        }
                    }

                    Formatter.Process(paths.Select(p => p.ToString().ToNPath()), context);
                }
            }
            catch (Exception x)
            {
                logger.Error($"Fatal: unexpected {x.GetType().Name}: {x.Message}\n{x.StackTrace}");
            }
        }

        public static int Main(string[] args)
        {
            var errorCount = 0;
            var localLogger = new DelegateLogger(DefaultLogger.Instance)
            {
                ErrorHandler = message =>
                {
                    ++errorCount;
                    DefaultLogger.Instance.Error(message);
                }
            };

            Execute(args, localLogger);
            return errorCount != 0 ? 1 : 0;
        }
    }
}

