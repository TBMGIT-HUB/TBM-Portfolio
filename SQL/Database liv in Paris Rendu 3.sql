DROP DATABASE IF EXISTS Database_Livin;
CREATE DATABASE Database_Livin;
USE Database_Livin;



-- Creation des tables :


CREATE TABLE IF NOT EXISTS Adresse(
    Id_adresse INT NOT NULL AUTO_INCREMENT,
    Numero_de_rue INT,
    Rue VARCHAR(50),
    Ville VARCHAR(50),
    Code_Postale INT,
    Metro_le_plus_proche VARCHAR(50),
    PRIMARY KEY(Id_adresse)
);

CREATE TABLE IF NOT EXISTS Ingredient(
    Nom_Ingredient_autorise VARCHAR(50),
    PRIMARY KEY(Nom_Ingredient_autorise)
);

CREATE TABLE IF NOT EXISTS Regime(
    Nom_Regime VARCHAR(50),
    PRIMARY KEY(Nom_Regime)
);

CREATE TABLE IF NOT EXISTS Recette(
    Recette_autorise VARCHAR(50),
    PRIMARY KEY(Recette_autorise)
);

CREATE TABLE IF NOT EXISTS Unite_de_mesure(
    Unite_autorise VARCHAR(50),
    PRIMARY KEY(Unite_autorise)
);

CREATE TABLE IF NOT EXISTS Nationalite(
    Nationalite_autorise VARCHAR(50),
    PRIMARY KEY(Nationalite_autorise)
);

CREATE TABLE IF NOT EXISTS Type_de_Preparation(
    Type_autorise VARCHAR(50),
    PRIMARY KEY(Type_autorise)
);

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
);

CREATE TABLE IF NOT EXISTS Cuisinier(
    Identifiant INT,
    Notation INT,
    Nb_Total_De_Plat INT,
    Nb_De_Plat_En_Cours INT,
    Nb_Total_De_Commande INT,
    Nb_De_Commande_En_Cours INT,
    PRIMARY KEY(Identifiant),
    FOREIGN KEY(Identifiant) REFERENCES Utilisateur(Identifiant)
);

CREATE TABLE IF NOT EXISTS Client(
    Identifiant INT,
    Notation INT,
    Nb_Commande_Total INT,
    Nb_De_Commande_En_Cours INT,
    PRIMARY KEY(Identifiant),
    FOREIGN KEY(Identifiant) REFERENCES Utilisateur(Identifiant)
);

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
    Photo VARCHAR(50),
    Recette_autorise VARCHAR(50) NOT NULL,
    Identifiant INT NOT NULL,
    PRIMARY KEY(Id_Plat),
    FOREIGN KEY(Recette_autorise) REFERENCES Recette(Recette_autorise),
    FOREIGN KEY(Nationalite) REFERENCES Nationalite(Nationalite_autorise),
    FOREIGN KEY(Type) REFERENCES Type_de_Preparation(Type_autorise),
    FOREIGN KEY(Identifiant) REFERENCES Cuisinier(Identifiant)
);

CREATE TABLE IF NOT EXISTS Ingredient_Total(
    Nom_Ingredient VARCHAR(50),
    Quantite_ INT,
    Unite VARCHAR(50),
    Unite_autorise VARCHAR(50),
    Id_Plat INT,
    Nom_Ingredient_autorise VARCHAR(50) NOT NULL,
    PRIMARY KEY(Id_Plat, Nom_Ingredient),
    FOREIGN KEY(Unite_autorise) REFERENCES Unite_de_mesure(Unite_autorise),
    FOREIGN KEY(Id_Plat) REFERENCES Plat_Propose(Id_Plat),
    FOREIGN KEY(Nom_Ingredient_autorise) REFERENCES Ingredient(Nom_Ingredient_autorise)
);

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
);

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
);

