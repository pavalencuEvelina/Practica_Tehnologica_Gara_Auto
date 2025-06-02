using Microsoft.Data.SqlClient;
using Practica_Gara_Auto.Clase;
using Practica_Gara_Auto.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Practica_Gara_Auto.WindowsFile
{
    /// <summary>
    /// Interaction logic for WindowAdmin.xaml
    /// </summary>
    public partial class WindowAdmin : Window
    {
        private UtilizatorModel utilizator;
        public WindowAdmin(UtilizatorModel u)
        {
            InitializeComponent();
            utilizator = u;

           
        }

        private bool meniuAscuns = false;
        private void ToggleMeniu_Click(object sender, RoutedEventArgs e)
        {
            if (!meniuAscuns)
            {
                var hide = (Storyboard)this.Resources["HideMeniu"];
                hide.Begin();
                BtnToggleMeniu.Content = "⇨";
                meniuAscuns = true;
            }
            else
            {
                var show = (Storyboard)this.Resources["ShowMeniu"];
                show.Begin();
                BtnToggleMeniu.Content = "⇦";
                meniuAscuns = false;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            const double LargeWidthThreshold = 1200;

            if (e.NewSize.Width >= LargeWidthThreshold)
            {
                // Meniu mai lat
                MeniuContainer.Width = 300;

                // Fonturi mărite
                SetMenuFontSize(16);
            }
            else
            {
                // Înapoi la valorile standard
                MeniuContainer.Width = 230;
                SetMenuFontSize(12);
            }
            double latime = this.ActualWidth;

            if (latime < 600 && !meniuAscuns)
            {
                var hide = (Storyboard)this.Resources["HideMeniu"];
                hide.Begin();
                BtnToggleMeniu.Content = "⇨";
                meniuAscuns = true;
            }
            else if (latime >= 600 && meniuAscuns)
            {
                var show = (Storyboard)this.Resources["ShowMeniu"];
                show.Begin();
                BtnToggleMeniu.Content = "⇦";
                meniuAscuns = false;
            }
        }

        private void SetMenuFontSize(double size)
        {
            // texturile de secțiune
            foreach (var tb in FindVisualChildren<TextBlock>(MeniuContainer))
                tb.FontSize = size;

            // butoanele
            foreach (var btn in FindVisualChildren<Button>(MeniuContainer))
                btn.FontSize = size;
        }

        // helper generic care parcurge recursiv VisualTree
        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) yield break;
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T t)
                    yield return t;

                foreach (var descendant in FindVisualChildren<T>(child))
                    yield return descendant;
            }
        }



        private void CautaAngajati_Click(object sender, RoutedEventArgs e)
        {
           
            MainContent.Content=new Angajati();
        }

        private void PasageriLunaAn_Click(object sender, RoutedEventArgs e)
        {
            
            MainContent.Content = new RaportPasageriLunaAn();
        }

        private void TipTransport_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new TipTransport();
        }

        private void Deconectare_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Te-ai deconectat.");
            MainWindow w = new MainWindow();
            w.Show();
            this.Close();
        }

        private void InchideAplicatia_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void AdaugareAngajat_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new UserControl1();
        }

        private void AdaugareContAngajat_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new AddUserAngajat();
        }

        private void ȘtergeUser_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new StergeUser();
        }

        private void ModificareAngajati_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new Modificare_angajat();
        }

        private void ModificareUser_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ModificareUser();
        }

        private void CurseSolicitate_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new CeleMaiSolicitateCurse();
        }

        private void BtnBackup_Click(object sender, RoutedEventArgs e)
        {
            

            try
            {
                
                string dbName = "Gara_Auto";
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string folder = @"C:\Backup\"; 
                string backupFile = System.IO.Path.Combine(
                    folder,
                    $"{dbName}_{timestamp}.bak");

               
                string sql = $@"
            BACKUP DATABASE [{dbName}]
            TO DISK = @BackupPath
            WITH INIT, COMPRESSION;
        ";

               
                using (var dbConn = new DataBaseConnection())
                using (var cmd = new SqlCommand(sql, dbConn.Connection))
                {
                    cmd.Parameters.AddWithValue("@BackupPath", backupFile);
                    cmd.CommandTimeout = 0; // fără timeout, dacă baza e mare
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show(
              $"Backup realizat cu succes!\nFișier: {backupFile}",
              "Backup reușit",
              MessageBoxButton.OK,
              MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
             $"Eroare la backup:\n{ex.Message}",
             "Eroare Backup",
             MessageBoxButton.OK,
             MessageBoxImage.Error);
            }
        }
    }
}
