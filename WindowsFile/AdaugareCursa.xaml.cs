using Microsoft.Data.SqlClient;
using Practica_Gara_Auto.Connection;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Practica_Gara_Auto.WindowsFile
{
    /// <summary>
    /// Interaction logic for AdaugareCursa.xaml
    /// </summary>
    public partial class AdaugareCursa : UserControl
    {
        private readonly string _mod;
        private readonly int? _idCursa;
        public int? ID_Cursa { get; private set; }

        public AdaugareCursa(string mod)
        {
            InitializeComponent();
            titlu.Text = "🚌 Adaugă Cursă Nouă";
            _mod = mod;            // "adaugare"
            _idCursa = null;
            ID_Cursa = null;
            InitComboBoxuri();
        }

        public AdaugareCursa(string mod, int idCursa)
        {
            InitializeComponent();
            titlu.Text = "🚌 Modifică Cursă";
            _mod = mod;            // "modificare"
            _idCursa = idCursa;
            ID_Cursa = idCursa;
            InitComboBoxuri();
            IncarcaDateCursa(_idCursa.Value);
        }

        private void InitComboBoxuri()
        {
            IncarcaTrasee();
            IncarcaTipTransport();
            IncarcaAngajati();
            
        }

        #region Încărcare date în ComboBox-uri

        private void IncarcaTrasee()
        {
            var listaTrasee = new List<TraseuModel>();
            using (var db = new DataBaseConnection())
            {
               
                string sql = @"
                    SELECT 
                        t.IDTraseu,
                        sp.Denumire AS Localitate_Pornire,
                        sd.Denumire AS Localitate_Destinatie,
                        t.Km
                    FROM Traseu t
                    JOIN Statie s1 ON t.PunctPornireID = s1.IDStatie
                    JOIN Localitate sp ON s1.IDLocalitate = sp.IDLocalitate
                    JOIN Statie s2 ON t.DestinatieID = s2.IDStatie
                    JOIN Localitate sd ON s2.IDLocalitate = sd.IDLocalitate
                    ORDER BY sp.Denumire, sd.Denumire;
                ";

                using (var cmd = new SqlCommand(sql, db.Connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listaTrasee.Add(new TraseuModel
                        {
                            IDTraseu = reader.GetInt32(0),
                            Afisare = $"{reader.GetString(1)} - {reader.GetString(2)} ({reader.GetDouble(3):0.00} km)"
                        });
                    }
                }
            }

            // Setăm lista ca sursă de date, fără DisplayMemberPath
            ComboTraseu.ItemsSource = listaTrasee;
            ComboTraseu.SelectedIndex = -1;
        }

        private void IncarcaTipTransport()
        {
            var listaTip = new List<TipTransportModel>();
            using (var db = new DataBaseConnection())
            {
     
                string sql = "SELECT IDTipTransport, Denumire, Nr_locuri FROM TipTransport ORDER BY Denumire;";
                using (var cmd = new SqlCommand(sql, db.Connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listaTip.Add(new TipTransportModel
                        {
                            IDTipTransport = reader.GetInt32(0),
                            Denumire = reader.GetString(1),
                            NrLocuri = reader.GetInt32(2)
                        });
                    }
                }
            }

            ComboTipTransport.ItemsSource = listaTip;
            ComboTipTransport.SelectedIndex = -1;
        }

        private void IncarcaAngajati()
        {
            var listaAngajati = new List<AngajatModel>();
            using (var db = new DataBaseConnection())
            {

                string sql = @"
                    SELECT 
                        a.IDAngajat,
                        a.Nume,
                        a.Prenume
                    FROM Angajat a
                    WHERE a.IDPost=3
                    ORDER BY a.Nume, a.Prenume
                    ;
                ";
                using (var cmd = new SqlCommand(sql, db.Connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listaAngajati.Add(new AngajatModel
                        {
                            IDAngajat = reader.GetInt32(0),
                            NumeComplet = $"{reader.GetString(1)} {reader.GetString(2)}"
                        });
                    }
                }
            }

            ComboAngajat.ItemsSource = listaAngajati;
            ComboAngajat.SelectedIndex = -1;
        }

       

        #endregion

        #region Încărcarea datelor (modificare)

        private void IncarcaDateCursa(int idCursa)
        {
            try
            {
                using (var db = new DataBaseConnection())
                {
                   
                    string sql = @"
                        SELECT 
                            IDTraseu,
                            IDTipTransport,
                            IDAngajat,
                            DataPlecare,
                            OraPlecare
                        FROM Cursa
                        WHERE IDCursa = @IDCursa;
                    ";
                    using (var cmd = new SqlCommand(sql, db.Connection))
                    {
                        cmd.Parameters.AddWithValue("@IDCursa", idCursa);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // 1) Traseu
                                int idTr = reader.GetInt32(0);
                                // Căutăm obiectul din ItemsSource cu acel ID
                                ComboTraseu.SelectedItem =
                                  (ComboTraseu.ItemsSource as List<TraseuModel>)
                                  .Find(x => x.IDTraseu == idTr);

                                // 2) TipTransport
                                int idTip = reader.GetInt32(1);
                                ComboTipTransport.SelectedItem =
                                  (ComboTipTransport.ItemsSource as List<TipTransportModel>)
                                  .Find(x => x.IDTipTransport == idTip);

                                // 3) Angajat
                                int idAng = reader.GetInt32(2);
                                ComboAngajat.SelectedItem =
                                  (ComboAngajat.ItemsSource as List<AngajatModel>)
                                  .Find(x => x.IDAngajat == idAng);

                                // 4) DataPlecare
                                DatePickerDataPlecare.SelectedDate = reader.GetDateTime(3).Date;

                                // 5) OraPlecare
                                TimeSpan oraPlec = reader.GetTimeSpan(4);
                                TextBoxOraPlecare.Text = oraPlec.ToString(@"hh\:mm");

                                // 6) IDProgram (null sau valoare)
                                
                            }
                            else
                            {
                                TextBlockFeedback.Text = "Cursa nu a fost găsită.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TextBlockFeedback.Text = $"Eroare la încărcarea datelor: {ex.Message}";
            }
        }

        #endregion

        #region Evenimente CheckBox Program


        #endregion

        #region Salvare (insert/update)

        private void BtnAdaugaCursa_Click(object sender, RoutedEventArgs e)
        {
            TextBlockFeedback.Text = "";
            TextBlockFeedback.Foreground = System.Windows.Media.Brushes.Red;

            // Validări comune
            if (!(ComboTraseu.SelectedItem is TraseuModel traseuSelectat))
            {
                TextBlockFeedback.Text = "Trebuie să selectați un traseu.";
                return;
            }
            if (!(ComboTipTransport.SelectedItem is TipTransportModel tipSelectat))
            {
                TextBlockFeedback.Text = "Trebuie să selectați un tip de transport.";
                return;
            }
            if (!(ComboAngajat.SelectedItem is AngajatModel angSelectat))
            {
                TextBlockFeedback.Text = "Trebuie să selectați un angajat.";
                return;
            }
            if (!DatePickerDataPlecare.SelectedDate.HasValue)
            {
                TextBlockFeedback.Text = "Trebuie să selectați o dată de plecare.";
                return;
            }
            if (string.IsNullOrWhiteSpace(TextBoxOraPlecare.Text))
            {
                TextBlockFeedback.Text = "Trebuie să introduceți ora de plecare.";
                return;
            }
            if (!TimeSpan.TryParseExact(
                    TextBoxOraPlecare.Text.Trim(), @"hh\:mm",
                    null, out TimeSpan oraPlecare))
            {
                TextBlockFeedback.Text = "Format incorect pentru ora de plecare. Folosiți HH:mm, ex. 08:30.";
                return;
            }

            // Colectăm valorile din UI
            int idTraseu = traseuSelectat.IDTraseu;
            int idTipTransport = tipSelectat.IDTipTransport;
            int idAngajat = angSelectat.IDAngajat;
            DateTime dataPlecare = DatePickerDataPlecare.SelectedDate.Value.Date;
            TimeSpan oraPlec = oraPlecare;

            try
            {
                using (var db = new DataBaseConnection())
                {
                    // ─── PASUL 1: verificare dacă șoferul există deja în aceeași zi și oră ───
                    string sqlVerif = @"
                SELECT COUNT(*) 
                FROM Cursa 
                WHERE 
                  IDAngajat    = @IDAngajat 
                  AND DataPlecare = @DataPlecare 
                  AND OraPlecare  = @OraPlecare
                  AND (
                        @Mod = 'adaugare' 
                        OR IDCursa <> @IDCursa
                      );
            ";

                    using (var cmdVerif = new SqlCommand(sqlVerif, db.Connection))
                    {
                        cmdVerif.Parameters.AddWithValue("@IDAngajat", idAngajat);
                        cmdVerif.Parameters.AddWithValue("@DataPlecare", dataPlecare);
                        cmdVerif.Parameters.AddWithValue("@OraPlecare", oraPlec);
                        cmdVerif.Parameters.AddWithValue("@Mod", _mod);
                        cmdVerif.Parameters.AddWithValue("@IDCursa", _idCursa ?? 0);

                        int count = Convert.ToInt32(cmdVerif.ExecuteScalar());
                        if (count > 0)
                        {
                            TextBlockFeedback.Text =
                                "Acest șofer are deja o cursă programată în aceeași zi și oră.";
                            return;
                        }
                    }

                    // ─── PASUL 2: dacă trecem verificarea, continuăm cu inserarea sau actualizarea ───
                    if (_mod == "adaugare")
                    {
                        using (var cmd = new SqlCommand("InsertCursa", db.Connection))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@IDTraseu", idTraseu);
                            cmd.Parameters.AddWithValue("@IDTipTransport", idTipTransport);
                            cmd.Parameters.AddWithValue("@IDAngajat", idAngajat);
                            cmd.Parameters.AddWithValue("@DataPlecare", dataPlecare);
                            cmd.Parameters.AddWithValue("@OraPlecare", oraPlec);

                            object result = cmd.ExecuteScalar();
                            int cursaNouaID = Convert.ToInt32(result);
                            ID_Cursa = cursaNouaID;

                            TextBlockFeedback.Foreground = System.Windows.Media.Brushes.Green;
                            TextBlockFeedback.Text = $"Cursa a fost adăugată cu succes! (ID = {cursaNouaID})";
                        }
                    }
                    else if (_mod == "modificare")
                    {
                        if (!_idCursa.HasValue)
                        {
                            TextBlockFeedback.Text = "ID-ul cursei nu este specificat pentru modificare.";
                            return;
                        }

                        using (var cmd = new SqlCommand("UpdateCursa", db.Connection))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@IDCursa", _idCursa.Value);
                            cmd.Parameters.AddWithValue("@IDTraseu", idTraseu);
                            cmd.Parameters.AddWithValue("@IDTipTransport", idTipTransport);
                            cmd.Parameters.AddWithValue("@IDAngajat", idAngajat);
                            cmd.Parameters.AddWithValue("@DataPlecare", dataPlecare);
                            cmd.Parameters.AddWithValue("@OraPlecare", oraPlec);

                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                ID_Cursa = _idCursa.Value;
                                TextBlockFeedback.Foreground = System.Windows.Media.Brushes.Green;
                                TextBlockFeedback.Text = $"Cursa (ID = {_idCursa.Value}) a fost actualizată cu succes!";
                            }
                            else
                            {
                                TextBlockFeedback.Text = "Nicio cursă nu a fost actualizată (verificați ID-ul).";
                            }
                        }
                    }
                    else
                    {
                        TextBlockFeedback.Text = $"Mod necunoscut: {_mod}";
                    }
                }
            }
            catch (Exception ex)
            {
                TextBlockFeedback.Foreground = System.Windows.Media.Brushes.Red;
                TextBlockFeedback.Text = $"Eroare la salvarea cursei: {ex.Message}";
            }
        }


        #endregion

        #region Anulează

        private void ButtonAnuleaza_Click(object sender, RoutedEventArgs e)
        {
            ComboTraseu.SelectedIndex = -1;
            ComboTipTransport.SelectedIndex = -1;
            ComboAngajat.SelectedIndex = -1;
            DatePickerDataPlecare.SelectedDate = null;
            TextBoxOraPlecare.Clear();
           
            TextBlockFeedback.Text = "";
            ID_Cursa = null;
        }

        #endregion

        public class TraseuModel
        {
            public int IDTraseu { get; set; }
            public string Afisare { get; set; }

            public override string ToString()
            {
                return Afisare;
            }
        }

        public class TipTransportModel
        {
            public int IDTipTransport { get; set; }
            public string Denumire { get; set; }
            public int NrLocuri { get; set; }

            public override string ToString()
            {
                return Denumire;
            }
        }

        public class AngajatModel
        {
            public int IDAngajat { get; set; }
            public string NumeComplet { get; set; }

            public override string ToString()
            {
                return NumeComplet;
            }
        }

        public class ProgramModel
        {
            public int? IDProgram { get; set; }
            public string Afisare { get; set; }

            public override string ToString()
            {
                return Afisare;
            }
        }


       
    }
}

