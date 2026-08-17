using System;
using Visual_Studio;
using System.Collections.Generic;
using System.IO;
using Visual_Studio;
using MySql.Data.MySqlClient;
using System.Text.Json.Serialization.Metadata;
using Mysqlx.Crud;

namespace Visual_Studio
{
    public class Peuplement
    {


        /// <summary>
        /// Exécute le processus de peuplement de la base de données.
        /// </summary>
        public static void ExecuterPeuplementComplet()
        {

            try
            {
                Role.ConnexionAdmin.Open();// Ouverture de la connexion à la base de données
                try
                {
                    // Peuplement des utilisateurs
                    PeuplerUtilisateursBase();

                    // Peuplement des données
                    PeuplerPlatsEtCommandes();
                    PeuplerNotations();
                    PeuplementRegimes();

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur lors du peuplement : " + ex.Message);
                }
            }
            catch (MySqlException ex)
            {
                //Console.WriteLine("Erreur de connexion à la base de données : " + ex.Message);
            }
            try
            {
                Role.ConnexionAdmin.Close(); // Fermeture de la connexion
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur de fermeture de la connexion : " + ex.Message);
            }
        }



        /// <summary>
        /// Insère des utilisateurs et leurs adresses associées dans la base de données.
        /// S'il y a des erreurs, c'est normal, car les utilisateurs sont déjà présents dans la base de données mais on tente des les ajouter comme même au cas où... Même si on sait que la database existe déjà mais on est pas sûr qu'elle soit peupler, alors on force le peuplement
        /// </summary>
        private static void PeuplerUtilisateursBase()
        {
            try
            {
                int idAdresseAdmin = ExecuterRequeteAvecId(
                    "INSERT INTO Adresse (Numero_de_rue, Rue, Ville, Code_Postale, Metro_le_plus_proche) " +
                    "VALUES ('1','Admin rue','Admin ville','74000','Chatelet 1');");

                int idUserAdmin = ExecuterRequeteAvecId(
                    "INSERT INTO Utilisateur (Nom, Prenom, Pseudo, Email, Telephone, Mot_De_Passe, Id_adresse, Entreprise) " +
                    "VALUES ('Admin Nom', 'Admin Prenom', 'Admin', 'admin@', '0000000000', 'admin', " + idAdresseAdmin + ", false);");

                // Adresse 1
                int idAdresse1 = ExecuterRequeteAvecId(
                    "INSERT INTO Adresse (Numero_de_rue, Rue, Ville, Code_Postale, Metro_le_plus_proche) " +
                    "VALUES (18, 'Rue des Lilas', 'Paris', 75020, 'Gambetta 3bis');");

                // Utilisateur 1
                int idUser1 = ExecuterRequeteAvecId(
                    "INSERT INTO Utilisateur (Nom, Prenom, Pseudo, Email, Telephone, Mot_De_Passe, Id_adresse, Entreprise) " +
                    "VALUES ('Martin', 'Pierre', 'ChefPierre', 'pierre@gmail.com', '0612345678', 'pierre', " + idAdresse1 + ", false);");

                ExecuterRequete(
                    "INSERT INTO Cuisinier (Identifiant, Nb_Total_De_Plat, Nb_De_Plat_En_Cours, Nb_Total_De_Commande, Nb_De_Commande_En_Cours) " +
                    "VALUES (" + idUser1 + ", 0, 0, 0, 0);");
                ExecuterRequete(
                    "INSERT INTO Client (Identifiant, Nb_Commande_Total, Nb_De_Commande_En_Cours) " +
                    "VALUES (" + idUser1 + ", 0, 0);");
                // Adresse 2
                int idAdresse2 = ExecuterRequeteAvecId(
                    "INSERT INTO Adresse (Numero_de_rue, Rue, Ville, Code_Postale, Metro_le_plus_proche) " +
                    "VALUES (5, 'Avenue Mozart', 'Paris', 75016, 'La Motte-Picquet - Grenelle 10');");

                // Utilisateur 2
                int idUser2 = ExecuterRequeteAvecId(
                    "INSERT INTO Utilisateur (Nom, Prenom, Pseudo, Email, Telephone, Mot_De_Passe, Id_adresse, Entreprise) " +
                    "VALUES ('Dubois', 'Marie', 'Mariedo', 'marie@gmail.com', '0698765432', 'marie', " + idAdresse2 + ", false);");

                ExecuterRequete("INSERT INTO Cuisinier (Identifiant, Nb_Total_De_Plat, Nb_De_Plat_En_Cours, Nb_Total_De_Commande, Nb_De_Commande_En_Cours) " +
                    "VALUES (" + idUser2 + ", 0, 0, 0, 0);");
                ExecuterRequete("INSERT INTO Client (Identifiant, Nb_Commande_Total, Nb_De_Commande_En_Cours) " +
                    "VALUES (" + idUser2 + ", 0, 0);");
                
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur création utilisateurs : " + ex.Message);
                Console.ReadKey();
            }
        }



