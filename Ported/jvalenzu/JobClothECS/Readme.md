![JobClothSimEcs in action](JobClothSimEcs.gif)


## basics

* Three phases
  * constraint simulation (serially process over almost all the edges)
  * a mesh vertex update (gravity applied, with some simplified collision and response at plane y=0)
  * update of the render mesh
  
The contraint simulation job, ClothBarSimJob, processes serially because the graph between vertices is connected.  It's possible to partition them for separate simulation islands, but given the measured results I'm doubtful that would be an improvement.

Played around with a three different modelings of the constraint and mesh simulation:

* IJobParallelOverVertices goes wide for every mesh instance.
* IJob creates a single job per mesh instance.
* ForEach does all the work together in one lambda.

ForEach was a consistent winner.  The cost of manual iteration on the main thread is enormous, and the increased potential for utilization via more jobs did not provide a win.  I was surprised at how poor IJobParallelBatch was.

When dealing with so many instances, there's an alternative modeling where we go wide on vertices from all mesh instances (i.e. transpose the number of jobs: build a list via manual iteration and ScheduleBatch *one* mesh updating job which processes all the meshes).

## timings

These numbers reflect a snapshot in time on a specific machine (i9-9900K 3.6Ghz 16 cores), they may not accurately reflect the current code.

|instances | Job type          | setup (ms) | sim (ms) |
| :------- | :---------------- | ---------: | -------: |
|        1 | IJob              |      ~0.37 |    ~0.17 |
|        1 | IJobParallelBatch |      ~0.47 |    ~0.51 |
|        1 | ForEach           |      ~0.11 |    ~0.06 |
|     4369 | (original)        |     ~47.23 |   ~48.71 |
|     4369 | IJob              |     ~16.47 |    ~6.45 |
|     4369 | IJobParallelBatch |     ~26.93 |   ~15.28 |
|     4369 | ForEach           |      ~0.07 |    ~6.84 |

A little surprised how much better IJob is than IJobParallelBatch - while IJob doesn't go as wide as I expected I did see it on more than one worker thread.

## notes

* Apologies for the copy pasta, I wouldn't usually structure a project like this.
* I'd considered and dismissed using a shared component to filter objects instead of separate ClothInstance types.
* During manual iteration, I'd initially preallocated a JobHandle array and used entityInQueryIndex to fill it in, combining at the very end to produce a final JobHandle.  Unfortunately this required duplicating the EntityQuery logic - using an explicit EntityQuery to get the entityCount (to preallocate handles).  This got out of sync at some point with the query that was actually iterating the entities and ended up costing an inordinate amount.  Would be nice to be able to setup a Unity.Entities.CodeGeneratedJobForEach.ForEachLambdaJobDescription, get entity count, then Run.
* When profiling I ended up having the eyeball/manually copy data a lot.  It would be great to either save out to csv/json or at least copy/paste rows.  max time per job across all runs in a frame would be great too.
* I couldn't get the subscene based conversion workflow working reliably.
* Found it unintuitive that safety handles are by type instead of instance.
