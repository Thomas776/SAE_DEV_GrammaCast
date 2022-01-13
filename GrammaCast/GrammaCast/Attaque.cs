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
        private string fontPath;
        private string spritePath;
        private SpriteFont attaqueFont;
        private AnimatedSprite asAttack;
        private string attaqueLettre;
        //public Timer timerAttaque;
        Random rand = new Random();



        public Attaque()
        {
            FontPath = "font";
            SpritePath = spriteChemin[rand.Next(spriteChemin.Length)];
            Actif = false;
            Final = false;
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
            if (this.Final)
            {
                this.AsAttack.Play("attack");
                this.Final = false;
                this.Actif = false;
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
        public string AttaqueLettre
        {
            get => attaqueLettre;
            private set => attaqueLettre = value;
        }
        public bool GetLetter()
        {
            string[] test = new string[100];
            KeyboardState keyboardState = Keyboard.GetState();
            foreach(string testLettre in test)
            {
                if (testLettre == keyboardState.GetPressedKeys().ToString()) testLettre = keyboardState.GetPressedKeys().ToString();
            }
            ;
            if (this.AttaqueLettre ==)
            {
                this.Final = true;
                return true;
            }
            else return false;
        }
    }
}
