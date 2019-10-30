# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).


## [0.1.0-preview.9] - 2019-09-30

### Changed

- Fixed incorrect return value for api validation

## [0.1.0-preview.8] - 2019-09-30

### Changed

- Removed api validation when running in batch mode
- Introduced api for invoking validation manually

## [0.1.0-preview.7] - 2019-09-19

### Changed

- Disabled scraping for non-editor builds
- Made validation insensitive to line endings

## [0.1.0-preview.6] - 2019-09-11

### Changed

- Renamed extension of API Scrapings to .api 
- Implemented pre import callback to avoid double compilation
- Fixed erro that cancelled formating when progress dialog was shown
- Moved editorconfig and shoudly dependencies to be in-project dlls until they are available in production repository

## [0.1.0-preview.5] - 2019-04-01

### Changed

- Added API Scraping
- Doc updates to README.
- Fixed the .editorconfig template to not generate errors in Rider, plus added more rules.
- Added support for 2019.1, though .2 is required to format local packages outside the project root.

## [0.1.0-preview.4] - 2019-03-21

### Changed

- Avoid unnecessary formatting after a import triggered by a previous formatting.
- Set execution permission for uncrustify in mac64/linux64 folders.
- Select uncrustify executable based on current platform.
- Fix crash when backup files were made read-only.
- Fix incompatibility with *UnityEditor.PS4.Extensions.dll* (compilation errors due to NiceIO name clashes).
- Allow users to disable auto formating (disable_auto_format option in .editorconfig).

## [0.1.0] - 2019-02-25

### This is the first release of *Unity Package \<Coding\>*
