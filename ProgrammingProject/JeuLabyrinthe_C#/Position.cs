using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JeuLabyrinthe
{
    internal class Position
    {
        #region Attributs
        private int numLigne;
        private int numColonne;
        #endregion

        #region Constructeur
        public Position(int numLigne, int numColonne)
        {
            if (numLigne >= 0)
            {
                this.numLigne = numLigne;
            }
            else
            {
                throw new Exception("Le numéro de ligne ne peut pas être négatif.");
            }

            if (numColonne >= 0)
            {
                this.numColonne = numColonne;
            }
            else
            {
                throw new Exception("Le numéro de colonne ne peut pas être négatif.");
            }
        }
        #endregion

        #region Propriétés
        public int NumLigne
        {
            get { return numLigne; }
            set
            {
                if (value >= 0) numLigne = value;
                else throw new Exception("Le numéro de ligne ne peut pas être négatif.");
            }
        }

        public int NumColonne
        {
            get { return numColonne; }
            set
            {
                if (value >= 0) numColonne = value;
                else throw new Exception("Le numéro de colonne ne peut pas être négatif.");
            }
        }
        #endregion

        #region Méthodes
        public override string ToString()
        {
            return $"Position : \nx = {numLigne}\ny = {numColonne}";
        }

        public bool EstEgale(Position pos)
        {
            return this.numColonne == pos.NumColonne && this.numLigne == pos.NumLigne;
        }
        #endregion
    }
}
