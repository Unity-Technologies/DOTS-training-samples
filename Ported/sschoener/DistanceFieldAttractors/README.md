# Port Report
This is a port of the original `DistanceFieldAttractors` project to DOTS. It targets
 * `entities 0.1.1-preview`,
 * `hybrid rendeder 0.1.1-preview`.

## Notes on the original performance

### In-Editor Performance
<div style="text-align:center"><img src="report_images/orig_editor_frametime.png" /></div>

The original implementation has a median frame time of `10.28ms` in the editor, of which about `5ms` are spent on the actual particle system:

<div style="text-align:center"><img src="report_images/orig_editor_medianframe.png" /></div>



### In-Player Performance
<div style="text-align:center"><img src="report_images/orig_player_frametime.png" /></div>

The original implementation has a median frame time of `10.28ms` in the player, of which about `3ms` are spent on the actual particle system (most of it is just waiting for `vsync`):

<div style="text-align:center"><img src="report_images/orig_player_medianframe.png" /></div>

Interestingly, the time required for the particle update varies quite heavily with the worst frame taking `31.55ms` to finish, with about `27.5ms` in the particle system.

<div style="text-align:center"><img src="report_images/orig_player_maxframe.png" /></div>