CREATE TABLE IF NOT EXISTS Commande(
    Id_Commande INT NOT NULL AUTO_INCREMENT,
    Trajet VARCHAR(50),
    Nb_de_part INT,
    Etat_de_la_commande BOOL,
    Id_Plat INT NOT NULL,
    Identifiant INT NOT NULL,
    PRIMARY KEY(Id_Commande),
    FOREIGN KEY(Id_Plat) REFERENCES Plat_Propose(Id_Plat),
    FOREIGN KEY(Identifiant) REFERENCES Client(Identifiant)
);

CREATE TABLE IF NOT EXISTS Suit_le_régime(
    Id_Plat INT,
    Nom_Regime VARCHAR(50),
    PRIMARY KEY(Id_Plat, Nom_Regime),
    FOREIGN KEY(Id_Plat) REFERENCES Plat_Propose(Id_Plat),
    FOREIGN KEY(Nom_Regime) REFERENCES Regime(Nom_Regime)
);




/*
-- Role 

-- Création des rôles

DROP ROLE IF EXISTS `admin_role`;
DROP ROLE IF EXISTS `creation_profil_role`;
DROP ROLE IF EXISTS `connexion_profil_role`;
DROP ROLE IF EXISTS `cuisinier_role`;
DROP ROLE IF EXISTS `client_role`;

DROP USER IF EXISTS 'user_admin'@'localhost';
DROP USER IF EXISTS 'user_creation_profil'@'localhost';
DROP USER IF EXISTS 'user_connexion_profil'@'localhost';
DROP USER IF EXISTS 'user_cuisinier'@'localhost';
DROP USER IF EXISTS 'user_client'@'localhost';

CREATE ROLE `admin_role`;
GRANT ALL PRIVILEGES ON Database_Livin.* TO `admin_role`;

CREATE ROLE `creation_profil_role`;
GRANT SELECT, INSERT, UPDATE, DELETE ON Utilisateur TO `creation_profil_role`;
GRANT SELECT, INSERT, UPDATE, DELETE ON Adresse TO `creation_profil_role`;
GRANT SELECT, INSERT, UPDATE, DELETE ON Cuisinier TO `creation_profil_role`;
GRANT SELECT, INSERT, UPDATE, DELETE ON Client TO `creation_profil_role`;

CREATE ROLE `connexion_profil_role`;
GRANT SELECT ON Utilisateur TO `connexion_profil_role`;

CREATE ROLE `cuisinier_role`;
GRANT SELECT, INSERT, UPDATE, DELETE ON Plat_Propose TO `cuisinier_role`;
GRANT SELECT, UPDATE ON Commande TO `cuisinier_role`;
GRANT SELECT, UPDATE ON Cuisinier TO `cuisinier_role`;
GRANT INSERT, UPDATE ON Notation_Client TO `cuisinier_role`;
GRANT SELECT ON Recette TO `cuisinier_role`;
GRANT SELECT ON Nationalite TO `cuisinier_role`;
GRANT SELECT ON Type_de_Preparation TO `cuisinier_role`;

CREATE ROLE `client_role`;
GRANT SELECT ON Plat_Propose TO `client_role`;
GRANT INSERT ON Commande TO `client_role`;
GRANT SELECT, UPDATE ON Client TO `client_role`;
GRANT SELECT, UPDATE ON Utilisateur TO `client_role`;

CREATE USER 'user_admin'@'localhost' IDENTIFIED BY 'root';
CREATE USER 'user_creation_profil'@'localhost' IDENTIFIED BY 'root';
CREATE USER 'user_connexion_profil'@'localhost' IDENTIFIED BY 'root';
CREATE USER 'user_cuisinier'@'localhost' IDENTIFIED BY 'root';
CREATE USER 'user_client'@'localhost' IDENTIFIED BY 'root';

GRANT `admin_role` TO 'user_admin'@'localhost';
GRANT `creation_profil_role` TO 'user_creation_profil'@'localhost';
GRANT `connexion_profil_role` TO 'user_connexion_profil'@'localhost';
GRANT `cuisinier_role` TO 'user_cuisinier'@'localhost';
GRANT `client_role` TO 'user_client'@'localhost';

SET DEFAULT ROLE ALL TO 'user_admin'@'localhost';
SET DEFAULT ROLE ALL TO 'user_creation_profil'@'localhost';
SET DEFAULT ROLE ALL TO 'user_connexion_profil'@'localhost';
SET DEFAULT ROLE ALL TO 'user_cuisinier'@'localhost';
SET DEFAULT ROLE ALL TO 'user_client'@'localhost';

*/



