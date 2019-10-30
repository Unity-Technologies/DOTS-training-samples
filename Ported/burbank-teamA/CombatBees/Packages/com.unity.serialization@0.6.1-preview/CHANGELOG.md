# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [0.6.1-preview] - 2019-10-25
### Fixed
* Fixed a major serialization regression for `UnityEngine.Object` derived objects.

## [0.6.0-preview] - 2019-10-25
### Changed
* ***Breaking change*** `JsonSerialization.Deserialize` will now return a disposable `VisitResult` containing logs, errors and exceptions that occurred during deserialization.
* Updated `com.unity.properties` to version `0.10.0-preview`.

### Added
* Support JSON serialization of `System.DateTime` and `System.TimeSpan`.

## [0.5.1-preview] - 2019-10-21
### Added
* Support JSON serialization of `UnityEditor.GlobalObjectId`.
* Support JSON serialization of `UnityEditor.GUID`.
* New method `DeserializeFromStream` to deserialize from stream object.

### Changed
* Updated `com.unity.properties` to version `0.9.1-preview`.
* Deserialization will now attempt to construct the destination container using `PropertyContainer.Construct` utility.
* Deserialization will now attempt to read type information field `$type` by default.

## [0.5.0-preview] - 2019-10-07
### Changed
* Updated `com.unity.properties` to version `0.9.0-preview`.

## [0.4.1-preview] - 2019-09-25
### Changed
* Updated `com.unity.properties` to version `0.8.1-preview`.

## [0.4.0-preview] - 2019-09-24
### Added
* Support JSON serialization of `UnityEngine.Object`.

### Changed
* Now requires Unity 2019.3 minimum.
* Now requires `com.unity.properties` version `0.8.0-preview`.

## [0.3.1-preview] - 2019-09-16
### Added
* Support JSON serialization of `DirectoryInfo` and `FileInfo` using string as underlying type.

## [0.3.0-preview] - 2019-08-28
### Changed
* Updated `com.unity.properties` to version `0.7.1-preview`.
* Support JSON serialization of enums using [numeric integral types](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types) as underlying type.

## [0.2.1-preview] - 2019-08-08
### Changed
* Support for Unity 2019.1.

## [0.2.0-preview] - 2019-08-06
### Changed
* `JsonVisitor` virtual method `GetTypeInfo` now provides the property, container and value parameters to help with type resolution.

## [0.1.0-preview] - 2019-07-22
* This is the first release of *Unity.Serialization*.
