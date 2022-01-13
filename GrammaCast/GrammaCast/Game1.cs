using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;

namespace GrammaCast
{
    public class Game1 : Game
    {
        public GraphicsDeviceManager _graphics;
        public SpriteBatch _spriteBatch;
        public SpriteBatch SpriteBatch { get; set; }
        public Vector2 positionHero;
        Hero heroMage;
        MapForet mapForet;
        Boss bossGolem;
        Ennemi[] ennemisForet;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            mapForet = new MapForet("foret");
            if (mapForet.Path == "foret") positionHero = new Vector2(112, 720);
            else positionHero = new Vector2(200, 200);
            bossGolem = new Boss("BossSprite.sf", new Vector2(GraphicsDevice.Viewport.Width/2, GraphicsDevice.Viewport.Height/4));
            heroMage = new Hero("HeroSprite.sf", positionHero, 125) { map = mapForet };

            ennemisForet = new Ennemi[]
            {
                new Ennemi(new Vector2(112, 530),40) { map = mapForet, perso = heroMage},
                new Ennemi(new Vector2(528, 480),40) { map = mapForet, perso = heroMage},
                new Ennemi(new Vector2(512, 116),40) { map = mapForet, perso = heroMage},
                new Ennemi(new Vector2(336, 48),40) { map = mapForet, perso = heroMage}
            };

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            mapForet.LoadContent(Content, GraphicsDevice);
            _graphics.PreferredBackBufferWidth = mapForet.TileMap.Height * mapForet.TileMap.TileHeight;
            _graphics.PreferredBackBufferHeight = mapForet.TileMap.Width * mapForet.TileMap.TileWidth;
            _graphics.ApplyChanges();
            bossGolem.LoadContent(Content);
            heroMage.LoadContent(Content);
            foreach (Ennemi ef in ennemisForet)
            {
                ef.LoadContent(Content);
            }


            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            mapForet.Update(gameTime);
            bossGolem.Update(gameTime, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            heroMage.Update(gameTime, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            foreach (Ennemi ef in ennemisForet)
            {
                ef.Update(gameTime, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            //GraphicsDevice.BlendState = BlendState.AlphaBlend;
            _spriteBatch.Begin();
            mapForet.Draw();
            bossGolem.Draw(gameTime, _spriteBatch);
            heroMage.Draw(gameTime, _spriteBatch);
            foreach (Ennemi ef in ennemisForet)
            {
                ef.Draw(gameTime, _spriteBatch);
            }
            _spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
