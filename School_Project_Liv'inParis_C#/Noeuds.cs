using System;
using Visual_Studio;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.PortableExecutable;

namespace Visual_Studio
{
    /// <summary>
    /// Représente un graphe non orienté avec ses nœuds et algorithmes associés
    /// </summary>
    internal class Noeuds
    {
        /// <summary>
        /// Liste d'adjacence représentant les connexions du graphe
        /// </summary>
        public Dictionary<int, List<int>> noeuds = new Dictionary<int, List<int>>();

        public string Fichier = "";
        /// <summary>
        /// Initialise un graphe à partir d'un fichier texte
        /// </summary>
        /// <param name="fichier">Chemin d'accès au fichier de données</param>
        public Noeuds(string fichier)
        {
            this.Fichier = fichier;
            this.noeuds = CreationDeLaListeDeNoeudsParFichier(this.Fichier);
        }

        /// <summary>
        /// Construit la liste d'adjacence à partir d'un fichier texte
        /// </summary>
        /// <param name="fichier">Chemin du fichier source</param>
        /// <returns>Dictionnaire représentant le graphe</returns>
        public static Dictionary<int, List<int>> CreationDeLaListeDeNoeudsParFichier(string fichier)
        {
            Dictionary<int, List<int>> noeuds = new Dictionary<int, List<int>>();
            using (StreamReader fichierLecture = new StreamReader(fichier))// amorçage de la lecture du fichier
            {
                fichierLecture.ReadLine(); // Ignorer l'en-tête

                while (!fichierLecture.EndOfStream)
                {
                    string[] lecture = fichierLecture.ReadLine().Split(';');
                    if (lecture.Length < 6) continue; // Vérifier qu'il y a assez de colonnes
                    if (int.TryParse(lecture[0].Trim(), out int stationId))
                    {
                        if (!noeuds.ContainsKey(stationId)) noeuds[stationId] = new List<int>();
                    }
                    //si on a bien un id de station précédente et un temps de station prédente et que la conversion en int passe pour les deux alors on continue
                    if (int.TryParse(lecture[2].Trim(), out int StationPré) &&
                    int.TryParse(lecture[4].Trim(), out int tempsPré))
                    {
                        noeuds[stationId].Add(StationPré);
                    }

                    //si on a bien un id de station suivant et un temps de station suivant et que la conversion en int passe pour les deux alors on continue

                    if (int.TryParse(lecture[3].Trim(), out int StationSuiv) &&
                        int.TryParse(lecture[4].Trim(), out int tempsSuiv))
                    {
                        noeuds[stationId].Add(StationSuiv);
                    }
                    if (!string.IsNullOrWhiteSpace(lecture[5]))//si la case excel n'est pas vide
                    {
                        string[] ChangementsCorresp = lecture[5].Trim().Split(' ');// on divise la case selon comment nous l'avons au préalable écrite
                        for (int i = 0; i + 1 < ChangementsCorresp.Length; i += 2) // le format est "correspondance temps correspondance2 temps2 ..."
                        {
                            //si on a bien un id de station et un temps de station et que la conversion en int passe pour les deux alors on les ajoute dans le graphe à leur place

                            if (int.TryParse(ChangementsCorresp[i].Trim(), out int StationChangement) &&
                                int.TryParse(ChangementsCorresp[i + 1].Trim(), out int TempsChangement))
                            {
                                if (!noeuds.ContainsKey(StationChangement))
                                {
                                    noeuds[StationChangement] = new List<int>();

                                    noeuds[StationChangement].Add(stationId);
                                    noeuds[stationId].Add(StationChangement);

                                }
                            }
                        }
                    }
                }
                return noeuds;
            }
        }

