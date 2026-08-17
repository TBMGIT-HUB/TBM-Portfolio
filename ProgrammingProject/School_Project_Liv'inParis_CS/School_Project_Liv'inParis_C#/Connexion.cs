using System;
using Visual_Studio;
using MySql.Data.MySqlClient;


// Chose à faire :
// - Pour plus tard, il faudra vérifier si le métro le plus proches est dans notres base de donnée. pour ne pas faire plenter le programme




namespace Visual_Studio
{
    /// <summary>
    /// Classe gérant la connexion des utilisateurs.
    /// </summary>
    class Connexion
    {
        // L'identifiant de l'utilisateur qui est connecté permettra de faire des requêtes SQL dans les autres classes
        private static int IdentifiantConnexion;

        /// <summary>
        /// Propriété d'accès à l'identifiant de connexion
        /// </summary>
        public static int Identifiant
        {
            get { return IdentifiantConnexion; }
            set { IdentifiantConnexion = value; } // permet de rétablir l'identifiant à 0 lors de la déconnexion du compte
        }

        /// <summary>
        /// Méthode de connexion principale
        /// </summary>
        public static void Login()
        {
            Console.Clear();
            Console.WriteLine("=== Connexion au compte ===");
            Console.WriteLine();
            Console.Write("Entrez votre email : ");
            string Email = Console.ReadLine();
            Console.Write("Entrez votre mot de passe : ");
            string Mot_De_Passe = Console.ReadLine();

            int count = 0;  // variable pour stocker le résultat du COUNT(*)

            try
            {
                // Ouvrir la connexion via Role.ConnexionConnexion_profil
                Role.ConnexionConnexion_profil.Open();
                try
                {
                    // Vérification de la correspondance entre Email et Mot_De_Passe
                    string testCorrespondance = "SELECT COUNT(*) FROM Utilisateur WHERE Email = @Email AND Mot_De_Passe = @Mot_De_Passe;";
                    MySqlCommand cmdTest = new MySqlCommand(testCorrespondance, Role.ConnexionConnexion_profil);
                    cmdTest.Parameters.AddWithValue("@Email", Email);
                    cmdTest.Parameters.AddWithValue("@Mot_De_Passe", Mot_De_Passe);
                    count = Convert.ToInt32(cmdTest.ExecuteScalar());
                }
                catch (MySqlException e)
                {
                    Console.WriteLine("Erreur test correspondance Email et Mot de passe : " + e.ToString());
                    Console.ReadKey();
                    return; // On quitte en cas d'erreur dans la requête
                }
            }
            catch (MySqlException e)
            {
                Console.WriteLine("Erreur lors de l'ouverture de la connexion (Connexion_profil) : " + e.ToString());
                Console.ReadKey();
                return; // On quitte en cas d'erreur d'ouverture de connexion
            }

            if (count == 1)
            {
                // Récupération de l'identifiant si la correspondance est trouvée
                Console.WriteLine("Connexion réussie !");
                try
                {
                    string recuperationIdentifiant = "SELECT Identifiant FROM Utilisateur WHERE Email = @Email AND Mot_De_Passe = @Mot_De_Passe;";
                    MySqlCommand cmdIdentifiant = new MySqlCommand(recuperationIdentifiant, Role.ConnexionConnexion_profil);
                    cmdIdentifiant.Parameters.AddWithValue("@Email", Email);
                    cmdIdentifiant.Parameters.AddWithValue("@Mot_De_Passe", Mot_De_Passe);
                    IdentifiantConnexion = Convert.ToInt32(cmdIdentifiant.ExecuteScalar());
                    Console.WriteLine("\nL'identifiant de l'utilisateur : " + IdentifiantConnexion);
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Erreur lors de la récupération de l'identifiant : " + ex.Message);
                    Console.ReadKey();
                    return;
                }
            }
            else
            {
                // Gestion des erreurs de connexion
                try
                {
                    // Vérification si l'Email est valide, pour informer à l'utilisateur si c'est son mot de pass qui est incorrect ou si c'est son email qui n'existe pas
                    string testCorrespondance = "SELECT COUNT(*) FROM Utilisateur WHERE Email = @Email;";
                    MySqlCommand cmdTest = new MySqlCommand(testCorrespondance, Role.ConnexionConnexion_profil);
                    cmdTest.Parameters.AddWithValue("@Email", Email);
                    count = Convert.ToInt32(cmdTest.ExecuteScalar());
                }
                catch (MySqlException e)
                {
                    Console.WriteLine("Erreur test correspondance Email et Mot de passe : " + e.ToString());
                    Console.ReadKey();
                    return; // On quitte en cas d'erreur dans la requête
                }

                if (count == 1)
                {
                    Console.WriteLine("Le mot de passe est incorrect.\n");
                }
                else
                {
                    Console.WriteLine("Identifiants incorrects.\n");
                }


                // Fermer la connexion après l'opération
                try
                {
                    Role.ConnexionConnexion_profil.Close();
                }
                catch (MySqlException e)
                {
                    Console.WriteLine("Erreur lors de la fermeture de la connexion : " + e.Message);
                    Console.ReadKey();
                }





                // Menu des options après échec
                Console.WriteLine("1. Réessayer la connexion");
                Console.WriteLine("2. Créer un compte");
                Console.WriteLine("3. Quitter");
                Console.Write("Votre choix : \n");
                char choix = '0';
                bool choixValide = false;
                do
                {
                    choix = Console.ReadKey().KeyChar;
                    Console.WriteLine();
                    if (choix == '1' || choix == '2' || choix == '3')
                    {
                        choixValide = true;
                    }
                    else
                    {
                        Console.WriteLine("Veuillez entrer 1, 2 ou 3");
                    }
                }
                while (!choixValide);



                switch (choix)
                {
                    case '1':
                        // On continue la boucle pour réessayer la connexion
                        Console.WriteLine("Réessai de connexion...");
                        Thread.Sleep(700);
                        Login();
                        choixValide = true;
                        break;
                    case '2':
                        // Appel à la création de compte
                        CreateAccount();
                        choixValide = true;
                        break;
                    case '3':
                        Console.WriteLine("Au revoir !");
                        break;
                    default:
                        Console.WriteLine("Choix invalide, réessayer.");
                        Thread.Sleep(700);
                        Console.Clear();
                        break;
                }
            }
            try
            {
                Role.ConnexionConnexion_profil.Close();
            }
            catch (MySqlException e)
            {
                Console.WriteLine("Erreur lors de la fermeture de la connexion : " + e.Message);
                Console.ReadKey();
            }
        }










