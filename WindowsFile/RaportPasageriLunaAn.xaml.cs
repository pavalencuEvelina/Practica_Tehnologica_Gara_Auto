using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using FastReport;
using FastReport.Export.PdfSimple;
using Microsoft.Data.SqlClient;
using Practica_Gara_Auto.Connection;
using Xceed.Document.NET;
using Xceed.Words.NET;


namespace Practica_Gara_Auto.WindowsFile
{
    public partial class RaportPasageriLunaAn : UserControl
    {
        public RaportPasageriLunaAn()
        {
            InitializeComponent();
            DateSelector.SelectedDate = DateTime.Today;
        }

         // Sus, la using-uri

// ... (restul using-urilor)
 // asigură-te că ai acest using




    private void BtnGenerare_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string pdfPath = Path.Combine(Path.GetTempPath(), "RaportPasageri.pdf");

                // Step 1: Get Data from SQL
                var dataTable = GetPasageriData(DateSelector.SelectedDate);
                MessageBox.Show($"Loaded rows: {dataTable.Rows.Count}");
                // Step 2: Load Report and Register Data
                using (Report report = new Report())
                {
                    report.Load("C:\\Users\\user\\source\\repos\\Practica_Gara_Auto\\Rapoarte\\RaportAnLuna.frx");

                    report.RegisterData(dataTable, "Table");
                    // Setează parametrii raportului
                    report.SetParameterValue("Luna", DateSelector.SelectedDate?.ToString("MMMM"));
                    report.SetParameterValue("An", DateSelector.SelectedDate?.Year);
                    // Must match table name in .frx
                    var dataBand = report.FindObject("Data1") as DataBand;
                    if (dataBand != null)
                    {
                        dataBand.DataSource = report.GetDataSource("Table");
                    }
                    report.GetDataSource("Table").Enabled = true;

                    // Step 3: Prepare Report
                    report.Prepare();

                    // Step 4: Export PDF
                    PDFSimpleExport pdfExport = new PDFSimpleExport();
                    report.Export(pdfExport, pdfPath);
                }

                // Step 5: Show in WebView2
                webView.Source = new Uri(pdfPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la generarea raportului: {ex.Message}");
            }
        }

        private DataTable GetPasageriData(DateTime? date)
        {
            DataTable table = new DataTable();

            using (DataBaseConnection conn = new DataBaseConnection())
            {
                string query = @"
            SELECT DISTINCT IDPasager, Nume, Prenume FROM Pasageri_DataRezervare
            WHERE MONTH(DataRezervare) = @Luna AND YEAR(DataRezervare) = @An";

                using (SqlCommand cmd = new SqlCommand(query, conn.Connection))
                {
                    cmd.Parameters.AddWithValue("@Luna", date?.Month ?? 1);
                    cmd.Parameters.AddWithValue("@An", date?.Year ?? 2024);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(table);
                    }
                }

                // Poți închide conexiunea explicit, dar e deja închisă prin using și Dispose()
                conn.Close();  // opțional
            }

            return table;
        }

    }
}
