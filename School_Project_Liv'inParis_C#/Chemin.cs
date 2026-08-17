using System;
using Visual_Studio;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Visual_Studio
{
    /// <summary>
    /// Classe utilitaire pour les algorithmes de plus court chemin
    /// </summary>
    class Chemin
    {

        /// <summary>
        /// Valeur représentant l'infini algorithmique
        /// </summary>
        
        // divisé "l'infini" par 2 évite apparemment les problèmes de out of range etc. j'avoue pour le coup
        //je savais pas que ça aidait, avant que chatGPT me l'apprenne.
        public const int INF = int.MaxValue / 2; 


        /// <summary>
        /// Matrice d'adjacence du graphe
        /// </summary>
        public int[,] graphe;

        /// <summary>
        /// Sommet source pour les calculs
        /// </summary>
        public int sommetDeDépart;


        /// <summary>
        /// Initialise un calculateur de chemin
        /// </summary>
        /// <param name="sommetDeDépart">Index du nœud de départ</param>
        /// <param name="graphe">Matrice de poids carrée</param>
        public Chemin(int sommetDeDépart, int[,] graphe)

        {
            this.graphe = graphe;
            this.sommetDeDépart = sommetDeDépart;

        
        }


        /// <summary>
        /// Algorithme de Dijkstra pour les plus courts chemins
        /// </summary>
        public (int[] distances, int[] predecesseurs) Dijkstra()
        {
            int taille = this.graphe.GetLength(0);

            int[] distances = new int[taille];
            int[] predecesseurs = new int[taille]; 
            bool[] visite = new bool[taille]; // on notera si un noeud est bloqué/visité comme vu en classe

            #region init
            for (int i = 0; i < taille; i++)
            {
                distances[i] = INF; // au départ tout est null on a aucune distance. c'est ce que l'on faisiat en classe
                predecesseurs[i] = -1; // aucun prédécesseur : on initialise
            }
            distances[this.sommetDeDépart] = 0;
            #endregion
            //PriorityQueue est comme un Queue classique sauf que l'on associe à un ID ici son poids comme "ordre de Priorité" 
            // ça nous aide donc a appliquer plus facilement la méthode vue en classe qui était de sélectionner tel ou tel sommet en fonction du poids total qu'il fallait faire
            // pour l'atteindre. Ici, on utilisera le même pricnicpe
            PriorityQueue<int, int> file = new PriorityQueue<int, int>();
            file.Enqueue(this.sommetDeDépart, 0); // ajout du sommet d'où l'on part

            while (file.Count > 0)
            {
                int courant = file.Dequeue();

                if (visite[courant] == true)
                {
                    visite[courant] = true;
                }

                for (int voisin = 0; voisin < taille; voisin++)
                {
                    // On ignore les valeurs INF et les auto-connexions
                    if (voisin != courant && graphe[courant, voisin] < INF)
                    {

                        int nouvelleDistance = distances[courant] + graphe[courant, voisin];

                        if (nouvelleDistance < distances[voisin])
                        {
                            distances[voisin] = nouvelleDistance;
                            predecesseurs[voisin] = courant; // si il existe un voisin on l'ajoute. Dans le cas où il n'y aurait pas de voisin on aurait pas de chemin donc retournerai -1
                            file.Enqueue(voisin, nouvelleDistance);
                        }
                    }
                }
            }

            return (distances, predecesseurs);
        }


        /// <summary>
        /// Reconstruit un chemin à partir des prédécesseurs
        /// </summary>
        /// <param name="predecesseurs">Tableau des prédécesseurs</param>
        /// <param name="destination">Nœud cible</param>
        /// <returns>Liste ordonnée des nœuds du chemin</returns>
        /// <remarks>
        /// Retourne une liste vide si le chemin est impossible
        /// L'ordre est inversé pour présenter du départ à l'arrivée
        /// </remarks>
        public List<int> ReconstruireChemin(int[] predecesseurs, int destination)
        {
            List<int> chemin = new List<int>();

            // Si le sommet de destination n'est pas accessible
            if (predecesseurs[destination] == -1 && destination != this.sommetDeDépart)
            {
                return chemin; // retourne une liste vide
            }

            while (destination != -1)
            {
                chemin.Add(destination);
                destination = predecesseurs[destination];
            }

            chemin.Reverse();
            return chemin;
        }


        /// <summary>
        /// Algorithme de Bellman-Ford pour poids quelconques
        /// </summary>
        /// <returns>
        /// Tuple similaire à Dijkstra
        /// </returns>
        public (int[] distances, int[] predecesseurs) Bellman_Ford()
        {
            int INF = int.MaxValue/2;
            int n = this.graphe.GetLength(0); // Nombre de sommets

            int[] distances = new int[n];
            int[] predecesseurs = new int[n];

            // Initialisation
            for (int i = 0; i < n; i++)
            {
                distances[i] = INF;
                predecesseurs[i] = -1;
            }
            distances[this.sommetDeDépart] = 0;

            // Relaxation des arêtes (n-1 itérations)
            for (int i = 0; i < n - 1; i++)
            {
                for (int u = 0; u < n; u++)
                {
                    for (int v = 0; v < n; v++)
                    {
                        if (this.graphe[u, v] != INF) // Si une arête existe
                        {
                            if (distances[u] != INF && distances[u] + this.graphe[u, v] < distances[v])
                            {
                                distances[v] = distances[u] + this.graphe[u, v];
                                predecesseurs[v] = u;
                            }
                        }
                    }
                }
            }

            // Détection des cycles négatifs
            for (int u = 0; u < n; u++)
            {
                for (int v = 0; v < n; v++)
                {
                    if (this.graphe[u, v] != INF && distances[u] != INF && distances[u] + this.graphe[u, v] < distances[v])
                    {
                        throw new InvalidOperationException("Le graphe contient un cycle négatif !");
                    }
                }
            }

            return (distances, predecesseurs);
        }


        /// <summary>
        /// Algorithme de Floyd-Warshall pour tous les couples
        /// </summary>
        public (int[,] distances, int[,] predecesseurs) Floyd_Warshall()
        {
            int INF = 0;
            int n = this.graphe.GetLength(0);

            // Initialisation des distances et des prédécesseurs
            int[,] distances = new int[n, n];
            int[,] predecesseurs = new int[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    distances[i, j] = this.graphe[i, j];
                    if (this.graphe[i, j] != INF && i != j)
                    {
                        predecesseurs[i, j] = i; // Le prédécesseur de j est i
                    }
                    else
                    {
                        predecesseurs[i, j] = -1; // Pas de prédécesseur
                    }
                }
            }

            // Algorithme Floyd-Warshall
            for (int k = 0; k < n; k++)
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (distances[i, k] != INF && distances[k, j] != INF && distances[i, k] + distances[k, j] < distances[i, j])
                        {
                            distances[i, j] = distances[i, k] + distances[k, j];
                            predecesseurs[i, j] = predecesseurs[k, j];
                        }
                    }
                }
            }

            return (distances, predecesseurs);
        }


        /// <summary>
        /// Reconstruit un chemin spécifique depuis la matrice Floyd-Warshall
        /// </summary>
        /// <param name="predecesseurs">Matrice des prédécesseurs</param>
        /// <param name="depart">Nœud source</param>
        /// <param name="arrivee">Nœud destination</param>
        /// <remarks>
        /// Parcourt la matrice de prédécesseurs en marche arrière
        /// Gère les chemins avec étapes intermédiaires multiples
        /// </remarks>
        public List<int> ReconstruireCheminFloyd(int[,] predecesseurs, int depart, int arrivee)
        {
            List<int> chemin = new List<int>();
            if (predecesseurs[depart, arrivee] == -1)
            {
                return chemin; // Pas de chemin
            }

            int courant = arrivee;
            while (courant != depart)
            {
                chemin.Add(courant);
                courant = predecesseurs[depart, courant];
            }
            chemin.Add(depart);
            chemin.Reverse();

            return chemin;
        }
        
        
    }
}
