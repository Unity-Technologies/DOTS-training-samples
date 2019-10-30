# Profile Analyzer

## What Is This
The Profile Analyzer aggregates and visualises frame and marker data from a set of Unity Profiler frames to help you reason about their behaviour over a number of frames, complementing the single frame analysis already available in the Unity Profiler.


Features:
* Multi frame analysis of a single scan
  * Each marker is shown with its median, min, max values over all frames, including histogram and box and whisker plots to view the distribution
  * Various filtering options are available to limit the markers displayed by thread, call depth and name substrings.
  * Data can be sorted for each of the displayed values.
* Comparison of two multi frame profile scans
  * Each marker is shown with the difference in values between the scans, including with a visualisation to help quickly identify the key differences.
  * Supports comparison of scans from two different Unity versions, or before and after an optimization is applied.

## How To Run
Add the Profile Analyzer folder to your Unity project or install as a package.
The 'Profile Analyzer' tool is opened via the menu item below the 'Window/Analysis' Menu in the Unity menu bar (or just in the 'Window' menu prior to 2018.1).

## Capturing Data
Use the standard Unity Profiler to record profiling data from your application.
In the Profile Analyzer pull the profiler data from the Unity Profiler.
The profile data will be analyzed and appear in the single view along with both sides of the Compare View.
This capture can be saved as a .pdata file for later comparision or sharing with others.

## Comparing Two Data Sets
Pull data or load a previous .pdata file into the Left and Right slots to compare the two sets, comparison results will instantly appear.

## More Information
For more information on the UI and common workflows please see the [full documentation](Documentation~/index.md).