        /// <summary>
        /// Vérifie l'existence d'une valeur dans une colonne de la table Utilisateur
        /// </summary>
        public static bool TupleExisteUtilisateur(string colonne, string valeur)
        {
            string table = "Utilisateur";
            int count = 0;
            try
            {
                Role.ConnexionCreationProfil.Open();

                string verification_tuple_dans_table = "SELECT COUNT(*) FROM " + table + " WHERE " + colonne + " = @valeur;";
                MySqlCommand cmd = new MySqlCommand(verification_tuple_dans_table, Role.ConnexionCreationProfil);
                try
                {
                    cmd.Parameters.AddWithValue("@valeur", valeur);
                    count = Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch (MySqlException e)
                {
                    Console.WriteLine(" Erreur lors de la vérification de colonne: " + colonne + "et de valeur : " + valeur + "\nEt l'erreure est : " + e.ToString());
                    Console.ReadKey();
                    return true; //Si il y a une erreur, on retourne true pour dire qu'il y a eu un preoblème avec la valeur
                }
            }
            catch (MySqlException e)
            {
                Console.WriteLine(" Erreur lors de la connexion CreationProfil pour la vérification du tuple dans la table Utilisateur: " + e.ToString());
                Console.ReadKey();
            }

            try
            {
                Role.ConnexionCreationProfil.Close();
            }
            catch (MySqlException e)
            {
                Console.WriteLine("Erreur lors de la fermeture de la connexion : " + e.Message);
                Console.ReadKey();
            }

            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



















        /// <summary>
        /// Méthode de création de compte utilisateur
        /// </summary>
        public static void CreateAccount()
        {
            string connexionString = "SERVER=localhost;PORT=3306;DATABASE=Database_Livin;UID=root;PASSWORD=root";


            Console.Clear();
            Console.WriteLine("=== Création de compte ===");

            // Demander si c'est une entreprise (O/N)
            bool EstEntreprise = false;
            char reponse;
            do
            {
                Console.Write("Est-ce une entreprise ? (O/N) : ");
                reponse = Console.ReadKey().KeyChar;
                if (reponse == 'O' || reponse == 'o')
                {
                    EstEntreprise = true;
                }
                else if (reponse == 'N' || reponse == 'n')
                {
                    EstEntreprise = false;
                }
                else
                {
                    Console.WriteLine("Veuillez répondre par O (oui) ou N (non).");
                }
            }
            while ((reponse != 'O' && reponse != 'o') && (reponse != 'N' && reponse != 'n'));
            Console.WriteLine();
            // Pour une entreprise le pseudo correspond au nom de l'entreprise
            string Pseudo = "";
            bool PseudoValide = false;
            if (EstEntreprise)
            {
                do
                {
                    Console.Write("Entrez le nom de votre entreprise (dans la table --> pseudo) : ");
                    Pseudo = Console.ReadLine().Trim();
                    if (string.IsNullOrEmpty(Pseudo))
                    {
                        Console.WriteLine("Le nom de l'entreprise ne peut pas être vide.");
                    }
                    else if (TupleExisteUtilisateur("Pseudo", Pseudo)) // Verifie si le pseudo est déjà dans la table
                    {
                        Console.WriteLine("Ce nom d'entreprise est déjà utilisé. Veuillez en choisir un autre.");
                    }
                    else
                    {
                        PseudoValide = true;
                    }

                }
                while (!PseudoValide);
            }
            else
            {
                // Pour un particulier, on demande le pseudo 
                do
                {
                    Console.Write("Entrez votre pseudo : ");
                    Pseudo = Console.ReadLine().Trim();
                    if (string.IsNullOrEmpty(Pseudo))
                    {
                        Console.WriteLine("Le pseudo ne peut pas être vide.");
                    }
                    else if (TupleExisteUtilisateur("Pseudo", Pseudo)) // Verifie si le pseudo est déjà dans la table
                    {
                        Console.WriteLine("Ce pseudo est déjà utilisé. Veuillez en choisir un autre.");
                    }
                    else
                    {
                        PseudoValide = true;
                    }
                }
                while (!PseudoValide);
            }




            // Saisie de l'email, avec vérification qu'il contient un '@'
            string Email = "";
            bool EmailValide = false;
            do
            {
                Console.Write("Entrez votre adresse email : ");
                Email = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(Email))
                {
                    Console.WriteLine("L'email ne peut pas être vide.");
                }
                else if (!Email.Contains("@"))
                {
                    Console.WriteLine("L'adresse email doit contenir un '@'.");
                }
                else if (TupleExisteUtilisateur("Email", Email)) // Verifie si l'email est déjà dans la table
                {
                    Console.WriteLine("Cet email est déjà utilisé. Veuillez en choisir un autre.");
                }
                else
                {
                    EmailValide = true;
                }
            }
            while (!EmailValide);



            // Saisie du mot de passe
            string password = "";
            do
            {
                Console.Write("Entrez votre mot de passe : ");
                password = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(password))
                {
                    Console.WriteLine("Le mot de passe ne peut pas être vide.");
                }
            }
            while (string.IsNullOrEmpty(password));

            // Demande du nom et prénom
            string NomUtilisateur = "";
            do
            {
                Console.Write("Entrez votre nom : ");
                NomUtilisateur = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(NomUtilisateur))
                {
                    Console.WriteLine("Le nom ne peut pas être vide.");
                }
            }
            while (string.IsNullOrEmpty(NomUtilisateur));

