using System.Collections.Generic;
using System.Linq;

namespace AstarTestProject.Pathfinding
{
    public class Path
    {
        #region Instance Fields

        private Stack<Node> _route;

        #endregion

        #region Constructor

        public Path()
        {
            _route = new Stack<Node>();
        }

        #endregion

        #region Public Methods 

        /// <summary>
        /// Get how many elements the path contains.
        /// </summary>
        public int Count()
        {
           return _route.Count;
        }

        /// <summary>
        /// Add a node to the end of the path.
        /// </summary>
        /// <param name="node"></param>
        public void Push(Node node)
        {
            _route.Push(node);           
        }

        /// <summary>
        /// Reverse the order of the path
        /// </summary>     
        public void Reverse()
        {
            _route.Reverse();
        }

        /// <summary>
        /// Remove a node from the end of the path.
        /// </summary>
        /// <returns></returns>
        public Node Pop()
        {
            return _route.Pop();
        }

        #endregion
    }
}
