using System;
using Visual_Studio;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Reflection.PortableExecutable;

namespace Visual_Studio
{
    /// <summary>
    /// Représente un graphe pondéré orienté avec matrice d'adjacence
    /// </summary>
    class Graphe
    {
        /// <summary>
        /// Matrice de poids des connexions entre nœuds
        /// </summary>
        public int[,] graphe;


        /// <summary>
        /// Chemin d'accès au fichier source
        /// </summary>
        public string fichierExcel;

        /// <summary>
        /// Dictionnaire de correspondance nom de station -> ID numérique
        /// </summary>
        public Dictionary<string,int> DicoIDNom = new Dictionary<string, int>();

        /// <summary>
        /// Initialise un graphe à partir d'un fichier CSV structuré
        /// </summary>
        /// <param name="fichierExcel">Chemin du fichier source</param>
        public Graphe(string fichierExcel)
        {
            this.fichierExcel = fichierExcel;
            this.graphe = CreationDuGrapheParExcel();
        }


        /// <summary>
        /// Construit la matrice d'adjacence à partir du fichier source
        /// </summary>
        /// <returns>Matrice de poids initialisée</returns>
        public int[,] CreationDuGrapheParExcel()
        {
            int tailleMat = 333;
            int[,] MatriceAdjacence = new int[tailleMat, tailleMat];

            #region init
            for (int i = 0; i < tailleMat; i++)
            {
                for (int j = 0; j < tailleMat; j++)
                {
                    if (i == j)
                    {
                        MatriceAdjacence[i, j] = 0; // Même station -> temps nul
                    }
                    else
                    {
                        MatriceAdjacence[i, j] = Chemin.INF; // Pas de connexion directe // on prend la csnt INF de Chemin
                    }
                }
            }
            #endregion

            using (StreamReader fichierLecture = new StreamReader(this.fichierExcel))// amorçage de la lecture du fichier
            {
                fichierLecture.ReadLine(); // Ignorer l'en-tête

                while (!fichierLecture.EndOfStream)
                {
                    string[] lecture = fichierLecture.ReadLine().Split(';');
                    if (lecture.Length < 6) continue; // Vérifier qu'il y a assez de colonnes


                    if (int.TryParse(lecture[0].Trim(), out int stationId))//si on a bien un id de station et que la conversion en int passe alors on continue
                    {
                        this.DicoIDNom.Add(lecture[1],stationId);

                    }
                    /*
                    if (int.TryParse(lecture[0].Trim(), out int stationId))//si on a bien un id de station et que la conversion en int passe alors on continue
                    {
                        if (!this.DicoIDNom.ContainsKey(lecture[1].Trim())) // Vérifier si la clé existe déjà
                        {
                            this.DicoIDNom.Add(lecture[1].Trim(), stationId);
                        }
                        else
                        {
                            // Vous pouvez choisir de ne rien faire, de mettre à jour l'ID si nécessaire, ou de gérer l'erreur différemment.
                        }
                    }
                    */

                    //si on a bien un id de station précédente et un temps de station prédente et que la conversion en int passe pour les deux alors on continue
                    if (int.TryParse(lecture[2].Trim(), out int StationPré) &&
                        int.TryParse(lecture[4].Trim(), out int tempsPré))
                    {
                        MatriceAdjacence[stationId, StationPré] = tempsPré;
                    }

                    //si on a bien un id de station suivant et un temps de station suivant et que la conversion en int passe pour les deux alors on continue

                    if (int.TryParse(lecture[3].Trim(), out int StationSuiv) &&
                        int.TryParse(lecture[4].Trim(), out int tempsSuiv))
                    {
                        MatriceAdjacence[stationId, StationSuiv] = tempsSuiv;
                    }

                    #region correspondance
                    if (!string.IsNullOrWhiteSpace(lecture[5]))//si la case excel n'est pas vide
                    {
                        string[] ChangementsCorresp = lecture[5].Trim().Split(' ');// on divise la case selon comment nous l'avons au préalable écrite
                        for (int i = 0; i + 1 < ChangementsCorresp.Length; i += 2) // le format est "correspondance temps correspondance2 temps2 ..."
                        {
                            //si on a bien un id de station et un temps de station et que la conversion en int passe pour les deux alors on les ajoute dans le graphe à leur place

                            if (int.TryParse(ChangementsCorresp[i].Trim(), out int StationChangement) &&
                                int.TryParse(ChangementsCorresp[i + 1].Trim(), out int TempsChangement))
                            {
                                MatriceAdjacence[StationChangement, stationId] = TempsChangement;
                                MatriceAdjacence[stationId, StationChangement] = TempsChangement;
                            }
                        }
                    }
                    #endregion
                }
            }
            return MatriceAdjacence;
        }



        /// <summary>
        /// Dictionnaire des noms de stations validé
        /// </summary>
        public Dictionary<string, int> DicoIDNOM
        {
            get
            {
                if (this.graphe == null)
                    throw new InvalidOperationException("Erreur : Graphe non initialisé.");
                return this.DicoIDNom;
            }
        }
            
        /// <summary>
        /// Propriété permettant d'accéder à la matrice d'adjacence du graphe de manière sécurisé.
        /// </summary>
        public int[,] GRAPHE
        {
            get
            {
                if (this.graphe == null)
                    throw new InvalidOperationException("Erreur : Graphe non initialisé.");
                return this.graphe;
            }
        }


