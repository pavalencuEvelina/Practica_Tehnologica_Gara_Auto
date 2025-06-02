using System;
using System.Data;
using System.IO;
using System.Windows.Controls;
using FastReport;
using FastReport.Export.PdfSimple;
using Practica_Gara_Auto.Connection;
using Microsoft.Data.SqlClient;
using System.Windows;

namespace Practica_Gara_Auto.WindowsFile
{
    public partial class ListaPasageriCursaXDataD : UserControl
    {
        public ListaPasageriCursaXDataD()
        {
            InitializeComponent();
            Loaded += ListaPasageriCursaXDataD_Loaded;
        }

        private void ListaPasageriCursaXDataD_Loaded(object sender, System.Windows.RoutedEventArgs e)
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

        private void BtnGenerare_Click(object sender, RoutedEventArgs e)
        {
            if (ComboCurse.SelectedItem == null || DateSelector.SelectedDate == null)
            {
                MessageBox.Show("Selectează o cursă și o dată!");
                return;
            }

            int idCursa = int.Parse(ComboCurse.SelectedItem.ToString());
            DateTime dataSelectata = DateSelector.SelectedDate.Value.Date;

            foreach (var file in Directory.GetFiles(Path.GetTempPath(), "RaportPasageriCursaXDataD_*.pdf"))
            {
                try { File.Delete(file); } catch { }
            }

            string pdfPath = Path.Combine(Path.GetTempPath(), $"RaportPasageriCursaXDataD_{Guid.NewGuid()}.pdf");
            DataTable table = GetPasageriByCursa(idCursa, dataSelectata);

            using (Report report = new Report())
            {
                report.Load("C:\\Users\\user\\source\\repos\\Practica_Gara_Auto\\Rapoarte\\ListaPasageriCursaXDataD.frx");
                report.RegisterData(table, "PasageriCursa");
                var dataBand = report.FindObject("Data1") as FastReport.DataBand;
                if (dataBand != null)
                    dataBand.DataSource = report.GetDataSource("PasageriCursa");
                report.GetDataSource("PasageriCursa").Enabled = true;
                report.Prepare();
                report.Export(new PDFSimpleExport(), pdfPath);
            }

            webView.Source = new Uri(pdfPath);
        }


        private DataTable GetPasageriByCursa(int idCursa, DateTime data)
        {
            DataTable table = new DataTable();
            using (var dbConn = new DataBaseConnection())
            {
                string query = @"
            SELECT p.IDPasager, p.Nume, p.Prenume, b.Nr_loc, c.DataPlecare
            FROM Bilet b
            JOIN Pasager p ON b.IDPasager = p.IDPasager
            JOIN Cursa c ON b.IDCursa = c.IDCursa
            WHERE b.IDCursa = @IDCursa AND CAST(c.DataPlecare AS DATE) = @Data";

                using var cmd = new SqlCommand(query, dbConn.Connection);
                cmd.Parameters.AddWithValue("@IDCursa", idCursa);
                cmd.Parameters.AddWithValue("@Data", data);
                using var adapter = new SqlDataAdapter(cmd);
                adapter.Fill(table);
            }
            return table;
        }

    }
}
