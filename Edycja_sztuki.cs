using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Media;
using MySql.Data.MySqlClient;

namespace Etykiety_stabilizacja
{
    public partial class Edycja_sztuki : Form
    {
        public Edycja_sztuki()
        {
            InitializeComponent();
        }
        public Edycja_sztuki(string ID_stabilizacja, string[,] tab_wzory, bool usuwanie_wiersza, string data_parti)
        {
            InitializeComponent();
            str_ID_stabilizacja = ID_stabilizacja;
            str_data_parti = data_parti;
            flaga_usuwanie_wiersza = usuwanie_wiersza;

            dostep_do_bazy.dane_poloczenia();

            polocz = new MySql.Data.MySqlClient.MySqlConnection("server=" + dostep_do_bazy.str_IP_serwer + "; Port=3306; uid=" + dostep_do_bazy.str_Login + "; pwd=" + dostep_do_bazy.str_Haslo + "; database=" + dostep_do_bazy.str_Baza_danych + "");


            polocz_MSSQL.ConnectionString = "Data Source='" + dostep_do_bazy.str_IP_serwer_MSSQL + "'; Initial Catalog='" + dostep_do_bazy.str_Baza_danych_MSSQL + "'; User id='" + dostep_do_bazy.str_Login_MSSQL + "'; Password='" + dostep_do_bazy.str_Haslo_MSSQL + "';";

            polocz_dziewiarnia = new MySql.Data.MySqlClient.MySqlConnection("server=" + dostep_do_bazy.str_IP_serwer_dziewiania + "; Port=3306; uid=" + dostep_do_bazy.str_Login_dziewiarnia + "; pwd=" + dostep_do_bazy.str_Haslo_dziewiarnia + "; database=" + dostep_do_bazy.str_Baza_danych_dziewiarnia + "");

            polocz_drukarnia_baza = new MySql.Data.MySqlClient.MySqlConnection("server=" + dostep_do_bazy.str_IP_serwer_drukarnia_baza + "; Port=3306; uid=" + dostep_do_bazy.str_Login_drukarnia_baza + "; pwd=" + dostep_do_bazy.str_Haslo_drukarnia_baza + "; database=" + dostep_do_bazy.str_Baza_drukarnia_baza + "");

            polocz_zamowienia_drukarnia = new MySql.Data.MySqlClient.MySqlConnection("server=" + dostep_do_bazy.str_IP_serwer_zamowienia_drukarnia + "; Port=3306; uid=" + dostep_do_bazy.str_Login_zamowienia_druakarnia + "; pwd=" + dostep_do_bazy.str_Haslo_zamowienia_drukarnia + "; database=" + dostep_do_bazy.str_Baza_zamowienia_drukarnia + "");

            for(int i=0; i<15; i++)
            {
                if (!tab_wzory[i, 0].Equals(""))
                {
                    
                    tab_wzory_edycja[i] = tab_wzory[i, 0];
                    tab_klient_edycja[i] = tab_wzory[i, 1];
                }
                else
                {
                    tab_wzory_edycja[i] = "";
                    tab_klient_edycja[i] = "";
                }
                
            }
        }

        public MySql.Data.MySqlClient.MySqlConnection polocz;
        public MySql.Data.MySqlClient.MySqlConnection polocz_dziewiarnia;
        public MySql.Data.MySqlClient.MySqlConnection polocz_drukarnia_baza;
        public MySql.Data.MySqlClient.MySqlConnection polocz_zamowienia_drukarnia;
        private SqlConnection polocz_MSSQL = new SqlConnection();
        Dostep_do_bazy dostep_do_bazy = new Dostep_do_bazy();

        string str_ID_stabilizacja;
        string str_data_parti;
        bool flaga_usuwanie_wiersza;
        public bool flaga_drukuj_etykiete = false;

        string[] tab_wzory_edycja = new string[15];
        string[] tab_klient_edycja = new string[15];

