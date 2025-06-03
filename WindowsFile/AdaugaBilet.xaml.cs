using Microsoft.Data.SqlClient;
using Practica_Gara_Auto.Clase;
using Practica_Gara_Auto.Connection;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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


namespace Practica_Gara_Auto.WindowsFile
{
    /// <summary>
    /// Interaction logic for AdaugaBilet.xaml
    /// </summary>
    public partial class AdaugaBilet : UserControl
    {
        private CursaModel _cursaSelectata;
        private const double PretPeKm = 0.35;
        UtilizatorModel _utilizatorModel;

        public AdaugaBilet(UtilizatorModel u)
        {
            _utilizatorModel = u;
            InitializeComponent();
            IncarcaCurse(); // populăm ComboAngajati.ItemsSource mai întâi

            // După ce lista e completă în ComboAngajati, forțăm selecția pe primul element:
            if (ComboAngajati.Items.Count > 0)
            {
                ComboAngajati.SelectedIndex = 0;            // setăm vizual primul element
                _cursaSelectata = ComboAngajati.Items[0] as CursaModel; // atribuim modelul
            }

            // Afișăm prețul inițial pentru 1 bilet și cursa de pe poziția 0:
            NumericUpDownControl.Value = 1;
            RecalculeazaPret();

            // Înregistrăm handler-ii, de-acum încolo orice schimbare reapelază RecalculeazaPret()
            ComboAngajati.SelectionChanged += ComboAngajati_SelectionChanged;
            NumericUpDownControl.ValueChanged += NumericUpDownControl_ValueChanged;

        }

        private void IncarcaCurse()
        {
            var listaCurse = new List<CursaModel>();

            // Exemplu de query: aducem IDCursa și Km
            using (DataBaseConnection conn = new DataBaseConnection())
            {

                string sql = @"
                                SELECT 
                            IDCursa,
                            Traseu,
                            km,
                            DataPlecare,
                            OraPlecare
                        FROM Curse_Complet
                        WHERE DataPlecare >= CAST(GETDATE() AS DATE)
                        ORDER BY IDCursa;
                                                                     ";


                using (var cmd = new SqlCommand(sql, conn.Connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int idCursa = reader.GetInt32(0);
                        string traseu = reader.GetString(1);
                        double km = reader.GetDouble(2);
                        DateTime dt = reader.GetDateTime(3).Date
                                        + (TimeSpan)reader[4]; // OraPlecare

                        // Formatăm textul afișat în ComboBox
                        string afisaj = $"Cursa {idCursa} - Traseu {traseu} — {dt:dd.MM.yyyy HH:mm}";

                        listaCurse.Add(new CursaModel
                        {
                            IDCursa = idCursa,
                            Km = km,
                            DenumireAfisaj = afisaj
                        });
                    }
                }

                // Legăm lista la ComboBox:
                ComboAngajati.ItemsSource = listaCurse;
            }
        }

        // La schimbarea cursei, salvăm modelul și recalculăm prețul
        private void ComboAngajati_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _cursaSelectata = ComboAngajati.SelectedItem as CursaModel;
            RecalculeazaPret();
        }

