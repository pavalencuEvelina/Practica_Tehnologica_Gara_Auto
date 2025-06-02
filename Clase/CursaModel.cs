using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practica_Gara_Auto.Clase
{
    public class CursaModel
    {
        public int IDCursa { get; set; }
        public double Km { get; set; }
        public string DenumireAfisaj { get; set; } // textul pe care-l vezi în ComboBox
        public override string ToString() => DenumireAfisaj;
    }

}
