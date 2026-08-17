using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Visual_Studio;

namespace Visual_Studio
{
    /// <summary>
    /// Classe représentant les liens (arêtes) d'un graphe basé sur une matrice d'adjacence.
    /// </summary>
    class Liens
    {
        
        /// <summary>
        /// Instance de la classe `Graphe` qui stocke la matrice d'adjacence associée.
        /// Permet d'accéder aux informations du graphe et de ses connexions.
        /// </summary>
        public Graphe graphePourLien;
        /// <summary>
        /// Tableau stockant les arêtes du graphe sous forme de paires de nœuds connectés.
        /// Chaque élément est un tableau de deux entiers représentant un lien entre deux nœuds.
        /// </summary>
        public int[][] liens;

        
        public Liens(int sommetDeDépart)
        {
            this.liens = CréationDuTableauDesLiens();
        }

       
        public int[][] CréationDuTableauDesLiens()
        {
            this.liens = new int[this.graphePourLien.nombreDeLienGraphe][];

            int k = 0;
            for (int i = 0; i < graphePourLien.GRAPHE.GetLength(0); i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (graphePourLien.GRAPHE[i, j] == 1)
                    {
                        this.liens[k] = new int[] { i + 1, j + 1 };
                        k++;
                    }
                }
            }
            return this.liens;
        }

        /// <summary>
        /// Affiche la liste des liens (arêtes) du graphe dans la console.
        /// </summary>
        public void AffichageDesLiens()
        {
            Console.WriteLine("\nNombre de liens : " + this.graphePourLien.nombreDeLienGraphe);
            for (int i = 0; i < this.liens.Length; i++)
            {
                Console.Write(this.liens[i][0] + "--" + this.liens[i][1]);
                Console.WriteLine();
            }
        }
    }
}