        /// <summary>
        /// Implémentation de l'algorithme de Welsh-Powell pour le coloriage de graphe
        /// </summary>
        /// <returns>Dictionnaire des couleurs attribuées à chaque sommet</returns>
        public Dictionary<int, int> Welsh_Powell()
        {
            int degréSommet = 0;
            PriorityQueue<int, int> QueueSommets = new PriorityQueue<int, int>();
            // selon learn.microsoft.com nous pouvons récupérer les sommets et leur liste de liaison 
            // via une variable qui récupère la clé et la valeur d'un dictionary.
            foreach (KeyValuePair<int, List<int>> LiaisonsSommet in this.noeuds)
            {
                degréSommet = LiaisonsSommet.Value.Count();
                QueueSommets.Enqueue(LiaisonsSommet.Key, -degréSommet);
            }


            Dictionary<int, int> couleurSommet = new Dictionary<int, int>();
            int couleurCourante = 0;


            while (QueueSommets.Count > 0)
            {
                couleurCourante++;

                // une queue quadn elle se vide ne permet pas de garder les valeurs. pour pouvoir travailler sur celle-ci on les mets dans une liste
                List<int> listeSommets2 = new List<int>();
                while (QueueSommets.Count > 0)
                {
                    listeSommets2.Add(QueueSommets.Dequeue());
                }

                foreach (int sommet in listeSommets2)
                {
                    if (couleurSommet.ContainsKey(sommet))
                        continue; // déjà colorié

                    // Vérifier si un voisin a la même couleur
                    bool peutColorier = true;
                    foreach (int voisin in this.noeuds[sommet])
                    {
                        if (couleurSommet.TryGetValue(voisin, out int couleurVoisin) && couleurVoisin == couleurCourante)
                        //vérif si un voisin ou plus a la couleur actuelle
                        {
                            peutColorier = false;
                            break;
                        }
                    }

                    if (peutColorier)
                    {
                        couleurSommet[sommet] = couleurCourante;
                    }
                }
                // on vérif si les sommets suivants ont une couleur si non on les rajoute dans la queue pour la prochaine itération de couleur
                foreach (int sommet in listeSommets2)
                {
                    if (!couleurSommet.ContainsKey(sommet))
                    {
                        int degre = this.noeuds[sommet].Count;
                        QueueSommets.Enqueue(sommet, -degre);
                    }
                }
            }
            Console.WriteLine("le graphe est " + couleurCourante + "-coloriable.");
            return couleurSommet;
        }

        public Dictionary<int, int> Chu_Liu_Edmonds(int racine)
        {
            // Création d'une copie du graphe pour travailler sans affecter l'original
            var grapheTravail = new Dictionary<int, List<int>>();
            var poids = new Dictionary<(int, int), double>();

            // Initialisation avec des poids par défaut (1.0)
            foreach (var kvp in noeuds)
            {
                grapheTravail[kvp.Key] = new List<int>(kvp.Value);
                foreach (int dest in kvp.Value)
                {
                    poids[(kvp.Key, dest)] = 1.0;
                }
            }

            var parent = new Dictionary<int, int>(); // Contiendra les arêtes de l’arborescence finale.

            while (true)
            {
                // Étape 1: Sélection des arêtes entrantes minimales
                var minInEdge = new Dictionary<int, (int source, double weight)>();

                // Pour chaque nœud (hors racine qui est donné au départ), on cherche l’arête entrante avec le poids le plus faible. autrement dit on cherche le parent du noeud

                foreach (int node in grapheTravail.Keys.Where(n => n != racine))
                {
                    foreach (int source in grapheTravail.Keys)
                    {
                        if (grapheTravail[source].Contains(node))
                        {
                            double weight = poids[(source, node)];
                            if (!minInEdge.ContainsKey(node) || weight < minInEdge[node].weight)
                            {
                                minInEdge[node] = (source, weight);
                            }
                        }
                    }
                }

                // Construction du graphe des arêtes minimales
                // on modifie en créant un nouvau graphe dans lequel on insérera les nouvelles valeurs. cad les valeurs min calculer auparavant
                var grapheMinimal = new Dictionary<int, List<int>>();
                foreach (var node in grapheTravail.Keys)
                {
                    grapheMinimal[node] = new List<int>();
                }
                foreach (var kvp in minInEdge)
                {
                    grapheMinimal[kvp.Value.source].Add(kvp.Key);
                }

                // vérification d'un cycle
                var noeudsCycle = new Noeuds(this.Fichier);
                noeudsCycle.noeuds = grapheMinimal;
                bool aCycle = noeudsCycle.ContientCycle();

                if (!aCycle)
                {
                    // si il n'y a pas de cycle alors l'arborescence est déjà tout trouver.
                    // Par compte si il n'y en a un alors on peut tourner le cycle à l'infini si il permet de diminuer les poids
                    foreach (var kvp in minInEdge)
                    {
                        parent[kvp.Key] = kvp.Value.source;
                    }
                    break;
                }

                // Étape 2: Contraction du cycle 
                // d'après wiki, (dont le psuedo code m'a aidé à coder), la contraction de cycle permet de réduire le cyle en un noeud avec une arête entrante et sortante redéfinie.
                ContracterCycle(grapheTravail, minInEdge, poids);
            }

            return parent;
        }

