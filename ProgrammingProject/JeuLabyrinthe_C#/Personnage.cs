using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JeuLabyrinthe
{
    internal class Personnage
    {
        #region Attributs
        private Position posPerso; // Position actuelle du personnage
        private Labyrinthe labyrinthe; // Référence au labyrinthe pour vérifier les déplacements
        private bool aCle = false;
        #endregion

        #region Constructeur
        public Personnage(Labyrinthe laby)
        {
            this.labyrinthe = laby;
            this.posPerso = laby.Depart; // Position initiale du personnage
        }
        #endregion

        #region Propriétés
        public Position PositionActuelle
        {
            get { return posPerso; }
        }

        public bool ACle
        {
            get { return aCle; }
        }
        #endregion

        #region Méthodes
        // Vérifie si le personnage est arrivé à la position finale
        public bool EstArrivee()
        {
            return this.posPerso.EstEgale(labyrinthe.Arrivee);
        }

        // Demande à l'utilisateur de saisir une direction pour déplacer le personnage
        public void DeplacementSuivant()
        {
            if (aCle)
            {
                Console.WriteLine("Clé en poche");
            }
            Console.WriteLine("Saisissez une flèche (gauche/droite/haut/bas) pour déplacer le personnage :");

            ConsoleKeyInfo keyInfo = Console.ReadKey();  // Capture l'entrée de l'utilisateur
            Position nouvellePosition = new Position(posPerso.NumLigne, posPerso.NumColonne);

            switch (keyInfo.Key)
            {
                case ConsoleKey.LeftArrow:
                    nouvellePosition.NumColonne--; // Déplacement à gauche
                    break;
                case ConsoleKey.RightArrow:
                    nouvellePosition.NumColonne++; // Déplacement à droite
                    break;
                case ConsoleKey.UpArrow:
                    nouvellePosition.NumLigne--; // Déplacement vers le haut
                    break;
                case ConsoleKey.DownArrow:
                    nouvellePosition.NumLigne++; // Déplacement vers le bas
                    break;
                default:
                    Console.WriteLine("Entrée invalide. Utilisez les flèches directionnelles.");
                    return;
            }

            // Vérifie si le déplacement est valide (pas de mur ni hors limites)
            if (!labyrinthe.EstUnMur(nouvellePosition) && nouvellePosition.NumLigne >= 0 && nouvellePosition.NumColonne >= 0)
            {
                this.posPerso = nouvellePosition;

                // NOUVEAU : ramassage de la clé
                if (labyrinthe.EstUneCle(posPerso))
                {
                    aCle = true;
                    labyrinthe.RamasserCle(posPerso);
                    Console.WriteLine("Vous avez trouvé la clé !");
                }

                Console.WriteLine($"Personnage déplacé en position : {posPerso.NumLigne}, {posPerso.NumColonne}");
            }
            else
            {
                Console.WriteLine("Déplacement impossible : il y a un mur ou un obstacle !");
            }
        }
        #endregion
    }
}
