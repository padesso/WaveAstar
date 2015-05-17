using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.TiledMap;

namespace AstarTestProject.Pathfinding
{
    public class Node
    {
        #region Enums

        /// <summary>
        /// Used to calculate G based on location of node relative to current
        /// </summary>
        public enum GCost
        {
            Orthogonal = 10,
            Diagonal = 14
        };

        #endregion


        #region Instance Fields

        private readonly int _pathfindingCost;
        private readonly bool _passable = true;  //TODO: is it better to be passable by default or not?

        private int _f;
        private int _g;
        private int _h;

        #endregion

        #region Constructor

        public Node(LayerTile tile)
        {
            try
            {
                X = tile.X;
                Y = tile.Y;

                _pathfindingCost = int.Parse(tile.TilesetTile.Properties["PathFinderCost"]);

                if (_pathfindingCost < 0)
                    _passable = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating node on tile at: " + 
                    X.ToString() + ", " + Y.ToString() +
                    ex.Message.ToString());
            }
        }

        #endregion


        #region Parameters

        /// <summary>
        /// The x position on the tiled map of this node.
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// THe y position on the tiled map of this node.
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// The node from which we navigated here.  Used to calculate 
        /// the G as well as creating the route back from the goal.
        /// </summary>
        public Node Parent { get; set; }

        /// <summary>
        /// F = G + H : Used by the priority queue to find the best node to navigate to
        /// </summary>
        public int F
        {
            get { return _f; }
        }

        /// <summary>
        /// The movement cost to move from the starting point
        /// 10 for horizontal/vertical
        /// 14 for diagonal
        /// add PathFindingCost
        /// </summary>
        public int G
        {
            get { return _g; }
            set
            {                
                _g = value;
                _f = _g + _h;
            }
        }

        /// <summary>
        /// The estimated movement cost to move from that given square on the grid to the final destination
        /// </summary>
        public int H
        {
            get { return _h; }
            set
            {
                _h = value;
                _f = _g + _h;
            }
        }

        /// <summary>
        /// The value specified in Tiled to modify the cost of navigating this tile.
        /// -1 - not passable
        /// 0 - no restriction
        /// > 1 - more and more restrictive
        /// </summary>
        public int PathfindingCost
        {
            get { return _pathfindingCost; }
        }

        /// <summary>
        /// Is the tile passable or is it just a wall?
        /// </summary>
        public bool Passable
        {
            get { return _passable; }
        }

        #endregion   
    }
}
