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
    public partial class TransportCursaX : UserControl
    {
        public TransportCursaX()
        {
            InitializeComponent();
            Loaded += TransportCursaX_Loaded;
        }

        private void TransportCursaX_Loaded(object sender, System.Windows.RoutedEventArgs e)
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

            foreach (var file in Directory.GetFiles(Path.GetTempPath(), "RaportTransportCursaX_*.pdf"))
            {
                try { File.Delete(file); } catch { }
            }

            string pdfPath = Path.Combine(Path.GetTempPath(), $"RaportTransportCursaX_{Guid.NewGuid()}.pdf");
            DataTable table = GetTransportByCursa(int.Parse(ComboCurse.SelectedItem.ToString()));

            using (Report report = new Report())
            {
                report.Load("C:\\Users\\user\\source\\repos\\Practica_Gara_Auto\\Rapoarte\\TransportCursaX.frx");
                report.RegisterData(table, "TransportCursa");
                var dataBand = report.FindObject("Data1") as FastReport.DataBand;
                if (dataBand != null)
                    dataBand.DataSource = report.GetDataSource("TransportCursa");
                report.GetDataSource("TransportCursa").Enabled = true;
                report.Prepare();
                report.Export(new PDFSimpleExport(), pdfPath);
            }

            webView.Source = new Uri(pdfPath);
        }

        private DataTable GetTransportByCursa(int idCursa)
        {
            DataTable table = new DataTable();
            using (var dbConn = new DataBaseConnection())
            {
                string query = @"
                    SELECT tt.Denumire AS TipTransport, tt.Nr_Locuri AS Capacitate
                    FROM Cursa c
                    JOIN TipTransport tt ON c.IDTipTransport = tt.IDTipTransport
                    WHERE c.IDCursa = @IDCursa";

                using var cmd = new SqlCommand(query, dbConn.Connection);
                cmd.Parameters.AddWithValue("@IDCursa", idCursa);
                using var adapter = new SqlDataAdapter(cmd);
                adapter.Fill(table);
            }
            return table;
        }
    }
}
