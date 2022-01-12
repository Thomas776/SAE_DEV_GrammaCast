using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Tiled;
using System;

namespace GrammaCast
{
    class Ennemi
    {
        public Map map;
        private int vitesseEnnemi;
        private AnimatedSprite asEnnemi;
        private string path;
        public Timer timerDeplacement;
        int indice = 0;

        public Ennemi(string path, Vector2 positionEnnemi, int vitesseEnnemi)
        {
            Path = path;
            PositionEnnemi = positionEnnemi;
            VitesseEnnemi = vitesseEnnemi;
        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
        {

            SpriteSheet spriteSheet = Content.Load<SpriteSheet>(this.Path, new JsonContentLoader());
            this.ASEnnemi = new AnimatedSprite(spriteSheet);
        }

        public void Update(GameTime gameTime, float windowWidth, float windowHeight)
        {
            Random rand = new Random();
            string animation;
            float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float walkSpeed = deltaSeconds * this.VitesseEnnemi;

            int timeMax = rand.Next(2, 6);
            Vector2 deplacement = new Vector2(0,0);

            if (timerDeplacement == null || timerDeplacement.AddTick(deltaSeconds) == false)
            {
                indice = rand.Next(1, 5);
                timerDeplacement = new Timer(timeMax);
                Console.WriteLine($"test {indice}");
            }
            switch (indice)
            {
                case 1:
                    deplacement.Y = 2;
                    break;
                case 2:
                    deplacement.X = 2;
                    break;
                case 3:
                    deplacement.Y = -2;
                    break;
                case 4:
                    deplacement.X = -2;
                    break;
                default:
                    break;
            }

            if (deplacement.X == 0 && deplacement.Y == -2)
            {
                
                ushort tx = (ushort)(this.PositionEnnemi.X / map.TileMap.TileWidth);
                ushort ty = (ushort)(this.PositionEnnemi.Y / map.TileMap.TileHeight - 1);
                animation = "walkNorth";
                if (!IsCollision(tx, ty))
                    this.PositionEnnemi.Y -= walkSpeed;

            }
            else if (deplacement.X == -2 && deplacement.Y == 0)
            {
                ushort tx = (ushort)(this.PositionEnnemi.X / map.TileMap.TileWidth - 1);
                ushort ty = (ushort)(this.PositionEnnemi.Y / map.TileMap.TileHeight);
                animation = "walkWest";
                if (!IsCollision(tx, ty))
                    this.PositionEnnemi.X -= walkSpeed;

            }
            else if (deplacement.X == 0 && deplacement.Y == 2)
            {
                ushort tx = (ushort)(this.PositionEnnemi.X / map.TileMap.TileWidth);
                ushort ty = (ushort)(this.PositionEnnemi.Y / map.TileMap.TileHeight + 1);
                animation = "walkSouth";
                if (!IsCollision(tx, ty))
                    this.PositionEnnemi.Y += walkSpeed;

            }
            else if (deplacement.X == 2 && deplacement.Y == 0)
            {
                ushort tx = (ushort)(this.PositionEnnemi.X / map.TileMap.TileWidth + 1);
                ushort ty = (ushort)(this.PositionEnnemi.Y / map.TileMap.TileHeight);
                animation = "walkEast";
                if (!IsCollision(tx, ty))
                    this.PositionEnnemi.X += walkSpeed;

            }
            else animation = "idle";


            this.ASEnnemi.Play(animation);
            this.ASEnnemi.Update(gameTime);
        }
        public void Draw(GameTime gameTime, SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(this.ASEnnemi, this.PositionEnnemi);
        }

        public string Path
        {
            get => path;
            private set => path = value;
        }
        public AnimatedSprite ASEnnemi
        {
            get => asEnnemi;
            private set => asEnnemi = value;
        }
        public Vector2 PositionEnnemi;

        public int VitesseEnnemi
        {
            get => vitesseEnnemi;
            private set => vitesseEnnemi = value;
        }
        private bool IsCollision(ushort x, ushort y)
        {
            // définition de tile qui peut être null (?)
            TiledMapTile? tile;
            if (map.TileMapLayer[0].TryGetTile(x, y, out tile) == false)
                return false;
            if (tile.Value.IsBlank)
                return true;
            return false;
        }
    }
}
