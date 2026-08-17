using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boogle
{
    internal class Joueur
    {
        string nom;
        int score;
        List<string> mots_trouves = new List<string>();

        #region Constructeurs
        /// <summary>
        /// "Création du Constructeur 'Joueur' qui permet de définir un Joueur avec son nom, son score et les mots qu'il a trouvé."
        /// </summary>
        /// <param name="nom"></param>
        /// <param name="score"></param>
        /// <param name="mots_trouves"></param>
        public Joueur(string nom, int score, List<string> mots_trouves)
        {
            this.nom = nom;
            this.score = score;
            this.mots_trouves = new List<string>(mots_trouves); 
        }
        #endregion

        #region Propriétés
        public List<string> Mots_trouves
        {
            get { return this.mots_trouves; }
        }
        public string Nom
        {
            get { return this.nom; }
        }
        public int Score
        {
            get { return this.score; }
            set { this.score = value; }
        }
        #endregion

        
        ///<summary> "cherche si un mot à déjà été dit par le joueur" </summary>
        ///<param name= "mot" > "mot recherché" </param>
        /// <returns> " un bouléen " </returns>
        public bool Contain(string mot)
        {
            int compteur = 0;
            for (int i = 0; i < this.mots_trouves.Count; i++)
            {
                if (this.mots_trouves[i] == mot)
                {
                    compteur++;
                }
            }
            if (compteur == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        ///<summary> "ajoute le mot aux mots déjà trouvés" </summary>
        ///ajoute au score, la valeur du mot
        ///<param name= "mot" > "mot à ajouter" </param>
        public void Add_Mot(string mot)
        {
            if (!Contain(mot))
            {
                this.mots_trouves.Add(mot);
                this.score = this.score + De.ValeurMot(mot);
            }
        }
        ///<summary> " Décrit le joueur dans une chaine de caractères" </summary>
        /// <returns> " Le nom du joueur, son score et les mots qu'il a trouvé" </returns>
        public string toString()
        {
            string res_mots = "";
            
            for (int i = 1; i < this.mots_trouves.Count - 1; i++)
            {
                if (Contain(this.mots_trouves[i]))
                {
                    res_mots += this.mots_trouves[i] + ", "; 
                }
            }
            res_mots = res_mots + this.mots_trouves[this.mots_trouves.Count - 1];
            return "Nom : " + this.nom + "\n" + "Score : " + this.score + "\n" + "Mots trouves : " + res_mots;
        }

        
    }
}
