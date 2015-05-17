# WaveAstar
Generic A* implementation for the Wave Engine with Tiled integration

Dependencies:
-WaveEngine 1.4 with reference to TiledMap source or binary
-Algorithmia library (add via NuGet)
-Tiled map built in 0.11.0

Currently, the system solves best path very quickly and perfectly for non-diagonal movement on an orthogonal map.  The diagonal movement finds a path but it isn't always the best path - I'm working on addressing this.  I've not done any work on implementation for isometric, staggered or hexagonal Tiled maps.
