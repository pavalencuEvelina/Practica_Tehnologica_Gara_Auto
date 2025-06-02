using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using FastReport;
using FastReport.Export.PdfSimple;
using Practica_Gara_Auto.Connection; // Clasa de conexiune
using Microsoft.Data.SqlClient;
using Microsoft.Win32;
using ClosedXML.Excel; // Trebuie să fie Microsoft.Data, nu System.Data

namespace Practica_Gara_Auto.WindowsFile
{
    public partial class CeleMaiSolicitateCurse : UserControl
    {
        public CeleMaiSolicitateCurse()
        {
            InitializeComponent();
            Loaded += CeleMaiSolicitateCurse_Loaded; // Generează raportul automat la încărcare
        }
        DataTable table;

        private void CeleMaiSolicitateCurse_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                string pdfPath = Path.Combine(Path.GetTempPath(), "RaportCeleMaiSolicitateCurse.pdf");

                var dataTable = GetCeleMaiSolicitateCurse();
                MessageBox.Show($"Date încărcate: {dataTable.Rows.Count} rânduri");

                using (Report report = new Report())
                {
                    report.Load("C:\\Users\\user\\source\\repos\\Practica_Gara_Auto\\Rapoarte\\CeleMaiSolicitateCurse.frx"); // Schimbă cu calea corectă la tine
                    report.RegisterData(dataTable, "CeleMaiSolicitateCurse");

                    var dataBand = report.FindObject("Data1") as FastReport.DataBand;
                    if (dataBand != null)
                    {
                        dataBand.DataSource = report.GetDataSource("CeleMaiSolicitateCurse");
                    }
                    report.GetDataSource("CeleMaiSolicitateCurse").Enabled = true;

                    report.Prepare();

                    PDFSimpleExport pdfExport = new PDFSimpleExport();
                    report.Export(pdfExport, pdfPath);
                }

                webView.Source = new Uri(pdfPath); // Asta merge în WebView2
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la generarea raportului: {ex.Message}");
            }
        }

        private DataTable GetCeleMaiSolicitateCurse()
        {
             table = new DataTable();

            using (var dbConn = new DataBaseConnection())
            {
                string query = @"
                SELECT TOP 10 
                    t.IDTraseu, 
                    tr.Localitate_Pornire AS PunctPornire, 
                    tr.Localitate_Destinatie AS Destinatie, 
                    COUNT(b.IDBilet) AS NrRezervari,
                    CONVERT(varchar(5), c.OraPlecare, 108) AS Ora
                FROM Bilet b
                JOIN Cursa c ON b.IDCursa = c.IDCursa
                JOIN Traseu t ON c.IDTraseu = t.IDTraseu
                JOIN Trasee tr ON tr.IDTraseu = t.IDTraseu
                GROUP BY t.IDTraseu, tr.Localitate_Pornire, tr.Localitate_Destinatie, c.OraPlecare
                ORDER BY NrRezervari DESC";


                using (var cmd = new SqlCommand(query, dbConn.Connection))
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(table);
                }
            }

            return table;
        }

        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            if (table == null ||table.Rows.Count == 0)
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
                        worksheet.Cell(1, 1).InsertTable(table);
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


    }
}
