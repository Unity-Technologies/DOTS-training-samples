# TODO

## Don't forget to...

- Update README and CHANGELOG

Ensure compliant with design guidelines:

- [Human Interface Guidelines](https://unitytech.github.io/unityeditor-hig/topics/checklist.html)
- [Package Design Guidelines](https://confluence.hq.unity3d.com/pages/viewpage.action?spaceKey=DEV&title=Package+Design+Guidelines)

## Near Term

    * Use `Assert.Ignore` to disable test suites in the editor when running under nunit and not unity
        * Try just calling a Unity method and see if MissingMethodException (or whatever) happens
        * Add a ^ TODO for "make this an Application.isUnity function"

## Soon

    * Write performance tests (generate 1000 files, format all; change a few files, detect and update via assetdatabase callback..)

## Eventually

    * Get NiceIO changes integrated into github.com/scottbilas/NiceIO (also add tests, and PR upstream)

## Future

    // update menu command to "format all" with preview option w/ confirmation
    // - needed because of external editors, post-pull, etc.
    // - implies batch mode and hash-based caching for performance
    // detect package being added to a project
    // - copy template .editorconfig to root (includes encoding and format rules)
    // - initiate "format all" (preview-first version)
    // move old file to backup directory or perhaps side by side with .unc.cs~ ext
    // profiling stats, telemetry, tooling reporting ("usage data")
    // versioning (code, toolchain, package)
    // logging
    // help
    // check file is a real file, not a symlink or folder or missing or whatev
    // command line version that can be called from a vscode extension etc.
    // - implies we can't assume that formatter exclusively called on in-project files
    // document publish process (involves copying unc bins and cfgs from unity-meta and bumping versions)
    // editorconfig setting to say "format with this ruleset". example:
    // [*.cs]
    // unity-format = cs # default-expands to Packages/com.unity.coding/Coding~/Configs/Uncrustify/$name.ini
    // ^^ this implies that if we cannot find an editorconfig setting for the current file, we don't format it.
    //    which would mean an editor telling us to format a file on the desktop or whatever will get ignored.
    //    interesting test case where we have a file from a different unity.coding-enabled project (with an older
    //    unity.coding toolchain version) is being formatted by this one. do it anyway? try to find the other older
    //    package and have it do it? (obviously this isn't worth worrying about now or maybe ever)
    // path validation
    // static analysis and jetbrains command line tools, roslyn analyzers
    // analysis rules for "nocommit" "nocheckin" "todo" etc.
    // avoid files with git or hg conflict markers, or embedded "this file automatically generated" (probably a few rx's can do this)
    // only output format patch, don't modify files
    // feed back progress while running processes, give ability to cancel
    // ability to turn off on-demand
    // generic formatting for all text files, uncrustify formatting for cs cpp etc.
    // do something about "Editor Settings" option "Line Endings for New Scripts"
    // special "unity developer" option that cranks the requirements and validation. For example requires an .editorconfig that matches our standard one.
    // on unspecified end_of_line or charset, detect and preserve existing
    // detect binary file and ignore
