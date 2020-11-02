# Changelog

## [Unreleased]
### Changed
* Entities Window: Improved search field
* Updated package `com.unity.entities` to `0.17.0-preview.21`
* Updated package `com.unity.properties` to `1.6.0-preview`
* Updated package `com.unity.properties.ui` to `1.6.1-preview`
* Updated package `com.unity.serialization` to `1.6.1-preview`
* Minimum Unity version required is now `2020.1.2f1`

### Fixed
* Entities Window: Selecting entities now respects the locked state of the inspector
* Entities Window: Selecting entities is now fully undoable
* Entities Window: Fix performance issue forcing the UI to refresh when no actual change needed to be rendered
* Entities Window: Include Disabled and Prefab entities in results when filtering by component types
* Entities Window: Fix issue with items wrongly showed as selected

### Added
* Systems Window: Add support for unmanaged systems

## [0.11.0] - 2020-10-06
### Added
* Added a new backend for inspecting an `Entity`. It is used by default when installing the `com.unity.dots.editor` package, but it can be turned on and off through the `DOTS Editor` preferences menu.
* Entities window: Added autocompletion for component type when filtering entities with `c:...`

### Changed
* Systems window: Now shows a message when no system match the requested search
* Systems Window: Now shows an error message when failing to resolve a component type name when typing `c:mycomponent`
* Updated package `com.unity.entities` to `0.16.0-preview.18`
* Updated package `com.unity.jobs` to `0.7.0-preview.15`
* Updated package `com.unity.properties` to `1.5.0-preview`
* Updated package `com.unity.properties.ui` to `1.5.0-preview`
* Updated package `com.unity.serialization` to `1.5.0-preview`

### Fixed
* Entities Window: Fixed an issue where the search filter would not be applied after changing the selected world
* Entities Window: Improved error handling when incrementally updating the view model, fixing native memory leaks
* Entities Window: Now properly unsubscribes to selection change event
* Entities Window: Fixed a null reference issue when selecting an entity
* Systems Window: Fixed an issue where the details section contents keep changing while searching
* Systems Window: Fixed an issue where searching returns the wrong results
* Systems Window: Fixed an issue where the details section can sometimes hide the selected system
* Systems Window: Fixed an issue where deselecting a component would remove the substring from the entire search string

## [0.10.0] - 2020-08-14
### Changed
* Entities window: Improved change detector performance
* Entities window: Added specific view when no world exists
* Systems window: Added specific view when no world exists
* Updated package `com.unity.entities` to `0.14.0-preview.18`
* Updated package `com.unity.jobs` to `0.5.0-preview.14`
* Updated package `com.unity.properties` to `1.4.3-preview`
* Updated package `com.unity.properties.ui` to `1.4.3-preview`
* Updated package `com.unity.serialization` to `1.4.3-preview`
* Updated package `com.unity.test-framework.performance` to `2.3.1-preview`

### Fixed
* Systems window: Removed unnecessary code running after the window is closed

## [0.9.0] - 2020-07-24
### Changed
* Updated package `com.unity.entities` to `0.13.0-preview.24`
* Updated package `com.unity.jobs` to `0.4.0-preview.18`
* Updated package `com.unity.properties` to `1.3.1-preview`
* Updated package `com.unity.properties.ui` to `1.3.1-preview`
* Updated package `com.unity.serialization` to `1.3.1-preview`
* Updated package `com.unity.burst` to `1.3.2`
* Minimum Unity version required is now 2020.1.0b15

## [0.8.2] - 2020-07-21
### Fixed
* Entities Window: Fixed entity selection in Unity 2020.2

## [0.8.1] - 2020-07-20
### Fixed
* Entities Window: Now supports non-default world bootstrappers (works with netcode setup and custom user world bootstrappers)
* Entities Window: Inspector now clears selection when deselecting an entity
* Entities Window: Window selection now clears when selecting something else in the editor
* Entities Window: Search by any component type instead of only `IComponentData` and `ISharedComponentData`
* Entities Window: Made component types case insensitive in search
* Entities Window: Now supports duplicate component type name (if multiple types are found all of them will be used in the search)
* Entities Window: Now always shows results as a listview instead of a mix of treeview/listview based on the search
* Entities Window: Now shows an error message when failing to resolve a component type name when typing `c:mycomponent`
* Entities Window: Now shows a message when no entity match the requested search
* Entities Window: Changed visual elements height from 20px to 16px
* System Window: Fixed the issue with cropped system name becoming un-cropped for a moment upon tree view refresh
* System Window: Changed visual elements height from 20px to 16px
* Fixed restoration of the previously selected world after domain reload

## [0.8.0] - 2020-06-18
### Added
* Entities window was added, accessible through Window > DOTS > Entities

### Fixed
* Systems Window: Fixed systems not appearing in non-default world
* Systems Window: Fixed system name/entityCount/runningTime label misalignment
* Systems Window: Fixed issue where adding a system in multiple groups would log errors
* Fixed constant inspector repainting triggered by selecting a non-converted GameObject

### Changed
* Systems Window: The search field is now visible by default and will persist its state across domain reloads
* Systems Window: Updated disabled systems with "-" instead of 0 for entity matching count and running time
* Systems Window: The window can now be opened under 'Window > DOTS > Systems' instead of 'Window > DOTS > Systems Schedule'
* Systems Window: Removed "Show Inactive Systems" option from settings menu so that all systems are shown
* Updated package `com.unity.entities` to `0.11.1-preview.4`
* Updated package `com.unity.jobs` to `0.2.10-preview.12`

## [0.7.0] - 2020-05-25
### Changed
* Updated System windows to be public
* Updated minimum Unity version to 2019.3.12f1
* Updated package `com.unity.entities` to `0.11.0-preview.7`
* Updated package `com.unity.jobs` to `0.2.10-preview.11`
* Updated package `com.unity.burst` to `1.3.0-preview.12`

## [0.6.0] - 2020-05-01
### Added
* Added package `com.unity.test-framework.performance` version `2.0.8-preview` as a dependency

### Changed
* Updated package `com.unity.entities` from `0.9.0-preview.6` to `0.10.1-preview.6`
* Updated package `com.unity.properties` from `1.1.1-preview` to `1.2.0-preview`
* Updated package `com.unity.serialization` from `1.1.1-preview` to `1.2.0-preview`
* Updated package `com.unity.properties.ui` from `1.1.1-preview` to `1.2.0-preview`
* Updated package `com.unity.jobs` from `0.2.8-preview.3` to `0.2.10-preview.3`
* Updated package `com.unity.burst` from `1.3.0-preview.7` to `1.3.0-preview.10`

## [0.5.1] - 2020-04-27
### Changed
* Updated package dependencies

## [0.5.0] - 2020-04-09
### Changed
* Updated package dependencies

## [0.4.0] - 2020-03-17
### Changed
* Updated package dependencies

## [0.3.0] - 2020-01-17
### Changed
* Updated package dependencies

### Fixed
* Fixed multi-selected GameObjects conversion toggle issue
* Fixed inaccurate conversion message

## [0.2.0] - 2020-01-14
### Changed
* Updated package dependencies

## [0.1.0] - 2019-11-26
### Added
* Added Entity Conversion preview window to the inspector when a GameObject in a subscene is selected
* Added a "ConvertToEntity" checkbox to enable and disable GameObject to Entity conversion
* Added redundant "ConvertToEntity" component warning in Subscenes
