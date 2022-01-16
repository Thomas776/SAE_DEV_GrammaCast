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
    public class Hero
    {
        public MapVillage[] mapV;
        public MapForet mapF;
        public MapBoss[] mapB;

        private int vitesseHero;
        private AnimatedSprite asHero;
        private string path;
        int indiceAnimation = 0;
        string animationBase = "idleSouth";

        public Hero(string path, Vector2 positionHero, int vitesseHero)
        {
            Path = path;
            PositionHero = positionHero;
            VitesseHero = vitesseHero;
            Block = false;
        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
        {

            SpriteSheet spriteSheet = Content.Load<SpriteSheet>(this.Path, new JsonContentLoader());
            this.ASHero = new AnimatedSprite(spriteSheet);
        }

        public void Update(GameTime gameTime, float windowWidth, float windowHeight)
        {
            string animation = animationBase;
            if (this.Block == false) 
                animation = this.DeplacementHero(gameTime, windowWidth, windowHeight, ref indiceAnimation);
            else
            {                
                switch (indiceAnimation)
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
        public bool Block;
        public string DeplacementHero(GameTime gameTime, float windowWidth, float windowHeight, ref int indiceAnimation)
        {
            float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float walkSpeed = deltaSeconds * this.VitesseHero;
            string animation = animationBase;
            
            KeyboardState keyboardState = Keyboard.GetState();
            //collision fenêtre
            if (this.PositionHero.X >= windowWidth - this.ASHero.TextureRegion.Width / 3)
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
            if (mapV[0].Actif)
            {
                animation = DeplacementV(mapV[0], walkSpeed);
            }
            else if (mapV[1].Actif)
            {
                animation = DeplacementV(mapV[1], walkSpeed);
            }
            else if (mapF.Actif)
            {
                animation = DeplacementF(mapF, walkSpeed);
            }
            else if (mapB[0].Actif)
            {
                animation = DeplacementB(mapB[0], walkSpeed);
            }
            else if (mapB[1].Actif)
            {
                animation = DeplacementB(mapB[1], walkSpeed);
            }

            return animation;
        }
        public string DeplacementV(MapVillage map, float walkSpeed)
        {
            string animation = animationBase;

            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth - 1);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight);
                animation = "walkWest";
                if (!map.IsCollisionHero(tx, ty))
                    this.PositionHero.X -= walkSpeed;
                indiceAnimation = 1;
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth + 1);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight);
                animation = "walkEast";
                if (!map.IsCollisionHero(tx, ty))
                    this.PositionHero.X += walkSpeed;
                indiceAnimation = 2;
            }
            else if (keyboardState.IsKeyDown(Keys.Up))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight);
                animation = "walkNorth";
                if (!map.IsCollisionHero(tx, ty))
                    this.PositionHero.Y -= walkSpeed;

                indiceAnimation = 3;
            }
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight + 1);
                animation = "walkSouth";
                if (!map.IsCollisionHero(tx, ty))
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
            return animation;
        }
        public string DeplacementF(MapForet map, float walkSpeed)
        {
            string animation = animationBase;

            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth - 1);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight);
                animation = "walkWest";
                if (!map.IsCollisionHero(tx, ty))
                    this.PositionHero.X -= walkSpeed;
                indiceAnimation = 1;
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth + 1);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight);
                animation = "walkEast";
                if (!map.IsCollisionHero(tx, ty))
                    this.PositionHero.X += walkSpeed;
                indiceAnimation = 2;
            }
            else if (keyboardState.IsKeyDown(Keys.Up))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight);
                animation = "walkNorth";
                if (!map.IsCollisionHero(tx, ty))
                    this.PositionHero.Y -= walkSpeed;

                indiceAnimation = 3;
            }
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight + 1);
                animation = "walkSouth";
                if (!map.IsCollisionHero(tx, ty))
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
            return animation;
        }
        public string DeplacementB(MapBoss map, float walkSpeed)
        {
            string animation = animationBase;

            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth - 1);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight);
                animation = "walkWest";
                if (!map.IsCollisionHero(tx, ty))
                    this.PositionHero.X -= walkSpeed;
                indiceAnimation = 1;
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth + 1);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight);
                animation = "walkEast";
                if (!map.IsCollisionHero(tx, ty))
                    this.PositionHero.X += walkSpeed;
                indiceAnimation = 2;
            }
            else if (keyboardState.IsKeyDown(Keys.Up))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight);
                animation = "walkNorth";
                if (!map.IsCollisionHero(tx, ty))
                    this.PositionHero.Y -= walkSpeed;

                indiceAnimation = 3;
            }
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight + 1);
                animation = "walkSouth";
                if (!map.IsCollisionHero(tx, ty))
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
            return animation;
        }
        public bool TestTransitionV(MapVillage map)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth - 1);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight);

                if (map.IsTransition(tx, ty))
                    return true;
                return false;
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth + 1);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight);

                if (map.IsTransition(tx, ty))
                    return true;
                return false;
            }
            else if (keyboardState.IsKeyDown(Keys.Up))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight);

                if (map.IsTransition(tx, ty))
                    return true;
                return false;
            }
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight + 1);

                if (map.IsTransition(tx, ty))
                    return true;
                return false;
            }
            else return false;
        }
        public bool TestTransitionF(MapForet map)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth - 1);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight);

                if (map.IsTransition(tx, ty))
                    return true;
                return false;
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth + 1);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight);

                if (map.IsTransition(tx, ty))
                    return true;
                return false;
            }
            else if (keyboardState.IsKeyDown(Keys.Up))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight);

                if (map.IsTransition(tx, ty))
                    return true;
                return false;
            }
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight + 1);

                if (map.IsTransition(tx, ty))
                    return true;
                return false;
            }
            else return false;
        }
        public bool TestTransitionB(MapBoss map)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth - 1);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight);

                if (map.IsTransition(tx, ty))
                    return true;
                return false;
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth + 1);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight);

                if (map.IsTransition(tx, ty))
                    return true;
                return false;
            }
            else if (keyboardState.IsKeyDown(Keys.Up))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight);

                if (map.IsTransition(tx, ty))
                    return true;
                return false;
            }
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight + 1);

                if (map.IsTransition(tx, ty))
                    return true;
                return false;
            }
            else return false;
        }

    }

}
