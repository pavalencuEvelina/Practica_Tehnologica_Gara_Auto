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
    public partial class ListaCurseDestinatieZ : UserControl
    {
        public ListaCurseDestinatieZ()
        {
            InitializeComponent();
            Loaded += ListaCurseDestinatieZ_Loaded;
        }

        private void ListaCurseDestinatieZ_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDestinatii();
        }

        private void LoadDestinatii()
        {
            using var dbConn = new DataBaseConnection();
            using var cmd = new SqlCommand("SELECT DISTINCT l.Denumire FROM Localitate l JOIN Statie s ON l.IDLocalitate = s.IDLocalitate JOIN Traseu t ON t.DestinatieID = s.IDStatie", dbConn.Connection);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ComboDestinatii.Items.Add(reader["Denumire"].ToString());
            }
        }

        private void ComboDestinatii_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboDestinatii.SelectedItem == null) return;

            foreach (var file in Directory.GetFiles(Path.GetTempPath(), "RaportListaCurseDestinatie_*.pdf"))
            {
                try { File.Delete(file); } catch { }
            }

            string pdfPath = Path.Combine(Path.GetTempPath(), $"RaportListaCurseDestinatie_{Guid.NewGuid()}.pdf");
            DataTable table = GetCurseByDestinatie(ComboDestinatii.SelectedItem.ToString());

            using (Report report = new Report())
            {
                report.Load("C:\\Users\\user\\source\\repos\\Practica_Gara_Auto\\Rapoarte\\ListaCurseDestinatieZ.frx");
                report.RegisterData(table, "CurseDestinatie");
                report.GetDataSource("CurseDestinatie").Enabled = true;

                var dataBand = report.FindObject("Data1") as FastReport.DataBand;
                if (dataBand != null)
                    dataBand.DataSource = report.GetDataSource("CurseDestinatie");

                report.Prepare();
                report.Export(new PDFSimpleExport(), pdfPath);
            }

            webView.Source = new Uri(pdfPath);
        }

        private DataTable GetCurseByDestinatie(string destinatie)
        {
            DataTable table = new DataTable();
            using (var dbConn = new DataBaseConnection())
            {
                string query = @"
                    SELECT c.IDCursa, c.DataPlecare, c.OraPlecare,
                           tt.Denumire AS TipTransport,
                           lp.Denumire AS Localitate_Pornire,
                           ld.Denumire AS Localitate_Destinatie
                    FROM Cursa c
                    JOIN Traseu t ON c.IDTraseu = t.IDTraseu
                    JOIN Statie sp ON t.PunctPornireID = sp.IDStatie
                    JOIN Localitate lp ON sp.IDLocalitate = lp.IDLocalitate
                    JOIN Statie sd ON t.DestinatieID = sd.IDStatie
                    JOIN Localitate ld ON sd.IDLocalitate = ld.IDLocalitate
                    JOIN TipTransport tt ON c.IDTipTransport = tt.IDTipTransport
                    WHERE ld.Denumire = @Destinatie";

                using var cmd = new SqlCommand(query, dbConn.Connection);
                cmd.Parameters.AddWithValue("@Destinatie", destinatie);
                using var adapter = new SqlDataAdapter(cmd);
                adapter.Fill(table);
            }
            return table;
        }
    }
}
