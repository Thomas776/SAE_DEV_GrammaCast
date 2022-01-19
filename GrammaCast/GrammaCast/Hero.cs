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
        /// Hero
        /// Personnage principal du jeu
      
        public MapVillage[] mapV;
        public MapForet mapF;
        public MapBoss[] mapB;

        private int vitesseHero; //vitesse de déplacement
        private AnimatedSprite asHero;
        private string path; //chemin d'accès du .sf (spritesheet)
        int indiceAnimation = 0; //indique quelle animation il doit faire (à voir dans le update)
        string animationBase = "idleSouth"; //animation de base pour éviter que ce soit null
        public int maxHP = 10;
        public int hp = 10;
        Texture2D rectHp; //barre de vie quand il combat le boss
        int rect; //longueur de la barre de vie
        public Hero(string path, Vector2 positionHero, int vitesseHero)
        {
            Path = path;
            PositionHero = positionHero;
            VitesseHero = vitesseHero;
            Block = false; //quand il est bloqué, plusieurs interractions sont possibles, en particulier des dialogues
        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content, GraphicsDevice gd)
        {
            //load du spritesheet
            SpriteSheet spriteSheet = Content.Load<SpriteSheet>(this.Path, new JsonContentLoader());
            this.ASHero = new AnimatedSprite(spriteSheet);

            //load de la barre de vie
            rectHp = new Texture2D(gd, gd.Viewport.Width/2, 10);
            Color[] data = new Color[gd.Viewport.Width/2 * 10];
            for (int i = 0; i < data.Length; i++) data[i] = Color.Red;
            rectHp.SetData(data);
        }

        public void Update(GameTime gameTime, int windowWidth, int windowHeight)
        {
            rect = this.hp * (windowWidth/3) / this.maxHP; //calcul de la longueur de la barre de vie en produit en croix
            string animation = animationBase;


            if (this.Block == false) 
                animation = this.DeplacementHero(gameTime, windowWidth, windowHeight, ref indiceAnimation);
            else
            {                
                switch (indiceAnimation)
                {
                    case 0:
                        animation = "idleSouth";
                        break;
                    case 1:
                        animation = "idleWest";
                        break;
                    case 2:
                        animation = "idleEast";
                        break;
                    case 3:
                        animation = "idleNorth";
                        break;
                }
            }
            this.ASHero.Play(animation);
            this.ASHero.Update(gameTime);
        }
        public void Draw(GameTime gameTime, SpriteBatch _spriteBatch, int windowWidth, int windowHeight)
        {
            _spriteBatch.Draw(this.ASHero, this.PositionHero);
            if (mapB[1].Actif) //si la map du boss est activée, la barre de vie apparaît
            {
                _spriteBatch.Draw(this.rectHp, new Rectangle(0, windowHeight-15, rect, 10), Color.Red);
            }
        }
        //définition des paramètres de la classe
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
        public Vector2 PositionHero;

        public int VitesseHero
        {
            get => vitesseHero;
            private set => vitesseHero = value;
        }
        public bool Block;
        public string DeplacementHero(GameTime gameTime, float windowWidth, float windowHeight, ref int indiceAnimation)
        {
            //permet de gérer les déplacements
            float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float walkSpeed = deltaSeconds * this.VitesseHero;
            string animation = animationBase;
            
            KeyboardState keyboardState = Keyboard.GetState();
            //collision fenêtre
            if (this.PositionHero.X >= windowWidth - this.ASHero.TextureRegion.Width / 3)
            {
                this.PositionHero.X -= walkSpeed;
            }
            else if (this.PositionHero.Y >= windowHeight - this.ASHero.TextureRegion.Height / 3)
            {
                this.PositionHero.Y -= walkSpeed;
            }
            else if (this.PositionHero.X <= 0 + this.ASHero.TextureRegion.Width / 3)
            {
                this.PositionHero.X += walkSpeed;
            }
            else if (this.PositionHero.Y <= 0 + this.ASHero.TextureRegion.Height / 3)
            {
                this.PositionHero.Y += walkSpeed;
            }
            ///déplacement
            ///
            /// j'ai fait 3 méthodes différentes car je n'ai pas eu le temps de voir les héritages 
            /// donc obligation de faire une méthode par type de map
            /// 
            if (mapV[0].Actif)
            {
                animation = DeplacementV(mapV[0], walkSpeed);
            }
            else if (mapV[1].Actif)
            {
                animation = DeplacementV(mapV[1], walkSpeed);
            }
            else if (mapF.Actif)
            {
                animation = DeplacementF(mapF, walkSpeed);
            }
            else if (mapB[0].Actif)
            {
                animation = DeplacementB(mapB[0], walkSpeed);
            }
            else if (mapB[1].Actif)
            {
                animation = DeplacementB(mapB[1], walkSpeed);
            }

            return animation;
        }

        //commentaire sur le "public string DeplacementV(MapVillage map, float walkSpeed)"
        //je n'ai pas eu le temps de travailler efficacement les collisions
        public string DeplacementV(MapVillage map, float walkSpeed)
        {
            string animation = animationBase;

            KeyboardState keyboardState = Keyboard.GetState();
            //test des touches directionnelles
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth - 1); //tuile à gauche
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight);
                animation = "walkWest";
                if (!map.IsCollisionHero(tx, ty))//si pas de collision
                    this.PositionHero.X -= walkSpeed;//avance à gauche
                indiceAnimation = 1;
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth + 1); //tuile à droite
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight);
                animation = "walkEast";
                if (!map.IsCollisionHero(tx, ty))//si pas de collision
                    this.PositionHero.X += walkSpeed;//avance à droite
                indiceAnimation = 2;
            }
            else if (keyboardState.IsKeyDown(Keys.Up))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth);//tuile au dessus
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight);
                animation = "walkNorth";
                if (!map.IsCollisionHero(tx, ty))//si pas de collision
                    this.PositionHero.Y -= walkSpeed;//avance en haut

                indiceAnimation = 3;
            }
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth);//tuile en dessous
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight + 1);
                animation = "walkSouth";
                if (!map.IsCollisionHero(tx, ty))//si pas de collision
                    this.PositionHero.Y += walkSpeed;//avance en bas
                indiceAnimation = 0;
            }
            else switch (indiceAnimation)
                {
                    case 0:
                        animation = "idleSouth";
                        break;
                    case 1:
                        animation = "idleWest";
                        break;
                    case 2:
                        animation = "idleEast";
                        break;
                    case 3:
                        animation = "idleNorth";
                        break;
                }
            return animation;
        }
        public string DeplacementF(MapForet map, float walkSpeed)
        {
            string animation = animationBase;

            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth - 1);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight);
                animation = "walkWest";
                if (!map.IsCollisionHero(tx, ty))
                    this.PositionHero.X -= walkSpeed;
                indiceAnimation = 1;
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth + 1);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight);
                animation = "walkEast";
                if (!map.IsCollisionHero(tx, ty))
                    this.PositionHero.X += walkSpeed;
                indiceAnimation = 2;
            }
            else if (keyboardState.IsKeyDown(Keys.Up))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight);
                animation = "walkNorth";
                if (!map.IsCollisionHero(tx, ty))
                    this.PositionHero.Y -= walkSpeed;

                indiceAnimation = 3;
            }
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight + 1);
                animation = "walkSouth";
                if (!map.IsCollisionHero(tx, ty))
                    this.PositionHero.Y += walkSpeed;
                indiceAnimation = 0;
            }
            else switch (indiceAnimation)
                {
                    case 0:
                        animation = "idleSouth";
                        break;
                    case 1:
                        animation = "idleWest";
                        break;
                    case 2:
                        animation = "idleEast";
                        break;
                    case 3:
                        animation = "idleNorth";
                        break;
                }
            return animation;
        }
        public string DeplacementB(MapBoss map, float walkSpeed)
        {
            string animation = animationBase;

            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth - 1);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight);
                animation = "walkWest";
                if (!map.IsCollisionHero(tx, ty))
                    this.PositionHero.X -= walkSpeed;
                indiceAnimation = 1;
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth + 1);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight);
                animation = "walkEast";
                if (!map.IsCollisionHero(tx, ty))
                    this.PositionHero.X += walkSpeed;
                indiceAnimation = 2;
            }
            else if (keyboardState.IsKeyDown(Keys.Up))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight);
                animation = "walkNorth";
                if (!map.IsCollisionHero(tx, ty))
                    this.PositionHero.Y -= walkSpeed;

                indiceAnimation = 3;
            }
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight + 1);
                animation = "walkSouth";
                if (!map.IsCollisionHero(tx, ty))
                    this.PositionHero.Y += walkSpeed;
                indiceAnimation = 0;
            }
            else switch (indiceAnimation)
                {
                    case 0:
                        animation = "idleSouth";
                        break;
                    case 1:
                        animation = "idleWest";
                        break;
                    case 2:
                        animation = "idleEast";
                        break;
                    case 3:
                        animation = "idleNorth";
                        break;
                }
            return animation;
        }

        //test des transition pour voir s'il faut changer de map
        //même problème qu'avec les déplacements, manque de temps pour voir les héritages
        //donc plusieurs méthodes pour la même chose
        public bool TestTransitionV(MapVillage map)
        {
            //pareil que pour les collisions
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth - 1);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight);

                if (map.IsTransition(tx, ty))
                    return true;
                return false;
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth + 1);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight);

                if (map.IsTransition(tx, ty))
                    return true;
                return false;
            }
            else if (keyboardState.IsKeyDown(Keys.Up))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight);

                if (map.IsTransition(tx, ty))
                    return true;
                return false;
            }
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight + 1);

                if (map.IsTransition(tx, ty))
                    return true;
                return false;
            }
            else return false;
        }
        public bool TestTransitionF(MapForet map)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth - 1);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight);

                if (map.IsTransition(tx, ty))
                    return true;
                return false;
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth + 1);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight);

                if (map.IsTransition(tx, ty))
                    return true;
                return false;
            }
            else if (keyboardState.IsKeyDown(Keys.Up))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight);

                if (map.IsTransition(tx, ty))
                    return true;
                return false;
            }
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight + 1);

                if (map.IsTransition(tx, ty))
                    return true;
                return false;
            }
            else return false;
        }
        public bool TestTransitionB(MapBoss map)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth - 1);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight);

                if (map.IsTransition(tx, ty))
                    return true;
                return false;
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth + 1);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight);

                if (map.IsTransition(tx, ty))
                    return true;
                return false;
            }
            else if (keyboardState.IsKeyDown(Keys.Up))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight);

                if (map.IsTransition(tx, ty))
                    return true;
                return false;
            }
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                ushort tx = (ushort)(this.PositionHero.X / map.TileMap.TileWidth);
                ushort ty = (ushort)(this.PositionHero.Y / map.TileMap.TileHeight + 1);

                if (map.IsTransition(tx, ty))
                    return true;
                return false;
            }
            else return false;
        }

    }

}