-- Code : 

-- Ajout des utlisateurs :

DESCRIBE Utilisateur; -- Vérification de la création du tableau

-- Ajout des clients 
-- Dans un premier temps, nous devons insérer l'adresse pour pouvoir créer l'utilisateur, puis nous pouvons créer son profil Client à 'Durand Medhy'
INSERT INTO Adresse (Id_adresse, Numero_de_rue, Rue, Ville, Code_Postale, Metro_le_plus_proche) 
VALUES (1, 15, 'Rue Cardinet', 'Paris', 75017, 'Cardinet');
INSERT INTO Utilisateur (Identifiant, Nom, Prenom, Pseudo, Email, Telephone, Id_adresse) 
VALUES (1, 'Durand', 'Medhy', 'DurandM', 'Mdurand@gmail.com', 1234567890, 1);
INSERT INTO Client (Identifiant, Notation, Nb_Commande_Total, Nb_De_Commande_En_Cours) 
VALUES (1, 0, 0, 0);

-- Test ajout du premier client :
SELECT * FROM Utilisateur WHERE Nom = 'Durand';

-- Ajout des cuisiniers
-- Même procédé qu'avec le premier client, mais cette fois-ci pour le cuisinier 'Dupond Marie'
INSERT INTO Adresse (Id_adresse, Numero_de_rue, Rue, Ville, Code_Postale, Metro_le_plus_proche) 
VALUES (2, 30, 'Rue de la République', 'Paris', 75011, 'République');
-- NB : Dans le tableau Excel, il est écrit que le cuisinier possède également un Identifiant 1, ce qui ne va pas dans notre cas car le cuisinier et le client partagent le même compte utilisateur, ainsi nous changeons le numéro d'identifiant à 2
INSERT INTO Utilisateur (Identifiant, Nom, Prenom, Pseudo, Email, Telephone, Id_adresse) 
VALUES (2, 'Dupond', 'Marie', 'DupondM', 'Mdupond@gmail.com', 987654321, 2); -- Son numéro de téléphone a été changer, car dans la logique des choses veut que nous ne pouvons pas associer un même numéro à deux clients car un numéro de téléphone ne se partage pas et est donc unique à une entitée.
INSERT INTO Cuisinier (Identifiant, Notation, Nb_Total_De_Plat, Nb_De_Plat_En_Cours, Nb_Total_De_Commande, Nb_De_Commande_En_Cours) 
VALUES (2, 0, 0, 0, 0, 0);

-- Test ajout du premier cuisinier 
SELECT * FROM Cuisinier WHERE Identifiant = 2;

-- Insertion des plats proposés
-- Dans un premier temps, avant de rajouter des plats à notre cuisinier, nous devons ajouter les recettes dans notre liste de recettes proposées dans notre banque de données
INSERT INTO Recette (Recette_autorise) VALUES ('Raclette'), ('Salade de fruit');
-- Insertion des nationalités
INSERT INTO Nationalite (Nationalite_autorise) VALUES ('Française'),("Indifférent");
-- Insertion des Type de plat 
INSERT INTO Type_de_Preparation (Type_autorise) VALUES ('Plat'),("Dessert");

