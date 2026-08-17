using MySql.Data.MySqlClient;
using System;
using Visual_Studio;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading; 
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections;

namespace Visual_Studio
{
    class Client
    {
        /// <summary>
        /// Methode d'affichage du menu du client.
        /// </summary>
        public static void MenuClient()
        {
            char choix = '0';
            bool quitter = false;
            do
            {
                Console.Clear();
                Console.WriteLine("=== Espace Client ===");
                Console.WriteLine();
                Console.WriteLine("1. Voir les plats disponibles et Commander");
                Console.WriteLine("2. Gérer vos Commandes");
                Console.WriteLine("3. Noter un Cuisinier");
                Console.WriteLine("4. Afficher votre profil");
                Console.WriteLine("5. Quitter");
                Console.WriteLine();
                Console.Write("Votre choix : ");

                choix = Console.ReadKey().KeyChar;
                Console.WriteLine(); 

                switch (choix)
                {
                    case '1':
                        CommanderPlat();
                        break;
                    case '2':
                        GererCommandes();
                        break;
                    case '3':
                        NoterCuisinier();
                        break;
                    case '4':
                        AfficherProfilClient();
                        break;
                    case '5':
                        quitter = true;
                        break;
                    default:
                        Console.WriteLine("Choix invalide. Veuillez entrer 1, 2, 3 ou 4.");
                        Thread.Sleep(700); 
                        break;
                }
            }
            while (!quitter);
        }






































        /// <summary>
        /// Gère le processus de consultation des plats et de passation de commande,
        /// en calculant les portions disponibles. 
        /// </summary>
        public static void CommanderPlat()
        {
            Console.Clear();
            Console.WriteLine("--- Commande d'un plat ---");

            // Afficher les plats disponibles (calcule les parts restantes)
            List<Tuple<int, int>> platsDisponibles = AfficherPlatsDisponibles();

            if (platsDisponibles == null || platsDisponibles.Count == 0)
            {
                Console.WriteLine("\nAppuyez sur une touche pour revenir au menu...");
                Console.ReadKey();
                return;
            }

            // Choisir le plat
            int idPlatChoisi = 0;
            int maxPartsPlatChoisi = 0;
            bool idValide = false;
            do
            {
                Console.Write("\nEntrez l'ID du plat que vous souhaitez commander (ou Q pour quitter) : ");
                string saisieId = Console.ReadLine().Trim(); 

                if (saisieId.ToUpper() == "Q") 
                    return;

                if (!int.TryParse(saisieId, out idPlatChoisi))
                {
                    Console.WriteLine("Veuillez entrer un ID numérique valide.");
                }
                else
                {
                    Tuple<int, int> platTupleTrouve = null;
                    foreach (Tuple<int, int> p in platsDisponibles) //véridie si l'id rentré par l'utilisateur est dans la liste affiché
                    {
                        if (p.Item1 == idPlatChoisi)
                        {
                            platTupleTrouve = p;
                            break;
                        }
                    }

                    if (platTupleTrouve == null)
                    {
                        Console.WriteLine("Cet ID de plat n'est pas dans la liste des plats disponibles actuellement.");
                    }
                    else
                    {
                        maxPartsPlatChoisi = platTupleTrouve.Item2; // Parts calculées lors de l'affichage
                        if (maxPartsPlatChoisi <= 0) 
                        {
                            Console.WriteLine("Ce plat n'a plus de parts disponibles.");
                        }
                        else
                        {
                            idValide = true;
                        }
                    }
                }
            } while (!idValide);

            // Choisir le nombre de parts
            int nombrePartsDemandees = 0;
            bool partsValides = false;
            do
            {
                // Afficher le max de parts possible pour ce plat
                Console.Write("Combien de parts souhaitez-vous commander (1-" + maxPartsPlatChoisi + ") ? (ou Q pour quitter) : ");
                string saisieParts = Console.ReadLine().Trim();

                if (saisieParts.ToUpper() == "Q")
                    return;

                if (!int.TryParse(saisieParts, out nombrePartsDemandees))
                {
                    Console.WriteLine("Veuillez entrer un nombre valide.");
                }
                else if (nombrePartsDemandees <= 0)
                {
                    Console.WriteLine("Vous devez commander au moins 1 part.");
                }
                else if (nombrePartsDemandees > maxPartsPlatChoisi)
                {
                    Console.WriteLine("Vous ne pouvez pas commander plus de " + maxPartsPlatChoisi + " part(s) pour ce plat.");
                }
                else
                {
                    partsValides = true;
                }
            } while (!partsValides);

            // Récupérer l'ID du Cuisinier qui a proposé ce plat
            int idCuisinier = RecupererCuisinierIdDePlatId(idPlatChoisi);
            if (idCuisinier == -1) // sécurité en plus
            {
                Console.WriteLine("Erreur: Impossible de trouver le cuisinier associé à ce plat.");
                Console.WriteLine("Appuyez sur une touche pour continuer...");
                Console.ReadKey();
                return;
            }

            // Début des opérations de base de données pour la commande
            // Insérer la commande
            InsererCommande(idPlatChoisi, nombrePartsDemandees);
            // Mettre à jour les compteurs Client
            UpdateCompteursClient(true); // true pour incrémenter
            // Mettre à jour les compteurs Cuisinier
            UpdateCompteursCuisinier(idCuisinier, true); // true pour incrémenter


            Console.WriteLine("\nCommande passée avec succès !");

            Console.WriteLine("\nAppuyez sur une touche pour revenir au menu...");
            Console.ReadKey();
        }













