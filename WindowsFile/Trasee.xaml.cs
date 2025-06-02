using Practica_Gara_Auto.Connection;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
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
    /// Interaction logic for Trasee.xaml
    /// </summary>
    public partial class Trasee : UserControl
    {
        private DataTable _traseeTable = null;
        public Trasee()
        {
            InitializeComponent();
            LoadTrasee();
        }

        private void LoadTrasee()
        {
            string query = "SELECT * FROM dbo.Trasee";


            var db = new DataBaseConnection();
            _traseeTable = new DataTable();
           

            using (SqlDataAdapter adapter = new SqlDataAdapter(query, db.Connection))
            {
                adapter.Fill(_traseeTable);
            }

            DataGridTrasee.ItemsSource = _traseeTable.DefaultView;

            SearchLocalitatePornire.Items.Add("No Filter");
            SqlCommand cmdPornire = new SqlCommand("SELECT DISTINCT Localitate_Pornire FROM Trasee", db.Connection);
            SqlDataReader readerPornire = cmdPornire.ExecuteReader();
            while (readerPornire.Read())
            {
                SearchLocalitatePornire.Items.Add(readerPornire.GetString(0));
            }
            readerPornire.Close();

            SearchLocalitatePornire.SelectedIndex = 0;

            SearchLocalitateDestinatie.Items.Add("No Filter");
            SqlCommand cmdDestinatie = new SqlCommand("SELECT DISTINCT Localitate_Destinatie FROM Trasee", db.Connection);
            SqlDataReader readerDestinatie = cmdDestinatie.ExecuteReader();
            while (readerDestinatie.Read())
            {
                SearchLocalitateDestinatie.Items.Add(readerDestinatie.GetString(0));
            }
            readerDestinatie.Close();
            db.Close();

            SearchLocalitateDestinatie.SelectedIndex = 0;
            


        }

        private void SearchTextChanged(object sender, EventArgs e)
        {
            string destinatie = SearchLocalitateDestinatie.SelectedItem?.ToString().Replace("'", "''"); 
            string pornire= SearchLocalitatePornire.SelectedItem?.ToString().Replace("'", "''");


            string filter = "";

            if (!string.IsNullOrWhiteSpace(destinatie) && destinatie != "No Filter")
                filter += $"Localitate_Destinatie LIKE '{destinatie}%'";

            if (!string.IsNullOrWhiteSpace(pornire) && pornire!= "No Filter")
            {
                if (filter != "") filter += " AND ";
                filter += $"Localitate_Pornire LIKE '{pornire}%'";
            }

            _traseeTable.DefaultView.RowFilter = filter;

        }
    }
}
