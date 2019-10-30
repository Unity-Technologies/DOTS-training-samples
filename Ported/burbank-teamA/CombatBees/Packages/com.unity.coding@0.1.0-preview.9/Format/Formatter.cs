using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using EditorConfig.Core;
using Unity.Coding.Editor;
using Unity.Coding.Utils;

namespace Unity.Coding.Format
{
    public static class Formatter
    {
        const int k_HeaderLineCount = 20;
        const int k_DefaultTabWidth = 4;

        public static IDictionary<string, long> Process(IEnumerable<NPath> pathsToFormat, FormatContext formatContext, Func<FormatItem, bool> cancellableBeforeFormatCallback = null)
        {
            if (formatContext == null)
                formatContext = new FormatContext();

            if ((formatContext.Options & FormatOptions.DryRun) != 0)
                throw new NotImplementedException("DryRun not supported yet");

            var formatted = new Dictionary<string, long>();
            var formatItems = new List<FormatItem>();
            foreach (var path in pathsToFormat)
            {

                FormatItem item;
                if (!File.Exists(path))
                {
                    formatContext.Logger.Error($"File {path} doesn't exist'");
                    continue;
                }
                try
                {
                    item = FormatItem.TryCreate(new NPath(path), formatContext);
                }
                catch (Exception inner)
                {
                    var outer = new Exception($"Fatal occurred while configuring formatting for '{path}' (see inner exception for details)", inner);
                    outer.Data["pathToFormat"] = path.ToString();
                    throw outer;
                }
                if(item == null)
                    continue;

                if(!CheckHeader(item))
                    continue;
                formatItems.Add(item);
                if (cancellableBeforeFormatCallback != null &&
                    cancellableBeforeFormatCallback.Invoke(item))
                {
                    return formatted;
                }
            }

            if (formatItems.Any())
            {
                // avoid requiring ThisPackageRoot if we are only using the Generic formatter, which has no external
                // tooling dependencies
                if (formatItems.SelectMany(fi => fi.Formatters).Any(f => f != FormatterType.Generic))
                    EnsureFilePermissionsOnUnixLikeOS(formatContext.ThisPackageRoot);

                // FUTURE: this is a good spot to group by formatter, then let the formatter segment (for example,
                // uncrustify would want to segment by associated .ini file), so we can do batching with response
                // files to avoid per-file process spawn overhead.

                var sw = Stopwatch.StartNew();
                foreach (var formatItem in formatItems)
                {
                    if (cancellableBeforeFormatCallback != null &&
                        cancellableBeforeFormatCallback.Invoke(formatItem))
                        break;

                    if (Process(formatItem))
                        formatted[formatItem.Path] = File.GetLastWriteTime(formatItem.Path).Ticks;
                }

                sw.Stop();
                formatContext.Logger.Debug($"Formatted {formatItems.Count} files. Took {sw.ElapsedMilliseconds}ms");
            }

            return formatted;
        }

        public static void Process(NPath pathToFormat, FormatContext formatContext)
            => Process(pathToFormat.WrapInEnumerable(), formatContext);

        public static bool Process(FormatItem formatItem)
        {
            var (text, lines) = ReadLines(formatItem.Path);

            foreach (var formatter in formatItem.Formatters)
            {
                switch (formatter)
                {
                    case FormatterType.Uncrustify:
                        if (!FormatUncrustify(formatItem, lines))
                            lines = null;
                        break;
                    case FormatterType.Generic:
                        FormatGeneric(formatItem, lines);
                        break;
                    default:
                        throw new InvalidOperationException($"Missing handler for formatter type {formatter.ToString().ToLower()}");
                }

                // abort the whole thing if any formatter failed
                if (lines == null)
                    break;
            }

            var formatted = false;
            if (lines != null)
            {
                var eol = formatItem.EditorConfig.EndOfLine;

                // have to detect eol (note that this will unavoidably normalize eol's, but that's a good thing
                if (eol == null)
                    eol = StringUtility.DetectEolType(text) == "\n" ? EndOfLine.LF : EndOfLine.CRLF;

                formatted = OverwriteFileIfChanged(formatItem, lines, eol.Value);
            }

            return formatted;
        }

        public static bool HasEditorConfigInDirectoryTree(IEnumerable<NPath> paths)
        {
            var folders = paths.Select(p => p.Parent);
            foreach (var folder in folders)
            {
                if (folder.FileExists(".editorconfig") || folder.ParentContaining(".editorconfig") != null)
                {
                    return true;
                }
            }
            return false;
        }

