namespace JeuLabyrinthe
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] schema1 = Labyrinthe.GenererLabyrinthe(36, 81, 25);

            // Création d'un Labyrinthe
            int nbLignes = schema1.Length;
            int nbColonnes = schema1[0].Length;

            Labyrinthe labyrinthe = new Labyrinthe(schema1, nbLignes, nbColonnes);

            // Création de Personnage
            Personnage personnage = new Personnage(labyrinthe);

            // Boucle du jeu 
            while (true)
            {
                Console.Clear();
                labyrinthe.Afficher(personnage.PositionActuelle);

                if (personnage.EstArrivee())
                {
                    if (personnage.ACle)
                    {
                        break; // victoire
                    }
                    else
                    {
                        Console.WriteLine("La porte est verrouillée ! Trouvez d'abord la clé (c).");
                    }
                }

                personnage.DeplacementSuivant();
            }

            Console.WriteLine("Félicitations ! Vous avez atteint la case d'arrivée.");
        }
    }
}
