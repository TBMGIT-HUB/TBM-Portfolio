using System;
using System.Xml;
using Visual_Studio;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Tls; // Important pour interagir avec MySQL






namespace Visual_Studio
{
    /// <summary>
    /// Classe principale pour gérer toutes les opérations liées au cuisinier
    /// </summary>
    class Cuisinier
    {


        /// <summary>
        /// Methode d'affichage du menu du cuisinier, lui permettant de créer un plat ou de gérer ses plats et commandes.
        /// </summary>
        public static void MenuCuisinier()
        {
            char choix = '0';
            bool quitter = false;
            do
            {
                Console.Clear();
                Console.WriteLine("=== Espace Cuisinier ===");
                Console.WriteLine();
                Console.WriteLine("1. Créer un nouveau plat");
                Console.WriteLine("2. Gérer vos Plats Et Commandes");
                Console.WriteLine("3. Afficher votre profil");
                Console.WriteLine("4. Quitter");
                Console.WriteLine();
                Console.Write("Votre choix : ");

                choix = Console.ReadKey().KeyChar;
                Console.WriteLine();

                switch (choix)
                {
                    case '1':
                        CreerPlat();
                        break;
                    case '2':
                        GererPlatsEtCommandes();
                        break;
                    case '3':
                        AfficherProfilCuisinier();
                        break;
                    case '4':
                        quitter = true;
                        break; 
                    default:
                        Console.WriteLine("Choix invalide. Veuillez entrer 1, 2 ou 3");
                        Thread.Sleep(700);
                        Console.Clear();
                        break;
                }
            }
            while (!quitter);
        }

        /// <summary>
        /// Permets de créer un nouveau plat en respectant les contraintes de la base de données.
        /// </summary>
        public static void CreerPlat()
        {
            Console.Clear();
            Console.WriteLine("--- Création d'un nouveau plat ---");

            //Section 0: Saisie du nom du plat
            Console.Write("Entrez le nom du plat : ");
            string nomPlat = Console.ReadLine();


            //Section 1: Saisie et validation du nom de la recette qui doit exister dans la table Recette

            string recette = "";
            bool recetteValide = false;
            do
            {
                Console.Write("Entrez le nom de la recette (doit être dans la table Recette) : ");
                recette = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(recette))
                {
                    Console.WriteLine("Le nom de la recette ne peut pas être vide.");
                }
                else if (!ElementExiste("Recette", "Recette_autorise", recette))
                {
                    Console.WriteLine("Cette recette n'existe pas dans la table Recette.");
                }
                else
                {
                    recetteValide = true;
                }
            } while (!recetteValide);

            // Section 2: Saisie et validation du type de préparation qui doit exister dans la table Type_de_Preparation
            string typePreparation = "";
            bool typeValide = false;
            do
            {
                Console.Write("Entrez le type de préparation (doit être dans la table Type_de_Preparation) : ");
                typePreparation = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(typePreparation))
                {
                    Console.WriteLine("Le type de préparation ne peut pas être vide.");
                }
                else if (!ElementExiste("Type_de_Preparation", "Type_autorise", typePreparation))
                {
                    Console.WriteLine("Ce type de préparation n'existe pas.");
                }
                else
                {
                    typeValide = true;
                }
            } while (!typeValide);

            //Section 3: Vérification si c'est une variante de recette est stockée comme booléen dans la base
            bool variante = false;
            char reponse;
            do
            {
                Console.Write("Est-ce une variante de recette ? (O/N) : ");
                reponse = Console.ReadKey().KeyChar;
                Console.WriteLine();
                if (reponse == 'O' || reponse == 'o')
                {
                    variante = true;
                }
                else if (reponse == 'N' || reponse == 'n')
                {
                    variante = false;
                }
                else
                {
                    Console.WriteLine("Veuillez répondre par O ou N.");
                }
            } while (reponse != 'O' && reponse != 'o' && reponse != 'N' && reponse != 'n');

