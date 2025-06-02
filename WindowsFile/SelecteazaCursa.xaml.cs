using Microsoft.Data.SqlClient;
using Practica_Gara_Auto.Connection;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Practica_Gara_Auto.WindowsFile
{
    /// <summary>
    /// Interaction logic for SelecteazaCursa.xaml
    /// </summary>
    public partial class SelecteazaCursa : Window
    {
        public int? SelectedIDCursa { get; private set; }

        public SelecteazaCursa()
        {
            InitializeComponent();
            LoadCurseDinVedere();
        }

        // Clasă simplă corespunzătoare coloanelor din vederea Curse_Complet
        private class CursaInfo
        {
            public int IDCursa { get; set; }
            public string Traseu { get; set; }
            // Poți adăuga proprietăți suplimentare, dacă extinzi SELECT-ul (ex. DataPlecare, OraPlecare etc.)
        }

        private void LoadCurseDinVedere()
        {
            var lista = new List<CursaInfo>();

            try
            {
                using (var db = new DataBaseConnection())
                {

                    string sql = @"
                        SELECT IDCursa, Traseu
                        FROM Curse_Complet
                        ORDER BY IDCursa;
                    ";

                    using (var cmd = new SqlCommand(sql, db.Connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new CursaInfo
                            {
                                IDCursa = reader.GetInt32(0),
                                Traseu = reader.GetString(1)
                                
                            });
                        }
                    }
                }

                DataGridCurse.ItemsSource = lista;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Eroare la încărcarea listelor de curse: {ex.Message}",
                    "Eroare",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridCurse.SelectedItem is CursaInfo ci)
            {
                SelectedIDCursa = ci.IDCursa;
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show(
                    "Selectați o cursă din tabel înainte de a apăsa OK.",
                    "Atenție",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
