using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;

namespace GrammaCast
{
    public class Boss
    {
        public MapBoss map;
        public Hero hero;
        private AnimatedSprite asBoss;
        private AnimatedSprite asLaserLaunch;
        private AnimatedSprite asLaser;
        private AnimatedSprite asArm;
        private string path;
        public int maxHP = 10000;
        public float hp = 10000;
        public bool Actif;
        string animation = "idleéveilBoss";
        Timer timerEveil;
        Timer timerAttaque;
        Timer timerRepos;
        Timer timerDeath;
        public bool Dead;
        Texture2D rectHp;
        int rect;
        Random rand = new Random();
        bool Pret;
        int x;
        int y;
        int animationRand;
        int vitesse = 500;
        Vector2 positionBras;
        public bool changementMusique;
        public Boss(string path, Vector2 positionBoss)
        {
            Path = path;
            PositionBoss = positionBoss;
            Actif = false;
            Dead = false;
        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content, GraphicsDevice gd)
        {

            SpriteSheet spriteSheet = Content.Load<SpriteSheet>(this.Path, new JsonContentLoader());
            this.ASBoss = new AnimatedSprite(spriteSheet);
            spriteSheet = Content.Load<SpriteSheet>("laserlaunchSprite.sf", new JsonContentLoader());
            this.ASLaserLaunch = new AnimatedSprite(spriteSheet);
            spriteSheet = Content.Load<SpriteSheet>("rayonmagiqueSprite.sf", new JsonContentLoader());
            this.ASLaser = new AnimatedSprite(spriteSheet);
            spriteSheet = Content.Load<SpriteSheet>("glowingarm1Sprite.sf", new JsonContentLoader());
            this.ASArm = new AnimatedSprite(spriteSheet);
            rectHp = new Texture2D(gd,gd.Viewport.Width, 10);
            Color[] data = new Color[gd.Viewport.Width * 10];
            for (int i = 0; i < data.Length; i++) data[i] = Color.Red;
            rectHp.SetData(data);
        }

        public void Update(GameTime gameTime, int windowWidth, int windowHeight)
        {
            y = windowHeight / 3;
            float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float vitesseBras = deltaSeconds * vitesse;
            rect = (int)Math.Round(this.hp,0) * windowWidth / this.maxHP;
            if (map.Actif)
            {                
                if (!this.Dead)
                {
                    if (!this.Actif)
                    {
                        this.Eveil(gameTime);                        
                    }
                    else
                    {
                        if (!this.Pret)
                        {
                            ProchaineAttaque(gameTime);
                            animation = "idleBoss";
                            positionBras = new Vector2(windowWidth + 100, rand.Next(windowHeight/5,windowHeight/5*3));
                        }
                        if (this.hp <= 0)
                        {
                            this.Dead = true;
                            animation = "deathoreveilBoss";
                            timerDeath = new Timer(2.8f);
                        }
                        if (this.Pret)
                        {
                            if (timerAttaque == null)
                            {
                                timerAttaque = new Timer(rand.Next(5, 10));
                                x = rand.Next(windowWidth/4, windowWidth / 4 * 3);
                                animationRand = rand.Next(4);
                                switch (animationRand)
                                {
                                    case 0:
                                        animation = "lightBoss"; break;
                                    case 1:
                                        animation = "attackArmureBoss"; break;
                                    case 2:
                                        animation = "lightHeadBoss"; break;
                                    default:
                                        animation = "attackBrasBoss"; break;
                                }
                            }
                            else if (timerAttaque.Tick <= 2.16f)
                            {
                                this.ASLaserLaunch.Play("laserlaunch");
                            }
                            else
                            {
                                this.ASLaser.Play("laser");
                            }
                            if(this.IsCollision() && timerAttaque.Tick > 3)
                            {
                                hero.hp -= 1;
                                timerAttaque = null;
                                timerRepos = null;
                                this.Pret = false;
                            }
                            else if (timerAttaque.AddTick(deltaSeconds) == false)
                            {
                                timerAttaque = null;
                                timerRepos = null;
                                this.Pret = false;
                            }
                            if (positionBras.X > 0)
                            {
                                positionBras.X -= vitesseBras;
                            }
                            else
                            {
                                positionBras.X = windowWidth + 100;
                                positionBras.Y = rand.Next(windowHeight / 5, windowHeight / 5 * 3);
                            }                          
                            this.ASArm.Play("glowing");
                        }
                    }                                     
                }
                else if (timerDeath.AddTick(deltaSeconds) == false)
                {
                    animation = "idleéveilBoss";
                    this.Actif = false;
                    changementMusique = true;
                }
                this.ASBoss.Play(animation);
            }                
            this.ASBoss.Update(gameTime);
            this.ASLaserLaunch.Update(gameTime);
            this.ASLaser.Update(gameTime);
        }
        public void Draw(GameTime gameTime, SpriteBatch _spriteBatch, int windowWidth, int windowHeight)
        {
            
            _spriteBatch.Draw(this.ASBoss, this.PositionBoss);
            _spriteBatch.Draw(this.rectHp, new Rectangle(0,0, rect, 10), Color.Red);
            if (this.Pret)
            {
                if (timerAttaque != null && this.hp > 0)
                {
                    if (timerAttaque.Tick <= 2.16f)
                        _spriteBatch.Draw(this.ASLaserLaunch, new Vector2(x, 40));
                    else
                        _spriteBatch.Draw(this.ASLaser, new Vector2(x, y));
                    _spriteBatch.Draw(this.ASArm, positionBras);
                }                
            }
        }

