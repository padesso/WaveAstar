#region Using Statements
using System;
using System.Collections.Generic;
using AstarTestProject.Pathfinding;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Animation;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Resources;
using WaveEngine.Framework.Services;
using WaveEngine.TiledMap;
using WaveEngine.Framework.Physics2D;
#endregion

namespace AstarTestProject
{
    public class MyScene : Scene
    {
        private TiledMap _tiledMap;

        protected override void CreateScene()
        {
            //Insert your scene definition here.
            CreateCamera();
            CreateTiledMap();

            // Adds Mouse control. See MouseBehavior.cs for details.
            //this.AddSceneBehavior(new MouseBehavior(), SceneBehavior.Order.PostUpdate);
            //this.AddSceneBehavior(new PickingBehavior(), SceneBehavior.Order.PostUpdate);
        }

        private void CreateCamera()
        {
            // Create a 2D camera
            var camera = new FixedCamera2D("Camera2D")
            {
                BackgroundColor = new Color("#5a93e0"),
            };

            var camera2DComponent = camera.Entity.FindComponent<Camera2D>();
            camera2DComponent.Zoom = Vector2.One / 1.5f;
            camera2DComponent.Position = new Vector3(400, 200, float.MaxValue);
            //camera2DComponent.Rotation = .125f;

            this.AddSceneBehavior(new MouseBehavior(), SceneBehavior.Order.PostUpdate);

            EntityManager.Add(camera);          
        }

        private void CreateTiledMap()
        {
            try
            {
                var map = new Entity("map")
                .AddComponent(new Transform2D())
                .AddComponent(this._tiledMap = new TiledMap("Content/test.tmx")
                {
                    MinLayerDrawOrder = -10,
                    MaxLayerDrawOrder = -0
                });

                this.EntityManager.Add(map);
                //RenderManager.DebugLines = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        protected override void Start()
        {
            base.Start();
            
            //Load the A* engine and tell it what layer to use for pathfinding
            Astar astar = new Astar(_tiledMap, "Pathfinding");

            //Find your way from 0,0 to 19,19
            Path path = astar.Solve(0,0,19,19);

            //Draw the path
            while (path.Count() > 0)
            {
                //TODO:

                Node tempNode = path.Pop();

                Entity crateEntity = new Entity() { Tag = "path" }
                 .AddComponent(new Transform2D() { LocalPosition = new Vector2((tempNode.X * _tiledMap.TileWidth) + (_tiledMap.TileWidth / 2), (tempNode.Y * _tiledMap.TileHeight) + (_tiledMap.TileHeight / 2)), Origin = Vector2.Center, DrawOrder = -9 })
                 .AddComponent(new Sprite("Content/ball.png") { SourceRectangle = new Rectangle(0, 128, 16, 16) })
                 .AddComponent(new SpriteRenderer(DefaultLayers.Alpha, AddressMode.PointWrap))
                 .AddComponent(new CircleCollider())
                 ;

                this.EntityManager.Add(crateEntity);

                
            }

            Console.WriteLine("Testing Astar");
        }
    }
}
