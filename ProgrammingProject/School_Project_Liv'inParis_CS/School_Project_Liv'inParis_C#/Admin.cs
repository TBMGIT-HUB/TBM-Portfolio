using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Xml;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Serialization;

namespace Visual_Studio
{
    internal class Admin
    {
        /// <summary>
        /// Affiche le menu de l'espace administrateur et gère les choix de l'utilisateur.
        /// </summary>
        public static void Menu()
        {
            char choix = '0';
            bool quitter = false;
            do
            {
                Console.Clear();
                Console.WriteLine("=== Espace Administrateur ===");
                Console.WriteLine();
                Console.WriteLine("1. Statistiques globals de l'application");
                Console.WriteLine("2. Coloration de graphes (Algo de Welsh-Powell)");
                Console.WriteLine("3. Couverture de graphe (Algo de Chu-Liu / Edmonds)");
                Console.WriteLine("4. Exporter la BDD en Json et Xml");
                Console.WriteLine("5. Importer la BDD depuis Json ou depuis Xml");
                Console.WriteLine("6. Quitter");
                Console.WriteLine();
                Console.Write("Votre choix : ");

                choix = Console.ReadKey().KeyChar;
                Console.WriteLine();

                switch (choix)
                {
                    case '1':
                        Statistiques();
                        break;
                    case '2':
                        Coloration_De_Graphe();
                        break;
                    case '3':
                        Couverture_de_graphe();
                        break;
                    case '4':
                        Exporter_La_BDD();
                        break;
                    case '5':
                        Importer_La_BDD();
                        break;
                    case '6':
                        quitter = true;
                        break; 
                    default:
                        Console.WriteLine("Choix invalide. Veuillez entrer 1, 2, 3, 4, 5 ou Q");
                        Thread.Sleep(700);
                        Console.Clear();
                        break;
                }
            }
            while (!quitter);
        }




