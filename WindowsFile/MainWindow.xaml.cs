using Microsoft.Data.SqlClient;
using Practica_Gara_Auto.Clase;
using Practica_Gara_Auto.Connection;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BCrypt.Net;

namespace Practica_Gara_Auto.WindowsFile
{
    public partial class MainWindow : Window
    {
        private bool isSyncing = false;

        public MainWindow()
        {
            InitializeComponent();
        }



private void BtnAutentificare_Click(object sender, RoutedEventArgs e)
    {
        string identificator = txtEmail.Text.Trim();
           
        string parola = GetParola();
        if (string.IsNullOrWhiteSpace(identificator) || string.IsNullOrWhiteSpace(parola))
        {
            MessageBox.Show("Completează toate câmpurile!", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        string queryUser = "SELECT ID, Email, Rol, IDLegat, ParolaHash FROM Utilizator WHERE Email = @email";

        UtilizatorModel utilizator = null;

        var db = new DataBaseConnection();
        SqlCommand cmd = new SqlCommand(queryUser, db.Connection);
        cmd.Parameters.AddWithValue("@email", identificator);

        var reader = cmd.ExecuteReader();
        string parolaHashFromDb = null;

        if (reader.Read())
        {
            parolaHashFromDb = reader.GetString(4);

            // Verificăm parola cu bcrypt
            if (BCrypt.Net.BCrypt.Verify(parola, parolaHashFromDb))
            {
                utilizator = new UtilizatorModel
                {
                    IdUtilizator = reader.GetInt32(0),
                    Email = reader.GetString(1),
                    Rol = reader.GetString(2),
                    IdPersoana = reader.GetInt32(3)
                };
            }
        }
        reader.Close();

        if (utilizator == null)
        {
            db.Close();
            MessageBox.Show("Email sau parolă greșită!", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        // Dacă e angajat sau pasager, citește nume + prenume
        if (utilizator.Rol.ToLower() == "angajat")
        {
            SqlCommand cmdAngajat = new SqlCommand("SELECT Nume, Prenume,Denumire as Post FROM Angajat JOIN Post ON Angajat.IDPost=Post.IDPost WHERE IDAngajat = @id", db.Connection);
            cmdAngajat.Parameters.AddWithValue("@id", utilizator.IdPersoana);
            var angajatReader = cmdAngajat.ExecuteReader();
            if (angajatReader.Read())
            {
                utilizator.Nume = angajatReader.GetString(0);
                utilizator.Prenume = angajatReader.GetString(1);
                utilizator.Post = angajatReader.GetString(2);
            }
            angajatReader.Close();
        }
        else if (utilizator.Rol.ToLower() == "pasager")
        {
            SqlCommand cmdPasager = new SqlCommand("SELECT Nume, Prenume FROM Pasager WHERE IDPasager = @id", db.Connection);
            cmdPasager.Parameters.AddWithValue("@id", utilizator.IdPersoana);
            var pasagerReader = cmdPasager.ExecuteReader();
            if (pasagerReader.Read())
            {
                utilizator.Nume = pasagerReader.GetString(0);
                utilizator.Prenume = pasagerReader.GetString(1);
            }
            pasagerReader.Close();
        }

        db.Close();

        Window nextWindow = null;

        switch (utilizator.Rol.ToLower())
        {
            case "admin":
              
                nextWindow = new WindowAdmin(utilizator);
                break;
            case "angajat":
                
                nextWindow = new WindowAngajat(utilizator);
                break;
            case "pasager":
                
                nextWindow = new WindowClient(utilizator);
                break;
            default:
                MessageBox.Show("Rol necunoscut.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
        }

        nextWindow.Show();
        this.Close();
    }


    private string GetParola()
        {
            return txtParolaHidden.Visibility == Visibility.Visible
                ? txtParolaHidden.Password
                : txtParolaVisible.Text;
        }

        private bool isPasswordVisible = false;

        private void BtnTogglePassword_Click(object sender, RoutedEventArgs e)
        {
            isPasswordVisible = !isPasswordVisible;

            if (isPasswordVisible)
            {
                txtParolaVisible.Text = txtParolaHidden.Password;
                txtParolaVisible.Visibility = Visibility.Visible;
                txtParolaHidden.Visibility = Visibility.Collapsed;
                imgEye.Source = new BitmapImage(new Uri("show-password-3.png", UriKind.Relative));
            }
            else
            {
                txtParolaHidden.Password = txtParolaVisible.Text;
                txtParolaHidden.Visibility = Visibility.Visible;
                txtParolaVisible.Visibility = Visibility.Collapsed;
                imgEye.Source = new BitmapImage(new Uri("hide-password.png", UriKind.Relative));
            }
        }



        private void Inregistrare_Click(object sender, RoutedEventArgs e)
        {
            // Deschide o fereastră de înregistrare (dacă ai una)
            WindowInregistrare window = new WindowInregistrare();
            window.ShowDialog();
            
        }

        private void txtEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtEmailPlaceholder.Visibility = string.IsNullOrEmpty(txtEmail.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }



        // Sincronizează textul între textbox și passwordbox
        private void txtParolaVisible_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!isSyncing && txtParolaHidden.Visibility == Visibility.Collapsed)
            {
                isSyncing = true;
                txtParolaHidden.Password = txtParolaVisible.Text;
                isSyncing = false;
            }
        }

        private void txtParolaHidden_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!isSyncing && txtParolaVisible.Visibility == Visibility.Collapsed)
            {
                isSyncing = true;
                txtParolaVisible.Text = txtParolaHidden.Password;
                isSyncing = false;
            }

            placeholderText.Visibility = string.IsNullOrEmpty(txtParolaHidden.Password)
        ? Visibility.Visible
        : Visibility.Collapsed;

            if (!isPasswordVisible)
                txtParolaVisible.Text = txtParolaHidden.Password;


        }

       
    }
}
