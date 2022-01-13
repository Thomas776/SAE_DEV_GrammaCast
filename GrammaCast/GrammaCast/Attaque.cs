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
    class Attaque
    {
        public string[] spriteChemin = new string[] { "IceCastSprite.sf" };
        private char[] alphabet = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        private string fontPath;
        private string spritePath;
        private SpriteFont attaqueFont;
        private AnimatedSprite asAttack;
        public Timer timerAttaque;
        Random rand = new Random();


        public Attaque(string fontPath)
        {
            FontPath = fontPath;
            SpritePath = spriteChemin[rand.Next(spriteChemin.Length)];
        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            this.AttaqueFont = Content.Load<SpriteFont>(this.FontPath);
            SpriteSheet spriteSheet = Content.Load<SpriteSheet>(this.FontPath, new JsonContentLoader());
            this.AsAttack = new AnimatedSprite(spriteSheet);
        }
        public void Update(GameTime gameTime, float windowWidth, float windowHeight)
        {

            this.AsAttack.Play("attack");
            this.AsAttack.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch _spriteBatch)
        {
            _spriteBatch.DrawString(this.AttaqueFont, $"{this.alphabet[rand.Next(alphabet.Length)]}", this.PositionAttaque, Color.White);
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
        public Vector2 PositionAttaque;
        public AnimatedSprite AsAttack
        {
            get => asAttack;
            private set => asAttack = value;
        }
    }
}
