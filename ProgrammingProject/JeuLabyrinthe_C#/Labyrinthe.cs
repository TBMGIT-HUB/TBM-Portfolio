using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JeuLabyrinthe
{
    internal class Labyrinthe
    {
        #region Attributs
        private int[,] matrice;
        private int nbLigne;
        private int nbColonne;

        private Position depart;
        private Position arrivee;
        private Position cle;
        #endregion

        #region Constructeur
        public Labyrinthe(string[] matlab, int nbLigne, int nbColonne)
        {
            this.nbLigne = nbLigne;
            this.nbColonne = nbColonne;

            bool existenceArDep = ExistenceDepartArrivee(matlab);
            bool existenceMur = ExistenceMur(matlab);
            bool existenceLigneColonne = ExistenceLigneColonne(matlab);

            if (!existenceArDep || !existenceMur || !existenceLigneColonne)
            {
                Console.WriteLine("Erreur ! Problème dans l'écriture du schéma.");
            }
            else
            {
                this.matrice = new int[this.nbLigne, this.nbColonne];
                for (int i = 0; i < nbLigne; i++)
                {
                    for (int k = 0; k < nbColonne; k++)
                    {
                        switch (matlab[i][k])
                        {
                            case '*':
                            case '-':
                            case '|':
                                this.matrice[i, k] = 1; // mur
                                break;
                            case ' ':
                                this.matrice[i, k] = 0; // espace
                                break;
                            case 'd':
                                this.matrice[i, k] = 2; // départ
                                this.depart = new Position(i, k);
                                break;
                            case 'a':
                                this.matrice[i, k] = 3; // arrivée
                                this.arrivee = new Position(i, k);
                                break;
                            case 'c':
                                this.matrice[i, k] = 5; // clé
                                this.cle = new Position(i, k);
                                break;
                        }
                    }
                }
            }
        }
        #endregion

        #region Propriétés
        public int[,] Matrice => matrice;

        public int NBLigne
        {
            get { return nbLigne; }
            set { nbLigne = value; }
        }

        public int NBColonne
        {
            get { return nbColonne; }
            set { nbColonne = value; }
        }

        public Position Depart => depart;
        public Position Arrivee => arrivee;

        public Position Cle => cle;

        #endregion

        #region Méthodes
        public bool EstUnMur(Position pos)
        {
            return matrice[pos.NumLigne, pos.NumColonne] == 1;
        }

        public bool EstOccupee(Position pos)
        {
            return EstUnMur(pos) || matrice[pos.NumLigne, pos.NumColonne] == 4; // 4 correspond au personnage
        }

        public bool MarquerPassage(Position pos)
        {
            return EstUnMur(pos) || EstOccupee(pos) ||
                   matrice[pos.NumLigne - 1, pos.NumColonne] == 4 ||
                   matrice[pos.NumLigne, pos.NumColonne - 1] == 4 ||
                   matrice[pos.NumLigne + 1, pos.NumColonne] == 4 ||
                   matrice[pos.NumLigne, pos.NumColonne + 1] == 4;
        }

        public void Afficher(Position perso)
        {
            for (int i = 0; i < nbLigne; i++)
            {
                for (int j = 0; j < nbColonne; j++)
                {
                    // Affichage du personnage
                    if (perso.NumLigne == i && perso.NumColonne == j)
                    {
                        Console.Write('.');
                    }
                    else
                    {
                        switch (matrice[i, j])
                        {
                            case 1:
                                AfficherMur(i, j);
                                break;
                            case 0:
                                Console.Write(' ');
                                break;
                            case 2:
                                Console.Write('d');
                                break;
                            case 3:
                                Console.Write('a');
                                break;
                            case 5:
                                Console.Write('c');
                                break;
                        }
                    }
                }
                Console.WriteLine();
            }
        }

        public new string ToString()
        {
            for (int i = 0; i < nbLigne; i++)
            {
                for (int j = 0; j < nbColonne; j++)
                {
                    Console.Write(matrice[i, j] == 1 ? '*' : (matrice[i, j] == 0 ? ' ' : '.'));
                }
                Console.WriteLine();
            }
            return string.Empty;
        }
        #endregion

        #region Fonctions de validation
        public bool ExistenceLigneColonne(string[] matlab)
        {
            for (int i = 0; i < matlab.Length; i++)
            {
                if (matlab[i].Length != this.nbColonne)
                {
                    throw new Exception("Les lignes n'ont pas toutes le même nombre de colonnes.");
                }
            }
            return true;
        }

        public bool ExistenceDepartArrivee(string[] matlab)
        {
            int compte = 0;
            for (int i = 0; i < this.nbLigne; i++)
            {
                for (int j = 0; j < this.nbColonne; j++)
                {
                    if (matlab[i][j] == 'd' || matlab[i][j] == 'a')
                    {
                        compte++;
                    }
                }
            }
            return compte == 2;
        }

        public bool ExistenceMur(string[] matlab)
        {
            for (int i = 0; i < matlab.Length; i++)
            {
                for (int j = 0; j < matlab[0].Length; j++)
                {
                    // Vérifier uniquement les bords du labyrinthe
                    if ((i == 0 || i == matlab.Length - 1 || j == 0 || j == matlab[0].Length - 1) && matlab[i][j] != '*')
                    {
                        throw new Exception("Le labyrinthe n'est pas fermé par des murs.");
                    }
                }
            }
            return true;
        }

        static bool CheminExiste(char[,] grille, Position depart, Position arrivee)
        {
            bool[,] visite = new bool[
                grille.GetLength(0),
                grille.GetLength(1)
            ];

            Queue<Position> file = new Queue<Position>();

            file.Enqueue(depart);
            visite[depart.NumLigne, depart.NumColonne] = true;


            int[] dl = { -1, 1, 0, 0 };
            int[] dc = { 0, 0, -1, 1 };


            while (file.Count > 0)
            {
                Position actuelle = file.Dequeue();


                if (actuelle.EstEgale(arrivee))
                    return true;


                for (int i = 0; i < 4; i++)
                {
                    int nl = actuelle.NumLigne + dl[i];
                    int nc = actuelle.NumColonne + dc[i];


                    if (nl >= 0 && nl < grille.GetLength(0) &&
                       nc >= 0 && nc < grille.GetLength(1))
                    {

                        if (!visite[nl, nc] && grille[nl, nc] != '*')
                        {
                            visite[nl, nc] = true;
                            file.Enqueue(new Position(nl, nc));
                        }
                    }
                }
            }

            return false;
        }

        static void AjouterPassages(char[,] grille, int nombre)
        {
            Random rnd = new Random();

            int h = grille.GetLength(0);
            int l = grille.GetLength(1);

            for (int i = 0; i < nombre; i++)
            {
                int x = rnd.Next(2, h - 2);
                int y = rnd.Next(2, l - 2);


                if (grille[x, y] == '*')
                {
                    int voisins = 0;

                    if (grille[x + 1, y] == ' ') voisins++;
                    if (grille[x - 1, y] == ' ') voisins++;
                    if (grille[x, y + 1] == ' ') voisins++;
                    if (grille[x, y - 1] == ' ') voisins++;


                    // Evite de créer une zone bizarre
                    if (voisins == 2)
                    {
                        grille[x, y] = ' ';
                    }
                }
            }
        }

        public bool EstUneCle(Position pos)
        {
            return matrice[pos.NumLigne, pos.NumColonne] == 5;
        }

        public void RamasserCle(Position pos)
        {
            if (EstUneCle(pos))
            {
                matrice[pos.NumLigne, pos.NumColonne] = 0; // la clé disparaît, redevient un espace vide
            }
        }


        public static string[] GenererLabyrinthe(int lignes, int colonnes, int difficulte)
        {
            if (lignes % 2 == 0) lignes++;
            if (colonnes % 2 == 0) colonnes++;

            char[,] grille = new char[lignes, colonnes];

            for (int i = 0; i < lignes; i++)
            {
                for (int j = 0; j < colonnes; j++)
                {
                    grille[i, j] = '*';
                }
            }

            Random rnd = new Random();

            // Directions : haut, bas, gauche, droite
            int[] dL = { -2, 2, 0, 0 };
            int[] dC = { 0, 0, -2, 2 };

            void Creuser(int l, int c)
            {
                grille[l, c] = ' ';

                // Mélange aléatoire des directions
                List<int> dirs = new List<int> { 0, 1, 2, 3 };

                for (int i = 0; i < dirs.Count; i++)
                {
                    int r = rnd.Next(i, dirs.Count);
                    (dirs[i], dirs[r]) = (dirs[r], dirs[i]);
                }

                foreach (int k in dirs)
                {
                    int nl = l + dL[k];
                    int nc = c + dC[k];

                    // Vérifie qu'on reste dans la zone intérieure
                    if (nl > 0 && nl < lignes - 1 &&
                        nc > 0 && nc < colonnes - 1 &&
                        grille[nl, nc] == '*')
                    {
                        // Ouvre le mur intermédiaire
                        grille[l + dL[k] / 2, c + dC[k] / 2] = ' ';

                        // Continue le creusement
                        Creuser(nl, nc);
                    }
                }
            }

            // Départ du creusement
            Creuser(1, 1);
            AjouterPassages(grille, difficulte * 5);

            grille[1, 1] = 'd';

            // Placement de l'arrivée :
            double distanceMax = 0;
            Position meilleure = new Position(1, 1);


            for (int i = 1; i < lignes - 1; i++)
            {
                for (int j = 1; j < colonnes - 1; j++)
                {
                    if (grille[i, j] == ' ')
                    {
                        double distance = Math.Sqrt(
                            Math.Pow(i - 1, 2) +
                            Math.Pow(j - 1, 2)
                        );

                        if (distance > distanceMax)
                        {
                            distanceMax = distance;
                            meilleure = new Position(i, j);
                        }
                    }
                }
            }

            grille[meilleure.NumLigne, meilleure.NumColonne] = 'a';
            Position depart = new Position(1, 1);


            if (!CheminExiste(grille, depart, meilleure))
            {
                return GenererLabyrinthe(lignes, colonnes, difficulte);
            }
            // Placement de la clé (sur une case ouverte, différente du départ et de l'arrivée)
            List<Position> casesOuvertes = new List<Position>();
            for (int i = 1; i < lignes - 1; i++)
            {
                for (int j = 1; j < colonnes - 1; j++)
                {
                    if (grille[i, j] == ' ' && !(i == 1 && j == 1) && !(i == meilleure.NumLigne && j == meilleure.NumColonne))
                    {
                        casesOuvertes.Add(new Position(i, j));
                    }
                }
            }

            if (casesOuvertes.Count > 0)
            {
                Position posCle = casesOuvertes[rnd.Next(casesOuvertes.Count)];
                grille[posCle.NumLigne, posCle.NumColonne] = 'c';
            }

            // Conversion char[,] vers string[]
            string[] resultat = new string[lignes];


            for (int i = 0; i < lignes; i++)
            {
                char[] ligne = new char[colonnes];

                for (int j = 0; j < colonnes; j++)
                {
                    ligne[j] = grille[i, j];
                }

                resultat[i] = new string(ligne);
            }


            return resultat; ;
        }



        private void AfficherMur(int ligne, int colonne)
        {
            bool horizontal = false;
            bool vertical = false;


            // Mur aligné horizontalement
            if (colonne > 0 && colonne < nbColonne - 1 &&
                matrice[ligne, colonne - 1] == 1 &&
                matrice[ligne, colonne + 1] == 1)
            {
                horizontal = true;
            }


            // Mur aligné verticalement
            if (ligne > 0 && ligne < nbLigne - 1 &&
                matrice[ligne - 1, colonne] == 1 &&
                matrice[ligne + 1, colonne] == 1)
            {
                vertical = true;
            }


            if (horizontal)
                Console.Write('-');

            else if (vertical)
                Console.Write('|');

            else
                Console.Write('*');
        }


        #endregion

    }
}
