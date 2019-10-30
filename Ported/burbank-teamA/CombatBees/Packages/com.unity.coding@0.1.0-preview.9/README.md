# The Unity Coding Support Tools Package

Tooling and configurations to ensure code is automatically formatted, encoded, and linted according to standard Unity development conventions and best practices.

The idea is that you add this to your project and *bam* all your code is formatted. In the future this package will also configure compilers to emit warnings about code that violates our code conventions. Later on, you will be able to write your own analyzers for rules that are specific to your project.

_Note to maintainers: this package should be kept in sync with tooling and configs in the [unity-meta](https://ono.unity3d.com/unity-extra/unity-meta) repo._

# Feedback channels

If you have any questions or suggestions please reach us at #devs-coding-package in Slack.

# Requirements

- As of today, this package should be added to a project, i.e, your package cannot depend on this package; instead, the recommended way to setup it is to have a *Unity Project* which depends both in your package and in the Unity Coding package (you can see an example [here](https://github.cds.internal.unity3d.com/unity/com.unity.coding/tree/master/UnityCoding))
- Unity 2019.3 or newer
- Your project need to be configured to use .Net 4.0
- One or more `.editorconfig` file(s) used to define which sources will get formatted (see below)
- (Optional but recommended) a `.gitattributes` at the root that configures eol's for certain file types so that
  - git and the formatters are aligned, avoiding annoying warnings
  - Certain tools that are picky about eol's (notably T4) work properly and smoothly

# How to install

- Create a project (if you don't have one) to host your package

- Configure ProjectRoot/Packages/manifest.json to enable the staging registry by adding the following line just after the *dependencies* node:

    `"registry": "https://staging-packages.unity.com"`

- Also in the manifest.json, add this line to the *dependencies* section

    `"com.unity.coding" : "0.1.0-preview.9"`

    > Note: If you get compilation errors, make sure your project is configured to use the new scripting runtime.
    >
    > Go to *Project Settings/Player* select *.Net 4.x equivalent* in *Scriping Runtime Version* (this option was removed in 2019.2 along with the old runtime)

- To enable formatting, copy [Coding~/Configs/EditorConfig/.editorconfig](Coding~/Configs/EditorConfig/.editorconfig) from the `com.unity.coding` package root to `ProjectRoot/.editorconfig` which will configure sources under `Assets/` and `Packages/` folders to be formatted. Copy to `PackageRoot/.editorconfig` to configure package formatting.

- Copy the `.gitattributes` from the same location as `.editorconfig` into your repo root. If you've already got one there, then append the template into it.

- To do the initial format, right-click the `Assets/` folder (or package root folder) in the Unity Editor and select `Format Code`.

# Formatting

## How to configure

The formatter uses `.editorconfig` files ([EditorConfig](https://editorconfig.org/)) to figure out which files should be formatted.

In general, given a file **F**, the formatter will process it if it finds a `.editorconfig` that matches that file. The formatter will look for that file starting at the directory where **F** is stored and all of its parents until it finds a match.

Inside the `.editorconfig` file, in a session that matches the files you want to format, add a property called **formatters** which can be set to **generic** and/or **uncrustify**, for example:

```ini
# this will only format cs files that are under "MyFolder" folder (not in subfolders)
[MyFolder/*.cs]
formatters = uncrustify, generic

# this will format cs files under "MyFolder" folder and any of its subfolders
[MyFolder/**.cs]
formatters = uncrustify, generic
```

You can also place a .editorconfig file in the subfolder, and take advantage of inheritance. There are many other ways to configure how your repo is formatted. Visit the [official site](https://editorconfig.org/) for more details about `.editorconfig`.

## How to use it

Once configured the formatter will ensure that any changed or added source will be formatted.

If you want to do a initial format (after adding UnityCoding to your project) or in cases where you tweak your `.editorconfig` file you'll need to explicitly run the formatter by right clicking the files (or folders) you want to format and selecting `Format Code` from the context menu.

If, for any reason, you want to disable auto-formating (i.e, you want to format only explicitly) you can set `disable-auto-format` to `true` in your `.editorconfig`

## Known issues

## FAQ

Q: I've changed `.editorconfig` to include some more files to be formatted, then switched back to Editor, but nothing got formatted.

A: The formatter's auto-format ability only notices when you change the source files, and doesn't try to figure out what files might be affected by a change to `.editorconfig` itself. You can just right-click the file (or a folder) you want and select `Format Code` from the context menu.

# API Scraping

The public API of your .asmdefs can be tracked in checked-in .api files which are regenerated in the editor after compilation. .api files capture the entire public API from a single assembly in the form of C#-like declarations. For more info on the .api format, see ([this confluence page](https://confluence.hq.unity3d.com/pages/viewpage.action?spaceKey=DEV&title=Tracking+API+Through+.api+and+.platform.api)).

## How to configure

The template in UnityCoding\Packages\com.unity.coding\Coding~\Configs\EditorConfig\.editorconfig is set up to scrape non-test asmdefs. Or you can add `scrape_api = true` for the `.asmdef`s you wish to scrape in your `.editorconfig`. 

Ex, from the template
```ini
[*.asmdef]
scrape_api = true

[**/Tests/**.asmdef]
scrape_api = false
```

## How to use it

Once configured, files named `<assembly name>.api` will be generated next to each configured asmdef after every compilation in the editor.

Note that if these files are updated while the editor is run with `-batchmode`, an error will be logged to the console and the editor will close with a non-zero exit code. This is to cause continuous integration builds to fail if the `.api` files are not fully up to date in the repository.