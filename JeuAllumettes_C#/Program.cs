namespace JeuAllumettes
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Règles du Jeu : \n");
            Console.WriteLine("\tLe jeu des allumettes se joue à deux. On dispose sur la table d'allumettes les unes à côté des autres.\n\tTour à tour, les deux joueurs doivent prendre entre 1 et 3 allumettes parmi celles disposées sur la table.\n\tCelui qui prend la dernière allumette a perdu.");
            Console.WriteLine("\tEn générale, ce jeu se joue avec vingt allumettes.\n\tCependant, je vous propose d'y jouer avec le nombre d'allumettes de votre choix. Bonne chance ! \n\n");
            JeuDesAllumettes();

        }
        static void JeuDesAllumettes()
        {
            Console.WriteLine("Combien d'allumettes ?");
            int taille = Convert.ToInt32(Console.ReadLine());
            bool[] allumettes = CreerTasAllumette(taille);

            int noJoueur = 0;

            while (FinPartie(allumettes) == false)
            {
                noJoueur = (noJoueur % 2) + 1;
                Console.WriteLine();
                Console.WriteLine();
                Console.Write("C'est au tour de joueur: ");
                Console.Write(noJoueur);
                Console.WriteLine();
                Console.WriteLine();


                AfficherTasAllumettes(allumettes);
                int? p1 = DemanderNombreAllumettesARetirer(3);
                if (p1 == null)
                {
                    Console.WriteLine("Vous avez quitté le jeu.");
                    return; 
                }
                int i = 1;
                while (i <= p1)
                {
                    Console.WriteLine();
                    Console.WriteLine();

                    Console.WriteLine("Saisir la position où prendre l'allumette " + i + " : ");


                    int q1 = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine();
                    Console.WriteLine();
                    bool a = PositionValide(allumettes, q1);
                    while (a == false)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Saisir la position où prendre l'allumette " + i + " : ");
                        q1 = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine();
                        a = PositionValide(allumettes, q1);
                    }
                    RetirerUneAllumette(allumettes, q1);
                    AfficherTasAllumettes(allumettes);
                    int n = NombreAllumettesRestantes(allumettes);
                    Console.WriteLine("Il reste " + n + " allumettes.");

                    if (FinPartie(allumettes) == true)
                    { break; }

                    i++;
                }
            }
            Console.Write("Le joueur ");
            Console.Write(noJoueur);
            if (PartieGagnee(allumettes))
            {
                Console.Write(" a gagné !");
            }
            else
            {
                Console.Write(" a perdu !");
            }
            Console.ReadKey();
        }
        static bool[] CreerTasAllumette(int taille)
        {
            bool[] tasallumettes = new bool[taille];
            for (int i = 0; i < tasallumettes.Length; i++)
            {
                tasallumettes[i] = true;
            }
            return tasallumettes;
        }
        static void AfficherTasAllumettes(bool[] tasallumettes)
        {
            int n = tasallumettes.Length;
            int largeurChiffres = n.ToString().Length; 

            Console.Write(" ");
            for (int i = 0; i < n; i++)
            {
                Console.Write((i + 1).ToString().PadLeft(largeurChiffres) + " ");
            }
            Console.WriteLine();

            int largeurTotale = 1 + n * (largeurChiffres + 1);
            Console.WriteLine(new string('-', largeurTotale));

            Console.Write("|");
            for (int i = 0; i < n; i++)
            {
                string symbole = tasallumettes[i] ? "*" : " ";
                Console.Write(symbole.PadLeft(largeurChiffres) + "|");
            }
            Console.WriteLine();

            Console.WriteLine(new string('-', largeurTotale));
            Console.WriteLine();
        }

        static bool PositionValide(bool[] tasallumettes, int index)
        {
            if (tasallumettes == null || tasallumettes.Length == 0)
            {
                return false;
            }
            else if (0 <= (index - 1) && (index - 1) < tasallumettes.Length)
            {
                return true;
            }
            else { return false; }
        }
        static bool RetirerUneAllumette(bool[] tasallumettes, int index)
        {
            while (tasallumettes[index - 1] == false)
            {
                Console.WriteLine("Aucune allumette. Choisir une autre allumette");
                index = Convert.ToInt32(Console.ReadLine());
            }
            tasallumettes[index - 1] = false;
            if (tasallumettes[index - 1] == true)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        static int NombreAllumettesRestantes(bool[] tasallumettes)
        {
            int n = 0;
            if (tasallumettes == null || tasallumettes.Length == 0)
            {
                return 0;
            }
            for (int i = 0; i < tasallumettes.Length; i++)
            {
                if (tasallumettes[i] == true)
                {
                    n++;
                }
            }
            return n;
        }
        static int? DemanderNombreAllumettesARetirer(int max)
        {
            while (true)
            {
                Console.WriteLine("Saisir un nombre entier entre 1 et " + max + " (ou appuyer sur Entrée pour quitter)");
                string saisie = Console.ReadLine();

                if (string.IsNullOrEmpty(saisie))
                {
                    return null; 
                }

                if (int.TryParse(saisie, out int n) && n >= 1 && n <= max)
                {
                    return n;
                }

                // Saisie invalide (lettre, nombre hors plage, etc.) -> on reboucle simplement
                Console.WriteLine("Saisie invalide, réessayez.");
            }
        }
        static bool PartieGagnee(bool[] tasallumettes)
        {
            if (tasallumettes == null || tasallumettes.Length == 0)
            {
                return false;
            }
            int n = 0;
            for (int i = 0; i < tasallumettes.Length; i++)
            {
                if (tasallumettes[i] == true)
                {
                    n++;
                }
            }
            if (n > 1 || n < 1)
            {
                return false;
            }
            else { return true; }
        }

        static bool FinPartie(bool[] tasallumettes)
        {
            if (tasallumettes == null || tasallumettes.Length == 0)
            {
                return false;
            }
            bool fin = true;
            for (int i = 0; i < tasallumettes.Length; i++)
            {
                if (tasallumettes[i] == false)
                {
                    fin = true;
                }
                else if (tasallumettes[i] == true)
                {
                    fin = false;
                    break;
                }
            }
            return fin;
        }
    }
}
