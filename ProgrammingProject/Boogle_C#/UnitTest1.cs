using Boogle;
using System;

namespace UnitTest_Boogle
{
    public class UnitTest1
    {
        /// <summary>
        /// "TestUnitaireDesméthodes : notre version de VisualStudio plantait ou ne fonctionnait pas lorsque nous avons essayé de faire de vrais tests unitaires.
        /// Nous avons donc décidé de faire une sorte de classe dans le programme qui joue le même rôle qu'un TestUnitaire."
        /// </summary>
        /// <returns></returns>
        public string TestUnitaireDesMéthodes()
        {
            string rep = "Tous les Tests sont correctes";
            if(!Test_Sortie_Echap())rep = "Un Test ne passe pas";
            if (!Test_ContientChiffres()) rep = "Un Test ne passe pas";
            if (!Test_Contain())rep = "Un Test ne passe pas";
            if (!Test_Add_Mot()) rep = "Un Test ne passe pas";
            if (!Test_Constructeur()) rep = "Un Test ne passe pas";
            if (!Test_ValeurAssociéLettre())rep = "Un Test ne passe pas";
            if (!Test_ValeurMot()) rep = "Un Test ne passe pas";
            if (!Test_CreerEtLireDepuisFichier_Probabilites())rep = "Un Test ne passe pas";
            if (!Test_CreerDeAleatoire()) rep = "Un Test ne passe pas";
            if (!Test_Lance()) rep = "Un Test ne passe pas";
            if (!Test_ToString())rep = "Un Test ne passe pas";
            return rep;
        }

        #region Les Méthodes Testées
        public bool Test_Sortie_Echap()
        {
            // Arrange
            string input = "Echap";

            // Act
            bool result = Jeu.Sortie(input);

            if (!result)
            {
                Console.WriteLine("Problème à Test_Sortie_Echap");
                return false;
            }
            return true;
        }



        public bool Test_ContientChiffres()
        {
            // Arrange
            string input = "Test123";

            // Act
            bool result = Jeu.ContientChiffres(input);

            if (!result)
            {
                Console.WriteLine("Problème à Test_ContientChiffres");
                return false;
            }
            return true;
        }


        public bool Test_Contain()
        {
            // Arrange
            List<string> mots = new List<string> { "mot1", "mot2" };
            Joueur joueur = new Joueur("John", 10, mots);

            // Act
            bool result = joueur.Contain("mot1");

            if (!result)
            {
                Console.WriteLine("Problème à Test_Contain");
                return false;
            }
            return true;
        }

       

        public bool Test_Add_Mot()
        {
            // Arrange
            Joueur joueur = new Joueur("John", 10, new List<string>());

            // Act
            joueur.Add_Mot("nouveauMot");

            if (joueur.Mots_trouves.Count != 1 || joueur.Mots_trouves[0] != "nouveauMot")
            {
                Console.WriteLine("Problème à Test_Add_Mot");
                return false;
            }
            return true;
        }

        public bool Test_Constructeur()
        {
            // Arrange
            var faces = new List<char> { 'A', 'B', 'C','D','E','F' };

            // Act
            var de = new De(faces);

            if (de == null)
            {
                Console.WriteLine("Problème à Test_Constructeur");
                return false;
            }
            return true;
        }

        public bool Test_ValeurAssociéLettre()
        {
            // Arrange
            string cheminFichier = "valeurs.txt";
            File.WriteAllLines(cheminFichier, new[] { "A;1", "B;2", "C;3" });

            // Act
            var result = De.ValeurAssociéLettre(cheminFichier);

            if (result["A"] != 1 || result["B"] != 2 || result["C"] != 3)
            {
                Console.WriteLine("Problème à Test_ValeurAssociéLettre");
                return false;
            }
            return true;
        }

        

        public bool Test_ValeurMot()
        {
            string mot = "ABC";

            // Act
            var result = De.ValeurMot(mot);

            if (result != 7) 
            {
                Console.WriteLine("Problème à Test_ValeurMot");
                return false;
            }
            return true;
        }

        public bool Test_CreerEtLireDepuisFichier_Probabilites()
        {
            // Arrange
            string cheminFichier = "probabilites.txt";
            File.WriteAllLines(cheminFichier, new[] { "A;1;50", "B;2;30", "C;3;20" });

            // Act
            var result = De.CreerEtLireDepuisFichier(cheminFichier);

            if (result["A"] != 0.5 || result["B"] != 0.3 || result["C"] != 0.2)
            {
                Console.WriteLine("Problème à Test_CreerEtLireDepuisFichier_Probabilites");
                return false;
            }
            return true;
        }

        public bool Test_CreerDeAleatoire()
        {
            // Arrange
            var probabilites = new Dictionary<string, double>{{ "A", 0.5 },{ "B", 0.3 },{ "C", 0.2 }};

            // Act
            var de = De.CreerDeAleatoire(probabilites);

            if (de == null)
            {
                Console.WriteLine("Problème à Test_CreerDeAleatoire");
                return false;
            }
            return true;
        }

        public bool Test_Lance()
        {
            // Arrange
            var faces = new List<char> { 'A', 'B', 'C', 'D','E','F' };
            var de = new De(faces);

            // Act
            de.Lance();

            if (!faces.Contains(de.LettreVisible))
            {
                Console.WriteLine("Problème à Test_Lance");
                return false;
            }
            return true;
        }

        public bool Test_ToString()
        {
            // Arrange
            var faces = new List<char> { 'A', 'B', 'C' };
            var de = new De(faces);
            de.Lance();

            // Act
            var description = de.ToString();

            if (!description.Contains("Face Visible : "))
            {
                Console.WriteLine("Problème à Test_ToString");
                return false;
            }
            return true;
        }
        #endregion
    }
}
