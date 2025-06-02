using DocumentFormat.OpenXml.Bibliography;
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
    /// Interaction logic for StergereCurse.xaml
    /// </summary>
    public partial class StergereCurse : UserControl
    {
        public StergereCurse()
        {
            InitializeComponent();
            LoadComboBoxOptions();
        }

        private void BtnSterge_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)ComboCursa.SelectedItem;
            try
            {

                using (var db = new DataBaseConnection())
                {
                    SqlCommand cmd = new SqlCommand("sp_DeleteCursa", db.Connection);
                    cmd.CommandType = CommandType.StoredProcedure;


                    cmd.Parameters.AddWithValue("@IDCursa", id);

                    cmd.ExecuteNonQuery();
                }

                var raspuns = MessageBox.Show(
                    "Ești sigur că vrei să ștergi cursa?",
                    "Confirmare ștergere",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (raspuns != MessageBoxResult.Yes)
                {
                    ResetFields();
                    return;
                }
                MessageBox.Show("Cursă ștearsă cu succes!");
                ResetFields();


            }
            catch (SqlException ex)
            {

                MessageBox.Show("Eroare SQL: " + ex.Message, "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadComboBoxOptions()
        {
            ComboCursa.Items.Clear();

            using (var db = new DataBaseConnection()) // 
            {
                // === Angajat ===
                SqlCommand cmdAngajat = new SqlCommand("SELECT IDCursa FROM Cursa", db.Connection);
                SqlDataReader readerAngajat = cmdAngajat.ExecuteReader();
                while (readerAngajat.Read())
                {
                    int id = readerAngajat.GetInt32(0);
                    ComboCursa.Items.Add(id);
                }
                readerAngajat.Close();
            }
        }

        private void ResetFields()
        {
            ComboCursa.SelectedIndex = 0;
        }
    }
}
