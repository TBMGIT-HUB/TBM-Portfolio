using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MySql.Data.MySqlClient;
using System.Xml.Serialization;
using Visual_Studio;
using MySql.Data.MySqlClient;
using System.Data;



namespace Visual_Studio 
{
    /// <summary>
    /// Classe qui représente un utilisateur à exporter.
    /// </summary>
    public class UtilisateurExport
    {
        public int Identifiant { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Pseudo { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public int Id_adresse { get; set; }
        public bool Entreprise { get; set; }

        public UtilisateurExport() { }
    }



    /// <summary>
    /// Classe qui représente un client (lié à un utilisateur) à exporter.
    /// </summary>
    public class ClientExport
    {
        public int Identifiant { get; set; } // Clé étrangère vers Utilisateur.Identifiant
        public int Nb_Commande_Total { get; set; }
        public int Nb_De_Commande_En_Cours { get; set; }

        public ClientExport() { }
    }

    /// <summary>
    /// Classe qui représente un cuisinier (lié à un utilisateur) à exporter.
    /// </summary>
    public class CuisinierExport
    {
        public int Identifiant { get; set; } // Clé étrangère vers Utilisateur.Identifiant
        public int Nb_Total_De_Plat { get; set; }
        public int Nb_De_Plat_En_Cours { get; set; }
        public int Nb_Total_De_Commande { get; set; }
        public int Nb_De_Commande_En_Cours { get; set; }

        public CuisinierExport() { }
    }




    /// <summary>
    /// Classe qui représente un plat proposé à exporter.
    /// </summary>
    public class PlatProposeExport
    {
        public int Id_Plat { get; set; }
        public string Nom { get; set; }
        public string Type { get; set; }
        public bool Variante_de_recette_bool { get; set; }
        public string Nationalite { get; set; }
        public int Nb_De_Client { get; set; }
        public int Prix_par_personne { get; set; }
        public DateTime Date_de_fabrication { get; set; }
        public DateTime Date_de_peremption { get; set; }
        public string Recette_autorise { get; set; }
        public int Identifiant_Cuisinier { get; set; }

        public PlatProposeExport() { }
    }


    /// <summary>
    /// Classe qui représente une commande à exporter.
    /// </summary>
    public class CommandeExport
    {
        public int Id_Commande { get; set; }
        public int Nb_de_part { get; set; }
        public bool Etat_de_la_commande { get; set; }
        public int Id_Plat { get; set; }
        public int Identifiant_Client { get; set; }

        public CommandeExport() { }
    }


    /// <summary>
    /// Classe qui représente les données à exporter de la base de données.
    /// </summary>
    public class ExportData
    {

        public List<UtilisateurExport> Utilisateurs { get; set; }

        public List<ClientExport> Clients { get; set; }

        public List<CuisinierExport> Cuisiniers { get; set; }

        public List<PlatProposeExport> PlatsProposes { get; set; }

        public List<CommandeExport> Commandes { get; set; }

        public ExportData()
        {
            Utilisateurs = new List<UtilisateurExport>();
            Clients = new List<ClientExport>(); 
            Cuisiniers = new List<CuisinierExport>(); 
            PlatsProposes = new List<PlatProposeExport>();
            Commandes = new List<CommandeExport>();
        }
    }

    /// <summary>
    /// Classe responsable de l'extraction et de l'exportation des données de la base.
    /// </summary>
    public class DatabaseExporter
    {

        /// <summary>
        /// Récupère les données de la base de données et les structure dans un objet ExportData.
        /// </summary>
        /// <returns></returns>
        public ExportData StructureBDD()
        {
            ExportData exportData = new ExportData();

            try
            {
                Role.ConnexionAdmin.Open();
                Console.WriteLine("Connexion à la base de données réussie.");

                exportData.Utilisateurs = StructureUtilisateurs();
                Console.WriteLine("Récupéré " + exportData.Utilisateurs.Count + " utilisateurs.");

                exportData.Clients = StructureClients();
                Console.WriteLine("Récupéré " + exportData.Clients.Count + " clients.");

                exportData.Cuisiniers = StructureCuisiniers();
                Console.WriteLine("Récupéré " + exportData.Cuisiniers.Count + " cuisiniers.");

                exportData.PlatsProposes = StructurePlatsProposes();
                Console.WriteLine("Récupéré " + exportData.PlatsProposes.Count + " plats proposés.");

                exportData.Commandes = StructureCommandes();
                Console.WriteLine("Récupéré " + exportData.Commandes.Count + " commandes.");


                Role.ConnexionAdmin.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur MySQL : " + ex.Message);
                return null;
            }
            return exportData;
        }


        /// <summary>
        /// Récupère les utilisateurs de la base de données et les structure dans une liste d'objets UtilisateurExport.
        /// </summary>
        /// <returns></returns>
        private List<UtilisateurExport> StructureUtilisateurs()
        {
            List<UtilisateurExport> utilisateurs = new List<UtilisateurExport>();
            MySqlDataReader reader = null;
            try
            {
                string requete = "SELECT Identifiant, Nom, Prenom, Pseudo, Email, Telephone, Id_adresse, Entreprise FROM Utilisateur";
                MySqlCommand commande = new MySqlCommand(requete, Role.ConnexionAdmin);
                reader = commande.ExecuteReader();
                while (reader.Read())
                {
                    UtilisateurExport user = new UtilisateurExport
                    {
                        Identifiant = reader.GetInt32("Identifiant"),
                        Nom = reader.GetString("Nom"),
                        Prenom = reader.GetString("Prenom"),
                        Pseudo = reader.GetString("Pseudo"),
                        Email = reader.GetString("Email"),
                        Telephone = reader.GetString("Telephone"),
                        Id_adresse = reader.GetInt32("Id_adresse"),
                        Entreprise = reader.GetBoolean("Entreprise")
                    };
                    utilisateurs.Add(user);
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("La table est vide SQL: " + ex.Message);

            }
            catch (Exception ex)
            {
                Console.WriteLine("La table est vide : " + ex.Message);
            }
            reader.Close();
          
            return utilisateurs;
        }




        // <summary>
        /// Récupère les données des clients depuis la base de données.
        /// </summary>
        /// <returns>Une liste d'objets ClientExport.</returns>
        private List<ClientExport> StructureClients()
        {
            List<ClientExport> clients = new List<ClientExport>();
            MySqlDataReader reader = null;
            try
            {
                string requete = "SELECT Identifiant, Nb_Commande_Total, Nb_De_Commande_En_Cours FROM Client";
                MySqlCommand commande = new MySqlCommand(requete, Role.ConnexionAdmin);
                reader = commande.ExecuteReader();
                while (reader.Read())
                {
                    ClientExport client = new ClientExport
                    {
                        Identifiant = reader.GetInt32("Identifiant"),
                        Nb_Commande_Total = reader.GetInt32("Nb_Commande_Total"),
                        Nb_De_Commande_En_Cours = reader.GetInt32("Nb_De_Commande_En_Cours")
                    };
                    clients.Add(client);
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("La table est vide SQL: " + ex.Message);

            }
            catch (Exception ex)
            {
                Console.WriteLine("La table est vide : " + ex.Message);
            }
            reader.Close();

            return clients;
        }

        /// <summary>
        /// Récupère les données des cuisiniers depuis la base de données.
        /// </summary>
        /// <returns>Une liste d'objets CuisinierExport.</returns>
        private List<CuisinierExport> StructureCuisiniers()
        {
            List<CuisinierExport> cuisiniers = new List<CuisinierExport>();
            MySqlDataReader reader = null;
            try
            {
                string requete = "SELECT Identifiant, Nb_Total_De_Plat, Nb_De_Plat_En_Cours, Nb_Total_De_Commande, Nb_De_Commande_En_Cours FROM Cuisinier";
                MySqlCommand commande = new MySqlCommand(requete, Role.ConnexionAdmin);
                reader = commande.ExecuteReader();
                while (reader.Read())
                {
                    CuisinierExport cuisinier = new CuisinierExport
                    {
                        Identifiant = reader.GetInt32("Identifiant"),
                        Nb_Total_De_Plat = reader.GetInt32("Nb_Total_De_Plat"),
                        Nb_De_Plat_En_Cours = reader.GetInt32("Nb_De_Plat_En_Cours"),
                        Nb_Total_De_Commande = reader.GetInt32("Nb_Total_De_Commande"),
                        Nb_De_Commande_En_Cours = reader.GetInt32("Nb_De_Commande_En_Cours")
                    };
                    cuisiniers.Add(cuisinier);
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("La table est vide SQL: " + ex.Message);

            }
            catch (Exception ex)
            {
                Console.WriteLine("La table est vide : " + ex.Message);
            }
            reader.Close();

            return cuisiniers;
        }



        /// <summary>
        /// Récupère les plats proposés de la base de données et les structure dans une liste d'objets PlatProposeExport.
        /// </summary>
        /// <returns></returns>
        private List<PlatProposeExport> StructurePlatsProposes()
        {
            List<PlatProposeExport> plats = new List<PlatProposeExport>();
            MySqlDataReader reader = null;
            try
            {
                string requete = "SELECT Id_Plat, Nom, Type, Variante_de_recette_bool, Nationalite, Nb_De_Client, Prix_par_personne, Date_de_fabrication, Date_de_peremption, Recette_autorise, Identifiant FROM Plat_Propose";
                MySqlCommand commande = new MySqlCommand(requete, Role.ConnexionAdmin);
                reader = commande.ExecuteReader();
                while (reader.Read())
                {
                    PlatProposeExport plat = new PlatProposeExport
                    {
                        Id_Plat = reader.GetInt32("Id_Plat"),
                        Nom = reader.GetString("Nom"),
                        Type = reader.GetString("Type"),
                        Variante_de_recette_bool = reader.GetBoolean("Variante_de_recette_bool"),
                        Nationalite = reader.GetString("Nationalite"),
                        Nb_De_Client = reader.GetInt32("Nb_De_Client"),
                        Prix_par_personne = reader.GetInt32("Prix_par_personne"),
                        Date_de_fabrication = reader.GetDateTime("Date_de_fabrication"),
                        Date_de_peremption = reader.GetDateTime("Date_de_peremption"),
                        Recette_autorise = reader.GetString("Recette_autorise"),
                        Identifiant_Cuisinier = reader.GetInt32("Identifiant")
                    };
                    plats.Add(plat);
                }
            }
            catch(MySqlException ex)
            {
                Console.WriteLine("La table est vide SQL: " + ex.Message);

            }
            catch (Exception ex)
            {
                Console.WriteLine("La table est vide : " + ex.Message);
            }
            reader.Close();

            return plats;
        }



        /// <summary>
        /// Récupère les commandes de la base de données et les structure dans une liste d'objets CommandeExport.
        /// </summary>
        /// <returns></returns>
        private List<CommandeExport> StructureCommandes()
        {
            List<CommandeExport> commandes = new List<CommandeExport>();
            MySqlDataReader reader = null;
            try
            {
                string requete = "SELECT Id_Commande, Nb_de_part, Etat_de_la_commande, Id_Plat, Identifiant FROM Commande";
                MySqlCommand commande = new MySqlCommand(requete, Role.ConnexionAdmin);
                reader = commande.ExecuteReader();
                while (reader.Read())
                {
                    CommandeExport cmd = new CommandeExport
                    {
                        Id_Commande = reader.GetInt32("Id_Commande"),
                        Nb_de_part = reader.GetInt32("Nb_de_part"),
                        Etat_de_la_commande = reader.GetBoolean("Etat_de_la_commande"),
                        Id_Plat = reader.GetInt32("Id_Plat"),
                        Identifiant_Client = reader.GetInt32("Identifiant")
                    };
                    commandes.Add(cmd);
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("La table est vide SQL: " + ex.Message);

            }
            catch (Exception ex)
            {
                Console.WriteLine("La table est vide : " + ex.Message);
            }
            
            reader.Close();
            return commandes;
        }

    }
}
