# Tips and Troubleshooting

## How are the statistics represented?
Distribution of frame and marker time is shown as a [histogram](https://en.wikipedia.org/wiki/Histogram) and [box and whisker plot](https://en.wikipedia.org/wiki/Box_plot), these graphs help visualise how frame and marker time is distributed across all the individual instances in the data set.

## Single View

### **Even distribution**
Here the graphs show a distribution of marker calls that range from 14.27ms to 35.50ms. The *histogram* shows that many of the buckets are being hit and at a fairly even amount. This is also evident in the *box and whisker plot* where the box is quite large and appears towards the middle of the upper and lower bounds.

![Distribution.](images/distribution-wide.png)

### **Outlier**
Here the graphs show a distribution of marker calls that range from 48.22ms to 259.39ms. The *histogram* shows that the lower end buckets are being used most and only some the more expensive buckets being hit. This is also evident in the *box and whisker plot* where the box appears towards the bottom of the range but the whisker or upper bound of the range is quite far away.

![Distribution.](images/distribution-outlier.png)

## Compare View
### **Similar Distribution**
Here you can see two distributions that are similar, both the *histograms* and *box and whisker plots* show a very similar pattern, using this information we could reason that the marker activity in both sets is similar.

![Distribution.](images/distribution-similar.png)

### **Different Distribution**
Here you can see two distributions that are quite different, both the *histograms* and *box and whisker plots* show our marker in the left (blue) data set to be running for longer using the more expensive buckets of the *histogram* and drawn higher up the range of the *box and whisker plot*, using this information we could reason that the marker activity in left (blue) data set is more costly and worthy of further investigation.

![Distribution.](images/distribution-different.png)

### **Overlapping Distribution**
Here you can see two distributions that are similar, both have the same lower bound and have some overlap in the middle of the range but the right (orange) dataset is also using some of the more expensive buckets and having an higher upper bounds, using this information we could reason that the activity in right (orange) data set is more costly or is being called more times and worthy of further investigation.


![Distribution.](images/distribution-trending-longer.png)

[Back to manual](manual.md)