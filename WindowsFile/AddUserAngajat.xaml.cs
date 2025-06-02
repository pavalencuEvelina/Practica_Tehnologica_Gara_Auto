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
    /// Interaction logic for AddUserAngajat.xaml
    /// </summary>
    public partial class AddUserAngajat : UserControl
    {
        public AddUserAngajat()
        {
            InitializeComponent();
            LoadComboBoxOptions();
            ResetFields();
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

        private void BtnCreeaza_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)ComboAngajati.SelectedItem;
            string email = TxtEmail.Text;
            string parola = txtParolaVizibila.Text.Trim();
            string confirmare_parola = txtConfirmareParolaVizibila.Text.Trim();



            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@") || !email.Contains("."))
            {
                MessageBox.Show("Introduceți un email valid.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            if (string.IsNullOrWhiteSpace(parola) || parola.Length < 6)
            {
                MessageBox.Show("Parola trebuie să aibă cel puțin 6 caractere.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (parola != confirmare_parola)
            {
                MessageBox.Show("Parolele nu coincid.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string parolaHash = BCrypt.Net.BCrypt.HashPassword(parola);

                using (var db = new DataBaseConnection())
                {
                    SqlCommand cmd = new SqlCommand("sp_UserAngajatNou", db.Connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@ParolaHash", parolaHash);
                    cmd.Parameters.AddWithValue("@IdLegat", id);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("User angajat creat cu succes!");
                ResetFields();


            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("Acest angajat are deja cont"))
                    MessageBox.Show("Acest angajat are deja cont", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                else if (ex.Message.Contains("Email deja folosit."))
                    MessageBox.Show("Email deja folosit.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                    MessageBox.Show("Eroare SQL: " + ex.Message, "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }

        private void BtnAnuleaza_Click(object sender, RoutedEventArgs e)
        {
            ResetFields();
        }

        private void ResetFields()
        {
            LoadComboBoxOptions();
            ComboAngajati.SelectedIndex = 0;
            TxtEmail.Text = "";
            txtParola.Password = "";
            txtConfirmareParola.Password = "";
            txtParolaVizibila.Text = "";
            txtConfirmareParolaVizibila.Text = "";
        }

        private void LoadComboBoxOptions()
        {
            ComboAngajati.Items.Clear();

            using (var db = new DataBaseConnection()) // 
            {
                // === Angajat ===
                SqlCommand cmdPost = new SqlCommand("SELECT IDAngajat FROM Angajat_Complet WHERE AreCont='Nu'", db.Connection);
                SqlDataReader readerPost = cmdPost.ExecuteReader();
                while (readerPost.Read())
                {
                    int id = readerPost.GetInt32(0);
                    ComboAngajati.Items.Add(id);
                }
                readerPost.Close();

            }


        }

    }
}
