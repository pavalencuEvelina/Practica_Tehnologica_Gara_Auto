using Practica_Gara_Auto.Connection;
using System;
using System.Collections.Generic;

using Microsoft.Data.SqlClient;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;

namespace Practica_Gara_Auto.WindowsFile
{
    /// <summary>
    /// Interaction logic for ModificareUser.xaml
    /// </summary>
    public partial class ModificareUser : UserControl
    {
        public ModificareUser()
        {
            InitializeComponent();
            LoadComboBoxOptions();
            ResetFields();
           
        }

        private bool parolaVizibila = false;
        private bool parolaNouaVizibila = false;
        private bool parolaNouaConfirmareVizibila = false;
        private void btnToggleParola_Click(object sender, RoutedEventArgs e)
        {
            parolaVizibila = !parolaVizibila;

            if (parolaVizibila)
            {
                txtParolaVizibila.Text = txtParolaCurenta.Password;
                txtParolaCurenta.Visibility = Visibility.Collapsed;
                txtParolaVizibila.Visibility = Visibility.Visible;
                imgOchi.Source = new BitmapImage(new Uri("/WindowsFile/show-password-3.png", UriKind.Relative));
            }
            else
            {
                txtParolaCurenta.Password = txtParolaVizibila.Text;
                txtParolaCurenta.Visibility = Visibility.Visible;
                txtParolaVizibila.Visibility = Visibility.Collapsed;
                imgOchi.Source = new BitmapImage(new Uri("/WindowsFile/hide-password.png", UriKind.Relative));
            }
        }
        private void txtParola_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!parolaVizibila)
                txtParolaVizibila.Text = txtParolaCurenta.Password;
        }

        private void txtParolaVizibila_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (parolaVizibila)
                txtParolaCurenta.Password = txtParolaVizibila.Text;
        }

        private void btnToggleParolaNoua_Click(object sender, RoutedEventArgs e)
        {
            parolaNouaVizibila = !parolaNouaVizibila;

            if (parolaNouaVizibila)
            {
                txtParolaNouaVizibila.Text = txtParolaNoua.Password;
                txtParolaNoua.Visibility = Visibility.Collapsed;
                txtParolaNouaVizibila.Visibility = Visibility.Visible;
                imgOchi2.Source = new BitmapImage(new Uri("/WindowsFile/show-password-3.png", UriKind.Relative));
            }
            else
            {
                txtParolaNoua.Password = txtParolaNouaVizibila.Text;
                txtParolaNoua.Visibility = Visibility.Visible;
                txtParolaNouaVizibila.Visibility = Visibility.Collapsed;
                imgOchi2.Source = new BitmapImage(new Uri("/WindowsFile/hide-password.png", UriKind.Relative));
            }
        }

        private void btnToggleParolaNouaConfirmare_Click(object sender, RoutedEventArgs e)
        {
            parolaNouaConfirmareVizibila = !parolaNouaConfirmareVizibila;

            if (parolaNouaConfirmareVizibila)
            {
                txtParolaNouaConfirmareVizibila.Text = txtParolaNouaConfirmare.Password;
                txtParolaNouaConfirmare.Visibility = Visibility.Collapsed;
                txtParolaNouaConfirmareVizibila.Visibility = Visibility.Visible;
                imgOchi3.Source = new BitmapImage(new Uri("/WindowsFile/show-password-3.png", UriKind.Relative));
            }
            else
            {
                txtParolaNouaConfirmare.Password = txtParolaNouaConfirmareVizibila.Text;
                txtParolaNouaConfirmare.Visibility = Visibility.Visible;
                txtParolaNouaConfirmareVizibila.Visibility = Visibility.Collapsed;
                imgOchi3.Source = new BitmapImage(new Uri("/WindowsFile/hide-password.png", UriKind.Relative));
            }
        }

        private void ButtonAnuleaza_Click(object sender, RoutedEventArgs e)
        {
            Load();
        }

        private void ButtonSalveaza_Click(object sender, RoutedEventArgs e)
        {
            // 1) Obține ID-ul utilizatorului selectat
            int id = (int)ComboUser.SelectedItem;

            // 2) Citește parola curentă din controlul corect
            string parolaCurenta;
            if (parolaVizibila)
                parolaCurenta = txtParolaVizibila.Text.Trim();
            else
                parolaCurenta = txtParolaCurenta.Password.Trim();

            // 3) Citește parola nouă și confirmarea din controlul corect
            string parolaNoua;
            if (parolaNouaVizibila)
                parolaNoua = txtParolaNouaVizibila.Text.Trim();
            else
                parolaNoua = txtParolaNoua.Password.Trim();

            string confirmParolaNoua;
            if (parolaNouaConfirmareVizibila)
                confirmParolaNoua = txtParolaNouaConfirmareVizibila.Text.Trim();
            else
                confirmParolaNoua = txtParolaNouaConfirmare.Password.Trim();

            // 4) Validări inițiale
            if (string.IsNullOrWhiteSpace(parolaNoua) || parolaNoua.Length < 6)
            {
                MessageBox.Show("Parola trebuie să aibă cel puțin 6 caractere.",
                                "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (parolaNoua != confirmParolaNoua)
            {
                MessageBox.Show("Parolele nu coincid.",
                                "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Restul codului (verificare bcrypt, apel SP etc.) rămâne la fel:
            try
            {
                using (var db = new DataBaseConnection())
                {
                    // 5) Citește hash-ul curent din baza de date
                    string storedHash;
                    using (var cmdHash = new SqlCommand(
                        "SELECT ParolaHash FROM Utilizator WHERE IDLegat = @id AND Rol = 'Angajat'",
                        db.Connection))
                    {
                        cmdHash.Parameters.AddWithValue("@id", id);
                        storedHash = (string)cmdHash.ExecuteScalar();
                    }

                    if (string.IsNullOrEmpty(storedHash))
                    {
                        MessageBox.Show("Nu s-a găsit contul pentru angajatul selectat.",
                                        "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // 6) Verifică parola curentă cu BCrypt.Verify
                    if (!BCrypt.Net.BCrypt.Verify(parolaCurenta, storedHash))
                    {
                        MessageBox.Show("Parola curentă este greșită! Încercați din nou.",
                                        "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // 7) Generează hash-ul pentru parola nouă
                    string newHash = BCrypt.Net.BCrypt.HashPassword(parolaNoua);

                    // 8) Apelează procedura stocată cu noul hash
                    using (var cmdUpdate = new SqlCommand("sp_UpdateUserAngajat", db.Connection))
                    {
                        cmdUpdate.CommandType = CommandType.StoredProcedure;
                        cmdUpdate.Parameters.AddWithValue("@IDLegat", id);
                        cmdUpdate.Parameters.AddWithValue("@ParolaHashNoua", newHash);
                        cmdUpdate.ExecuteNonQuery();
                    }

                    MessageBox.Show("Parola a fost schimbată cu succes!",
                                    "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                    ResetFields();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Eroare la actualizarea parolei: " + ex.Message,
                                "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void SearchTextChanged(object sender, EventArgs e)
        {

            Load();

        }

        private void Load()
        {
            int id = (int)ComboUser.SelectedItem;

            using (var db = new DataBaseConnection())
            {

                var cmd = new SqlCommand(@"
                            SELECT Email
                            FROM Utilizator_angajat
                            WHERE IDLegat = @id", db.Connection);
                cmd.Parameters.AddWithValue("@id", id);

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {

                    txtEmail.Text = reader.GetString(0);
                    txtEmail.IsEnabled = false;
                    txtParolaVizibila.Text = "";
                    txtParolaCurenta.Password = "";
                    txtParolaNoua.Password = "";
                    txtParolaNouaConfirmare.Password = "";
                    txtParolaNouaVizibila.Text = "";
                    txtParolaNouaConfirmareVizibila.Text = "";


                }
            }
        }

        private void LoadComboBoxOptions()
        {
            ComboUser.Items.Clear();

            using (var db = new DataBaseConnection())
            {

                SqlCommand cmdID = new SqlCommand("SELECT IDLegat FROM Utilizator_angajat ORDER BY IDLegat", db.Connection);
                SqlDataReader readerID = cmdID.ExecuteReader();
                while (readerID.Read())
                {
                    int id = readerID.GetInt32(0);
                    ComboUser.Items.Add(id);
                }
                readerID.Close();


            }

        }

        private void ResetFields()
        {
            ComboUser.SelectedIndex = 0;

        }
    }
}
