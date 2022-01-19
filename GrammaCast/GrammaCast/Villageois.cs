using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using System;


namespace GrammaCast
{
    /*
    Cette classe représente un villageois dans existant dans le jeu.
    */
    public class Villageois
    {
        public MapVillage map;
        string path;
        public SpriteSheet villageoisSprite; // Stocke les sprites associés au villageois
        public Hero perso;
        private int vitesseVillageois;
        private AnimatedSprite asVillageois; // Gère les animations via les sprites définis précédement
        public Timer timerDeplacement; // Temps restant avant de changer d'animation
        
        /*Animation à produire pour le villageois
        Valeurs possibles : idle, walkNorth, walkSouth, walkEast, walkWest*/
        public string animation = "idle";

        /* Représente le type de déplacement voulu pour le villageois
        Valeurs possibles :
        1 : déplacement en Y+ : Sud
        2 : déplacement en X+ : Est
        3 : déplacement en Y- : Nord
        4 : déplacement en X- : Ouest
        n'importe quelle autre valeur : aucun déplacement
        */
        int indice = 0;


        Random rand = new Random();

        public Villageois(Vector2 positionVillageois, string path)
        {
            Path = path;
            PositionVillageois = positionVillageois;
            vitesseVillageois = 80;
            Block = false;
        }

// Charge le Sprite (visuel) du villageois depuis le chemin donné par le constructeur
        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            this.VillageoisSprite = Content.Load<SpriteSheet>(this.Path, new JsonContentLoader());
            this.ASVillageois = new AnimatedSprite(this.VillageoisSprite);
        }

        // Met à jour le villageois
        public void Update(GameTime gameTime)
        {
            float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            string animation;

            if (!this.Block)
            {
                animation = this.Deplacement(gameTime);
            }
            else
            {
                if (this.EstProche())
                {
                    perso.Block = true;
                }
                animation = "idle";
            }


            this.ASVillageois.Play(animation);
            this.ASVillageois.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(this.ASVillageois, this.PositionVillageois);
        }
        public string Path
        {
            get => path;
            private set => path = value;
        }
        public SpriteSheet VillageoisSprite
        {
            get => villageoisSprite;
            private set => villageoisSprite = value;
        }
        public AnimatedSprite ASVillageois
        {
            get => asVillageois;
            private set => asVillageois = value;
        }
        public Vector2 PositionVillageois;

        public int VitesszVillageois
        {
            get => vitesseVillageois;
            private set => vitesseVillageois = value;
        }
        
        /* Permet de bloquer le déplacement du villageois
        cet attribut est actuellement utilisé pour bloquer le villageois s'il est proche du joueur,
        afin de commencer un dialogue (le joueur à un attribut similaire)*/
        public bool Block;

        // Gère le déplacement du villageois et renvoi l'animation à lui donner pour ce déplacement
        private string Deplacement(GameTime gameTime)
        {
            string animation;
            Random rand = new Random();
            float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float walkSpeed = deltaSeconds * this.vitesseVillageois;

            int timeMax = rand.Next(1, 3);
            Vector2 deplacement = new Vector2(0, 0);

            if (timerDeplacement == null || timerDeplacement.AddTick(deltaSeconds) == false)
            {
                indice = rand.Next(1, 5);
                timerDeplacement = new Timer(timeMax);
            }

            // On regarde le déplacement voulu
            switch (indice)
            {
                case 1:
                    deplacement.Y = 2;
                    break;
                case 2:
                    deplacement.X = 2;
                    break;
                case 3:
                    deplacement.Y = -2;
                    break;
                case 4:
                    deplacement.X = -2;
                    break;
                default:
                    break;
            }
            // On détermine l'animation associée à ce déplacement


            //collisions
            if (deplacement.X == 0 && deplacement.Y == -2)
            {
                ushort tx = (ushort)(this.PositionVillageois.X / map.TileMap.TileWidth);
                ushort ty = (ushort)(this.PositionVillageois.Y / map.TileMap.TileHeight - 1);
                if (map.IsCollisionZone(tx, ty))
                    animation = "idle";
                else
                {
                    animation = "walkNorth";
                    this.PositionVillageois.Y -= walkSpeed;
                }
                    
            }
            else if (deplacement.X == -2 && deplacement.Y == 0)
            {
                ushort tx = (ushort)(this.PositionVillageois.X / map.TileMap.TileWidth - 1);
                ushort ty = (ushort)(this.PositionVillageois.Y / map.TileMap.TileHeight);
                if (map.IsCollisionZone(tx, ty))
                    animation = "idle";
                else
                {
                    animation = "walkWest";
                    this.PositionVillageois.X -= walkSpeed;
                }
            }
            else if (deplacement.X == 0 && deplacement.Y == 2)
            {
                ushort tx = (ushort)(this.PositionVillageois.X / map.TileMap.TileWidth);
                ushort ty = (ushort)(this.PositionVillageois.Y / map.TileMap.TileHeight + 1);
                if (map.IsCollisionZone(tx, ty))
                    animation = "idle";
                else
                {
                    animation = "walkSouth";
                    this.PositionVillageois.Y += walkSpeed;
                }
            }
            else if (deplacement.X == 2 && deplacement.Y == 0)
            {
                ushort tx = (ushort)(this.PositionVillageois.X / map.TileMap.TileWidth + 1);
                ushort ty = (ushort)(this.PositionVillageois.Y / map.TileMap.TileHeight);
                if (map.IsCollisionZone(tx, ty))
                    animation = "idle";
                else
                {
                    animation = "walkEast";
                    this.PositionVillageois.X += walkSpeed;
                }
            }
            else animation = "idle";
            return animation;
        }

        // Retourne true si le villagois est proche du joueur
        private bool EstProche()
        {
            float posX = Math.Abs(this.PositionVillageois.X - perso.PositionHero.X);
            float posY = Math.Abs(this.PositionVillageois.Y - perso.PositionHero.Y);
            if (posX <= 50 && posY <= 50)
            {
                return true;
            }
            else return false;
        }
    }
}
