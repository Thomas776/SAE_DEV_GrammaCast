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

        //indice des tableaux de map pour les transitions 
        int indice = 0; //village
        int indiceB = 0; //boss

        private Song songTitle;
        private Song songForest;
        private Song songBoss;
        private Song songFinal;

        public bool changementActif; //permet d'indiquer s'il faut changer la musique
        Texture2D _darken; //filtre noir au début de la partie qui disparait quand le boss meurt

        Dialogue dialogue;

        string songActuel = "s"; //permet de charger le dernier song

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {

            // On charge les différentes maps
            mapForet = new MapForet("ReBuild");
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

            //les dialogue
            dialogue = new Dialogue() { perso = heroMage, villageois = villageois[0], map = mapBoss, golem = bossGolem};
            villageois[0].Block = true; //permet de lancer le dialogue dès le départ quand le joueur va se rapprocher

            MediaPlayer.IsRepeating = true; //pour éviter qu'elle se coupe
            MediaPlayer.Volume = 0.25f; //sinon la musiques est trop forte

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

            _darken = Content.Load<Texture2D>("nouar"); //filtre noir au début de la partie qui disparait quand le boss meurt

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
            if (mapVillage[indice].Actif) //si map village
            {
                mapVillage[indice].Update(gameTime); //update village
                if (heroMage.TestTransitionV(mapVillage[indice])) //si y'a une transition
                {
                    if (indice == 0) //première map
                    {
                        mapVillage[indice].Actif = false; //map actuel
                        indice++;                           //changement map
                        mapVillage[indice].Actif = true; //map suivante
                        positionHero = new Vector2(20, heroMage.PositionHero.Y); //nouvelle position joueur
                        heroMage.PositionHero = positionHero;
                    }
                    else
                    {
                        if (heroMage.PositionHero.X < GraphicsDevice.Viewport.Width / 2) //pour la 2e map il y a 2 transitions dont test de
                                                                               //ou se situe le perso
                                                                               //donc si le perso se trouve dans la partie gauche de la map
                        {
                            mapVillage[indice].Actif = false;//map actuel
                            indice--;                           //changement map (ici toujours village)
                            mapVillage[indice].Actif = true;//map suivante
                            positionHero = new Vector2(GraphicsDevice.Viewport.Width - 20, heroMage.PositionHero.Y); //changement position perso
                            heroMage.PositionHero = positionHero;
                        }
                        else //sinon le perso est dans la partie droite donc changer vers le foret
                        {
                            mapVillage[indice].Actif = false; //map village
                            mapForet.Actif = true; //foret
                            //l'indice ne change pas pour revenir via la foret au village

                            positionHero = new Vector2(20, heroMage.PositionHero.Y);
                            heroMage.PositionHero = positionHero;
                            if (!bossGolem.Dead) 
                                changementActif = true; //permet d'indiquer s'il faut changer la musique
                        }
                    }
                }
            }
            else if (mapForet.Actif) //si la foret est active
            {
                mapForet.Update(gameTime);
                if (heroMage.PositionHero.Y > GraphicsDevice.Viewport.Height / 2) //si le perso se trouve en la partie inférieure
                {
                    if (heroMage.TestTransitionF(mapForet))
                    {
                        //retour sur la map village
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
                    //passage vers la map de la tour
                    if (heroMage.TestTransitionF(mapForet))
                    {
                        mapBoss[indiceB].Actif = true;
                        mapForet.Actif = false;
                        positionHero = new Vector2(heroMage.PositionHero.X, GraphicsDevice.Viewport.Height - 60);
                        heroMage.PositionHero = positionHero;
                    }
                }

            }
            else if (mapBoss[indiceB].Actif) //map boss
            {
                mapBoss[indiceB].Update(gameTime);
                if(indiceB == 0) //si map tour
                {
                    if (heroMage.PositionHero.Y > GraphicsDevice.Viewport.Height / 4 * 3) //si le joueur se trouve tout en bas de la map
                    {
                        if (heroMage.TestTransitionB(mapBoss[indiceB]))
                        {
                            //retour foret
                            mapForet.Actif = true;
                            mapBoss[indiceB].Actif = false;
                            positionHero = new Vector2(heroMage.PositionHero.X, 20);
                            heroMage.PositionHero = positionHero;
                        }
                    }
                    if (heroMage.TestTransitionB(mapBoss[indiceB]))
                    {
                        //sinon test pour savoir si le perso peut aller combattre le boss
                        if (bossGolem.Dead)
                        {
                            //ne peut pas y retourner
                        }
                        else if (attaqueGramma.NbrPoint())
                        {
                            //si le perso a assez de point (3000)

                            mapBoss[indiceB].Actif = false;
                            indiceB++;
                            mapBoss[indiceB].Actif = true;
                            positionHero = new Vector2(380, 380);
                            heroMage.PositionHero = positionHero;

                            if (!bossGolem.Dead)
                                changementActif = true; //permet d'indiquer s'il faut changer la musique

                            MediaPlayer.Volume = 0.05f; //baisse la musique
                            heroMage.Block = true; //pour activer le dilaogue
                        }
                        else if (!bossGolem.Dead)
                        {
                            heroMage.Block = true; //sinon il ne peut pas aller dans la tour
                        }  
                    }
                }
                else
                {
                    if (heroMage.TestTransitionB(mapBoss[indiceB]) && bossGolem.Dead) //ne peut sortir que si le boss est mort
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

                else if (songActuel != "songFinal") //change la musique
                    changementActif = true;
            }
            
            heroMage.Update(gameTime, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            if (mapForet.Actif && !bossGolem.Dead) //fait apparaitre les ennemis que si le boss est actif et que le perso est dans la foret
            {
                foreach (Ennemi ef in ennemisForet)
                {
                    ef.Update(gameTime, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
                }
            }

            if (attaqueGramma.Actif)
                attaqueGramma.Update(gameTime, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            if (bossGolem.hp <= 0 && !bossGolem.Dead) //ne fonctionne pas, pas eu le temps de vérfier
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
                _spriteBatch.Draw(_darken, new Vector2(0,0), Color.White * 0.25f);

            if (heroMage.Block)
            {
                dialogue.Draw(gameTime, _spriteBatch, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            }

            _spriteBatch.End();
            
            base.Draw(gameTime);
        }
        public void Musique()
        {
            //permet de changer la musique selon la situations
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
