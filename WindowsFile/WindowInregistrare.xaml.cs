using Microsoft.Data.SqlClient;
using Practica_Gara_Auto.Connection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Practica_Gara_Auto.WindowsFile
{
    /// <summary>
    /// Interaction logic for WindowInregistrare.xaml
    /// </summary>
    public partial class WindowInregistrare : Window
    {
        public WindowInregistrare()
        {
            InitializeComponent();
        }

        private bool parolaVizibila = false;

        private void btnToggleParola_Click(object sender, RoutedEventArgs e)
        {
            parolaVizibila = !parolaVizibila;

            if (parolaVizibila)
            {
                txtParolaVizibila.Text = txtParola.Password;
                txtParola.Visibility = Visibility.Collapsed;
                txtParolaVizibila.Visibility = Visibility.Visible;
                imgOchi.Source = new BitmapImage(new Uri("/WindowsFile/show-password-3.png", UriKind.Relative));
            }
            else
            {
                txtParola.Password = txtParolaVizibila.Text;
                txtParola.Visibility = Visibility.Visible;
                txtParolaVizibila.Visibility = Visibility.Collapsed;
                imgOchi.Source = new BitmapImage(new Uri("/WindowsFile/hide-password.png", UriKind.Relative));
            }
        }

        private void txtParola_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!parolaVizibila)
                txtParolaVizibila.Text = txtParola.Password;
        }

        private void txtParolaVizibila_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (parolaVizibila)
                txtParola.Password = txtParolaVizibila.Text;
        }

        private bool confirmareParolaVizibila = false;

        private void btnToggleConfirmareParola_Click(object sender, RoutedEventArgs e)
        {
            confirmareParolaVizibila = !confirmareParolaVizibila;

            if (confirmareParolaVizibila)
            {
                txtConfirmareParolaVizibila.Text = txtConfirmareParola.Password;
                txtConfirmareParola.Visibility = Visibility.Collapsed;
                txtConfirmareParolaVizibila.Visibility = Visibility.Visible;
                imgOchiConfirmare.Source = new BitmapImage(new Uri("/WindowsFile/show-password-3.png", UriKind.Relative));
            }
            else
            {
                txtConfirmareParola.Password = txtConfirmareParolaVizibila.Text;
                txtConfirmareParola.Visibility = Visibility.Visible;
                txtConfirmareParolaVizibila.Visibility = Visibility.Collapsed;
                imgOchiConfirmare.Source = new BitmapImage(new Uri("/WindowsFile/hide-password.png", UriKind.Relative));
            }
        }

        private void txtConfirmareParola_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!confirmareParolaVizibila)
                txtConfirmareParolaVizibila.Text = txtConfirmareParola.Password;
        }

        private void txtConfirmareParolaVizibila_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (confirmareParolaVizibila)
                txtConfirmareParola.Password = txtConfirmareParolaVizibila.Text;
        }



        private void txtEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtEmailPlaceholder.Visibility = string.IsNullOrWhiteSpace(txtEmail.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void txtNume_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtNumePlaceholder.Visibility = string.IsNullOrWhiteSpace(txtNume.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void txtPrenume_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtPrenumePlaceholder.Visibility = string.IsNullOrWhiteSpace(txtPrenume.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void txtTelefon_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtTelefonPlaceholder.Visibility = string.IsNullOrWhiteSpace(txtTelefon.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void btnInregistrare_Click(object sender, RoutedEventArgs e)
        {
            string nume=txtNume.Text.Trim();
            string prenume = txtPrenume.Text;
            string email = txtEmail.Text.Trim();
            string nr_telefon = txtTelefon.Text.Trim();
            string parola = txtParolaVizibila.Text.Trim();
            string confirmare_parola = txtConfirmareParolaVizibila.Text.Trim();


            if (string.IsNullOrWhiteSpace(nume) || !nume.All(char.IsLetter))
            {
                MessageBox.Show("Numele trebuie să conțină doar litere.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(prenume) || !prenume.All(char.IsLetter))
            {
                MessageBox.Show("Prenumele trebuie să conțină doar litere.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@") || !email.Contains("."))
            {
                MessageBox.Show("Introduceți un email valid.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Verificăm dacă emailul sau telefonul există deja
          


            if (string.IsNullOrWhiteSpace(nr_telefon) || nr_telefon.Length != 9 || !nr_telefon.All(char.IsDigit))
            {
                MessageBox.Show("Introduceți un număr de telefon valid (exact 9 cifre).", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(parola) || parola.Length < 6)
            {
                MessageBox.Show("Parola trebuie să aibă cel puțin 6 caractere.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (parola != confirmare_parola)
            {
                MessageBox.Show("Parolele nu coincid.","Eroare",MessageBoxButton.OK,MessageBoxImage.Warning);
                return;
            }

            try
            {

                string parolaHash = BCrypt.Net.BCrypt.HashPassword(parola);

                using (var db = new DataBaseConnection())
                {
                    SqlCommand cmd = new SqlCommand("sp_InregistrarePasager", db.Connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Nume", nume);
                    cmd.Parameters.AddWithValue("@Prenume", prenume);
                    cmd.Parameters.AddWithValue("@NrTelefon", nr_telefon);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@ParolaHash", parolaHash);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Cont creat cu succes!");
                this.Close();


            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("Email deja folosit"))
                    MessageBox.Show("Adresa de email este deja înregistrată.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                else if (ex.Message.Contains("Pasagerul are deja cont"))
                    MessageBox.Show("Acest pasager are deja un cont.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                    MessageBox.Show("Eroare SQL: " + ex.Message, "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
