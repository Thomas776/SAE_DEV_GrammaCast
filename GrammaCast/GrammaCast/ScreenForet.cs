using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;

namespace GrammaCast
{
    public class MapForet
    {
        private TiledMap tileMap;
        private TiledMapRenderer tileMapRenderer;
        private TiledMapTileLayer[] tileMapLayer;
        private string path;

        public MapForet(string path)
        {
            Path = path;
        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content, GraphicsDevice gd)
        {
            this.TileMap = Content.Load<TiledMap>(this.Path);
            this.TileMapRenderer = new TiledMapRenderer(gd, this.TileMap);
            this.TileMapLayer = new[] { this.TileMap.GetLayer<TiledMapTileLayer>("zone"),
                this.TileMap.GetLayer<TiledMapTileLayer>("sol"),
                this.TileMap.GetLayer<TiledMapTileLayer>("chemin"),
                this.TileMap.GetLayer<TiledMapTileLayer>("deco"),
                this.TileMap.GetLayer<TiledMapTileLayer>("obstacles"),
                this.TileMap.GetLayer<TiledMapTileLayer>("hauteur")};


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
