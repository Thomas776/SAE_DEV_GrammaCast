using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;

namespace GrammaCast
{
    class Boss
    {
        private AnimatedSprite asBoss;
        private string path;
        private Vector2 positionBoss;

        public Boss(string path, Vector2 positionBoss)
        {
            Path = path;
            PositionBoss = positionBoss;
        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
        {

            SpriteSheet spriteSheet = Content.Load<SpriteSheet>(this.Path, new JsonContentLoader());
            this.ASBoss = new AnimatedSprite(spriteSheet);
        }

        public void Update(GameTime gameTime, float windowWidth, float windowHeight)
        {
            string animation;
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.A)) animation = "idleBoss";
            else if (keyboardState.IsKeyDown(Keys.Z)) animation = "lightBoss";
            else if (keyboardState.IsKeyDown(Keys.E)) animation = "attackBrasBoss";
            else if (keyboardState.IsKeyDown(Keys.R)) animation = "lightHeadBoss";
            else if (keyboardState.IsKeyDown(Keys.T)) animation = "attackArmureBoss";
            else if (keyboardState.IsKeyDown(Keys.Y)) animation = "deathoreveilBoss";
            else if (keyboardState.IsKeyDown(Keys.U)) animation = "idleDeathBoss";
            else if (keyboardState.IsKeyDown(Keys.I)) animation = "eveilBoss";
            else animation = "idleéveilBoss";

            this.ASBoss.Play(animation);
            this.ASBoss.Update(gameTime);
        }
        public void Draw(GameTime gameTime, SpriteBatch _spriteBatch)
        {
            
            _spriteBatch.Draw(this.ASBoss, this.PositionBoss);
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
        public Vector2 PositionBoss;

    }
}