        // La schimbarea numărului de bilete, recalculează prețul
        private void NumericUpDownControl_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            RecalculeazaPret();
        }

        // Metodă comună de recalcule:
        private void RecalculeazaPret()
        {
            if (_cursaSelectata == null)
            {
                LabelPret.Content = "Preț: 0 lei";
                return;
            }

            int numarBilete = NumericUpDownControl.Value ?? 0;
            if (numarBilete <= 0)
            {
                LabelPret.Content = "Preț: 0 lei";
                return;
            }

            // Preț per bilet = km * PretPeKm
            double pretUnitar = _cursaSelectata.Km * PretPeKm;
            double total = pretUnitar * numarBilete;

            // Poți formata, de ex., cu două zecimale:
            LabelPret.Content = $"Preț: {total:0.00} lei";
        }


        // Când utilizatorul apasă „Cumpără”
        private void BtnCumpara_Click(object sender, RoutedEventArgs e)
        {
            if (_cursaSelectata == null)
            {
                MessageBox.Show("Te rog selectează o cursă.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int numarBilete = NumericUpDownControl.Value ?? 0;
            if (numarBilete <= 0)
            {
                MessageBox.Show("Te rog alege cel puțin un bilet.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime dataRezervare = DateTime.Today;

            // 1. Preluăm toate locurile deja ocupate pentru cursa selectată
            var locuriOcupate = new HashSet<int>();
            using (var dbConn = new DataBaseConnection())
            {
                
                string sqlLocuri = @"
            SELECT Nr_loc
              FROM Bilet
             WHERE IDCursa = @IdCursa
        ";
                using (var cmdLoc = new SqlCommand(sqlLocuri, dbConn.Connection))
                {
                    cmdLoc.Parameters.AddWithValue("@IdCursa", _cursaSelectata.IDCursa);
                    using (var reader = cmdLoc.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            locuriOcupate.Add(reader.GetInt32(0));
                        }
                    }
                }
            }

            // 2. Găsim primele 'numarBilete' seat-uri libere
            var locuriDeAlocat = new List<int>();
            int candidateSeat = 1;
            while (locuriDeAlocat.Count < numarBilete)
            {
                if (!locuriOcupate.Contains(candidateSeat))
                {
                    locuriDeAlocat.Add(candidateSeat);
                    locuriOcupate.Add(candidateSeat);
                }
                candidateSeat++;
            }

            // 3. Calculăm prețul unitar și total
            const double PretPeKm = 0.35;
            double pretUnitar = _cursaSelectata.Km * PretPeKm;
            double totalComanda = pretUnitar * numarBilete;

            // 4. Construim mesajul de confirmare
            //    Îl listăm așa: Cursa, Data/Ora, Număr bilete, Locuri, Preț unitar și Total
            var sb = new StringBuilder();
            sb.AppendLine("Detalii comandă:");
            sb.AppendLine($" • Cursa: {_cursaSelectata.DenumireAfisaj}");
            sb.AppendLine($" • Nume cumpărător: {_utilizatorModel.Nume}");
            sb.AppendLine($" • Prenume cumpărător: {_utilizatorModel.Prenume}");
            sb.AppendLine($" • Data rezervării: {dataRezervare:dd.MM.yyyy}");
            sb.AppendLine($" • Număr bilete: {numarBilete}");
            sb.AppendLine($" • Locuri alocate: {string.Join(", ", locuriDeAlocat)}");
            sb.AppendLine($" • Preț unitar (per bilet): {pretUnitar:0.00} lei");
            sb.AppendLine($" • Total de plată: {totalComanda:0.00} lei");
            sb.AppendLine();
            sb.Append("Ești sigur că vrei să confirmi comanda?");

            // 5. Afișăm un MessageBox cu Da/Nu
            var rezultat = MessageBox.Show(
                sb.ToString(),
                "Confirmă comanda biletelor",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (rezultat != MessageBoxResult.Yes)
            {
                // Dacă nu confirmă, ieșim fără să inserăm nimic
                NumericUpDownControl.Value = 1;
                ComboAngajati.SelectedIndex = 0;
                LabelPret.Content = "Preț: 0 lei";
                return;
            }

            // 6. Dacă utilizatorul a răspuns „Da”, efectuăm inserările
            try
            {
                using (var dbConn = new DataBaseConnection())
                {
                    

                    foreach (int locNou in locuriDeAlocat)
                    {
                        using (var cmd = new SqlCommand("sp_InsertBilet", dbConn.Connection))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@IdCursa", _cursaSelectata.IDCursa);
                            cmd.Parameters.AddWithValue("@IDPasager", _utilizatorModel.IdPersoana);
                            cmd.Parameters.AddWithValue("@Nr_loc", locNou);
                            cmd.Parameters.AddWithValue("@Data_Rezervare", dataRezervare);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                MessageBox.Show(
                    $"Ai cumpărat {numarBilete} bilete cu succes! Total plătit: {totalComanda:0.00} lei",
                    "Succes",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                // 1. Construiește antetul RTF cu tab stop la 720 twips (~0.5 inch)
                string continutRTF = @"{\rtf1\ansi\ansicpg1250\uc1\deftab720
\pard\qc\b Chitant\u259? Rezervare Bilete\b0\par
\pard\par

\pard\b Detalii pasager:\b0\par
\u8226? Nume complet: " + EscapeUnicode(_utilizatorModel.NumeComplet) + @"\par
\u8226? ID pasager: " + _utilizatorModel.IdPersoana + @"\par
\pard\par

\pard\b Detalii curs\u259?:\b0\par
\u8226? Cursa: " + EscapeUnicode(_cursaSelectata.DenumireAfisaj) + @"\par
\u8226? Kilometraj: " + _cursaSelectata.Km.ToString("0.00") + @" km\par
\u8226? Data rezerv\u259?rii: " + dataRezervare.ToString("dd.MM.yyyy") + @"\par
\pard\par

\pard\b Detalii bilete:\b0\par
 Loc\tab Pret unitar\tab Subtotal\par
";

                // 2. Adaugă rândurile de tabel cu locuriDeAlocat
                foreach (int loc in locuriDeAlocat)
                {
                    continutRTF += " " + loc + @"\tab "
                                   + pretUnitar.ToString("0.00") + @" lei\tab "
                                   + pretUnitar.ToString("0.00") + @" lei\par
";
                }

                continutRTF += @"
\pard\b Total de plat\u259?:\b0 " + totalComanda.ToString("0.00") + @" lei\par
\pard\par

\pard Emis la: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm") + @"\par
}";

                // 3. Salvează și deschide fișierul RTF
                string tempFolder = Path.GetTempPath();
                string fileName = $"Chitanta_{_utilizatorModel.IdPersoana}_{DateTime.Now:yyyyMMdd_HHmmss}.rtf";
                string filePath = Path.Combine(tempFolder, fileName);
                File.WriteAllText(filePath, continutRTF, Encoding.ASCII);

                Process.Start(new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });


                // 7. Resetăm interfața
                NumericUpDownControl.Value = 1;
                ComboAngajati.SelectedIndex = 0;
                LabelPret.Content = "Preț: 0 lei";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Eroare la cumpărarea biletelor: {ex.Message}",
                    "Eroare",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private string EscapeUnicode(string input)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in input)
            {
                if (c > 127)
                {
                    // transformăm caracterul în \uNNNN?
                    sb.Append(@"\u").Append(Convert.ToUInt16(c)).Append('?');
                }
                else
                {
                    // caracter ASCII simplu
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }




        private void BtnAnuleaza_Click(object sender, RoutedEventArgs e)
        {
            // Pur și simplu resetezi interfața, după cum dorești
            NumericUpDownControl.Value = 1;
            ComboAngajati.SelectedIndex = 0;
            LabelPret.Content = "Preț: 0 lei";
        }

      

        private void Increase_Click(object sender, RoutedEventArgs e)
        {
            NumericUpDownControl.Value += 1;
        }

        private void Decrease_Click(object sender, RoutedEventArgs e)
        {
            if (NumericUpDownControl.Value == 1)
            {
                NumericUpDownControl.Value = 1;
            }
            else NumericUpDownControl.Value -= 1;
        }

    }
}