        /// <summary>
        /// Compte le nombre d'arêtes actives dans le graphe
        /// </summary>
        public int nombreDeLienGraphe
        {
            get
            {
                int nombreDeLien = 0;
                for (int i = 0; i < this.graphe.GetLength(1); i++)
                {
                    for (int j = 0; j < i; j++)
                    {
                        if (this.graphe[i, j] == 1) nombreDeLien++;
                    }
                }
                return nombreDeLien;
            }
        }


        /// <summary>
        /// Affiche la matrice d'adjacence dans la console avec coloration
        /// </summary>
        public void AfficheGraphe()
        {
            if (this.graphe == null || this.graphe.Length == 0)
            {
                Console.WriteLine("GRAPHE NULL");
            }
            int n = 0;
            for (int i = 0; i < this.graphe.GetLength(0); i++)
            {
                for (int j = 0; j < this.graphe.GetLength(1); j++)
                {
                    if (this.graphe[i, j] == 60)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("INF ");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(this.graphe[i, j] + " ");
                    }
                }
                Console.WriteLine();
                Console.ResetColor();
                n++;
            }

            Console.WriteLine("nombre de ligne : " + n);
        }



        /// <summary>
        /// Calcule le coût total d'un chemin
        /// </summary>
        /// <param name="chemin">Liste ordonnée des nœuds du chemin</param>
        /// <returns>Temps total en secondes</returns>
        public int CalculerPoidsChemin(List<int> chemin)
        {
            int poidsTotal = 0;
            for (int i = 0; i < chemin.Count - 1; i++)
            {
                int stationActuelle = chemin[i];
                int stationSuivante = chemin[i + 1];
                poidsTotal += this.graphe[stationActuelle, stationSuivante];
            }
            return poidsTotal;
        }

        /// <summary>
        /// Trouve le chemin optimal avec étapes intermédiaires (Dijkstra)
        /// </summary>
        /// <param name="depart">ID station de départ</param>
        /// <param name="arrivee">ID station d'arrivée</param>
        /// <param name="etapes">IDs des étapes obligatoires</param>
        /// <returns>Liste ordonnée des nœuds du chemin</returns>
        public List<int> TrouverCheminDjikstra(int depart, int arrivee, params int[] etapes)
        {
            List<int> cheminComplet = new List<int>();
            int dernierPoint = depart;
            cheminComplet.Add(depart);

            // Vérification des étapes intermédiaires. Si non, on skip ce très long "if"
            if (etapes != null && etapes.Length > 0)
            {
                foreach (int etape in etapes)
                {
                    Chemin cheminEtape = new Chemin(dernierPoint, this.graphe);
                    var (distances, predecesseurs) = cheminEtape.Dijkstra();

                    // Vérifier si l'étape est accessible
                    if (predecesseurs[etape] == -1)
                    {
                        throw new Exception("Aucune connexion trouvée entre "+dernierPoint+" et "+etape);
                    }

                    List<int> sousChemin = cheminEtape.ReconstruireChemin(predecesseurs, etape);

                    // si le copte des chemin est null ou négatif c'est qu'il y a un gros problème
                    // on veut aussi éviter les doublons car lors de la reconstructionChemin, on par d'un chemin du genre 1,2,4 pour reprendre sur 4,... 
                    // or si on les sommes ça nous fera 1,2,4,4,... ce que nous voulons pas d'où cette vérif
                    if (cheminComplet.Count > 0 && sousChemin.Count > 0 && cheminComplet.Last() == sousChemin.First())
                    {
                        sousChemin.RemoveAt(0);
                    }

                    cheminComplet.AddRange(sousChemin);
                    dernierPoint = etape;
                }
            }

            // Chemin final vers la destination
            Chemin cheminFinal = new Chemin(dernierPoint, this.graphe);
            var (distancesFinal, predecesseursFinal) = cheminFinal.Dijkstra();

            // Vérifier si la destination est accessible pour une hsitoire de sécurité (on a eu déjà eu des problèmes lors des test)
            if (predecesseursFinal[arrivee] == -1)
            {
                throw new Exception("Aucune connexion trouvée entre "+dernierPoint+" et "+arrivee);
            }

            List<int> sousCheminFinal = cheminFinal.ReconstruireChemin(predecesseursFinal, arrivee);

            if (cheminComplet.Count > 0 && sousCheminFinal.Count > 0 && cheminComplet.Last() == sousCheminFinal.First())
            {
                sousCheminFinal.RemoveAt(0);
            }

            cheminComplet.AddRange(sousCheminFinal);

            return cheminComplet; //iic on procède la même manière qu'au dessus
        }


        /// <summary>
        /// Algorithme de Bellman-Ford pour chemins avec poids négatifs
        /// </summary>
        /// <param name="depart">ID station de départ</param>
        /// <param name="arrivee">ID station d'arrivée</param>
        /// <returns>Chemin le plus court trouvé</returns>
        public List<int> TrouverCheminBellmanFord(int depart, int arrivee)
        {
            Chemin chemin = new Chemin(depart, this.graphe);
            var (distances, predecesseurs) = chemin.Bellman_Ford();
            return chemin.ReconstruireChemin(predecesseurs, arrivee);
        }


        /// <summary>
        /// Algorithme Floyd-Warshall pour tous les plus courts chemins
        /// </summary>
        /// <param name="depart">ID station de départ</param>
        /// <param name="arrivee">ID station d'arrivée</param>
        /// <returns>Chemin le plus court entre deux nœuds</returns>
        public List<int> TrouverCheminFloydWarshall(int depart, int arrivee)
        {
            Chemin chemin = new Chemin(depart, this.graphe);
            var (distances, predecesseurs) = chemin.Floyd_Warshall();
            return chemin.ReconstruireCheminFloyd(predecesseurs, depart, arrivee);
        }

    }
}