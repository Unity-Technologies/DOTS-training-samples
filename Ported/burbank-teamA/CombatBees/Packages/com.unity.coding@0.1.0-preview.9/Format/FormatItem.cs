using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EditorConfig.Core;
using Unity.Coding.Utils;

namespace Unity.Coding.Format
{
    [Flags]
    public enum FormatOptions
    {
        DryRun = 1 << 0,    // don't modify any files, but report which would have changed
    }

    public enum FormatterType
    {
        Generic,            // apply the generic formatter (uses default .editorconfig rules)
        Uncrustify,         // apply uncrustify tool with rule set (.ini file) chosen based on file extension
    }

    public enum FormatTriggerKind
    {
        UserInitiated,      // user explicitly started the format process
        Automatic,          // automatic formatting after detecting file has changed.
    }

    public class FormatContext
    {
        NPath m_ThisPackageRoot;

        public NPath BaseDir { get; set; } = NPath.CurrentDirectory;    // base dir used for relative paths
        public FormatOptions Options { get; set; }
        public ILogger Logger { get; set; } = DefaultLogger.Instance;
        public FormatTriggerKind TriggerKind {get; set;}

        public NPath ThisPackageRoot // root dir of this package
        {
            get => m_ThisPackageRoot ?? throw new Exception("Must configure the Unity.Coding package root in order to access its pack-in features");
            set
            {
                if (!value.DirectoryMustExist().Combine("Coding~").DirectoryExists())
                    throw new ArgumentException($"Given path is not the package root for com.unity.coding '{value}'");
                m_ThisPackageRoot = Path.GetFullPath(value).ToNPath();
            }
        }

        readonly HashSet<string> m_UsedErrors = new HashSet<string>();    // avoid spamming the same error over and over from bad editorconfig settings etc. (when formatting many files)

        public void LogError(string message)
        {
            if (m_UsedErrors.Add(message))
                Logger.Error(message);
        }
    }

    public class FormatItem
    {
        public FormatContext Context;
        public NPath Path;                              // absolute path of file we working with
        public FileConfiguration EditorConfig;          // settings from .editorconfig that apply to this file
        public int? TabWidth;                           // indent_size and tab_width are a mess to deal with - preprocess and put a single number here
        public bool? TrimTrailingNewlines;              // need this until https://github.com/editorconfig/editorconfig/issues/269
        public IEnumerable<FormatterType> Formatters;   // which formatter(s) to use, if any
        public string IgnoreHeaderPattern;              // an rx that is used to decide whether to skip a file or not based on the top HeaderLineCount lines of its contents (such as generated files)

        const string k_IgnoreIfInHeaderKey = "ignore_if_in_header";
        const string k_TrimTrailingNewlinesKey = "trim_trailing_newlines";
        const string k_FormattersKey = "formatters";
        const string k_DisableAutoFormatKey = "disable_auto_format";

        public override string ToString() => Path;

