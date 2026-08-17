using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boogle
{
    internal class Dictionnaire
    {
        public List<string> mots;
        public string langue;

        #region Constructeur
        public Dictionnaire(string fichier, string langue)
        {
            this.langue = langue;
            mots = new List<string>();/// j'ai pas compris pourquoi en rajoutant ça tout marche (source chat).... à questionner
                                      /// On considère les mots du fichier comme un texte. On divise le texte par ligne.
                                      /// Lire toutes les lignes du fichier
            var lignes = File.ReadAllLines(fichier);

            foreach (var ligne in lignes)
            {
                /// Vérifier si la ligne n'est pas vide ou null
                if (!string.IsNullOrWhiteSpace(ligne))/// sinon
                {
                    /// Utiliser Split pour séparer les mots, en supprimant les espaces multiples pour chaque ligne
                    var motsDeLaLigne = ligne.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    /// Ajouter chaque mot à la liste mots
                    foreach (var mot in motsDeLaLigne)
                    {
                        /// Vérifier que le mot n'est pas null ou vide après l'utilisation de Split
                        if (!string.IsNullOrEmpty(mot))/// sinon
                        {
                            mots.Add(mot);  /// Ajouter chaque mot à la liste
                        }
                    }
                }
            }
            mots = TrieDicoParFusion(mots);
            /// ligne.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) :
            /// Utilise Split pour séparer les mots par des espaces.
            /// StringSplitOptions.RemoveEmptyEntries garantit que les espaces multiples sont ignorés et que les mots vides ne seront pas ajoutés à la liste.
            /// Si la ligne contient plusieurs espaces consécutifs, Split les ignore et découpe correctement les mots.
        }
        #endregion

        #region Méthodes
        public string toString()
        {
            Dictionary<char, int> motsRépartiParLettre = new Dictionary<char, int>();
            Dictionary<int, int> motsRépartiParLongueur = new Dictionary<int, int>();

            for (char lettre = 'a'; lettre <= 'z'; lettre++)
            {
                motsRépartiParLettre[lettre] = 0;
            }

            string resultat = "Langue du dictionnaire : " + langue + "\n"; 

            foreach (string mot in mots)
            {
                int longueurDuMot = mot.Length;

                
                if (motsRépartiParLongueur.ContainsKey(longueurDuMot))
                    motsRépartiParLongueur[longueurDuMot]++;
                else
                    motsRépartiParLongueur[longueurDuMot] = 1;
                

                if (!string.IsNullOrEmpty(mot))
                {
                    char premierelettre = char.ToLower(mot[0]);
                    if (motsRépartiParLettre.ContainsKey(premierelettre))
                    { motsRépartiParLettre[premierelettre]++; }
                    else { motsRépartiParLettre[premierelettre] = 1; }
                }

            }

            resultat += "Nombre total de mots : " + mots.Count + "\n";
            resultat += "\nRépartition des mots par première lettre :\n";
            foreach (var kvp in motsRépartiParLettre.OrderBy(kvp => kvp.Key))
                resultat += " - " + kvp.Key + " : " + kvp.Value + " mot(s)\n";

            resultat += "\nRépartition des mots par longueur :\n";
            foreach (var kvp in motsRépartiParLongueur.OrderBy(kvp => kvp.Key))
            {
                resultat += " - Longueur " + kvp.Key + " : " + kvp.Value + " mot(s)\n";
            }
            return resultat;
        }
        #region Trie
        
        public List<string> TrieDicoParFusion(List<string> ListDeString)//trie le plus rapide !
        {
            
            if (ListDeString == null || mots.Count <= 1)
            {
                return ListDeString;
            }
            
            int milieuList = ListDeString.Count / 2;
            if(milieuList == 0)
            {
                return ListDeString;
            }
            List<string> PartieGauche = ListDeString.GetRange(0, milieuList);
            List<string> PartieDroite = ListDeString.GetRange(milieuList, ListDeString.Count - milieuList);


           return Fusion(TrieDicoParFusion(PartieGauche), TrieDicoParFusion(PartieDroite));
        }

        static List<string> Fusion(List<string> tab1, List<string> tab2)
        {
            List<string> ListComplete = new List<string>();
            int i = 0, j = 0;

            while (i < tab1.Count && j < tab2.Count)
            {
                if (string.Compare(tab1[i], tab2[j]) <=0)
                {
                    ListComplete.Add(tab1[i++]);
                }
                else
                {
                    ListComplete.Add(tab2[j++]);
                }
            }

            while (i < tab1.Count)
            {
                ListComplete.Add(tab1[i++]);
            }

            while (j < tab2.Count)
            {
                ListComplete.Add(tab2[j++]);
            }

            return ListComplete;
        }
        /*
        public List<string> TrieDicoParRapide(List<string> listDeString)
        {
            if (listDeString == null || listDeString.Count <= 1)
            {
                return listDeString; // Rien à trier si la liste est vide ou contient un seul élément.
            }

            QuickSort(listDeString, 0, listDeString.Count - 1); // Appel du tri rapide sur toute la liste.
            return listDeString; // Retourne la liste triée.
        }

        private void QuickSort(List<string> list, int debut, int fin)
        {
            if (debut < fin)
            {
                /// Divise la liste en deux parties en utilisant un pivot
                string pivot = list[fin]; /// On choisit le dernier élément comme pivot
                int i = debut - 1;        /// Indice du plus grand élément inférieur au pivot

                for (int j = debut; j < fin; j++)
                {
                    /// Compare les éléments avec le pivot
                    if (string.Compare(list[j], pivot) <= 0)
                    {
                        i++;
                        string varTemp = list[i];
                        list[i] = list[j];
                        list[j] = varTemp;
                    }
                }

                /// Place le pivot dans sa position correcte
                string tempo = list[i + 1];
                list[i + 1] = list[fin];
                list[fin] = tempo;
                int pivotIndex = i+1;

                QuickSort(list, debut, pivotIndex - 1); /// Partie gauche
                QuickSort(list, pivotIndex + 1, fin);   /// Partie droite
            }
        }
        */
        /*
        public List<string> TrieDicoParTas(List<string> listDeString)
        {
            if (listDeString == null || listDeString.Count <= 1)
            {
                return listDeString; // Pas besoin de trier
            }

            // Construire le tas (max-heap)
            for (int i = listDeString.Count / 2 - 1; i >= 0; i--)
            {
                Entasser(listDeString, listDeString.Count, i);
            }

            // Trier en extrayant les éléments du tas
            for (int i = listDeString.Count - 1; i > 0; i--)
            {
                // Échanger l'élément racine (max) avec le dernier élément
                string temp = listDeString[0];
                listDeString[0] = listDeString[i];
                listDeString[i] = temp;

                // Reconstruire le tas sur les éléments restants
                Entasser(listDeString, i, 0);
            }

            return listDeString;
        }

        private void Entasser(List<string> list, int n, int i)
        {
            int plusGrand = i; // Initialise le plus grand comme étant la racine
            int gauche = 2 * i + 1; // Enfant gauche
            int droite = 2 * i + 2; // Enfant droit

            // Si l'enfant gauche est plus grand que la racine
            if (gauche < n && string.Compare(list[gauche], list[plusGrand]) > 0)
            {
                plusGrand = gauche;
            }

            // Si l'enfant droit est plus grand que le plus grand actuel
            if (droite < n && string.Compare(list[droite], list[plusGrand]) > 0)
            {
                plusGrand = droite;
            }

            // Si le plus grand n'est pas la racine
            if (plusGrand != i)
            {
                string temp = list[i];
                list[i] = list[plusGrand];
                list[plusGrand] = temp;

                // Récursivement, entasser le sous-arbre affecté
                Entasser(list, n, plusGrand);
            }
        }
        */


        #endregion

        /// <summary>
        /// RechDicoRecursif : recherche dichotomique(vue en cours) récursive dans une liste de mots.
        /// On peut savoir ainsi si un mot donné est présent dans un dictionnaire trié dans l'ordre alpahbétique.
        /// </summary>
        /// <param name="mot"></param>
        /// <param name="debut"></param>
        /// <param name="fin"></param>
        /// <returns></returns>

        #region Recherche du Mot
        public bool RechDichoRecursif(string mot, int debut = 0, int fin = -1)
        {
            if (fin == -1)
            {
                fin = mots.Count - 1;
            }
            if (fin < debut)
                return false;

            int milieu = (debut + fin) / 2;


            int comparaison = string.Compare(mots[milieu], mot, StringComparison.OrdinalIgnoreCase);


            if (comparaison == 0)
            {
                return true;
            }

            else if (comparaison > 0)
            {

                return RechDichoRecursif(mot, debut, milieu - 1);
            }

            else
            {
                return RechDichoRecursif(mot, milieu + 1, fin);
            }
        }
        #endregion

        #endregion
    }
}
