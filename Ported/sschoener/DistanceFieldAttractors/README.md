# Port Report
This is a port of the original `DistanceFieldAttractors` project to DOTS. It targets
 * `entities 0.1.1-preview`,
 * `hybrid rendeder 0.1.1-preview`.


## The original project
The project simulates a bunch of particles that are attracted or repelled from the surface of a 3D model. They take on different colors depending on their position relative to the surface.

The logic in the original setup is rather straight forward:
 1. On game start, the `ParticleManager` spawns a bunch of particles. They are referred to as `Orbiters`. For rendering purposes, the particles are split into batches. There is a global choice for the current model (`DistanceField.instance`) that just takes its default value.
 2. At a fixed update rate, the `ParticleManager` updates all particle velocities and positions. The color of a particle is then determined by how far it is from the surface and whether it is inside the model or not.
 3. Every frame all meshes are drawn with a call to `Graphics.DrawMeshInstanced`. The colors are setup using `MaterialPropertyBlock`s, which might be the most interesting problem to solve to port it over to DOTS. Every once in a while, the `Update` within `DistanceField` will change to the next model.

### Notes on the original performance

#### In-Editor Performance
<div style="text-align:center"><img src="report_images/orig_editor_frametime.png" /></div>

The original implementation has a median frame time of `10.28ms` in the editor, of which about `5ms` are spent on the actual particle system:

<div style="text-align:center"><img src="report_images/orig_editor_medianframe.png" /></div>


#### In-Player Performance
<div style="text-align:center"><img src="report_images/orig_player_frametime.png" /></div>

The original implementation has a median frame time of `10.28ms` in the player, of which about `3ms` are spent on the actual particle system (most of it is just waiting for `vsync`):

<div style="text-align:center"><img src="report_images/orig_player_medianframe.png" /></div>

Interestingly, the time required for the particle update varies quite heavily with the worst frame taking `31.55ms` to finish, with about `27.5ms` in the particle system.

<div style="text-align:center"><img src="report_images/orig_player_maxframe.png" /></div>

## Porting it over
The central decisions I'm taking to port this over are:
 * Remove the dependency on the fixed update. The goal is to run it at a smooth 60 FPS for simulation and rendering.
 * Keep rendering using `Graphics.DrawMeshInstanced` by copying data from the ECS to an array in every frame. Using the hybrid renderer is out of the question since as of writing it does not properly support per instance data. Alternatively, it might be interesting to explore whether the color could be computed in a shader, but that might mean duplicating the somewhat brittle code for calculating the distance from the model.
 * Depend as little as possible on pre-written systems. That excludes the usage of the `Transform` systems from the entities package.
 * Keep absolutely no persistent state in any system. This means that the single model in the scene will have to be stored on some entity.
 * Make sure that there are conversion systems in place that still allow to change colors etc. from within the editor.


### Ideas
 * `UpdateLocalToWorld`: Optimize `LookRotation` since second argument is constant. Does Burst already figure that out?
 * `UpdateParticlesSpinMixer`: Use `sincos`