using Microsoft.CodeAnalysis.VisualBasic.Syntax;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Practica_Gara_Auto.WindowsFile
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
            LoadComboBoxOptions();
            ResetFields();
        }

        private void ButtonSalveaza_Click(object sender, RoutedEventArgs e)
        {
            string nume = txtNume.Text.Trim();
            string prenume = txtPrenume.Text;
            string CNP = txtCNP.Text.Trim();
            string nr_telefon = txtTelefon.Text.Trim();
            string gen = radioFeminin.IsChecked==true ? radioFeminin.Content.ToString() : radioMasculin.Content.ToString();
            DateTime data_nastere = DatePicker1.SelectedDate.Value;
            string post=cmbPost.SelectedItem.ToString();
            string localitate=cmbLocalitate.SelectedItem.ToString();



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

            if (string.IsNullOrWhiteSpace(CNP) || CNP.All(char.IsLetter) || CNP.Length!=13)
            {
                MessageBox.Show("CNP-ul trebuie să conțină 13 cifre.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(nr_telefon) || nr_telefon.All(char.IsLetter) ||  nr_telefon.Length != 9)
            {
                MessageBox.Show("Nr de telefon trebuie să conțină 13 cifre.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {

                using (var db = new DataBaseConnection())
                {
                    SqlCommand cmd = new SqlCommand("sp_AngajatNou", db.Connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Nume", nume);
                    cmd.Parameters.AddWithValue("@Prenume", prenume);
                    cmd.Parameters.AddWithValue("@NrTelefon", nr_telefon);
                    cmd.Parameters.AddWithValue("@CNP", CNP);
                    cmd.Parameters.AddWithValue("@Post", post);
                    cmd.Parameters.AddWithValue("@Localitate", localitate);
                    cmd.Parameters.AddWithValue("@Gen", gen);
                    cmd.Parameters.AddWithValue("@Data_nastere", data_nastere);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Angajat creat cu succes!");

               ResetFields();


            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("Numărul de telefon aparține altui angajat"))
                    MessageBox.Show("Numărul de telefon aparține altui angajat.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                else if (ex.Message.Contains("CNP-ul aparține altui angajat."))
                    MessageBox.Show("CNP-ul aparține altui angajat.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                else if (ex.Message.Contains("Angajatul nu poate avea vârsta mai mică de 18 ani."))
                    MessageBox.Show("Angajatul nu poate avea vârsta mai mică de 18 ani.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                    MessageBox.Show("Eroare SQL: " + ex.Message, "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }




        }

        private void LoadComboBoxOptions()
        {
           cmbLocalitate.Items.Clear();
            cmbPost.Items.Clear();

            using (var db = new DataBaseConnection()) // clasa ta de conexiune
            {
                // === Post ===
                SqlCommand cmdPost = new SqlCommand("SELECT Denumire FROM Post", db.Connection);
                SqlDataReader readerPost = cmdPost.ExecuteReader();
                while (readerPost.Read())
                {
                    cmbPost.Items.Add(readerPost.GetString(0));
                }
                readerPost.Close();

                // === Localitate ===
                SqlCommand cmdLoc = new SqlCommand("SELECT Denumire FROM Localitate", db.Connection);
                SqlDataReader readerLoc = cmdLoc.ExecuteReader();
                while (readerLoc.Read())
                {
                    cmbLocalitate.Items.Add(readerLoc.GetString(0));
                }
                readerLoc.Close();
            }

           
        }

        private void ButtonAnuleaza_Click(object sender, RoutedEventArgs e)
        {
            ResetFields();
        }

        private void ResetFields()
        {
            txtNume.Text = "";
            txtPrenume.Text = "";
            txtTelefon.Text = "";
            txtCNP.Text = "";
            cmbPost.SelectedIndex = 0;
            cmbLocalitate.SelectedIndex = 0;
            DatePicker1.SelectedDate = DateTime.Now;
            radioMasculin.IsChecked = true;
            radioFeminin.IsChecked = false;


        }



    }
}
