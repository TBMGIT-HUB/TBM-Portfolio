using System;
using Visual_Studio;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visual_Studio
{
    /// <summary>
    /// Classe représentant une station de métro
    /// </summary>
    public class Station
    {

        /// <summary>
        /// Identifiant unique de la station
        /// </summary>
        public int Id { get; }


        /// <summary>
        /// Nom de la station
        /// </summary>
        public string Name { get; }


        /// <summary>
        /// Ligne principale de métro à laquelle la station appartient
        /// </summary>
        public string PrimaryLine { get; }


        /// <summary>
        /// Coordonnées géographiques de la station (longitude, latitude)
        /// </summary>
        public PointF GeoCoordinates { get; }



        /// <summary>
        /// Coordonnées à l'écran calculées pour l'affichage
        /// </summary>
        public PointF ScreenCoordinates { get; set; }




        /// <summary>
        /// Constructeur d'une station de métro
        /// </summary>
        /// <param name="id">Identifiant unique de la station</param>
        /// <param name="name">Nom de la station</param>
        /// <param name="line">Ligne principale de la station</param>
        /// <param name="longitude">Coordonnée longitude</param>
        /// <param name="latitude">Coordonnée latitude</param>
        public Station(int id, string name, string line, float longitude, float latitude)
        {
            Id = id;
            Name = name;
            PrimaryLine = line;
            GeoCoordinates = new PointF(longitude, latitude);
        }


        /// <summary>
        /// Représentation textuelle de la station
        /// </summary>
        /// <returns>Une chaîne formatée contenant l'ID, le nom et la ligne de la station</returns>
        public override string ToString() => $"{Id}: {Name} (Ligne {PrimaryLine})";

    }
}
