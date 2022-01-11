using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;

namespace GrammaCast
{
    public class Hero
    {
        private Vector2 positionHero;
        private Vector2 vitesseHero;
        private AnimatedSprite asHero;
        private string path;

        public Hero(string path, Vector2 positionHero, Vector2 vitesseHero)
        {
            Path = path;
            PositionHero = positionHero;
            VitesseHero = vitesseHero;
        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
        {

            SpriteSheet spriteSheet = Content.Load<SpriteSheet>(Path, new JsonContentLoader());
            AnimatedSprite asHero = new AnimatedSprite(spriteSheet);
        }

        public void Update(GameTime gameTime)
        {
            this.ASHero.Play("idleWest");
            this.ASHero.Update(gameTime);
        }
        public void Draw(GameTime gameTime, SpriteBatch _spriteBatch)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(this.ASHero, this.PositionHero);
            _spriteBatch.End();
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
        public Vector2 PositionHero
        {
            get => positionHero;
            private set => positionHero = value;
        }
        public Vector2 VitesseHero
        {
            get => VitesseHero;
            private set => VitesseHero = value;
        }

    }
}