        private void Edycja_sztuki_Load(object sender, EventArgs e)
        {
            try
            {
                dataGridView_waga.RowCount = 1;

                if (flaga_usuwanie_wiersza == true)
                {
                    button_usun.Enabled = true;
                }
                if (flaga_usuwanie_wiersza == false)
                {
                    button_usun.Enabled = false;
                }

                flaga_drukuj_etykiete = false;

                wypelni_comboBox();
                wypelni_wartosci();
            }catch(Exception){

            }

        }

        

        public void wypelni_comboBox()
        {
            comboBox_wzor.Items.AddRange(tab_wzory_edycja);

            
        }

        public void wypelni_wartosci()
        {
            try
            {
                polocz.Open();
                //MySql.Data.MySqlClient.MySqlCommand szukaj_parti_do_drukku = new MySql.Data.MySqlClient.MySqlCommand("SELECT ID_stabilizacja,  Nr_sztuki, Metry, Kilogramy, Uwagi, Wzor, Klient FROM ETYKIETY_STABILIZACJA_TAB WHERE Nr_parti = \'" + textBox_nr_parti.Text.ToString() + "\' AND Artykul = \'" + label_artykul_baza.Text.ToString() + "\';", polocz);
                MySql.Data.MySqlClient.MySqlCommand nr_parti = new MySql.Data.MySqlClient.MySqlCommand("SELECT  Nr_parti FROM ETYKIETY_STABILIZACJA_TAB WHERE ID_stabilizacja = \'" + str_ID_stabilizacja + "\';", polocz);
                label_nr_parti_baza.Text = nr_parti.ExecuteScalar().ToString();

                MySql.Data.MySqlClient.MySqlCommand nr_sztuki = new MySql.Data.MySqlClient.MySqlCommand("SELECT  Nr_sztuki FROM ETYKIETY_STABILIZACJA_TAB WHERE ID_stabilizacja = \'" + str_ID_stabilizacja + "\';", polocz);
                textBox_nr_sztuki.Text = nr_sztuki.ExecuteScalar().ToString();

                MySql.Data.MySqlClient.MySqlCommand metry = new MySql.Data.MySqlClient.MySqlCommand("SELECT  Metry FROM ETYKIETY_STABILIZACJA_TAB WHERE ID_stabilizacja = \'" + str_ID_stabilizacja + "\';", polocz);
                textBox_metry.Text = metry.ExecuteScalar().ToString();

                MySql.Data.MySqlClient.MySqlCommand waga = new MySql.Data.MySqlClient.MySqlCommand("SELECT  Kilogramy FROM ETYKIETY_STABILIZACJA_TAB WHERE ID_stabilizacja = \'" + str_ID_stabilizacja + "\';", polocz);
                double d_temp = Convert.ToDouble(waga.ExecuteScalar().ToString());

                dataGridView_waga.Rows[0].Cells[0].Value = d_temp;

                MySql.Data.MySqlClient.MySqlCommand uwagi = new MySql.Data.MySqlClient.MySqlCommand("SELECT Uwagi FROM ETYKIETY_STABILIZACJA_TAB WHERE ID_stabilizacja = \'" + str_ID_stabilizacja + "\';", polocz);
                textBox_uwagi.Text = uwagi.ExecuteScalar().ToString();
               

                MySql.Data.MySqlClient.MySqlCommand wzor = new MySql.Data.MySqlClient.MySqlCommand("SELECT Wzor FROM ETYKIETY_STABILIZACJA_TAB WHERE ID_stabilizacja = \'" + str_ID_stabilizacja + "\';", polocz);
                string str_wzor = wzor.ExecuteScalar().ToString();
                comboBox_wzor.Items.Add(str_wzor);
                comboBox_wzor.Text = str_wzor;

                polocz.Close();
            }catch(Exception){
                polocz.Close();
            }
        }

