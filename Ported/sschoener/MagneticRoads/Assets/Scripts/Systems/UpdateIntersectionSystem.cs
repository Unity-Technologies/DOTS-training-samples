using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Systems {
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(UpdateTransformSystem))]
    public class UpdateIntersectionSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            inputDeps.Complete();
            int frame = 1 + UnityEngine.Time.frameCount;
            var splineBlob = TrackSplinesBlob.Instance;
            var intersectionBlob = IntersectionsBlob.Instance;
            Entities.ForEach((Entity entity, ref LocalIntersectionComponent localIntersection, ref OnSplineComponent onSpline, ref CarSpeedComponent speed, in CoordinateSystemComponent coords) =>
            {
                if (onSpline.InIntersection)
                {
                    if (speed.SplineTimer < 1)
                        return;
                    // we're exiting an intersection - make sure the next road
                    // segment has room for us before we proceed
                    if (TrackSplines.GetQueue(onSpline.Spline, onSpline.Direction, onSpline.Side).Count <= splineBlob.Value.Splines[onSpline.Spline].MaxCarCount)
                    {
                        Intersections.Occupied[localIntersection.Intersection][(localIntersection.Side + 1) / 2] = false;
                        onSpline.InIntersection = false;
                        onSpline.Dirty = true;
                        speed.SplineTimer = 0f;
                    }
                    else
                    {
                        speed.SplineTimer = 1f;
                        speed.NormalizedSpeed = 0f;
                    }
                }
                else
                {
                    if (speed.SplineTimer < 1)
                    {
                        var queue = TrackSplines.GetQueue(onSpline.Spline, onSpline.Direction, onSpline.Side);
                        for (int i = 0; i < queue.Count; i++)
                        {
                            if (queue[i].Entity == entity)
                            {
                                var queueEntry = queue[i];
                                queueEntry.SplineTimer = speed.SplineTimer;
                                queue[i] = queueEntry;
                                break;
                            }
                        }
                        return;
                    }
                    
                    // we're exiting a road segment - first, we need to know
                    // which intersection we're entering
                    ushort intersection;
                    {
                        ref var blobSpline = ref splineBlob.Value.Splines[onSpline.Spline];
                        if (onSpline.Direction == 1)
                        {
                            intersection = blobSpline.EndIntersection;
                            localIntersection.Bezier.start = blobSpline.Bezier.end;
                        }
                        else
                        {
                            intersection = blobSpline.StartIntersection;
                            localIntersection.Bezier.start = blobSpline.Bezier.start;
                        }
                    }

                    // now we need to know which road segment we'll move into
                    // (dead-ends force u-turns, but otherwise, u-turns are not allowed)
                    int newSplineIndex = 0;
                    ref var intersectionNeighbors = ref intersectionBlob.Value.Intersections[intersection].Neighbors; 
                    if (intersectionNeighbors.Count > 1)
                    {
                        int mySplineIndex = intersectionNeighbors.IndexOfSpline(onSpline.Spline);
                        newSplineIndex = new Random((uint)((entity.Index + 1) * frame * 47701)).NextInt(intersectionNeighbors.Count - 1);
                        if (newSplineIndex >= mySplineIndex)
                        {
                            newSplineIndex++;
                        }
                    }

                    
                    ushort newSpline;
                    unsafe
                    {
                        newSpline = intersectionNeighbors.Splines[newSplineIndex];
                    }

                    // make sure that our side of the intersection (top/bottom)
                    // is empty before we enter
                    if (Intersections.Occupied[intersection][(localIntersection.Side + 1) / 2])
                    {
                        speed.SplineTimer = 1f;
                        speed.NormalizedSpeed = 0f;
                    }
                    else
                    {
                        var previousLane = TrackSplines.GetQueue(onSpline.Spline, onSpline.Direction, onSpline.Side);

                        // to avoid flipping between top/bottom of our roads,
                        // we need to know our new spline's normal at our entrance point
                        float3 newNormal;
                        ref var newBlobSpline = ref splineBlob.Value.Splines[newSpline];
                        if (newBlobSpline.StartIntersection == intersection)
                        {
                            onSpline.Direction = 1;
                            newNormal = newBlobSpline.Geometry.startNormal;
                            localIntersection.Bezier.end = newBlobSpline.Bezier.start;
                        }
                        else
                        {
                            onSpline.Direction = -1;
                            newNormal = newBlobSpline.Geometry.endNormal;
                            localIntersection.Bezier.end = newBlobSpline.Bezier.end;
                        }

                        // now we'll prepare our intersection spline - this lets us
                        // create a "temporary lane" inside the current intersection
                        {
                            ref var intersectionData = ref intersectionBlob.Value.Intersections[intersection];
                            var pos = intersectionData.Position;
                            var norm = intersectionData.Normal;
                            localIntersection.Bezier.anchor1 = (pos + localIntersection.Bezier.start) * .5f;
                            localIntersection.Bezier.anchor2 = (pos + localIntersection.Bezier.end) * .5f;
                            localIntersection.Geometry.startTangent = math.round(math.normalize(pos - localIntersection.Bezier.start));
                            localIntersection.Geometry.endTangent = math.round(math.normalize(pos - localIntersection.Bezier.end));
                            localIntersection.Geometry.startNormal = norm;
                            localIntersection.Geometry.endNormal = norm;
                        }

                        if (onSpline.Spline == newSpline)
                        {
                            // u-turn - make our intersection spline more rounded than usual
                            float3 perp = math.cross(localIntersection.Geometry.startTangent, localIntersection.Geometry.startNormal);
                            localIntersection.Bezier.anchor1 += .5f * RoadGeneratorDots.intersectionSize * localIntersection.Geometry.startTangent;
                            localIntersection.Bezier.anchor2 += .5f * RoadGeneratorDots.intersectionSize * localIntersection.Geometry.startTangent;
                            localIntersection.Bezier.anchor1 -= localIntersection.Side * RoadGeneratorDots.trackRadius * .5f * perp;
                            localIntersection.Bezier.anchor2 += localIntersection.Side * RoadGeneratorDots.trackRadius * .5f * perp;
                        }

                        localIntersection.Intersection = intersection;
                        localIntersection.Length = localIntersection.Bezier.MeasureLength(RoadGeneratorDots.splineResolution);

                        onSpline.InIntersection = true;
                        onSpline.Dirty = true;

                        // to maintain our current orientation, should we be
                        // on top of or underneath our next road segment?
                        // (each road segment has its own "up" direction, at each end)
                        onSpline.Side = (sbyte) (math.dot(newNormal, coords.Up) > 0f ? 1 : -1);

                        // should we be on top of or underneath the intersection?
                        localIntersection.Side = (sbyte) (math.dot(localIntersection.Geometry.startNormal, coords.Up) > 0f ? 1 : -1);

                        // block other cars from entering this intersection
                        Intersections.Occupied[intersection][(localIntersection.Side + 1) / 2] = true;

                        // remove ourselves from our previous lane's list of cars
                        {
                            for (int i = 0; i < previousLane.Count; i++)
                            {
                                if (previousLane[i].Entity == entity)
                                {
                                    previousLane.RemoveAt(i);
                                    break;
                                }
                            }
                        }

                        // add "leftover" spline timer value to our new spline timer
                        // (avoids a stutter when changing between splines)
                        speed.SplineTimer = (speed.SplineTimer - 1f) * splineBlob.Value.Splines[onSpline.Spline].MeasuredLength / localIntersection.Length;
                        onSpline.Spline = newSpline;

                        var queue = TrackSplines.GetQueue(onSpline.Spline, onSpline.Direction, onSpline.Side);
                        queue.Add(new QueueEntry
                        {
                            Entity = entity,
                            SplineTimer = speed.SplineTimer
                        });
                    }
                }
                
            }).WithoutBurst().WithName("UpdateIntersection").Run();
            return default;
        }
    }
}
