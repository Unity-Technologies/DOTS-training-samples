# Changelog

## [1.3.0] - 2019-8-26

Remove metadata collectors tests
Switch to errors from exceptions when parsing results
Increase minimum unity version to 2019.3

## [1.2.6] - 2019-8-22

### Categorize performance tests as `performance`

Categorize performance tests as performance
Remove profiler section on docs as the feature was removed
ProfilerMarkers can now be called with string params
Switch measuring frames and methods to stopwatch

## [1.2.5] - 2019-6-17

### Test publish for CI

## [1.2.4] - 2019-6-17

### Test publish for CI

## [1.2.3] - 2019-6-14

### Update changelog

## [1.2.2] - 2019-6-13

### Add support for domain reload

## [1.2.1] - 2019-6-7

### Fix bug that would cause player build failures

## [1.2.0] - 2019-5-23

### Increase unity version to 2019.2

## [1.1.0] - 2019-5-22

### Update assembly definition formats to avoid testables in package manifest

## [1.0.9] - 2019-5-21

#### Update scripting runtime setting for 2019.3

## [1.0.8] - 2019-3-8

#### Automation test deploy

## [1.0.7] - 2019-3-8

#### Automation test deploy

## [1.0.6] - 2019-3-4

### Update changelog

## [1.0.5] - 2019-3-4

### Add conditional support for 2019.1

## [1.0.4] - 2019-2-18

### remove unnecessary meta files

## [1.0.3] - 2019-2-18

### package.json update

## [1.0.2] - 2019-2-18

### package.json update

## [1.0.1] - 2019-2-18

### Updated Documentation to reflect breaking changes

## [1.0.0] - 2019-2-15

### Refactor attributes

## [0.1.50] - 2019-1-15

### Change results paths to persistent data

## [0.1.49] - 2018-12-4

### Revert changes to profiler and GC

## [0.1.48] - 2018-11-22

### Doc updates and ignore GC api in editor due to api issues

## [0.1.47] - 2018-11-14

### Remove debug logs

## [0.1.46] - 2018-11-14

### Fix breaking changes introduced by testrunner API rename

## [0.1.45] - 2018-11-8

### Fix breaking changes to data submodule

## [0.1.44] - 2018-11-8

### Disable GC and update API to work around warning

## [0.1.43] - 2018-10-30

### Fix method measurements setup and cleanup

## [0.1.42] - 2018-10-15

### Improvements to report window and minor fixes

Save profiler output on perf tests
Add a button on report window to open profiler output for test
Remove unsupported features for legacy scripting runtime
Fix version attribute for test cases
Remove unnecessary assembly definition

## [0.1.41] - 2018-10-2

### Test report graph

## [0.1.40] - 2018-9-17

### Update documentation

## [0.1.39] - 2018-9-14

### Remove duplicate module from docs

## [0.1.38] - 2018-9-14

### Documentation updates

## [0.1.36] - 2018-8-27

### ProfilerMarkers now take params as arguments

## [0.1.35] - 2018-8-27

### Measure.Method improvements

Add GC allocation to Measure.Method
Add setup/cleanup for Measure.Method
Move order of calls for Measure.Scope

## [0.1.34] - 2018-8-16

### Obsolete warnings

## [0.1.33] - 2018-8-3

### Small fixes

Obsolete warnings, doc update with modules and internals, ValueSource fix

## [0.1.32] - 2018-7-9

### Add custom measurement/warmup counts

Method and Frames measurements can now specify custom warmup, measurement and iteration counts

## [0.1.31] - 2018-7-04

### mark metadata tests with performance category

## [0.1.30] - 2018-6-27

### fix Method measurement

## [0.1.29] - 2018-6-12

### Moving back to json in xml due to multiple instabilities


## [0.1.28] - 2018-6-01

### Remove json printing from output


## [0.1.27] - 2018-5-31

### Add meta files to npm ignore


## [0.1.26] - 2018-5-31

### Preparing package for moving to public registry

Inversed changelog order
Excluded CI files from published package


## [0.1.25] - 2018-5-31

### Remove missing meta files


## [0.1.24] - 2018-5-31

### Print out json to xml by default for backwards compatability


## [0.1.23] - 2018-5-30

### Issues with packman, bumping up version

Issues with packman, bumping up version


## [0.1.22] - 2018-5-29

### Measure.Method Execution and Warmup count

Can now specify custom execution and warmup count


## [0.1.21] - 2018-5-25

### Fix issues introduced by .18 fix


## [0.1.19] - 2018-5-24

### Rename package

Package has been renamed to `com.unity.test-framework.performance` to match test framework


## [0.1.18] - 2018-5-24

### Fix SetUp and TearDown for 2018.1


## [0.1.17] - 2018-5-23

### Meatada collecting and changes to method/frames measurements

Refactor Method and Frames measurements
Metadata collected using internal test runner API and player connection for 2018.3+


## [0.1.16] - 2018-5-09

### Bug fix

Bug fix regarding measureme methods being disposed twice


## [0.1.15] - 2018-5-02

### Bug fix for metadata test

The test was failing if a json file was missing for playmode tests


## [0.1.14] - 2018-4-30

### Measure method refactor

Introduced SampleGroupDefinition
Addition of measuring a method or frames for certain amount of times or for duration
Refactored measuring methods
Removes linq usage for due to issues with AOT platforms


## [0.1.13] - 2018-4-15

### Updates to aggregation and metadata for android

Fixed android metadata collecting
Removed totaltime from frametime measurements
Added total, std and sample count aggregations
Added sample unit to multi sample groups


## [0.1.12] - 2018-4-11

### Change naming and fix json serialization


## [0.1.11] - 2018-4-09

### Fix 2018.1 internal namespaces

Fix 2018.1 internal namespaces


## [0.1.10] - 2018-4-09

### Collect metadata and update coding style

Change fields to UpperCamelCase
Added editmode and playmode tests that collect metadata


## [0.1.9] - 2018-4-06

### Add json output for 2018.1

After test run, we will now print json output


## [0.1.8] - 2018-4-03

### Fix for 2018.1

Fix an exception on 2018.1


## [0.1.7] - 2018-4-03

### improvements to overloads and documentation

Changed some of the names to match new convention
Addressed typos in docs
Multiple overloads replaced by using default arguments


## [0.1.6] - 2018-3-28

### improvements to overloads and documentation

Measure.Custom got a new overload with SampleGroup
Readme now includes installation and more examples


## [0.1.5] - 2018-3-20

### Adding checks for usage outside of Performance tests

Adding checks for usage outside of Performance tests


## [0.1.4] - 2018-3-20

### Adding system info to performance test output

Preparing for reporting test data


## [0.1.3] - 2018-03-14

### Removed tests

Temporarily removing tests from the package into separate repo.


## [0.1.2] - 2018-03-14

### Bug fix

Update for a missing bracket


## [0.1.1] - 2018-03-14

### Updates to test results and measurement methods

Test output now includes json that can be used to parse performance data from TestResults.xml
Added defines to be compatible with 2018.1 and newer
Removed unnecessary overloads for measurements due to introduction of SampleGroup
Measurement methods can now take in SampleGroup as argument.


## [0.1.0] - 2018-02-27

### This is the first release of *Unity Package performancetesting*.

Initial version.
