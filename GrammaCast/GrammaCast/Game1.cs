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
        MapVillage[] mapVillage;
        Boss bossGolem;
        Ennemi[] ennemisForet;
        public Attaque attaqueGramma;
        int indice = 0;


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
            mapVillage = new MapVillage[] { new MapVillage("LeHameau"), new MapVillage("LeHameau_2") };
            mapVillage[0].Actif = true;


            Vector2 positionHero = new Vector2(64, 192);
            bossGolem = new Boss("BossSprite.sf", new Vector2(GraphicsDevice.Viewport.Width/2, GraphicsDevice.Viewport.Height/4));
            heroMage = new Hero("HeroSprite.sf", positionHero, 125) { mapV = mapVillage, mapF = mapForet };
            attaqueGramma = new Attaque() { perso = heroMage};
            ennemisForet = new Ennemi[]
            {
                new Ennemi(new Vector2(112, 530),40) { map = mapForet, perso = heroMage, attaqueLetter = attaqueGramma},
                new Ennemi(new Vector2(528, 480),40) { map = mapForet, perso = heroMage, attaqueLetter = attaqueGramma},
                new Ennemi(new Vector2(512, 116),40) { map = mapForet, perso = heroMage, attaqueLetter = attaqueGramma},
                new Ennemi(new Vector2(336, 48),40) { map = mapForet, perso = heroMage, attaqueLetter = attaqueGramma}
            };
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            foreach(MapVillage v in mapVillage)
            {
                v.LoadContent(Content, GraphicsDevice);
            }
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
            attaqueGramma.LoadContent(Content);


            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (mapVillage[indice].Actif)
            {
                mapVillage[indice].Update(gameTime);
                if (heroMage.TestTransitionV(mapVillage[indice]))
                {
                    if(indice == 0)
                    {
                        mapVillage[indice].Actif = false;
                        indice = 1;
                        mapVillage[indice].Actif = true;
                        positionHero = new Vector2(20, heroMage.PositionHero.Y);
                        heroMage.PositionHero = positionHero;
                    }
                    else
                    {
                        if (heroMage.PositionHero.X < GraphicsDevice.Viewport.Width / 2)
                        {
                            mapVillage[indice].Actif = false;
                            indice = 0;
                            mapVillage[indice].Actif = true;
                            positionHero = new Vector2(GraphicsDevice.Viewport.Width - 20, heroMage.PositionHero.Y);
                            heroMage.PositionHero = positionHero;
                        }
                        else
                        {
                            mapVillage[indice].Actif = false;
                            mapForet.Actif = true;
                            positionHero = new Vector2(20, heroMage.PositionHero.Y);
                            heroMage.PositionHero = positionHero;
                        }

                    }

                    
                }
            }
            else if (mapForet.Actif)
            {
                mapForet.Update(gameTime);
                if (heroMage.TestTransitionF(mapForet))
                {
                    mapVillage[indice].Actif = true;
                    mapForet.Actif = false;
                    positionHero = new Vector2(GraphicsDevice.Viewport.Width - 20, heroMage.PositionHero.Y);
                    heroMage.PositionHero = positionHero;
                }
            }
            bossGolem.Update(gameTime, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            heroMage.Update(gameTime, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            if (mapForet.Actif)
            {
                foreach (Ennemi ef in ennemisForet)
                {
                    ef.Update(gameTime, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
                }
            }
            if (attaqueGramma.Actif) attaqueGramma.Update(gameTime);

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            //GraphicsDevice.BlendState = BlendState.AlphaBlend;
            _spriteBatch.Begin();
            if (mapVillage[indice].Actif)
                mapVillage[indice].Draw();
            else if 
                (mapForet.Actif) mapForet.Draw();
            bossGolem.Draw(gameTime, _spriteBatch);
            if (mapForet.Actif)
            {
                foreach (Ennemi ef in ennemisForet)
                {
                    ef.Draw(gameTime, _spriteBatch);
                }
            }
                
            if (attaqueGramma.Actif) 
                attaqueGramma.Draw(gameTime, _spriteBatch);
            heroMage.Draw(gameTime, _spriteBatch);
            
            _spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