        public static FormatItem TryCreate(NPath pathToFormat, FormatContext formatContext)
        {
            // SECTION: test to see if we should even have a FormatItem

            // relative paths use the project root
            if (pathToFormat.IsRelative)
            {
                if (formatContext.BaseDir == null)
                    throw new Exception("Relative paths require a valid BaseDir to be set");
                pathToFormat = System.IO.Path.GetFullPath(formatContext.BaseDir.Combine(pathToFormat)).ToNPath();
            }

            // double-check
            pathToFormat.ThrowIfRelative();
            pathToFormat.FileMustExist();

            // if no formatters are configured, there's nothing for us to do, so ignore
            var formattersConfig = FastEditorConfigParser.ParseForSingleValue(pathToFormat, k_FormattersKey);
            if (formattersConfig.IsNullOrWhiteSpace())
                return null;

            // SECTION: now that we know we will need to format, build a FormatItem
            var hadErrors = false;
            Action<string> logError = message =>
            {
                hadErrors = true;
                formatContext.LogError(message);
            };

            // this doesn't do any caching currently. for each file, EditorConfig.Core scans up to root, gathers .editorconfigs, parses
            // them, iterates properties. perf testing will show if this needs caching.
            var editorConfig = new EditorConfigParser().Parse(pathToFormat).First();

            // we could support this, but let's disallow ancient mac \r eol, which can only be an accident if used today
            if (editorConfig.EndOfLine == EndOfLine.CR)
                logError(".editorconfig: end_of_line can only be lf or crlf");

            bool? trimTrailingNewlines = null;
            var trimTrailingNewlinesStr = editorConfig.Properties.GetValueOr(k_TrimTrailingNewlinesKey);
            if (trimTrailingNewlinesStr != null)
            {
                bool parsed;
                if (bool.TryParse(trimTrailingNewlinesStr, out parsed))
                    trimTrailingNewlines = parsed;
                else
                    logError($".editorconfig: invalid value for {k_TrimTrailingNewlinesKey}");
            }

            // should this file only be formatted explicitly?
            var disableAutoFormatStr = editorConfig.Properties.GetValueOr(k_DisableAutoFormatKey, "false");
            if (Boolean.TryParse(disableAutoFormatStr, out var disableAutoFormat))
            {
                if (formatContext.TriggerKind == FormatTriggerKind.Automatic && disableAutoFormat)
                    return null;
            }
            else
                logError($".editorconfig: disable_auto_format value should be either true or false: `{disableAutoFormatStr}`");

            // we only support indent_size == tab_width
            var tabWidth = editorConfig.TabWidth;
            if (editorConfig.IndentSize?.NumberOfColumns != null)
            {
                if (tabWidth == null)
                    tabWidth = editorConfig.IndentSize.NumberOfColumns;
                else if (tabWidth != editorConfig.IndentSize.NumberOfColumns)
                    logError(".editorconfig: indent_size and tab_width being different widths is not supported");
            }

            // tabs-to-spaces is unsupported right now in any formatter
            if (editorConfig.IndentStyle == IndentStyle.Tab)
                logError(".editorconfig: indent_style `tab` is currently unsupported");

            var hasCodeFormatter = false;
            var formatters = new List<FormatterType>();

            foreach (var formatterConfig in formattersConfig.Split(',').Select(f => f.Trim()))
            {
                var formatter = EnumUtility.TryParseNoCaseOr<FormatterType>(formatterConfig);
                if (formatter != null)
                {
                    formatters.Add(formatter.Value);
                    if (formatter != FormatterType.Generic)
                        hasCodeFormatter = true;
                }
                else
                {
                    var formatterNames = EnumUtility.GetNames<FormatterType>().Select(s => s.ToLower());
                    logError($".editorconfig: `{k_FormattersKey}` with formatter type of '{formatterConfig}' unknown (choose one of {formatterNames.StringJoin(" ")})");
                }
            }

            if (hasCodeFormatter)
            {
                // we ship formatter config files set to 4-wide spaces. it's confusing to have, say, a 2-wide .editorconfig and a 4-wide
                // uncrustify.cfg. the solution would be to pass the 2-wide setting through to uncrustify as an override. until that's done
                // we need to error to avoid the confusion. if this is annoying, add an option mask_unsupported_formatter_options or something.

                if (editorConfig.IndentSize?.NumberOfColumns != null && editorConfig.IndentSize.NumberOfColumns != 4)
                    logError(".editorconfig: indent_size must be 4 when used with a code formatter");
                if (editorConfig.TabWidth != null && editorConfig.TabWidth != 4)
                    logError(".editorconfig: tab_width must be 4 when used with a code formatter");
                if (editorConfig.MaxLineLength != null)
                    logError(".editorconfig: max_line_length is not supported with a code formatter");
            }

            FormatItem formatItem = null;

            if (!hadErrors)
            {
                formatItem = new FormatItem
                {
                    Context = formatContext,
                    Path = pathToFormat,
                    EditorConfig = editorConfig,
                    TabWidth = tabWidth,
                    TrimTrailingNewlines = trimTrailingNewlines,
                    Formatters = formatters,

                    // keep this as string, not convert to rx. want to take advantage of rx caching from static match func.
                    IgnoreHeaderPattern = editorConfig.Properties.GetValueOr(k_IgnoreIfInHeaderKey),
                };
            }

            return formatItem;
        }
    }
}
