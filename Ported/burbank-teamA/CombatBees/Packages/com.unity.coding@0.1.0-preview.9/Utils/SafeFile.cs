using System;
using System.IO;

namespace Unity.Coding.Utils
{
    public static class SafeFile
    {
        public const string TmpExtension = ".tmp~";
        public const string BakExtension = ".bak~";

        // TODO: move these file utility methods into NiceIO, add tests, then PR upstream

        public static void SetReadOnly(string path, bool set = true)
        {
            var attrs = File.GetAttributes(path);
            var newAttrs = set
                ? attrs | FileAttributes.ReadOnly
                : attrs & ~FileAttributes.ReadOnly;
            File.SetAttributes(path, newAttrs);
        }

        public static void ForceDeleteFile(string path)
        {
            if (File.Exists(path))
            {
                SetReadOnly(path, false);
                File.Delete(path);
            }
        }

        // TODO: add tests (see https://stackoverflow.com/a/1528151/14582)
        public static void AtomicWrite(string path, Action<string> write)
        {
            // note that File.Delete doesn't throw if file doesn't exist

            // dotnet doesn't have an atomic move operation (have to pinvoke to something in the OS to get that,
            // and even then on windows it's not guaranteed). so the "atomic" part of this name is just to ensure
            // that partially written file never happens.

            if (File.Exists(path) && (File.GetAttributes(path) & FileAttributes.ReadOnly) != 0)
                throw new UnauthorizedAccessException($"Cannot overwrite read-only file '{path}'.");

            var tmpPath = path + TmpExtension;

            try
            {
                ForceDeleteFile(tmpPath);
                write(tmpPath);

                // only mess with original file if the action actually caused a new file to be created
                if (File.Exists(tmpPath))
                {
                    // temporarily keep the old file, until we're sure the new file is moved
                    var bakPath = path + BakExtension;
                    ForceDeleteFile(bakPath);
                    File.Move(path, bakPath);

                    // do the actual move
                    File.Move(tmpPath, path);

                    // now the old one can go away
                    // FUTURE: based on option to func, keep bak file
                    ForceDeleteFile(bakPath);
                }
            }
            finally
            {
                try
                {
                    ForceDeleteFile(tmpPath);
                }
                catch
                {
                    // failure to cleanup a tmp file isn't critical
                }
            }

            // FUTURE: options to throw on existing/auto-overwrite
        }
    }
}
