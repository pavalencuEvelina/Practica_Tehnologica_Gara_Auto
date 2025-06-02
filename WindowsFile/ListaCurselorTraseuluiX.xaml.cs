using System;
using System.Data;
using System.IO;
using System.Windows.Controls;
using FastReport;
using FastReport.Export.PdfSimple;
using Practica_Gara_Auto.Connection;
using Microsoft.Data.SqlClient;

namespace Practica_Gara_Auto.WindowsFile
{
    public partial class ListaCurselorTraseuluiX : UserControl
    {
        public ListaCurselorTraseuluiX()
        {
            InitializeComponent();
            Loaded += ListaCurse_Loaded;
        }

        private void ListaCurse_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            LoadTrasee();
        }

        private void LoadTrasee()
        {
            using var dbConn = new DataBaseConnection();
            using var cmd = new SqlCommand("SELECT IDTraseu FROM Traseu", dbConn.Connection);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ComboTrasee.Items.Add(reader["IDTraseu"].ToString());
            }
        }

        private void ComboTrasee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboTrasee.SelectedItem == null) return;

            // Șterge PDF-uri vechi
            foreach (var file in Directory.GetFiles(Path.GetTempPath(), "RaportListaCurse_*.pdf"))
            {
                try { File.Delete(file); } catch { }
            }

            string pdfPath = Path.Combine(Path.GetTempPath(), $"RaportListaCurse_{Guid.NewGuid()}.pdf");
            DataTable table = GetCurseByTraseu(int.Parse(ComboTrasee.SelectedItem.ToString()));

            using (Report report = new Report())
            {
                report.Load("C:\\Users\\user\\source\\repos\\Practica_Gara_Auto\\Rapoarte\\ListaCurselorTraseuluiX.frx");
                report.RegisterData(table, "ListaCurse");
                var dataBand = report.FindObject("Data1") as FastReport.DataBand;
                if (dataBand != null)
                    dataBand.DataSource = report.GetDataSource("ListaCurse");
                report.GetDataSource("ListaCurse").Enabled = true;
                report.Prepare();
                report.Export(new PDFSimpleExport(), pdfPath);
            }

            webView.Source = new Uri(pdfPath);
        }

        private DataTable GetCurseByTraseu(int idTraseu)
        {
            DataTable table = new DataTable();
            using (var dbConn = new DataBaseConnection())
            {
                string query = @"
                    SELECT c.IDCursa, c.OraPlecare, 
                           CONVERT(varchar(10), c.DataPlecare, 104) AS DataPlecare,
                           lp.Denumire AS Localitate_Pornire,
                           ld.Denumire AS Localitate_Destinatie
                    FROM Cursa c
                    JOIN Traseu t ON c.IDTraseu = t.IDTraseu
                    JOIN Statie sp ON t.PunctPornireID = sp.IDStatie
                    JOIN Localitate lp ON sp.IDLocalitate = lp.IDLocalitate
                    JOIN Statie sd ON t.DestinatieID = sd.IDStatie
                    JOIN Localitate ld ON sd.IDLocalitate = ld.IDLocalitate
                    WHERE c.IDTraseu = @IDTraseu";

                using var cmd = new SqlCommand(query, dbConn.Connection);
                cmd.Parameters.AddWithValue("@IDTraseu", idTraseu);
                using var adapter = new SqlDataAdapter(cmd);
                adapter.Fill(table);
            }
            return table;
        }
    }
}