        /// <summary>
        /// Affiche les plats commandables (non périmés) et retourne la liste
        /// de leurs IDs et du nombre de parts disponibles calculé.
        /// </summary>
        /// <returns>Liste de Tuples (ID Plat, Parts Disponibles Calculées), ou null/vide en cas d'erreur/aucun plat.</returns>
        private static List<Tuple<int, int>> AfficherPlatsDisponibles()
        {
            List<Tuple<int, int>> idsEtPartsPlats = new List<Tuple<int, int>>();
            Console.WriteLine("\n--- Plats Disponibles ---");
            MySqlDataReader reader = null;

            try
            {
                Role.ConnexionClient.Open();

                // Requête pour obtenir les plats non périmés avec infos de base et pseudo du cuisinier
                string requette = @"
                    SELECT p.Id_Plat, p.Nom, p.Prix_par_personne, p.Type, p.Nationalite,
                           p.Nb_De_Client AS PartsInitiales,
                           u.Pseudo AS CuisinierPseudo
                            FROM Plat_Propose p
                            JOIN Utilisateur u ON p.Identifiant = u.Identifiant 
                            WHERE p.Date_de_peremption > NOW()";

                MySqlCommand cmd = new MySqlCommand(requette, Role.ConnexionClient);
                reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    Console.WriteLine("Aucun plat disponible pour le moment.");
                    reader.Close();
                    Role.ConnexionClient.Close();
                    return idsEtPartsPlats; // Retourne une liste vide
                }


                List<(int IdPlat, string Nom, int Prix, string Type, string Nationalite, int PartsInitiales, string Pseudo)> platsTemp = new List<(int IdPlat, string Nom, int Prix, string Type, string Nationalite, int PartsInitiales, string Pseudo)>();

                while (reader.Read())
                {
                    platsTemp.Add((
                        reader.GetInt32("Id_Plat"),
                        reader.GetString("Nom"),
                        reader.GetInt32("Prix_par_personne"),
                        reader.GetString("Type"),
                        reader.GetString("Nationalite"),
                        reader.GetInt32("PartsInitiales"),
                        reader.GetString("CuisinierPseudo")
                    ));
                }
                try
                {
                    reader.Close();
                }
                catch (Exception ex)
                {

                }

                Console.WriteLine("ID\tNom du Plat\t\tPrix/Pers.\tType\t\tNationalité\tParts Restantes\tCuisinier");
                Console.WriteLine("---------------------------------------------------------------------------------------------------------");

                int partsDispo = 0;
                foreach ((int IdPlat, string Nom, int Prix, string Type, string Nationalite, int PartsInitiales, string Pseudo) plat in platsTemp)
                {
                    int partsCommandees = 0;
                    try
                    {
                        string requettePartsCmd = "SELECT IFNULL(SUM(Nb_de_part), 0) FROM Commande WHERE Id_Plat = @IdPlat"; // obliger de mettre IFNULL car si aucune commande n'existe pour un plat on a une erreur de DBNull
                        MySqlCommand cmdPartsCmd = new MySqlCommand(requettePartsCmd, Role.ConnexionClient);
                        cmdPartsCmd.Parameters.AddWithValue("@IdPlat", plat.IdPlat);
                        partsCommandees = Convert.ToInt32(cmdPartsCmd.ExecuteScalar());

                        partsDispo = plat.PartsInitiales - partsCommandees;
                        // Afficher seulement si des parts sont disponibles
                        if (partsDispo > 0)
                        {
                            Console.WriteLine(plat.IdPlat + "\t" + plat.Nom + "\t\t" + plat.Prix + " euros\t\t" + plat.Type + "\t\t" + plat.Nationalite + "\t" + partsDispo + "\t\t" + plat.Pseudo);
                            idsEtPartsPlats.Add(Tuple.Create((int)plat.IdPlat, partsDispo));
                        }
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine("Erreur générale lors de la récupération des plats : " + ex.Message);
                        return null; // Indique une erreur
                    }
                }
                Console.WriteLine("---------------------------------------------------------------------------------------------------------");

            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur SQL lors de la récupération des plats disponibles : " + ex.Message);            
                return null; // Indique une erreur
            }
            
            
            try
            {
                Role.ConnexionClient.Close();
            }
            catch (Exception ex)
            {

            }
           
