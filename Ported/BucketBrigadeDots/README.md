# Optimizations discussed

## Fix cross-system dependencies and ordering
- Correctness / latency (update data before rendering data)
- Avoid true sharing (don't make read/write jobs block on each other)
- (Ideally) Avoid false sharing (two jobs both touch transforms, but for non-overlapping entities)

## Jobify big compute jobs
- Fire presentation system (linear memory access): "easy"
- Fire spreading logic: more difficult (non-linear read/write access)
  - Needed double buffering
  - Built-in memcpy vs DIY memcpy for copying NativeArray to DynamicBuffer: 20x speed up (2ms -> 0.1s)

## Rendering time >> simulation time
- ExecuteGpuUploads is ~half of frame time
  - Possible cause: we're dirtying ~1M transforms (fire wobble)
  - Possible solution: stop dirtying most transforms (gameplay change)

## Fix false cross-job sharing / blocking
- Different systems each update a different object type's transform (e.g. FireUpdateSystem, WorkerUpdateSystem)
- Problem: job system sees two systems updating transforms and deduces a cross-system dependency
- Solutions:
  - Turn off safety checks / cross-system analysis
  - Have systems write to temporary buffers, then join results into a single update-all-transforms job