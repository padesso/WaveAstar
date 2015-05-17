# WaveAstar
Generic A* implementation for the Wave Engine with Tiled integration.  This implementation is very heavily based on this: http://www.policyalmanac.org/games/aStarTutorial.htm

Dependencies:
-WaveEngine 1.4 with reference to TiledMap source or binary
-Algorithmia library (add via NuGet)
-Tiled map built in 0.11.0

In Tiled, create a tile layer name "Pathfinding" (or whatever you choose, just make sure that you pass the name when instantiating the Astar class).  Populate this layer with tiles that contain a custom property named "PathFinderCost".  Tiles without this custom property will be assumed as passable with a cost of 0.  A PathFinderCost of -1 makes the tile not-passable (a wall) and a higher value makes it less desirable in the path finding system.

Currently, the system solves best path very quickly and perfectly for non-diagonal movement on an orthogonal map. The diagonal movement finds a path but it isn't always the best path - I'm working on addressing this. I've not done any work on implementation for isometric, staggered or hexagonal Tiled maps.

Pull requests are always welcome - I hope somebody finds it useful.