            return idsEtPartsPlats; 
        }


        /// <summary>
        /// Récupère l'ID du cuisinier qui propose un plat donné.
        /// </summary>
        /// <param name="idPlat">ID du plat.</param>
        /// <returns>ID du cuisinier, ou -1 si non trouvé ou erreur.</returns>
        private static int RecupererCuisinierIdDePlatId(int idPlat)
        {
            int cuisinierId = -1;
            try
            {
                Role.ConnexionClient.Open();

                try
                {
                    string query = "SELECT Identifiant FROM Plat_Propose WHERE Id_Plat = @IdPlat";
                    MySqlCommand cmd = new MySqlCommand(query, Role.ConnexionClient);
                    cmd.Parameters.AddWithValue("@IdPlat", idPlat);
                    cuisinierId = Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Erreur " + idPlat + ": " + ex.Message);
                }

            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur lors de la connexion : " + ex.Message);

            }
            try
            {
                Role.ConnexionClient.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur lors de la déconnexion : " + ex.Message);
            }
            return cuisinierId;
        }





        /// <summary>
        /// Insère une nouvelle commande dans laBDD.
        /// </summary>
        /// <param name="idPlat">ID du plat commandé.</param>
        /// <param name="nbParts">Nombre de parts commandées.</param>
        private static void InsererCommande(int idPlat, int nbParts)
        {
            try
            {
                Role.ConnexionClient.Open();
                try
                {
                    string requeteInsertion = @"
                    INSERT INTO Commande (Id_Plat, Identifiant, Etat_de_la_commande, Nb_de_part)
                    VALUES (@IdPlat, @IdClient, FALSE, @NbParts)";

                    MySqlCommand cmd = new MySqlCommand(requeteInsertion, Role.ConnexionClient);
                    cmd.Parameters.AddWithValue("@IdPlat", idPlat);
                    cmd.Parameters.AddWithValue("@IdClient", Connexion.Identifiant);
                    cmd.Parameters.AddWithValue("@NbParts", nbParts);
                    cmd.ExecuteNonQuery();
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Erreur SQL lors de l'insertion de la commande : " + ex.Message);
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur lors de la connexion : " + ex.Message);

            }
            try
            {
                Role.ConnexionClient.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur lors de la déconnexion : " + ex.Message);
            }
        }



















        /// <summary>
        /// Met à jour les compteurs pour le Client connecté.
        /// </summary>
        /// <param name="incrementer">True pour incrémenter (nouvelle commande), False pour décrémenter (annulation).</param>
        private static void UpdateCompteursClient(bool incrementer)
        {
            try
            {
                Role.ConnexionClient.Open();
                try
                {
                    string requeteUpdate;
                    if (incrementer)
                    {
                        requeteUpdate = @"UPDATE Client
                                      SET Nb_Commande_Total = Nb_Commande_Total + 1,
                                          Nb_De_Commande_En_Cours = Nb_De_Commande_En_Cours + 1
                                      WHERE Identifiant = @IdClient";
                    }
                    else // Pour la suppresion d'une commande annulée
                    {
                        requeteUpdate = @"UPDATE Client
                                      SET Nb_Commande_Total = Nb_Commande_Total - 1,
                                          Nb_De_Commande_En_Cours = Nb_De_Commande_En_Cours - 1
                                      WHERE Identifiant = @IdClient";
                    }

                    MySqlCommand cmd = new MySqlCommand(requeteUpdate, Role.ConnexionClient);
                    cmd.Parameters.AddWithValue("@IdClient", Connexion.Identifiant);
                    cmd.ExecuteNonQuery();
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Erreur SQL UpdateCompteursClient pour ID " + Connexion.Identifiant + ": " + ex.Message);
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur lors de la connexion : " + ex.Message);
            }
            try
            {
                Role.ConnexionClient.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur lors de la fermeture de la connexion: " + ex.Message);
            }
        }





        /// <summary>
        /// Met à jour les compteurs pour un Cuisinier donné.
        /// </summary>
        /// <param name="idCuisinier">ID du cuisinier concerné.</param>
        /// <param name="incrementer">True pour incrémenter (nouvelle commande), False pour décrémenter (annulation).</param>
        private static void UpdateCompteursCuisinier(int idCuisinier, bool incrementer)
        {
            try
            {
                Role.ConnexionClient.Open();
                try
                {
                    string requeteUpdate;
                    if (incrementer)
                    {
                        requeteUpdate = @"UPDATE Cuisinier
                                      SET Nb_Total_De_Commande = Nb_Total_De_Commande + 1,
                                          Nb_De_Commande_En_Cours = Nb_De_Commande_En_Cours + 1
                                      WHERE Identifiant = @IdCuisinier";
                    }
                    else // Pour la suppresion d'une commande annulée
                    {
                        requeteUpdate = @"UPDATE Cuisinier
                                      SET Nb_Total_De_Commande = Nb_Total_De_Commande - 1,
                                          Nb_De_Commande_En_Cours = Nb_De_Commande_En_Cours - 1
                                      WHERE Identifiant = @IdCuisinier";
                    }

                    MySqlCommand cmd = new MySqlCommand(requeteUpdate, Role.ConnexionClient);
                    cmd.Parameters.AddWithValue("@IdCuisinier", idCuisinier);
                    cmd.ExecuteNonQuery();
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Erreur SQL UpdateCompteursCuisinier pour ID " + idCuisinier + ": " + ex.Message);
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur lors de la connexion : " + ex.Message);
            }
            try
            {
                Role.ConnexionClient.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur lors de la fermeture de la connexion: " + ex.Message);
            }
        }
















        





















































        /// <summary>
        /// Menu pour gérer les commandes existantes du client.
        /// </summary>
        public static void GererCommandes()
        {
            char choix = '0';
            bool quitter = false;

            do
            {
                Console.Clear();
                Console.WriteLine("--- Gestion de vos Commandes ---");
                Console.WriteLine();
                Console.WriteLine("1. Voir mes commandes");
                Console.WriteLine("2. Annuler une commande ('En cours' uniquement)");
                Console.WriteLine("3. Retour au menu principal");
                Console.WriteLine();
                Console.Write("Votre choix : ");

                choix = Console.ReadKey().KeyChar;
                Console.WriteLine(); 

                switch (choix)
                {
                    case '1':
                        AfficherMesCommandes(); 
                        Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        break;
                    case '2':
                        AnnulerCommande(); 
                        break;
                    case '3':
                        quitter = true; 
                        break;
                    default:
                        Console.WriteLine("Choix invalide.");
                        Thread.Sleep(700); 
                        break;
                }
            } while (!quitter);
        }

        /// <summary>
        /// Affiche les commandes passées par le client connecté, incluant le nombre de parts.
        /// </summary>
        private static void AfficherMesCommandes()
        {
            Console.Clear();
            Console.WriteLine("\n--- Vos Commandes ---");
            MySqlDataReader reader = null;
            try
            {
                Role.ConnexionClient.Open();
                try
                {
                    string requette = @"
                    SELECT c.Id_Commande, p.Nom AS NomPlat, c.Nb_de_part, c.Etat_de_la_commande
                    FROM Commande c
                    JOIN Plat_Propose p ON c.Id_Plat = p.Id_Plat
                    WHERE c.Identifiant = @IdClient
                    ORDER BY c.Id_Commande DESC";

                    MySqlCommand cmd = new MySqlCommand(requette, Role.ConnexionClient);
                    cmd.Parameters.AddWithValue("@IdClient", Connexion.Identifiant);
                    reader = cmd.ExecuteReader();

                    if (!reader.HasRows)
                    {
                        Console.WriteLine("Vous n'avez passé aucune commande pour le moment.");
                        Role.ConnexionClient.Close();
                        return;
                    }


                    Console.WriteLine("ID Cmd\tPlat Commandé\t\tParts\tEtat");
                    Console.WriteLine("------------------------------------------------------------");

                    int id = 0;
                    string nomPlat = "";
                    int nbParts = 0;
                    bool etatBool = false;
                    string etatString = "";
                    while (reader.Read())
                    {
                        id = reader.GetInt32("Id_Commande");
                        nomPlat = reader.GetString("NomPlat");
                        nbParts = reader.GetInt32("Nb_de_part");
                        etatBool = reader.GetBoolean("Etat_de_la_commande");
                        if (etatBool)
                        {
                            etatString = "Livrée";
                        }
                        else
                        {
                            etatString = "En cours";
                        }

                        Console.WriteLine(id + "\t" + nomPlat + "\t\t" + nbParts + "\t" + etatString);
                    }
                    Console.WriteLine("------------------------------------------------------------");


                    try
                    {
                        reader.Close();
                    }
                    catch
                    {

                    }
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Erreur SQL lors de l'affichage de vos commandes : " + ex.Message);
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur SQL lors de la connexion : " + ex.Message);
            }
            
            try
            {
                Role.ConnexionClient.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur SQL lors de la deconnexion : " + ex.Message);

            }
        }



        /// <summary>
        /// Permet au client d'annuler une de ses commandes si elle est encore "En cours".
        /// Supprime la commande et met à jour les compteurs client/cuisinier.
        /// </summary>
        private static void AnnulerCommande()
        {
            Console.Clear();
            Console.WriteLine("--- Annulation d'une commande ---");

            //Afficher les commandes annulables (celles avec Etat_de_la_commande = FALSE)
            List<int> idsCommandesAnnulables = AfficherCommandesAnnulables();

            // Si la liste est nulle (erreur) ou vide (aucune commande annulable)
            if (idsCommandesAnnulables == null || idsCommandesAnnulables.Count == 0)
            {
                Console.WriteLine("\nAppuyez sur une touche pour revenir...");
                Console.ReadKey();
                return;
            }

            // Demander l'id de la commande à annuler
            int idCommandeAnnuler = 0;
            bool idValide = false;
            do
            {
                Console.Write("\nEntrez l'ID de la commande 'En cours' à annuler (ou Q pour quitter) : ");
                string saisie = Console.ReadLine().Trim();

                if (saisie.ToUpper() == "Q")
                    return; 

                if (!int.TryParse(saisie, out idCommandeAnnuler))
                {
                    Console.WriteLine("Veuillez entrer un ID numérique valide.");
                }
                // Vérifier si l'ID saisi est dans la liste des commandes annulables affichées
                else if (!idsCommandesAnnulables.Contains(idCommandeAnnuler))
                {
                    Console.WriteLine("Cet ID ne correspond pas à une de vos commandes 'En cours' annulable.");
                }
                else
                {
                    idValide = true; // L'ID est valide et correspond à une commande annulable
                }
            } while (!idValide);

            // Confirmation
            Console.Write("Confirmez-vous l'annulation de la commande " + idCommandeAnnuler + " ? (O/N) : ");
            char conf = Console.ReadKey().KeyChar;
            Console.WriteLine(); 
            if (conf != 'O' && conf != 'o')
            {
                Console.WriteLine("Annulation abandonnée.");
                Console.WriteLine("Appuyez sur une touche...");
                Console.ReadKey();
                return; 
            }




            //  Récupérer l'ID du Cuisinier avant de supprimer la commande
            int idCuisinier = RecupererCuisinierIdDePlatId(idCommandeAnnuler);
            try
            {
                Role.ConnexionClient.Open();
                try
                {
                    //  Supprimer la commande si elle est bien 'En cours' (Etat_de_la_commande = FALSE)
                    string queryDelete = "DELETE FROM Commande WHERE Id_Commande = @IdCmd AND Identifiant = @IdClient AND Etat_de_la_commande = FALSE";
                    MySqlCommand cmdDelete = new MySqlCommand(queryDelete, Role.ConnexionClient);
                    cmdDelete.Parameters.AddWithValue("@IdCmd", idCommandeAnnuler);
                    cmdDelete.Parameters.AddWithValue("@IdClient", Connexion.Identifiant);
                    cmdDelete.ExecuteNonQuery();

                    Console.WriteLine("Commande " + idCommandeAnnuler + " supprimée...");
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Erreur SQL lors de l'annulation de la commande : " + ex.Message);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de la connexion : " + ex.Message);           
            }
            try
            {
                Role.ConnexionClient.Close();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Erreur lors de la deconnexion : " + ex.Message);
            }


            // Mettre à jour les compteurs (Client et Cuisinier)
            UpdateCompteursClient(false); // false = décrémenter
            UpdateCompteursCuisinier(idCuisinier, false); // false = décrémenter

            Console.WriteLine("\nAppuyez sur une touche pour revenir...");
            Console.ReadKey();
        }


        /// <summary>
        /// Affiche les commandes "En cours" (annulables) du client connecté et retourne leurs IDs.
        /// </summary>
        /// <returns>Liste des IDs des commandes annulables, ou null en cas d'erreur.</returns>
        private static List<int> AfficherCommandesAnnulables()
        {
            List<int> ids = new List<int>(); //permet de récuper l'id des commandes annulablent
            Console.WriteLine("\n--- Commandes 'En cours' pouvant être annulées ---");

            MySqlDataReader reader = null;
            try
            {
                Role.ConnexionClient.Open();
                try
                {
                    // Sélectionne les commandes du client où Etat_de_la_commande est FALSE
                    string query = @"
                     SELECT c.Id_Commande, p.Nom AS NomPlat, c.Nb_de_part
                     FROM Commande c
                     JOIN Plat_Propose p ON c.Id_Plat = p.Id_Plat
                     WHERE c.Identifiant = @IdClient AND c.Etat_de_la_commande = FALSE 
                     ORDER BY c.Id_Commande DESC";
                    MySqlCommand cmd = new MySqlCommand(query, Role.ConnexionClient);
                    cmd.Parameters.AddWithValue("@IdClient", Connexion.Identifiant);
                    reader = cmd.ExecuteReader();

                    if (!reader.HasRows)
                    {
                        Console.WriteLine("Vous n'avez aucune commande 'En cours' actuellement.");
                        reader.Close();
                        Role.ConnexionClient.Close();
                        return ids; // Retourne liste vide
                    }


                    Console.WriteLine("ID Cmd\tPlat Commandé\t\tParts");
                    Console.WriteLine("-------------------------------------------------");

                    int id = 0;
                    string nomPlat = "";
                    int nbParts = 0;
                    while (reader.Read())
                    {
                        id = reader.GetInt32("Id_Commande");
                        nomPlat = reader.GetString("NomPlat");
                        nbParts = reader.GetInt32("Nb_de_part");

                        Console.WriteLine(id + "\t" + nomPlat + "\t\t" + nbParts);
                        ids.Add(id); // Ajouter l'ID à la liste retournée
                    }
                    Console.WriteLine("-------------------------------------------------");

                    try
                    {
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Erreur SQL lors de l'affichage des commandes annulables : " + ex.Message);
                    return null; // Indique une erreur
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur lors de la connexion : " + ex.Message);
                return null; // Indique une erreur
            }
            
            try
            {
                Role.ConnexionClient.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de la deconnexion : " + ex.Message);
            }
            return ids; // Retourne la liste des IDs
        }













































        /// <summary>
        /// Permet au client de noter un cuisinier pour une commande livrée. 
        /// </summary>
        public static void NoterCuisinier()
        {
            Console.Clear();
            Console.WriteLine("--- Noter un Cuisinier ---");

            // Afficher les cuisiniers distincts des commandes livrées du client
            List<(int OrderId, string NomPlat, int ChefId, string ChefPseudo)> commandesLivrees = InfoCuisiniersCommandesLivrees();


            if ( commandesLivrees == null || commandesLivrees.Count == 0) // Aucune commande livrée trouvée
            {
                Console.WriteLine("Vous n'avez pas de commande livrée à noter pour le moment.");
                Console.WriteLine("Appuyez sur une touche pour revenir...");
                Console.ReadKey();
                return;
            }



            // Choisir le cuisinier à noter
            int idCommandeSelectionnee = 0;
            bool commandeValide = false;
            int idCuisinierNote = -1;
            string pseudoCuisinier = "";
            string nomPlatCommande = "";
            do
            {
                Console.Write("\nEntrez l'ID de la commande que vous souhaitez noter (ou Q pour quitter) : ");
                string saisie = Console.ReadLine().Trim();
                if (saisie.ToUpper() == "Q")
                {
                    return;
                }
                if (!int.TryParse(saisie, out idCommandeSelectionnee))
                {
                    Console.WriteLine("Veuillez entrer un ID numérique valide.");
                }
                else
                {
                    bool trouvée = false;
                    foreach ((int OrderId, string NomPlat, int ChefId, string ChefPseudo) commande in commandesLivrees)
                    {
                        if (commande.OrderId == idCommandeSelectionnee)
                        {
                            trouvée = true;
                            idCuisinierNote = commande.ChefId;
                            pseudoCuisinier = commande.ChefPseudo;
                            nomPlatCommande = commande.NomPlat;
                            break;
                        }
                    }
                    if (!trouvée)
                    {
                        Console.WriteLine("Cet ID ne correspond à aucune commande livrée.");
                    }
                    else
                    {
                        commandeValide = true;
                    }
                }
            } while (!commandeValide);

            // Saisir la note (0 à 5)
            int note = 0;
            bool noteValide = false;
            do
            {
                Console.Write("\n\nEntrez votre note pour le chef '"+pseudoCuisinier+"' (pour la commande de "+nomPlatCommande+") (0 à 5) : ");
                char noteChar = Console.ReadKey().KeyChar;
                Console.WriteLine();

                if (!int.TryParse(noteChar.ToString(), out note) || note < 0 || note > 5)
                {
                    Console.WriteLine("Veuillez entrer un nombre entre 0 et 5.\n");
                }
                else
                {
                    noteValide = true;
                }
            } while (!noteValide);


            // Saisir le commentaire
            Console.Write("Laissez un commentaire (facultatif, appuyez sur Entrée pour passer) : ");
            string commentaire = Console.ReadLine();


            // Insérer la notation dans la table Notation_Cuisinier
            // Passe l'ID du cuisinier noté, la note, et le commentaire
            InsererNotationChef(idCuisinierNote, idCommandeSelectionnee, note, commentaire);

            Console.WriteLine("\nMerci pour votre notation !");

            Console.WriteLine("\nAppuyez sur une touche pour revenir au menu...");
            Console.ReadKey();
        }


        /// <summary>
        /// Récupère la liste des commadnes effectuées par les cuisiniers (ID et Pseudo) dont le client connecté
        /// a reçu au moins une commande (Etat_de_la_commande = TRUE).
        /// </summary>
        /// <returns> List<(int CommandeId, string NomPlat, int ChefId, string ChefPseudo)> ou null en cas d'erreur.</returns>
        private static List<(int CommandeId, string NomPlat, int ChefId, string ChefPseudo)> InfoCuisiniersCommandesLivrees()
        {
            List<(int CommandeId, string NomPlat, int ChefId, string ChefPseudo)> commandesLivrees = new List<(int, string, int, string)>();
            MySqlDataReader reader = null;
            try
            {
                Role.ConnexionClient.Open();
                try
                {
                    string requette = @"
                        SELECT c.Id_Commande, p.Nom AS NomPlat, p.Identifiant AS ChefId, u.Pseudo AS ChefPseudo
                        FROM Commande c
                        JOIN Plat_Propose p ON c.Id_Plat = p.Id_Plat
                         JOIN Utilisateur u ON p.Identifiant = u.Identifiant
                        WHERE c.Identifiant = @IdClient AND c.Etat_de_la_commande = TRUE
                        ORDER BY c.Id_Commande DESC";
                    MySqlCommand cmd = new MySqlCommand(requette, Role.ConnexionClient);
                    cmd.Parameters.AddWithValue("@IdClient", Connexion.Identifiant);
                    reader = cmd.ExecuteReader();

                    if (!reader.HasRows)
                    {
                        return null;
                    }

                    Console.WriteLine("ID Cmd\tPlat Commandé\tChef ID\tChef Pseudo");
                    Console.WriteLine("-------------------------------------------------------------");
                    int CommandeId = 0;
                    string nomPlat = "";
                    int chefId = 0;
                    string chefPseudo = "";
                    while (reader.Read())
                    {
                        CommandeId = reader.GetInt32("Id_Commande");
                        nomPlat = reader.GetString("NomPlat");
                        chefId = reader.GetInt32("ChefId");
                        chefPseudo = reader.GetString("ChefPseudo");
                        commandesLivrees.Add((CommandeId, nomPlat, chefId, chefPseudo));

                        Console.WriteLine(CommandeId + "\t" + nomPlat + "\t" + chefId + "\t" + chefPseudo);
                    }
                    Console.WriteLine("-------------------------------------------------------------");

                    reader.Close();
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Erreur SQL InfoCuisiniersCommandesLivrees: " + ex.Message);
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur lors de la connexion : " + ex.Message);
            }
            try
            {
                Role.ConnexionClient.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur lors de la deconnexion : " + ex.Message);
            }
            return commandesLivrees; 
        }

        /// <summary>
        /// Insère une nouvelle notation pour un cuisinier (donnée par le client connecté).
        /// </summary>
        /// <param name="idCuisinierNote">ID du cuisinier qui est noté.</param>
        /// <param name="note">La note attribuée (0-5).</param>
        /// <param name="commentaire">Le commentaire laissé.</param>
        private static void InsererNotationChef(int idCuisinierNote, int Id_Commande, int note, string commentaire)
        {
            try
            {
                Role.ConnexionClient.Open();
                try
                {
                    string requette = @"INSERT INTO Notation_Cuisinier 
                               (Notation, Commentaire, DateNotation, Id_Commande, Id_Client, Identifiant)
                               VALUES (@Notation, @Commentaire, @DateNotation, @Id_Commande, @Id_Client, @Identifiant);";
                    MySqlCommand cmd = new MySqlCommand(requette, Role.ConnexionClient);
                    cmd.Parameters.AddWithValue("@Notation", note);
                    cmd.Parameters.AddWithValue("@Commentaire", commentaire);
                    cmd.Parameters.AddWithValue("@DateNotation", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Id_Commande", Id_Commande);
                    cmd.Parameters.AddWithValue("@Id_Client", Connexion.Identifiant);
                    cmd.Parameters.AddWithValue("@Identifiant", idCuisinierNote);
                    cmd.ExecuteNonQuery();

                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Erreur SQL lors de l'insertion de la notation : " + ex.Message);
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur lors dde la connexion : " + ex.Message);       
            }
            try
            {
                Role.ConnexionClient.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur lors de la deconnexion : " + ex.Message);
            }
        }



































































        public static void AfficherProfilClient()
        {
            Console.Clear();
            Console.WriteLine("=== Votre Profil ===");

            try
            {
                Role.ConnexionClient.Open();

                try
                {
                    // Récupération des sats dans la table Cuisinier
                    string requette = @"SELECT Nb_Commande_Total, Nb_De_Commande_En_Cours 
                                      FROM Client 
                                      WHERE Identifiant = @id;";
                    MySqlCommand cmdProfil = new MySqlCommand(requette, Role.ConnexionClient);
                    cmdProfil.Parameters.AddWithValue("@id", Connexion.Identifiant);
                    MySqlDataReader readerProfil = cmdProfil.ExecuteReader();

                    if (readerProfil.Read())
                    {
                        int nbTotalCommandes = readerProfil.GetInt32("Nb_Commande_Total");
                        int nbCommandesEnCours = readerProfil.GetInt32("Nb_De_Commande_En_Cours");

                        Console.WriteLine("Nombre total de commandes réalisées : " + nbTotalCommandes);
                        Console.WriteLine("Nombre de commandes en cours (non livrées) : " + nbCommandesEnCours);
                    }
                    else
                    {
                        Console.WriteLine("Profil non trouvé.");
                    }
                    readerProfil.Close();
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Erreur lors de l'affichage du profil : " + ex.Message);
                    Console.ReadKey();
                }

                int nombreAvis = 0;
                try
                {
                    string requetteNombreAvis = "SELECT COUNT(*) FROM Notation_Client WHERE Identifiant = @id;";
                    MySqlCommand cmdNombreAvis = new MySqlCommand(requetteNombreAvis, Role.ConnexionClient);
                    cmdNombreAvis.Parameters.AddWithValue("@id", Connexion.Identifiant);
                    nombreAvis = Convert.ToInt32(cmdNombreAvis.ExecuteScalar());
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Erreur lors du calcul du nombre d'avis: " + ex.Message);
                    Console.ReadKey();
                }

                if (nombreAvis == 0)
                {
                    Console.WriteLine("Aucun avis reçu pour le moment.");
                }
                else
                {
                    try
                    {
                        // Calcul de la moyenne des notes
                        string requette = "SELECT AVG(Notation) AS Moyenne FROM Notation_Client WHERE Identifiant = @id;";
                        MySqlCommand cmdMoy = new MySqlCommand(requette, Role.ConnexionClient);
                        cmdMoy.Parameters.AddWithValue("@id", Connexion.Identifiant);
                        double moyenne = Convert.ToDouble(cmdMoy.ExecuteScalar());
                        Console.WriteLine("\nMoyenne des notes reçues : " + moyenne.ToString("0.00")); // permet d'arrondir la moyenne à 2 chiffres après la virgule

                        // Affichage des avis laissés par les Cuisiniers
                        string queryAvis = @"SELECT nc.Notation, nc.Commentaire, nc.DateNotation, 
                                    p.Nom AS NomPlat, u.Pseudo AS PseudoCuisinier
                                     FROM Notation_Client nc
                                     JOIN Commande c ON nc.Id_Commande = c.Id_Commande
                                     JOIN Plat_Propose p ON c.Id_Plat = p.Id_Plat
                                     JOIN Utilisateur u ON nc.Id_Cuisinier = u.Identifiant
                                     WHERE nc.Identifiant = @id
                                     ORDER BY nc.DateNotation DESC;";
                        MySqlCommand cmdAvis = new MySqlCommand(queryAvis, Role.ConnexionClient);
                        cmdAvis.Parameters.AddWithValue("@id", Connexion.Identifiant);
                        MySqlDataReader readerAvis = cmdAvis.ExecuteReader();

                        Console.WriteLine("\n=== Avis laissés par les cuisiniers ===");
                        if (readerAvis.HasRows)
                        {
                            while (readerAvis.Read())
                            {
                                int note = readerAvis.GetInt32("Notation");
                                string commentaire = readerAvis.GetString("Commentaire");
                                DateTime dateNotation = readerAvis.GetDateTime("DateNotation");
                                string nomPlat = readerAvis.GetString("NomPlat");
                                string pseudoCuisinier = readerAvis.GetString("PseudoCuisinier");

                                Console.WriteLine("\nDate: " + dateNotation.ToString("dd/MM/yyyy"));
                                Console.WriteLine("Note: " + note);
                                Console.WriteLine("Plat: " + nomPlat);
                                Console.WriteLine("Cuisiniers: " + pseudoCuisinier);
                                if (!string.IsNullOrEmpty(commentaire))
                                    Console.WriteLine("Commentaire: " + commentaire);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Aucun avis n'a été trouvé.");
                        }
                        readerAvis.Close();
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine("Erreur lors de l'affichage des avis : " + ex.Message);
                        Console.ReadKey();
                    }
                }

            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur lors de l'affichage du profil : " + ex.Message);
                Console.ReadKey();
            }
            try
            {
                Role.ConnexionClient.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur lors de la fermeture de la connexion : " + ex.Message);
                Console.ReadKey();
            }

            Console.WriteLine("\nAppuyez sur une touche pour revenir au menu...");
            Console.ReadKey();
        }

    } 
} 



