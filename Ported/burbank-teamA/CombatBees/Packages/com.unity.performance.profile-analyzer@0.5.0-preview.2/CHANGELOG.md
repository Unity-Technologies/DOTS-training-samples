# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [0.5.0-preview.2] - 2019-09-19
* Minor documentation update to fix the changelog formatting

## [0.5.0-preview.1] - 2019-09-18

### Features 
* Added self time option to display 'exclusive' time of markers excluding time in children.
* Added ability to filter to a parent marker to reduce the marker list to a part of the tree.
* Added option to filter the column list to set groups (including custom).
* Added column for total marker time over the frame
* Added copy to clipboard on table entries
* Added export option for marker table 

### Enhancements
* Improved Top N Markers graph to make it clearer this is for the median frames of each data set.
* Added thread count display (next to marker count).
* Added frame index to more tooltips to improve clarity of the data values (marker table, frame and marker summary).
* Added additional visual bars for total and count diffs. Added abs count column.
* Improved performance of adding to include/exclude filter via the right click menu by only refreshing the table (and no longer rerunning full analysis)
* Improved performance for scrolling by caching strings for profile table and comparison table.
* Added unaccounted time into the Top N Markers graph when total is less than the median frame time
* Added grow/shrink selection hot keys and menu options
* Added tooltip info for frame time duration for selection range

### Fixes
* Fixed issue with combined marker count when data sets have some unique markers.
* Fixed bars less than 1 pixel wide to clamp to min width of 1.
* Fixed help text for new editor skin in 2019.3
* Fixed bug with calculation of the auto right depth offset (see with 2017.4/2018.4 comparisons)
* Improved the frame offset times in the frame time and comparison frame time exports
* Fixed bug with missing first frame of data / frame offset incorrect when reloading .pdata

## [0.4.0-preview.5] - 2019-04-2

* Updated package.json file to indicate this package is valid for all unity versions

## [0.4.0-preview.4] - 2019-04-2

* Fixed issue in 2017.4 with unsupported analytics API and a GUI style.

## [0.4.0-preview.3] - 2019-04-1

* First public release of Profile Analyzer. 

## [0.1.0-preview] - 2018-12-07

* This is the first beta release of Profile Analyzer

The profile analyzer tool augments the standard Unity Profiler. It provides multi frame analysis of the profiling data.