        private void button_zamknij_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button_zapisz_Click(object sender, EventArgs e)
        {
            try
            {
                SoundPlayer klik_drukuj = new SoundPlayer(@"button20.wav");
                klik_drukuj.Play();
                zapisz();
                this.Close();
            }catch(Exception)
            {

            }
        }
        public void zapisz()
        {
            try
            {
                string str_klient_do_edycji = "";
                string str_uwagi_do_zapisania = "";
                for (int i = 0; i < 15; i++)
                {
                    if (tab_wzory_edycja[i].Equals(comboBox_wzor.Text.ToString()))
                    {
                        str_klient_do_edycji = tab_klient_edycja[i];
                    }
                }

                if(textBox_uwagi.Text.ToString().Equals(""))
                {
                    str_uwagi_do_zapisania = "BEZ UWAG";
                }
                else
                {
                    str_uwagi_do_zapisania = textBox_uwagi.Text.ToString();
                }

                polocz.Open();

                //MySql.Data.MySqlClient.MySqlCommand edytuj = new MySql.Data.MySqlClient.MySqlCommand("UPDATE `ETYKIETY_STABILIZACJA_TAB` SET `Nr_sztuki`= \'" + textBox_nr_sztuki.Text.ToString() + "\', `Metry`= \'" + textBox_metry.Text.ToString() + "\',`Kilogramy`=\'" + dataGridView_waga.Rows[0].Cells[0].Value.ToString().Replace(",", ".") + "\',`Uwagi`=\'" + str_uwagi_do_zapisania + "\',`Wzor`=\'" + comboBox_wzor.Text.ToString() + "\',`Klient`=\'" + str_klient_do_edycji + "\' WHERE ID_stabilizacja = \'" + str_ID_stabilizacja + "\';", polocz);
                MySql.Data.MySqlClient.MySqlCommand edytuj = new MySql.Data.MySqlClient.MySqlCommand("UPDATE `ETYKIETY_STABILIZACJA_TAB` SET `Nr_sztuki`= \'" + textBox_nr_sztuki.Text.ToString() + "\', `Metry`= \'" + textBox_metry.Text.ToString() + "\',`Kilogramy`=\'" + dataGridView_waga.Rows[0].Cells[0].Value.ToString().Replace(",", ".") + "\',`Uwagi`=\'" + str_uwagi_do_zapisania + "\',`Wzor`=\'" + comboBox_wzor.Text.ToString() + "\' WHERE ID_stabilizacja = \'" + str_ID_stabilizacja + "\';", polocz);
               
                
                edytuj.ExecuteNonQuery();

                polocz.Close();
            }catch(Exception){
                polocz.Close();
            }



        }

        private void button_usun_Click(object sender, EventArgs e)
        {
            try
            {
                SoundPlayer klik_drukuj = new SoundPlayer(@"button20.wav");
                klik_drukuj.Play();

                if(textBox_nr_sztuki.Text.ToString().Equals("1"))
                {
                    zmien_status_na_wydrukowane();
                }

                polocz.Open();

                MySql.Data.MySqlClient.MySqlCommand edytuj = new MySql.Data.MySqlClient.MySqlCommand("DELETE FROM `ETYKIETY_STABILIZACJA_TAB` WHERE ID_stabilizacja = \'" + str_ID_stabilizacja + "\';", polocz);
                edytuj.ExecuteNonQuery();

                polocz.Close();

                this.Close();
            }catch(Exception){
                polocz.Close();
            }

        }