        private void ContracterCycle(
            Dictionary<int, List<int>> graphe,
            Dictionary<int, (int, double)> minInEdge,
            Dictionary<(int, int), double> poids)
        {
            var visited = new Dictionary<int, int>();
            List<int> cycle = new List<int>();

            // Détection approximative du cycle 
            // Parcours des parents minimaux pour détecter un cycle. Si on retombe sur un nœud déjà en cours de parcours, un cycle est trouvé.
            foreach (int node in minInEdge.Keys)
            {
                if (!visited.ContainsKey(node))
                {
                    List<int> path = new List<int>();
                    int current = node;

                    while (true)
                    {
                        if (visited.TryGetValue(current, out int status))
                        {
                            if (status == 1) // Cycle détecté
                            {
                                int startIndex = path.IndexOf(current);
                                cycle = path.Skip(startIndex).ToList();
                                break;
                            }
                            break;
                        }
                        visited[current] = 1;
                        path.Add(current);

                        if (!minInEdge.ContainsKey(current)) break;
                        var InfosPré = minInEdge[current];
                        int parent = InfosPré.Item1;
                        current = parent;
                    }

                    if (cycle.Count > 0) break;
                }
            }
            // on sort si pas de cycle
            if (cycle.Count == 0) return;

            // Contraction du cycle en un super-nœud
            // On nomme le super-nœud par une valeur négative (pour éviter les conflits d’ID).
            int superNode = -Math.Abs(cycle.Min());
            graphe[superNode] = new List<int>();

            // Mise à jour des connexions
            foreach (int node in cycle)
            {
                // Suppression des anciens nœuds du cycle et redirection des arêtes
                graphe.Remove(node);

                // Mise à jour des connexions entrantes dans le cycle
                // pour cela, on redirige les arêtes entrantes vers le super-nœud, en ajustant les poids.
                foreach (int source in graphe.Keys.ToList())
                {
                    if (graphe[source].Contains(node))
                    {
                        graphe[source].Remove(node);
                        graphe[source].Add(superNode);

                        // Ajustement des poids
                        var infosPré = minInEdge[node];
                        double parentPoids = infosPré.Item2;
                        double newWeight = poids[(source, node)] - parentPoids;
                        poids[(source, superNode)] = newWeight;
                    }
                }

                // Mise à jour des connexions sortantes du cycle
                // pour cela on fait la même que précédemment, on redirige les arêtes sortantes du cycle vers l’extérieur, via le super-nœud.
                foreach (int dest in graphe.Keys.ToList())
                {
                    if (cycle.Contains(dest)) continue;

                    if (graphe[node].Contains(dest))
                    {
                        graphe[superNode].Add(dest);
                        poids[(superNode, dest)] = poids[(node, dest)];
                    }
                }
            }
        }

        /// <summary>
        /// Parcours en largeur (Breadth-First Search)
        /// </summary>
        /// <param name="depart">Nœud de départ du parcours</param>
        public void ParcoursEnLargeur(int depart)
        {
            HashSet<int> visite = new HashSet<int>();
            Queue<int> file = new Queue<int>();
            Dictionary<int, List<int>> noeudsPourParcours = this.noeuds;
            file.Enqueue(depart);
            visite.Add(depart);

            Console.WriteLine("\nBFS : \n");
            Console.Write("file de l'ordre de visite :\t");
            while (file.Count > 0)
            {
                int sommet = file.Dequeue();
                Console.Write(sommet + " ");


                if (noeudsPourParcours.ContainsKey(sommet))
                {
                    foreach (int voisin in noeudsPourParcours[sommet])
                    {
                        if (!visite.Contains(voisin))
                        {
                            file.Enqueue(voisin);
                            visite.Add(voisin);
                        }
                    }
                }

            }
            Console.WriteLine("");
        }