        public string Path
        {
            get => path;
            private set => path = value;
        }
        public AnimatedSprite ASBoss
        {
            get => asBoss;
            private set => asBoss = value;
        }
        
        public AnimatedSprite ASLaser
        {
            get => asLaser;
            private set => asLaser = value;
        }
        
        public AnimatedSprite ASLaserLaunch
        {
            get => asLaserLaunch;
            private set => asLaserLaunch = value;
        }
        public AnimatedSprite ASArm
        {
            get => asArm;
            private set => asArm = value;
        }
        public Vector2 PositionBoss;
        public bool Eveil(GameTime gt)
        {
            float deltaSeconds = (float)gt.ElapsedGameTime.TotalSeconds;
            if (timerEveil == null)
            {
                timerEveil = new Timer(10f);
                return false;
            }
            else
            {                           
                if (timerEveil.AddTick(deltaSeconds) == false)
                {
                    this.Actif = true;
                    return true;
                }
                else
                {
                    if (timerEveil.Tick >= 5.25f)
                    {
                        animation = "eveilBoss";
                        return false;
                    }
                }
            }
            return false;
        }
        public void ProchaineAttaque(GameTime gt)
        {
            float deltaSeconds = (float)gt.ElapsedGameTime.TotalSeconds;
            if (this.Actif)
            {
                if (timerRepos == null || timerRepos.AddTick(deltaSeconds) == false)
                {
                    timerRepos = new Timer(rand.Next(2, 10));
                }
                else if (timerRepos.AddTick(deltaSeconds) == false)
                {
                    this.Pret = true;
                }
            }
        }
        public bool IsCollision()
        {
            Rectangle rectlaser = new Rectangle(x, y, this.ASLaser.TextureRegion.Width, this.ASLaser.TextureRegion.Height);
            Rectangle rectarm = new Rectangle((int)positionBras.X, (int)positionBras.Y, this.ASArm.TextureRegion.Width, this.ASArm.TextureRegion.Height);
            Rectangle rectHero = new Rectangle((int)hero.PositionHero.X, (int)hero.PositionHero.Y, hero.ASHero.TextureRegion.Width, hero.ASHero.TextureRegion.Height);
            if (rectlaser.Contains(rectHero) || rectarm.Contains(rectHero))
            {

                return true;
            }
                
            else
                return false;
        }

        /*
                    KeyboardState keyboardState = Keyboard.GetState();
                    if (keyboardState.IsKeyDown(Keys.A)) animation = "idleBoss";
                    else if (keyboardState.IsKeyDown(Keys.Z)) animation = "lightBoss";
                    else if (keyboardState.IsKeyDown(Keys.E)) animation = "attackBrasBoss";
                    else if (keyboardState.IsKeyDown(Keys.R)) animation = "lightHeadBoss";
                    else if (keyboardState.IsKeyDown(Keys.T)) animation = "attackArmureBoss";
                    else if (keyboardState.IsKeyDown(Keys.Y)) animation = "deathoreveilBoss";
                    else if (keyboardState.IsKeyDown(Keys.U)) animation = "idleDeathBoss";
                    else if (keyboardState.IsKeyDown(Keys.I)) animation = "eveilBoss";
                    else animation = "idleéveilBoss";*/
    }
}