            string PrenomUtilisateur = "";
            do
            {
                Console.Write("Entrez votre prénom : ");
                PrenomUtilisateur = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(PrenomUtilisateur))
                {
                    Console.WriteLine("Le prénom ne peut pas être vide.");
                }
            }
            while (string.IsNullOrEmpty(PrenomUtilisateur));


            // Saisie du téléphone --> doit comporter exactement 10 chiffres
            string telephone;
            bool TelephoneValide = false;

            do
            {
                Console.Write("Entrez votre numéro de téléphone (10 chiffres) : ");
                telephone = Console.ReadLine().Trim();

                if (string.IsNullOrEmpty(telephone))
                {
                    Console.WriteLine("Le téléphone ne peut pas être vide.");
                }
                else if (!telephone.All(char.IsDigit) || telephone.Length != 10)
                {
                    Console.WriteLine("Le téléphone doit comporter exactement 10 chiffres.");
                }
                else if (TupleExisteUtilisateur("Telephone", telephone)) // Verifie si le téléphone est déjà dans la table
                {
                    Console.WriteLine("Ce numéro de téléphone est déjà utilisé. Veuillez en choisir un autre.");
                }
                else
                {
                    TelephoneValide = true;
                }

            } while (!TelephoneValide);






            // Saisie des informations pour pouvoir créer sa table adresse (numéro de rue, rue, ville, code postal, métro le plus proche)                     
            int NumeroRue = 0;
            bool NumeroRueValide = false;
            do
            {
                Console.Write("Entrez le numéro de rue : ");
                string input = Console.ReadLine().Trim();
                if (int.TryParse(input, out NumeroRue))
                {
                    NumeroRueValide = true;
                }
                else
                {
                    Console.WriteLine("Veuillez entrer un numéro valide.");
                }
            }
            while (!NumeroRueValide);


