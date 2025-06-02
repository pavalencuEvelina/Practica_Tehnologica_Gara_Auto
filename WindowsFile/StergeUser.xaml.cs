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
    /// Interaction logic for StergeUser.xaml
    /// </summary>
    public partial class StergeUser : UserControl
    {
        public StergeUser()
        {
            InitializeComponent();
            LoadComboBoxOptions();
            ResetFields();
        }

        private void BtnSterge_Click(object sender, RoutedEventArgs e)
        {
            int id= (int)ComboUser.SelectedItem;
            try
            {
                
                using (var db = new DataBaseConnection())
                {
                    SqlCommand cmd = new SqlCommand("sp_DeleteUser", db.Connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                   
                    cmd.Parameters.AddWithValue("@IdUser", id);

                    cmd.ExecuteNonQuery();
                }

                var raspuns = MessageBox.Show(
                    "Ești sigur că vrei să ștergi (dezactivezi) contul acestui angajat?",
                    "Confirmare ștergere",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (raspuns != MessageBoxResult.Yes)
                {
                    ResetFields();
                    return;
                }
                MessageBox.Show("User angajat șters cu succes!");
                ResetFields();


            }
            catch (SqlException ex)
            {
               
                    MessageBox.Show("Eroare SQL: " + ex.Message, "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void LoadComboBoxOptions()
        {
            ComboUser.Items.Clear();

            using (var db = new DataBaseConnection()) // 
            {
                // === Angajat ===
                SqlCommand cmdAngajat = new SqlCommand("SELECT IDAngajat FROM Conturi_Angajat", db.Connection);
                SqlDataReader readerAngajat = cmdAngajat.ExecuteReader();
                while (readerAngajat.Read())
                {
                    int id = readerAngajat.GetInt32(0);
                    ComboUser.Items.Add(id);
                }
                readerAngajat.Close();

            }


        }

        private void ResetFields()
        {
            ComboUser.SelectedIndex = 0;
        }
    }
}
