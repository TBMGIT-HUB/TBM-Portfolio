using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boogle
{
    internal class Plateau
    {
        De[,] matrice;
        char[,] valeursup;
        int taille; 
        string Langue = "";
        string Diminutif = "";

        #region Constructeur
        public Plateau(int taille, string FichierLangue, string DiminutifDeLaLangue) 
        {
            this.Langue = FichierLangue;
            this.Diminutif = DiminutifDeLaLangue;
            this.taille = taille;
            this.matrice = new De[taille, taille]; 
            this.valeursup = new char[taille, taille];
            for (int i = 0; i < this.taille; i++)
            {
                for (int j = 0; j < this.taille; j++)
                {
                    De monDeij = De.CreerDeAleatoire(De.CreerEtLireDepuisFichier("Lettres.txt"));
                    monDeij.Lance(); 
                    this.matrice[i, j] = monDeij;
                    this.valeursup[i, j] = monDeij.LettreVisible;
                }
            }
        }
        #endregion
        #region Propriétés
        public int Taille
        {
            get { return taille; }
        }
        public char[,] Valeursup
        {
            get { return valeursup; }
        }
        #endregion
        #region Méthodes
        /// <summary>
        /// "toString : 
        /// </summary>
        /// <returns></returns>
        public string toString()
        {
            string res = "";
            for (int i = 0; i < this.taille; i++)
            {
                for (int j = 0; j < this.taille; j++)
                {
                    res = res + this.valeursup[i, j] + " ";
                }
                res = res + " \n";
            }
            return res;
        }
        ///<summary> "Test si le mot entré par l'utilisateur est dans le plateau et est correct" </summary>
        ///<param name= "mot" > "mot recherché" </param>
        ///<param name= "compteur_lettres_ok" > "compteur s'incrémentant à chaque nouvelle lettre du mot recherché validé" </param>
        ///<param name= "index" > "index de la lettre en cours de vérification du mot recherché" </param>
        ///<param name= "l_n_1" > "ligne de la  lettre précedemment verifiée" </param>
        ///<param name= "c_n_1" > "colonne de la  lettre précedemment verifiée" </param>
        /// <returns> " un bouléen " </returns>
        public bool Test_Plateau(string mot, ref int compteur_lettres_ok, Joueur j, bool[,] matrice_memo_lettres_utilisées = null, int index = 0, int l_n_1 = 0, int c_n_1 = 0)
        {
            
            Dictionnaire dicoFR = new Dictionnaire(Langue,Diminutif);
            if (matrice_memo_lettres_utilisées == null)
            {
                matrice_memo_lettres_utilisées = new bool[this.taille, this.taille];
            }
            bool flag = false;
            if (index == 0)
            {
                List<int[]> liste_coordonnees_lettre_1 = new List<int[]>();
                for (int i = 0; i < this.taille; i++)
                {
                    for (int k = 0; k < this.taille; k++)
                    {
                        if (this.valeursup[i, k] == mot[0])
                        {
                            liste_coordonnees_lettre_1.Add(new int[] { i, k });
                        }
                    }
                }
                if (liste_coordonnees_lettre_1.Count > 0)
                {
                    compteur_lettres_ok += 1;
                    for (int i = 0; i < liste_coordonnees_lettre_1.Count; i++)
                    {
                        matrice_memo_lettres_utilisées[liste_coordonnees_lettre_1[i][0], liste_coordonnees_lettre_1[i][1]] = true; 
                        Test_Plateau(mot, ref compteur_lettres_ok, j, matrice_memo_lettres_utilisées, 1, liste_coordonnees_lettre_1[i][0], liste_coordonnees_lettre_1[i][1]);
                    }
                }
            }
            for (int i = 0; i < j.Mots_trouves.Count; i++)
            {
                if (j.Mots_trouves[i] == mot)
                {
                    flag = true;
                }

            }
            if (compteur_lettres_ok == mot.Length && dicoFR.RechDichoRecursif(mot) && flag == false)
            {
                return true; 
            }
            else
            {
                if (index < mot.Length)
                {
                    if (l_n_1 > 0 && c_n_1 > 0 && this.valeursup[l_n_1 - 1, c_n_1 - 1] == mot[index] && matrice_memo_lettres_utilisées[l_n_1 - 1, c_n_1 - 1] == false)
                    {
                        compteur_lettres_ok += 1;
                        matrice_memo_lettres_utilisées[l_n_1 - 1, c_n_1 - 1] = true;
                        return Test_Plateau(mot, ref compteur_lettres_ok, j, matrice_memo_lettres_utilisées, index + 1, l_n_1 - 1, c_n_1 - 1); 
                    }
                    else if (l_n_1 > 0 && this.valeursup[l_n_1 - 1, c_n_1] == mot[index] && matrice_memo_lettres_utilisées[l_n_1 - 1, c_n_1] == false)
                    {
                        compteur_lettres_ok += 1;
                        matrice_memo_lettres_utilisées[l_n_1 - 1, c_n_1] = true;
                        return Test_Plateau(mot, ref compteur_lettres_ok, j, matrice_memo_lettres_utilisées, index + 1, l_n_1 - 1, c_n_1); 
                    }
                    else if (l_n_1 > 0 && c_n_1 < this.valeursup.GetLength(1) - 1 && this.valeursup[l_n_1 - 1, c_n_1 + 1] == mot[index] && matrice_memo_lettres_utilisées[l_n_1 - 1, c_n_1 + 1] == false)
                    {
                        compteur_lettres_ok += 1;
                        matrice_memo_lettres_utilisées[l_n_1 - 1, c_n_1 + 1] = true;
                        return Test_Plateau(mot, ref compteur_lettres_ok, j, matrice_memo_lettres_utilisées, index + 1, l_n_1 - 1, c_n_1 + 1);
                    }
                    else if (c_n_1 > 0 && this.valeursup[l_n_1, c_n_1 - 1] == mot[index] && matrice_memo_lettres_utilisées[l_n_1, c_n_1 - 1] == false)
                    {
                        compteur_lettres_ok += 1;
                        matrice_memo_lettres_utilisées[l_n_1, c_n_1 - 1] = true;
                        return Test_Plateau(mot, ref compteur_lettres_ok, j, matrice_memo_lettres_utilisées, index + 1, l_n_1, c_n_1 - 1); 
                    }
                    else if (c_n_1 < this.valeursup.GetLength(1) - 1 && this.valeursup[l_n_1, c_n_1 + 1] == mot[index] && matrice_memo_lettres_utilisées[l_n_1, c_n_1 + 1] == false)
                    {
                        compteur_lettres_ok += 1;
                        matrice_memo_lettres_utilisées[l_n_1, c_n_1 + 1] = true;
                        return Test_Plateau(mot, ref compteur_lettres_ok, j, matrice_memo_lettres_utilisées, index + 1, l_n_1, c_n_1 + 1); 
                    }
                    else if (l_n_1 < this.valeursup.GetLength(0) - 1 && c_n_1 > 0 && this.valeursup[l_n_1 + 1, c_n_1 - 1] == mot[index] && matrice_memo_lettres_utilisées[l_n_1 + 1, c_n_1 - 1] == false)
                    {
                        compteur_lettres_ok += 1;
                        matrice_memo_lettres_utilisées[l_n_1 + 1, c_n_1 - 1] = true;
                        return Test_Plateau(mot, ref compteur_lettres_ok, j, matrice_memo_lettres_utilisées, index + 1, l_n_1 + 1, c_n_1 - 1);
                    }
                    else if (l_n_1 < this.valeursup.GetLength(0) - 1 && this.valeursup[l_n_1 + 1, c_n_1] == mot[index] && matrice_memo_lettres_utilisées[l_n_1 + 1, c_n_1] == false)
                    {
                        compteur_lettres_ok += 1;
                        matrice_memo_lettres_utilisées[l_n_1 + 1, c_n_1] = true;
                        return Test_Plateau(mot, ref compteur_lettres_ok, j, matrice_memo_lettres_utilisées, index + 1, l_n_1 + 1, c_n_1); 
                    }
                    else if (l_n_1 < this.valeursup.GetLength(0) - 1 && c_n_1 < this.valeursup.GetLength(1) - 1 && this.valeursup[l_n_1 + 1, c_n_1 + 1] == mot[index] && matrice_memo_lettres_utilisées[l_n_1 + 1, c_n_1 + 1] == false)
                    {
                        compteur_lettres_ok += 1;
                        matrice_memo_lettres_utilisées[l_n_1 + 1, c_n_1 + 1] = true;
                        return Test_Plateau(mot, ref compteur_lettres_ok, j, matrice_memo_lettres_utilisées, index + 1, l_n_1 + 1, c_n_1 + 1);
                    }
                }
                return false;
            }
        }
    }
        
        #endregion
    
}
