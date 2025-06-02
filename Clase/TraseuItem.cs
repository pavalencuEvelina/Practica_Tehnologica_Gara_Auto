using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practica_Gara_Auto.Clase
{
    class TraseuItem
    {
        public int IDTraseu { get; set; }
        public string LocalitatePornire { get; set; }
        public string LocalitateDestinatie { get; set; }

        public override string ToString()
            => $"{IDTraseu} : {LocalitatePornire} - {LocalitateDestinatie}";
    }
}