-- Premier plat : Raclette 
INSERT INTO Plat_Propose (Id_Plat, Nom, Type, Nb_De_Client, Prix_par_personne, Variante_de_recette_bool, Nationalite, Date_de_fabrication, Date_de_peremption, Photo, Recette_autorise) 
VALUES (1, 'Raclette', 'Plat', 6, 10, 0, 'Française', '2025-01-10', '2025-01-15', NULL, 'Raclette');
-- Deuxième plat : Salade de fruit
INSERT INTO Plat_Propose (Id_Plat, Nom, Type, Nb_De_Client, Prix_par_personne, Variante_de_recette_bool, Nationalite, Date_de_fabrication, Date_de_peremption, Photo, Recette_autorise) 
VALUES (2, 'Salade de fruit', 'Dessert', 6, 5, 0, 'Indifférent', '2025-01-10', '2025-01-15', NULL, 'Salade de fruit');

-- Rattachement du cuisinier aux plats via la table 'Propose'
INSERT INTO Propose (Identifiant, Id_Plat) VALUES (2, 1);
INSERT INTO Propose (Identifiant, Id_Plat) VALUES (2, 2);

-- Maintenant nous devons modifier les informations du profil du Cuisinier 'Dupond Marie' car maintenant il propose un plat , il a donc des commandes en cours
UPDATE Cuisinier SET Nb_Total_De_Plat = 2, Nb_De_Plat_En_Cours = 2, Nb_Total_De_Commande = 2, Nb_De_Commande_En_Cours = 2 WHERE Identifiant = 2;

-- Ajout des commandes passées par le client
INSERT INTO Commande (Id_Commande, Trajet, Id_Plat, Identifiant) 
VALUES (1, 'République --> Cardinet', 1, 1);
INSERT INTO Commande (Id_Commande, Trajet, Id_Plat, Identifiant) 
VALUES (2, 'République --> Cardinet', 2, 1);

-- Même chose avec le client, modification de des ses commandes :
UPDATE Client SET Nb_Commande_Total = 2, Nb_De_Commande_En_Cours = 2 WHERE Identifiant = 1;

-- Insertion des régimes et association aux plats
INSERT INTO Regime (Nom_Regime) 
VALUES ('Végétarien');
-- Association du régime au plat via la table 'Suit_le_régime', ici seul "Salade de fruit" possède un régime particulier
INSERT INTO Suit_le_régime (Id_Plat, Nom_Regime) 
VALUES (2, 'Végétarien');


-- Insertion des ingrédients aux plats
-- Avant cela, nous devons remplir la table des unités pour s'assurer qu'il n'y ait pas d'unités inventées par les utilisateurs
INSERT INTO Unite_de_mesure (Unite_autorise) VALUES ('g'), ('Pièces');

-- Pour la Raclette :
-- D'abord insertion des noms d'ingrédients dans la table Ingredient (les ingrédients autorisés)
INSERT INTO Ingredient (Nom_Ingredient_autorise) 
VALUES ('raclette fromage'), ('pommes_de_terre'), ('jambon'), ('cornichon');
-- Puis insertion des détails dans la table Ingredient_Total
INSERT INTO Ingredient_Total (Nom_Ingredient, Quantite_, Unite)
VALUES ('raclette fromage', 250, 'g'),
       ('pommes_de_terre', 200, 'g'),
       ('jambon', 200, 'g'),
       ('cornichon', 3, 'Pièces');

-- Pour la Salade de fruit :
-- D'abord insertion des noms d'ingrédients dans la table Ingredient
INSERT INTO Ingredient (Nom_Ingredient_autorise) 
VALUES ('fraise'), ('kiwi'), ('sucre');
-- Puis insertion des détails dans la table Ingredient_Total
INSERT INTO Ingredient_Total (Nom_Ingredient, Quantite_, Unite)
VALUES ('fraise', 100, 'g'),
       ('kiwi', 100, 'g'),
       ('sucre', 10, 'g');





-- Nous avons dans un premier temps ajouté uniquement les éléments essentiels pour pouvoir ajouter le client 'Durand Medhy' et le cuisinier 'Dupond Marie', ainsi que leurs plats associés.
-- Nous allons augmenter les données dans les tables des ingrédients autorisés, unités de mesure, régimes, des types de préparation, des nationalités, et Recette.

