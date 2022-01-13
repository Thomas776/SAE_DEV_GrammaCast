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
        public MapForet map;
        public Hero perso;
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
            Actif = true;
        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
        {

            SpriteSheet spriteSheet = Content.Load<SpriteSheet>(this.Path, new JsonContentLoader());
            this.ASEnnemi = new AnimatedSprite(spriteSheet);
        }

        public void Update(GameTime gameTime, float windowWidth, float windowHeight)
        {
            if (this.Actif)
            {
                if (this.EstProche())
                {
                    perso.Block = true;

                }

                else
                {
                    string animation = this.Deplacement(gameTime);
                    this.ASEnnemi.Play(animation);
                }

                this.ASEnnemi.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch _spriteBatch)
        { 
            if (this.Actif) _spriteBatch.Draw(this.ASEnnemi, this.PositionEnnemi);
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
        public bool Actif;
        private string Deplacement(GameTime gameTime)
        {
            string animation;
            Random rand = new Random();
            float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float walkSpeed = deltaSeconds * this.VitesseEnnemi;

            int timeMax = rand.Next(2, 6);
            Vector2 deplacement = new Vector2(0, 0);

            if (timerDeplacement == null || timerDeplacement.AddTick(deltaSeconds) == false)
            {
                indice = rand.Next(1, 5);
                timerDeplacement = new Timer(timeMax);
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
                if (!IsCollisionEnnemi(tx, ty)) this.PositionEnnemi.Y -= walkSpeed;

            }
            else if (deplacement.X == -2 && deplacement.Y == 0)
            {
                ushort tx = (ushort)(this.PositionEnnemi.X / map.TileMap.TileWidth - 1);
                ushort ty = (ushort)(this.PositionEnnemi.Y / map.TileMap.TileHeight);
                animation = "walkWest";
                if (!IsCollisionEnnemi(tx, ty)) this.PositionEnnemi.X -= walkSpeed;

            }
            else if (deplacement.X == 0 && deplacement.Y == 2)
            {
                ushort tx = (ushort)(this.PositionEnnemi.X / map.TileMap.TileWidth);
                ushort ty = (ushort)(this.PositionEnnemi.Y / map.TileMap.TileHeight + 1);
                animation = "walkSouth";
                if (!IsCollisionEnnemi(tx, ty)) this.PositionEnnemi.Y += walkSpeed;

            }
            else if (deplacement.X == 2 && deplacement.Y == 0)
            {
                ushort tx = (ushort)(this.PositionEnnemi.X / map.TileMap.TileWidth + 1);
                ushort ty = (ushort)(this.PositionEnnemi.Y / map.TileMap.TileHeight);
                animation = "walkEast";
                if (!IsCollisionEnnemi(tx, ty)) this.PositionEnnemi.X += walkSpeed;
            }
            else animation = "idle";
            return animation;
        }

        public bool IsCollisionEnnemi(ushort x, ushort y)
        {

            TiledMapTile? tile;
            if (map.TileMapLayerZone.TryGetTile(x, y, out tile) == false)
                return true;
            if (tile.Value.IsBlank)
                return true;
            return false;
        }
        private bool EstProche()
        {
            float posX = Math.Abs(this.PositionEnnemi.X - perso.PositionHero.X);
            float posY = Math.Abs(this.PositionEnnemi.Y - perso.PositionHero.Y);
            if (posX <= 50 && posY <= 50)
            {
                if (perso.IsCollisionZone()) return true;
                else return false;
            }                
            else return false;
        }
    }
}
