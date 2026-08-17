using System;
using System.Collections.Generic;
using System.IO;
using Visual_Studio;
using MySql.Data.MySqlClient;
using System.Text.Json.Serialization.Metadata;

namespace Visual_Studio
{
    /// <summary>
    /// Classe principale de l'application Liv'In Paris.
    /// </summary>
    internal class Program
    {
        static void Main(string[] args)
        {
            
            MySqlConnection maConnexionTestDataBase = null;

            try
            {
                string connexionString = "SERVER=localhost;PORT=3306;" +
                                         "DATABASE=Database_Livin;" +
                                         "UID=root;PASSWORD=root";
                // à modifier en fonction des informations personnelles de l'utilisateur

                maConnexionTestDataBase = new MySqlConnection(connexionString);
                maConnexionTestDataBase.Open();
                Console.WriteLine("Connexion à la base de données réussie, pas besoin de créer la DataBase.");
                Initialisation_BDD.InitialiserTables(); //intitialiser les tables comme même, car si elles existent déjà alors les commandes iront dans le catch donc il y aura pas de problème, au cas où elles n'existent pas
            }
            catch (MySqlException e)
            {
                Console.WriteLine(" La DataBase n'existe pas : \n" + e.ToString());
                Initialisation_BDD.InitialiserDataBase();
            }
            maConnexionTestDataBase.Close();


            Initialisation_BDD.PeuplementTable();
            Console.WriteLine("Tables initialisées.");
            Console.WriteLine("Tables peuplées.");

            Role.CreerRolesEtUtilisateurs(); //Creation des rôles dans la BDD, et des utilisateurs puis lance les étapes pour la connexion à la BDD des Users
            Console.WriteLine("Roles et utilisateurs créés."); 

            Peuplement.ExecuterPeuplementComplet(); // permet de peupler des utilisateurs, des plats et commandes, des notations et des régimes
            Console.WriteLine("Peuplement terminé avec succès !");






            Thread.Sleep(700);

            bool quitterApplication = false;

            while (!quitterApplication)
            {
                bool choixValide = false;
                char choix = '0';

                // Boucle d'authentification
                while (Connexion.Identifiant == 0)
                {
                    Console.Clear();
                    choixValide = false;
                    choix = '0';

                    while (!choixValide)
                    {
                        // Affichage du menu d'authentification
                        Console.WriteLine("==========================================");
                        Console.WriteLine(" Bienvenue sur l'application Liv'In Paris !");
                        Console.WriteLine("==========================================");
                        Console.WriteLine();
                        Console.WriteLine("Veuillez choisir une option :");
                        Console.WriteLine("1. Se connecter");
                        Console.WriteLine("2. Créer un compte");
                        Console.WriteLine("3. Quitter l'application");
                        Console.Write("Votre choix : ");

                        choix = Console.ReadKey().KeyChar;
                        Console.WriteLine();

                        switch (choix)
                        {
                            case '1':
                                choixValide = true;
                                break;
                            case '2':
                                choixValide = true;
                                break;
                            case '3':
                                Console.WriteLine("Merci d'avoir utilisé l'application. Au revoir !");
                                Thread.Sleep(700);
                                choixValide = true;
                                quitterApplication = true; 
                                break;
                            default:
                                Console.WriteLine("Choix invalide. Veuillez entrer 1, 2 ou 3.");
                                Thread.Sleep(700);
                                Console.Clear();
                                break;
                        }
                    }


                    if (choix == '1')
                    {
                        Connexion.Login();
                    }
                    else if (choix == '2')
                    {
                        Connexion.CreateAccount();
                    }
                    else if (choix == '3')
                    {
                        break; 
                    }
                }


                // Si l'utilisateur a choisi de quitter l'application, on sort de la boucle
                if (quitterApplication)
                {
                    break;
                }
                


                // Boucle du menu principal
                bool sessionActive = true;
                while (sessionActive)
                {
                    if (Connexion.Identifiant == 1) // si l'utilisateur est un admin
                    {
                        
                        Console.Clear();
                        Console.WriteLine("Identifiant Programme : " + Connexion.Identifiant);
                        Console.WriteLine("=== Menu Principale ===");
                        Console.WriteLine();
                        Console.WriteLine("1. Menu Admin");
                        Console.WriteLine("2. Se déconnecter");
                        Console.WriteLine();
                        Console.Write("Votre choix : ");

                        choix = Console.ReadKey().KeyChar;
                        //Console.WriteLine();

                        switch (choix)
                        {
                            case '1':
                                Admin.Menu();
                                break;
                            case '2':
                                sessionActive = false;
                                Connexion.Identifiant = 0; // Réinitialisation pour se déconnecter
                                break;
                            default:
                                Console.WriteLine("Choix invalide. Veuillez entrer 1, 2 ou 3.");
                                Thread.Sleep(700);
                                break;
                        }


                    }
                    else 
                    {
                        Console.Clear();
                        Console.WriteLine("Identifiant Programme : " + Connexion.Identifiant);
                        Console.WriteLine("=== Menu Principale ===");
                        Console.WriteLine();
                        Console.WriteLine("1. Menu Cuisinier");
                        Console.WriteLine("2. Menu Client");
                        Console.WriteLine("3. Se déconnecter");
                        Console.WriteLine();
                        Console.Write("Votre choix : ");

                        choix = Console.ReadKey().KeyChar;
                        Console.WriteLine();

                        switch (choix)
                        {
                            case '1':
                                Cuisinier.MenuCuisinier();
                                break;
                            case '2':
                                Client.MenuClient();
                                break;
                            case '3':
                                sessionActive = false;
                                Connexion.Identifiant = 0; // Réinitialisation pour se déconnecter
                                break;
                            default:
                                Console.WriteLine("Choix invalide. Veuillez entrer 1, 2 ou 3.");
                                Thread.Sleep(700);
                                break;
                        }
                    }
                }
            }

            Console.WriteLine("Appuyez sur une touche pour quitter...");
            Console.ReadKey();



        }
    }
}
        
    

            