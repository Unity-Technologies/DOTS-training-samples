# Workflows
## Ordering Frames by Length

This workflow will take you through the steps needed to arrange the pulled frame data into a frame length ordered set with the shortest frames to the left of the x-axis and longer frames ordered further to the right creating and s-curve style graph. This groups frames with similar performance together enabling sub ranges to be made over a set of similar performing frames without the inclusion of outlier frames, useful to further normalize the data analyzed and to focus only on the shortest, longest or average frames in the data set(s).

### 1. Collect performance data to analyze
1. Switch to the [Single View](single-view.md) or [Compare View](compare-view.md) using the associated button from the views toolbar.

2. Follow the instructions in the [collecting and viewing data](collecting-and-viewing-data.md) workflow, in the case of the Compare View make sure you pull data for both the upper (*left hand*) and lower (*right hand*) data sets.

## Single View
#### Ordered by Frame Number
![FilterSystem.](images/single-view-frame-control-order-by-frame-number.png)

#### Ordered by Frame Time
![FilterSystem.](images/single-view-frame-control-order-by-frame-length.png)

### 2. Ordering and selecting the frames of interest
1. In the [Frame Control](frame-range-selection.md), right click to access its context menu and select _Order by Frame Time_.

2. Select a range of frames with similar performance from the middle of the distribution by using *Left Mouse Button Down -> Drag -> Left Mouse Button Up*.

![FilterSystem.](images/single-view-frame-control-order-by-frame-length-range.png)

The Single view will now be filtered to only data from the frames sharing similar performance from the middle of data set.

## Compare View
### Ordered by Frame Number
![FilterSystem.](images/compare-view-frame-control-order-by-frame-number.png)

### Ordered by Frame Time
![FilterSystem.](images/compare-view-frame-control-order-by-frame-length.png)

### 2. Ordering and selecting the frames of interest
1. In the [Frame Control](frame-range-selection.md), right click to access its context menu and select _Order by Frame Time_.

2. Select a range of frames with similar performance from the middle of the distribution by using *Left Mouse Button Down -> Drag -> Left Mouse Button Up*.

![FilterSystem.](images/compare-view-frame-control-order-by-frame-length-range.png)

The Compare view will now be filtered to only data from the frames sharing similar performance from the middle of data sets.

[Back to manual](manual.md)