# Change log

## [0.1.2] - 2019-12-31

**This version requires Unity 2019.3 0b6+**

### New Features

* New `Native(Multi)HashMap.GetKeyValueArrays` that will query keys and values
  at the same time into parallel arrays.
* Added UnsafeStream, providing functionality of NativeStream container but
  without any safety mechanism (intended for advanced users only).

### Changes

* Updated dependencies for this package.

### Fixes

* Fixed NativeQueue pool leak.


## [0.1.1] - 2019-08-06

### Fixes

* `NativeHashMap.Remove(TKey key, TValueEQ value)` is now supported in bursted code.
* Adding deprecated `NativeList.ToDeferredJobArray()` back in - Use `AsDeferredJobArray()`
  instead. The deprecated function will be removed in 3 months. This can not be auto-upgraded
  prior to Unity `2019.3`.
* Fixing bug where `TryDequeue` on an empty `NativeQueue` that previously had enqueued elements could leave it in
  an invalid state where `Enqueue` would fail silently afterwards.

### Changes

* Updated dependencies for this package.


## [0.1.0] - 2019-07-30

### New Features

* NativeMultiHashMap.Remove(key, value) has been addded. It lets you remove
  all key & value pairs from the hashmap.
* Added ability to dispose containers from job (DisposeJob).
* Added UnsafeList.AddNoResize, and UnsafeList.AddRangeNoResize.
* BlobString for storing string data in a blob

### Upgrade guide

* `Native*.Concurrent` is renamed to `Native*.ParallelWriter`.
* `Native*.ToConcurrent()` function is renamed to `Native*.AsParallelWriter()`.
* `NativeStreamReader/Writer` structs are subclassed and renamed to
  `NativeStream.Reader/Writer` (note: changelot entry added retroactively).

### Changes

* Deprecated ToConcurrent, added AsParallelWriter instead.
* Allocator is not an optional argument anymore, user must always specify the allocator.
* Added Allocator to Unsafe\*List container, and removed per method allocator argument.
* Introduced memory intialization (NativeArrayOptions) argument to Unsafe\*List constructor and Resize.

### Fixes

* Fixed UnsafeList.RemoveRangeSwapBack when removing elements near the end of UnsafeList.
* Fixed safety handle use in NativeList.AddRange.


## [0.0.9-preview.20] - 2019-05-24

### Changes

* Updated dependencies for `Unity.Collections.Tests`


## [0.0.9-preview.19] - 2019-05-16

### New Features

* JobHandle NativeList.Dispose(JobHandle dependency) allows Disposing the container from a job.
* Exposed unsafe NativeSortExtension.Sort(T* array, int length) method for simpler sorting of unsafe arrays
* Imporoved documentation for `NativeList`
* Added `CollectionHelper.WriteLayout` debug utility

### Fixes

* Fixes a `NativeQueue` alignment issue.


## [0.0.9-preview.18] - 2019-05-01

Change tracking started with this version.

<!-- Template for version sections
## [0.0.0-preview.0]

### New Features


### Upgrade guide


### Changes


### Fixes
-->
