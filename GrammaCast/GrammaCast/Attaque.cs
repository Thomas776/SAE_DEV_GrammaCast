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
        public static string[] spriteChemin = new string[] { "IceCastSprite.sf",
            "FireCastSprite.sf", "HolyExplosionSprite.sf", "IceShatterSprite.sf", "PoisonCastSprite.sf"};
        private string[] alphabet = new string[] { "A", "B", "C", "D", "E", "F", "G",
            "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        private SpriteSheet[] attaqueSprite = new SpriteSheet[spriteChemin.Length];
        public Hero perso;
        public Ennemi ennemi;
        private string fontPath;

        private SpriteFont attaqueFont;
        private AnimatedSprite asAttack;
        private string attaqueLettre;
        public Timer timerAnimation;
        public Timer timerAttaque;
        Random rand = new Random();
        public float point = 400;
        public float sommePoint = 0;


        public Attaque()
        {
            FontPath = "font";
            Actif = false;
            Final = false;
            Animation = false;
            AttaqueLettre = this.alphabet[rand.Next(alphabet.Length)];
        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            this.AttaqueFont = Content.Load<SpriteFont>(this.FontPath);
            for (int i = 0; i < attaqueSprite.Length; i++)
            {
                attaqueSprite[i] = Content.Load<SpriteSheet>(spriteChemin[i], new JsonContentLoader());
            }
            this.AsAttack = new AnimatedSprite(attaqueSprite[rand.Next(attaqueSprite.Length)]);
        }
        public void Update(GameTime gameTime)
        {

            float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (this.Actif)
            {
                if (timerAttaque == null)
                {
                    timerAttaque = new Timer(1000);
                }
                else
                    timerAttaque.AddTick(deltaSeconds);
            }
            if (this.Final)
            {
                this.AsAttack.Play("attack");

                if (timerAnimation.AddTick(deltaSeconds) == false)
                {
                    
                    sommePoint += point / timerAttaque.Tick;
                    Console.WriteLine($"{sommePoint}, {timerAttaque.Tick}");
                    timerAttaque = null;
                    this.Final = false;
                    this.Animation = false;
                    this.Actif = false;
                    this.AttaqueLettre = this.alphabet[rand.Next(alphabet.Length)];
                    this.AsAttack = new AnimatedSprite(attaqueSprite[rand.Next(attaqueSprite.Length)]);
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
            if (this.Animation)
                _spriteBatch.Draw(this.AsAttack, new Vector2(perso.PositionHero.X, perso.PositionHero.Y - 100));
            _spriteBatch.DrawString(this.AttaqueFont, $"{this.AttaqueLettre}", new Vector2(perso.PositionHero.X,perso.PositionHero.Y -100), Color.White);
            
        }
        public string FontPath
        {
            get => fontPath;
            private set => fontPath = value;
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
