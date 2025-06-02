using Practica_Gara_Auto.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Data.SqlClient;
using Practica_Gara_Auto.Clase;
using Microsoft.Web.WebView2.Core;

namespace Practica_Gara_Auto.WindowsFile
{
    /// <summary>
    /// Interaction logic for TraseuAPI.xaml
    /// </summary>
    public partial class TraseuAPI : UserControl
    {
        private readonly DataBaseConnection _dbConn;
        public TraseuAPI()
        {
            InitializeComponent();
            _dbConn = new DataBaseConnection();

            // la încărcare inițializezi WebView2 și populezi ComboBox-ul
            Loaded += async (_, __) =>
            {
                    // inițializează WebView2
                    await webView.EnsureCoreWebView2Async();

                    // legăm evenimentele de încărcare
                    webView.CoreWebView2.NavigationStarting += CoreWebView2_NavigationStarting;
                    webView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
                    webView.Source = new Uri("C:\\Users\\user\\source\\repos\\Practica_Gara_Auto\\WebMap\\map.html");
                LoadTrasee();
            };
        }

        private void LoadTrasee()
        {
            var lista = new List<TraseuItem>();

            using var cmd = new SqlCommand(@"
        SELECT IDTraseu,
               Localitate_Pornire,
               Localitate_Destinatie
        FROM TraseeTotal
        ORDER BY IDTraseu",
              _dbConn.Connection);

            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                lista.Add(new TraseuItem
                {
                    IDTraseu = rdr.GetInt32(0),
                    LocalitatePornire = rdr.GetString(1),
                    LocalitateDestinatie = rdr.GetString(2)
                });
            }

            cmbTrasee.ItemsSource = lista;


        }

        private void CoreWebView2_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            // arată bara când începe navigarea
            pbLoading.Visibility = Visibility.Visible;
        }

        private void CoreWebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            // ascunde bara când s-a terminat încărcarea
            pbLoading.Visibility = Visibility.Collapsed;

            // acum putem popula combo-box-ul
            LoadTrasee();
        }

        private async void cmbTrasee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbTrasee.SelectedItem is not TraseuItem sel) return;
            int traseuId = sel.IDTraseu;

            // 1) coordonate
            var coords = new List<object>();
            using (var cmd = new SqlCommand(
                @"SELECT StartLat, StartLon, EndLat, EndLon 
      FROM TraseeTotal
      WHERE IDTraseu = @id",
                _dbConn.Connection))
            {
                cmd.Parameters.AddWithValue("@id", traseuId);
                using var rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    var startLat = (double)rdr.GetDecimal(0);
                    var startLon = (double)rdr.GetDecimal(1);
                    var endLat = (double)rdr.GetDecimal(2);
                    var endLon = (double)rdr.GetDecimal(3);

                    coords.Add(new { lat = startLat, lon = startLon });
                    coords.Add(new { lat = endLat, lon = endLon });
                }
            }
            var coordsJson = JsonSerializer.Serialize(coords);
            await webView.CoreWebView2.ExecuteScriptAsync(
                $"window.drawRoute({coordsJson});");
           
           
        }

       
    }
}