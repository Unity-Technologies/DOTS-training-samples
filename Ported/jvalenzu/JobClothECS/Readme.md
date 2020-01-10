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

|instances | Job type          | # threads | setup (ms) | sim (ms) |
| :------- | :---------------- | --------: | ---------: | -------: |
|        1 | IJob              |        15 |      ~0.37 |    ~0.17 |
|        1 | IJobParallelBatch |        15 |      ~0.47 |    ~0.51 |
|        1 | ForEach           |        15 |      ~0.11 |    ~0.06 |
|     4369 | IJob              |        15 |     ~16.47 |    ~6.45 |
|     4369 | IJobParallelBatch |        15 |     ~26.93 |   ~15.28 |
|     4369 | ForEach           |        15 |      ~0.07 |    ~6.84 |

A little surprised how much better IJob is than IJobParallelBatch - while IJob doesn't go as wide as I expected I did see it on more than one worker thread.

## notes

* I would not usually structure a project with so much copy and paste.
* I'd considered and dismissed using a shared component to filter objects instead of separate ClothInstance types.
* I have to duplicate the EntityQuery logic, once explicit to get the entityCount (to allocate handles) and once implicit to get query
  * would be nice to be able to setup a Unity.Entities.CodeGeneratedJobForEach.ForEachLambdaJobDescription, get entity count, then Run

* wishlist
  + would like to be able to save out profiler info to text/json
  + would like max time per job across all runs in a frame
* I couldn't get the subscene based conversion workflow working reliably.
* Find it a little odd that safety handles are by type instead of instance, requiring me to use NativeDisableContainerSafetyRestriction on currentVertexState, oldVertexState.
