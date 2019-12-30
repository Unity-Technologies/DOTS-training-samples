TODO
[x] read Unity.Jobs
  [x] What is IJob?
      It's the interface structs must derive from to be run through the Job system.  such structs must implement an Execute method and
      can pass data through a NativeArray (blittable data is allowed as members, but is copied).
  [x] What is IJobParallelFor?
      IJobParallelFor is another interface structs can derive from.  This executes a job in separate batches that may be scheduled on different cores, i.e. they run in parallel.  There's some flexibility to how big the batches are, an index is passed to the Execute function in this case to inform the execution about how it was invoked.
  
[x] read Unity.Collections information

[x] write description of how program works
  [x] identify read and writes in existing program
  [x] what are pins?
  
Update is driven through the ClothSim component.  Two jobs are created:

* BarJob
* MeshJob

## Preliminary/initialization

* verticles: the vertices for the simulation mesh.  This is a native array which collects the job outputs, is a persistent allocation and initialized from the render mesh.
* pins: pins is a native array and is 0 for "free" vertices and '1' for pinned vertices, which are not allowed to move.  It is immutable after initialization.  It is initialized base on a heuristic on the normal and mesh position.
* bars
barlookup is a hashset storing "pairs."  There are pairs between each vertex in a triangle.
For each unique pair, we allocate a bar entry.  This is a pair of position indices.  So each entry of bar is a pair of vertices in the same triangle.
* barLengths: a parallel array to bars which is the distance between these pairs

* oldVertices: initialized to vertex positions
* editedVertices: a managed array copied from the meshjob, the updated vertex positions

## Update

BarJob (serial) does the actual simulation:
* for each vertex pair
  o we find their current position and their pin value.
  o we calculate their current distance
  o we find the delta between their current position and initial position.
  o we generate a direction vector from the first vertex (p1) to second (p2)
    + if both vertices are pinned, we do nothing
    + if one is pinned and the other is free, we move free to pin to correct the distance to original
    + otherwise we split the distance so that the resulting distance is original
    
This job executes serially because the vertex updates for previous positions affects the vertices for the next position.

UpdateMeshJob (parallel):
* for each vertex
  o get current vertex position (vert) and old vertex position (oldVert)
  o apply gravity to old vertex position
  o push vert away from oldVert along direction vector # this is substitute for velocity
  o calculate world position of vert, worldPos
  o save our current vertex position to oldVert
  o if the worldPos y coordinate is less than 0, then
    + transform old vertex to oldWorldPos
    + average oldWorldPos y coordinate with new worldPos y coordinate
    + 0 out worldPos.y
    + reverse transform vert and oldVert
    
[ ] Followup work: can we make barJob parallel as well?  Maybe we could sort vertices topographically and chunk them in islands.
[ ] read through ECS introductory material

notes:
* would be handle to have batch/chunk size in execute
* So I'm running into problems trying to model the problem.

I have 8000 vertices and I'd like them (MeshJob) processed in parallel.  Here are the options as I see them:

* one component per vertex, iterate with ForEach.  This seems nuts.  Also since multiple instances of the same component is frowned on that would eman 8000 entities for one GO which seems nuts.
* one component for all the vertices, which doesn't imply a lot of parallelism.
* IJobChunk operates in chunks of archtype instances which doesn't solve the problem - I'd have to have multiple entities to run in parallel
* https://docs.unity3d.com/Packages/com.unity.entities@0.0/manual/manual_iteration.html
* It seems like there are a lot of sequence points on the main thread to collect jobs and kick them off again.  Is there a workflow that allows us to kick jobs off from other jobs?

Alternatives:
* naive, one entity and IJobParallelFor over vertices
* naive + Burst compile
* filter update mesh job on pins
* one entity/component/mesh per letter, use ForEach
* one entity/component per vertex
* one entity/shared component/component instances cover range or chunked by output.

[ ] Fill out JobSystem code until it compiles
[ ] How do we convert component to match?
[ ] break out sharable data?