        static bool CheckHeader(FormatItem formatItem)
        {
            if (formatItem.IgnoreHeaderPattern.IsNullOrEmpty())
                return true;

            var header = File.ReadLines(formatItem.Path).Take(k_HeaderLineCount).StringJoin('\n');
            try
            {
                if (Regex.IsMatch(header, formatItem.IgnoreHeaderPattern))
                    return false;
            }
            catch (ArgumentException e)
            {
                formatItem.Context.LogError($".editorconfig: bad regex '{formatItem.IgnoreHeaderPattern}' (error is '{e.Message}')");
                return false;
            }

            return true;
        }

        static readonly Regex k_LineSplitRegex = new Regex("\r\n|\n|\r", RegexOptions.Compiled);

        static (string text, List<string> lines) ReadLines(NPath path)
        {
            // TODO: handle encoding-guessing just like Format\Formatter\Generic.pm does
            // TODO: handle binary-guessing and fail

            var text = File.ReadAllText(path);

            // split ourselves rather than using File.ReadLines() so we avoid the ambiguity of a file that is or isn't eol-terminated
            return (text, k_LineSplitRegex.Split(text).ToList());
        }

        static void WriteLines(Stream stream, IEnumerable<string> lines, Charset? charSet, EndOfLine endOfLine)
        {
            Encoding encoding;
            switch (charSet)
            {
                // TODO: write byte-to-byte comparison tests of the various supported encodings
                case Charset.Latin1: encoding = Encoding.ASCII; break;
                case Charset.UTF16BE: encoding = Encoding.BigEndianUnicode; break;
                case Charset.UTF16LE: encoding = Encoding.Unicode; break;
                case Charset.UTF8BOM: encoding = Encoding.UTF8; break;

                case Charset.UTF8:
                case null:
                    encoding = new UTF8Encoding(false);
                    break;

                default:
                    throw new InvalidOperationException($"Unexpected charset {charSet}");
            }

            var eolText = endOfLine == EndOfLine.CRLF ? "\r\n" : "\n";
            var eolBytes = encoding.GetBytes(eolText);

            using (var writer = new BinaryWriter(stream, encoding, true))
            {
                // write newlines between each line. if we want to ensure a trailing newline at end of file, then we
                // expect an extra blank line to have been added by the formatter.
                var first = true;
                foreach (var line in lines)
                {
                    // note: can't just call writer.Write(line) because it stores length as well. need to store bytes.

                    if (first)
                        first = false;
                    else
                        writer.Write(eolBytes);

                    writer.Write(encoding.GetBytes(line));
                }
            }
        }

        static bool OverwriteFileIfChanged(FormatItem formatItem, IEnumerable<string> lines, EndOfLine eol)
        {
            // write to a buffer so we can do an exact comparison vs the file already on disk. let's avoid generating io writes if
            // there is no actual change. but we need to have the raw bytes so that differences in encoding and EOL's are not masked.

            var newFileBuffer = new MemoryStream();
            WriteLines(newFileBuffer, lines, formatItem.EditorConfig.Charset, eol);
            var newFileBytes = newFileBuffer.GetBuffer();
            var newFileLength = (int)newFileBuffer.Length; // not newFileBytes.Length!

            var match = new FileInfo(formatItem.Path).Length == newFileLength; // do cheap length check first
            if (match)
            {
                var oldFileBytes = File.ReadAllBytes(formatItem.Path);

                // must do the byte compare vs disk
                for (var i = 0; i < newFileLength; ++i)
                {
                    if (newFileBytes[i] != oldFileBytes[i])
                    {
                        match = false;
                        break;
                    }
                }
            }

            // ok we have to write it
            if (!match)
            {
                // TODO: copy the permission bits over ($mode & 0777) to the new file
                // TODO: backup under ./Temp/Format (subfolders? replace folder names with _?) configurable via a new FormatContext.BackupRoot
                SafeFile.AtomicWrite(formatItem.Path, writePath =>
                {
                    using (var writeFile = File.OpenWrite(writePath))
                        writeFile.Write(newFileBytes, 0, newFileLength);
                });
            }

            return !match;
        }