        /// <summary>
        /// Affiche les statistiques globales de l'application.
        /// </summary>
        public static void Statistiques()
        {
            Console.Clear();
            Console.WriteLine("=== Statistiques Globales de l'Application ===");
            Console.WriteLine("--------------------------------------------");

            // Ouverture de la connexion
            try
            {
                Role.ConnexionAdmin.Open();
                try
                {
                    string requete;
                    MySqlCommand commande = Role.ConnexionAdmin.CreateCommand();

                    // 0. Nombre total d'utilisateurs (sans admin ID 1)
                    requete = "SELECT IFNULL(COUNT(*), 0) FROM Utilisateur WHERE Identifiant > 1";
                    commande.CommandText = requete;
                    int nbUtilisateurs = Convert.ToInt32(commande.ExecuteScalar());
                    Console.WriteLine("Nombre total d'utilisateurs inscrits : " + nbUtilisateurs);

                    // 1. Nombre de cuisiniers
                    requete = "SELECT IFNULL(COUNT(*), 0) FROM Cuisinier WHERE Identifiant > 1";
                    commande.CommandText = requete;
                    int nbCuisiniers = Convert.ToInt32(commande.ExecuteScalar());
                    Console.WriteLine("Nombre de cuisiniers : " + nbCuisiniers);

                    // 2. Nombre de clients
                    requete = "SELECT IFNULL(COUNT(*), 0) FROM Client WHERE Identifiant > 1";
                    commande.CommandText = requete;
                    int nbClients = Convert.ToInt32(commande.ExecuteScalar());
                    Console.WriteLine("Nombre de clients : " + nbClients);

                    // 3. Nombre de clients ayant passé au moins une commande livrée
                    requete = @"SELECT u.Pseudo, 
                        IFNULL(COUNT(c.Id_Commande), 0) AS NbCommandes
                        FROM Client cl
                        LEFT JOIN Utilisateur u ON cl.Identifiant = u.Identifiant
                        LEFT JOIN Commande c ON cl.Identifiant = c.Identifiant AND c.Etat_de_la_commande = TRUE
                        GROUP BY cl.Identifiant, u.Pseudo
                        ORDER BY NbCommandes DESC";
                    commande.CommandText = requete;
                    MySqlDataReader reader2 = commande.ExecuteReader();
                    Console.WriteLine("\nClients et leur nombre de commandes livrées:");
                    while (reader2.Read())
                    {
                        Console.Write("- ");
                        Console.Write(reader2.GetString(0));
                        Console.Write(": ");
                        Console.Write(reader2.GetInt32(1));
                        Console.WriteLine(" commandes");
                    }
                    reader2.Close();




                    Console.WriteLine();




                    // 4. Nombre total de plats proposés
                    requete = "SELECT IFNULL(COUNT(*), 0) FROM Plat_Propose";
                    commande.CommandText = requete;
                    int nbPlats = Convert.ToInt32(commande.ExecuteScalar());
                    Console.WriteLine("Nombre total de plats proposés : " + nbPlats);

                    // 5. Nombre total de commandes passées
                    requete = "SELECT IFNULL(COUNT(*), 0) FROM Commande";
                    commande.CommandText = requete;
                    int nbCommandes = Convert.ToInt32(commande.ExecuteScalar());
                    Console.WriteLine("Nombre total de commandes passées : " + nbCommandes);

                    // 6. Nombre total de commandes livrées
                    requete = "SELECT IFNULL(COUNT(*), 0) FROM Commande WHERE Etat_de_la_commande = TRUE";
                    commande.CommandText = requete;
                    int nbCommandesLivrees = Convert.ToInt32(commande.ExecuteScalar());
                    Console.WriteLine("Nombre total de commandes livrées : " + nbCommandesLivrees);




                    Console.WriteLine();




                    // 7. Prix moyen d'une part d'un plat
                    requete = "SELECT IFNULL(AVG(Prix_par_personne), 0) FROM Plat_Propose";
                    commande.CommandText = requete;
                    double prixMoyenPart = Convert.ToDouble(commande.ExecuteScalar());
                    Console.WriteLine("Prix moyen d'une part d'un plat : " + prixMoyenPart.ToString("0.00") + " euros"); // permet d'arrondir la moyenne à 2 chiffres après la virgule

                    requete = "SELECT IFNULL(COUNT(DISTINCT Identifiant), 0) FROM Commande WHERE Etat_de_la_commande = TRUE";
                    commande.CommandText = requete;
                    int nbClientsAyantCommande = Convert.ToInt32(commande.ExecuteScalar());
                    Console.WriteLine("Nombre de clients ayant passé au moins une commande livrée : " + nbClientsAyantCommande);
                    // 8. Dépense moyenne par client (commandes livrées)
                    requete = @"SELECT IFNULL(SUM(c.Nb_de_part * p.Prix_par_personne), 0) 
                        FROM Commande c JOIN Plat_Propose p ON c.Id_Plat = p.Id_Plat 
                        WHERE c.Etat_de_la_commande = TRUE";
                    commande.CommandText = requete;
                    double totalDepenses = Convert.ToDouble(commande.ExecuteScalar()); 

                    if (nbClientsAyantCommande == 0)
                    {
                        Console.WriteLine("Aucun client n'a passé de commande livrée.");
                    }
                    else
                    {
                        double depenseMoyenneClient = totalDepenses / nbClientsAyantCommande;
                        Console.WriteLine("Dépense moyenne par client (commandes livrées) : " + depenseMoyenneClient.ToString("0.00") + " euros"); 
                    }

                    Console.WriteLine();

                    // 9. Note moyenne reçue par les cuisiniers
                    requete = @"SELECT u.Pseudo, 
                        IFNULL(COUNT(c.Id_Commande), 0) AS NbCommandes
                        FROM Client cl
                        LEFT JOIN Utilisateur u ON cl.Identifiant = u.Identifiant
                        LEFT JOIN Commande c ON cl.Identifiant = c.Identifiant AND c.Etat_de_la_commande = TRUE
                        GROUP BY cl.Identifiant, u.Pseudo
                        ORDER BY NbCommandes DESC";
                    commande.CommandText = requete;
                    MySqlDataReader reader3 = commande.ExecuteReader();
                    Console.WriteLine("\nClients et leur nombre de commandes livrées:");
                    while (reader3.Read())
                    {
                        Console.Write("- ");
                        Console.Write(reader3.GetString(0));
                        Console.Write(": ");
                        Console.Write(reader3.GetInt32(1));
                        Console.WriteLine(" commandes");
                    }
                    reader3.Close();

                    // 10. Note moyenne reçue par les clients
                    requete = "SELECT IFNULL(AVG(Notation), 0) FROM Notation_Client";
                    commande.CommandText = requete;
                    double noteMoyenneClients = Convert.ToDouble(commande.ExecuteScalar());
                    Console.WriteLine("Note moyenne reçue par les clients : " + noteMoyenneClients.ToString("0.00") + "/5.00");



                    Console.WriteLine();



                    // 11. Pourcentage de commandes livrées
                    requete = "SELECT IFNULL(Count(*), 0) FROM Commande";
                    commande.CommandText = requete;
                    double pourcentageLivreestest = Convert.ToDouble(commande.ExecuteScalar());
                    requete = "SELECT IFNULL(Count(*), 0) FROM Commande WHERE Etat_de_la_commande = True";
                    commande.CommandText = requete;
                    double pourcentageLivreestest2 = Convert.ToDouble(commande.ExecuteScalar());
                    double res = (pourcentageLivreestest2 / pourcentageLivreestest) * 100;
                    Console.WriteLine("Pourcentage de commandes livrées : " + res.ToString("0.00") + "%");

                    // 12. Moyenne du nombre d'ingrédients par plat
                    requete = @"SELECT IFNULL(AVG(nb), 0) FROM (SELECT COUNT(*) AS nb FROM Ingredient_Total GROUP BY Id_Plat) AS sous";
                    commande.CommandText = requete;
                    double moyenneIngredients = Convert.ToDouble(commande.ExecuteScalar());
                    Console.WriteLine("Moyenne du nombre d'ingrédients par plat : " + moyenneIngredients.ToString("0.00"));




                    Console.WriteLine();




                    // 13. Top 3 recettes les plus utilisées
                    requete = @"SELECT IFNULL(Recette_autorise, '') FROM Plat_Propose 
                        GROUP BY Recette_autorise ORDER BY COUNT(*) DESC LIMIT 3";
                    commande.CommandText = requete;
                    MySqlDataReader reader = commande.ExecuteReader(); // réutilser pour les autres Top 3
                    int compteur = 0; // réutilisaton pour plus tard
                    Console.WriteLine("Top 3 recettes les plus utilisées : ");
                    while (reader.Read())
                    { 
                        compteur++;
                        Console.WriteLine("\t- " + compteur+ " " + reader.GetString(0)); // ici on doit mettre 0 car sinon ca ne marceh pas avec la condition dans la requete SQL
                    }
                    reader.Close();
                    

                    // 14. Top 3 ingrédients les plus utilisés
                    requete = @"SELECT IFNULL(Nom_Ingredient_autorise, '') 
                        FROM Ingredient_Total GROUP BY Nom_Ingredient_autorise 
                        ORDER BY SUM(Quantite_) DESC LIMIT 3";
                    commande.CommandText = requete;
                    reader = commande.ExecuteReader();
                    compteur = 0;
                    Console.WriteLine("Top 3 ingrédients les plus utilisés : ");
                    while (reader.Read())
                    {
                        compteur++;
                        Console.WriteLine("\t- " + compteur + " " + reader.GetString(0)); // ici on doit mettre 0 car sinon ca ne marceh pas avec la condition dans la requete SQL
                    }
                    reader.Close();

                    // 15. Top 3 régimes les plus suivis
                    requete = "SELECT IFNULL(Nom_Regime, '') FROM Suit_le_régime GROUP BY Nom_Regime ORDER BY COUNT(*) DESC LIMIT 3";
                    commande.CommandText = requete;
                    reader = commande.ExecuteReader();
                    compteur = 0;
                    Console.WriteLine("Top 3 régimes les plus suivis : " );
                    while (reader.Read())
                    {
                        compteur++;
                        Console.WriteLine("\t- " + compteur + " " + reader.GetString(0)); // ici on doit mettre 0 car sinon ca ne marceh pas avec la condition dans la requete SQL
                    }              
                    reader.Close();

                    Console.WriteLine();

                    // 16. Top 3 des cuisiniers ayant fait le plus de commandes livrées
                    requete = @"SELECT IFNULL(u.Nom, ''), IFNULL(u.Prenom, ''), IFNULL(COUNT(*), 0) 
                        FROM Commande c JOIN Plat_Propose p ON c.Id_Plat = p.Id_Plat JOIN Cuisinier cu ON p.Identifiant = cu.Identifiant 
                        JOIN Utilisateur u ON cu.Identifiant = u.Identifiant 
                        WHERE c.Etat_de_la_commande = TRUE GROUP BY cu.Identifiant ORDER BY COUNT(*) DESC LIMIT 3";
                    commande.CommandText = requete;
                    reader = commande.ExecuteReader();
                    compteur = 0;
                    Console.WriteLine("Cuisinier le plus actif : ");
                    while (reader.Read())
                    {
                        compteur++;
                        Console.WriteLine("\t- " + compteur + ". " + reader.GetString(0) + " " + reader.GetString(1) + " avec " + reader.GetInt32(2) + " commandes"); 
                    }
                    reader.Close();


                    // 17. Top 3 des clients ayant commandé le plus
                    requete = @"SELECT IFNULL(u.Nom, ''), IFNULL(u.Prenom, ''), IFNULL(COUNT(*), 0) 
                        FROM Commande c JOIN Client cl ON c.Identifiant = cl.Identifiant 
                        JOIN Utilisateur u ON cl.Identifiant = u.Identifiant 
                        GROUP BY cl.Identifiant ORDER BY COUNT(*) DESC LIMIT 3";
                    commande.CommandText = requete;
                    reader = commande.ExecuteReader();
                    compteur = 0;
                    Console.WriteLine("Client le plus actif : ");
                    while (reader.Read())
                    {
                        compteur++;
                        Console.WriteLine("\t- " + compteur + ". " + reader.GetString(0) + " " + reader.GetString(1) + " avec " + reader.GetInt32(2) + " commandes");
                    }
                    reader.Close();

                    // 18. Cuisinier par nombre de livraisons effectuées
                    requete = @"SELECT IFNULL(u.Nom, 'N/A'), IFNULL(u.Prenom, 'N/A'), COUNT(c.Id_Commande)
                                FROM Commande c
                                JOIN Plat_Propose p ON c.Id_Plat = p.Id_Plat
                                JOIN Utilisateur u ON p.Identifiant = u.Identifiant
                                WHERE c.Etat_de_la_commande = TRUE
                                GROUP BY p.Identifiant, u.Nom, u.Prenom
                                ORDER BY COUNT(c.Id_Commande) DESC;";
                    commande.CommandText = requete;
                    reader = commande.ExecuteReader();
                    Console.WriteLine("\n1. Nombre de livraisons par Cuisinier :");
                    if (!reader.HasRows)
                    {
                        Console.WriteLine("   Aucune livraison trouvée.");
                    }
                    else
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine("   - " + reader.GetString(0) + " " + reader.GetString(1) + ": " + reader.GetInt32(2) + " livraison(s)");
                        }
                    }
                    reader.Close();


                    // 19. Commandes pour un client et une nationalité donnés ordonnée par date
                    Console.WriteLine("\nCommandes par Client et Nationalité :");

                    int ClientIdChoisi = 0;
                    bool IdValide = false;
                    do
                    {
                        Console.Write("Entrez l'ID du client à consulter : ");
                        string clientIdInput = Console.ReadLine(); 
                        if (int.TryParse(clientIdInput, out ClientIdChoisi) && ClientIdChoisi > 0)
                        {
                            IdValide = true;
                        }
                        else
                        {
                            Console.WriteLine("ID invalide. Veuillez entrer un nombre entier positif.");
                        }
                    } while (!IdValide);

                    string NationaliteChoisi = "";
                    do
                    {
                        Console.Write("Entrez la nationalité des plats à filtrer : ");
                        NationaliteChoisi = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(NationaliteChoisi))
                        {
                            Console.WriteLine("La nationalité ne peut pas être vide.");
                        }
                    } while (string.IsNullOrWhiteSpace(NationaliteChoisi));

