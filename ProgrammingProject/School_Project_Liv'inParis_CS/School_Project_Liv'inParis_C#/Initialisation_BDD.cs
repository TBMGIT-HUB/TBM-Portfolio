using System;
using Visual_Studio;
using System.Security.Cryptography.Xml;
using Google.Protobuf.WellKnownTypes;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;



namespace Visual_Studio
{
    /// <summary>
    /// Classe responsable de la création de toutes les tables de la base de données.
    /// </summary>
    public class Initialisation_BDD
    {
        /// <summary>
        /// Crée la base de données principale si elle n'existe pas
        /// </summary>
        public static void InitialiserDataBase()
        {
            MySqlConnection ConnexionSansBDD = null;
            try
            {
                string connexionString = "SERVER=localhost;PORT=3306;UID=root;PASSWORD=root";

                ConnexionSansBDD = new MySqlConnection(connexionString);
                ConnexionSansBDD.Open();

                string NomDataBase = "Database_Livin";
                string creatDataBase = "CREATE DATABASE IF NOT EXISTS " + NomDataBase + ";";
                MySqlCommand createDbCmd = ConnexionSansBDD.CreateCommand();
                createDbCmd.CommandText = creatDataBase;
                createDbCmd.ExecuteNonQuery();
                Console.WriteLine("Base de données '" + NomDataBase + "' vient d'être créée.");
            }
            catch (MySqlException e)
            {
                Console.WriteLine(" ErreurConnexion : " + e.ToString());
                return;
            }
            ConnexionSansBDD.Close();
            InitialiserTables();
        }



