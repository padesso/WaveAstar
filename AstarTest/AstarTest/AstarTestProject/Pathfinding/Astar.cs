using SD.Tools.Algorithmia.PriorityQueues;
using System;
using System.Collections.Generic;
using WaveEngine.TiledMap;

namespace AstarTestProject.Pathfinding
{
    public class Astar
    {
        #region Instance Fields

        private readonly TiledMap _map;
        private readonly string _pathfindingLayer;

        private Node[,] _collisionNodes;
        private SimplePriorityQueue<Node> _openSet;
        private List<Node> _closedSet;
        private Path _path;

        private Node _startNode;
        private Node _goalNode;

        #endregion

        #region Constructor

        /// <summary>
        /// Create an A* instance
        /// </summary>
        /// <param name="map"></param>
        /// <param name="pathfindingLayer"></param>
        public Astar(TiledMap map, string pathfindingLayer)
        {
            _map = map;
            _pathfindingLayer = pathfindingLayer;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Solve and return a path
        /// </summary>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="goalX"></param>
        /// <param name="goalY"></param>
        /// <returns></returns>
        public Path Solve(int startX, int startY, int goalX, int goalY)
        {
            try
            {
                //Get an instance of the grid to work on 
                _collisionNodes = GetCollisionNodes();
                _openSet = new SimplePriorityQueue<Node>(CompareNodesByF);  
                _closedSet = new List<Node>();
                _path = new Path();
            
                _startNode = _collisionNodes[startX, startY];
                _goalNode = _collisionNodes[goalX, goalY];

                //Add the starting square (or node) to the open list.
                _openSet.Add(_startNode);

                while (!_closedSet.Contains(_goalNode) && _openSet.Count > 0)
                {
                    Node currentNode = _openSet.Remove(); //Get the node with the lowest cost from the priority queue                    

                    List<Node> neighbors = Neighbors(currentNode, true);
                    Node bestNeighbor = neighbors[0];

                    foreach (Node currentNeighborNode in neighbors)
                    {
                        //If it is not passable or if it is on the closed list, ignore it.                      
                        if (!currentNeighborNode.Passable || _closedSet.Contains(currentNeighborNode))
                            continue;

                        //If it isn’t on the open list, add it to the open list. Make the current square the parent of this square. 
                        if (!_openSet.Contains(currentNeighborNode))
                        {
                            currentNeighborNode.Parent = currentNode;
                            currentNeighborNode.H = ManhattanDistance(currentNeighborNode, _goalNode);  //F is calculated when setting G or H
                            currentNeighborNode.G = CalculateG(currentNode, currentNeighborNode);
                            _openSet.Add(currentNeighborNode);
                        }
                        else //already on open set
                        {
                            //check to see if this path to that square is better, using G cost as the measure. 
                            //A lower G cost means that this is a better path. If so, change the parent of the square 
                            //to the current square, and recalculate the G and F scores of the square. 
                            if (currentNeighborNode.G < bestNeighbor.G)
                            {
                                currentNeighborNode.Parent = currentNode;
                                bestNeighbor = currentNeighborNode;
                            }
                        }
                    }

                    _closedSet.Add(currentNode);
                }

                if (_closedSet.Contains(_goalNode))
                {
                    Console.WriteLine("Found a path");
                    Node currentNode = _goalNode;

                    while (currentNode != null)
                    {                        
                        _path.Push(currentNode);
                        currentNode = currentNode.Parent;
                    }
                }

                return _path;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error solving A*: " + ex.Message);
                return null;
            }
        }

        

        #endregion

        #region Private Methods

        /// <summary>
        /// Get an instance of nodes to work on when solving A*
        /// </summary>
        /// <returns>Return a 2 dimensional array of Nodes</returns>
        private Node[,] GetCollisionNodes()
        {            
            TiledMapLayer collisionLayer = _map.TileLayers[_pathfindingLayer];
            Node[,] collisionNodes = new Node[collisionLayer.TiledMap.Width, collisionLayer.TiledMap.Height];

            for (int xTileIndex = 0; xTileIndex < collisionLayer.TiledMap.Width; xTileIndex++)
            {
                for (int yTileIndex = 0; yTileIndex < collisionLayer.TiledMap.Height; yTileIndex++)
                {
                    collisionNodes[xTileIndex, yTileIndex] =
                        new Node(_map.TileLayers[_pathfindingLayer].GetLayerTileByMapCoordinates(xTileIndex, yTileIndex));
                }
            }

            return collisionNodes;
        }

        /// <summary>
        /// Used to sort the priority queue by calculated value from start to goal with heuristic.
        /// A lower value calculated distance results in a 1, which sorts it 
        /// higher on the priority queue.  A 0 means they are equal and a -1 means it was further and ends up
        /// lower inthe priority queue.
        /// </summary>
        /// <param name="compareNodeOne"></param>
        /// <param name="compareNodeTwo"></param>
        /// <returns>Returns an int if node one is less than, equal or greater than two.   
        /// A lower value calculated distance results in a 1, which sorts it 
        /// higher on the priority queue.  A 0 means they are equal and a -1 means it was further and ends up
        /// lower in the priority queue.</returns>
        private int CompareNodesByF(Node compareNodeOne, Node compareNodeTwo)
        {
            if (compareNodeOne.F < compareNodeTwo.F)
            {
                return 1;
            }
            else if (compareNodeOne.F > compareNodeTwo.F)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Calculate the how many turns it takes to navigate a grid
        /// between two nodes. Like a cab driving through Manhattan
        /// </summary>
        /// <param name="startNode"></param>
        /// <param name="goalNode"></param>
        /// <returns></returns>
        private int ManhattanDistance(Node startNode, Node goalNode)
        {
            if (startNode == null) throw new ArgumentNullException("startNode");
            if (goalNode == null) throw new ArgumentNullException("goalNode");

            try
            {
                double xd = startNode.X - goalNode.X;
                double yd = startNode.Y - goalNode.Y;

                return 10 * ((int)(Math.Abs(xd) + Math.Abs(yd)));  //Factor of 10 due to easy math for 10/14 ortho/diag G
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error calculating Manhattan distance: " + ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// Measure the straight line distance between two positions on a grid
        /// </summary>
        /// <param name="startNode"></param>
        /// <param name="goalNode"></param>
        /// <returns></returns>
        private static int ManhattanShortcut(Node startNode, Node goalNode)
        {
            if (startNode == null) throw new ArgumentNullException("startNode");
            if (goalNode == null) throw new ArgumentNullException("goalNode");

            try
            {
                int xDistance = Math.Abs(startNode.X - goalNode.X);
                int yDistance = Math.Abs(startNode.Y - startNode.Y);
                if (xDistance > yDistance)
                    return 14*yDistance + 10*(xDistance - yDistance);

                return 14 * xDistance + 10 * (yDistance - xDistance);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error calculating Eucliedean distance: " + ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// Find all of the neighbors of a given node and calculate the cost to navigate to the node.
        /// </summary>
        /// <param name="node">The node to find the neighbors of</param>
        /// <param name="allowDiagonals">Include square diagonal to the node?</param>
        /// <returns></returns>
        private List<Node> Neighbors(Node node, bool allowDiagonals)
        {
            try
            {
                List<Node> nodeNeighbors = new List<Node>();

                //Check west neighbor
                if (node.X > 0)
                {
                    Node westNode = _collisionNodes[node.X - 1, node.Y];
                    nodeNeighbors.Add(westNode);

                    if (node.Y > 0 && allowDiagonals)
                    {
                        //Check north west neighbor
                        Node northWestNode = _collisionNodes[node.X - 1, node.Y - 1];
                        nodeNeighbors.Add(northWestNode);
                    }

                    if (node.Y < _map.TileLayers[_pathfindingLayer].TiledMap.Height - 1 && allowDiagonals) 
                    {
                        //Check south west neighbor
                        Node southWestNode = _collisionNodes[node.X - 1, node.Y + 1];
                        nodeNeighbors.Add(southWestNode);
                    }
                }
                
                if (node.X < _map.TileLayers[_pathfindingLayer].TiledMap.Width - 1)
                {
                    //Check east neighbor
                    Node eastNode = _collisionNodes[node.X + 1, node.Y];
                    nodeNeighbors.Add(eastNode);

                    if (node.Y > 0)
                    {
                        //Check north neighbor
                        Node northNode = _collisionNodes[node.X, node.Y - 1];
                        nodeNeighbors.Add(northNode);

                        
                        if (allowDiagonals)
                        {
                            //Check north east neighbor
                            Node northEastNode = _collisionNodes[node.X + 1, node.Y - 1];
                            nodeNeighbors.Add(northEastNode);
                        }
                    }

                    if (node.Y < _map.TileLayers[_pathfindingLayer].TiledMap.Height - 1)
                    {
                        if (allowDiagonals)
                        {
                            //Check south east neighbor
                            Node southEastNode = _collisionNodes[node.X + 1, node.Y + 1];
                            nodeNeighbors.Add(southEastNode);
                        }

                        //Check south neighbor
                        Node southNode = _collisionNodes[node.X, node.Y + 1];
                        nodeNeighbors.Add(southNode);
                    }
                }
                return nodeNeighbors;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error finding neighbors: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Calculate the G cost between two nodes. 10 for ortho and 14 for diagonal
        /// </summary>
        /// <param name="currentNode"></param>
        /// <param name="currentNeighborNode"></param>
        /// <returns>Returns an int G value for node being evaluated (currentNeighborNode)</returns>
        private int CalculateG(Node currentNode, Node currentNeighborNode)
        {
            int gMod = (int)Node.GCost.Diagonal;
            if (currentNode.X != currentNeighborNode.X && currentNode.Y != currentNeighborNode.Y)
                return currentNode.G + gMod;

            gMod = (int)Node.GCost.Orthogonal;

            return currentNode.G + gMod;
        }

        #endregion
    }
}
