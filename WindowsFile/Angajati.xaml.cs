using Microsoft.Data.SqlClient;
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

namespace Practica_Gara_Auto.WindowsFile
{
    /// <summary>
    /// Interaction logic for Angajati.xaml
    /// </summary>
    public partial class Angajati : UserControl
    {
        private DataTable _angajatiTable=null;
        public Angajati()
        {
            InitializeComponent();
            LoadAngajati();
           
        }

        private void LoadAngajati()
        {
            string query = "SELECT * FROM Angajat_Complet";
           

            var db = new DataBaseConnection();
            _angajatiTable = new DataTable();

            using (SqlDataAdapter adapter = new SqlDataAdapter(query, db.Connection))
            {
                adapter.Fill(_angajatiTable);
            }

            DataGridAngajati.ItemsSource = _angajatiTable.DefaultView;

            SearchPost.Items.Add("No Filter");
            SqlCommand cmdPost = new SqlCommand("SELECT Denumire FROM Post", db.Connection);
            SqlDataReader readerPost = cmdPost.ExecuteReader();
            while (readerPost.Read())
            {
                SearchPost.Items.Add(readerPost.GetString(0));
            }
            readerPost.Close();
            db.Close();

            SearchPost.SelectedIndex = 0;

            
        }

        private void SearchTextChanged(object sender, EventArgs e)
        {
            string nume = SearchNume.Text.Replace("'", "''");
            string prenume = SearchPrenume.Text.Replace("'", "''");
            string post = SearchPost.SelectedItem?.ToString().Replace("'", "''");

            string filter = "";

            if (!string.IsNullOrWhiteSpace(nume))
                filter += $"Nume LIKE '{nume}%'";

            if (!string.IsNullOrWhiteSpace(prenume))
            {
                if (filter != "") filter += " AND ";
                filter += $"Prenume LIKE '{prenume}%'";
            }

            if (!string.IsNullOrWhiteSpace(post) && post != "No Filter")
            {
                if (filter != "") filter += " AND ";
                filter += $"Post = '{post}'";
            }

            _angajatiTable.DefaultView.RowFilter = filter; 

        }
    }
}


