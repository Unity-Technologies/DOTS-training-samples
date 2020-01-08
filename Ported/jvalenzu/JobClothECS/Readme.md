## basics

* Three phases
  * constraint simulation (serially process over almost all the edges)
  * a mesh vertex update (gravity applied, with some simplified collision and response at plane y=0)
  * update of the render mesh
  
The contraint simulation job, ClothBarSimJob, processes serially because the graph between vertices is connected so partitioning into separate simulation islands is non-trivial.

Played around with a couple of different modelings of the mesh vertex update.

* IJobParallelOverVertices goes wide for every mesh instance.
* IJob creates a single job per mesh instance.

## timings

These numbers reflect a snapshot in time on a specific machine (i9-9900K 3.6Ghz 16 cores), they may not accurately reflect the current code.

Job type          | instances | worker threads | ClothSetup cost (ms) | ClothBarSimJob (ms) | ClothSimVertexJob (ms)
----------------------------------------------------------------------------------------------------------------
IJob              |      4369 |             15 |                ~5.8  |               ~4.95 | 0.86
IJobParallelBatch |      4369 |             15 |               ~17.16 |               ~5.80 | 12.20
IJob              |         1 |             15 |                ~0.03 |               ~0.04 | 0.01
IJobParallelBatch |         1 |             15 |                ~0.19 |               ~0.13 | 0.34

A little surprised how much better IJob is than IJobParallelBatch - while IJob doesn't go as wide as I expected I did see it on more than one worker thread.

## notes

* wishlist
  + would like to be able to save out profiler info to text/json
  + would like max time per job across all runs in a frame
* I couldn't get the subscene based conversion workflow working reliably.
* Find it a little odd that safety handles are by type instead of instance, requiring me to use NativeDisableContainerSafetyRestriction on currentVertexState, oldVertexState.
