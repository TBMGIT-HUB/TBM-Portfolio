using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Boogle
{
    internal class Jeu
    {
        #region Langue
        public string fichier = "";
        public string diminutif = "";
        public string FICHIER
        {
            get { return fichier; }
            set { fichier = value; }
        }
        public string DIMINUTIF
        {
            get { return diminutif; }
            set { diminutif = value; }
        }
        #endregion
        public void Lancer_jeu()
        {
            Console.WriteLine("Bienvenue dans le jeu du Boogle !\n");
            Console.WriteLine("Pour sortir du jeu, taper 'Echap'.\n");

            Console.WriteLine("Entrez le nombre de joueurs :\n");
            TimeSpan timeRemaining;

            int nb_joueurs = TestEntrerInt();
            List<string> liste = new List<string> { "" };
            Joueur[] j = new Joueur[nb_joueurs];
            for (int i = 0; i < nb_joueurs; i++)
            {
                Console.WriteLine("\nVeuillez entrer le nom du joueur " + (i + 1) + " :\n");
                string LeNom = VerifNom();
                j[i] = new Joueur(LeNom, 0, liste);
            }
            Console.WriteLine("\nVeuillez entrer la taille du plateau :\n");
            int taille_plateau = TestEntrerInt();
            string choixLangue;
            do
            {
                Console.WriteLine("Sélection des langues :\n\n\t\tEnglish (taper 'EN') \t\t Français (taper 'FR')");
                choixLangue = Console.ReadLine().ToUpper();

                if (choixLangue == "EN")
                {
                    FICHIER = "MotsPossiblesEN.txt";
                    DIMINUTIF = "EN";
                }
                else if (choixLangue == "FR")
                {
                    FICHIER = "MotsPossiblesFR.txt";
                    DIMINUTIF = "FR";
                }
                else
                {
                    Console.WriteLine("Langue non reconnue. Réessayez !");
                }

            } while (choixLangue != "EN" && choixLangue != "FR");
            Console.Clear();
            Console.WriteLine("\nLe jeu peut commencer !");
            for (int i = 0; i < nb_joueurs; i++)
            {
                if (FICHIER == "MotsPossiblesEN.txt")
                {
                    Plateau p = new Plateau(taille_plateau, FICHIER, DIMINUTIF);
                    Console.WriteLine(p.toString());
                    DateTime endDate = DateTime.Now + TimeSpan.FromMinutes(1);
                    Console.WriteLine("C'est à " + j[i].Nom + " de jouer !");
                    while (true) 
                    {
                        Console.WriteLine("\nTapez le mot à chercher (Tapez 'Echap' pour sortir):");
                        string mot = VerifNom().ToUpper();
                        int compteur = 0;
                        timeRemaining = endDate - DateTime.Now; 
                        if (timeRemaining.TotalSeconds <= 0)
                        {
                            Console.WriteLine("Temps écoulé !");
                            break;
                        }
                        bool res = p.Test_Plateau(mot, ref compteur, j[i]); 
                        if (res) 
                        {
                            j[i].Add_Mot(mot);
                            Console.Clear();
                            Console.WriteLine(p.toString());
                            Console.WriteLine("Bravo !");
                            Console.WriteLine(j[i].toString());
                            Console.WriteLine("C'est à " + j[i].Nom + " de jouer !");
                        }
                        else
                        {
                            
                            Console.Clear();
                            Console.WriteLine(p.toString());
                            Console.WriteLine("\nDommage le mot n'existe pas !");
                            Console.WriteLine(j[i].toString());
                            Console.WriteLine("C'est à " + j[i].Nom + " de jouer !");
                        }
                    }
                }
                else if (FICHIER == "MotsPossiblesFR.txt")
                {
                    Plateau p = new Plateau(taille_plateau, FICHIER, DIMINUTIF);
                    Console.WriteLine(p.toString());
                    DateTime endDate = DateTime.Now + TimeSpan.FromMinutes(1);
                    Console.WriteLine("C'est à " + j[i].Nom + " de jouer !");
                    while (true) 
                    {
                        Console.WriteLine("\nTapez le mot à chercher (Tapez 'Echap' pour sortir):");
                        string mot = VerifNom().ToUpper();
                        int compteur = 0;
                        timeRemaining = endDate - DateTime.Now; 
                        if (timeRemaining.TotalSeconds <= 0)
                        {
                            Console.WriteLine("Temps écoulé !");
                            break;
                        }
                        bool res = p.Test_Plateau(mot, ref compteur, j[i]); 
                        if (res) 
                        {
                            
                            j[i].Add_Mot(mot);
                            Console.Clear();
                            Console.WriteLine(p.toString());
                            Console.WriteLine("Bravo !");
                            Console.WriteLine(j[i].toString());
                            Console.WriteLine("C'est à " + j[i].Nom + " de jouer !");
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine(p.toString());
                            Console.WriteLine("\nDommage le mot n'existe pas !");
                            Console.WriteLine(j[i].toString());
                            Console.WriteLine("C'est à " + j[i].Nom + " de jouer !");
                        }
                    }
                }


            }
            Console.WriteLine("Fin du jeu !");
            Console.WriteLine("Résultats finals :");
            for (int i = 0; i < nb_joueurs; i++)
            {
                Console.WriteLine(j[i].toString());
            }
            List<string> mot_trouves_total = new List<string>();
            for(int i = 0; i < nb_joueurs; i++)
            {
                if (j[i].Mots_trouves.Count > 1)
                {
                    for (int k = 1; k < j[i].Mots_trouves.Count; k++)
                    {
                        mot_trouves_total.Add(j[i].Mots_trouves[k]);
                    }
                }
                else
                {
                    Console.WriteLine("Le nuage de mot n'a pas pu être créé en raison d'un manque de mots trouvés");
                }
            }
            Nuage nuage = new Nuage(mot_trouves_total);
            nuage.GenerateWordCloud("nuage_de_mots.png");
        }

            ///<summary>
            /// mettre fin au jeu sans bug
            /// </summary>
            /// <param name="lecture"></param>
            /// <returns></returns>
        public static bool Sortie(string lecture)
        {
            bool rep = false;
            if (!string.IsNullOrEmpty(lecture) && lecture.ToUpper() == "ECHAP")
            {
                Console.WriteLine("\nVous avez choisi de quitter le jeu. Au Plaisir de vous revoir !");
                rep = true;
            }
            return rep;
        }
        ///<summary>
        /// débogage des val entrées
        /// </summary>
        /// <returns></returns>
        public static int TestEntrerInt()
        {
            int valeurSaisie;

            while (true)
            {
                string lecture = Console.ReadLine();

                if (Sortie(lecture))
                {
                    Environment.Exit(0);
                }
                if (int.TryParse(lecture, out valeurSaisie))
                {
                    
                    if (valeurSaisie > 0)
                    {
                        return valeurSaisie;
                    }
                    else
                    {
                        Console.WriteLine("\nErreur : Vous devez entrer un entier strictement positif. Veuillez réessayer.\n");
                    }
                }
                else
                {
                    Console.WriteLine("\nErreur : Vous devez entrer un entier valide. Veuillez réessayer.\n");
                }
            }
        }
        ///<summary>
        /// vérif de la saisi du nom
        /// </summary>
        /// <returns></returns>
        public static string VerifNom()
        {
            while (true)
            {
                string lecture = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(lecture) && !Sortie(lecture))
                {
                    Console.WriteLine("Erreur Saisie : Le mot ne peut pas être vide. Veuillez réessayer.\n");
                    continue;
                }

                if (ContientChiffres(lecture) && !Sortie(lecture))
                {
                    Console.WriteLine("Erreur Saisie : Le mot ne doit pas contenir de chiffres. Veuillez réessayer.\n");
                    continue;
                }
                if (Sortie(lecture))
                {
                    Environment.Exit(0);
                }
                return lecture;
            }
        }
        public static bool ContientChiffres(string lecture)
        {
            foreach (char caractère in lecture)
            {
                if (char.IsDigit(caractère))
                    return true;
            }
            return false;
        }
    
    }
}
