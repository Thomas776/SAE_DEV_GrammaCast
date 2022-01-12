using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;

namespace GrammaCast
{
    class Map
    {
        private TiledMap tiledMap;
        private TiledMapRenderer tiledMapRenderer;
        private string path;

        public Map(string path) 
        {
            Path = path;
        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content, GraphicsDevice gd)
        {
            this.TiledMap = Content.Load<TiledMap>(this.Path);
            this.TiledMapRenderer = new TiledMapRenderer(gd, this.TiledMap);
        }
        public void Update(GameTime gameTime)
        {
            this.TiledMapRenderer.Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            this.TiledMapRenderer.Draw();
            spriteBatch.End();
        }
        public string Path
        {
            get => path;
            private set => path = value;
        }
        public TiledMap TiledMap
        {
            get => tiledMap;
            private set => tiledMap = value;
        }
        public TiledMapRenderer TiledMapRenderer
        {
            get => tiledMapRenderer;
            private set => tiledMapRenderer = value;
        }
    }
}