        public void zmien_status_na_wydrukowane()
        {
            string str_sprawdz_nr_karta = "";
            try
            {
                polocz.Open();

                MySql.Data.MySqlClient.MySqlCommand sprawdz_Nr_karta = new MySql.Data.MySqlClient.MySqlCommand("SELECT Nr_karta FROM ETYKIETY_STABILIZACJA_TAB WHERE ID_stabilizacja = \'" + str_ID_stabilizacja + "\';", polocz);
                str_sprawdz_nr_karta = sprawdz_Nr_karta.ExecuteScalar().ToString();

                polocz.Close();
            }
            catch (Exception)
            {
                polocz.Close();
                str_sprawdz_nr_karta = "";
            }

            string str_sprawdz_klient = "";
            try
            {
                polocz.Open();

                MySql.Data.MySqlClient.MySqlCommand sprawdz_klient = new MySql.Data.MySqlClient.MySqlCommand("SELECT Klient FROM KARTA_TAB WHERE ID_Karta_nr = \'" + str_sprawdz_nr_karta + "\';", polocz);
                str_sprawdz_klient = sprawdz_klient.ExecuteScalar().ToString();

                polocz.Close();
            }
            catch (Exception)
            {
                polocz.Close();
                str_sprawdz_nr_karta = "";
            }


            ////////////////////zmiana statusu w systemie drukarnia zeszyt/////////////////////////////
            if (str_sprawdz_klient.Equals("PAKAITA"))
            {
                try
                {
                    polocz_drukarnia_baza.Open();

                    MySql.Data.MySqlClient.MySqlCommand zmien_status = new MySql.Data.MySqlClient.MySqlCommand("UPDATE DRUKARNIA_TAB SET DRUKARNIA_TAB.ID_Status_drukarnia = \'4\' WHERE DRUKARNIA_TAB.Nr_parti = \'" + label_nr_parti_baza.Text.ToString() + "\' AND DRUKARNIA_TAB.Data = \'" + str_data_parti + "\' AND DRUKARNIA_TAB.ID_Klient_drukarnia = \'10\';", polocz_drukarnia_baza);
                    zmien_status.ExecuteNonQuery();

                    polocz_drukarnia_baza.Close();
                }
                catch (Exception)
                {
                    polocz_drukarnia_baza.Close();
                }
            }

            if (str_sprawdz_nr_karta.Equals("0"))
            {
                try
                {
                    polocz_drukarnia_baza.Open();

                    MySql.Data.MySqlClient.MySqlCommand zmien_status = new MySql.Data.MySqlClient.MySqlCommand("UPDATE DRUKARNIA_TAB SET DRUKARNIA_TAB.ID_Status_drukarnia = \'4\' WHERE DRUKARNIA_TAB.Nr_parti = \'" + label_nr_parti_baza.Text.ToString() + "\' AND DRUKARNIA_TAB.Data = \'" + str_data_parti + "\';", polocz_drukarnia_baza);
                    zmien_status.ExecuteNonQuery();

                    polocz_drukarnia_baza.Close();
                }
                catch (Exception)
                {
                    polocz_drukarnia_baza.Close();
                }
            }

            
            if (!str_sprawdz_nr_karta.Equals("0"))
            {

                ////////////////////zmiana statusu w systemie An-Farb///////////////////////////////////
                if (str_sprawdz_klient.Equals("TRANSTEX"))
                {
                    try
                    {
                        polocz_MSSQL.Open();

                        SqlCommand zmien_status_MSSQL = new SqlCommand("update dbo.karty set dbo.karty.stan_zlecenia = '3' where dbo.karty.karta_nr = \'" + str_sprawdz_nr_karta + "\' AND dbo.karty.stan_zlecenia != '5' AND dbo.karty.stan_zlecenia != '4';", polocz_MSSQL);
                        zmien_status_MSSQL.ExecuteNonQuery();


                        polocz_MSSQL.Close();
                    }
                    catch (Exception)
                    {
                        polocz_MSSQL.Close();
                    }

                }
                else
                {
                    try
                    {
                        polocz_MSSQL.Open();

                        SqlCommand zmien_status_MSSQL = new SqlCommand("update dbo.karty set dbo.karty.stan_zlecenia = '3' where dbo.karty.karta_nr = \'" + str_sprawdz_nr_karta + "\' AND dbo.karty.stan_zlecenia != '5' AND dbo.karty.stan_zlecenia != '8' AND dbo.karty.stan_zlecenia != '4';", polocz_MSSQL);
                        zmien_status_MSSQL.ExecuteNonQuery();


                        polocz_MSSQL.Close();
                    }
                    catch (Exception)
                    {
                        polocz_MSSQL.Close();
                    }

                }
                

                /////////////////zmiana PRODUKCJA STAB TAB////////////////////////////////////////////////

                string str_nr_stabilizacji = "";

                StreamReader odczytu_nr_stab = new StreamReader("ustawienia_stab.txt");
                str_nr_stabilizacji = odczytu_nr_stab.ReadLine();
                odczytu_nr_stab.Close();

                //string str_sprawdz_stabilizacje = "";

                try
                {
                    polocz.Open();


                    MySqlCommand usun_czy_byla_stabilizacja = new MySqlCommand("DELETE FROM `PRODUKCJA_STAB_TAB` WHERE `ID_Karta_nr`=\'" + str_sprawdz_nr_karta + "\' AND `ID_stab` = \'" + str_nr_stabilizacji + "\';", polocz);
                    usun_czy_byla_stabilizacja.ExecuteNonQuery();

                    polocz.Close();

                }
                catch (Exception)
                {
                    //str_sprawdz_stabilizacje = "";
                    polocz.Close();
                }

            }

            

        }

