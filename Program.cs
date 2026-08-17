using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using UnitTest_Boogle;

namespace Boogle
{
    class Program
    {
        public static void Main(string[] args)
        {
            #region TestUnit
            Console.WriteLine("Testunitaire :\n");
            UnitTest1 Test = new UnitTest1();
            Console.WriteLine("\n\n\t"+Test.TestUnitaireDesMéthodes()+"\n\n");
            #endregion

            #region JEU
            Jeu j = new Jeu();
            j.Lancer_jeu();
            #endregion
        }
    }
}
