using System;
using Visual_Studio;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using MySql.Data.MySqlClient;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace Visual_Studio
{

    /// <summary>
    /// Classe de gestion des rôles et utilisateurs de la base de données
    /// </summary>
    public class Role
    {

        /// <summary>
        /// Crée les rôles et utilisateurs avec leurs permissions respectives
        /// </summary>
        public static void CreerRolesEtUtilisateurs()
        {
            string[] commands = new string[]
            {
                // Suppression des rôles existants
                "DROP ROLE IF EXISTS `admin_role`;",
                "DROP ROLE IF EXISTS `creation_profil_role`;",
                "DROP ROLE IF EXISTS `connexion_profil_role`;",
                "DROP ROLE IF EXISTS `cuisinier_role`;",
                "DROP ROLE IF EXISTS `client_role`;",

                // Suppression des utilisateurs existants
                "DROP USER IF EXISTS 'user_admin'@'localhost';",
                "DROP USER IF EXISTS 'user_creation_profil'@'localhost';",
                "DROP USER IF EXISTS 'user_connexion_profil'@'localhost';",
                "DROP USER IF EXISTS 'user_cuisinier'@'localhost';",
                "DROP USER IF EXISTS 'user_client'@'localhost';",

                // Création du rôle administrateur avec tous les privilèges
                "CREATE ROLE `admin_role`;",
                "GRANT ALL PRIVILEGES ON Database_Livin.* TO `admin_role`;",

                // Création du rôle création de profil avec accès aux tables de profils
                "CREATE ROLE `creation_profil_role`;",
                "GRANT SELECT, INSERT, UPDATE, DELETE ON Utilisateur TO `creation_profil_role`;",
                "GRANT SELECT, INSERT, UPDATE, DELETE ON Adresse TO `creation_profil_role`;",
                "GRANT SELECT, INSERT, UPDATE, DELETE ON Cuisinier TO `creation_profil_role`;",
                "GRANT SELECT, INSERT, UPDATE, DELETE ON Client TO `creation_profil_role`;",

                // Création du rôle connexion avec accès lecture à la table Utilisateur
                "CREATE ROLE `connexion_profil_role`;",
                "GRANT SELECT ON Utilisateur TO `connexion_profil_role`;",

                // Création du rôle cuisinier avec accès aux tables métier
                "CREATE ROLE `cuisinier_role`;",
                "GRANT SELECT, INSERT, UPDATE, DELETE ON Plat_Propose TO `cuisinier_role`;",
                "GRANT SELECT, UPDATE ON Commande TO `cuisinier_role`;",
                "GRANT SELECT, UPDATE ON Cuisinier TO `cuisinier_role`;",
                "GRANT INSERT, UPDATE ON Notation_Client TO `cuisinier_role`;",
                "GRANT SELECT, UPDATE ON Utilisateur TO `cuisinier_role`;",
                "GRANT SELECT, UPDATE ON Adresse TO `cuisinier_role`;",
                "GRANT SELECT, INSERT, UPDATE ON notation_cuisinier TO `cuisinier_role`;",
                "GRANT SELECT, INSERT, UPDATE ON suit_le_régime TO `cuisinier_role`;",
                "GRANT SELECT, INSERT, UPDATE ON ingredient TO `cuisinier_role`;",
                "GRANT SELECT ON Recette TO `cuisinier_role`;",
                "GRANT SELECT ON Nationalite TO `cuisinier_role`;",
                "GRANT SELECT ON regime TO `cuisinier_role`;",
                "GRANT SELECT ON Type_de_Preparation TO `cuisinier_role`;",
                "GRANT SELECT ON unite_de_mesure TO `cuisinier_role`;",
                "GRANT SELECT, INSERT, UPDATE ON ingredient_total TO `cuisinier_role`;",
                

                // Création du rôle client avec accès aux fonctionnalités client
                "CREATE ROLE `client_role`;",
                "GRANT SELECT, UPDATE ON Plat_Propose TO `client_role`;",
                "GRANT SELECT, INSERT, DELETE ON Commande TO `client_role`;",
                "GRANT SELECT, UPDATE ON Client TO `client_role`;",
                "GRANT SELECT, UPDATE ON Utilisateur TO `client_role`;",
                "GRANT SELECT, UPDATE ON Adresse TO `client_role`;",
                "GRANT SELECT ON Notation_Client TO `client_role`;",
                "GRANT SELECT, INSERT, UPDATE ON notation_cuisinier TO `client_role`;",
                "GRANT SELECT, INSERT, UPDATE ON suit_le_régime TO `client_role`;",
                "GRANT SELECT, INSERT, UPDATE ON ingredient TO `client_role`;",
                "GRANT SELECT, INSERT, UPDATE ON cuisinier TO `client_role`;",
                
                



                // Création des utilisateurs avec mot de passe
                "CREATE USER 'user_admin'@'localhost' IDENTIFIED BY 'root';",
                "CREATE USER 'user_creation_profil'@'localhost' IDENTIFIED BY 'root';",
                "CREATE USER 'user_connexion_profil'@'localhost' IDENTIFIED BY 'root';",
                "CREATE USER 'user_cuisinier'@'localhost' IDENTIFIED BY 'root';",
                "CREATE USER 'user_client'@'localhost' IDENTIFIED BY 'root';",

                // Attribution des rôles aux utilisateurs
                "GRANT `admin_role` TO 'user_admin'@'localhost';",
                "GRANT `creation_profil_role` TO 'user_creation_profil'@'localhost';",
                "GRANT `connexion_profil_role` TO 'user_connexion_profil'@'localhost';",
                "GRANT `cuisinier_role` TO 'user_cuisinier'@'localhost';",
                "GRANT `client_role` TO 'user_client'@'localhost';",

                // Activation des rôles par défaut
                "SET DEFAULT ROLE ALL TO 'user_admin'@'localhost';",
                "SET DEFAULT ROLE ALL TO 'user_creation_profil'@'localhost';",
                "SET DEFAULT ROLE ALL TO 'user_connexion_profil'@'localhost';",
                "SET DEFAULT ROLE ALL TO 'user_cuisinier'@'localhost';",
                "SET DEFAULT ROLE ALL TO 'user_client'@'localhost';",

            };

            //Execution des commandes
            MySqlConnection maConnexion = null;
            try
            {
                string connexionString = "SERVER=localhost;PORT=3306;" +
                                         "DATABASE=Database_Livin;" +
                                         "UID=root;PASSWORD=root";
                maConnexion = new MySqlConnection(connexionString);
                maConnexion.Open();

                // Exécution de chaque commande SQL
                foreach (string commandText in commands)
                {
                    try
                    {
                        MySqlCommand cmd = maConnexion.CreateCommand();
                        cmd.CommandText = commandText;
                        cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine("Erreur pour la commande  : " + commandText.Trim() + "\n Message d'erreur : " + ex.Message);
                    }
                }
            }
            catch (MySqlException e)
            {
                Console.WriteLine(" ErreurConnexion : " + e.ToString());
                return;
            }
            maConnexion.Close();

            CreerConnexion();
        }




        // Propriétés de connexion pour chaque rôle
        private static MySqlConnection connexion_admin;
        public static MySqlConnection ConnexionAdmin
        {
            get { return connexion_admin; }
        }

        private static MySqlConnection connexion_creation_profil;
        public static MySqlConnection ConnexionCreationProfil
        {
            get { return connexion_creation_profil; }
        }

        private static MySqlConnection connexion_connexion_profil;
        public static MySqlConnection ConnexionConnexion_profil
        {
            get { return connexion_connexion_profil; }
        }

        private static MySqlConnection connexion_cuisinier;
        public static MySqlConnection ConnexionCuisinier
        {
            get { return connexion_cuisinier; }
        }

        private static MySqlConnection connexion_client;
        public static MySqlConnection ConnexionClient
        {
            get { return connexion_client; }
        }


        /// <summary>
        /// Initialise les connexions MySQL pour chaque type d'utilisateur
        /// </summary>
        public static void CreerConnexion()
        {
            try
            {
                // Connexion administrateur
                string connexionStringAdmin = "SERVER=localhost;PORT=3306;DATABASE=Database_Livin;UID=user_admin;PASSWORD=root";
                connexion_admin = new MySqlConnection(connexionStringAdmin);
            }
            catch (MySqlException e)
            {
                Console.WriteLine(" Erreur lors de la création de l'objet de connexion Admin: " + e.ToString());
            }

            try
            {
                // Connexion création de profil
                string connexionStringCreationProfil = "SERVER=localhost;PORT=3306;DATABASE=Database_Livin;UID=user_creation_profil;PASSWORD=root";
                connexion_creation_profil = new MySqlConnection(connexionStringCreationProfil);
            }
            catch (MySqlException e)
            {
                Console.WriteLine(" Erreur lors de la création de l'objet de connexion user_creation_profil: " + e.ToString());
            }

            try
            {
                // Connexion authentification
                string connexionStringConnexionProfil = "SERVER=localhost;PORT=3306;DATABASE=Database_Livin;UID=user_connexion_profil;PASSWORD=root";
                connexion_connexion_profil = new MySqlConnection(connexionStringConnexionProfil);
            }
            catch (MySqlException e)
            {
                Console.WriteLine(" Erreur lors de la création de l'objet de connexion user_connexion_profil: " + e.ToString());
            }

            try
            {
                // Connexion cuisinier
                string connexionStringCuisinier = "SERVER=localhost;PORT=3306;DATABASE=Database_Livin;UID=user_cuisinier;PASSWORD=root";
                connexion_cuisinier = new MySqlConnection(connexionStringCuisinier);
            }
            catch (MySqlException e)
            {
                Console.WriteLine(" Erreur lors de la création de l'objet de connexion user_cuisinier: " + e.ToString());
            }

            try
            {
                // Connexion client
                string connexionStringClient = "SERVER=localhost;PORT=3306;DATABASE=Database_Livin;UID=user_client;PASSWORD=root";
                connexion_client = new MySqlConnection(connexionStringClient);
            }
            catch (MySqlException e)
            {
                Console.WriteLine(" Erreur lors de la création de l'objet de connexion user_client: " + e.ToString());
            }
        }
    }
}