        private void textBox_metry_Click(object sender, EventArgs e)
        {
            try
            {
                Klawiatura_numeryczna klawiatura_numeryczna = new Klawiatura_numeryczna(true,false, false);
                klawiatura_numeryczna.ShowDialog();

                if (klawiatura_numeryczna.flaga_drukuj == true && (!klawiatura_numeryczna.textBox__wartosc_cyfrowa.Text.ToString().Equals("")))
                {
                    textBox_metry.Text = klawiatura_numeryczna.textBox__wartosc_cyfrowa.Text.ToString();
                }
                else
                {
                    if (klawiatura_numeryczna.flaga_drukuj == true)
                    {
                        textBox_metry.Text = "0";
                    }
                }
            }catch(Exception){

            }
        }

        private void button_drukuj_Click(object sender, EventArgs e)
        {
            try
            {
                SoundPlayer klik_drukuj = new SoundPlayer(@"button20.wav");
                klik_drukuj.Play();
                zapisz();
                flaga_drukuj_etykiete = true;
                this.Close();
            }catch(Exception){

            }


        }

        

        private void textBox_nr_sztuki_Click(object sender, EventArgs e)
        {
            try
            {
                Klawiatura_numeryczna klawiatura_numeryczna = new Klawiatura_numeryczna(false, false, true);
                klawiatura_numeryczna.ShowDialog();

                if (klawiatura_numeryczna.flaga_drukuj == true && (!klawiatura_numeryczna.textBox_nr_sztuki.Text.ToString().Equals("")))
                {
                    textBox_nr_sztuki.Text = klawiatura_numeryczna.textBox_nr_sztuki.Text.ToString();
                }
                else
                {
                    if (klawiatura_numeryczna.flaga_drukuj == true)
                    {
                        textBox_nr_sztuki.Text = "0";
                    }
                }
            }catch(Exception){

            }

        }

        private void textBox_uwagi_Click(object sender, EventArgs e)
        {
            try
            {
                string str_uwagi = "";
                bool flaga_edycja_uwagi = true;

                if (textBox_uwagi.Text.ToString().Equals("BEZ UWAG"))
                {
                    str_uwagi = "";
                }
                else
                {
                    str_uwagi = textBox_uwagi.Text.ToString();
                }

                Uwagi_do_sztuki uwagi_do_sztuki = new Uwagi_do_sztuki(flaga_edycja_uwagi, str_uwagi);
                uwagi_do_sztuki.ShowDialog();

                textBox_uwagi.Text = uwagi_do_sztuki.richTextBox1.Text.ToString();
            }catch(Exception ex){
                MessageBox.Show("Nie można otworzyć uwag\n" + ex + "", "UWAGA !!", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void dataGridView_waga_Click(object sender, EventArgs e)
        {
            try
            {
                Klawiatura_numeryczna klawiatura_numeryczna = new Klawiatura_numeryczna(false, true, false);
                klawiatura_numeryczna.ShowDialog();

                if (klawiatura_numeryczna.flaga_drukuj == true && (!klawiatura_numeryczna.textBox_kg.Text.ToString().Equals("")))
                {
                    //textBox_metry.Text = klawiatura_numeryczna.textBox__wartosc_cyfrowa.Text.ToString();
                    dataGridView_waga.Rows[0].Cells[0].Value = klawiatura_numeryczna.textBox_kg.Text.ToString();
                }
                else
                {
                    if (klawiatura_numeryczna.flaga_drukuj == true)
                    {
                        dataGridView_waga.Rows[0].Cells[0].Value = "";
                    }
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
