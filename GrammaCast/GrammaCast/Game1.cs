using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace GrammaCast
{
    /*
    Classe du jeu.
    La classe s'appelle "Game1" pour ne pas la confondre avec son parent Game (venant de MonoGame)
    */
    public class Game1 : Game
    {
        public GraphicsDeviceManager _graphics;
        public SpriteBatch _spriteBatch;
        public SpriteBatch SpriteBatch { get; set; }
        public Vector2 positionHero;
        Hero heroMage;
        MapForet mapForet;
        MapVillage[] mapVillage;
        MapBoss[] mapBoss;
        Boss bossGolem;
        Ennemi[] ennemisForet;
        Villageois[] villageois;
        public Attaque attaqueGramma;
        public AttaqueBoss attaqueSpell;
        int indice = 0;
        int indiceB = 0;
        private Song songTitle;
        private Song songForest;
        private Song songBoss;
        private Song songFinal;
        public bool changementActif; //permet d'indiquer s'il faut changer la musique
        Texture2D _darken;
        Dialogue dialogue;
        string songActuel = "s";

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {

            // On charge les différentes maps
            mapForet = new MapForet("foret");
            mapVillage = new MapVillage[] { new MapVillage("LeHameau"), new MapVillage("LeHameau_2") };
            mapVillage[0].Actif = true; // Map de départ
            mapBoss = new MapBoss[] { new MapBoss("ZoneDeLaTour"), new MapBoss("ZoneFinale") };


            Vector2 positionHero = new Vector2(64, 192); // Position de départ du joueur
            
            // ..On initialise notre joueur, et le boss..
            heroMage = new Hero("HeroSprite.sf", positionHero, 125) { mapV = mapVillage, mapF = mapForet, mapB = mapBoss };
            bossGolem = new Boss("BossSprite.sf", new Vector2(387, 65)) 
            { map = mapBoss[1], hero = heroMage};
            bossGolem.Block = true;

            // ..les attaques..
            attaqueGramma = new Attaque() { perso = heroMage};
            attaqueSpell = new AttaqueBoss() {perso = heroMage, golem = bossGolem };

            // ..les ennemis..
            ennemisForet = new Ennemi[]
            {
                new Ennemi(new Vector2(112, 530),40) { map = mapForet, perso = heroMage, attaqueLetter = attaqueGramma},
                new Ennemi(new Vector2(528, 480),40) { map = mapForet, perso = heroMage, attaqueLetter = attaqueGramma},
                new Ennemi(new Vector2(512, 116),40) { map = mapForet, perso = heroMage, attaqueLetter = attaqueGramma},
                new Ennemi(new Vector2(336, 48),40) { map = mapForet, perso = heroMage, attaqueLetter = attaqueGramma}
            };

            // ..et les villageois !
            villageois = new Villageois[]
            {
                new Villageois(new Vector2(256, 192),"villagerSprite.sf") { map = mapVillage[0], perso = heroMage},
                new Villageois(new Vector2(448, 176),"villager2Sprite.sf") { map = mapVillage[1], perso = heroMage},
                new Villageois(new Vector2(240, 624),"villageoiseSprite.sf") { map = mapVillage[1], perso = heroMage}
            };

            dialogue = new Dialogue() { perso = heroMage, villageois = villageois[0], map = mapBoss, golem = bossGolem};
            villageois[0].Block = true;

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.25f;

            changementActif = false;  //permet d'indiquer s'il faut changer la musique
            base.Initialize(); // On initialise Game (le parent)
        }

        // Après leur initialisation, on charge leurs sprites
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            songTitle = Content.Load<Song>("menu");
            songForest = Content.Load<Song>("forest");
            songBoss = Content.Load<Song>("bossMusic");
            songFinal = Content.Load<Song>("endgame");
            foreach (MapVillage v in mapVillage)
            {
                v.LoadContent(Content, GraphicsDevice);
            }
            foreach (MapBoss b in mapBoss)
            {
                b.LoadContent(Content, GraphicsDevice);
            }
            mapForet.LoadContent(Content, GraphicsDevice);
            _graphics.PreferredBackBufferWidth = mapForet.TileMap.Height * mapForet.TileMap.TileHeight;
            _graphics.PreferredBackBufferHeight = mapForet.TileMap.Width * mapForet.TileMap.TileWidth;
            _graphics.ApplyChanges();
            heroMage.LoadContent(Content, GraphicsDevice);
            bossGolem.LoadContent(Content, GraphicsDevice);            
            foreach (Ennemi ef in ennemisForet)
            {
                ef.LoadContent(Content);
            }
            foreach (Villageois v in villageois)
            {
                v.LoadContent(Content);
            }
            attaqueGramma.LoadContent(Content);
            attaqueSpell.LoadContent(Content);
            dialogue.LoadContent(Content);
            _darken = Content.Load<Texture2D>("nouar");
            MediaPlayer.Play(songTitle);
        }

        // Fonction appelée par la bibliothèque
        // On s'en sert pour transmettre le signal d'update à toutes les entités/etc... du jeu
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            if (mapVillage[0].Actif)
            {
                villageois[0].Update(gameTime);
            }
            else if (mapVillage[1].Actif)
            {
                villageois[1].Update(gameTime);
                villageois[2].Update(gameTime);
            }

            //transition, changement de map et updates des maps
            if (mapVillage[indice].Actif) 
            {
                mapVillage[indice].Update(gameTime);
                if (heroMage.TestTransitionV(mapVillage[indice]))
                {
                    if (indice == 0)
                    {
                        mapVillage[indice].Actif = false;
                        indice++;
                        mapVillage[indice].Actif = true;
                        positionHero = new Vector2(20, heroMage.PositionHero.Y);
                        heroMage.PositionHero = positionHero;
                    }
                    else
                    {
                        if (heroMage.PositionHero.X < GraphicsDevice.Viewport.Width / 2)
                        {
                            mapVillage[indice].Actif = false;
                            indice--;
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
                            if (!bossGolem.Dead)
                                changementActif = true; //permet d'indiquer s'il faut changer la musique
                        }
                    }
                }
            }
            else if (mapForet.Actif)
            {
                mapForet.Update(gameTime);
                if (heroMage.PositionHero.Y > GraphicsDevice.Viewport.Height / 2)
                {
                    if (heroMage.TestTransitionF(mapForet))
                    {
                        mapVillage[indice].Actif = true;
                        mapForet.Actif = false;
                        positionHero = new Vector2(GraphicsDevice.Viewport.Width - 20, heroMage.PositionHero.Y);
                        heroMage.PositionHero = positionHero;
                        if (!bossGolem.Dead)
                            changementActif = true;
                    }
                }
                else
                {
                    if (heroMage.TestTransitionF(mapForet))
                    {
                        mapBoss[indiceB].Actif = true;
                        mapForet.Actif = false;
                        positionHero = new Vector2(heroMage.PositionHero.X, GraphicsDevice.Viewport.Height - 60);
                        heroMage.PositionHero = positionHero;
                    }
                }

            }
            else if (mapBoss[indiceB].Actif)
            {
                mapBoss[indiceB].Update(gameTime);
                if(indiceB == 0)
                {
                    if (heroMage.PositionHero.Y > GraphicsDevice.Viewport.Height / 4 * 3)
                    {
                        if (heroMage.TestTransitionB(mapBoss[indiceB]))
                        {
                            mapForet.Actif = true;
                            mapBoss[indiceB].Actif = false;
                            positionHero = new Vector2(heroMage.PositionHero.X, 20);
                            heroMage.PositionHero = positionHero;
                        }
                    }
                    if (heroMage.TestTransitionB(mapBoss[indiceB]))
                    {
                        if (bossGolem.Dead)
                        {

                        }
                        else if (attaqueGramma.NbrPoint())
                        {
                            mapBoss[indiceB].Actif = false;
                            indiceB++;
                            mapBoss[indiceB].Actif = true;
                            positionHero = new Vector2(380, 380);
                            heroMage.PositionHero = positionHero;
                            if (!bossGolem.Dead)
                                changementActif = true; //permet d'indiquer s'il faut changer la musique
                            MediaPlayer.Volume = 0.05f;
                            heroMage.Block = true;
                        }
                        else if (!bossGolem.Dead)
                        {
                            heroMage.Block = true;
                        }  
                    }
                }
                else
                {
                    if (heroMage.TestTransitionB(mapBoss[indiceB]) && bossGolem.Dead)
                    {
                        mapBoss[indiceB].Actif = false;
                        indiceB--;
                        mapBoss[indiceB].Actif = true;
                        positionHero = new Vector2(376, 464);
                        heroMage.PositionHero = positionHero;
                    }
                }
                
            }
            if (mapBoss[1].Actif == true)
            {
                bossGolem.Update(gameTime, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
                if (!bossGolem.Dead)
                    attaqueSpell.Update(gameTime, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
                else if (songActuel != "songFinal")
                    changementActif = true;
            }
            
            heroMage.Update(gameTime, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            if (mapForet.Actif && !bossGolem.Dead)
            {
                foreach (Ennemi ef in ennemisForet)
                {
                    ef.Update(gameTime, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
                }
            }
            if (attaqueGramma.Actif)
                attaqueGramma.Update(gameTime, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            if (bossGolem.hp <= 0 && !bossGolem.Dead)
            {
                MediaPlayer.Volume -= 0.01f;
            }

            if (changementActif)  //permet d'indiquer s'il faut changer la musique
            {
                Musique();
            }

            if (heroMage.Block)
            {
                dialogue.Update(gameTime);
            }

            base.Update(gameTime);
            
        }

        // Dessine les éléments du jeu
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            if (mapVillage[indice].Actif)
                mapVillage[indice].Draw();
            else if (mapForet.Actif) 
                mapForet.Draw();
            else if (mapBoss[indiceB].Actif)
                mapBoss[indiceB].Draw();
            if (mapBoss[1].Actif == true)
            {
                bossGolem.Draw(gameTime, _spriteBatch, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

                if (attaqueSpell.Actif)
                    attaqueSpell.Draw(gameTime, _spriteBatch);
            }
            if (mapForet.Actif && !bossGolem.Dead)
            {
                foreach (Ennemi e in ennemisForet)
                {
                    e.Draw(gameTime, _spriteBatch);
                }
            }
            if (mapVillage[0].Actif)
            {
                villageois[0].Draw(gameTime, _spriteBatch);
            }
            else if (mapVillage[1].Actif)
            {
                villageois[1].Draw(gameTime, _spriteBatch);
                villageois[2].Draw(gameTime, _spriteBatch);
            }
            if (attaqueGramma.Actif) 
                attaqueGramma.Draw(gameTime, _spriteBatch);
            heroMage.Draw(gameTime, _spriteBatch, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            if (!bossGolem.Dead)
                _spriteBatch.Draw(_darken, new Vector2(0,0), Color.White * 0.2f);
            if (heroMage.Block)
            {
                dialogue.Draw(gameTime, _spriteBatch, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            }
            _spriteBatch.End();
            
            base.Draw(gameTime);
        }
        public void Musique()
        {
            if ((mapForet.Actif || mapBoss[0].Actif) && !bossGolem.Dead)
            {
                MediaPlayer.Play(songForest);
                changementActif = false; 
                MediaPlayer.Volume = 0.25f;
            }
            else if (bossGolem.Actif)
            {
                MediaPlayer.Play(songBoss);
                changementActif = false;
                MediaPlayer.Volume = 0.25f;
            }
            
            else if (bossGolem.Dead && !bossGolem.Actif)
            {
                MediaPlayer.Play(songFinal);
                changementActif = false;
                MediaPlayer.Volume = 0.25f;
                songActuel = "songFinal";
            }
            else if (mapVillage[0].Actif || (mapVillage[1].Actif))
            {
                MediaPlayer.Play(songTitle);
                changementActif = false;
                MediaPlayer.Volume = 0.25f;
            }
        }
    }
}
