using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using FastReport;
using FastReport.Export.PdfSimple;
using Practica_Gara_Auto.Connection;
using Microsoft.Data.SqlClient;

namespace Practica_Gara_Auto.WindowsFile
{
    public partial class ListaPasageriCursa : UserControl
    {
        public ListaPasageriCursa()
        {
            InitializeComponent();
            Loaded += ListaPasageriCursa_Loaded;
        }

        private void ListaPasageriCursa_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCurse();
        }

        private void LoadCurse()
        {
            using var dbConn = new DataBaseConnection();
            using var cmd = new SqlCommand("SELECT IDCursa FROM Cursa", dbConn.Connection);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ComboCurse.Items.Add(reader["IDCursa"].ToString());
            }
        }

        private void ComboCurse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboCurse.SelectedItem == null) return;

            // Curățare PDF-uri vechi generate de raport
            foreach (var file in Directory.GetFiles(Path.GetTempPath(), "RaportListaPasageri_*.pdf"))
            {
                try { File.Delete(file); } catch { }
            }

            // Nume unic pentru PDF
            string pdfPath = Path.Combine(Path.GetTempPath(), $"RaportListaPasageri_{Guid.NewGuid()}.pdf");
            DataTable table = GetPasageriByCursa(int.Parse(ComboCurse.SelectedItem.ToString()));

            using (Report report = new Report())
            {
                report.Load("C:\\Users\\user\\source\\repos\\Practica_Gara_Auto\\Rapoarte\\ListaPasageriCursa.frx");
                report.RegisterData(table, "ListaPasageri");
                var dataBand = report.FindObject("Data1") as FastReport.DataBand;
                if (dataBand != null)
                    dataBand.DataSource = report.GetDataSource("ListaPasageri");
                report.GetDataSource("ListaPasageri").Enabled = true;
                report.Prepare();
                report.Export(new PDFSimpleExport(), pdfPath);
            }

            webView.Source = new Uri(pdfPath);
        }


        private DataTable GetPasageriByCursa(int idCursa)
        {
            DataTable table = new DataTable();
            using (var dbConn = new DataBaseConnection())
            {
                string query = @"
                    SELECT p.IDPasager, p.Nume, p.Prenume, b.Nr_loc
                    FROM Bilet b
                    JOIN Pasager p ON b.IDPasager = p.IDPasager
                    WHERE b.IDCursa = @IDCursa";

                using var cmd = new SqlCommand(query, dbConn.Connection);
                cmd.Parameters.AddWithValue("@IDCursa", idCursa);
                using var adapter = new SqlDataAdapter(cmd);
                adapter.Fill(table);
            }
            return table;
        }
    }
}
