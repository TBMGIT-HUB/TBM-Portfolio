using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Boogle
{
    internal class De
    {
        private List<char> Faces;    
        /// <summary>
        /// "Liste des faces du dé (lettres)."
        /// </summary>
        public char LettreVisible { get; private set; } 
        /// <summary>
        /// "Lettre visible après le lancer."
        /// </summary>
        /// <param name="faces"></param>

        #region Constructeur
        public De(List<char> faces)
        {
            Faces = faces;
        }
        #endregion

        #region Méthodes
        /// <summary>
        /// "ValeurassociéLettre : On récupère depuis le fichier lettre.txt les deux premières colonnes, soit la lettre est sa valeur."
        /// </summary>
        /// <param name="fichier"></param>
        /// <returns></returns>
        public static Dictionary<string, int> ValeurAssociéLettre(string fichier)
        {
            Dictionary<string, int> dicVal = new Dictionary<string, int>();
            foreach(var ligne in File.ReadAllLines(fichier))
            {
                var parties = ligne.Split(';');
                string lettre = parties[0];
                int nmbPoint = int.Parse(parties[1]);
                dicVal[lettre] = nmbPoint;
            }
            return dicVal;
        }
        /// <summary>
        /// "ValeurMot : un mot est composé de lettre. Ainsi, sa valeur est la somme des valeurs des lettres qui le composent.
        /// On récupère dans <param name="dicVal"></param> les valeurs de chaque lettre et on les somme."
        /// </summary>
        /// <param name="mot"></param>
        /// <returns></returns>
        public static int ValeurMot(string mot)
        {
            Dictionary<string, int> dicVal = ValeurAssociéLettre("Lettres.txt");
            int motPoint = 0;
            for (int i = 0; i < mot.Length; i++)
            {
                if (dicVal.TryGetValue(mot[i].ToString(), out int valeur))
                {
                    motPoint += valeur;
                }
                else
                {
                    Console.WriteLine(valeur + " non trouvée, utilisez l'alphabet français ou anglais. ");
                    return 0;
                }
            }
            return motPoint;
        }

        /// <summary>
        /// "Créer et lire les probabilités depuis un fichier."
        /// </summary>
        /// <param name="fichier"></param>
        /// <returns></returns>
        public static Dictionary<string, double> CreerEtLireDepuisFichier(string fichier)

        {
            Dictionary<string, double> probabilites = new Dictionary<string, double>();
            int total = 0;

            foreach (var ligne in File.ReadLines(fichier))
            {
                var parties = ligne.Split(';');
                string lettre = parties[0];
                int nmb = int.Parse(parties[2]);

                total += nmb;

                probabilites[lettre] = nmb;
            }

            foreach (var lettre in probabilites.Keys)
            {
                probabilites[lettre] /= total;
            }

            return probabilites;
        }

        /// <summary>
        /// "Créer un dé aléatoire en fonction des probabilités."
        /// </summary>
        /// <param name="probabilites"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static De CreerDeAleatoire(Dictionary<string, double> probabilites)
        {
            List<char> faces = new List<char>();
            Random random = new Random();

            if (probabilites == null || probabilites.Count == 0)
            {
                throw new ArgumentException("Les probabilités sont vides ou invalides.");
            }

            foreach (var entry in probabilites)
            {
                int occurrences = (int)(entry.Value * 100);

                
                if (occurrences > 0)
                {
                    for (int i = 0; i < occurrences; i++)
                    {
                        faces.Add(entry.Key[0]); 
                    }
                }
            }

            if (faces.Count == 0)
            {
                throw new ArgumentException("Aucune face valide n'a été ajoutée au dé. Vérifiez les probabilités.");
            }

            for (int i = faces.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                char temp = faces[i];
                faces[i] = faces[j];
                faces[j] = temp;
            }

            return new De(faces);  
        }

        /// <summary>
        /// "Lancer le dé et tirer une lettre en fonction des faces."
        /// </summary>
        public void Lance()
        {
            Random random = new Random();
            int index = random.Next(Faces.Count);
            LettreVisible = Faces[index]; 
        }

        /// <summary>
        /// "Description du dé."
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Face Visible : " + LettreVisible;
        }
        #endregion
    }
}
