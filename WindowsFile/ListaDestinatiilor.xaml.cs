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
    public partial class ListaDestinatiilor : UserControl
    {
        public ListaDestinatiilor()
        {
            InitializeComponent();
            Loaded += ListaDestinatii_Loaded;
        }

        private void ListaDestinatii_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            string pdfPath = Path.Combine(Path.GetTempPath(), $"RaportListaDestinatii_{Guid.NewGuid()}.pdf");

            DataTable table = GetDestinatiiDisponibile();

            using (Report report = new Report())
            {
                report.Load("C:\\Users\\user\\source\\repos\\Practica_Gara_Auto\\Rapoarte\\ListaDestinatiilor.frx");
                report.RegisterData(table, "ListaDestinatii");
                var dataBand = report.FindObject("Data1") as FastReport.DataBand;
                if (dataBand != null)
                    dataBand.DataSource = report.GetDataSource("ListaDestinatii");
                report.GetDataSource("ListaDestinatii").Enabled = true;
                report.Prepare();
                report.Export(new PDFSimpleExport(), pdfPath);
            }

            webView.Source = new Uri(pdfPath);
        }

        private DataTable GetDestinatiiDisponibile()
        {
            DataTable table = new DataTable();
            using (var dbConn = new DataBaseConnection())
            {
                string query = @"
                    SELECT DISTINCT l.Denumire AS Destinatie
                    FROM Statie s
                    JOIN Localitate l ON s.IDLocalitate = l.IDLocalitate";

                using var cmd = new SqlCommand(query, dbConn.Connection);
                using var adapter = new SqlDataAdapter(cmd);
                adapter.Fill(table);
            }
            return table;
        }
    }
}