                    requete = @"SELECT c.Id_Commande,
                        IFNULL(p.Nom, 'Plat Inconnu') AS NomPlat,c.Nb_de_part,IFNULL(p.Nationalite, 'N/A') AS Nationalite
                        FROM Commande c JOIN Plat_Propose p ON c.Id_Plat = p.Id_Plat WHERE c.Identifiant = @ClientId
                        AND p.Nationalite = @Nationalite ORDER BY c.Id_Commande DESC;";

                    commande.CommandText = requete;
                    commande.Parameters.Clear();
                    commande.Parameters.AddWithValue("@ClientId", ClientIdChoisi);
                    commande.Parameters.AddWithValue("@Nationalite", NationaliteChoisi.Trim());
                    reader = commande.ExecuteReader();

                    Console.WriteLine("Commandes pour Client ID " + ClientIdChoisi + " (Plats: " + NationaliteChoisi + ") :");

                    if (!reader.HasRows)
                    {
                        Console.WriteLine("Aucune commande correspondante trouvée pour ces critères.");
                    }
                    else
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine(" - ID: " + reader.GetInt32(0) + ", Plat: " + reader.GetString(1) + ", Parts: " + reader.GetInt32(2));
                        }
                    }
                    reader.Close();


                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Erreur SQL lors de la récupération des statistiques : " + ex.Message);
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Impossible d'ouvrir la connexion : " + ex.Message);
            }

            // Fermeture de la connexion
            try
            {
                Role.ConnexionAdmin.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur lors de la fermeture de la connexion : " + ex.Message);
            }

            Console.WriteLine("--------------------------------------------");
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }









        /// <summary>
        /// Affiche le menu de coloration de graphes et gère les choix de l'utilisateur.
        /// </summary>
        public static void Coloration_De_Graphe()
        {
            Console.Clear();
            Console.WriteLine("=== Coloration de graphes (Welsh-Powell) ===");
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Arcs.csv");

            Noeuds noeuds = new Noeuds(filePath);
            noeuds.AfficheListe();
            foreach (KeyValuePair<int, int> pairDeValeur in noeuds.Welsh_Powell())
            {
                Console.WriteLine("sommet : " + pairDeValeur.Key + " --- > couleur : " + pairDeValeur.Value);
            }
            Dictionary<int, int> WelshNoeuds = noeuds.Welsh_Powell();

            
            Console.WriteLine("Algorithme implémenté.");
            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey();
        }








        public static void Couverture_de_graphe()
        {
            Console.Clear();
            Console.WriteLine("=== Couverture de graphe (Chu-Liu / Edmonds) ===");
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Arcs.csv");

            Noeuds noeuds = new Noeuds(filePath);
            noeuds.AfficheListe();
            Dictionary<int, int> arborescence = noeuds.Chu_Liu_Edmonds(1);
            foreach (var kvp in arborescence)
            {
                Console.WriteLine("Noeud "+kvp.Key+" a pour parent "+kvp.Value);
            }
            Console.WriteLine("Algorithme implémenté.");
            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey();
        }










        









        /// <summary>
        /// Affiche le menu d'exportation de la base de données et gère les choix de l'utilisateur.
        /// </summary>
        public static void Exporter_La_BDD()
        {
            Console.Clear();
            Console.WriteLine("=== Exporter la BDD en Json et Xml ===");
            Console.WriteLine();
            Console.WriteLine("1. Exporter en XML");
            Console.WriteLine("2. Exporter en JSON");
            Console.WriteLine("3. Retour");
            Console.Write("Votre choix : ");

            char choix = Console.ReadKey().KeyChar;
            Console.WriteLine();

            switch (choix)
            {
                case '1':
                    ExporterEnXML(); 
                    break;
                case '2':
                    ExporterEnJSON();
                    break;
                case '3':
                    break;
                default:
                    Console.WriteLine("Choix de format invalide.");
                    break;
            }
        }

        


        public static void Importer_La_BDD()
        {
            Console.Clear();
            Console.WriteLine("=== Importer depuis Fichier ===");
            Console.WriteLine("1. Importer depuis XML");
            Console.WriteLine("2. Importer depuis JSON");
            Console.WriteLine("3. Retour");
            Console.Write("Votre choix : ");
            char choix = Console.ReadKey().KeyChar;
            Console.WriteLine();

            string fichierXml = "export_database_livin.xml";
            string fichierJson = "export_database_livin.json";
            ExportData donneesImportees = null;

            switch (choix)
            {
                case '1':
                    donneesImportees = DeserealiserXML(fichierXml);
                    break;
                case '2':
                    donneesImportees = DeserealiserJson(fichierJson);
                    break;
                case '3':
                    return;
                default:
                    Console.WriteLine("Choix invalide.");
                    break;
            }

            if (donneesImportees != null)
            {
                Console.WriteLine("\nImportation réussie. Données chargées en mémoire.");
            }
            else
            {
                Console.WriteLine("\nL'importation a échoué.");
            }
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }



        public static void ExporterEnXML()
        {
            Console.WriteLine("\n--- Lancement de l'exportation XML ---");



            // Création de l'instance de la classe DatabaseExporter
            DatabaseExporter exporter = new DatabaseExporter();

            //  1: Récupérer les données
            Console.WriteLine("Récupération des données depuis la base...");
            // Récupère les données de la base de données
            ExportData donnees = exporter.StructureBDD();

            //  2: Vérifier si les données ont été récupérées
            // Si nul on n'exporte rien
            if (donnees != null)
            {
                //  3: Définir le chemin du fichier de sortie
                string nomFichier = "export_database_livin.xml";
                Console.WriteLine("Exportation des données vers le fichier : " + nomFichier);

                //  4: Exporter vers XML, créer le fichier XML
                XmlSerializer serializer = new XmlSerializer(typeof(ExportData));
                StreamWriter writer = new StreamWriter(nomFichier);
                serializer.Serialize(writer, donnees);
                writer.Close();
                Console.WriteLine("Données exportées avec succès dans " + nomFichier);


                Console.WriteLine("--- Fin de l'exportation XML ---");

                Console.WriteLine("Appuyez sur une touche pour afficher le document...");
                Console.ReadKey();

                // Affichage du document XML
                Console.Clear();
                LectureXML(nomFichier);

                Console.WriteLine("\nAppuyez sur une touche pour revenir au menu...");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("L'exportation XML a échoué car les données n'ont pas pu être récupérées de la base.");
                Console.ReadKey();
            }

        }

        static void LectureXML(string nomFichier)
        {
            XmlDocument docXml = new XmlDocument();
            docXml.Load(nomFichier);
            // Affichage de la racine
            XmlElement racine = docXml.DocumentElement;

            Console.WriteLine("racine : " + racine.Name);
            Console.WriteLine();

            // Affichage des attributs de la racine
            foreach (XmlNode e in racine)
            {
                Console.WriteLine("balise : " + e.Name);

                // Affichage des attributs de la balise
                if (e.Attributes != null)
                {
                    // Affichage des attributs de la balise
                    foreach (XmlAttribute a in e.Attributes)
                    {
                        Console.Write("  attribut :");
                        Console.Write(" nom = " + a.Name);
                        Console.WriteLine(", valeur : " + a.Value);
                    }
                }

                // Affichage du contenu texte de la balise
                foreach (XmlNode e2 in e.ChildNodes)
                {
                    if (e2.NodeType == XmlNodeType.Text)
                    {
                        Console.WriteLine("  InnerText : " + e2.InnerText);
                    }
                    else
                    {
                        Console.WriteLine("  sous-balise : " + e2.Name);

                        // Affichage du contenu texte de la sous-balise
                        Console.WriteLine("    contenu : " + e2.InnerText);

                        // Affichage des attributs de la sous-balise
                        if (e2.Attributes != null && e2.Attributes.Count > 0)
                        {
                            foreach (XmlAttribute attr in e2.Attributes)
                            {
                                Console.WriteLine("    attribut : nom = " + attr.Name + ", valeur = " + attr.Value);
                            }
                        }
                    }
                }
            }
        }















        /// <summary>
        /// Exportation de la base de données au format JSON.
        /// </summary>

        public static void ExporterEnJSON()
        {
            Console.WriteLine("\n--- Lancement de l'exportation JSON ---");


            DatabaseExporter exporter = new DatabaseExporter();

            //  1: Récupérer les données
            Console.WriteLine("Récupération des données depuis la base...");
            ExportData donnees = exporter.StructureBDD();


            // 2: Vérifier si les données ont été récupérées
            if (donnees != null)
            {
                // 3: Définir le chemin du fichier de sortie
                string nomFichier = "export_database_livin.json";
                Console.WriteLine("Exportation des données vers le fichier : " + nomFichier);

                // 4: Exporter vers JSON
                StreamWriter writer = new StreamWriter(nomFichier);
                JsonTextWriter jwriter = new JsonTextWriter(writer); ;
                
                jwriter.Formatting = Newtonsoft.Json.Formatting.Indented; // Indentation pour le formatage

                // Écrire la racine du JSON
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(jwriter, donnees); // Sérialise l'objet ExportData complet

                Console.WriteLine("Données exportées avec succès dans " + nomFichier);
                jwriter.Close();


                Console.WriteLine("--- Fin de l'exportation JSON ---");

                Console.WriteLine("Appuyez sur une touche pour afficher le document...");
                Console.ReadKey();

                // Affichage du document JSON
                Console.Clear();
                LectureJson(nomFichier);

                Console.WriteLine("\nAppuyez sur une touche pour revenir au menu...");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("L'exportation JSON a échoué car les données n'ont pas pu être récupérées de la base.");
                Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                Console.ReadKey();
            }


            
        }





        /// <summary>
        /// Lit un fichier JSON et affiche son contenu de manière formatée.
        /// </summary>
        /// <param name="nomFichier"></param>

        static void LectureJson(string nomFichier)
        {
            // Ouvrir le fichier JSON
            StreamReader reader = new StreamReader(nomFichier);
            JsonTextReader jreader = new JsonTextReader(reader);

            // Lire le contenu du fichier JSON
            while (jreader.Read())
            {
                switch (jreader.TokenType)
                {
                    // Afficher le type de jeton
                    case JsonToken.StartObject: 
                        Console.WriteLine("{");
                        break;
                    // Afficher la fin de l'objet
                    case JsonToken.EndObject:
                        Console.WriteLine("}");
                        break;
                    // Afficher le début d'un tableau
                    case JsonToken.StartArray:
                        Console.WriteLine("[");
                        break;
                    // Afficher la fin d'un tableau
                    case JsonToken.EndArray:
                        Console.WriteLine("]");
                        break;
                    // Afficher le nom de la propriété
                    case JsonToken.PropertyName:
                        // Une tabulation avant le nom de la propriété
                        Console.Write("\t\"" + jreader.Value + "\": ");
                        break;
                    // Afficher la valeur Null
                    case JsonToken.Null:
                        Console.WriteLine("null");
                        break;
                    default:
                        Console.WriteLine(jreader.Value);
                        break;
                }
            }
            // Fermer le lecteur JSON
            jreader.Close();
        }

        static ExportData DeserealiserXML(string nomFichier)
        {
            Console.WriteLine("\n--- Lancement de la désérialisation XML depuis " + nomFichier + " ---");
            ExportData donneesImportees = null;
            StreamReader reader = null;

            try
            {
                if (!File.Exists(nomFichier))
                {
                    Console.WriteLine("ERREUR: Le fichier XML '" + nomFichier + "' n'a pas été trouvé.");
                    return null;
                }

                XmlSerializer serializer = new XmlSerializer(typeof(ExportData));
                reader = new StreamReader(nomFichier);

                donneesImportees = (ExportData)serializer.Deserialize(reader);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERREUR inattendue lors de la désérialisation XML : " + ex.Message);
            }
            finally
            {
                if (reader != null)
                {
                    try
                    {
                        reader.Close();
                    }
                    catch (Exception Ex)
                    {
                        Console.WriteLine("Erreur lors de la fermeture du lecteur XML.");
                    }
                }
                Console.WriteLine("--- Fin de la désérialisation XML ---");
            }
            return donneesImportees;
        }

        /// <summary>
        /// Désérialise un fichier JSON sans utiliser de bloc 'using'.
        /// </summary>
        static ExportData DeserealiserJson(string nomFichier)
        {
            Console.WriteLine("\n--- Lancement de la désérialisation JSON depuis " + nomFichier + " ---");
            ExportData donneesImportees = null;
            StreamReader fileReader = null;
            JsonTextReader jsonReader = null;

            try
            {
                if (!File.Exists(nomFichier))
                {
                    Console.WriteLine("ERREUR: Le fichier JSON '" + nomFichier + "' n'a pas été trouvé.");
                    return null;
                }

                fileReader = new StreamReader(nomFichier);
                jsonReader = new JsonTextReader(fileReader);

                JsonSerializer serializer = new JsonSerializer();
                donneesImportees = serializer.Deserialize<ExportData>(jsonReader);

                Console.WriteLine("Désérialisation JSON terminée avec succès.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERREUR inattendue lors de la désérialisation JSON : " + ex.Message);
            }
            finally
            {
                if (jsonReader != null)
                {
                    try
                    {
                        jsonReader.Close();
                    }
                    catch (Exception Ex) { Console.WriteLine("Erreur fermeture JsonReader: " + Ex.Message); }
                }
                if (fileReader != null)
                {
                    try
                    {
                        fileReader.Close();
                    }
                    catch (Exception Ex) { Console.WriteLine("Erreur fermeture StreamReader: " + Ex.Message); }
                }
                Console.WriteLine("--- Fin de la désérialisation JSON ---");
            }

            return donneesImportees;
        }
    }
}
