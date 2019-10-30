# About our EditorConfig files

[EditorConfig](http://editorconfig.org) is a standard way of specifying typical file encoding rules such as tabs, end-of-lines, and trailing whitespace removal. We have extended EditorConfig with several properties custom to this package.

We also include in the package various templates and config files that apply or support Unity's coding conventions standards. For EditorConfig, the `.editorconfig` in this directory is used as a template to ensure we remain consistent across packages.

## EditorConfig properties

### Standard properties: Fully Supported

- **end_of_line** (defaults to lf)
- **charset** (defaults to utf-8)
- **trim_trailing_whitespace**
- **insert_final_newline**
- **root**

TODO: The 'defaults' above are not to official editorconfig spec. If a value is not configured, the formatter should try to detect and preserve the existing file's state.

### Standard properties: Partially Supported

- **indent_style** (only `space` is supported)
- **indent_size**, **tab_width** (only `4` is supported)

TODO: implementation of these is easy in the EditorConfig formatter, but need to pass along the config option to the custom code formatter as well.

### Custom properties

- **trim_trailing_newlines**

  See https://github.com/editorconfig/editorconfig/issues/269 for background. Every editor supports an option like this, but it's not part of official spec (yet). We support it.

- **ignore_if_in_header**

  This is a regex that can be used to decide whether a file should be skipped from all formatting if it matches in the header (currently 20 lines are checked). This is a good way to detect and skip machine-generated files that cannot easily be excluded via globbing.

- **formatters**

  Can be set to assign one or more formatters (comma-delimited) to run before the EditorConfig pass. Formatters are run in the order specified here.

- **disable_auto_format**

  If set, any auto-formatting that the tooling may do in response to a user-changed file is disabled. Manually forcing formatting to run will still work.

## Formatters

This package currently supports two formatters: `generic` and `uncrustify`.

- **generic**

  The generic formatter does simple cleanup based on .editorconfig properties that normally only the editor would apply on a save. It can apply all of the properties listed above.

  When using a language-specific formatter (such as Uncrustify) it's a good idea to set the generic formatter to also run, to ensure the exact .editorconfig settings are applied to the resulting file.
  
  Note that the generic formatter should always be _last_ after any language-specific formatters, otherwise it may silently stomp on things that require syntax awareness. For example it would replace tabs with spaces inside of verbatim C# strings, which is something Uncrustify would detect and catch first.

- **uncrustify**

  The Uncrustify rules to format files in Unity standard convention live in this package under `com.unity.coding/Coding~/Configs/Uncrustify`.
  
  We use a build of Uncrustify from a custom fork that Unity maintains on [GitHub](https://github.com/Unity-Technologies/uncrustify). Binaries for supported platforms are kept in this package under `com.unity.coding/Format/Uncrustify/bin`.
