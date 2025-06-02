using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practica_Gara_Auto.Clase
{
    public class UtilizatorModel
    {
            public string Nume { get; set; }
            public string Prenume { get; set; }
            public int IdUtilizator { get; set; }
             public int IdPersoana { get; set; }
          public string Email { get; set; }
            public string Rol { get; set; }

        public string Post { get; set; }
        public string NumeComplet => string.IsNullOrWhiteSpace(Nume) ? Email : $"{Nume} {Prenume}";
    }
}