            //Section 4: Saisie de la nationalité (doit être dans la table Nationalite)
            string nationalite = "";
            bool nationaliteValide = false;
            do
            {
                Console.Write("Entrez la nationalité (doit être dans la table Nationalite) : ");
                nationalite = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(nationalite))
                {
                    Console.WriteLine("La nationalité ne peut pas être vide.");
                }
                else if (!ElementExiste("Nationalite", "Nationalite_autorise", nationalite))
                {
                    Console.WriteLine("Cette nationalité n'existe pas.");
                }
                else
                {
                    nationaliteValide = true;
                }
            } while (!nationaliteValide);

            //Section 5: Saisie du nombre de clients (doit être supérieur ou égal à 1)
            int nbClients = 0;
            bool nbClientsValide = false;
            do
            {
                Console.Write("Entrez le nombre de clients (supérieur ou égal à 1) : ");
                string nbClientsStr = Console.ReadLine().Trim();
                if (!int.TryParse(nbClientsStr, out nbClients))
                {
                    Console.WriteLine("Veuillez entrer un nombre valide.");
                }
                else if (nbClients < 1)
                {
                    Console.WriteLine("Le nombre de clients doit être supérieur ou égal à 1.");
                }
                else
                {
                    nbClientsValide = true;
                }
            } while (!nbClientsValide);

            //Section 6: Saisie du prix par personne (doit être supérieur à 0)
            int prixParPersonne = 0;
            bool prixValide = false;
            do
            {
                Console.Write("Entrez le prix par personne (supérieur à 0) : ");
                string prixStr = Console.ReadLine().Trim();
                if (!int.TryParse(prixStr, out prixParPersonne))
                {
                    Console.WriteLine("Veuillez entrer un nombre valide.");
                }
                else if (prixParPersonne <= 0)
                {
                    Console.WriteLine("Le prix doit être supérieur à 0.");
                }
                else
                {
                    prixValide = true;
                }
            } while (!prixValide);

            //Section 7: Saisie de la date de fabrication
            DateTime dateFabrication;
            bool dateFabValide = false;
            do
            {
                Console.Write("Entrez la date de fabrication (format jj/mm/aaaa) : ");
                string dateFabStr = Console.ReadLine().Trim();
                if (!DateTime.TryParse(dateFabStr, out dateFabrication))
                {
                    Console.WriteLine("Veuillez entrer une date valide.");
                }
                else
                {
                    dateFabValide = true;
                }
            } while (!dateFabValide);

            //Section 8: Saisie de la date de péremption (doit être postérieure à la date de fabrication)
            DateTime datePeremption;
            bool datePerempValide = false;
            do
            {
                Console.Write("Entrez la date de péremption (format jj/mm/aaaa) : ");
                string datePerempStr = Console.ReadLine().Trim();
                if (!DateTime.TryParse(datePerempStr, out datePeremption))
                {
                    Console.WriteLine("Veuillez entrer une date valide.");
                }
                else if (datePeremption <= dateFabrication)
                {
                    Console.WriteLine("La date de péremption doit être postérieure à la date de fabrication.");
                }
                else
                {
                    datePerempValide = true;
                }
            } while (!datePerempValide);




            // Insertion dans la table Plat_Propose
            int nb = 0; // Nombre de lignes affectées, pour véridier que l'insertion a été bien faite pour ensuite incrémenter 
            int idPlat = -1; // Pour récupérer l'ID du plat inséré
            try
            {
                Role.ConnexionCuisinier.Open();
                try
                {
                    string requeteInsertion = @"INSERT INTO Plat_Propose 
                        (Nom, Type, Variante_de_recette_bool, Nationalite, Nb_De_Client, Prix_par_personne, Date_de_fabrication, Date_de_peremption, Recette_autorise, Identifiant)
                        VALUES (@Nom, @Type, @Variante, @Nationalite, @NbClients, @Prix, @DateFab, @DatePeremp, @Recette, @Identifiant);";
                    MySqlCommand cmdInsertion = new MySqlCommand(requeteInsertion, Role.ConnexionCuisinier);
                    cmdInsertion.Parameters.AddWithValue("@Nom", nomPlat);
                    cmdInsertion.Parameters.AddWithValue("@Type", typePreparation);
                    cmdInsertion.Parameters.AddWithValue("@Variante", variante);
                    cmdInsertion.Parameters.AddWithValue("@Nationalite", nationalite);
                    cmdInsertion.Parameters.AddWithValue("@NbClients", nbClients);
                    cmdInsertion.Parameters.AddWithValue("@Prix", prixParPersonne);
                    cmdInsertion.Parameters.AddWithValue("@DateFab", dateFabrication);
                    cmdInsertion.Parameters.AddWithValue("@DatePeremp", datePeremption);
                    cmdInsertion.Parameters.AddWithValue("@Recette", recette);
                    cmdInsertion.Parameters.AddWithValue("@Identifiant", Connexion.Identifiant);
                    nb = cmdInsertion.ExecuteNonQuery();
                    idPlat = (int)cmdInsertion.LastInsertedId;
                    Role.ConnexionCuisinier.Close();

                    try
                    {
                        Console.WriteLine("\n--- Régimes du plat ---");
                        bool continuerRegime = true;
                        string nomRegime = "";
                        while (continuerRegime)
                        {
                            Console.Write("Entrez un régime suivi par le plat (ou 'Aucun' s'il n'en a pas et 'Terminer' pour arréter la saisie) : ");
                            nomRegime = Console.ReadLine().Trim();
                            if (nomRegime.ToLower() == "terminer")
                            {
                                continuerRegime = false;
                            }
                            else if (!ElementExiste("Regime", "Nom_Regime", nomRegime))
                            {
                                Console.WriteLine("Ce régime n'existe pas dans la table Regime.");
                            }
                            else
                            {
                                Role.ConnexionCuisinier.Open();
                                // Insertion dans la table Suit_le_régime
                                try
                                {
                                    string requeteInsertionRegime = "INSERT INTO Suit_le_régime (Id_Plat, Nom_Regime) VALUES (@IdPlat, @NomRegime);";
                                    MySqlCommand cmdInsertionRegime = new MySqlCommand(requeteInsertionRegime, Role.ConnexionCuisinier);
                                    cmdInsertionRegime.Parameters.AddWithValue("@IdPlat", idPlat);
                                    cmdInsertionRegime.Parameters.AddWithValue("@NomRegime", nomRegime);
                                    cmdInsertionRegime.ExecuteNonQuery();
                                }
                                catch (MySqlException ex)
                                {
                                    Console.WriteLine("Erreur lors de l'insertion du régime : " + ex.Message);
                                    Console.ReadKey();
                                }
                                    Role.ConnexionCuisinier.Close();
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine("Erreur lors de l'insertion du régime : " + ex.Message);
                        Console.ReadKey();
                    }

                 
                    try
                    {
                        // Saisie des ingrédients
                        Console.WriteLine("\n--- Ingrédients du plat ---");
                        bool continuerIngredient = true;
                        while (continuerIngredient)
                        {
                            // Boucle pour la saisie et validation de l'ingrédient
                            string nomIngredient = "";
                            bool ingredientValide = false;
                            while (!ingredientValide)
                            {
                                Console.Write("Entrez le nom de l'ingrédient (ou 'terminer' pour finir la création du plat) : ");
                                nomIngredient = Console.ReadLine().Trim();
                                if (nomIngredient.ToLower() == "terminer")
                                {
                                    // Sortir des deux boucles si l'utilisateur termine la saisie
                                    continuerIngredient = false;
                                    ingredientValide = true;
                                    break;
                                }
                                else if (!ElementExiste("Ingredient", "Nom_Ingredient_autorise", nomIngredient))
                                {
                                    Console.WriteLine("Cet ingrédient n'existe pas dans la table Ingredient.");
                                }
                                else
                                {
                                    ingredientValide = true;
                                }
                            }

                            if (!continuerIngredient)
                            {
                                break;
                            }

                            // Validation de la quantité
                            int quantite = 0;
                            bool quantiteValide = false;
                            while (!quantiteValide)
                            {
                                Console.Write("Entrez la quantité : ");
                                if (!int.TryParse(Console.ReadLine().Trim(), out quantite))
                                {
                                    Console.WriteLine("Veuillez entrer une quantité valide.");
                                }
                                else
                                {
                                    quantiteValide = true;
                                }
                            }

                            // Boucle pour la saisie et validation de l'unité de mesure
                            string unite = "";
                            bool uniteValide = false;
                            while (!uniteValide)
                            {
                                Console.Write("Entrez l'unité de mesure (doit être dans la table Unite_de_mesure) : ");
                                unite = Console.ReadLine().Trim();
                                if (!ElementExiste("Unite_de_mesure", "Unite_autorise", unite))
                                {
                                    Console.WriteLine("Cette unité de mesure n'existe pas.");
                                }
                                else
                                {
                                    uniteValide = true;
                                }
                            }
                            Console.WriteLine();

                            // Une fois que l'ingrédient, la quantité et l'unité sont validés, on insère dans la table Ingredient_Total
                            try
                            {
                                Role.ConnexionCuisinier.Open();
                            }
                            catch (MySqlException ex)
                            {
                                Console.WriteLine("Erreur lors de l'ouverture : " + ex.Message);
                            }
                            try
                            {
                                string requeteInsertionIngredient = @"
                                    INSERT INTO Ingredient_Total (Nom_Ingredient, Quantite_, Unite, Unite_autorise, Id_Plat, Nom_Ingredient_autorise)
                                    VALUES (@NomIngredient, @Quantite, @Unite, @UniteAutorise, @IdPlat, @NomIngredientAutorise);";
                                MySqlCommand cmdInsertionIngredient = new MySqlCommand(requeteInsertionIngredient, Role.ConnexionCuisinier);
                                cmdInsertionIngredient.Parameters.AddWithValue("@NomIngredient", nomIngredient);
                                cmdInsertionIngredient.Parameters.AddWithValue("@Quantite", quantite);
                                cmdInsertionIngredient.Parameters.AddWithValue("@Unite", unite);
                                cmdInsertionIngredient.Parameters.AddWithValue("@UniteAutorise", unite);
                                cmdInsertionIngredient.Parameters.AddWithValue("@IdPlat", idPlat);
                                cmdInsertionIngredient.Parameters.AddWithValue("@NomIngredientAutorise", nomIngredient);
                                cmdInsertionIngredient.ExecuteNonQuery();
                            }
                            catch (MySqlException ex)
                            {
                                Console.WriteLine("Erreur lors de l'insertion de l'ingrédient : " + ex.Message);
                                Console.ReadKey();
                            }
                            try
                            {
                                Role.ConnexionCuisinier.Close();
                            }
                            catch(MySqlException ex)
                            {
                                Console.WriteLine("Erreur lors de la fermeture : " + ex.Message);
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine("Erreur lors de l'insertion de l'ingrédient : " + ex.Message);
                        Console.ReadKey();
                    }




                    Console.WriteLine("\nPlat créé avec succès !");
                    Thread.Sleep(800);

                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Erreur lors de l'insertion du plat : " + ex.Message);
                    Console.ReadKey();
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur lors de l'ouverture de la connexion : " + ex.Message);
                Console.ReadKey();
            }



            Console.WriteLine("Appuyez sur une touche pour revenir au menu...");
            Console.ReadKey();
        }

        /// <summary>
        /// Méthode qui vérifie si un élément existe dans une table donnée.
        /// Retourne true si l'élément existe, false sinon.
        /// </summary>
        /// <param name="table">Nom de la table</param>
        /// <param name="colonne">Nom de la colonne</param>
        /// <param name="valeur">Valeur à vérifier</param>
        /// <returns>booléen indiquant si l'élément existe</returns>
        public static bool ElementExiste(string table, string colonne, string valeur)
        {
            bool existe = false;
            try
            {
                Role.ConnexionCuisinier.Open();
                string requete = "SELECT COUNT(*) FROM " + table + " WHERE " + colonne + " = @valeur;";
                MySqlCommand cmd = new MySqlCommand(requete, Role.ConnexionCuisinier);
                cmd.Parameters.AddWithValue("@valeur", valeur);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                if (count > 0)
                {
                    existe = true;
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur lors de la vérification de l'existence de l'élément : " + ex.Message);
                Console.ReadKey();
            }
            try
            {
                Role.ConnexionCuisinier.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur lors de la fermeture de la connexion : " + ex.Message);
                Console.ReadKey();
            }
            return existe;
        }






        /// <summary>
        /// Permet d'afficher une table de la base de données.
        /// </summary>
        public static void AfficherTable(string requeteAfficheTableau)
        {
            try
            {
                // Ouverture de la connexion (ici on utilise ConnexionCuisinier)
                Role.ConnexionCuisinier.Open();

                try
                {
                    MySqlCommand cmd = new MySqlCommand(requeteAfficheTableau, Role.ConnexionCuisinier);
                    cmd.Parameters.AddWithValue("@Identifiant", Connexion.Identifiant);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Console.Write(reader.GetName(i) + "\t");
                    }
                    Console.WriteLine();

                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.Write(reader.GetValue(i).ToString() + "\t");
                        }
                        Console.WriteLine();
                    }
                    reader.Close();
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Erreur lors de l'affichage de la table : " + ex.Message);
                    Console.ReadKey();
                }
            }
            catch (MySqlException e)
            {
                Console.WriteLine("Erreur de connexion : " + e.ToString());
                Console.ReadKey();
                Role.ConnexionCuisinier.Close();
                return;
            }
            try
            {
                Role.ConnexionCuisinier.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur lors de la fermeture de la connexion : " + ex.Message);
                Console.ReadKey();
            }
        }



































































        /// <summary>
        /// Affichage pour la gestion des plats et commandes du cuisinier
        /// </summary>
        public static void GererPlatsEtCommandes()
        {
            char choix = '0';
            bool quitter = false;
            do
            {
                Console.Clear();
                Console.WriteLine("=== Gestion des plats et commandes du cuisinier ===");
                Console.WriteLine("Veuillez choisir une option :");
                Console.WriteLine("1. Afficher l'itinéraire d'une commande");
                Console.WriteLine("2. Modifier l'état d'une commande (livraison)");
                Console.WriteLine("3. Supprimer un plat de votre liste");
                Console.WriteLine("4. Retour à l'Espace Cuisinier");
                Console.WriteLine();
                Console.Write("Votre choix : ");

                choix = Console.ReadKey().KeyChar;
                Console.WriteLine();

                switch (choix)
                {
                    case '1':
                        AfficherItinéraire();
                        break;
                    case '2':
                        ModifierEtatCommande();
                        break;
                    case '3':
                        SupprimerPlat();
                        break;
                    case '4':
                        quitter = true;
                        break;
                    default:
                        Console.WriteLine("Choix invalide. Veuillez entrer 1, 2 ou 3");
                        Thread.Sleep(700);
                        Console.Clear();
                        break;
                }
            }
            while (!quitter);
        }







        /// <summary>
        /// Affichage l'itinéraire que doit suivre le cuisinier pour la commande du client
        /// </summary>
        public static void AfficherItinéraire()
        {
            Console.Clear();
            Console.WriteLine("=== Affichage de l'itinéraire ===");
            // Vérification s'il y a des plats pour ce cuisinier
            int nombreDePlats = 0;
            try
            {
                Role.ConnexionCuisinier.Open();
                try
                {
                    string requette = "SELECT COUNT(*) FROM Commande WHERE Id_Plat IN (SELECT Id_Plat FROM Plat_Propose WHERE Identifiant = @Identifiant);";
                    MySqlCommand cmdCount = new MySqlCommand(requette, Role.ConnexionCuisinier);
                    cmdCount.Parameters.AddWithValue("@Identifiant", Connexion.Identifiant);
                    nombreDePlats = Convert.ToInt32(cmdCount.ExecuteScalar());
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Erreur lors de la vérification des plats : " + ex.Message);
                    Console.ReadKey();
                    Role.ConnexionCuisinier.Close();
                    return;
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur lors de la connexion: " + ex.Message);
                Console.ReadKey();
                Role.ConnexionCuisinier.Close();
                return;
            }
            try
            {
                Role.ConnexionCuisinier.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur lors de la fermeture de la connexion." + ex.Message);
                Console.ReadKey();
            }


            if (nombreDePlats == 0)
            {
                Console.WriteLine("Vous n'avez pas de plat.");
                Console.WriteLine("Appuyez sur une touche pour revenir au menu...");
                Console.ReadKey();
                return;
            }



            // Demande que l'utilisateur entre l'ID de la commande à afficher
            string reaquetteAfficherTable = "SELECT * FROM Commande WHERE Id_Plat IN(SELECT Id_Plat FROM Plat_Propose WHERE Identifiant = @Identifiant);";
            AfficherTable(reaquetteAfficherTable);

            // Saisie de l'ID de la commande pour l'itinéraire de cette commande
            int idAfficher = 0;
            bool idValide = false;
            do
            {
                Console.Write("Entrez l'ID de la commande à afficher (Q pour quitter) : ");
                string saisie = Console.ReadLine();
                if (saisie == "Q" || saisie == "q")
                {
                    return;
                }
                else if (!int.TryParse(saisie, out idAfficher))
                {
                    Console.WriteLine("Veuillez entrer un numéro valide.");
                }
                else if (!CommandeExistePourCuisinier(idAfficher))
                {
                    Console.WriteLine("Cet ID n'existe pas ou ne vous appartient pas.");
                }
                else
                {
                    idValide = true;
                }
            }
            while (!idValide);


            string metroClient = "";
            string metroCuisinier = "";
            try
            {
                Role.ConnexionCuisinier.Open();

                try
                {
                    // Récupérer le métro le plus proche du client
                    string requetteClient = @"
                        SELECT a.Metro_le_plus_proche
                        FROM Utilisateur u
                        JOIN Adresse a ON u.Id_adresse = a.Id_adresse
                        WHERE u.Identifiant = (SELECT c.Identifiant FROM Commande c WHERE c.Id_Commande = @idCommande);";
                    MySqlCommand cmdClient = new MySqlCommand(requetteClient, Role.ConnexionCuisinier);
                    cmdClient.Parameters.AddWithValue("@idCommande", idAfficher);
                    metroClient = Convert.ToString(cmdClient.ExecuteScalar());

                    // Récupérer le métro le plus proche du cuisinier
                    string requetteCuisinier = @"
                        SELECT a.Metro_le_plus_proche
                        FROM Plat_Propose p
                        JOIN Utilisateur u ON p.Identifiant = u.Identifiant
                        JOIN Adresse a ON u.Id_adresse = a.Id_adresse
                        WHERE p.Id_Plat = (SELECT c.Id_Plat FROM Commande c WHERE c.Id_Commande = @idCommande);";
                    MySqlCommand cmdCuisinier = new MySqlCommand(requetteCuisinier, Role.ConnexionCuisinier);
                    cmdCuisinier.Parameters.AddWithValue("@idCommande", idAfficher);
                    metroCuisinier = Convert.ToString(cmdCuisinier.ExecuteScalar());
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Erreur lors de la récupération des stations : " + ex.Message);
                    Console.ReadKey();
                    Role.ConnexionCuisinier.Close();
                    return;
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur lors de l'ouverture de la connexion " + ex.Message);
            }
            try
            {
                Role.ConnexionCuisinier.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur lors de la fermeture de la connexion : " + ex.Message);
            }


            // Affichage de l'itinéraire
            Console.WriteLine("\n=== Itinéraire ===");
            try
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Arcs.csv");
                //Console.WriteLine("Chemin du fichier Arcs.csv : " + filePath);
                try
                {
                    string[] lines = System.IO.File.ReadAllLines(filePath);
                    if (lines.Length > 0)
                    {
                        Console.WriteLine("Première ligne du fichier : "+lines[0]);
                    }
                    else
                    {
                        Console.WriteLine("Le fichier Arcs.csv est vide.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERREUR LECTURE SIMPLE : "+ex.Message);
                    return;
                }

                Graphe Graphe = new Graphe(filePath);
                Noeuds noeuds = new Noeuds(filePath);

                Console.WriteLine("Station du cuisinier : " + metroCuisinier);
                Console.WriteLine("Station du client    : " + metroClient);
                try
                {
                    Dictionary<string, int> IDetNOM = Graphe.DicoIDNOM;
                    
                    if (IDetNOM.TryGetValue(metroCuisinier, out int ID_metroCuisinier) && IDetNOM.TryGetValue(metroClient, out int ID_metroCLient))
                    {
                        Console.WriteLine("ID trouvé : " +ID_metroCuisinier);
                        Console.WriteLine("ID trouvé : " +ID_metroCLient);
                        try
                        {
                            List<int> PCM = Graphe.TrouverCheminDjikstra(ID_metroCuisinier, ID_metroCLient);
                            
                            //noeuds.AfficheListe();
                            /*
                            foreach (KeyValuePair<int, int> pairDeValeur in noeuds.Welsh_Powell())
                            {
                                Console.WriteLine("sommet : " + pairDeValeur.Key + " --- > couleur : " + pairDeValeur.Value);
                            }
                            */

                            Dictionary<int, int> WelshNoeuds = noeuds.Welsh_Powell();
                            int poids = Graphe.CalculerPoidsChemin(PCM);

                            for (int i = 0; i < PCM.Count; i++)
                            {
                                Console.Write(PCM[i] + " ");
                            }
                            Console.WriteLine("\nPoids total:" + poids +" minutes");

                            /*
                            System.Windows.Forms.Application.EnableVisualStyles();
                            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
                            System.Windows.Forms.Application.Run(new MetroForm(PCM, WelshNoeuds));
                            */

                            try
                            {
                                using (MetroForm metroForm = new MetroForm(PCM, WelshNoeuds))
                                {
                                    metroForm.ShowDialog();
                                }
                                Console.WriteLine("Fenêtre d'itinéraire fermée.");
                            }
                            catch (Exception exForm)
                            {
                                Console.WriteLine("ERREUR lors de l'affichage de MetroForm: " + exForm.Message);
                            }
                        }
                        catch
                        {
                            Console.WriteLine("ERREUR : problème dans le lancement de djikstra");
                        }
                    }
                    else { Console.WriteLine("ERREUR : ID non trouvés"); Console.WriteLine("Client : " + metroClient + " Cuisinier : " + metroCuisinier); }
                }
                catch
                {
                    Console.WriteLine("ERRERU : problème quant à l'utilisation des métros");
                }
            }
            catch
            {
                Console.WriteLine("ERREUR : problème dans la lecture du fichier excel ou dans la création du graphe");
            }
            
            


           
            Console.WriteLine("Appuyez sur une touche pour revenir au menu...");
            Console.ReadKey();
        }








































        /// <summary>
        /// Permet de supprimer un plat appartenant au cuisinier.
        /// On affiche d'abord la liste des plats, puis on demande l'ID à supprimer après vérification.
        /// On empêche la suppression si le plat est lié à une commande.
        /// </summary>
        private static void SupprimerPlat()
        {
            bool quitter = false;
            while(!quitter)
            {
                Console.Clear();
                Console.WriteLine("=== Suppression d'un plat ===");

                // Vérification s'il y a des plats pour ce cuisinier
                int nombreDePlats = 0;
                try
                {
                    Role.ConnexionCuisinier.Open();
                    try
                    {
                        string requette = "SELECT COUNT(*) FROM Plat_Propose WHERE Identifiant = @Identifiant;";
                        MySqlCommand cmdCount = new MySqlCommand(requette, Role.ConnexionCuisinier);
                        cmdCount.Parameters.AddWithValue("@Identifiant", Connexion.Identifiant);
                        nombreDePlats = Convert.ToInt32(cmdCount.ExecuteScalar());
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine("Erreur lors de la vérification des plats : " + ex.Message);
                        Console.ReadKey();
                        Role.ConnexionCuisinier.Close();
                        return;
                    }
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Erreur lors de la connexion: " + ex.Message);
                    Console.ReadKey();

                    return;
                }
                try
                {
                    Role.ConnexionCuisinier.Close();
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Erreur lors de la fermeture de la connexion." + ex.Message);
                }

                if (nombreDePlats == 0)
                {
                    Console.WriteLine("Vous n'avez pas de plat.");
                    Console.WriteLine("Appuyez sur une touche pour revenir au menu...");
                    Console.ReadKey();
                    quitter = true;
                    return;
                }



                // Affichage de la liste des plats du cuisinier
                try
                {
                    string requeteAfficheTableau = "SELECT * FROM Plat_Propose WHERE Identifiant = @Identifiant;";
                    AfficherTable(requeteAfficheTableau);
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Erreur lors de l'affichage de vos plats : " + ex.Message);
                    Console.ReadKey();
                    return;
                }


                // Saisie de l'ID du plat à supprimer
                int idASupprimer = 0;
                bool idValide = false;
                do
                {
                    Console.Write("\nEntrez l'ID du plat à supprimer (Q pour revenir à la Gestion des plats et commandes) : ");
                    string saisie = Console.ReadLine();
                    if (saisie == "Q" || saisie == "q")
                    {
                        return;
                    }
                    else if (!int.TryParse(saisie, out idASupprimer))
                    {
                        Console.WriteLine("Veuillez entrer un numéro valide.");
                    }
                    else if (!PlatExistePourCuisinier(idASupprimer))
                    {
                        Console.WriteLine("Cet ID n'existe pas ou ne vous appartient pas.");
                    }
                    else
                    {
                        // Vérification que le plat n'est pas utilisé dans une commande
                        bool estDansCommande = false;
                        try
                        {
                            Role.ConnexionCuisinier.Open();
                            try
                            {
                                string requette = "SELECT COUNT(*) FROM Commande WHERE Id_Plat = @id;";
                                MySqlCommand cmdCheck = new MySqlCommand(requette, Role.ConnexionCuisinier);
                                cmdCheck.Parameters.AddWithValue("@id", idASupprimer);
                                int count = Convert.ToInt32(cmdCheck.ExecuteScalar());
                                if (count > 0)
                                {
                                    // Le plat est utilisé dans une commande
                                    // On empêche la suppression
                                    estDansCommande = true;
                                }
                            }
                            catch (MySqlException ex)
                            {
                                Console.WriteLine("Erreur lors de la vérification de la commande : " + ex.Message);
                                Console.ReadKey();
                            }
                        }
                        catch (MySqlException ex)
                        {
                            Console.WriteLine("Erreur lors de l'ouverture de la connexion." + ex.Message);
                            Console.ReadKey();

                        }
                        try
                        {
                            Role.ConnexionCuisinier.Close();
                        }
                        catch (MySqlException ex)
                        {
                            Console.WriteLine("Erreur lors de la fermeture de la connexion." + ex.Message);
                            Console.ReadKey();

                        }


                        if (estDansCommande)
                        {
                            Console.WriteLine("Ce plat est associé à une commande et ne peut pas être supprimé.");
                        }
                        else
                        {
                            idValide = true;

                            // Confirmation de suppression
                            char confirmation;
                            bool confirmationValide = false;
                            do
                            {
                                Console.Write("Confirmez-vous la suppression de ce plat ? (O/N) : ");
                                confirmation = Console.ReadKey().KeyChar;
                                Console.WriteLine();
                                if (confirmation == 'O' || confirmation == 'o' || confirmation == 'N' || confirmation == 'n')
                                {
                                    confirmationValide = true;
                                }
                                else
                                {
                                    Console.WriteLine("Veuillez répondre par O ou N.");
                                }
                            }
                            while (!confirmationValide);
                            if (confirmation == 'O' || confirmation == 'o')
                            {
                                // Suppression du plat
                                int nb_lignes = 0;
                                try
                                {
                                    Role.ConnexionCuisinier.Open();
                                    try
                                    {
                                        string requette = "DELETE FROM Plat_Propose WHERE Id_Plat = @id AND Identifiant = @idCuisinier;";
                                        MySqlCommand cmdDelete = new MySqlCommand(requette, Role.ConnexionCuisinier);
                                        cmdDelete.Parameters.AddWithValue("@id", idASupprimer);
                                        cmdDelete.Parameters.AddWithValue("@idCuisinier", Connexion.Identifiant);
                                        nb_lignes = cmdDelete.ExecuteNonQuery();
                                        if (nb_lignes > 0)
                                        {
                                            Console.WriteLine("Plat supprimé avec succès.\n");
                                        }
                                        else
                                        {
                                            Console.WriteLine("Erreur : aucun plat n'a été supprimé.");
                                        }
                                    }
                                    catch (MySqlException ex)
                                    {
                                        Console.WriteLine("Erreur lors de la suppression du plat : " + ex.Message);
                                        Console.ReadKey();

                                    }
                                }
                                catch (MySqlException ex)
                                {
                                    Console.WriteLine("Erreur lors de l'ouverture : " + ex.Message);
                                    Console.ReadKey();

                                }
                                try
                                {
                                    Role.ConnexionCuisinier.Close();
                                }
                                catch (MySqlException ex)
                                {
                                    Console.WriteLine("Erreur lors de la fermeture de la connexion." + ex.Message);
                                    Console.ReadKey();

                                }
                            }
                            else
                            {
                                Console.WriteLine("Suppression annulée.\n");
                            }
                        }                      
                    }
                }
                while (!idValide);



                Console.WriteLine("Appuyez sur une touche pour continuer...");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Vérifie que le plat existe pour le cuisinier donné
        /// </summary>
        public static bool PlatExistePourCuisinier(int idPlat)
        {
            bool existe = false;
            try
            {
                Role.ConnexionCuisinier.Open();
                try
                {
                    string requette = "SELECT COUNT(*) FROM Plat_Propose WHERE Id_Plat = @id AND Identifiant = @idCuisinier;";
                    MySqlCommand cmd = new MySqlCommand(requette, Role.ConnexionCuisinier);
                    cmd.Parameters.AddWithValue("@id", idPlat);
                    cmd.Parameters.AddWithValue("@idCuisinier", Connexion.Identifiant);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    if (count > 0)
                    {
                        existe = true;
                    }
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Erreur lors de la vérification du plat : " + ex.Message);
                    Console.ReadKey();

                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur lors l'ouverture de la connexion : " + ex.Message);
                Console.ReadKey();

            }
            try
            {
                Role.ConnexionCuisinier.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur lors de la fermeture de la connexion." + ex.Message);
                Console.ReadKey();

            }
            return existe;
        }





































































        /// <summary>
        /// Permet au cuisinier de modifier l'état d'une commande associée à l'un de ses plats
        /// </summary>
        public static void ModifierEtatCommande()
        {
            Console.Clear();
            Console.WriteLine("=== Modification de l'état d'une commande ===");


            // Vérification s'il y a des plats pour ce cuisinier
            int nombreDePlats = 0;
            try
            {
                Role.ConnexionCuisinier.Open();
                try
                {
                    string requette = @"SELECT COUNT(*) FROM Commande c
                             JOIN Plat_Propose p ON c.Id_Plat = p.Id_Plat
                             WHERE p.Identifiant = @Identifiant;";
                    MySqlCommand cmdCount = new MySqlCommand(requette, Role.ConnexionCuisinier);
                    cmdCount.Parameters.AddWithValue("@Identifiant", Connexion.Identifiant);
                    nombreDePlats = Convert.ToInt32(cmdCount.ExecuteScalar());
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Erreur lors de la vérification des plats : " + ex.Message);
                    Console.ReadKey();
                    return;
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur lors de la connexion: " + ex.Message);
                Console.ReadKey();
                return;
            }
            try
            {
                Role.ConnexionCuisinier.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur lors de la fermeture de la connexion." + ex.Message);
            }

            if (nombreDePlats == 0)
            {
                Console.WriteLine("Vous n'avez pas de plat.");
                Console.WriteLine("Appuyez sur une touche pour revenir au menu...");
                Console.ReadKey();
                return;
            }




            // Afficher la liste des commandes liées aux plats du cuisinier
            try
            {
                Role.ConnexionCuisinier.Open();

                try
                {
                    string requette = @"SELECT c.Id_Commande, c.Etat_de_la_commande, p.Nom 
                             FROM Commande c
                             JOIN Plat_Propose p ON c.Id_Plat = p.Id_Plat
                             WHERE p.Identifiant = @id;";
                    MySqlCommand cmd = new MySqlCommand(requette, Role.ConnexionCuisinier);
                    cmd.Parameters.AddWithValue("@id", Connexion.Identifiant);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    /*
                    if (!reader.HasRows)
                    {
                        Console.WriteLine("Aucune commande associée à vos plats n'a été trouvée.");
                        reader.Close();
                        return;
                    }*/
                    Console.WriteLine("Voici vos commandes :");
                    while (reader.Read())
                    {
                        int idCommande = reader.GetInt32("Id_Commande");
                        bool etat = reader.GetBoolean("Etat_de_la_commande");
                        string nomPlat = reader.GetString("Nom");
                        if (etat)
                            Console.WriteLine("Commande : " + idCommande + " - Plat  : " + nomPlat + " | Etat : Livrée");
                        else
                        {
                            Console.WriteLine("Commande : " + idCommande + " - Plat  : " + nomPlat + " | Etat : En cours");
                        }
                    }
                    reader.Close();
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Erreur lors de l'affichage des commandes : " + ex.Message);
                    Console.ReadKey();
                    return;
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur lors l'ouverture de la connexion : " + ex.Message);
                Console.ReadKey();
            }
            try
            {
                Role.ConnexionCuisinier.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur lors de la fermeture de la connexion." + ex.Message);
                Console.ReadKey();
            }

            // Saisie de l'ID de la commande à modifier
            int idCommandeAModifier = 0;
            bool idCommandeValide = false;
            string saisie_id = "";
            do
            {
                Console.Write("Entrez l'ID de la commande à modifier (Q pour quitter): ");
                saisie_id = Console.ReadLine();
                if (saisie_id == "Q" || saisie_id == "q")
                {
                    return;
                }
                else if (!int.TryParse(saisie_id, out idCommandeAModifier))
                {
                    Console.WriteLine("Veuillez entrer un numéro valide.");
                }
                else if (!CommandeExistePourCuisinier(idCommandeAModifier))
                {
                    Console.WriteLine("Cette commande n'existe pas ou n'est pas associée à vos plats.");
                }
                else
                {
                    idCommandeValide = true;
                }
            }
            while (!idCommandeValide);

            // Saisie du nouvel état de la commande
            bool nouvelEtat = false;
            bool etatValide = false;
            char saisie_etat;
            do
            {
                Console.Write("Entrez le nouvel état de la commande (1 pour Livrée, 0 pour En cours) : ");
                saisie_etat = Console.ReadKey().KeyChar;
                Console.WriteLine();
                if (saisie_etat == '1')
                {
                    nouvelEtat = true;
                    etatValide = true;
                }
                else if (saisie_etat == '0')
                {
                    nouvelEtat = false;
                    etatValide = true;
                }
                else
                {
                    Console.WriteLine("Veuillez entrer 1 ou 0.");
                }
            }
            while (!etatValide);

            // Mise à jour de l'état de la commande
            // Si l'etat de la commande est la même alors on ne change rien, sinon change et cela peremt de changer ses stastistiques
            try
            {
                Role.ConnexionCuisinier.Open();
                try
                {
                    string requette = "UPDATE Commande SET Etat_de_la_commande = @etat WHERE Id_Commande = @idCommande;";
                    MySqlCommand cmdUpdate = new MySqlCommand(requette, Role.ConnexionCuisinier);
                    cmdUpdate.Parameters.AddWithValue("@etat", nouvelEtat);
                    cmdUpdate.Parameters.AddWithValue("@idCommande", idCommandeAModifier);
                    int lignes = cmdUpdate.ExecuteNonQuery();
                    if (lignes > 0)
                    {
                        Console.WriteLine("La commande a été mise à jour avec succès.");
                    }
                    else
                    {
                        Console.WriteLine("Erreur lors de la mise à jour de la commande.");
                    }
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Erreur lors de la mise à jour de la commande : " + ex.Message);
                    Console.ReadKey();
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur lors de l'ouverture de la connexion : " + ex.Message);
                Console.ReadKey();
            }
            finally
            {
                try
                {
                    Role.ConnexionCuisinier.Close();
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Erreur lors de la fermeture de la connexion." + ex.Message);
                    Console.ReadKey();

                }
            }

            // Si la commande a été livrée, on propose de laisser un avis sur le client
            if (nouvelEtat)
            {
                Console.WriteLine("Vous pouvez maintenant laisser un avis sur le client...");
                Thread.Sleep(700);
                LaisserAvisSurClient(idCommandeAModifier);
            }

            Console.WriteLine("Appuyez sur une touche pour revenir au menu...");
            Console.ReadKey();
        }

        /// <summary>
        /// Vérifie qu'une commande existe et est associée aux plats du cuisinier.
        /// </summary>
        private static bool CommandeExistePourCuisinier(int idCommande)
        {
            bool existe = false;
            try
            {
                Role.ConnexionCuisinier.Open();
                try
                {
                    string requette = @"SELECT COUNT(*) 
                             FROM Commande c
                             JOIN Plat_Propose p ON c.Id_Plat = p.Id_Plat
                             WHERE c.Id_Commande = @idCommande AND p.Identifiant = @idCuisinier;";
                    MySqlCommand cmd = new MySqlCommand(requette, Role.ConnexionCuisinier);
                    cmd.Parameters.AddWithValue("@idCommande", idCommande);
                    cmd.Parameters.AddWithValue("@idCuisinier", Connexion.Identifiant);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    if (count > 0)
                        existe = true;
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Erreur lors de la vérification de l'existence de la commande : " + ex.Message);
                    Console.ReadKey();
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur lors de la connexion : " + ex.Message);
                Console.ReadKey();
            }
            try
            {
                Role.ConnexionCuisinier.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur lors de la fermeture de la connexion." + ex.Message);
                Console.ReadKey();

            }
            return existe;
        }

        /// <summary>
        /// Permet au cuisinier ou au Client de laisser un avis sur un client ou au Cuisinier respectivement.
        /// </summary>
        /// <param name="table"> Donne sur quelle table on donne un avie : Notation_Cuisinier ou Notation_Client </param>
        /// <param name="Id_Maker"> Id de celui qui fait le requette, donc si c'est le cuisinier qui donne un avie sur le client alors l'id_maker sera celui du Cuisinier</param>
        /// <param name="Id_Taker">Id de celui qui recoit l'avie, donc sur l'exemple precedent se sera Id_Client</param>
        /// <param name="Id_Commande">Permet d'associer l'avie avec la commande</param>

        public static void LaisserAvisSurClient(int Id_Commande)
        {
            Console.Clear();
            Console.WriteLine("=== Laisser un avis sur un client ===");

            // Récupérer l'ID du client associé à cette commande
            int idClient = 0;
            try
            {
                Role.ConnexionCuisinier.Open();
                try
                {
                    string requette = "SELECT Identifiant FROM Commande WHERE Id_Commande = @Id_Commande;";
                    MySqlCommand cmd = new MySqlCommand(requette, Role.ConnexionCuisinier);
                    cmd.Parameters.AddWithValue("@Id_Commande", Id_Commande);
                    idClient = Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Erreur lors de la récupération de l'ID client : " + ex.Message);
                    Console.ReadKey();
                    return;
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur lors de la connexion : " + ex.Message);
                Console.ReadKey();
                return;
            }
            try
            {
                Role.ConnexionCuisinier.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur lors de la fermeture de la connexion." + ex.Message);
                Console.ReadKey();

            }

            // Saisie de la note
            int note = 0;
            bool noteValide = false;
            do
            {
                Console.Write("Entrez la note pour ce client (0 à 5) : ");
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
            }
            while (!noteValide);

            // Saisie du commentaire (facultatif)
            Console.Write("Entrez un commentaire : ");
            string commentaire = Console.ReadLine().Trim();

            // Insertion de l'avis dans la table Notation_Client
            try
            {
                Role.ConnexionCuisinier.Open();
                try
                {
                    string requette = @"INSERT INTO Notation_Client 
                               (Notation, Commentaire, DateNotation, Id_Commande, Id_Cuisinier, Identifiant)
                               VALUES (@Notation, @Commentaire, @DateNotation, @Id_Commande, @Id_Cuisinier, @Identifiant);";
                    MySqlCommand cmdInser = new MySqlCommand(requette, Role.ConnexionCuisinier);
                    cmdInser.Parameters.AddWithValue("@Notation", note);
                    cmdInser.Parameters.AddWithValue("@Commentaire", commentaire);
                    cmdInser.Parameters.AddWithValue("@DateNotation", DateTime.Now);
                    cmdInser.Parameters.AddWithValue("@Id_Commande", Id_Commande);
                    cmdInser.Parameters.AddWithValue("@Id_Cuisinier", Connexion.Identifiant);
                    cmdInser.Parameters.AddWithValue("@Identifiant", idClient);

                    cmdInser.ExecuteNonQuery();
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Erreur lors de l'insertion de l'avis : " + ex.Message);
                    Console.ReadKey();

                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur lors de l'insertion de l'avis : " + ex.Message);
                Console.ReadKey();

            }
            try
            {
                Role.ConnexionCuisinier.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur lors de la fermeture de la connexion." + ex.Message);
                Console.ReadKey();

            }



            Console.WriteLine("\nMerci pour votre notation !");

        }








































        /// <summary>
        /// Affichage du profil de Cuisinier : ses stats et des avis sur lui
        /// </summary>
        public static void AfficherProfilCuisinier()
        {
            Console.Clear();
            Console.WriteLine("=== Votre Profil ===");

            try
            {
                Role.ConnexionCuisinier.Open();

                try
                {
                    // Récupération des compteurs dans la table Cuisinier
                    string requette = @"SELECT Nb_Total_De_Commande, Nb_De_Commande_En_Cours, 
                                      Nb_Total_De_Plat, Nb_De_Plat_En_Cours
                                      FROM Cuisinier 
                                      WHERE Identifiant = @id;";
                    MySqlCommand cmdProfil = new MySqlCommand(requette, Role.ConnexionCuisinier);
                    cmdProfil.Parameters.AddWithValue("@id", Connexion.Identifiant);
                    MySqlDataReader readerProfil = cmdProfil.ExecuteReader();

                    if (readerProfil.Read())
                    {
                        int nbTotalCommandes = readerProfil.GetInt32("Nb_Total_De_Commande");
                        int nbCommandesEnCours = readerProfil.GetInt32("Nb_De_Commande_En_Cours");
                        int nbTotalPlats = readerProfil.GetInt32("Nb_Total_De_Plat");
                        int nbPlatsEnCours = readerProfil.GetInt32("Nb_De_Plat_En_Cours");

                        Console.WriteLine("Nombre total de commandes réalisées : " + nbTotalCommandes);
                        Console.WriteLine("Nombre de commandes en cours (non livrées) : " + nbCommandesEnCours);
                        Console.WriteLine("Nombre total de plats proposés : " + nbTotalPlats);
                        Console.WriteLine("Nombre de plats en cours de préparation : " + nbPlatsEnCours);
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
                    string requetteNombreAvis = "SELECT COUNT(*) FROM Notation_Cuisinier WHERE Identifiant = @id;";
                    MySqlCommand cmdNombreAvis = new MySqlCommand(requetteNombreAvis, Role.ConnexionCuisinier);
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
                        string requette = "SELECT AVG(Notation) AS Moyenne FROM Notation_Cuisinier WHERE Identifiant = @id;";
                        MySqlCommand cmdMoy = new MySqlCommand(requette, Role.ConnexionCuisinier);
                        cmdMoy.Parameters.AddWithValue("@id", Connexion.Identifiant);
                        double moyenne = Convert.ToDouble(cmdMoy.ExecuteScalar());
                        Console.WriteLine("\nMoyenne des notes reçues : " + moyenne.ToString("0.00")); // permet d'arrondir la moyenne à 2 chiffres après la virgule

                        // Affichage des avis laissés par les clients
                        string queryAvis = @"SELECT nc.Notation, nc.Commentaire, nc.DateNotation, 
                                    p.Nom AS NomPlat, u.Pseudo AS PseudoClient
                                     FROM Notation_Cuisinier nc
                                     JOIN Commande c ON nc.Id_Commande = c.Id_Commande
                                     JOIN Plat_Propose p ON c.Id_Plat = p.Id_Plat
                                     JOIN Utilisateur u ON nc.Id_Client = u.Identifiant
                                     WHERE nc.Identifiant = @id
                                     ORDER BY nc.DateNotation DESC;";
                        MySqlCommand cmdAvis = new MySqlCommand(queryAvis, Role.ConnexionCuisinier);
                        cmdAvis.Parameters.AddWithValue("@id", Connexion.Identifiant);
                        MySqlDataReader readerAvis = cmdAvis.ExecuteReader();

                        Console.WriteLine("\n=== Avis laissés par les clients ===");
                        if (readerAvis.HasRows)
                        {
                            while (readerAvis.Read())
                            {
                                int note = readerAvis.GetInt32("Notation");
                                string commentaire = readerAvis.GetString("Commentaire");
                                DateTime dateNotation = readerAvis.GetDateTime("DateNotation");
                                string nomPlat = readerAvis.GetString("NomPlat");
                                string pseudoClient = readerAvis.GetString("PseudoClient");

                                Console.WriteLine("\nDate: " + dateNotation.ToString("dd/MM/yyyy"));
                                Console.WriteLine("Note: " + note);
                                Console.WriteLine("Plat: " + nomPlat);
                                Console.WriteLine("Client: " + pseudoClient);
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
                Role.ConnexionCuisinier.Close();
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



