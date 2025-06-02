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
    /// <summary>
    /// Interaction logic for WindowAngajat.xaml
    /// </summary>
    public partial class WindowAngajat : Window
    {
        private UtilizatorModel u;
        public WindowAngajat(UtilizatorModel u)
        {
            this.u = u;
            InitializeComponent();
            Salut.Text = "Salut, " + u.Nume + " " + u.Prenume + "!";
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

            if (latime < 1000 && !meniuAscuns)
            {
                var hide = (Storyboard)this.Resources["HideMeniu"];
                hide.Begin();
                BtnToggleMeniu.Content = "⇨";
                meniuAscuns = true;
            }
            else if (latime >= 1000 && meniuAscuns)
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



        private void AdaugareCursa_Click(object sender, RoutedEventArgs e)

        {
            if (u.Post == "Dispecer")
            {
                MainContent.Content = new AdaugareCursa("adaugare");
            }
            else
            {
                MessageBox.Show("Doar Dispecerul are această opțiune disponibilă!.","Eroare" ,MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

  
        private void VeziTrasee_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new Trasee();
        }

        private void VeziCurse_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new Curse(u);
        }

        private void VeziBilete_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new VeziBiletele(u);
        }

        private void ModificareCursa_Click(object sender, RoutedEventArgs e)
        {
            if (u.Post == "Dispecer")
            {
                var dlg = new SelecteazaCursa
            {
                Owner = this
            };
            bool? rezultat = dlg.ShowDialog();

            if (rezultat == true && dlg.SelectedIDCursa.HasValue)
            {
                int idCursaSelectata = dlg.SelectedIDCursa.Value;

                // 2) Cream UserControl-ul în modul “modificare”
                var uc = new AdaugareCursa("modificare", idCursaSelectata);

                // 3) Îl punem în ContentControl
                MainContent.Content = uc;
            }
            else
            {
                // S-a anulat dialogul sau nicio cursă nu a fost selectată
            }
            }
            else
            {
                MessageBox.Show("Doar Dispecerul are această opțiune disponibilă!.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }



        private void StergeCursa_Click(object sender, RoutedEventArgs e)
        {
            if (u.Post == "Dispecer")
            {
                MainContent.Content = new StergereCurse();
             }
            else
            {
                MessageBox.Show("Doar Dispecerul are această opțiune disponibilă!.","Eroare" ,MessageBoxButton.OK, MessageBoxImage.Warning);
            }
}

        private void PasageriCursa_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new Practica_Gara_Auto.WindowsFile.ListaPasageriCursa();
        }

        private void ListaDestinatiia_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new Practica_Gara_Auto.WindowsFile.ListaDestinatiilor();
        }

        private void ListaCurse_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new Practica_Gara_Auto.WindowsFile.ListaCurselorTraseuluiX();
        }

        private void TipTransport_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new Practica_Gara_Auto.WindowsFile.TransportCursaX();
        }
        private void ListaPasageriCursaXDataD_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new Practica_Gara_Auto.WindowsFile.ListaPasageriCursaXDataD();
        }

        private void VeziHarta_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new TraseuAPI();
        }

        private void TipTransportAfisare_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new TipTransport();
        }
    }
}