INSERT INTO Unite_de_mesure (Unite_autorise) VALUES ('kg'), ('ml'), ('l'), ('Tasses'), ('Cuillères à soupe'), ('Cuillères à café'), ('Branche');
INSERT INTO Regime (Nom_Regime) VALUES ('Vegan'), ('Sans gluten'), ('Halal'), ('Casher');
INSERT INTO Type_de_Preparation (Type_autorise) VALUES ('Autre'), ('Entrée'), ('Boisson'), ('Apéritif'), ('Snack'), ('Accompagnement');
INSERT INTO Nationalite (Nationalite_autorise) VALUES ('Italienne'), ('Mexicaine'), ('Indienne'), ('Japonaise'), ('Chinoise'), ('Espagnole'), ('Thaïlandaise'), ('Marocaine'), ('Américaine');
INSERT INTO Recette (Recette_autorise) VALUES ('Création originale'), ('Inconue'), ('Pizza'), ('Burger'), ('Pâtes'), ('Tacos'), ('Sushi'), ('Quiche'), ('Gratin'), ('Soupe'), ('Sandwich'), ("Socktail"), ("Smoothie");
INSERT INTO Ingredient (Nom_Ingredient_autorise) VALUES ('Autre'), ('tomate'), ('pomme'), ('salade'), ('fromage'), ('riz'), ('poulet'), ('poivron'), ('oignon'), ('ail'), ('basilic'), ('champignon'), ('courgette'), ('carotte'), ('farine'), ('beurre'), ('lait'), ('œuf'), ('thym');





-- Test :


-- Nous allons maintenant tester des lignes de codes sur notre base de donée :

-- Affichage du contenu des tableaux principaux 
SELECT * FROM Utilisateur;  -- Voir tous les utilisateurs
SELECT * FROM Adresse;  -- Voir tous les adresses des utilisateurs
SELECT * FROM Plat_Propose; -- Voir tous les plats proposés
SELECT * FROM Ingredient_Total; -- Voir les ingrédients et leurs quantités
SELECT * FROM Commande; -- Voir les commandes passées
SELECT * FROM Recette; -- Voir les recettes enregistrées
SELECT * FROM Regime; -- Voir les régimes alimentaires définis
SELECT * FROM Nationalite; -- Voir les nationalités enregistrées
SELECT * FROM Type_de_Preparation; -- Voir les types de préparation existants


-- Affichage des profils des utlisateurs :
SELECT * FROM Utilisateur WHERE Nom = 'Durand';
SELECT * FROM Client WHERE Identifiant = 1;
SELECT * FROM Utilisateur WHERE Nom = 'Dupond';
SELECT * FROM Cuisinier WHERE Identifiant = 2;

-- Afficher la liste des nationalités autorisées par ordre alphabétique inverse
SELECT * FROM Nationalite ORDER BY Nationalite_autorise DESC;

-- Création d'un nouveau plat proposé par le cuisinier pour y faire des modifications :
INSERT INTO Plat_Propose (Id_Plat, Nom, Type, Nb_De_Client, Prix_par_personne, Variante_de_recette_bool, Nationalite, Date_de_fabrication, Date_de_peremption, Photo, Recette_autorise) 
VALUES (3, 'Pizza', 'Plat', 4, 12, 1, 'Italienne', '2025-02-01', '2025-02-05', NULL, 'Création originale');

-- Affichage du nouveau plat :
SELECT * FROM Plat_Propose WHERE Id_Plat = 3;

-- Mise à jour du nombre de parts du nouveau plat (modification de Nb_De_Client de 4 à 8)
UPDATE Plat_Propose SET Nb_De_Client = 8 WHERE Id_Plat = 3;

-- Affichage du plat modifié pour vérifier la mise à jour
SELECT Nb_De_Client FROM Plat_Propose WHERE Id_Plat = 3;

-- Calcul du nombre d'utilisatueur dans notre base de donné :
SELECT Count(Identifiant) FROM Utilisateur;

-- Calcul du nombre d'utilisateur dont le nom est 'Dupond' :
SELECT Count(Nom) FROM Utilisateur WHERE nom = 'Dupond';


