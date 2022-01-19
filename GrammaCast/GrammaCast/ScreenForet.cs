using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;

namespace GrammaCast
{
    public class MapForet
    {
        
        private TiledMap tileMap;
        private TiledMapRenderer tileMapRenderer;
        private TiledMapTileLayer tileMapLayerZone;
        private TiledMapTileLayer tileMapLayerTransition;
        private TiledMapTileLayer tileMapLayerObstacles;
        private TiledMapTileLayer tileMapLayerObstacles2;

        private string path;

        public MapForet(string path)
        {
            Path = path;
            Actif = false;
        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content, GraphicsDevice gd)
        {
            this.TileMap = Content.Load<TiledMap>(this.Path);
            this.TileMapRenderer = new TiledMapRenderer(gd, this.TileMap);

            //les différents calques d'obstacles ou utiles pour d'autres méthodes
            this.TileMapLayerZone = this.TileMap.GetLayer<TiledMapTileLayer>("zone");
            this.TileMapLayerTransition = this.TileMap.GetLayer<TiledMapTileLayer>("transition");
            this.TileMapLayerObstacles = this.TileMap.GetLayer<TiledMapTileLayer>("obstacles");
            this.TileMapLayerObstacles2 = this.TileMap.GetLayer<TiledMapTileLayer>("obstacles2");

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
        public TiledMapTileLayer TileMapLayerZone
        {
            get => tileMapLayerZone;
            private set => tileMapLayerZone = value;
        }
        public TiledMapTileLayer TileMapLayerTransition
        {
            get => tileMapLayerTransition;
            private set => tileMapLayerTransition = value;
        }
        public TiledMapTileLayer TileMapLayerObstacles
        {
            get => tileMapLayerObstacles;
            private set => tileMapLayerObstacles = value;
        }
        public TiledMapTileLayer TileMapLayerObstacles2
        {
            get => tileMapLayerObstacles2;
            private set => tileMapLayerObstacles2 = value;
        }
        public bool Actif;
        public bool IsCollisionZone(Hero perso) //si le perso est dans la zone, il pourra être bloqué pour enclencher un combat entre un ennemi et lui
        {
            TiledMapTile? tile;
            if (this.TileMapLayerZone.TryGetTile((ushort)perso.PositionHero.X, (ushort)perso.PositionHero.Y, out tile) == false)
                return true;
            if (!tile.Value.IsBlank)
                return true;
            return false;
        }
        public bool IsCollisionEnnemi(ushort x, ushort y) //permet de faire en sorte que l'ennemi se déplace dans une zone sans la quitter 
                                                          //permet aussi de lancer un combat si le perso est dans cette zone
        {
            TiledMapTile? tile;
            if (this.TileMapLayerZone.TryGetTile(x, y, out tile) == false)
                return true;
            if (tile.Value.IsBlank)
                return true;
            return false;
        }
        public bool IsCollisionHero(ushort x, ushort y) //check les collisions avec les obstacles
        {
            TiledMapTile? tile;
            if (this.TileMapLayerObstacles.TryGetTile(x, y, out tile) == false)
                return true;
            if (!tile.Value.IsBlank)
                return true;
            if (this.TileMapLayerObstacles2.TryGetTile(x, y, out tile) == false)
                return true;
            if (!tile.Value.IsBlank)
                return true;
            return false;
        }
        public bool IsTransition(ushort x, ushort y) //permet de vérifier si le joueur peut faire une transition d'une map à l'autre
        {
            TiledMapTile? tile;
            if (this.TileMapLayerTransition.TryGetTile(x, y, out tile) == false)
                return true;
            if (!tile.Value.IsBlank)
                return true;
            return false;
        }
    }
}