        int cuisinierIdPierre = 2;
        int clientIdMarie = 3;
        /// <summary>
        /// Insère des plats et leurs commandes associées dans la base de données.
        /// </summary>
        /// <summary>
        /// Insère des plats, les lie aux cuisiniers via Propose, et crée des commandes associées.
        /// </summary>
        private static void PeuplerPlatsEtCommandes()
        {
            try
            {
                int cuisinierIdPierre = 2; 
                int clientIdMarie = 3;    

                // Plat 1
                int idPlat1 = ExecuterRequeteAvecId(
                    "INSERT INTO Plat_Propose (Nom, Type, Variante_de_recette_bool, Nationalite, Nb_De_Client, Prix_par_personne, Recette_autorise, Identifiant, Date_de_fabrication, Date_de_peremption) " +
                    "VALUES ('Pizza Royale', 'Plat principal', False, 'Italienne', 8, 3, 'Pizza', " + cuisinierIdPierre + ", NOW(), DATE_ADD(NOW(), INTERVAL 4 DAY));"
                );
                if (idPlat1 > 0)
                {
                    ExecuterRequete(
                        "INSERT INTO Ingredient_Total (Nom_Ingredient, Quantite_, Unite, Unite_autorise, Id_Plat, Nom_Ingredient_autorise) " +
                        "VALUES ('Farine', 300, 'g', 'g', " + idPlat1 + ", 'farine')," +
                               "('Tomate', 200, 'g', 'g', " + idPlat1 + ", 'tomate');" 
                    );
                }
                else { Console.WriteLine("  ERREUR: Échec création Plat 1 (Pizza)"); }



                //  Plat 2 
                int idPlat2 = ExecuterRequeteAvecId(
                    "INSERT INTO Plat_Propose (Nom, Type, Variante_de_recette_bool, Nationalite, Nb_De_Client, Prix_par_personne, Recette_autorise, Identifiant, Date_de_fabrication, Date_de_peremption) " +
                    "VALUES ('Salade César', 'Entrée', False, 'Française', 5, 12, 'Salade', " + cuisinierIdPierre + ", NOW(), DATE_ADD(NOW(), INTERVAL 2 DAY));"
                );

                if (idPlat2 > 0)
                {
                    ExecuterRequete(
                        "INSERT INTO Ingredient_Total (Nom_Ingredient, Quantite_, Unite, Unite_autorise, Id_Plat, Nom_Ingredient_autorise) " +
                        "VALUES ('Salade', 100, 'g', 'g', " + idPlat2 + ", 'salade')," + 
                               "('Poulet', 150, 'g', 'g', " + idPlat2 + ", 'poulet');" 
                    );
                }
                else { Console.WriteLine("  ERREUR: Échec création Plat 2 (Salade)"); }

                //  Plat 3 
                int idPlat3 = ExecuterRequeteAvecId(
                    "INSERT INTO Plat_Propose (Nom, Type, Variante_de_recette_bool, Nationalite, Nb_De_Client, Prix_par_personne, Recette_autorise, Identifiant, Date_de_fabrication, Date_de_peremption) " +
                    "VALUES ('Burger Maison', 'Plat principal', True, 'Américaine', 1, 15, 'Burger', " + cuisinierIdPierre + ", NOW(), DATE_ADD(NOW(), INTERVAL 3 DAY));" 
                );

                if (idPlat3 > 0)
                {
                    // Ajouter ingrédients pour Plat 3
                    ExecuterRequete(
                         "INSERT INTO Ingredient_Total (Nom_Ingredient, Quantite_, Unite, Unite_autorise, Id_Plat, Nom_Ingredient_autorise) " +
                         "VALUES ('Fromage', 1, 'kg', 'kg', " + idPlat3 + ", 'fromage')," + 
                                "('Salade', 10, 'g', 'g', " + idPlat3 + ", 'salade');"
                     );
                }
                else { Console.WriteLine("  ERREUR: Échec création Plat 3 (Burger)"); }


                //  Mise à jour compteur Cuisinier Pierre (ID 1) pour les 3 plats
                if (idPlat1 > 0 || idPlat2 > 0 || idPlat3 > 0)
                {
                    ExecuterRequete("UPDATE Cuisinier SET Nb_Total_De_Plat = Nb_Total_De_Plat + 3, Nb_De_Plat_En_Cours = Nb_De_Plat_En_Cours + 3 WHERE Identifiant = " + cuisinierIdPierre + ";");
                }

                // Création de 2 commandes pour le client Marie (ID 2)
                if (idPlat1 > 0 && idPlat2 > 0)
                {
                    ExecuterRequete(
                        "INSERT INTO Commande(Id_Plat, Identifiant, Etat_de_la_commande, Nb_de_part)" +
                        "VALUES (" + idPlat1+ ","+ clientIdMarie +", TRUE, 4)," +   // true car on laisse un avis plsu tard donc la commadne est terminé
                                "(" + idPlat2+ ", "+ clientIdMarie + ", FALSE, 2);"
                    );


                    ExecuterRequete("UPDATE Client SET Nb_Commande_Total = Nb_Commande_Total + 1, Nb_De_Commande_En_Cours = Nb_De_Commande_En_Cours + 1 WHERE Identifiant = " + clientIdMarie + ";");
                    ExecuterRequete("UPDATE Cuisinier SET Nb_Total_De_Commande = Nb_Total_De_Commande + 1, Nb_De_Commande_En_Cours = Nb_De_Commande_En_Cours + 1 WHERE Identifiant = " + cuisinierIdPierre + ";");

                }
                else
                {
                    Console.WriteLine("Impossible de créer les commandes car les plats associés n'ont pas été créés correctement.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur création plats/commandes : " + ex.Message, ex);
            }
        }




        /// <summary>
        /// Insère des notations des clients et des cuisiniers dans la base de données.
        /// </summary>
        private static void PeuplerNotations()
        {
            int cuisinierIdPierre = 2;
            int clientIdMarie = 3;

            try
            {

                ExecuterRequete(
                    "INSERT INTO Notation_Client (Notation, Commentaire, DateNotation, Id_Commande, Id_Cuisinier, Identifiant) " +
                    "VALUES (4, 'Très bon service', NOW(), 1, 2, 3);");

                ExecuterRequete(
                    "INSERT INTO Notation_Cuisinier (Notation, Commentaire, DateNotation, Id_Commande, Id_Client, Identifiant) " +
                    "VALUES (5, 'Excellente pizza', NOW(), 1, 3, 2);");

            }
            catch (Exception ex)
            {
                throw new Exception("Erreur création notations : " + ex.Message);
            }

        }




        /// <summary>
        /// Associe des régimes alimentaires aux plats dans la base de données.
        /// </summary>
        private static void PeuplementRegimes()
        {
            try
            {
                ExecuterRequete(
                    "INSERT INTO Suit_le_régime (Id_Plat, Nom_Regime) " +
                    "VALUES (1, 'Végétarien'), (2, 'Sans gluten');");
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur association régimes : " + ex.Message);
            }
        }




        /// <summary>
        /// Exécute une requête SQL et retourne l'ID du dernier élément inséré.
        /// </summary>
        /// <param name="requete">Requête SQL à exécuter.</param>
        /// <returns>L'ID du dernier élément inséré.</returns>
        private static int ExecuterRequeteAvecId(string requete)
        {
            MySqlCommand cmdInsert = null;
            MySqlCommand cmdSelectId = null;
            int lastId = -1; 

            try
            {
                cmdInsert = new MySqlCommand(requete, Role.ConnexionAdmin);
                cmdInsert.ExecuteNonQuery();

                cmdSelectId = new MySqlCommand("SELECT LAST_INSERT_ID();", Role.ConnexionAdmin);
                lastId = Convert.ToInt32(cmdSelectId.ExecuteScalar());
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur lors de l'exécution de la requête avec ID : " + ex.Message);
            }
            return lastId;
        }




        /// <summary>
        /// Exécute une requête SQL sans retour de valeur.
        /// </summary>
        /// <param name="requete">Requête SQL à exécuter.</param>
        private static void ExecuterRequete(string requete)
        {
            new MySqlCommand(requete, Role.ConnexionAdmin).ExecuteNonQuery();
        }
    }
}