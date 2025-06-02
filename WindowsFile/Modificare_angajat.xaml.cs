using Microsoft.Data.SqlClient;
using Practica_Gara_Auto.Connection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
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
    /// Interaction logic for Modificare_angajat.xaml
    /// </summary>
    public partial class Modificare_angajat : UserControl
    {
       
        public Modificare_angajat()
        {
            InitializeComponent();
            LoadComboBoxOptions();
            ResetFields();
        }

        private void ButtonSalveaza_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)cmbID.SelectedItem;
            string nume = txtNume.Text.Trim();
            string prenume = txtPrenume.Text;
            string nr_telefon = txtTelefon.Text.Trim();
            string post = cmbPost.SelectedItem.ToString();
            string localitate = cmbLocalitate.SelectedItem.ToString();

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

            if (string.IsNullOrWhiteSpace(nr_telefon) || nr_telefon.All(char.IsLetter) || nr_telefon.Length != 9)
            {
                MessageBox.Show("Nr de telefon trebuie să conțină 13 cifre.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {

                using (var db = new DataBaseConnection())
                {
                    SqlCommand cmd = new SqlCommand("dbo.sp_UpdateAngajat", db.Connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Nume", nume);
                    cmd.Parameters.AddWithValue("@IDAngajat", id);
                    cmd.Parameters.AddWithValue("@Prenume", prenume);
                    cmd.Parameters.AddWithValue("@NrTelefon", nr_telefon);
                    cmd.Parameters.AddWithValue("@Post", post);
                    cmd.Parameters.AddWithValue("@Localitate", localitate);
                   
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Angajat modificat cu succes!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);

                ResetFields();


            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("Numărul de telefon aparține altui angajat."))
                    MessageBox.Show("Numărul de telefon aparține altui angajat.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                else if (ex.Message.Contains("Nu s-au făcut modificări (datele erau identice)."))
                    MessageBox.Show("Nu s-au făcut modificări (datele erau identice).", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show("Eroare SQL: " + ex.Message, "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void ButtonAnuleaza_Click(object sender, RoutedEventArgs e)
        {
            if (cmbID.SelectedItem is int selectedId)
                LoadAngajatData(selectedId);
        }

        private void SearchTextChanged(object sender, EventArgs e)
        {

            if (cmbID.SelectedItem is int selectedId)
                LoadAngajatData(selectedId);

        }

        private void LoadAngajatData(int angajatId)
        {
            int id = angajatId;

            using (var db = new DataBaseConnection())
            {

                var cmd = new SqlCommand(@"
            SELECT Nume, Prenume, Nr_telefon,
                   Localitate, Post
            FROM Angajat_Complet
            WHERE IDAngajat = @id", db.Connection);
                cmd.Parameters.AddWithValue("@id", id);

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {

                    txtNume.Text = reader.GetString(0);
                    txtPrenume.Text = reader.GetString(1);

                    txtTelefon.Text = reader.GetString(2);

                    string Localitate = reader.GetString(3);
                    string Post = reader.GetString(4);

                    cmbLocalitate.SelectedValue = Localitate;
                    cmbPost.SelectedValue = Post;
                }
            }
        }


        private void LoadComboBoxOptions()
        {
            cmbID.Items.Clear();
            cmbLocalitate.Items.Clear();
            cmbPost.Items.Clear();

            using (var db = new DataBaseConnection()) 
            {
                
                SqlCommand cmdID = new SqlCommand("SELECT IDAngajat FROM Angajat ORDER BY IDAngajat", db.Connection);
                SqlDataReader readerID = cmdID.ExecuteReader();
                while (readerID.Read())
                {
                    int id = readerID.GetInt32(0);
                    cmbID.Items.Add(id);
                }
                readerID.Close();

                SqlCommand cmdPost = new SqlCommand("SELECT Denumire FROM Post", db.Connection);
                SqlDataReader readerPost = cmdPost.ExecuteReader();
                while (readerPost.Read())
                {
                    cmbPost.Items.Add(readerPost.GetString(0)); ;
                }
                readerPost.Close();

                SqlCommand cmdLoc = new SqlCommand("SELECT Denumire FROM Localitate", db.Connection);
                SqlDataReader readerLoc = cmdLoc.ExecuteReader();
                while (readerLoc.Read())
                {
                    cmbLocalitate.Items.Add(readerLoc.GetString(0));
                }
                readerLoc.Close();

            }

        }

        private void ResetFields()
        {
            cmbID.SelectedIndex = 0;
        }

    }
}
