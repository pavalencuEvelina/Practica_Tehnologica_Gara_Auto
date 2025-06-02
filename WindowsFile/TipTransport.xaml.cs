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
    /// Interaction logic for TipTransport.xaml
    /// </summary>
    public partial class TipTransport : UserControl
    {
        
        public TipTransport()
        {
            InitializeComponent();
            LoadTrasee();
        }

        private void LoadTrasee()
        {
            string query = "SELECT * FROM dbo.TipTransport";


            var db = new DataBaseConnection();
            DataTable _traseeTable = new DataTable();


            using (SqlDataAdapter adapter = new SqlDataAdapter(query, db.Connection))
            {
                adapter.Fill(_traseeTable);
            }
            DataGridTip.ItemsSource = _traseeTable.DefaultView;
        }
        }
}
