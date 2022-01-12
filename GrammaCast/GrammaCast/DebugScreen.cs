using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;

namespace GrammaCast
{
    public class Map
    {
        private TiledMap tileMap;
        private TiledMapRenderer tileMapRenderer;
        private TiledMapTileLayer[] tileMapLayer;
        private string path;

        public Map(string path) 
        {
            Path = path;
        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content, GraphicsDevice gd)
        {
            this.TileMap = Content.Load<TiledMap>(this.Path);
            this.TileMapRenderer = new TiledMapRenderer(gd, this.TileMap);
            this.TileMapLayer = new [] { this.TileMap.GetLayer<TiledMapTileLayer>("Zone"),
                this.TileMap.GetLayer<TiledMapTileLayer>("Sol"),
                this.TileMap.GetLayer<TiledMapTileLayer>("Obstacles")};


        }
        public void Update(GameTime gameTime)
        {
            this.TileMapRenderer.Update(gameTime);
        }
        public void Draw()
        {
            this.TileMapRenderer.Draw();
        }

        public string Path
        {
            get => path;
            private set => path = value;
        }
        public TiledMap TileMap
        {
            get => tileMap;
            private set => tileMap = value;
        }
        public TiledMapRenderer TileMapRenderer
        {
            get => tileMapRenderer;
            private set => tileMapRenderer = value;
        }
        public TiledMapTileLayer[] TileMapLayer
        {
            get => tileMapLayer;
            private set => tileMapLayer = value;
        }
    }
}
