using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace GrammaCast
{
    class Dialogue
    {
        public Hero perso;
        public Villageois villageois;
        public Boss golem;
        public MapBoss[] map;
        public Timer timerEntree;

        //chemin et tableau de dialogue
        public static string[] villageoisPath = new string[]
        {"dialogV1", "dialogV2", "dialogV3", "dialogV4"};
        Texture2D[] villageoisDialog = new Texture2D[villageoisPath.Length];

        public static string[] heroPath = new string[]
        {"dialogHero"};
        Texture2D[] heroDialog = new Texture2D[heroPath.Length];

        public static string[] bossPath = new string[]
        {"dialogboss","dialogboss2","dialogboss3"};
        Texture2D[] bossDialog = new Texture2D[bossPath.Length];

        public static string[] bossFinalPath = new string[]
        {"dialogboss4","dialogboss5","dialogboss6","dialogboss7" };
        Texture2D[] bossFinalDialog = new Texture2D[bossFinalPath.Length];

        /*public static string[] villageoisFinalPath = new string[]
        { };
        Texture2D[] villageoisFinalDialog = new Texture2D[villageoisFinalPath.Length];*/

        // manque de temps pour l'implémentation

        int indice = 0;
        Texture2D dialogue;
        int maxIndice;

        public Dialogue()
        {
            Actif = false;
            Touche = false;
        }
        // Charge le Sprite (visuel) des dialogues
        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            for (int i = 0; i < villageoisDialog.Length; i++)
            {
                villageoisDialog[i] = Content.Load<Texture2D>(villageoisPath[i]);
            }
            for (int i = 0; i < heroDialog.Length; i++)
            {
                heroDialog[i] = Content.Load<Texture2D>(heroPath[i]);
            }
            for (int i = 0; i < bossDialog.Length; i++)
            {
                bossDialog[i] = Content.Load<Texture2D>(bossPath[i]);
            }
            for (int i = 0; i < bossFinalDialog.Length; i++)
            {
                bossFinalDialog[i] = Content.Load<Texture2D>(bossFinalPath[i]);
            }
        }
        public void Update(GameTime gameTime)
        {
            float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState keyboardState = Keyboard.GetState();

            //tests des différentes situations pour afficher les dialogues
            if (perso.Block && villageois.Block)
            {

                dialogue = villageoisDialog[indice];
                maxIndice = villageoisDialog.Length - 1;
                this.Actif = true;
                this.Touche = false;
            }
            if (map[0].Actif && perso.Block)
            {
                dialogue = heroDialog[indice];
                maxIndice = heroDialog.Length - 1;
                this.Actif = true;
                this.Touche = false;
            }
            if (map[1].Actif && !golem.Dead)
            {
                dialogue = bossDialog[indice];
                maxIndice = bossDialog.Length - 1;
                this.Actif = true;
                this.Touche = false;
            }
            if (golem.Dead && perso.Block && map[1].Actif)
            {
                dialogue = bossFinalDialog[indice];
                maxIndice = bossFinalDialog.Length - 1;
                this.Actif = true;
                this.Touche = false;
            }

            //si le dialogue est actif
            if (this.Actif)
            {
                //fait un timer pour éviter que les dialogues soient passé d'un coup, 
                //ça permet de laisser un temps entre chaque phrase
                if (timerEntree == null)
                {
                    timerEntree = new Timer(2f);
                }
                if (timerEntree.AddTick(deltaSeconds) == false)
                {
                    if (keyboardState.IsKeyDown(Keys.Enter))
                    {
                        this.Touche = true;
                        timerEntree = new Timer(2);
                    }
                }

                //si la touche entrée est appyée, Touche est true
                if (this.Touche)
                {
                    //permet de passer à la phrase suivante sauf si le dialogue est fini
                    if (indice == maxIndice)
                    {
                        if (map[1].Actif) //le premier dialogue du boss le débloque pour commencer le combat
                        {
                            golem.Block = false;
                            
                        }
                        if (golem.Dead) // le dernier dialogue va rendre inactif le boss
                        {
                            golem.Actif = false;
                            golem.Block = false;
                        }
                        perso.Block = false;
                        villageois.Block = false;
                        this.Actif = false;
                        indice = 0; //revient au départ à chaque fois pour éviter les out of range
                    }

                    else
                        indice++;
                    this.Touche = false;

                    timerEntree.AddTick(deltaSeconds);
                }
            }

        }
        public void Draw(GameTime gameTime, SpriteBatch _spriteBatch, int windowWidth, int windowHeight)
        {
            if (this.Actif)
                _spriteBatch.Draw(dialogue, new Rectangle(0, windowWidth - 200, windowWidth, 200), Color.White);
        }
        public bool Actif;
        public bool Touche;
    }

}

