using System;
using Visual_Studio;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Visual_Studio;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Visual_Studio
{
    /// <summary>
    /// Représente une connexion dirigée entre deux stations de métro
    /// </summary>
    public class Lien
    {
        /// <summary>
        /// Station d'origine de la connexion
        /// </summary>
        public Station FromStation { get; }

        /// <summary>
        /// Station de destination de la connexion
        /// </summary>
        public Station ToStation { get; }

        /// <summary>
        /// Ligne de transport associée à cette connexion
        /// </summary>
        public string Line { get; }

        /// <summary>
        /// Indique si la connexion est bidirectionnelle
        /// </summary>
        /// <value>
        /// True si la connexion fonctionne dans les deux sens, False pour un sens unique
        /// </value>
        public bool IsBidirectional { get; set; }


        /// <summary>
        /// Constructeur pour créer une connexion entre stations
        /// </summary>
        /// <param name="from">Station de départ</param>
        /// <param name="to">Station d'arrivée</param>
        /// <param name="line">Identifiant de ligne (ex: "M1", "RER-B")</param>
        public Lien(Station from, Station to, string line)
        {
            FromStation = from;
            ToStation = to;
            Line = line;
            IsBidirectional = false; // Assume unidirectional unless proven otherwise
        }


        /// <summary>
        /// Génère une clé unique pour cette connexion spécifique
        /// </summary>
        /// <returns>
        /// Tuple contenant (ID station départ, ID station arrivée, ligne)
        /// </returns>
        public Tuple<int, int, string> GetKey() => Tuple.Create(FromStation.Id, ToStation.Id, Line);


        /// <summary>
        /// Génère la clé inverse de la connexion (pour recherche de trajet retour)
        /// </summary>
        /// <returns>
        /// Tuple contenant (ID station arrivée, ID station départ, ligne)
        /// </returns>
        public Tuple<int, int, string> GetReverseKey() => Tuple.Create(ToStation.Id, FromStation.Id, Line);
    }
}
