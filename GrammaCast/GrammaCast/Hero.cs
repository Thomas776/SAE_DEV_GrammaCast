using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Tiled;

namespace GrammaCast
{
    public class Hero
    {
        public Map map;
        private int vitesseHero;
        private AnimatedSprite asHero;
        private string path;
        int indiceAnimation = 0;
        string animation = "idleSouth";

        public Hero(string path, Vector2 positionHero, int vitesseHero)
        {
            Path = path;
            PositionHero = positionHero;
            VitesseHero = vitesseHero;
        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
        {

            SpriteSheet spriteSheet = Content.Load<SpriteSheet>(this.Path, new JsonContentLoader());
            this.ASHero = new AnimatedSprite(spriteSheet);
        }

        public void Update(GameTime gameTime, float windowWidth, float windowHeight)
        {
            float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float walkSpeed = deltaSeconds * this.VitesseHero;

            KeyboardState keyboardState = Keyboard.GetState();
            //collision fenêtre
            if (this.PositionHero.X >= windowWidth - this.ASHero.TextureRegion.Width/3)
            {
                this.PositionHero.X -= walkSpeed;
            }
            else if (this.PositionHero.Y >= windowHeight - this.ASHero.TextureRegion.Height / 3)
            {
                this.PositionHero.Y -= walkSpeed;
            }
            else if (this.PositionHero.X <= 0 + this.ASHero.TextureRegion.Width / 3)
            {
                this.PositionHero.X += walkSpeed;
            }
            else if (this.PositionHero.Y <= 0 + this.ASHero.TextureRegion.Height / 3)
            {
                this.PositionHero.Y += walkSpeed;
            }
            //déplacement
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth - 1);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight);
                animation = "walkWest";
                if (!IsCollision(tx, ty))
                    this.PositionHero.X -= walkSpeed;
                indiceAnimation = 1;
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth + 1);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight);
                animation = "walkEast";
                if (!IsCollision(tx, ty))
                    this.PositionHero.X += walkSpeed;
                indiceAnimation = 2;
            }
            else if (keyboardState.IsKeyDown(Keys.Up))
            {
                ushort tx = (ushort)(this.PositionHero.X  / map.TileMap.TileWidth);
                ushort ty = (ushort)(this.PositionHero.Y  / map.TileMap.TileHeight);
                animation = "walkNorth";
                if (!IsCollision(tx, ty))
                    this.PositionHero.Y -= walkSpeed;

                indiceAnimation = 3;
            }
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight + 1);
                animation = "walkSouth";
                if (!IsCollision(tx, ty))
                    this.PositionHero.Y += walkSpeed;
                indiceAnimation = 0;
            }
            else switch (indiceAnimation)
                {
                    case 0:
                        animation = "idleSouth";
                        break;
                    case 1:
                        animation = "idleWest";
                        break;
                    case 2:
                        animation = "idleEast";
                        break;
                    case 3:
                        animation = "idleNorth";
                        break;
                }

            this.ASHero.Play(animation);
            this.ASHero.Update(gameTime);
        }
        public void Draw(GameTime gameTime, SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(this.ASHero, this.PositionHero);
        }

        public string Path
        {
            get => path;
            private set => path = value;
        }
        public AnimatedSprite ASHero
        {
            get => asHero;
            private set => asHero = value;
        }
        public Vector2 PositionHero;

        public int VitesseHero
        {
            get => vitesseHero;
            private set => vitesseHero = value;
        }
        private bool IsCollision(ushort x, ushort y)
        {
            // définition de tile qui peut être null (?)
            TiledMapTile? tile;
            if (map.TileMapLayer[2].TryGetTile(x, y, out tile) == false)
                return false;
            if (!tile.Value.IsBlank)
                return true;
            return false;
        }
    }
    
}
