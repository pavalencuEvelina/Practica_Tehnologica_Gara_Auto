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
using ClosedXML.Excel;
using Microsoft.Win32;
using Xceed.Words.NET;
using System.Net.NetworkInformation;
using Practica_Gara_Auto.Clase;


namespace Practica_Gara_Auto.WindowsFile
{
    /// <summary>
    /// Interaction logic for Curse.xaml
    /// </summary>
    public partial class Curse : UserControl
    {
        private DataTable _curseTable = null;
        private readonly UtilizatorModel _utilizator;

        public Curse(UtilizatorModel u)
        {
            _utilizator = u;
            InitializeComponent();
            LoadTrasee();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Dacă rolul curent este "Pasager", ascunde câmpurile și butoanele aferente:
            if (_utilizator.Rol == "Pasager")
            {
                textSofer.Visibility = Visibility.Collapsed;
                BorderText.Visibility = Visibility.Collapsed;
                SearchSofer.Visibility = Visibility.Collapsed;
                ExportToExcelButton.Visibility = Visibility.Collapsed;
                ExportToWordButton.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadTrasee()
        {
            // 1) Încărcăm datele din view-ul Cursa_Locuri_Stat
            string query = "SELECT * FROM dbo.Cursa_Locuri_Stat";
            var db = new DataBaseConnection();
            _curseTable = new DataTable();

            using (SqlDataAdapter adapter = new SqlDataAdapter(query, db.Connection))
            {
                adapter.Fill(_curseTable);
            }

            DataGridTrasee.ItemsSource = _curseTable.DefaultView;

            // 2) Populăm ComboBox-ul pentru "Traseu"
            SearchTraseu.Items.Clear();
            SearchTraseu.Items.Add("No Filter");
            using (var cmd = new SqlCommand("SELECT DISTINCT Traseu FROM Cursa_Locuri_Stat", db.Connection))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    SearchTraseu.Items.Add(reader.GetString(0));
                }
            }
            SearchTraseu.SelectedIndex = 0;

            // 3) Populăm ComboBox-ul pentru "Șofer"
            SearchSofer.Items.Clear();
            SearchSofer.Items.Add("No Filter");
            using (var cmd = new SqlCommand("SELECT DISTINCT Sofer FROM Cursa_Locuri_Stat", db.Connection))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    SearchSofer.Items.Add(reader.GetString(0));
                }
            }
            SearchSofer.SelectedIndex = 0;

            db.Close();
        }

        // Am schimbat EventArgs în SelectionChangedEventArgs
        private void SearchTextChanged(object sender, SelectionChangedEventArgs e)
        {
            string traseu = SearchTraseu.SelectedItem?.ToString().Replace("'", "''");
            string sofer = SearchSofer.SelectedItem?.ToString().Replace("'", "''");

            string filter = "";

            if (!string.IsNullOrWhiteSpace(traseu) && traseu != "No Filter")
            {
                filter += $"Traseu LIKE '{traseu}%'";
            }

            // Dacă user-ul e "Pasager", ignorăm filtrarea după sofer
            if (_utilizator.Rol != "Pasager" &&
                !string.IsNullOrWhiteSpace(sofer) && sofer != "No Filter")
            {
                if (!string.IsNullOrEmpty(filter))
                    filter += " AND ";
                filter += $"Sofer LIKE '{sofer}%'";
            }

            _curseTable.DefaultView.RowFilter = filter;
        }

       
        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            if (_curseTable == null || _curseTable.Rows.Count == 0)
            {
                MessageBox.Show("Nu există date de exportat!");
                return;
            }

            try
            {
                
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    Title = "Salvează fișierul Excel"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Curse");
                        worksheet.Cell(1, 1).InsertTable(_curseTable);
                        workbook.SaveAs(saveFileDialog.FileName);
                    }

                    MessageBox.Show("Export completat cu succes!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la export: {ex.Message}");
            }
        
    }

        private void ExportToWord_Click(object sender, RoutedEventArgs e)
        {
            if (_curseTable == null || _curseTable.Rows.Count == 0)
            {
                MessageBox.Show("Nu există date de exportat!");
                return;
            }

            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Word Document|*.docx",
                    Title = "Salvează fișierul Word"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    using (var doc = DocX.Create(saveFileDialog.FileName))
                    {
                        // Adaugă titlu
                        doc.InsertParagraph("Export Curse").FontSize(18).Bold().Alignment = Xceed.Document.NET.Alignment.center;


                        // Creează tabelul
                        var table = doc.AddTable(_curseTable.Rows.Count + 1, _curseTable.Columns.Count);

                        // Setează antetele
                        for (int col = 0; col < _curseTable.Columns.Count; col++)
                        {
                            table.Rows[0].Cells[col].Paragraphs[0].Append(_curseTable.Columns[col].ColumnName).Bold();
                        }

                        // Adaugă datele
                        for (int row = 0; row < _curseTable.Rows.Count; row++)
                        {
                            for (int col = 0; col < _curseTable.Columns.Count; col++)
                            {
                                table.Rows[row + 1].Cells[col].Paragraphs[0].Append(_curseTable.Rows[row][col].ToString());
                            }
                        }

                        // Adaugă tabelul în document
                        doc.InsertTable(table);

                        // Salvează
                        doc.Save();
                    }

                    MessageBox.Show("Export completat cu succes în Word!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la export: {ex.Message}");
            }
        }
    }
}
