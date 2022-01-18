using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using System;

namespace GrammaCast
{
    public class Ennemi
    {
        public static string[] ennemiSpritePath = new string[] 
        { "batSprite.sf", "snakeSprite.sf", "slimeSprite.sf", "slimeOrangeSprite.sf", "slimeblueSprite.sf", "slimeredSprite.sf"};
        public SpriteSheet[] ennemiSprite = new SpriteSheet[ennemiSpritePath.Length];
        public MapForet map;
        public Hero perso;
        public Attaque attaqueLetter;
        private int vitesseEnnemi;
        private AnimatedSprite asEnnemi;
        public Timer timerDeplacement;
        public Timer timerApparition;
        int indice = 0;
        
        Random rand = new Random();

        public Ennemi(Vector2 positionEnnemi, int vitesseEnnemi)
        {
            PositionEnnemi = positionEnnemi;
            VitesseEnnemi = vitesseEnnemi;
            Block = false;
            Actif = true;
        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            for(int i = 0; i < ennemiSprite.Length; i ++)
            {
                ennemiSprite[i] = Content.Load<SpriteSheet>(ennemiSpritePath[i], new JsonContentLoader());
            }            
            this.ASEnnemi = new AnimatedSprite(ennemiSprite[rand.Next(ennemiSprite.Length)]);
        }
        public void Update(GameTime gameTime, float windowWidth, float windowHeight)
        {
            float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            string animation;
            if (this.Actif)
            {
                if (!this.Block)
                {
                    if (this.EstProche())
                    {
                        perso.Block = true;
                        this.Block = true;
                        animation = "idle";
                        attaqueLetter.Actif = true;
                    }
                    else
                    {
                        animation = this.Deplacement(gameTime);
                    }
                }
                else
                {
                    animation = "idle";
                    if (attaqueLetter.Final)
                    {
                        this.Actif = false;
                        perso.Block = false;

                    }
                }
                this.ASEnnemi.Play(animation);
            }
            else
            {
                if (timerApparition == null)
                {
                    timerApparition = new Timer(rand.Next(5,20));
                }
                if (timerApparition.AddTick(deltaSeconds) == false)
                {
                    this.ASEnnemi = new AnimatedSprite(ennemiSprite[rand.Next(ennemiSprite.Length)]);
                    this.Actif = true;
                    this.Block = false;
                    timerApparition = null;
                }

            }
            this.ASEnnemi.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch _spriteBatch)
        { 
            if (this.Actif) 
                _spriteBatch.Draw(this.ASEnnemi, this.PositionEnnemi);
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
        public bool Block;
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
                if (!map.IsCollisionEnnemi(tx, ty)) 
                    this.PositionEnnemi.Y -= walkSpeed;

            }
            else if (deplacement.X == -2 && deplacement.Y == 0)
            {
                ushort tx = (ushort)(this.PositionEnnemi.X / map.TileMap.TileWidth - 1);
                ushort ty = (ushort)(this.PositionEnnemi.Y / map.TileMap.TileHeight);
                animation = "walkWest";
                if (!map.IsCollisionEnnemi(tx, ty)) 
                    this.PositionEnnemi.X -= walkSpeed;

            }
            else if (deplacement.X == 0 && deplacement.Y == 2)
            {
                ushort tx = (ushort)(this.PositionEnnemi.X / map.TileMap.TileWidth);
                ushort ty = (ushort)(this.PositionEnnemi.Y / map.TileMap.TileHeight + 1);
                animation = "walkSouth";
                if (!map.IsCollisionEnnemi(tx, ty)) 
                    this.PositionEnnemi.Y += walkSpeed;

            }
            else if (deplacement.X == 2 && deplacement.Y == 0)
            {
                ushort tx = (ushort)(this.PositionEnnemi.X / map.TileMap.TileWidth + 1);
                ushort ty = (ushort)(this.PositionEnnemi.Y / map.TileMap.TileHeight);
                animation = "walkEast";
                if (!map.IsCollisionEnnemi(tx, ty)) 
                    this.PositionEnnemi.X += walkSpeed;
            }
            else animation = "idle";
            return animation;
        }
        private bool EstProche()
        {
            float posX = Math.Abs(this.PositionEnnemi.X - perso.PositionHero.X);
            float posY = Math.Abs(this.PositionEnnemi.Y - perso.PositionHero.Y);
            if (posX <= 50 && posY <= 50)
            {
                if (map.IsCollisionZone(perso)) return true;
                else return false;
            }                
            else return false;
        }
    }
}
