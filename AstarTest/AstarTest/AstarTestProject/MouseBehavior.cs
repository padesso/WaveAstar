using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.TiledMap;

namespace AstarTestProject
{
    public class MouseBehavior : SceneBehavior
    {
        // input variables
        private Input _input;

        // Mouse position
        private Vector2 mousePosition = Vector2.Zero;

        public MouseBehavior()
            : base("MouseBehaviour")
        {
        }

        protected override void Update(TimeSpan gameTime)
        {
            _input = WaveServices.Input;

            if (_input.MouseState.LeftButton == WaveEngine.Common.Input.ButtonState.Pressed)
            {
                //Get the position of the click
                Vector3 screenPosition = new Vector3( _input.MouseState.X, _input.MouseState.Y, 0);
                int x = _input.MouseState.X;
                int y = _input.MouseState.Y;

                //get the map we are clicking on
                Entity map = Scene.EntityManager.Find("map");
                TiledMap tiledMap = map.FindComponent<TiledMap>();

                //get a camera instance to unproject the click
                Entity camera = Scene.EntityManager.Find("Camera2D");
                Camera2D camera2D = camera.FindComponent<Camera2D>();

                //Unproject the screen position of the click into a world position
                Vector2 worldPosition = camera2D.Unproject(ref screenPosition).ToVector2();

                //get the tile from the world position from the specified layer
                LayerTile tile = tiledMap.TileLayers["Pathfinding"].GetLayerTileByWorldPosition(worldPosition);
                
                Console.WriteLine("Click");
            }
        }

        protected override void ResolveDependencies()
        {
            
        }
    }
}
