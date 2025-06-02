using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practica_Gara_Auto.Connection
{
    class DataBaseConnection : IDisposable
    {
        private readonly string connectionString = @"Data Source=WIN-1JGE1PG7IQE;Initial Catalog=Gara_Auto;Integrated Security=True;TrustServerCertificate=True;";
        public SqlConnection Connection { get; private set; }

        public DataBaseConnection()
        {
            Connection = new SqlConnection(connectionString);

            try
            {
                Connection.Open();
            }
            catch (SqlException ex)
            {
                
                System.Windows.MessageBox.Show($"Eroare la conectarea bazei de date:\n{ex.Message}", "Conexiune eșuată", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        public void Dispose()
        {
            if (Connection != null && Connection.State == ConnectionState.Open)
                Connection.Close();
        }

        public void Close()
        {
            if (Connection != null && Connection.State == System.Data.ConnectionState.Open)
                Connection.Close();
        }
    }
}
