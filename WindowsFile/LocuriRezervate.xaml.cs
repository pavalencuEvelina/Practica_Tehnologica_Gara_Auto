using System;
using System.Data;
using System.IO;
using System.Windows.Controls;
using FastReport;
using FastReport.Export.PdfSimple;
using Practica_Gara_Auto.Connection;
using Microsoft.Data.SqlClient;
using Practica_Gara_Auto.Clase;

namespace Practica_Gara_Auto.WindowsFile
{
    public partial class LocuriRezervate : UserControl
    {
        UtilizatorModel u;
        public LocuriRezervate(UtilizatorModel u)
        {
            InitializeComponent();
            Loaded += LocuriRezervate_Loaded;
            this.u = u;
        }

        private void LocuriRezervate_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            LoadCurse();
        }

        private void LoadCurse()
        {
            using var dbConn = new DataBaseConnection();
            using var cmd = new SqlCommand("SELECT DISTINCT IDCursa FROM Bilet WHERE IDPasager = @IDPasager", dbConn.Connection);
            cmd.Parameters.AddWithValue("@IDPasager", u.IdPersoana); // presupunem că LoginSession.CurrentUserId conține ID-ul clientului logat

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ComboCurse.Items.Add(reader["IDCursa"].ToString());
            }
        }

        private void BtnGenerare_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ComboCurse.SelectedItem == null)
            {
                System.Windows.MessageBox.Show("Selectează o cursă!");
                return;
            }

            int idCursa = int.Parse(ComboCurse.SelectedItem.ToString());

            foreach (var file in Directory.GetFiles(Path.GetTempPath(), "RaportLocuriRezervate_*.pdf"))
            {
                try { File.Delete(file); } catch { }
            }

            string pdfPath = Path.Combine(Path.GetTempPath(), $"RaportLocuriRezervate_{Guid.NewGuid()}.pdf");
            DataTable table = GetLocuriRezervateForClient(u.IdPersoana, idCursa);

            using (Report report = new Report())
            {
                report.Load("C:\\Users\\user\\source\\repos\\Practica_Gara_Auto\\Rapoarte\\LocuriRezervate.frx");
                report.RegisterData(table, "LocuriRezervate");
                report.GetDataSource("LocuriRezervate").Enabled = true;
                var dataBand = report.FindObject("Data1") as FastReport.DataBand;
                if (dataBand != null)
                    dataBand.DataSource = report.GetDataSource("LocuriRezervate");
                report.GetDataSource("LocuriRezervate").Enabled = true;
                report.Prepare();
                report.Export(new PDFSimpleExport(), pdfPath);
            }

            webView.Source = new Uri(pdfPath);
        }

        private DataTable GetLocuriRezervateForClient(int idPasager, int idCursa)
        {
            DataTable table = new DataTable();
            using (var dbConn = new DataBaseConnection())
            {
                string query = @"
                    SELECT b.IDCursa, b.Nr_loc, c.OraPlecare, c.DataPlecare,
                           tt.Denumire AS TipTransport,
                           lp.Denumire AS Localitate_Pornire,
                           ld.Denumire AS Localitate_Destinatie
                    FROM Bilet b
                    JOIN Cursa c ON b.IDCursa = c.IDCursa
                    JOIN TipTransport tt ON c.IDTipTransport = tt.IDTipTransport
                    JOIN Traseu t ON c.IDTraseu = t.IDTraseu
                    JOIN Statie sp ON t.PunctPornireID = sp.IDStatie
                    JOIN Localitate lp ON sp.IDLocalitate = lp.IDLocalitate
                    JOIN Statie sd ON t.DestinatieID = sd.IDStatie
                    JOIN Localitate ld ON sd.IDLocalitate = ld.IDLocalitate
                    WHERE b.IDPasager = @IDPasager AND b.IDCursa = @IDCursa";

                using var cmd = new SqlCommand(query, dbConn.Connection);
                cmd.Parameters.AddWithValue("@IDPasager", idPasager);
                cmd.Parameters.AddWithValue("@IDCursa", idCursa);
                using var adapter = new SqlDataAdapter(cmd);
                adapter.Fill(table);
            }
            return table;
        }
    }
}
