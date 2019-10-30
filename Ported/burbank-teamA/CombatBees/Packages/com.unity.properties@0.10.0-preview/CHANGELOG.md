# Changelog
All notable changes to this package will be documented in this file.

## [0.10.0-preview] - 2019-10-25
### Changed
* ***Breaking change*** `PropertyContainer.Construct` and `PropertyContainer.Transfer` will now return a disposable `VisitResult` containing logs, errors and exceptions that occurred during visitation. 

## [0.9.2-preview] - 2019-10-21
### Added
* Added support for renamed fields using `UnityEngine.Serialization.FormerlySerializedAsAttribute` in the transfer visitor.

## [0.9.1-preview] - 2019-10-18
### Added
* Added `PropertyContainer.Construct` API call. This method can be used to initialize a tree using the default constructor for any uninitialized types.
* Added support for instantiating `UnityEngine.ScriptableObject` derived types using the `TypeConstruction` utility.

### Changed
* `PropertyContainer.Transfer` will now visit the source instead of the destination when transfering.

## [0.9.0-preview] - 2019-10-06
### Added
* Added `TypeConstruction.TryConstruct[...]` variants for instantiating types without throwing exceptions.
* Support for property drawers.

### Fixed
* Attributes on a collection will now be correctly propagated to its elements.

### Changed
* Connectors are now registered on the `BaseField<T>` directly instead of the explicit types (i.e. `IntegerField`), which will help with user defined types.
* Added `PropertyContainer.Visit` overload with `ref TVisitor`.
* ***Breaking change*** Changed all `IPropertyBag{T}.Accept` methods to pass the `TVisitor` by ref.

## [0.8.1-preview] - 2019-09-25
### Fixed
* Public fields and properties from base class will now again be reflected correctly.

## [0.8.0-preview] - 2019-09-24
### Added
* Added a `TypeConstruction` utility to allow the creation of new instances.
* Minimal unity version has been updated to 2019.3.
* Added `PropertyElement` to help with generic, property-backed UI inspectors

### Fixed
* Fixed all `PropertyContainer.Try[...]` methods to not throw exceptions when visiting nested types.
* Fixed property bag reflection duplicates when base class contains an internal field or property.

## [0.7.2-preview] - 2019-09-12
### Changed
* Exposed a default way to manually visit collection items, through `VisitCollectionElementCallback<TContainer>`

### Added
* Added `PropertyContainer.TryGetValueAtPath` and `PropertyContainer.TrySetValueAtPath`, which will try to set a value for a given `PropertyPath`.
* Added `PropertyContainer.TryGetCountAtPath` and `PropertyContainer.TrySetCountAtPath`, which will try to set the count of a collection for a given `PropertyPath`.
* Added `PropertyContainer.VisitAtPath` and `PropertyContainer.TryVisitAtPath`, which will do a partial visitation for a given `PropertyPath`.

## [0.7.1-preview] - 2019-08-29
### Fixed
* Narrowing conversions between supported enum types will not throw an `InvalidCastException` anymore. 

## [0.7.0-preview] - 2019-08-23
### Fixed
* Conversion to all supported underlying type of enums should now be supported.
* Type conversion should now work on derived types.

### Added
* Added `PropertyContainer.GetValueAtPath` and `PropertyContainer.SetValueAtPath`, which will set a value for a given `PropertyPath`.
* Added construction of a `PropertyPath` from a string (i.e. `Path.To.The.List[1].Value`).

### Changed
* ***Breaking change*** `IPropertyGetter` and `ICollectionPropertyGetter` are now passed by ref during visitation.

## [0.6.4-preview] - 2019-08-15

### Fixed
* Fixed property bag reflection for base class with private properties.
* Disabled generation of properties for reflected pointer fields in order to avoid casting errors.

## [0.6.3-preview] - 2019-08-06

### Fixed
* Fixed `System.Guid` properties `IsContainer` value to return `false`.
* Fixed property bag reflection for base class with private fields.
* Fixed property bag reflection for private properties.

## [0.6.2-preview] - 2019-07-29

### Fixed
* Fixed property bag resolution for boxed and interface types.

## [0.6.1-preview] - 2019-07-25

### Fixed
* Fixed the reflection property generator to correclty handle `IList<T>`, `List<T>` and `T[]` collection types.
* Fixed `ArgumentNullException` when visiting a null container.

## [0.6.0-preview] - 2019-07-19

### Added
* Added `[Property]` attribute which can be used on fields or C# properties. The attribute will force the reflection generator to include the member.

### Changed
* `PropertyBagResolver.RegisterProvider` has been removed and replaced with access to a static `ReflectedPropertyBagProvider`.
* `Unity.Properties.Reflection` assembly has been removed and merged with `Unity.Properties`.

### Fixed
* `TypeConverter` no longer warns if the source and destination types are the same.
* TypeConversion of enum types will now convert based on the value and not the index.
* PropertyContainer.Transfer now ensures destination type is a reference type when not passed by ref.
* Fix generated properties for `List<string>` incorrectly treating strings as container types.
* `UnmanagedProperty` can now be generated for `char` types during reflection.

## [0.5.0-preview] - 2019-04-29

### Changed
* Complete refactor of the Properties package.

<!-- Template for version sections
## [0.0.0-preview.0]

### New Features


### Upgrade guide


### Changes


### Fixes
-->