        /// <summary>
        /// Parcours en profondeur itératif (Depth-First Search)
        /// </summary>
        /// <param name="depart">Nœud de départ du parcours</param>
        public void ParcoursEnProfondeur(int depart)
        {
            HashSet<int> visite = new HashSet<int>();
            Stack<int> pile = new Stack<int>();
            List<int> noeudsMarqués = new List<int>();

            pile.Push(depart);

            Console.WriteLine("\nDFS : \n");
            Console.Write("pile de l'ordre de visite :\t");

            while (pile.Count > 0)
            {
                int sommet = pile.Peek();

                if (!visite.Contains(sommet))
                {
                    visite.Add(sommet);
                    Console.Write(sommet + " ");
                }

                bool PossessionDUnVoisin = false;

                if (this.noeuds.ContainsKey(sommet))
                {
                    foreach (int voisin in this.noeuds[sommet])
                    {
                        if (!visite.Contains(voisin))
                        {
                            pile.Push(voisin);
                            PossessionDUnVoisin = true;
                            break; // Pour faire un seul voisin à la fois
                        }
                    }
                }
                if (PossessionDUnVoisin == false)
                {
                    pile.Pop();
                    noeudsMarqués.Add(sommet);
                }

            }
            Console.WriteLine("\n\nordre de lecture et de marquage des sommets :\t");
            for (int i = 0; i < noeudsMarqués.Count; i++)
            {
                Console.Write(noeudsMarqués[i] + " ");
            }
        }

        /// <summary>
        /// Implémentation récursive du parcours en profondeur
        /// </summary>
        /// <param name="sommet">Nœud courant</param>
        /// <param name="visite">Ensemble des nœuds visités (gestion interne)</param>
        /// <param name="pile">Pile d'exécution (gestion interne)</param>
        /// <param name="aff">Contrôle d'affichage (interne)</param>
        public void DFSRécursif(int sommet, HashSet<int> visite = null, Stack<int> pile = null, string aff = "aff")
        {
            if (aff == "aff")
            {
                Console.WriteLine("\n\nDFSRécursif :\n");
                Console.Write("pile de l'ordre de visite :\t");
                aff = " ";
            }
            if (visite == null && pile == null)
            {
                visite = new HashSet<int>();
                pile = new Stack<int>();
            }
            if (pile == null)
            {
                pile.Push(sommet);
            }
            if (!pile.Contains(sommet))
            {
                pile.Push(sommet);
            }
            if (!visite.Contains(sommet))
            {
                Console.Write(sommet + " ");
                visite.Add(sommet);
                if (this.noeuds.ContainsKey(sommet))
                {
                    foreach (int voisin in this.noeuds[sommet])
                    {
                        if (!visite.Contains(voisin))
                        {
                            DFSRécursif(voisin, visite, pile, aff); // on appelle le voisin du voisin
                        }
                    }
                }
            }
            return; // quand on arrive au bout des voisins de voisins, on retourne au voisin précédent
        }

        /// <summary>
        /// Vérifie la connexité du graphe
        /// </summary>
        /// <returns>
        /// True si tous les nœuds sont connectés, False sinon
        /// </returns>
        public bool EstConnexe()
        {
            bool estconnexe = false;
            if (noeuds.Count == 0) return estconnexe; // Si le graphe est vide, il n'est pas connexe

            List<int> visite = new List<int>();
            List<int> file = new List<int>();   // File d'attente ( FIFO )

            int premierSommet = noeuds.Keys.First(); // On prend un sommet de départ quelconque

            file.Add(premierSommet);
            visite.Add(premierSommet);

            while (file.Count > 0)
            {
                int sommet = file[0]; // On récupère le premier élément ( FIFO )
                file.RemoveAt(0);

                if (noeuds.ContainsKey(sommet))
                {
                    foreach (int voisin in noeuds[sommet])
                    {
                        if (!visite.Contains(voisin))
                        {
                            file.Add(voisin);
                            visite.Add(voisin);
                        }
                    }
                }
            }
            if (visite.Count == noeuds.Count) { estconnexe = true; }
            return estconnexe;
        }

