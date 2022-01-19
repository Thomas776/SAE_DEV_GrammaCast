using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using System;


namespace GrammaCast
{
    /* Représente une attaque du boss. Une attaque est composée d'un mot à écrire,
    en utilisant la classe Attaque (donc lettre par lettre) */
    public class AttaqueBoss
    { 
        public static string[] spriteChemin = new string[] { "IceCastSprite.sf",
            "FireCastSprite.sf", "HolyExplosionSprite.sf", "IceShatterSprite.sf", "PoisonCastSprite.sf"};
        private string[] spell = new string[] { "FEUGLACIAL", "FEUENSORCELE", "EXPLOSIONDIVINE", "ECLATDEGLACE", "BRULUREDEPOISON" };
        private SpriteSheet[] attaqueSprite = new SpriteSheet[spriteChemin.Length];
        public Hero perso;
        public Boss golem;
        private string fontPath;


        private SpriteFont attaqueFont;
        private AnimatedSprite asAttackBoss;
        private string attaqueSpell;
        Timer timerAnimation;
        Timer timerAttaque;
        Timer timerProchaine;
        Random rand = new Random();
        public float point = 4000;
        int attack;
        public int indiceAttack = 0;
        char[] spellcast;

        public AttaqueBoss()
        {
            FontPath = "font";
            Actif = false;
            Final = false;
            Animation = false;
            attack = rand.Next(spell.Length);
            attaqueSpell = this.spell[attack];

        }
        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            this.AttaqueFont = Content.Load<SpriteFont>(this.FontPath);
            for (int i = 0; i < attaqueSprite.Length; i++)
            {
                attaqueSprite[i] = Content.Load<SpriteSheet>(spriteChemin[i], new JsonContentLoader());
            }
            this.AsAttackBoss = new AnimatedSprite(attaqueSprite[attack]);
        }
        public void Update(GameTime gameTime, float windowWidth, float windowHeight)
        {
            float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            this.PositionAttaque = new Vector2(perso.PositionHero.X, perso.PositionHero.Y + 25);
            if (golem.Actif)
            {
                this.ProchaineAttaque(gameTime);
            }

            if (this.Actif)
            {
                if (timerAttaque == null)
                {
                    timerAttaque = new Timer(1000);
                }
                else
                    timerAttaque.AddTick(deltaSeconds);
                spellcast = this.AttaqueSpell.ToCharArray();
                
                if (this.Final)
                {                    
                    this.AsAttackBoss.Play("attack");

                    if (timerAnimation.AddTick(deltaSeconds) == false)
                    {
                        // Charge la prochaine attaque
                        golem.hp -= point / timerAttaque.Tick;

                        timerAttaque = null;
                        this.Final = false;
                        this.Animation = false;
                        this.Actif = false;
                        timerProchaine = null;
                        attack = rand.Next(spell.Length);
                        this.AttaqueSpell = this.spell[attack];
                        this.AsAttackBoss = new AnimatedSprite(attaqueSprite[attack]);
                        indiceAttack = 0;
                    }
                }
                else
                {
                    if (indiceAttack < spellcast.Length)
                    this.GetLetter(spellcast[indiceAttack]);
                }
            }
            this.AsAttackBoss.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch _spriteBatch)
        {
            if (this.Animation)
            {
                _spriteBatch.Draw(this.AsAttackBoss, new Vector2(perso.PositionHero.X, perso.PositionHero.Y));
            }
            else
                _spriteBatch.DrawString(this.AttaqueFont, $"{this.spellcast[indiceAttack]}", this.PositionAttaque, Color.White);
        }
        public string FontPath
        {
            get => fontPath;
            private set => fontPath = value;
        }

        public SpriteFont AttaqueFont
        {
            get => attaqueFont;
            private set => attaqueFont = value;
        }
        public AnimatedSprite AsAttackBoss
        {
            get => asAttackBoss;
            private set => asAttackBoss = value;
        }
        public bool Actif;
        public bool Final;
        public bool Animation;
        public Vector2 PositionAttaque;
        public Vector2 PositionPoint;
        public string AttaqueSpell
        {
            get => attaqueSpell;
            private set => attaqueSpell = value;
        }
        public void GetLetter(char lettre)
        {
            string letter = lettre.ToString();
            var keyboardState = Keyboard.GetState();
            var keys = keyboardState.GetPressedKeys();
            foreach (var key in keys)
            {
                if (key.ToString() == letter)
                {
                    timerAnimation = new Timer(0.4f);
                    indiceAttack++;
                    if (indiceAttack == spellcast.Length)
                    {
                        this.Final = true;
                        this.Animation = true;
                    }

                }
            }
        }      
        public void ProchaineAttaque(GameTime gt)
        {
            float deltaSeconds = (float)gt.ElapsedGameTime.TotalSeconds;
            if (!this.Actif)
            {
                if (timerProchaine == null || timerProchaine.AddTick(deltaSeconds) == false)
                {
                    timerProchaine = new Timer(rand.Next(2, 10));
                }
                else if (timerProchaine.AddTick(deltaSeconds) == false)
                {
                    this.Actif = true;
                }
            }
        }
    }
}