        static void FormatGeneric(FormatItem formatItem, IList<string> lines)
        {
            var buffer = new StringBuilder();

            for (var i = 0; i < lines.Count; ++i)
            {
                var line = lines[i];

                // always strip utf8-bom's we see (can re-add later at write time if wanted). we do this per-line
                // because we have seen instances of naive tools doing file concatenation in non-encoding-aware ways,
                // resulting in mid-file bom's. should be completely safe to strip this sequence from anywhere,
                // as it never will represent a valid sequence.
                line = line.Replace("\xEF\xBB\xBF", "");

                // trim here just in case BOM was at end of line and preceded by whitespace
                if (formatItem.EditorConfig.TrimTrailingWhitespace == true)
                    line = line.TrimEnd();

                if (formatItem.EditorConfig.IndentStyle == IndentStyle.Space || formatItem.TabWidth != null)
                    line = line.ExpandTabs(formatItem.TabWidth ?? k_DefaultTabWidth, buffer);

                lines[i] = line;
            }

            if (formatItem.TrimTrailingNewlines == true)
            {
                while (lines.Count > 1 && lines.Last().IsEmpty())
                    lines.DropBack();
            }

            // only add the final newline if there isn't one, and it's been requested to add one
            if (formatItem.EditorConfig.InsertFinalNewline == true && (lines.Count == 1 || lines.Last().Any()))
                lines.Add("");
        }

        static bool FormatUncrustify(FormatItem formatItem, List<string> lines)
        {
            var packageRoot = formatItem.Context.ThisPackageRoot;
            var uncrustifyPath = packageRoot.Combine(UncrustifyRelativePathForCurrentPlatform());
            var settingsPath = packageRoot.Combine($"Coding~/Configs/Uncrustify/{formatItem.Path.ExtensionWithoutDot.ToLower()}.ini");

            var tempFormatPath = formatItem.Path.ChangeExtension(".tmp" + formatItem.Path.ExtensionWithDot);
            try
            {
                using (var file = File.Create(tempFormatPath))
                    WriteLines(file, lines, Charset.UTF8, EndOfLine.LF);

                var args = new[]
                {
                    "-L1,2",                // only want warnings and errors, and no status
                    "-c", settingsPath,     // config file to use
                    "-f", tempFormatPath,   // file to process
                    "-o", tempFormatPath,   // overwrite the thing
                };

                var stderr = new List<string>();
                var exitCode = ProcessUtility.ExecuteCommandLine(uncrustifyPath, args, null, null, stderr);

                // we should be able to use `--no-backup` but uncrustify complains that this can't be used with -f/-o. we're already working with a tmp
                // file to get atomic rewrite as well as avoid updating a file that doesn't change (so timestamp not affected).
                // https://gitlab.cds.internal.unity3d.com/upm-packages/core/com.unity.coding/issues/3
                tempFormatPath.Parent.Combine(tempFormatPath.FileName + ".unc-backup.md5~").DeleteIfExists();
                tempFormatPath.Parent.Combine(tempFormatPath.FileName + ".unc-backup~").DeleteIfExists();

                if (stderr.Any())
                {
                    formatItem.Context.LogError($"Errors encountered while formatting '{formatItem.Path}':\n  {stderr.StringJoin("\n  ")}");
                    return false;
                }

                if (exitCode != 0)
                {
                    formatItem.Context.LogError($"Fatal encountered while formatting '{formatItem.Path}'\n(Uncrustify gave an exit code of 0x{exitCode:X})");
                    return false;
                }

                lines.SetRange(ReadLines(tempFormatPath).lines);
            }
            finally
            {
                tempFormatPath.DeleteIfExists();
            }

            return true;
        }

        static void EnsureFilePermissionsOnUnixLikeOS(NPath packageRoot)
        {
            // only necessary on unixes
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return;

            var executablePath = packageRoot.Combine(UncrustifyRelativePathForCurrentPlatform());
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var ret = NativeUnix.GetFileMode(executablePath, out var originalPermission);
                if (ret != 0)
                {
                    throw new Exception($"Unable to get execution permission for `{executablePath}`. Error code: {ret}.\nFormatting with this formatter will not work.\nYou may need to set the permission manually by running `chmod u+x {executablePath}` in a console.");
                }

                // set permission for owner to run
                ret = NativeUnix.SetFileMode(executablePath, originalPermission | NativeUnix.UnixFilePermissions.S_IXUSR);
                if (ret != 0)
                {
                    throw new Exception($"Unable to set execution permission on `{executablePath}`. Error code: {ret}.\nFormatting with this formatter will not work.\nYou may set the permission manually by running `chmod u+x {executablePath}` in a console.");
                }
            }
        }

        static string UncrustifyRelativePathForCurrentPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return "Format/Uncrustify~/bin/win32/uncrustify.exe";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return "Format/Uncrustify~/bin/mac64/uncrustify";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return "Format/Uncrustify~/bin/linux64/uncrustify";

            throw new NotSupportedException($"Platform {RuntimeInformation.OSDescription} is not supported by the formatting tools.");
        }
    }
}