        /// <summary>
        /// Crée l'ensemble des tables de la base de données
        /// </summary>
        public static void InitialiserTables()
        {

            string[] commands = new string[]
            {
                // Table Adresse
                @"
                CREATE TABLE IF NOT EXISTS Adresse(
                    Id_adresse INT NOT NULL AUTO_INCREMENT,
                    Numero_de_rue INT,
                    Rue VARCHAR(50),
                    Ville VARCHAR(50),
                    Code_Postale INT,
                    Metro_le_plus_proche VARCHAR(50),
                    PRIMARY KEY(Id_adresse)
                );",
                // Table Ingredient
                @"
                CREATE TABLE IF NOT EXISTS Ingredient(
                    Nom_Ingredient_autorise VARCHAR(50),
                    PRIMARY KEY(Nom_Ingredient_autorise)
                );",      

                // Table Regime
                @"
                CREATE TABLE IF NOT EXISTS Regime(
                    Nom_Regime VARCHAR(50),
                    PRIMARY KEY(Nom_Regime)
                );",
                // Table Recette
                @"
                CREATE TABLE IF NOT EXISTS Recette(
                    Recette_autorise VARCHAR(50),
                    PRIMARY KEY(Recette_autorise)
                );",
                // Table Unite_de_mesure
                @"
                CREATE TABLE IF NOT EXISTS Unite_de_mesure(
                    Unite_autorise VARCHAR(50),
                    PRIMARY KEY(Unite_autorise)
                );",

                // Table Nationalite
                @"
                CREATE TABLE IF NOT EXISTS Nationalite(
                    Nationalite_autorise VARCHAR(50),
                    PRIMARY KEY(Nationalite_autorise)
                );",
                // Table Type_de_Preparation
                @"
                CREATE TABLE IF NOT EXISTS Type_de_Preparation(
                    Type_autorise VARCHAR(50),
                    PRIMARY KEY(Type_autorise)
                );",
                // Table Utilisateur
                @"
                CREATE TABLE IF NOT EXISTS Utilisateur(
                    Identifiant INT NOT NULL AUTO_INCREMENT,
                    Nom VARCHAR(50),
                    Prenom VARCHAR(50),
                    Pseudo VARCHAR(50),
                    Email VARCHAR(50) NOT NULL,
                    Telephone VARCHAR(50) NOT NULL,
                    Mot_De_Passe VARCHAR(50),
                    Id_adresse INT NOT NULL,
                    Entreprise BOOL,
                    PRIMARY KEY(Identifiant),
                    UNIQUE(Pseudo),
                    UNIQUE(Email),
                    UNIQUE(Telephone),
                    FOREIGN KEY(Id_adresse) REFERENCES Adresse(Id_adresse)
                );",
                // Table Cuisinier
                @"
                CREATE TABLE IF NOT EXISTS Cuisinier(
                    Identifiant INT,
                    Nb_Total_De_Plat INT,
                    Nb_De_Plat_En_Cours INT,
                    Nb_Total_De_Commande INT,
                    Nb_De_Commande_En_Cours INT,
                    PRIMARY KEY(Identifiant),
                    FOREIGN KEY(Identifiant) REFERENCES Utilisateur(Identifiant)
                );",
                // Table Client
                @"
                CREATE TABLE IF NOT EXISTS Client(
                    Identifiant INT,
                    Nb_Commande_Total INT,
                    Nb_De_Commande_En_Cours INT,
                    PRIMARY KEY(Identifiant),
                    FOREIGN KEY(Identifiant) REFERENCES Utilisateur(Identifiant)
                );",
                // Table Plat_Propose
                @"
                CREATE TABLE IF NOT EXISTS Plat_Propose(
                    Id_Plat INT NOT NULL AUTO_INCREMENT,
                    Nom VARCHAR(50),
                    Type VARCHAR(50),
                    Variante_de_recette_bool BOOL,
                    Nationalite VARCHAR(50),
                    Nb_De_Client INT,
                    Prix_par_personne INT,
                    Date_de_fabrication DATETIME,
                    Date_de_peremption DATETIME,
                    Recette_autorise VARCHAR(50) NOT NULL,
                    Identifiant INT NOT NULL,
                    PRIMARY KEY(Id_Plat),
                    FOREIGN KEY(Recette_autorise) REFERENCES Recette(Recette_autorise),
                    FOREIGN KEY(Nationalite) REFERENCES Nationalite(Nationalite_autorise),
                    FOREIGN KEY(Type) REFERENCES Type_de_Preparation(Type_autorise),
                    FOREIGN KEY(Identifiant) REFERENCES Cuisinier(Identifiant)
                );",

                //Table Ingredient_Total
                @"
                CREATE TABLE IF NOT EXISTS Ingredient_Total(
                    Nom_Ingredient VARCHAR(50),
                    Quantite_ INT,
                    Unite VARCHAR(50),
                    Unite_autorise VARCHAR(50),
                    Id_Plat INT,
                    Nom_Ingredient_autorise VARCHAR(50) NOT NULL,
                    PRIMARY KEY(Id_Plat, Nom_Ingredient),
                    FOREIGN KEY(Unite_autorise) REFERENCES Unite_de_mesure(Unite_autorise),
                    FOREIGN KEY(Id_Plat) REFERENCES Plat_Propose(Id_Plat) ON DELETE CASCADE,
                    FOREIGN KEY(Nom_Ingredient_autorise) REFERENCES Ingredient(Nom_Ingredient_autorise)
                );",
                


                // Table Notation_Client
                @"
                CREATE TABLE IF NOT EXISTS Notation_Client(
                    Id_Notation INT NOT NULL AUTO_INCREMENT,
                    Notation INT,
                    Commentaire VARCHAR(50),
                    DateNotation DATETIME,
                    Id_Commande INT,
                    Id_Cuisinier INT,
                    Identifiant INT NOT NULL,
                    PRIMARY KEY(Id_Notation),
                    FOREIGN KEY(Identifiant) REFERENCES Client(Identifiant)
                );",

                // Table Notation_Chef
                @"
                CREATE TABLE IF NOT EXISTS Notation_Cuisinier(
                    Id_Notation INT NOT NULL AUTO_INCREMENT,
                    Notation INT,
                    Commentaire VARCHAR(50),
                    DateNotation DATETIME,
                    Id_Commande INT,
                    Id_Client INT,
                    Identifiant INT NOT NULL,
                    PRIMARY KEY(Id_Notation),
                    FOREIGN KEY(Identifiant) REFERENCES Cuisinier(Identifiant)
                );",
                // Table Commande
                @"
                CREATE TABLE IF NOT EXISTS Commande(
                    Id_Commande INT NOT NULL AUTO_INCREMENT,
                    Nb_de_part INT,
                    Etat_de_la_commande BOOL,
                    Id_Plat INT NOT NULL,
                    Identifiant INT NOT NULL,
                    PRIMARY KEY(Id_Commande),
                    FOREIGN KEY(Id_Plat) REFERENCES Plat_Propose(Id_Plat) ON DELETE CASCADE,
                    FOREIGN KEY(Identifiant) REFERENCES Client(Identifiant)

                );",

                // Table Suit_le_régime
                @"
                CREATE TABLE IF NOT EXISTS Suit_le_régime(
                   Id_Plat INT,
                   Nom_Regime VARCHAR(50),
                   PRIMARY KEY(Id_Plat, Nom_Regime),
                   FOREIGN KEY(Id_Plat) REFERENCES Plat_Propose(Id_Plat) ON DELETE CASCADE,
                   FOREIGN KEY(Nom_Regime) REFERENCES Regime(Nom_Regime)
                );"
,
            };





            // Exécution de chaque commande
            MySqlConnection maConnexion = null;
            try
            {
                string connexionString = "SERVER=localhost;PORT=3306;" +
                                         "DATABASE=Database_Livin;" +
                                         "UID=root;PASSWORD=root";
                maConnexion = new MySqlConnection(connexionString);
                maConnexion.Open();

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

            PeuplementTable();
        }


        /// <summary>
        /// Insère les données de référence essentielles au fonctionnement
        /// </summary>
        public static void PeuplementTable()
        {
            string[] PeuplementDesTables = new string[]
            {
            "INSERT INTO Unite_de_mesure (Unite_autorise) VALUES ('kg'), ('ml'), ('l'), ('Tasses'), ('Cuillères à soupe'), ('Cuillères à café'), ('Branche'), ('g'), ('Pincée'), ('Verre'), ('Morceau');",
            "INSERT INTO Regime (Nom_Regime) VALUES ('Aucun'), ('Vegan'), ('Sans gluten'), ('Halal'), ('Casher'), ('Végétarien');",
            "INSERT INTO Type_de_Preparation (Type_autorise) VALUES ('Autre'), ('Entrée'), ('Boisson'), ('Apéritif'), ('Snack'), ('Accompagnement'), ('Plat principal'), ('Dessert'), ('Sauce');",
            "INSERT INTO Nationalite (Nationalite_autorise) VALUES ('Italienne'), ('Inconue'), ('Mexicaine'), ('Indienne'), ('Japonaise'), ('Chinoise'), ('Espagnole'), ('Thaïlandaise'), ('Marocaine'), ('Américaine'), ('Vietnamienne'), ('Brésilienne'), ('Grecque'), ('Libanaise'), ('Coréenne'), ('Suisse'), ('Canadienne'), ('Australienne'), ('Éthiopienne'), ('Sénégalaise'), ('Française');",
            "INSERT INTO Recette (Recette_autorise) VALUES ('Création originale'), ('Inconue'), ('Pizza'), ('Burger'), ('Pâtes'), ('Tacos'), ('Sushi'), ('Quiche'), ('Gratin'), ('Soupe'), ('Sandwich'), ('Cocktail'), ('Smoothie'), ('Salade'), ('Omelette'), ('Curry'), ('Ragoût'), ('Pancake'), ('Gâteau'), ('Tarte'), ('Risotto'), ('Paella'), ('Pho'), ('Ramen'), ('Pad Thai');",
            "INSERT INTO Recette (Recette_autorise) VALUES ('Croque-monsieur'), ('Spaghetti Bolognaise'), ('Lasagnes'), ('Carbonara'), ('Soupe à l''oignon'), ('Pot-au-feu'), ('Boeuf bourguignon'), ('Blanquette de veau'), ('Poulet basquaise'), ('Tajine de poulet aux olives'), ('Couscous'), ('Paella aux fruits de mer'), ('Tarte tatin'), ('Crêpes'), ('Gaufres'), ('Mousse au chocolat'), ('Île flottante'), ('Salade niçoise'), ('Taboulé'), ('Houmous'), ('Guacamole'), ('Burrito'), ('Quesadilla'), ('Enchilada'), ('Tempura'), ('Miso soup'), ('Onigiri'), ('Kimchi'), ('Falafel'), ('Hamburger'), ('Hot-dog'), ('Frites'), ('Poutine');",
            "INSERT INTO Ingredient (Nom_Ingredient_autorise) VALUES ('Autre'), ('tomate'), ('pomme'), ('salade'), ('fromage'), ('riz'), ('poulet'), ('poivron'), ('oignon'), ('ail'), ('basilic'), ('champignon'), ('courgette'), ('carotte'), ('farine'), ('beurre'), ('lait'), ('œuf'), ('thym'), ('sucre'), ('sel'), ('poivre'), ('huile d''olive'), ('vinaigre'), ('moutarde'), ('ketchup'), ('mayonnaise'), ('citron'), ('orange'), ('banane'), ('fraise'), ('myrtille'), ('framboise'), ('chocolat'), ('café'), ('thé'), ('menthe'), ('persil'), ('coriandre'), ('gingembre'), ('curcuma'), ('cumin'), ('cannelle'), ('noix'), ('amandes'), ('noisettes'), ('pistaches');",
            "INSERT INTO Ingredient (Nom_Ingredient_autorise) VALUES ('Boeuf'), ('Porc'), ('Poisson'), ('Crevettes'), ('Tofu'), ('Lentilles'), ('Pois chiches'), ('Haricots noirs'), ('Brocoli'), ('Chou-fleur'), ('Épinards'), ('Asperges'), ('Avocat'), ('Mangue'), ('Ananas'), ('Pastèque'), ('Melon'), ('Raisins'), ('Kiwi'), ('Pêche'), ('Abricot'), ('Prune'), ('Cerise'), ('Noix de coco'), ('Cacahuètes'), ('Sésame'), ('Tournesol'), ('Huile de coco'), ('Huile de sésame'), ('Vinaigre balsamique'), ('Sauce soja'), ('Sirop d''érable'), ('Miel'), ('Levure'), ('Bicarbonate de soude'), ('Extrait de vanille'), ('Rhum'), ('Vin blanc'), ('Vin rouge');"

            };

            MySqlConnection maConnexion = null;

            // Exécution de chaque commande
            try
            {
                string connexionString = "SERVER=localhost;PORT=3306;" +
                                            "DATABASE=Database_Livin;" +
                                            "UID=root;PASSWORD=root";
                maConnexion = new MySqlConnection(connexionString);
                maConnexion.Open();

                foreach (string commandText in PeuplementDesTables)
                {
                    try
                    {
                        MySqlCommand cmd = maConnexion.CreateCommand();
                        cmd.CommandText = commandText;
                        cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException ex)
                    {
                        //Console.WriteLine("Erreur pour la commande : " + commandText.Trim() + "\n Message d'erreur : " + ex.Message);
                    }
                }

            }
            catch (MySqlException e)
            {
                Console.WriteLine(" ErreurConnexion : " + e.ToString());
                return;
            }

            try
            {
                maConnexion.Close();
            }
            catch (MySqlException e)
            {
                Console.WriteLine("Erreur lors de la fermeture de la connexion : " + e.Message);
            }
        }

    }
}