        /// <summary>
        /// Détecte la présence d'au moins un cycle dans le graphe
        /// </summary>
        /// <returns>
        /// True si cycle détecté, False pour graphe acyclique
        /// </returns>
        public bool ContientCycle()
        {
            int maxId = noeuds.Keys.Max();
            int[] etat = new int[maxId + 1]; // Tableau d'états (-1: non visité (blanc), 0: en cours (jaune), 1: terminé (rouge))

            for (int i = 0; i < etat.Length; i++)
                etat[i] = -1; // Initialisation à "non visité" (blanc)

            foreach (var noeud in noeuds.Keys)
            {
                if (etat[noeud] == -1) // Si le sommet n'est pas encore visité
                {
                    if (DFS_Cycle(noeud, -1, etat) == true)
                        return true; // Cycle détecté
                }
            }
            return false;
        }

        /// <summary>
        /// Algorithme DFS modifié pour détecter un cycle dans un graphe non orienté.
        /// La méthode est private car elle n'est appelé que depuis ContientCycle donc elle n'as pas besoin d'être public
        /// </summary>
        /// <param name="noeud">Nœud actuel.</param>
        /// <param name="parent">Nœud parent.</param>
        /// <param name="visite">Ensemble des nœuds visités.</param>
        /// <returns>True si un cycle est trouvé, sinon False.</returns>
        private bool DFS_Cycle(int noeud, int parent, int[] etat)
        {
            etat[noeud] = 0; // Marquer comme jaune

            if (noeuds.ContainsKey(noeud))
            {
                foreach (int voisin in noeuds[noeud])
                {
                    if (voisin >= etat.Length) continue;

                    if (etat[voisin] == -1) // Si voisin pas encore visité
                    {
                        if (DFS_Cycle(voisin, noeud, etat) == true)
                            return true;
                    }
                    else if (etat[voisin] == 0 && voisin != parent) // Cycle détecté
                    {
                        return true;
                    }
                }
            }

            etat[noeud] = 1; // Marquer comme "complètement exploré" (rouge)
            return false;
        }

        /// <summary>
        /// Affiche les caractéristiques principales du graphe
        /// </summary>
        public void AnalyseGraphe()
        {
            int ordre = noeuds.Count;
            int taille = 0;
            foreach (var voisins in noeuds.Values)
            {
                taille += voisins.Count;
            }

            bool estOrienté = false;

            foreach (var sommet in noeuds)
            {
                foreach (int voisin in sommet.Value)
                {
                    if (!noeuds.ContainsKey(voisin) || !noeuds[voisin].Contains(sommet.Key))
                    {
                        estOrienté = true; // Une arête est à sens unique
                        break;
                    }
                }
                if (estOrienté)
                {
                    break;
                }
            }
            if (estOrienté == false)
            {
                taille = taille / 2; // le graphe est non orienté, chaque arête est compté 2 fois
            }

            Console.WriteLine("Analyse du Graphe :");
            Console.WriteLine("Ordre du graphe (nombre de sommets) : "+ordre);
            Console.WriteLine("Taille du graphe (nombre d'arêtes) : "+taille);
            if (estOrienté)
            {
                Console.WriteLine("Le graphe est orienté");
            }
            else
            {
                Console.WriteLine("Le graphe n'est pas orienté");
            }
        }



        /// <summary>
        /// Vérifie heuristiquement la planarité du graphe
        /// </summary>
        /// <returns>
        /// True si le graphe respecte certaines conditions de planarité
        /// </returns>
        public bool EstPlanaire()
        {
            Dictionary<int, int> couleursAssignees = this.Welsh_Powell();

            int nombreCouleursUtilisees = 0;
            if (couleursAssignees != null)
            {
                nombreCouleursUtilisees = couleursAssignees.Values.Max();
            }
            if (nombreCouleursUtilisees <4)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Affiche la liste d'adjacence dans la console
        /// </summary>
        public void AfficheListe()
        {
            if (this.noeuds == null)
            {
                Console.WriteLine("GRAPHE NULL");
            }
            foreach (KeyValuePair<int, List<int>> kvp in this.noeuds)
            {
                Console.Write(kvp.Key + ": ");
                foreach (int noeudsliés in kvp.Value)
                {
                    Console.Write(noeudsliés + " ");
                }
                Console.WriteLine();
            }

        }
    }
}

