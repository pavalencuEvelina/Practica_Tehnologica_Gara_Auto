using DocumentFormat.OpenXml.Office.Word;
using Microsoft.Data.SqlClient;
using Practica_Gara_Auto.Clase;
using Practica_Gara_Auto.Connection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Practica_Gara_Auto.WindowsFile
{
    /// <summary>
    /// Interaction logic for VeziBiletele.xaml
    /// </summary>
    public partial class VeziBiletele : UserControl
    {
        UtilizatorModel u;
        public VeziBiletele(UtilizatorModel u)
        {
            this.u = u;
            InitializeComponent();
            LoadBilete();
        }


        public void LoadBilete()
        {

            var db = new DataBaseConnection();
            DataTable _bileteTable = new DataTable();
            string query;
            if (u.Rol == "Angajat")
            {
                query = "SELECT IDBilet, NumePasager, PrenumePasager, Cursa, DataRezervare, Nr_loc FROM dbo.Bilete_Total";

                using (SqlDataAdapter adapter = new SqlDataAdapter(query,db.Connection))
                {
                    adapter.Fill(_bileteTable);
                }
            }
            else
            {
                query = "SELECT IDBilet, NumePasager, PrenumePasager, Cursa, DataRezervare, Nr_loc FROM dbo.Bilete_Total WHERE IDPasager=@Idp";
                using (SqlCommand cmd = new SqlCommand(query, db.Connection))
                {
                    cmd.Parameters.AddWithValue("@Idp", u.IdPersoana);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(_bileteTable);
                    }
                }


            }

            DataGridBilete.ItemsSource = _bileteTable.DefaultView;

        }
    }
}
