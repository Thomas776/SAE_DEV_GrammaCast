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
    public class Attaque
    {
        public string[] spriteChemin = new string[] { "IceCastSprite.sf", "FireCastSprite.sf", "HolyExplosionSprite.sf", "IceShatterSprite.sf", "MagicBarrierSprite.sf", "PoisonCastSprite.sf" ,};
        private string[] alphabet = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        public Hero perso;
        public Ennemi ennemi;
        private string fontPath;
        private string spritePath;
        private SpriteFont attaqueFont;
        private AnimatedSprite asAttack;
        private string attaqueLettre;
        public Timer timerAnimation;
        Random rand = new Random();

        private string _stringValue = string.Empty;

        public Attaque()
        {
            FontPath = "font";
            SpritePath = spriteChemin[rand.Next(spriteChemin.Length)];
            Actif = false;
            Final = false;
            Animation = false;
            AttaqueLettre = this.alphabet[rand.Next(alphabet.Length)];
        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            this.AttaqueFont = Content.Load<SpriteFont>(this.FontPath);
            SpriteSheet spriteSheet = Content.Load<SpriteSheet>(this.SpritePath, new JsonContentLoader());
            this.AsAttack = new AnimatedSprite(spriteSheet);
        }
        public void Update(GameTime gameTime/*, float windowWidth, float windowHeight*/)
        {

            float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (this.Final)
            {                
               
                this.AsAttack.Play("attack");
                if (timerAnimation.AddTick(deltaSeconds) == false)
                {
                    this.Final = false;
                    this.Animation = false;
                    this.Actif = false;
                    this.SpritePath = spriteChemin[rand.Next(spriteChemin.Length)];
                    this.AttaqueLettre = this.alphabet[rand.Next(alphabet.Length)];
                }
            }
            else
            {
                this.GetLetter();
            }

            this.AsAttack.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch _spriteBatch)
        {
            _spriteBatch.DrawString(this.AttaqueFont, $"{this.AttaqueLettre}", new Vector2(perso.PositionHero.X,perso.PositionHero.Y -100), Color.White);
            if (this.Animation) 
                _spriteBatch.Draw(this.AsAttack, new Vector2(perso.PositionHero.X, perso.PositionHero.Y - 100));
        }
        public string FontPath
        {
            get => fontPath;
            private set => fontPath = value;
        }
        public string SpritePath
        {
            get => spritePath;
            private set => spritePath = value;
        }
        public SpriteFont AttaqueFont
        {
            get => attaqueFont;
            private set => attaqueFont = value;
        }
        public AnimatedSprite AsAttack
        {
            get => asAttack;
            private set => asAttack = value;
        }
        public bool Actif;
        public bool Final;
        public bool Animation;
        public string AttaqueLettre
        {
            get => attaqueLettre;
            private set => attaqueLettre = value;
        }
        public bool GetLetter()
        {
            //Console.WriteLine("aaaa");
            var keyboardState = Keyboard.GetState();
            var keys = keyboardState.GetPressedKeys();
            foreach (var key in keys)
            {
                if (key.ToString() == this.AttaqueLettre)
                {
                    timerAnimation = new Timer(0.4f);
                    this.Final = true;
                    this.Animation = true;
                    return true;
                }
            }
            return false;


        }
    }
}
