using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using FastReport;
using FastReport.Export.PdfSimple;
using Practica_Gara_Auto.Connection;
using Microsoft.Data.SqlClient;

namespace Practica_Gara_Auto.WindowsFile
{
    public partial class ListaCurseLocalitateY : UserControl
    {
        public ListaCurseLocalitateY()
        {
            InitializeComponent();
        }

        private void BtnCauta_Click(object sender, RoutedEventArgs e)
        {
            string localitate = TxtLocalitate.Text.Trim();
            if (string.IsNullOrWhiteSpace(localitate))
            {
                MessageBox.Show("Introdu o localitate!");
                return;
            }

            var curse = GetCurseByLocalitate(localitate);
            if (curse.Rows.Count == 0)
            {
                MessageBox.Show("Nu există curse care pornesc sau au destinația în această localitate.");
                return;
            }

            string pdfPath = Path.Combine(Path.GetTempPath(), $"RaportCurseLocalitate_{Guid.NewGuid()}.pdf");
            using (Report report = new Report())
            {
                report.Load("C:\\Users\\user\\source\\repos\\Practica_Gara_Auto\\Rapoarte\\CursePrinLocalitate.frx");
                report.RegisterData(curse, "CurseLocalitate");
                var dataBand = report.FindObject("Data1") as FastReport.DataBand;
                if (dataBand != null)
                    dataBand.DataSource = report.GetDataSource("CurseLocalitate");
               
                report.GetDataSource("CurseLocalitate").Enabled = true;
                report.Prepare();
                report.Export(new PDFSimpleExport(), pdfPath);
            }

            webView.Source = new Uri(pdfPath);
        }




        private DataTable GetCurseByLocalitate(string localitate)
        {
            var table = new DataTable();

            using (var dbConn = new DataBaseConnection())
            {
                string query = @"
            SELECT 
                c.IDCursa,
                c.DataPlecare,
                c.OraPlecare,
                tt.Denumire        AS TipTransport,
                t.Localitate_Pornire,
                t.Localitate_Destinatie,
                t.km
            FROM Cursa      c
            JOIN TipTransport tt  ON c.IDTipTransport = tt.IDTipTransport
            JOIN Trasee     t   ON c.IDTraseu       = t.IDTraseu
            WHERE 
                LOWER(t.Localitate_Pornire)   = LOWER(@loc)
                OR LOWER(t.Localitate_Destinatie) = LOWER(@loc);
        ";

                using var cmd = new SqlCommand(query, dbConn.Connection);
                cmd.Parameters.AddWithValue("@loc", localitate.Trim());

                using var reader = cmd.ExecuteReader();
                table.Load(reader);
            }

            return table;
        }

    }
}
