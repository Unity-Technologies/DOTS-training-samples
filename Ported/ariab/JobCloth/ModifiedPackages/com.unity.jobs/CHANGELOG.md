# Change log

## [0.0.7-preview.14] - Next-Release-Date

### New Features
### Upgrade guide
### Changes
### Fixes


## [0.0.7-preview.13] - 2019-05-24

### Changes

* Updated dependency for `com.unity.collections` 


## [0.0.7-preview.12] - 2019-05-16

### New Features

* IJobParallelForDeferred has been added to allow a parallel for job to be scheduled even if it's for each count will only be known during another jobs execution. 

### Upgrade guide
* Previously IJobParallelFor had a overload with the same IJobParallelForDeferred functionality. This is no longer supported since it was not working in Standalone builds using Burst. Now you need to explicitly implement IJobParallelForDeferred if you want to use the deferred schedule parallel for.


## [0.0.7-preview.11] - 2019-05-01

Change tracking started with this version.

<!-- Template for version sections
## [0.0.0-preview.0]

### New Features


### Upgrade guide


### Changes


### Fixes
-->
