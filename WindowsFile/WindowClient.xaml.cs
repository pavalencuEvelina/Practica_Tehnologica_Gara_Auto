using Practica_Gara_Auto.Clase;
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
    public partial class WindowClient : Window
    {
        UtilizatorModel u;
        public WindowClient(UtilizatorModel u)
        {
            this.u = u;
            InitializeComponent();                        
            Salut.Text = "Salut, " + u.Nume + " " + u.Prenume+"!"; 

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


        private void BileteLunare_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new AdaugaBilet(u);
        }

        private void CautaDestinatie_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Afișăm cursele către destinația aleasă...");
        }

        private void ToateDestinatiile_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Afișăm toate destinațiile disponibile...");
        }

        private void TipTransport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Afișăm tipul de transport și capacitatea...");
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

        private void VeziHarta_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new TraseuAPI();
        }

        private void VeziCurse_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new Curse(u);
        }

        private void LocuriRezervate_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new LocuriRezervate(u);
        }

        private void CurseLocalitate_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ListaCurseLocalitateY();
        }

        private void CurseDestinatie_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ListaCurseDestinatieZ();
        }

        private void VeziBilete_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new VeziBiletele(u);
        }
    }
}