            string Rue = "";
            do
            {
                Console.Write("Entrez le nom de la rue : ");
                Rue = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(Rue))
                {
                    Console.WriteLine("La rue ne peut pas être vide.");
                }
            }
            while (string.IsNullOrEmpty(Rue));


            string Ville = "";
            do
            {
                Console.Write("Entrez la ville : ");
                Ville = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(Ville))
                {
                    Console.WriteLine("La ville ne peut pas être vide.");
                }
            }
            while (string.IsNullOrEmpty(Ville));


            int CodePostale = 0;
            bool CodePostaleValide = false;
            do
            {
                Console.Write("Entrez le code postal : ");
                string input = Console.ReadLine().Trim();
                if (int.TryParse(input, out CodePostale))
                {
                    CodePostaleValide = true;
                }
                else
                {
                    Console.WriteLine("Veuillez entrer un code postal valide.");
                }
            }
            while (!CodePostaleValide);


            string MetroProche = "";
            bool MetroValide = false;
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Arcs.csv");
            Graphe Graphe = new Graphe(filePath);
            do
            {
                Console.Write("Entrez le métro le plus proche : ");
                MetroProche = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(MetroProche))
                {
                    Console.WriteLine("Le métro le plus proche ne peut pas être vide.");
                }
                else
                {
                    //vérifie si le métro le plus proches est dans notres base de donnée.
                    try
                    {
                        
                        if (!Graphe.DicoIDNom.ContainsKey(MetroProche))
                        {
                            Console.WriteLine("ERREUR : La station mentionnée ne figure pas nos bases de données");
                        }
                        else
                        {
                            MetroValide = true;
                        }
                    }
                    catch
                    {
                        Console.WriteLine("ERREUR : Porblème dans connexion pour créer le graphe");
                    }
                }     
            }
            while (!MetroValide);

            
            


            
            Console.WriteLine("\n\nEn accédant à cette application, vous acceptez expressément les conditions suivantes :");
            Console.WriteLine("\n\t- Toutes vos données personnelles (informations d'identification, historiques, préférences, etc.) seront conservées pendant une durée maximale de 50 ans, \n\tsans possibilité de suppression anticipée sur demande.");
            Console.WriteLine("\n\t- Vous autorisez la vente de l'intégralité de vos informations personnelles à 238 partenaires commerciaux et tiers.");
            Console.WriteLine("\n\t- L'application ne saurait être tenue responsable en cas de fuite de données résultant d'une attaque informatique.");
            Console.WriteLine("\n\tIl n'y a bien évidemment aucun abus quant aux CGU de l'application");
            bool ConditionsAcceptees = false;
            do
            {
                Console.Write("\nAcceptez vous les conditions d'utilisation de l'application ? (O/N) : ");
                reponse = Console.ReadKey().KeyChar;
                Console.WriteLine();
                if (reponse == 'O' || reponse == 'o')
                {
                    ConditionsAcceptees = true;
                    Console.WriteLine("Merci d'avoir accepté les conditions d'utilisation.");
                }
                else if (reponse == 'N' || reponse == 'n')
                {
                    Console.WriteLine("Vous devez accepter les conditions pour continuer.");

                }
                else
                {
                    Console.WriteLine("Veuillez répondre par O (oui) ou N (non).");
                }
            }
            while (!ConditionsAcceptees);






            int Id_adresse;
            try
            {
                Role.ConnexionCreationProfil.Open();
                try
                {
                    Console.WriteLine("\nAjout des inforamations pour la table Adresse...");

                    string insertAdresse = "INSERT INTO Adresse (Numero_de_rue, Rue, Ville, Code_Postale, Metro_le_plus_proche) VALUES (@Numero, @Rue, @Ville, @CodePostal, @Metro);";
                    MySqlCommand cmdAdresse = new MySqlCommand(insertAdresse, Role.ConnexionCreationProfil);
                    cmdAdresse.Parameters.AddWithValue("@Numero", NumeroRue);
                    cmdAdresse.Parameters.AddWithValue("@Rue", Rue);
                    cmdAdresse.Parameters.AddWithValue("@Ville", Ville);
                    cmdAdresse.Parameters.AddWithValue("@CodePostal", CodePostale);
                    cmdAdresse.Parameters.AddWithValue("@Metro", MetroProche);
                    cmdAdresse.ExecuteNonQuery();
                    Id_adresse = (int)cmdAdresse.LastInsertedId;
                    Console.WriteLine("Adresse ajoutée avec succès. Id_adresse : " + Id_adresse);


                    try
                    {
                        Console.WriteLine("\nAjout des inforamations pour la table Utilisateur...");
                        string insertUtilisateur = "INSERT INTO Utilisateur (Pseudo, Email, Mot_De_Passe, Nom, Prenom, Telephone, Id_adresse) VALUES (@Pseudo, @Email, @Mot_De_Passe, @Nom, @Prenom, @Telephone, @Id_adresse);";

                        string insertUser = "INSERT INTO Utilisateur (Nom, Prenom, Pseudo, Email, Telephone, Mot_De_Passe, Id_adresse, Entreprise) " +
                                        "VALUES (@Nom, @Prenom, @Pseudo, @Email, @Telephone, @Mot_De_Passe, @Id_adresse, @Entreprise);";


                        MySqlCommand cmdUtilisateur = new MySqlCommand(insertUtilisateur, Role.ConnexionCreationProfil);
                        cmdUtilisateur.Parameters.AddWithValue("@Nom", NomUtilisateur);
                        cmdUtilisateur.Parameters.AddWithValue("@Prenom", PrenomUtilisateur);
                        cmdUtilisateur.Parameters.AddWithValue("@Pseudo", Pseudo);
                        cmdUtilisateur.Parameters.AddWithValue("@Email", Email);
                        cmdUtilisateur.Parameters.AddWithValue("@Telephone", telephone);
                        cmdUtilisateur.Parameters.AddWithValue("@Mot_De_Passe", password);
                        cmdUtilisateur.Parameters.AddWithValue("@Id_adresse", Id_adresse);
                        cmdUtilisateur.Parameters.AddWithValue("@Entreprise", EstEntreprise);
                        cmdUtilisateur.ExecuteNonQuery();
                        IdentifiantConnexion = (int)cmdUtilisateur.LastInsertedId;
                        Console.WriteLine("\nL'identifiant de l'utilisateur : " + IdentifiantConnexion);

                        // Création automatique des tables Cuisinier et Client en mettant les valeurs à 0
                        try
                        {
                            // Création de la table Cuisinier associé à l'utilisateur
                            string requeteCuisinier = @"INSERT INTO Cuisinier 
                                (Identifiant, Nb_Total_De_Plat, Nb_De_Plat_En_Cours, Nb_Total_De_Commande, Nb_De_Commande_En_Cours)
                                VALUES (@Identifiant, 0, 0, 0, 0);";
                            MySqlCommand cmdCuisinier = new MySqlCommand(requeteCuisinier, Role.ConnexionCreationProfil);
                            cmdCuisinier.Parameters.AddWithValue("@Identifiant", IdentifiantConnexion);
                            cmdCuisinier.ExecuteNonQuery();
                        }
                        catch (MySqlException ex)
                        {
                            Console.WriteLine("Erreur lors de la création de la table Cuisinier  associé à l'utilisateur :" + ex.Message);
                            Console.ReadKey();
                        }
                        try
                        {
                            // Création de la table Client associé à l'utilisateur
                            string requeteClient = @"INSERT INTO Client 
                                (Identifiant, Nb_Commande_Total, Nb_De_Commande_En_Cours)
                                VALUES (@Identifiant, 0, 0);";
                            MySqlCommand cmdClient = new MySqlCommand(requeteClient, Role.ConnexionCreationProfil);
                            cmdClient.Parameters.AddWithValue("@Identifiant", IdentifiantConnexion);
                            cmdClient.ExecuteNonQuery();
                        }
                        catch (MySqlException ex)
                        {
                            Console.WriteLine("Erreur lors de la création de la table Client associé à l'utilisateur " + ex.Message);
                            Console.ReadKey();
                        }
                    }
                    catch (MySqlException e)
                    {
                        Console.WriteLine(" Erreur lors de l'ajout des informations dans la table utilisateur : " + e.ToString());
                        try
                        {
                            //Supprimer le tuple qu'on vient d'ajouter dans la table Adresse, si nous avons une erreur lorsque nous ajoutons le tuple dans notre table utilisateur.
                            string DeleteAdresse = "DELETE FROM Adresse WHERE Id_adresse = @Id_adresse;";
                            MySqlCommand cmdDelete = new MySqlCommand(DeleteAdresse, Role.ConnexionCreationProfil);
                            cmdDelete.Parameters.AddWithValue("@Id_adresse", Id_adresse);
                            cmdDelete.ExecuteNonQuery();
                        }
                        catch (MySqlException ex)
                        {
                            Console.WriteLine(" Erreur lors de la suppression de l'adresse : " + ex.ToString());
                            Console.ReadKey();
                        }
                    }
                }
                catch (MySqlException e)
                {
                    Console.WriteLine(" Erreur lors de l'ajout des informations dans la table adresse : " + e.ToString());
                    Console.ReadKey();
                }
            }
            catch (MySqlException e)
            {
                Console.WriteLine(" Erreur lors de la connexion CreationProfil : " + e.ToString());
                Console.ReadKey();
            }

            try
            {
                Role.ConnexionCreationProfil.Close();
            }
            catch (MySqlException e)
            {
                Console.WriteLine("Erreur lors de la fermeture de la connexion : " + e.Message);
                Console.ReadKey();
            }

        }
    }
}
