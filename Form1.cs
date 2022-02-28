using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Media;
using Microsoft;
using System.Data.OleDb;
using System.Threading;
using System.Diagnostics;
using System.Runtime;

namespace Etykiety_stabilizacja
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            dostep_do_bazy.dane_poloczenia();

            polocz = new MySql.Data.MySqlClient.MySqlConnection("server=" + dostep_do_bazy.str_IP_serwer + "; Port=3306; uid=" + dostep_do_bazy.str_Login + "; pwd=" + dostep_do_bazy.str_Haslo + "; database=" + dostep_do_bazy.str_Baza_danych + "");


            polocz_MSSQL.ConnectionString = "Data Source='" + dostep_do_bazy.str_IP_serwer_MSSQL + "'; Initial Catalog='" + dostep_do_bazy.str_Baza_danych_MSSQL + "'; User id='" + dostep_do_bazy.str_Login_MSSQL + "'; Password='" + dostep_do_bazy.str_Haslo_MSSQL + "';";

            polocz_dziewiarnia = new MySql.Data.MySqlClient.MySqlConnection("server=" + dostep_do_bazy.str_IP_serwer_dziewiania + "; Port=3306; uid=" + dostep_do_bazy.str_Login_dziewiarnia + "; pwd=" + dostep_do_bazy.str_Haslo_dziewiarnia + "; database=" + dostep_do_bazy.str_Baza_danych_dziewiarnia + "");

            polocz_drukarnia_baza = new MySql.Data.MySqlClient.MySqlConnection("server=" + dostep_do_bazy.str_IP_serwer_drukarnia_baza + "; Port=3306; uid=" + dostep_do_bazy.str_Login_drukarnia_baza + "; pwd=" + dostep_do_bazy.str_Haslo_drukarnia_baza + "; database=" + dostep_do_bazy.str_Baza_drukarnia_baza + "");

            polocz_zamowienia_drukarnia = new MySql.Data.MySqlClient.MySqlConnection("server=" + dostep_do_bazy.str_IP_serwer_zamowienia_drukarnia + "; Port=3306; uid=" + dostep_do_bazy.str_Login_zamowienia_druakarnia + "; pwd=" + dostep_do_bazy.str_Haslo_zamowienia_drukarnia + "; database=" + dostep_do_bazy.str_Baza_zamowienia_drukarnia + "");



        }

        public MySql.Data.MySqlClient.MySqlConnection polocz;
        public MySql.Data.MySqlClient.MySqlConnection polocz_dziewiarnia;
        public MySql.Data.MySqlClient.MySqlConnection polocz_drukarnia_baza;
        public MySql.Data.MySqlClient.MySqlConnection polocz_zamowienia_drukarnia;
        private SqlConnection polocz_MSSQL = new SqlConnection();
        Dostep_do_bazy dostep_do_bazy = new Dostep_do_bazy();

        string str_sprawdz_nr_karta  = "0";

        string str_metry_do_druku;
        string str_metry_do_druku_edycja;
        string str_ktora_sztuka_do_druku;
        string str_klient_do_druku;
        string str_klient_do_zapisu;
        string str_kolor_do_druku;
        string str_nr_koloru_an_farb_do_druku;
        string str_nr_wzoru_na_etykiete;
        string str_data_parti;
        string str_klient_do_druku_etykieta;
        string str_kilogramy_do_druku;
        string str_klient_do_druku_edycja;
        string str_metry_temp_do_sprawdzenia_zmiany;
        string str_metry_do_druku_temp = "0";

        string str_sklad_dzianiny;
        string str_nr_parti_do_zapisania;

        string str_stan_licznika;
        string str_artykul_parti_barwionej;
        string str_nr_koloru_Torusnet;

        string[,] tab_wzory = new string[15,2];

       // string path_pomiar_metry = @"pomiary.csv";
       // string path_metry_txt = @"pomiary.txt";

        string str_port_Name = "";

        string str_gramatura_do_druku;
        string str_szerokosc_do_druku;

        bool flaga_wzory;
        bool flaga_kupony;
        bool flaga_uwagi;
        bool flaga_dwa_wzory;
        bool flaga_zapamietaj_wzor;
        bool flaga_cena;
        bool flaga_zapisz_metry_do_druku;
        bool flaga_edycja_metry_do_druku;
        bool flaga_drukowanie = true;

        int int_procent_zero;
        int int_procent_metry_dla_artykul = 0;
        int int_procent_metry_dla_klient = 0;
        int int_ktora_sztuka = 0;
        int int_klikniety_wiersz_do_zaznaczenia_wzoru;

        double d_kilogramy;


        Thread uruchom_automatycznie;


        private void Form1_Load(object sender, EventArgs e)
        {
            label_nr_wzoru_na_etykiete.Text = "";
            label_kupony_klient.Text = "";
            label_artykul_baza.Text = "";
            str_stan_licznika = "0";
            textBox_odczyt_metrow.Text = str_stan_licznika;

            button_wzory.BackColor = Color.Green;
            button_wzory.ForeColor = Color.Black;
            flaga_wzory = true;
            flaga_kupony = false;
            flaga_zapamietaj_wzor = true;

            button_zapamietaj_wzor.Visible = false;
            button_zapamietaj_wzor_ok.Visible = true;
            button_zwin.Visible = false;

            groupBox_karta_obieg.Visible = false;

            button_lista_zapamietanych_parti_gora.Visible = false;
            dataGridView_rozwijana_lista_parti.Visible = false;
            dataGridView_rozwijana_lista_parti.RowCount = 8;

            for (int i = 0; i < 7; i++)
            {
                dataGridView_rozwijana_lista_parti.Rows[i].Cells[0].Value = "";
                
            }
            

            //czysc_pomiar_metry();
            czysc_karte_obiegowa();
            wypelni_tab_wzory();

            ///////////////watek pobierania metrow ////////////////////////

            timer_sprawdz_port.Tick += new EventHandler(uruchamianie_watku);
            timer_sprawdz_port.Interval = 500;
            timer_sprawdz_port.Start();

            //////jesli port jest zamkniety to go otworz///////////
            if(!serialPort1.IsOpen)
            {
                try
                {
                    
                    otwieranie_serialPort();
                }
                catch (Exception)
                {
                    
                }
            }
            if (!serialPort1.IsOpen)
            {
                MessageBox.Show("Nie można połączyc z licznikiem.\n Pomiar nie będzie mozliwy !!!", "Uwaga !!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
            
            

        }

        public void otwieranie_serialPort()
        {
            try
            {
                odczyt_ustawien();
                serialPort1.PortName = str_port_Name;
                serialPort1.Open();
                // sprawdz_port();
            }
            catch (Exception)
            {
                //MessageBox.Show("Nie można połączyc z licznikiem.\n Pomiar nie będzie mozliwy !!!", "Uwaga !!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        public void odczyt_ustawien()
        {
            StreamReader odczyt_plik_ustawienia = new StreamReader("ustawienia_serial_port.txt");

            str_port_Name = Convert.ToString(odczyt_plik_ustawienia.ReadLine());
            /*
            comboBox_Baud_Rate.Text = Convert.ToString(odczyt_plik_ustawienia.ReadLine());
            comboBox_Data_Bits.Text = Convert.ToString(odczyt_plik_ustawienia.ReadLine());
            comboBox_Parity.Text = Convert.ToString(odczyt_plik_ustawienia.ReadLine());
            comboBox_Stop_Bits.Text = Convert.ToString(odczyt_plik_ustawienia.ReadLine());
             */

            odczyt_plik_ustawienia.Close();
        }

        public void wypelni_tab_wzory()
        {
            for (int i = 0; i < 15; i++)
            {
                tab_wzory[i, 0] = "";
                tab_wzory[i, 1] = "";
            }
        }

        public void sprawdz_procent_metry_dla_artykul()
        {
            string str_artykul;
            int_procent_metry_dla_artykul = 0;
           
            str_artykul = label_artykul_baza.Text.ToString();

            try
            {
                polocz.Open();

                MySqlCommand pobierz_procent_artykul = new MySqlCommand("SELECT procent_artykul FROM PRZELICZNIK_METRY_TAB WHERE Artykul = \'" + str_artykul + "\';", polocz);
                int_procent_metry_dla_artykul = Convert.ToInt32(pobierz_procent_artykul.ExecuteScalar().ToString());

                polocz.Close();
            }
            catch (Exception)
            {
                polocz.Close();
                int_procent_metry_dla_artykul = 0;
            }

        }
        public void sprawdz_procent_metry_dla_klient()
        {
            int int_pozycja = 0;
            string str_klient = "";

            int_procent_metry_dla_klient = 0;
            try
            {
                int_pozycja = str_klient_do_druku.IndexOf(" ");

                str_klient = str_klient_do_druku.Substring(0, int_pozycja);
            }
            catch (Exception) { }


            try
            {
                polocz.Open();

                MySqlCommand pobierz_procent_klient = new MySqlCommand("SELECT procent_klient FROM PRZELICZNIK_METRY_TAB WHERE Klient = \'" + str_klient + "\';", polocz);
                int_procent_metry_dla_klient = Convert.ToInt32(pobierz_procent_klient.ExecuteScalar().ToString());

                polocz.Close();
            }
            catch (Exception)
            {
                polocz.Close();
                int_procent_metry_dla_klient = 0;
            }

        }

        public string policz_metry(string metry)
        {
            try
            {
                int int_metry_przeliczone_zero = 0;
                int int_metry_przeliczone_klient = 0;
                int int_metry_przeliczone_artykul = 0;
               // int int_procent_klient = 0;
               // int int_procent_artykul = 0;
                
                int int_metry = 0;


                double d_suma_procent = 0.0;
                double d_metry = Convert.ToDouble(metry);
                double d_roznica = 0.0;
                

                
                

                ////////////////obliczanie punktu zero/////////////////
                //pobierz_zero();

                d_suma_procent = (Convert.ToDouble(int_procent_zero) / 100) * d_metry;
                d_roznica = d_metry + d_suma_procent;
                int_metry_przeliczone_zero = Convert.ToInt32(d_roznica);
                ///////////////////////////////////////////////////////

                ///////////////obliczanie procent klient//////////////////////
                
                /////////przeliczanie metrow wg procentów dla klienta /////////////////////////
                
                d_suma_procent = (Convert.ToDouble(int_procent_metry_dla_klient) / 100) * Convert.ToDouble(int_metry_przeliczone_zero);
                d_roznica = Convert.ToDouble(int_metry_przeliczone_zero) + d_suma_procent;
                int_metry = Convert.ToInt32(Math.Round(d_roznica, MidpointRounding.AwayFromZero));
                int_metry_przeliczone_klient = Convert.ToInt32(d_roznica);
                 

                ////////////////////////////////////////////////////////////////

                ////////////przeliczanie metrow wg procentów dla artykulu/////////////////////////

                d_suma_procent = (Convert.ToDouble(int_procent_metry_dla_artykul) / 100) * Convert.ToDouble(int_metry_przeliczone_klient);
                d_roznica = Convert.ToDouble(int_metry_przeliczone_klient) + d_suma_procent;
                int_metry = Convert.ToInt32(Math.Round(d_roznica, MidpointRounding.AwayFromZero));
                //int_metry_przeliczone_artykul = Convert.ToInt32(d_roznica);
                int_metry_przeliczone_artykul = int_metry;

                ////////////////////////////////////////////////////////////////

                
                
                metry = Convert.ToString(int_metry_przeliczone_artykul);
            }
            catch (Exception)
            {
                MessageBox.Show("Metry nie zostały policzone", "Uwaga !", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            str_metry_temp_do_sprawdzenia_zmiany = metry;

            return metry;

        }    

        private void textBox_nr_parti_Click(object sender, EventArgs e)
        {
            try
            {
                czysc_karte_obiegowa();
                dataGridView_wzory.RowCount = 1;
                dataGridView_wzory.RowCount = 15;
                dataGridView_wzory.Rows[0].Cells[0].Value = "";
                dataGridView_wzory.Rows[0].Cells[1].Value = "";
                dataGridView_wzory.Rows[0].Cells[2].Value = "";


                str_klient_do_druku_etykieta = "";

                Klawiatura_numeryczna klawiatura_numeryczna_nr_parti = new Klawiatura_numeryczna(true, false, false);
                klawiatura_numeryczna_nr_parti.ShowDialog();

                textBox_nr_parti.Text = klawiatura_numeryczna_nr_parti.textBox__wartosc_cyfrowa.Text.ToString();

                dopisz_parite_do_listy(textBox_nr_parti.Text.ToString());

                label_nr_wzoru_na_etykiete.Text = "";

                str_sklad_dzianiny = "";

                if (!(textBox_nr_parti.Text.ToString().Equals("")))
                {
                    szukaj_nr_parti();
                }

              //  pobierz_nr_parti_klient_data_artykul(0);
            }catch(Exception)
            {
                MessageBox.Show("Wystapił problem podczas wyszukiwania parti", "Uwaga !", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        public void dopisz_parite_do_listy(string nr_parti)
        {
            bool flaga_dopisz = true;
            string[] str_nr_parti_temp = new string[9];

            str_nr_parti_temp[0] = "";

            for (int i = 0; i < 7; i++ )
            {
                if(dataGridView_rozwijana_lista_parti.Rows[i].Cells[0].Value.ToString().Equals(nr_parti))
                {
                    flaga_dopisz = false;
                }
                str_nr_parti_temp[i+1] = dataGridView_rozwijana_lista_parti.Rows[i].Cells[0].Value.ToString();
            }
            if(flaga_dopisz)
            {
                for (int j = 0; j < 7; j++)
                {

                    if (j == 0)
                    {
                        dataGridView_rozwijana_lista_parti.Rows[j].Cells[0].Value = nr_parti;
                    }
                    else
                    {
                        dataGridView_rozwijana_lista_parti.Rows[j].Cells[0].Value = str_nr_parti_temp[j];
                    }
                    

                }
            
            }
        }
        public void szukaj_nr_parti()
        {
            DateTime pobierz_rok = DateTime.Now;
            string str_rok = pobierz_rok.Year.ToString("D2");

            try
            {

                dataSet_partie_temp.Clear();
                dataGridView_parite_temp.DataSource = dataSet_partie_temp;

                polocz_drukarnia_baza.Open();

                MySql.Data.MySqlClient.MySqlCommand szukaj_parti = new MySql.Data.MySqlClient.MySqlCommand("SELECT  DRUKARNIA_TAB.Nr_parti, KLIENT_TAB.Nazwa_Klient, DRUKARNIA_TAB.Data FROM KLIENT_TAB, DRUKARNIA_TAB WHERE DRUKARNIA_TAB.Nr_parti LIKE \'" + textBox_nr_parti.Text.ToString() + "%\' AND KLIENT_TAB.ID_Klient = DRUKARNIA_TAB.ID_Klient_drukarnia;", polocz_drukarnia_baza);
                MySqlDataAdapter adapter_kolory = new MySqlDataAdapter(szukaj_parti);
                adapter_kolory.Fill(dataSet_partie_temp, "Lista_parti");
                dataGridView_parite_temp.DataSource = dataSet_partie_temp.Tables["Lista_parti"];

                polocz_drukarnia_baza.Close();

            }
            catch (Exception e)
            {
                polocz_drukarnia_baza.Close();

                MessageBox.Show("Brak połączenia z bazą PRODUKCJA_FARBIARNIA\n"+e+"", "UWAGA !!", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            

            try
            {

                int int_ilosc_wierszy = dataGridView_parite_temp.RowCount;
                dataGridView_partie.RowCount = int_ilosc_wierszy;
                dataGridView_partie.Rows[0].Cells[0].Value = "Druk";
                for (int i = 1; i < int_ilosc_wierszy; i++)
                {
                    dataGridView_partie.Rows[i].DefaultCellStyle.BackColor = Color.White;
                    dataGridView_partie.Rows[i].Cells[0].Value = dataGridView_parite_temp.Rows[i-1].Cells[0].Value;
                    dataGridView_partie.Rows[i].Cells[1].Value = dataGridView_parite_temp.Rows[i-1].Cells[1].Value;
                    dataGridView_partie.Rows[i].Cells[2].Value = dataGridView_parite_temp.Rows[i-1].Cells[2].Value;

                    if (!dataGridView_partie.Rows[i].Cells[1].Value.ToString().Equals("PAKAITA"))
                    {
                        dataGridView_partie.Rows[i].Cells[3].Value = "0";
                    }

                    
                    if (dataGridView_partie.Rows[i].Cells[1].Value.ToString().Equals("PAKAITA"))
                    {
                        try
                        {
                            polocz.Open();

                            //MySql.Data.MySqlClient.MySqlCommand szukaj_parti = new MySql.Data.MySqlClient.MySqlCommand("SELECT  `Nr_partii`, `Klient`, `Numer_art`, `ID_Karta_nr`, `Data_przyjecia`  FROM `KARTA_TAB` WHERE `Nr_partii` LIKE \'" + textBox_nr_parti.Text.ToString() + "%\' AND `Numer_art` NOT LIKE \'%/D%\' AND `Numer_art` NOT LIKE \'%/T%\' AND (`Klient` = \'DAMAZ\' OR `Klient` = \'MARCZAK\')  AND `Data_przyjecia` LIKE \'" + str_rok + "%\';", polocz);
                            MySql.Data.MySqlClient.MySqlCommand Id_karta_nr = new MySql.Data.MySqlClient.MySqlCommand("SELECT `ID_Karta_nr` FROM `KARTA_TAB` WHERE `Nr_partii` LIKE \'" + textBox_nr_parti.Text.ToString() + "%\' AND `Klient` = \'PAKAITA\'  AND `Data_przyjecia` LIKE \'" + str_rok + "%\';", polocz);
                            dataGridView_partie.Rows[i].Cells[3].Value = Id_karta_nr.ExecuteScalar().ToString();

                            polocz.Close();
                        }catch(Exception){
                            polocz.Close();
                        }

                       
                    }
                     
                    
                }

                

            }catch(Exception ){

            }

            ////////////////////////////wybieranie parti  bawionych////////////////////////////////////

            try
            {
                
                dataSet_temp.Clear();
                dataGridView_temp.DataSource = dataSet_temp;

                polocz.Open();

                
                //MySql.Data.MySqlClient.MySqlCommand szukaj_parti = new MySql.Data.MySqlClient.MySqlCommand("SELECT  `Nr_partii`, `Klient`, `Numer_art`, `ID_Karta_nr`, `Data_przyjecia`  FROM `KARTA_TAB` WHERE `Nr_partii` LIKE \'" + textBox_nr_parti.Text.ToString() + "%\' AND `Numer_art` NOT LIKE \'%/D%\' AND `Numer_art` NOT LIKE \'%/T%\';", polocz);

                //MySql.Data.MySqlClient.MySqlCommand szukaj_parti = new MySql.Data.MySqlClient.MySqlCommand("SELECT  K.`Nr_partii`, K.`Klient`, K.`Numer_art`, K.`ID_Karta_nr`, K.`Data_przyjecia`, Z.`ID_Karta_nr`  FROM `KARTA_TAB` K LEFT JOIN `ZAFAKTUROWANE_ANFARB_TAB` Z ON K.`ID_Karta_nr` = Z.`ID_Karta_nr` WHERE K.`Nr_partii` LIKE \'" + textBox_nr_parti.Text.ToString() + "%\' AND K.`Numer_art` NOT LIKE \'%/D%\' AND K.`Numer_art` NOT LIKE \'%/T%\' AND Z.`ID_Karta_nr` IS NULL AND K.`Data_przyjecia` > \'2020-12-04 11:00:00\';", polocz);

                MySql.Data.MySqlClient.MySqlCommand szukaj_parti = new MySql.Data.MySqlClient.MySqlCommand("SELECT  K.`Nr_partii`, K.`Klient`, K.`Numer_art`, K.`ID_Karta_nr`, K.`Data_przyjecia`, Z.`ID_Karta_nr`  FROM `KARTA_TAB` K LEFT JOIN `ZAFAKTUROWANE_ANFARB_TAB` Z ON K.`ID_Karta_nr` = Z.`ID_Karta_nr` WHERE K.`Nr_partii` LIKE \'" + textBox_nr_parti.Text.ToString() + "%\' AND Z.`ID_Karta_nr` IS NULL AND K.`Data_przyjecia` > \'2020-12-04 11:00:00\';", polocz);
 
              
                MySqlDataAdapter adapter_kolory = new MySqlDataAdapter(szukaj_parti);
                adapter_kolory.Fill(dataSet_temp, "Lista_kolory");
                dataGridView_temp.DataSource = dataSet_temp.Tables["Lista_kolory"];

                polocz.Close();

            }
            catch (Exception)
            {
                polocz.Close();

            }

            try
            {

                int int_ilosc_wierszy_partie_barwione = dataGridView_temp.RowCount;
                int int_ilosc_wierszy_partie = dataGridView_partie.RowCount;

                dataGridView_partie.RowCount = int_ilosc_wierszy_partie + int_ilosc_wierszy_partie_barwione;

                dataGridView_partie.Rows[int_ilosc_wierszy_partie].Cells[0].Value = "Barw";
                dataGridView_partie.Rows[int_ilosc_wierszy_partie].DefaultCellStyle.BackColor = Color.Yellow;

                //string str_sprawdz_czy_zafakturowana = "";

                int int_gdzie_wpisac = int_ilosc_wierszy_partie + 1;

                for (int i = int_ilosc_wierszy_partie + 1; i < int_ilosc_wierszy_partie + int_ilosc_wierszy_partie_barwione; i++)
                {
                    /*
                    try
                    {
                        polocz_MSSQL.Open();

                        SqlCommand sprawdz_czy_zafakturowana = new SqlCommand("SELECT karta_nr FROM dbo.karty WHERE karta_nr = \'" + dataGridView_temp.Rows[i - int_ilosc_wierszy_partie - 1].Cells[3].Value.ToString() + "\' AND stan_zlecenia != \'5\' AND akt = \'1\';", polocz_MSSQL);
                        str_sprawdz_czy_zafakturowana = sprawdz_czy_zafakturowana.ExecuteScalar().ToString();

                        polocz_MSSQL.Close();
                    }
                    catch (Exception)
                    {
                        polocz_MSSQL.Close();
                        str_sprawdz_czy_zafakturowana = "";
                    }
                     */

                    //if (!str_sprawdz_czy_zafakturowana.Equals(""))
                    //{
                        dataGridView_partie.Rows[int_gdzie_wpisac].DefaultCellStyle.BackColor = Color.White;

                        dataGridView_partie.Rows[int_gdzie_wpisac].Cells[0].Value = dataGridView_temp.Rows[i - int_ilosc_wierszy_partie - 1].Cells[0].Value;
                        dataGridView_partie.Rows[int_gdzie_wpisac].Cells[1].Value = dataGridView_temp.Rows[i - int_ilosc_wierszy_partie - 1].Cells[1].Value;
                        //////////////////separacja daty przyjecia//////////////////////////////
                        string str_data_przyjecia = dataGridView_temp.Rows[i - int_ilosc_wierszy_partie - 1].Cells[4].Value.ToString();
                        int int_pozycja = str_data_przyjecia.IndexOf(" ");
                        str_data_przyjecia = str_data_przyjecia.Substring(0, int_pozycja);
                        str_data_przyjecia = str_data_przyjecia.Replace("-", "");
                        ////////////////////////////////////////////////////////////////////////
                        dataGridView_partie.Rows[int_gdzie_wpisac].Cells[2].Value = str_data_przyjecia;
                        dataGridView_partie.Rows[int_gdzie_wpisac].Cells[3].Value = dataGridView_temp.Rows[i - int_ilosc_wierszy_partie - 1].Cells[3].Value;

                        int_gdzie_wpisac++;
                    //}

                }
            }catch(Exception ex)
            {
                MessageBox.Show("Wystąpił problem z wyborem parti barwionych\n\n\n" + ex + "", "UWAGA !!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }




            ////////////////////////////////////////////////////////////////////////////////////
            //pobierz_nr_parti_klient_data_artykul(1);
           // wyswietl_sztuki_parti();
        }

        public void wyswietl_sztuki_parti()
        {
            try
            {
                dataSet_partie_temp.Clear();
                dataGridView_parite_temp.DataSource = dataSet_partie_temp;

                string str_zapytanie_SQL = "";

                if (!str_klient_do_druku_etykieta.Equals("Damaz") && str_sprawdz_nr_karta.Equals("0"))
                {
                    if (str_klient_do_druku_etykieta.Equals("ANFARB"))
                    {
                        str_zapytanie_SQL = "SELECT ID_stabilizacja,  Nr_sztuki, Metry, Kilogramy, Uwagi, Wzor,  Data, Klient FROM ETYKIETY_STABILIZACJA_TAB WHERE Nr_parti = \'" + str_nr_parti_do_zapisania + "\' AND Artykul = \'" + label_artykul_baza.Text.ToString() + "\' AND Nr_karta = \'" + str_sprawdz_nr_karta + "\' ORDER BY Nr_sztuki;";
             
                    }else{
                        str_zapytanie_SQL = "SELECT ID_stabilizacja,  Nr_sztuki, Metry, Kilogramy, Uwagi, Wzor,  Data, Klient FROM ETYKIETY_STABILIZACJA_TAB WHERE Nr_parti = \'" + str_nr_parti_do_zapisania + "\' AND Artykul = \'" + label_artykul_baza.Text.ToString() + "\' AND Nr_karta = \'" + str_sprawdz_nr_karta + "\' AND `Klient` LIKE \'%" + str_klient_do_druku_etykieta + "%\' ORDER BY Nr_sztuki;";
             
                    }
                   // str_zapytanie_SQL = "SELECT ID_stabilizacja,  Nr_sztuki, Metry, Kilogramy, Uwagi, Wzor,  Data, Klient FROM ETYKIETY_STABILIZACJA_TAB WHERE Nr_parti = \'" + str_nr_parti_do_zapisania + "\' AND Artykul = \'" + label_artykul_baza.Text.ToString() + "\' AND Nr_karta = \'" + str_sprawdz_nr_karta + "\' AND `Klient` LIKE \'%" + str_klient_do_druku_etykieta + "%\' ORDER BY Nr_sztuki;";
                }
                else
                {
                    str_zapytanie_SQL = "SELECT ID_stabilizacja,  Nr_sztuki, Metry, Kilogramy, Uwagi, Wzor,  Data, Klient FROM ETYKIETY_STABILIZACJA_TAB WHERE Nr_parti = \'" + str_nr_parti_do_zapisania + "\' AND Artykul = \'" + label_artykul_baza.Text.ToString() + "\' AND Nr_karta = \'" + str_sprawdz_nr_karta + "\' ORDER BY Nr_sztuki;";
                }

                polocz.Open();

                //MySql.Data.MySqlClient.MySqlCommand szukaj_parti_do_drukku = new MySql.Data.MySqlClient.MySqlCommand("SELECT ID_stabilizacja,  Nr_sztuki, Metry, Kilogramy, Uwagi, Wzor, Klient FROM ETYKIETY_STABILIZACJA_TAB WHERE Nr_parti = \'" + str_nr_parti_do_zapisania + "\' AND Artykul = \'" + label_artykul_baza.Text.ToString() + "\' ORDER BY Nr_sztuki;", polocz);

                MySql.Data.MySqlClient.MySqlCommand szukaj_parti_do_drukku = new MySql.Data.MySqlClient.MySqlCommand(str_zapytanie_SQL, polocz);
                
                
               // str_sprawdz_nr_karta
                MySqlDataAdapter adapter_kolory = new MySqlDataAdapter(szukaj_parti_do_drukku);
                adapter_kolory.Fill(dataSet_partie_temp, "Lista_sztuk");
                dataGridView_parite_temp.DataSource = dataSet_partie_temp.Tables["Lista_sztuk"];

                polocz.Close();

                int int_ilosc_wierszy = dataGridView_parite_temp.RowCount;
                dataGridView_sztuki.RowCount = int_ilosc_wierszy;


                
                for (int i = 0; i < int_ilosc_wierszy - 1; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        dataGridView_sztuki.Rows[i].Cells[j].Value = dataGridView_parite_temp.Rows[i].Cells[j].Value;

                        try
                        {
                            if (!dataGridView_sztuki.Rows[i].Cells[4].Value.ToString().Equals("BEZ UWAG"))
                            {
                                dataGridView_sztuki.Rows[i].DefaultCellStyle.ForeColor = Color.Red;
                            }
                        }catch(Exception)
                        {

                        }
                    }

                }

                try
                {
                    label_data_stabilizacji_baza.Text = dataGridView_parite_temp.Rows[0].Cells[6].Value.ToString();
                }
                catch (Exception)
                {
                    label_data_stabilizacji_baza.Text = "";
                }

              //  str_sklad_dzianiny = "";
              //  pobierz_sklad_surowcowy();


               int int_pozycja_scroll_stab_nr_1 = dataGridView_sztuki.RowCount;
                try
                {


                    dataGridView_sztuki.FirstDisplayedScrollingRowIndex = int_pozycja_scroll_stab_nr_1 - 1;
                }
                catch (Exception)
                { }

            }catch(Exception es){
                MessageBox.Show("Problem z wyswietleniem sztuk parti\n\n\n" + es + "", "Info", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void dataGridView_partie_Click(object sender, EventArgs e)
        {
            try
            {
                label_nr_wzoru_na_etykiete.Text = "";
                label_kupony_klient.Text = "";
                str_sprawdz_nr_karta = "0";
                str_nr_wzoru_na_etykiete = "";
                str_klient_do_druku = "";

                int int_klikniety_wiersz = 0;
                int_klikniety_wiersz = dataGridView_partie.CurrentCell.RowIndex;

                pobierz_nr_parti_klient_data_artykul(int_klikniety_wiersz);

                str_sklad_dzianiny = "";     
                pobierz_sklad_surowcowy();

                wyswietl_sztuki_parti();

                int int_sprawdz_drapanie = 0;
                string str_nr_parti_artykul;
                int_sprawdz_drapanie = label_artykul_baza.Text.ToString().IndexOf("DRAP");
                str_nr_parti_artykul = "Nr parti: "+textBox_nr_parti.Text.ToString()+"  Artykuł: "+label_artykul_baza.Text.ToString()+"";


                if(int_sprawdz_drapanie > 0)
                {
                    Zdjecie_do_drapania zdjecie_do_drapania = new Zdjecie_do_drapania(str_nr_parti_artykul);
                    zdjecie_do_drapania.ShowDialog();
                }

                
                
            }catch(Exception)
            {
               
            }

        }

        public void czysc_wyswietlone_wzory()
        {
            try
            {
                dataGridView_wzory.RowCount = 15;
                for (int i = 0; i < 15; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        dataGridView_wzory.Rows[i].Cells[j].Value = "";
                    }
                    dataGridView_wzory.Rows[i].DefaultCellStyle.BackColor = Color.White;
                }
            }catch(Exception)
            {

            }
        }

        public void pobierz_nr_parti_klient_data_artykul(int int_klikniety_wiersz)
        {
            try
            {
                string str_nr_karta = "";
                czysc_wyswietlone_wzory();
                czysc_karte_obiegowa();

                try
                {
                    str_nr_karta = dataGridView_partie.Rows[int_klikniety_wiersz].Cells[3].Value.ToString();
                    str_sprawdz_nr_karta = str_nr_karta;
                }catch(Exception)
                {
                    str_nr_karta = "0";
                }
                string str_nr_parti = dataGridView_partie.Rows[int_klikniety_wiersz].Cells[0].Value.ToString();
                str_nr_parti_do_zapisania = str_nr_parti;
                string str_klient = dataGridView_partie.Rows[int_klikniety_wiersz].Cells[1].Value.ToString();
                str_klient_do_druku_etykieta = str_klient;
                string str_data = dataGridView_partie.Rows[int_klikniety_wiersz].Cells[2].Value.ToString();
                str_data_parti = str_data;

                
                label_klient_baza.Text = str_klient;
                label_nr_parti_baza.Text = str_nr_parti;

                
                if(str_klient.Equals("PAKAITA"))
                {

                    wypelni_wzory(str_nr_parti, str_klient, str_data);
                }
                 
                 

                if (str_nr_karta.Equals("0"))
                {

                    wypelni_wzory(str_nr_parti, str_klient, str_data);

                    polocz_drukarnia_baza.Open();

                   // MySql.Data.MySqlClient.MySqlCommand artykul = new MySql.Data.MySqlClient.MySqlCommand("SELECT  DRUKARNIA_TAB.Nr_artykulu FROM DRUKARNIA_TAB WHERE DRUKARNIA_TAB.Nr_parti = \'" + str_nr_parti + "\' AND DRUKARNIA_TAB.Data = \'" + str_data + "\';", polocz_drukarnia_baza);

                    MySql.Data.MySqlClient.MySqlCommand artykul = new MySql.Data.MySqlClient.MySqlCommand("SELECT  D.Nr_artykulu FROM (DRUKARNIA_TAB D LEFT JOIN KLIENT_TAB K ON D.ID_Klient_drukarnia = K.ID_Klient ) WHERE D.Nr_parti = \'" + str_nr_parti + "\' AND D.Data = \'" + str_data + "\' AND K.Nazwa_Klient = \'" + str_klient + "\';", polocz_drukarnia_baza);
                    
                    
                    label_artykul_baza.Text = artykul.ExecuteScalar().ToString();

                    polocz_drukarnia_baza.Close();

                    sprawdz_procent_metry_dla_artykul();

                }
                else
                {
                   
                    //label_artykul_baza.Text = str_data;

                    info_karta_obiegowa(Convert.ToInt32(str_nr_karta));
                }

                
                if (str_klient.Equals("PAKAITA") && str_nr_karta.Equals("0"))
                {

                    int int_pozycja = 0;
                    //string str_artykul = label_artykul_baza.Text.ToString();
                    int_pozycja = str_nr_parti.IndexOf(" ");
                    try
                    {
                        str_nr_parti = str_nr_parti.Substring(0, int_pozycja);
                    }
                    catch (Exception)
                    {
                        //str_artykul = "";
                    }

                    polocz.Open();

                    MySql.Data.MySqlClient.MySqlCommand ID_karta = new MySql.Data.MySqlClient.MySqlCommand("SELECT `ID_Karta_nr` FROM `KARTA_TAB` WHERE  `Nr_partii` = \'" + str_nr_parti + "\' AND Klient = \'PAKAITA\' ORDER BY `ID_Karta_nr` DESC;", polocz);
                    str_nr_karta = Convert.ToString(ID_karta.ExecuteScalar().ToString());

                    polocz.Close();

                    info_karta_obiegowa(Convert.ToInt32(str_nr_karta));

                }

            }
            catch (Exception ex)
            {
                //MessageBox.Show("Klikniety dataGridViev.partie\n" + ex + "", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Information);
                polocz_drukarnia_baza.Close();
                polocz_zamowienia_drukarnia.Close();
                str_sprawdz_nr_karta = "";
                str_nr_parti_do_zapisania = "";
                str_klient_do_druku_etykieta = "";
                str_data_parti = "";

            }

        }
        public void wypelni_wzory(string nr_parti, string klient, string data)
		 {
			 string str_id_drukarnia;
			 string ktory_rekord;
			 string temp;
			 string str_data_produkcji_temp;
			 string str_rok;
			 string str_miesiac;
			 string str_dzien;


			 try{

			 polocz_drukarnia_baza.Open();

			 MySql.Data.MySqlClient.MySqlCommand id_drukarnia = new MySql.Data.MySqlClient.MySqlCommand("SELECT ID_Drukarnia FROM DRUKARNIA_TAB, KLIENT_TAB WHERE DRUKARNIA_TAB.Data = \'"+data+"\' AND DRUKARNIA_TAB.Nr_parti = \'"+nr_parti+"\' AND KLIENT_TAB.Nazwa_Klient = \'"+klient+"\' AND KLIENT_TAB.ID_Klient = DRUKARNIA_TAB.ID_Klient_drukarnia;", polocz_drukarnia_baza);
			 str_id_drukarnia = Convert.ToString(id_drukarnia.ExecuteScalar().ToString());

			 ktory_rekord = str_id_drukarnia;

			 polocz_drukarnia_baza.Close();

			 


			

			 /////////////////////////////////////WYSWIETL NAZWA WZORU///////////////////////////////////////////////////////
             dataSet_partie_temp.Clear();
             dataGridView_parite_temp.DataSource = dataSet_partie_temp;

             polocz_drukarnia_baza.Open();

             MySql.Data.MySqlClient.MySqlCommand wzory_razem = new MySql.Data.MySqlClient.MySqlCommand("SELECT WZORY_COPIE_METRY_TAB.ID_Nazwa_wzory_metry, WZORY_COPIE_METRY_TAB.Copie,  WZORY_COPIE_METRY_TAB.Metry FROM WZORY_DO_WYDRUKU_TAB, WZORY_COPIE_METRY_TAB WHERE WZORY_DO_WYDRUKU_TAB.ID_Wzory_do_wydruku = \'" + ktory_rekord + "\' AND (WZORY_DO_WYDRUKU_TAB.ID_Wzor_1 = WZORY_COPIE_METRY_TAB.ID_Wzory_copie OR WZORY_DO_WYDRUKU_TAB.ID_Wzor_2 = WZORY_COPIE_METRY_TAB.ID_Wzory_copie OR WZORY_DO_WYDRUKU_TAB.ID_Wzor_3 = WZORY_COPIE_METRY_TAB.ID_Wzory_copie OR WZORY_DO_WYDRUKU_TAB.ID_Wzor_4 = WZORY_COPIE_METRY_TAB.ID_Wzory_copie OR WZORY_DO_WYDRUKU_TAB.ID_Wzor_5 = WZORY_COPIE_METRY_TAB.ID_Wzory_copie OR WZORY_DO_WYDRUKU_TAB.ID_Wzor_6 = WZORY_COPIE_METRY_TAB.ID_Wzory_copie OR WZORY_DO_WYDRUKU_TAB.ID_Wzor_7 = WZORY_COPIE_METRY_TAB.ID_Wzory_copie OR WZORY_DO_WYDRUKU_TAB.ID_Wzor_8 = WZORY_COPIE_METRY_TAB.ID_Wzory_copie OR WZORY_DO_WYDRUKU_TAB.ID_Wzor_9 = WZORY_COPIE_METRY_TAB.ID_Wzory_copie OR WZORY_DO_WYDRUKU_TAB.ID_Wzor_10 = WZORY_COPIE_METRY_TAB.ID_Wzory_copie OR WZORY_DO_WYDRUKU_TAB.ID_Wzor_11 = WZORY_COPIE_METRY_TAB.ID_Wzory_copie OR WZORY_DO_WYDRUKU_TAB.ID_Wzor_12 = WZORY_COPIE_METRY_TAB.ID_Wzory_copie OR WZORY_DO_WYDRUKU_TAB.ID_Wzor_13 = WZORY_COPIE_METRY_TAB.ID_Wzory_copie OR WZORY_DO_WYDRUKU_TAB.ID_Wzor_14 = WZORY_COPIE_METRY_TAB.ID_Wzory_copie OR WZORY_DO_WYDRUKU_TAB.ID_Wzor_15 = WZORY_COPIE_METRY_TAB.ID_Wzory_copie);", polocz_drukarnia_baza);
             MySqlDataAdapter adapter_wzory = new MySqlDataAdapter(wzory_razem);
             adapter_wzory.Fill(dataSet_partie_temp, "Lista_wzorow");
             dataGridView_parite_temp.DataSource = dataSet_partie_temp.Tables["Lista_wzorow"];

             polocz_drukarnia_baza.Close();

             dataGridView_wzory.RowCount = 1;
             dataGridView_wzory.RowCount = 15;	

			for(int i=0; i<15; i++)
			{
				
                
				/////////////////////////////////////WYSWIETL NAZWA WZORU///////////////////////////////////////////////////////
			
				//MySql.Data.MySqlClient.MySqlCommand wzory_copie_metry1 = new MySql.Data.MySqlClient.MySqlCommand("SELECT ID_Nazwa_wzory_metry FROM WZORY_DO_WYDRUKU_TAB, WZORY_COPIE_METRY_TAB WHERE WZORY_DO_WYDRUKU_TAB.ID_Wzory_do_wydruku = \'"+ktory_rekord+"\' AND WZORY_DO_WYDRUKU_TAB.ID_Wzor_"+(i+1)+" = WZORY_COPIE_METRY_TAB.ID_Wzory_copie;", polocz_drukarnia_baza);
				//((DataGridViewComboBoxCell)dataGridView_wzory.Rows[i].Cells[1]).Items.Add(Convert.ToString(wzory_copie_metry1.ExecuteScalar().ToString()));						
				//dataGridView_wzory.Rows[i].Cells[0].Value = Convert.ToString(wzory_copie_metry1.ExecuteScalar().ToString());
                dataGridView_wzory.Rows[i].Cells[0].Value = dataGridView_parite_temp.Rows[i].Cells[0].Value.ToString();
                try
                {
                    tab_wzory[i, 0] = dataGridView_wzory.Rows[i].Cells[0].Value.ToString();
                }catch(Exception){
                    tab_wzory[i, 0] = "";
                }

				///////////////////////////////////////////////WYSWIETL ILOSC /////////////////////////////////////////////
			
				//MySql.Data.MySqlClient.MySqlCommand wzory_copie1 = new MySql.Data.MySqlClient.MySqlCommand("SELECT WZORY_COPIE_METRY_TAB.Copie FROM WZORY_DO_WYDRUKU_TAB, WZORY_COPIE_METRY_TAB WHERE WZORY_DO_WYDRUKU_TAB.ID_Wzory_do_wydruku = \'"+ktory_rekord+"\' AND WZORY_DO_WYDRUKU_TAB.ID_Wzor_"+(i+1)+" = WZORY_COPIE_METRY_TAB.ID_Wzory_copie;", polocz_drukarnia_baza);
			    //dataGridView_wzory.Rows[i].Cells[1].Value = Convert.ToString(wzory_copie1.ExecuteScalar().ToString());
                dataGridView_wzory.Rows[i].Cells[1].Value = dataGridView_parite_temp.Rows[i].Cells[1].Value.ToString();
				/////////////////////////////////////////////////WYSWIETL UWAGI///////////////////////////////////////////////////////

				//MySql.Data.MySqlClient.MySqlCommand wzory_metry1 = new MySql.Data.MySqlClient.MySqlCommand("SELECT WZORY_COPIE_METRY_TAB.Metry FROM WZORY_DO_WYDRUKU_TAB, WZORY_COPIE_METRY_TAB WHERE WZORY_DO_WYDRUKU_TAB.ID_Wzory_do_wydruku = \'"+ktory_rekord+"\' AND WZORY_DO_WYDRUKU_TAB.ID_Wzor_"+(i+1)+" = WZORY_COPIE_METRY_TAB.ID_Wzory_copie;", polocz_drukarnia_baza);
				//dataGridView_wzory.Rows[i].Cells[2].Value = Convert.ToString(wzory_metry1.ExecuteScalar().ToString());
                dataGridView_wzory.Rows[i].Cells[2].Value = dataGridView_parite_temp.Rows[i].Cells[2].Value.ToString();
                try
                {
                    tab_wzory[i, 1] = dataGridView_wzory.Rows[i].Cells[2].Value.ToString();
                }catch(Exception){
                    tab_wzory[i, 1] = "";
                }
                 
			
			}


             label_nr_wzoru_na_etykiete.Text = dataGridView_wzory.Rows[0].Cells[0].Value.ToString();
            // str_klient_do_zapisu = dataGridView_wzory.Rows[0].Cells[2].Value.ToString();
             str_klient_do_druku = dataGridView_wzory.Rows[0].Cells[2].Value.ToString();
             str_klient_do_zapisu = str_klient_do_druku;
             label_kupony_klient.Text = "";


			 polocz_drukarnia_baza.Close();

			 }catch(Exception)
			 {
				 polocz_drukarnia_baza.Close();
			 }

			


		 }

        public void pobierz_sklad_surowcowy()
        {
            int int_pozycja = 0;
            string str_artykul = "";
            string str_karta_id = "";
            string str_klient_id = "";

            dataSet_temp.Clear();
            dataGridView_temp.DataSource = dataSet_temp;

            try
            {
                

                str_sklad_dzianiny = "";

                try
                {
                    str_artykul = label_artykul_baza.Text.ToString();
                    int_pozycja = str_artykul.IndexOf(" ");
                    str_artykul = str_artykul.Substring(0, int_pozycja);
                }catch(Exception)
                {
                    str_artykul = "";
                }


                

                polocz_MSSQL.Open();

                SqlCommand klient_id = new SqlCommand("SELECT klient_id FROM dbo.klienci where dbo.klienci.klient_name = \'" + str_klient_do_druku_etykieta + "\';", polocz_MSSQL);
                str_klient_id = Convert.ToString(klient_id.ExecuteScalar());

                polocz_MSSQL.Close();



                if (str_sprawdz_nr_karta.Equals("0"))
                {
                    if (!str_klient_id.Equals("16000018"))  //różne od Rokiet
                    {
                        polocz_MSSQL.Open();

                        SqlCommand karta_id = new SqlCommand("SELECT karta_id FROM dbo.karty where dbo.karty.nr_partii = \'" + textBox_nr_parti.Text.ToString() + "\' AND dbo.karty.nr_art LIKE \'" + str_artykul + "%\' AND dbo.karty.klient_id = \'" + str_klient_id + "\';", polocz_MSSQL);
                        str_karta_id = Convert.ToString(karta_id.ExecuteScalar());

                        polocz_MSSQL.Close();
                    }
                    if (str_klient_id.Equals("16000018"))  // jeśli Rokiet
                    {
                        polocz_MSSQL.Open();

                        SqlCommand karta_id = new SqlCommand("SELECT karta_id FROM dbo.karty where dbo.karty.nr_partii = \'" + textBox_nr_parti.Text.ToString() + "\' AND dbo.karty.klient_id = \'" + str_klient_id + "\' ORDER BY karta_id DESC;", polocz_MSSQL);
                        str_karta_id = Convert.ToString(karta_id.ExecuteScalar());

                        polocz_MSSQL.Close();
                    }
                }
                
                if (!str_sprawdz_nr_karta.Equals("0"))
                {
                    polocz_MSSQL.Open();
                    SqlCommand karta_id = new SqlCommand("SELECT karta_id FROM dbo.karty where dbo.karty.nr_partii = \'" + textBox_nr_parti.Text.ToString() + "\' AND dbo.karty.nr_art LIKE \'" + str_artykul + "%\' AND dbo.karty.klient_id = \'" + str_klient_id + "\' AND dbo.karty.stan_zlecenia != \'5\';", polocz_MSSQL);
                    str_karta_id = Convert.ToString(karta_id.ExecuteScalar());
                    polocz_MSSQL.Close();
                }


                polocz_MSSQL.Open();

                SqlCommand karta_id_MSSQL = new SqlCommand("SELECT dbo.karty_wiersz.karta_id, dbo.sl_tkanina.tkan_name, dbo.karty_wiersz.tkanina_procent FROM dbo.karty_wiersz, dbo.sl_tkanina where dbo.karty_wiersz.karta_id = \'" + str_karta_id + "\' AND dbo.karty_wiersz.tkanina_id = dbo.sl_tkanina.tkan_id AND dbo.karty_wiersz.akt = \'1\' ORDER BY dbo.karty_wiersz.tkanina_procent DESC;", polocz_MSSQL);
                SqlDataAdapter adapter_karta_id = new SqlDataAdapter(karta_id_MSSQL);
                adapter_karta_id.Fill(dataSet_temp, "Lista_karta_id");
                dataGridView_temp.DataSource = dataSet_temp.Tables["Lista_karta_id"];

                polocz_MSSQL.Close();
            }
            catch (Exception)
            {
                polocz_MSSQL.Close();
            }

            dataSet_szer_gram.Clear();
            dataGridView_szer_gram.DataSource = dataSet_szer_gram;
            try
            {
                polocz_MSSQL.Open();

               // SqlCommand szer_gram = new SqlCommand("SELECT szer_stabilizacji_cm, ciezar_1m2_gr, szer_wydajnosc_mb_kg, karta_id FROM dbo.karty where dbo.karty.karta_nr = \'" + str_karta_id + "\';", polocz_MSSQL);


                SqlCommand szer_gram = new SqlCommand("SELECT szer_stabilizacji_cm, ciezar_1m2_gr, szer_wydajnosc_mb_kg, karta_id FROM dbo.karty where dbo.karty.nr_partii = \'" + textBox_nr_parti.Text.ToString() + "\' AND dbo.karty.nr_art LIKE \'" + str_artykul + "%\' AND dbo.karty.klient_id = \'" + str_klient_id + "\' AND dbo.karty.stan_zlecenia != \'5\';", polocz_MSSQL);
                SqlDataAdapter adapter_szer_gram = new SqlDataAdapter(szer_gram);
                adapter_szer_gram.Fill(dataSet_szer_gram, "Lista_szer_gram");
                dataGridView_szer_gram.DataSource = dataSet_szer_gram.Tables["Lista_szer_gram"];

                polocz_MSSQL.Close();
            }catch(Exception){
                polocz_MSSQL.Close();
            }

            try
            {
                str_szerokosc_do_druku = dataGridView_szer_gram.Rows[0].Cells[0].Value.ToString() + " cm";
            }catch(Exception){
                str_szerokosc_do_druku = "";
            }
            try
            {
                str_gramatura_do_druku = dataGridView_szer_gram.Rows[0].Cells[1].Value.ToString() + " gr";
            }catch(Exception){
                str_gramatura_do_druku = "";
            }


            try
            {
                int int_ilosc_wierszy = dataGridView_temp.RowCount;

                for (int i = 0; i < int_ilosc_wierszy - 1; i++)
                {
                    if (!dataGridView_temp.Rows[i].Cells[2].Value.ToString().Equals("0"))
                    {
                        if (!str_klient_do_druku_etykieta.Equals("RICH-S") && (!str_klient_do_druku_etykieta.Equals("TOMTEX")) && (!str_klient_do_druku_etykieta.Equals("TOMTEX/MATERACOWE")))
                        {
                            str_sklad_dzianiny += dataGridView_temp.Rows[i].Cells[1].Value.ToString() + " " + dataGridView_temp.Rows[i].Cells[2].Value.ToString() + "% ";
                        }
                        if (str_klient_do_druku_etykieta.Equals("RICH-S") && (!str_klient_do_druku_etykieta.Equals("TOMTEX")) && (!str_klient_do_druku_etykieta.Equals("TOMTEX/MATERACOWE")))
                        {
                            str_sklad_dzianiny += dataGridView_temp.Rows[i].Cells[1].Value.ToString() + "  ";
                        }
                        if (str_klient_do_druku_etykieta.Equals("TOMTEX") && (str_klient_do_druku_etykieta.Equals("TOMTEX/MATERACOWE")))
                        {
                           // str_sklad_dzianiny += dataGridView_temp.Rows[i].Cells[1].Value.ToString() + " " + dataGridView_temp.Rows[i].Cells[2].Value.ToString() + "% ";
                            str_sklad_dzianiny += dataGridView_temp.Rows[i].Cells[2].Value.ToString() + "% " + dataGridView_temp.Rows[i].Cells[1].Value.ToString() + ", ";
                        }
                    }
                }
                if (!str_klient_do_druku_etykieta.Equals("TOMTEX") && (!str_klient_do_druku_etykieta.Equals("TOMTEX/MATERACOWE")))
                {
                    if (str_sklad_dzianiny.Length > 20)
                    {
                        str_sklad_dzianiny = str_sklad_dzianiny.Substring(0, 20);
                    }
                }
            }catch(Exception){

            }
        }

        private void dataGridView_wzory_Click(object sender, EventArgs e)
        {
            try
            {
                int int_klikniety_wiersz = 0;

                if (str_stan_licznika.Equals("0"))
                {
                    
                    int_klikniety_wiersz = dataGridView_wzory.CurrentCell.RowIndex;
                    int_klikniety_wiersz_do_zaznaczenia_wzoru = int_klikniety_wiersz;
                    

                    label_nr_wzoru_na_etykiete.Font = new Font(FontFamily.GenericSansSerif, 24, FontStyle.Bold);
                    flaga_dwa_wzory = false;

                    if (flaga_wzory == true)
                    {
                        string str_nr_wzoru_etykieta = dataGridView_wzory.Rows[int_klikniety_wiersz].Cells[0].Value.ToString();

                        if (str_nr_wzoru_etykieta.Length > 24)
                        {
                            str_nr_wzoru_etykieta = str_nr_wzoru_etykieta.Substring(0, 24);
                        }

                        label_nr_wzoru_na_etykiete.Text = str_nr_wzoru_etykieta;
                    }
                    else
                    {
                        label_nr_wzoru_na_etykiete.Text = "KUPONY";
                        label_kupony_klient.Text = dataGridView_wzory.Rows[int_klikniety_wiersz].Cells[2].Value.ToString();
                    }

                    str_klient_do_druku = dataGridView_wzory.Rows[int_klikniety_wiersz].Cells[2].Value.ToString();
                    str_klient_do_zapisu = str_klient_do_druku;

                    sprawdz_procent_metry_dla_klient();
                }
                else
                {
                    MessageBox.Show("Aby wybrać wzór\nstan licznika = 0 m/b", "Uwaga !!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    for (int i = 0; i < 15; i++)
                    {

                        dataGridView_wzory.Rows[i].DefaultCellStyle.BackColor = Color.White;
                    }
                }

            }catch(Exception){}

        }

        Klawiatura_numeryczna klawiatura_numeryczna = new Klawiatura_numeryczna(true, false, false, false);
        private void textBox_odczyt_metrow_Click(object sender, EventArgs e)
        {
            
            try
            {
               // Klawiatura_numeryczna klawiatura_numeryczna = new Klawiatura_numeryczna(true, false, false);
               // Klawiatura_numeryczna klawiatura_numeryczna = new Klawiatura_numeryczna(true, false, false, true);
                klawiatura_numeryczna.flaga_metry_automat = true;
                klawiatura_numeryczna.textBox_kg.Text = "";
                klawiatura_numeryczna.textBox_nr_sztuki.Text = "";
                klawiatura_numeryczna.textBox__wartosc_cyfrowa.Text = "";
                klawiatura_numeryczna.ShowDialog();
                

               
                try
                {
                    d_kilogramy = Convert.ToDouble(klawiatura_numeryczna.textBox_kg.Text.ToString().Replace(".", ","));
                }
                catch (Exception)
                {
                    d_kilogramy = 0.0;
                }
                //////////dodaj do kilogramów 0.3 (firma Fabricant konkretne artykuły)/////////////////
                int int_sprawdz_nr_artykulu = 0;

                int_sprawdz_nr_artykulu = label_artykul_baza.Text.ToString().IndexOf("B504");

                if (int_sprawdz_nr_artykulu >= 0)
                {
                    d_kilogramy = d_kilogramy + 0.3;
                }
                int_sprawdz_nr_artykulu = label_artykul_baza.Text.ToString().IndexOf("B402");

                if (int_sprawdz_nr_artykulu >= 0)
                {
                    d_kilogramy = d_kilogramy + 0.3;
                }
                //////////////////////////////////////////////////////////////////////////////////



                if (klawiatura_numeryczna.flaga_drukuj == true && (!textBox_nr_parti.Text.ToString().Equals("")))
                {
                    str_ktora_sztuka_do_druku = klawiatura_numeryczna.textBox_nr_sztuki.Text.ToString();
                    str_metry_do_druku_temp = klawiatura_numeryczna.textBox__wartosc_cyfrowa.Text.ToString();
                    dodaj_sztuke_i_drukkuj_etykiete();

                    //label_poprzedni_pomiar_metrow.Text = str_metry_do_druku_temp;
                   
                   // textBox_odczyt_metrow.Text = str_stan_licznika + " m/b";
                }
            }catch(Exception)
            {

            }
             

        }

        public void dodaj_sztuke_i_drukkuj_etykiete()
        {
            int int_pozycja_cena = 0;
            flaga_cena = false;
            try
            {
                if (!str_klient_do_druku_etykieta.Equals(""))
                {
                   

                    str_nr_wzoru_na_etykiete = label_nr_wzoru_na_etykiete.Text.ToString();

                    int_pozycja_cena = str_nr_wzoru_na_etykiete.IndexOf("cena");
                    if (int_pozycja_cena >= 0)
                    {
                        str_nr_wzoru_na_etykiete = str_nr_wzoru_na_etykiete.Substring(0, int_pozycja_cena - 1);
                        flaga_cena = true;
                    }

                    if (str_nr_wzoru_na_etykiete.Length > 18 && flaga_dwa_wzory == false)
                    {

                        str_nr_wzoru_na_etykiete = str_nr_wzoru_na_etykiete.Substring(0, 18);
                    }

                    ktora_sztuka();
                    kilogramy_metry_do_zapisania();

                    label_poprzedni_pomiar_metrow.Text = str_metry_do_druku;

                    if (flaga_drukowanie == true)
                    {
                        drukuj_etykieta();
                    }

                    dodaj_sztuke();

                    wyswietl_sztuki_parti();

                    try
                    {
                        dataGridView_wzory.Rows[int_klikniety_wiersz_do_zaznaczenia_wzoru].DefaultCellStyle.BackColor = Color.Yellow;
                    }catch(Exception){

                    }
                   // int_klikniety_wiersz
                }
            }catch(Exception)
            {
              
            }

          // kilogramy_metry_do_zapisania();

        }

        public void kilogramy_metry_do_zapisania()
        {
            string str_kilogramy = "0.0";

            str_kilogramy = Convert.ToString(d_kilogramy);
            str_kilogramy = str_kilogramy.Replace(",", ".");

            str_kilogramy_do_druku = str_kilogramy;

            
            //int int_pozycja = 0;
            string str_metry = "";

            //int_pozycja = textBox_odczyt_metrow.Text.ToString().IndexOf("m/b");
             
           // str_metry = textBox_odczyt_metrow.Text.ToString();   //   .Substring(0, int_pozycja);
            str_metry = str_metry_do_druku_temp;
            str_metry = str_metry.Replace(" ", "");



            if (str_klient_do_druku_etykieta.Equals("DAMAZ") || str_klient_do_druku_etykieta.Equals("Damaz"))
            {
                str_metry = policz_metry(str_metry);
            }
            str_metry_do_druku = str_metry;
            

           // reset_metrow();
        }

        public void ktora_sztuka()
        {
            int int_ilosc_sztuk;
            

            int_ilosc_sztuk = dataGridView_sztuki.RowCount;


            if (flaga_zapamietaj_wzor == true)
            {
                if (int_ilosc_sztuk == 1)
                {
                    int_ktora_sztuka = int_ilosc_sztuk;
                }
                if (int_ilosc_sztuk > 1)
                {
                    int_ktora_sztuka = Convert.ToInt32(dataGridView_sztuki.Rows[(int_ilosc_sztuk - 2)].Cells[1].Value.ToString());
                    int_ktora_sztuka++;
                }
            }
            if (flaga_zapamietaj_wzor == false)
            {
                int_ktora_sztuka = Convert.ToInt32(button_zapamietaj_wzor.Text.ToString());
                button_zapamietaj_wzor.Text = Convert.ToString(int_ktora_sztuka + 1);
            }





            dataGridView_sztuki.RowCount = int_ilosc_sztuk + 1;




            if (str_ktora_sztuka_do_druku.Equals(""))
            {
                str_ktora_sztuka_do_druku = Convert.ToString(int_ktora_sztuka);
            }
            else
            {
                int_ktora_sztuka = Convert.ToInt32(str_ktora_sztuka_do_druku);
            }
        }

        public void dodaj_sztuke()
        {
            try
            {
                

                if (int_ktora_sztuka == 1)
                {
                    zmien_status_na_stabilizacja();
                }
                if (str_klient_do_druku_etykieta.Equals("PAKAITA"))
                {
                    zmien_status_na_stabilizacja();
                }
                if (str_klient_do_druku_etykieta.Equals("TRANSTEX"))
                {
                    zmien_status_na_stabilizacja();
                }
                if (str_klient_do_druku_etykieta.Equals("LUBATEX"))
                {
                    zmien_status_na_stabilizacja();
                }
                if (str_klient_do_druku_etykieta.Equals("PROVEL"))
                {
                    zmien_status_na_stabilizacja();
                }
                if (str_klient_do_druku_etykieta.Equals("TOMTEX") || str_klient_do_druku_etykieta.Equals("TOMTEX/MATERACOWE"))
                {
                    zmien_status_na_stabilizacja();
                }

                

                //zapisz(int_ktora_sztuka, str_metry_do_druku, str_klient_do_druku_etykieta, pobierz_uwagi_sztuki());
                zapisz(int_ktora_sztuka, str_metry_do_druku, str_klient_do_zapisu, pobierz_uwagi_sztuki());
            }catch(Exception)
            {
                MessageBox.Show("Wystąpił problem z dodaniem sztuki ", "Uwaga !", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }
        public String pobierz_date()
        {
            System.String razem;
            System.String dzien;
            System.String miesiac;
            System.String rok;
            DateTime data = DateTime.Now;
            dzien = data.Day.ToString("D2");
            miesiac = data.Month.ToString("D2");
            rok = data.Year.ToString("D2");
            razem = rok + "-" + miesiac + "-" + dzien;

            return razem;
        }
        public void zmien_status_na_stabilizacja()
        {
            string str_data_godzina = "";


            str_data_godzina = pobierz_date() + " " + pobierz_godzine_produkcji();

            try
            {
            if (str_sprawdz_nr_karta.Equals("0"))
            {
                try
                {
                    polocz_drukarnia_baza.Open();

                    MySql.Data.MySqlClient.MySqlCommand zmien_status = new MySql.Data.MySqlClient.MySqlCommand("UPDATE DRUKARNIA_TAB SET DRUKARNIA_TAB.ID_Status_drukarnia = \'6\' WHERE DRUKARNIA_TAB.Nr_parti = \'" + str_nr_parti_do_zapisania + "\' AND DRUKARNIA_TAB.Data = \'" + str_data_parti + "\';", polocz_drukarnia_baza);
                    zmien_status.ExecuteNonQuery();

                    polocz_drukarnia_baza.Close();
                }
                catch (Exception)
                {
                    polocz_drukarnia_baza.Close();
                }
                                    
            }

            try
            {
                String str_stabilizacja = "STABILIZACJA";
                String str_data = pobierz_date_rozdzielona();
                polocz_zamowienia_drukarnia.Open();

                MySql.Data.MySqlClient.MySqlCommand aktualizuj_wiersz = new MySql.Data.MySqlClient.MySqlCommand("UPDATE `ALERTY_TAB`, ZAMOWIENIA_TAB SET ALERTY_TAB.Data_stabilizacji_dzianiny=\'" + str_data + "\', ZAMOWIENIA_TAB.Status_zamowienia = \'" + str_stabilizacja + "\' WHERE ZAMOWIENIA_TAB.Nr_parti = \'" + label_nr_parti_baza.Text.ToString() + "\' AND ZAMOWIENIA_TAB.Artykul_zamowienia = \'" + label_artykul_baza.Text.ToString() + "\' AND ZAMOWIENIA_TAB.Wzory_zamowienia = \'" + label_nr_wzoru_na_etykiete.Text.ToString() + "\' AND ZAMOWIENIA_TAB.Wzory_zamowienia != \'\' AND ZAMOWIENIA_TAB.Status_wpis_do_zeszytu = \'1\' AND ZAMOWIENIA_TAB.Nr_wiersza = ALERTY_TAB.ID_nr_wiersza;", polocz_zamowienia_drukarnia);

               // MySql.Data.MySqlClient.MySqlCommand aktualizuj_wiersz = new MySql.Data.MySqlClient.MySqlCommand("UPDATE `ALERTY_TAB`, ZAMOWIENIA_TAB SET ALERTY_TAB.Data_stabilizacji_dzianiny=\'" + str_data + "\', ZAMOWIENIA_TAB.Status_zamowienia = \'" + str_stabilizacja + "\' WHERE ZAMOWIENIA_TAB.Nr_parti = \'" + label_nr_parti_baza.Text.ToString() + "\' AND ZAMOWIENIA_TAB.Artykul_zamowienia = \'" + label_artykul_baza.Text.ToString() + "\' AND ZAMOWIENIA_TAB.Wzory_zamowienia = \'" + label_nr_wzoru_na_etykiete.Text.ToString() + "\' AND ZAMOWIENIA_TAB.Nr_wiersza = ALERTY_TAB.ID_nr_wiersza;", polocz_zamowienia_drukarnia);
               
                aktualizuj_wiersz.ExecuteNonQuery();

                polocz_zamowienia_drukarnia.Close();
            }
            catch (Exception)
            {
                polocz_zamowienia_drukarnia.Close();
            }
									

            if (str_klient_do_druku_etykieta.Equals("PAKAITA"))
            {
                try
                {
                    polocz_drukarnia_baza.Open();

                    MySql.Data.MySqlClient.MySqlCommand zmien_status = new MySql.Data.MySqlClient.MySqlCommand("UPDATE DRUKARNIA_TAB SET DRUKARNIA_TAB.ID_Status_drukarnia = \'6\' WHERE DRUKARNIA_TAB.Nr_parti = \'" + str_nr_parti_do_zapisania + "\' AND DRUKARNIA_TAB.Data = \'" + str_data_parti + "\' AND DRUKARNIA_TAB.ID_Klient_drukarnia = \'10\';", polocz_drukarnia_baza);
                    zmien_status.ExecuteNonQuery();

                    polocz_drukarnia_baza.Close();
                }
                catch (Exception)
                {
                    polocz_drukarnia_baza.Close();
                }
                try
                {
                    polocz_MSSQL.Open();

                    SqlCommand zmien_status_MSSQL = new SqlCommand("update dbo.karty set dbo.karty.stan_zlecenia = '7' where dbo.karty.karta_nr = \'" + str_sprawdz_nr_karta + "\' AND dbo.karty.stan_zlecenia != '5' AND dbo.karty.stan_zlecenia != '8' AND dbo.karty.stan_zlecenia != '4';", polocz_MSSQL);
                    zmien_status_MSSQL.ExecuteNonQuery();


                    polocz_MSSQL.Close();
                }
                catch (Exception)
                {
                    polocz_MSSQL.Close();
                }
            }

            if (!str_sprawdz_nr_karta.Equals("0"))
            {

                if (str_klient_do_druku_etykieta.Equals("TRANSTEX") || str_klient_do_druku_etykieta.Equals("LUBATEX") || str_klient_do_druku_etykieta.Equals("TOMTEX") || str_klient_do_druku_etykieta.Equals("TOMTEX/MATERACOWE") || str_klient_do_druku_etykieta.Equals("PROVEL"))
                {
                    try
                    {
                        polocz_MSSQL.Open();

                        SqlCommand zmien_status_MSSQL = new SqlCommand("update dbo.karty set dbo.karty.stan_zlecenia = '4', dbo.karty.data_produkcji = \'" + str_data_godzina + "\' where dbo.karty.karta_nr = \'" + str_sprawdz_nr_karta + "\' AND dbo.karty.stan_zlecenia != '5' AND dbo.karty.stan_zlecenia != '8' AND dbo.karty.stan_zlecenia != '4';", polocz_MSSQL);
                        zmien_status_MSSQL.ExecuteNonQuery();


                        polocz_MSSQL.Close();
                    }
                    catch (Exception)
                    {
                        polocz_MSSQL.Close();
                    }
                    /*
                    try
                    {
                        polocz_MSSQL.Open();

                        SqlCommand zmien_status_MSSQL = new SqlCommand("update dbo.karty set dbo.karty.stan_zlecenia = '8' where dbo.karty.karta_nr = \'" + str_sprawdz_nr_karta + "\' AND dbo.karty.stan_zlecenia != '5' AND dbo.karty.stan_zlecenia != '8';", polocz_MSSQL);
                        zmien_status_MSSQL.ExecuteNonQuery();


                        polocz_MSSQL.Close();
                    }
                    catch (Exception)
                    {
                        polocz_MSSQL.Close();
                    }
                     */
                   
                }
                else
                {
                    try
                    {
                        polocz_MSSQL.Open();

                        SqlCommand zmien_status_MSSQL = new SqlCommand("update dbo.karty set dbo.karty.stan_zlecenia = '7' where dbo.karty.karta_nr = \'" + str_sprawdz_nr_karta + "\' AND dbo.karty.stan_zlecenia != '5' AND dbo.karty.stan_zlecenia != '8' AND dbo.karty.stan_zlecenia != '4';", polocz_MSSQL);
                        zmien_status_MSSQL.ExecuteNonQuery();


                        polocz_MSSQL.Close();
                    }
                    catch (Exception)
                    {
                        polocz_MSSQL.Close();
                    }
                }
                
                    

                    string str_nr_stabilizacji = "";

                    StreamReader odczytu_nr_stab = new StreamReader("ustawienia_stab.txt");
                    str_nr_stabilizacji = odczytu_nr_stab.ReadLine();
                    odczytu_nr_stab.Close();

                    string str_sprawdz_stabilizacje = "";

                    try
                    {
                        polocz.Open();


                        MySqlCommand sprawdz_czy_byla_stabilizacja = new MySqlCommand("SELECT `ID_Karta_nr` FROM `PRODUKCJA_STAB_TAB` WHERE `ID_Karta_nr`=\'" + str_sprawdz_nr_karta + "\' AND `ID_stab` = \'" + str_nr_stabilizacji + "\' AND `ID_stab` != \'10\';", polocz);
                        str_sprawdz_stabilizacje = sprawdz_czy_byla_stabilizacja.ExecuteScalar().ToString();

                        polocz.Close();

                    }
                    catch (Exception)
                    {
                        str_sprawdz_stabilizacje = "";
                        polocz.Close();
                    }

                    if (str_sprawdz_stabilizacje.Equals(""))
                    {
                        try
                        {
                            polocz.Open();


                            //MySqlCommand po_stabilizacji = new MySqlCommand("UPDATE `PRODUKCJA_STAB_TAB` SET `Data_produkcji`=\'" + ustawienia.str_nr_stabilizacji + "\' ,`Data_produkcji`=\'" + pobierz_date_rozdzielona() + "\',`Godzina_produkcji`=\'" + pobierz_godzine() + "\' WHERE  `ID_Karta_nr`=\'" + str_sprawdz_nr_karta + "\';", polocz);

                            MySqlCommand po_stabilizacji = new MySqlCommand("INSERT INTO `PRODUKCJA_STAB_TAB`(`ID_Karta_nr`, `ID_pracownik`, `ID_stab`, `Data_produkcji`, `Godzina_produkcji`, `Data_dodania`, `Uwagi`, `Wsad`) VALUES (\'" + str_sprawdz_nr_karta + "\',\'0\',\'" + str_nr_stabilizacji + "\',\'" + pobierz_date_rozdzielona() + "\',\'" + pobierz_godzine() + "\',\'\',\'\',\'\');", polocz);


                            po_stabilizacji.ExecuteNonQuery();

                            polocz.Close();

                        }
                        catch (Exception)
                        {
                            polocz.Close();
                        }
                    }



                }
            }catch(Exception)
            {
                MessageBox.Show("Status na stabilizacji nie został zmieniony", "Uwaga !", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        public String pobierz_godzine()
        {
            String razem;
            String str_godzina;
            String str_minuty;

            DateTime godzina = DateTime.Now;
            str_godzina = godzina.Hour.ToString("D2");
            str_minuty = godzina.Minute.ToString("D2");

            razem = str_godzina + ":" + str_minuty;

            return razem;
        }
        public String pobierz_godzine_produkcji()
        {
            System.String razem;
            System.String godzina;
            System.String minuta;
            System.String sekunda;
            DateTime data = DateTime.Now;
            godzina = data.Hour.ToString("D2");
            minuta = data.Minute.ToString("D2");
            //sekunda = data.Second.ToString("D2");
            sekunda = "00";
            razem = godzina + ":" + minuta + ":" + sekunda;

            return razem;
        }

        /*
        public string policz_metry(string metry)
        {
            try
            {
                int int_metry_przeliczone_zero = 0;
                int int_metry_przeliczone_klient = 0;
                int int_metry_przeliczone_artykul = 0;
                int int_procent_klient = 0;
                int int_procent_artykul = 0;
                int int_pozycja = 0;


                double d_suma_procent = 0.0;
                double d_metry = Convert.ToDouble(metry);
                double d_roznica = 0.0;

                string str_klient = "";
                string str_artykul;

                ////////////////obliczanie punktu zero/////////////////
                pobierz_zero();

                d_suma_procent = (Convert.ToDouble(int_procent_zero) / 100) * d_metry;
                d_roznica = d_metry + d_suma_procent;
                int_metry_przeliczone_zero = Convert.ToInt32(d_roznica);
                ///////////////////////////////////////////////////////

                ///////////////obliczanie procent klient//////////////////////
                try
                {
                    int_pozycja = str_klient_do_druku.IndexOf(" ");

                    str_klient = str_klient_do_druku.Substring(0, int_pozycja);
                }
                catch (Exception) { }


                try
                {
                    polocz.Open();

                    MySqlCommand pobierz_procent_klient = new MySqlCommand("SELECT procent_klient FROM PRZELICZNIK_METRY_TAB WHERE Klient = \'" + str_klient + "\';", polocz);
                    int_procent_klient = Convert.ToInt32(pobierz_procent_klient.ExecuteScalar().ToString());

                    polocz.Close();
                }
                catch (Exception)
                {
                    polocz.Close();
                    int_procent_klient = 0;
                }

                d_suma_procent = (Convert.ToDouble(int_procent_klient) / 100) * Convert.ToDouble(int_metry_przeliczone_zero);
                d_roznica = Convert.ToDouble(int_metry_przeliczone_zero) + d_suma_procent;
                int_metry_przeliczone_klient = Convert.ToInt32(d_roznica);

                ////////////////////////////////////////////////////////////////

                ///////////////obliczanie procent artykul//////////////////////
                str_artykul = label_artykul_baza.Text.ToString();

                try
                {
                    polocz.Open();

                    MySqlCommand pobierz_procent_artykul = new MySqlCommand("SELECT procent_artykul FROM PRZELICZNIK_METRY_TAB WHERE Artykul = \'" + str_artykul + "\';", polocz);
                    int_procent_artykul = Convert.ToInt32(pobierz_procent_artykul.ExecuteScalar().ToString());

                    polocz.Close();
                }
                catch (Exception)
                {
                    polocz.Close();
                    int_procent_klient = 0;
                }

                d_suma_procent = (Convert.ToDouble(int_procent_artykul) / 100) * Convert.ToDouble(int_metry_przeliczone_klient);
                d_roznica = Convert.ToDouble(int_metry_przeliczone_klient) + d_suma_procent;
                int_metry_przeliczone_artykul = Convert.ToInt32(d_roznica);

                ////////////////////////////////////////////////////////////////



                metry = Convert.ToString(int_metry_przeliczone_artykul);
            }
            catch (Exception)
            {
                MessageBox.Show("Metry nie zostały policzone", "Uwaga !", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

                return metry;
           
        }
         */

        public string pobierz_uwagi_sztuki()
        {
            
            int i = 0;
            string str_pobrane_uwagi = "";
            
                StreamReader odczyt_uwagi_temp = new StreamReader("uwagi_temp.txt");
                string line;

                while ((line = odczyt_uwagi_temp.ReadLine()) != null)
                {
                    if (i == 0)
                    {
                        str_pobrane_uwagi += line;
                    }
                    else
                    {
                        str_pobrane_uwagi += "\n" + line;
                    }
                    i++;

                }
                odczyt_uwagi_temp.Close();

            if(str_pobrane_uwagi.Equals(""))
            {
                str_pobrane_uwagi = "BEZ UWAG";
            }

                StreamWriter zapisz_uwagi_sztuki = new StreamWriter("uwagi_temp.txt");

                zapisz_uwagi_sztuki.Write("");

                zapisz_uwagi_sztuki.Close();

            

            return str_pobrane_uwagi;
           
        }

        public void zapisz(int nr_sztuki, string metry, string klient, string str_uwagi)
        {
            try
            {
                int int_sprawdz_ID_stabilizacja = 0;

                string str_nr_karta = "";


                if (!str_sprawdz_nr_karta.Equals("0"))
                {
                    str_nr_karta = str_sprawdz_nr_karta;
                }
                if (str_sprawdz_nr_karta.Equals("0"))
                {
                    zmien_status_na_stabilizacja();
                }

                


                polocz.Open();

                MySql.Data.MySqlClient.MySqlCommand sprawdz_ID_stabilizacja = new MySql.Data.MySqlClient.MySqlCommand("SELECT ID_stabilizacja FROM ETYKIETY_STABILIZACJA_TAB ORDER BY ID_stabilizacja DESC;", polocz);
                int_sprawdz_ID_stabilizacja = Convert.ToInt32(sprawdz_ID_stabilizacja.ExecuteScalar().ToString());



                MySql.Data.MySqlClient.MySqlCommand dodaj_wiersz = new MySql.Data.MySqlClient.MySqlCommand("INSERT INTO `ETYKIETY_STABILIZACJA_TAB`(`ID_stabilizacja`, `Nr_karta`, `Nr_parti`, `Artykul`, `Nr_sztuki`, `Metry`, `Kilogramy`, `Uwagi`, `Wzor`, `Data`, `Klient`) VALUES (\'" + (int_sprawdz_ID_stabilizacja + 1) + "\',\'" + str_nr_karta + "\',\'" + str_nr_parti_do_zapisania + "\',\'" + label_artykul_baza.Text.ToString() + "\',\'" + nr_sztuki + "\',\'" + metry + "\',\'" + str_kilogramy_do_druku + "\',\'" + str_uwagi + "\',\'" + label_nr_wzoru_na_etykiete.Text.ToString() + "\', \'" + pobierz_date_rozdzielona() + "\', \'" + klient + "\');", polocz);
                dodaj_wiersz.ExecuteNonQuery();

                polocz.Close();

            }catch(Exception)
            {
                polocz.Close();
                MessageBox.Show("Nie zapisano kolejnej sztuki", "Uwaga !", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }


        }

        private void button_drukuj_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox_odczyt_metrow.Text.ToString().Equals("0"))
                {
                    str_ktora_sztuka_do_druku = "";
                    d_kilogramy = 0.0;
                    str_metry_do_druku = "0";
                    str_metry_do_druku_temp = "0";
                    dodaj_sztuke_i_drukkuj_etykiete();
                }
                else
                {
                    reset_metrow();
                }

            }catch(Exception)
            {

            }
        }

        public string pobierz_date_rozdzielona()
        {
            DateTime data = DateTime.Now; ;
            string razem;
            string dzien;
            string miesiac;
            string rok;

            dzien = data.Day.ToString("D2");
            miesiac = data.Month.ToString("D2");
            rok = data.Year.ToString("D2");
            razem = rok + "-" + miesiac + "-" + dzien;

            return razem;
        }

        public void drukuj_etykieta()
        {
            if (!str_klient_do_druku_etykieta.Equals("PAKAITA") && !str_klient_do_druku_etykieta.Equals("FABRICANT") && !str_klient_do_druku_etykieta.Equals("TOMTEX") && !str_klient_do_druku_etykieta.Equals("TOMTEX/MATERACOWE") && !str_klient_do_druku_etykieta.Equals("TOMTEX BIS") && !str_klient_do_druku_etykieta.Equals("OGANES HMAJAK") && !str_klient_do_druku_etykieta.Equals("KNITTEX") && !str_klient_do_druku.Equals("KNITTEX"))
            {
               
               
                try
                {
                    printDialog_etykieta.Document = printDocument_etykieta;
                    printDocument_etykieta.DefaultPageSettings.Landscape = false;
                    printDocument_etykieta.Print();
                }
                catch (Exception) { }
                                                                                       
                 /*                                                                    
                printDocument_etykieta.DefaultPageSettings.Landscape = false;
                printPreviewDialog1.Document = printDocument_etykieta;
                printPreviewDialog1.ShowDialog();
                  */
                  
                
                                                                                                                                                                      
                 
            }

            if (str_klient_do_druku_etykieta.Equals("TOMTEX") || str_klient_do_druku_etykieta.Equals("TOMTEX/MATERACOWE") || str_klient_do_druku_etykieta.Equals("TOMTEX BIS"))
            {

                
                try
                {
                    printDialog_etykieta_TOMTEX.Document = printDocument_etykieta_TOMTEX;
                    //printDocument_etykieta.PrinterSettings.PrinterName = str_drukarka_do_etykiet;
                    printDocument_etykieta_TOMTEX.DefaultPageSettings.Landscape = false;
                    printDocument_etykieta_TOMTEX.Print();
                }
                catch (Exception) { }
                 
                 
                 
                
                 

                /*
                printDocument_etykieta_TOMTEX.DefaultPageSettings.Landscape = false;
                printPreviewDialog1.Document = printDocument_etykieta_TOMTEX;
                printPreviewDialog1.ShowDialog();
                 */
                 
                 
                 
                 
                 

            }

            if (str_klient_do_druku_etykieta.Equals("PAKAITA"))
            {
                            
                try
                {
                    printDialog_etykieta_Pakaita.Document = printDocument_etykieta_Pakaita;
                    //printDocument_etykieta.PrinterSettings.PrinterName = str_drukarka_do_etykiet;
                    printDocument_etykieta_Pakaita.DefaultPageSettings.Landscape = false;
                    printDocument_etykieta_Pakaita.Print();
                }
                catch (Exception) { }    
                 
                  
                
                /*
                printDocument_etykieta_Pakaita.DefaultPageSettings.Landscape = false;
                printPreviewDialog1.Document = printDocument_etykieta_Pakaita;
                printPreviewDialog1.Show();
                 */                            
                 
            }

            if (str_klient_do_druku_etykieta.Equals("KNITTEX") || str_klient_do_druku.Equals("KNITTEX"))
            {
                
                try
                {
                    printDialog_etykieta_Knittex.Document = printDocument_etykieta_Knittex;
                    printDocument_etykieta_Knittex.DefaultPageSettings.Landscape = false;
                    printDocument_etykieta_Knittex.Print();
                }
                catch (Exception) { }
                 
                 
                 
                /*
                printDocument_etykieta_Knittex.DefaultPageSettings.Landscape = false;
                printPreviewDialog1.Document = printDocument_etykieta_Knittex;
                printPreviewDialog1.ShowDialog();
                 */
                 
                 
                 
                 

            }

            if (str_klient_do_druku_etykieta.Equals("FABRICANT")) ////wcześniejsza nazwa firmy TORUSNET
            {
                int int_sprawdz_nr_artykulu = 0;

                int_sprawdz_nr_artykulu = label_artykul_baza.Text.ToString().IndexOf("P12");

                 if (int_sprawdz_nr_artykulu >= 0)
                 {


                
                     
                    try
                    {
                        printDialog_etykieta_Torusnet.Document = printDocument_etykieta_Torusnet;
                        printDocument_etykieta_Torusnet.DefaultPageSettings.Landscape = false;
                        printDocument_etykieta_Torusnet.Print();
                    }
                    catch (Exception) { }
                      
                

                    /*
                    printDocument_etykieta_Torusnet.DefaultPageSettings.Landscape = false;
                    printPreviewDialog1.Document = printDocument_etykieta_Torusnet;
                    printPreviewDialog1.ShowDialog();
                     */
                     
                     
                     
                }
                 else
                 {
                     
                     try
                     {
                         printDialog_etykieta.Document = printDocument_etykieta;                     
                         printDocument_etykieta.DefaultPageSettings.Landscape = false;
                         printDocument_etykieta.Print();
                     }
                     catch (Exception) { 
                     }                   
                     
                     /*        
                     printDocument_etykieta.DefaultPageSettings.Landscape = false;
                     printPreviewDialog1.Document = printDocument_etykieta;
                     printPreviewDialog1.ShowDialog(); 
                      */
                       
                           

                 }              

            }
            if (str_klient_do_druku_etykieta.Equals("OGANES HMAJAK"))
            {
                string str_artykul_hmajak = "";
                str_artykul_hmajak = label_artykul_baza.Text.ToString();
                

                if(str_artykul_hmajak.Equals("SILK POLIESTER"))
                {
                    
                    try
                    {
                        printDialog_etykieta_Oganes_PES.Document = printDocument_etykieta_Oganes_PES;
                        //printDocument_etykieta.PrinterSettings.PrinterName = str_drukarka_do_etykiet;
                        printDocument_etykieta_Oganes_PES.DefaultPageSettings.Landscape = false;
                        printDocument_etykieta_Oganes_PES.Print();
                    }
                    catch (Exception) { }
                     
                     
                    /*
                    printDocument_etykieta_Oganes_PES.DefaultPageSettings.Landscape = false;
                    printPreviewDialog1.Document = printDocument_etykieta_Oganes_PES;
                    printPreviewDialog1.Show();
                     */
                }
                else
                {
                    /////////////NORMALNA ETYKIETA///////////////////
                    try
                    {
                        printDialog_etykieta.Document = printDocument_etykieta;
                        printDocument_etykieta.DefaultPageSettings.Landscape = false;
                        printDocument_etykieta.Print();
                    }
                    catch (Exception) { }
                }

                


            }
             
             
        }



        private void printDocument_etykieta_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Graphics rysowanie = e.Graphics;
            Pen pioro = new Pen(System.Drawing.Color.Black);



            SolidBrush pedzel = new SolidBrush(Color.Black);
            Font czcionka_35 = new System.Drawing.Font("IDAutomationHC39M", 35, FontStyle.Bold);
            Font czcionka_30 = new System.Drawing.Font("IDAutomationHC39M", 30, FontStyle.Bold);
            Font czcionka_25 = new System.Drawing.Font("IDAutomationHC39M", 25, FontStyle.Bold);
            Font czcionka_26 = new System.Drawing.Font("IDAutomationHC39M", 26, FontStyle.Bold);
            Font czcionka_10 = new System.Drawing.Font("IDAutomationHC39M", 10, FontStyle.Bold);
            Font czcionka_15 = new System.Drawing.Font("IDAutomationHC39M", 15, FontStyle.Bold);
            Font czcionka_8 = new System.Drawing.Font("IDAutomationHC39M", 8);
            Font czcionka_8_bold = new System.Drawing.Font("IDAutomationHC39M", 8, FontStyle.Bold);
            Font czcionka_6_bold = new System.Drawing.Font("IDAutomationHC39M", 6, FontStyle.Bold);
            Font czcionka_6 = new System.Drawing.Font("IDAutomationHC39M", 6);
            Font czcionka_4 = new System.Drawing.Font("IDAutomationHC39M", 4);
            Font oFont = new System.Drawing.Font("IDAutomationHC39M", 10);

            SolidBrush black = new SolidBrush(Color.Black);
            SolidBrush White = new SolidBrush(Color.White);
            Pen czarny = new Pen(black);
            Pen pen_bialy = new Pen(White);

            int int_sparwdz_klienta = 0;
            int int_pozycja_cena = 0;
            string str_barcode = "";
            try
            {
                str_barcode = str_nr_wzoru_na_etykiete;
                
            }catch(Exception){
                str_barcode = "";
            }
            /*
            int_sparwdz_klienta = str_klient_do_druku.IndexOf("HMAJAK");

            if(int_sparwdz_klienta >= 0)
            {
                rysowanie.DrawImage(System.Drawing.Image.FromFile("metka_hmajak.png"), 0, 0, 397, 525);
            }
            else
            {
             */
                if (!str_klient_do_druku_etykieta.Equals("MARCZAK") && !str_klient_do_druku_etykieta.Equals("MIRAFO"))
                {
                    rysowanie.DrawImage(System.Drawing.Image.FromFile("metka_4.png"), 0, 0, 397, 525);
                }
                if (str_klient_do_druku_etykieta.Equals("MARCZAK"))
                {
                    rysowanie.DrawImage(System.Drawing.Image.FromFile("metka_marczak.png"), 0, 0, 397, 525);
                }
                if (str_klient_do_druku_etykieta.Equals("MIRAFO"))
                {
                    rysowanie.DrawImage(System.Drawing.Image.FromFile("metka_mirafo.png"), 0, 0, 397, 525);
                }

                /*
                if (str_klient_do_druku_etykieta.Equals("Damaz") || str_klient_do_druku_etykieta.Equals("DAMAZ"))
                {
                    rysowanie.DrawImage(System.Drawing.Image.FromFile("damaz_logo.png"), 110, 440, 177, 54);
                }
                 */
            //}

            ///////////////////////////drukowanie barcode //////////////////////////////////////
            if (!str_barcode.Equals(""))
            {
                Zen.Barcode.Code128BarcodeDraw barcode = Zen.Barcode.BarcodeDrawFactory.Code128WithChecksum;

                Point p = new Point(15, 10);  //-250

                try
                {
                    rysowanie.DrawImage(barcode.Draw(str_barcode, 50), p);
                }catch(Exception){

                }
            }
            
            /////////////////////koniec drukowania barcode ////////////////////////////////////////

            if (label_artykul_baza.Text.ToString().Equals("now by Hulsta # 156/21"))  ///drukowanie dla FABRICANT NR ARTYKULU singiel # B503/3L
            {
                rysowanie.DrawString("STRETCH 721", czcionka_15, black, 150, 78);
            }
            else
            {
                rysowanie.DrawString(label_artykul_baza.Text.ToString(), czcionka_15, black, 150, 78);
            }
            
            

            rysowanie.DrawString(str_sklad_dzianiny, czcionka_10, black, 175, 120);
            rysowanie.DrawString(textBox_nr_parti.Text.ToString(), czcionka_25, black, 170, 140);      
            rysowanie.DrawString(str_ktora_sztuka_do_druku, czcionka_25, black, 170, 180);

            //if (str_metry_do_druku.Equals("0")) { str_metry_do_druku = ""; }
            if (label_poprzedni_pomiar_metrow.Text.ToString().Equals("0") && flaga_edycja_metry_do_druku == false)
            { 
                str_metry_do_druku = "";
            }
            else
            {
               // str_metry_do_druku = label_poprzedni_pomiar_metrow.Text.ToString();
            }

            if (flaga_edycja_metry_do_druku == false)
            {
                rysowanie.DrawString(str_metry_do_druku, czcionka_25, black, 170, 217);
            }
            if (flaga_edycja_metry_do_druku == true)
            {
                rysowanie.DrawString(str_metry_do_druku_edycja, czcionka_25, black, 170, 217);
            }

            if (!str_klient_do_druku_etykieta.Equals("SONTEX"))
            {
                if (str_kilogramy_do_druku.Equals("0")) { str_kilogramy_do_druku = ""; }
                rysowanie.DrawString(str_kilogramy_do_druku, czcionka_25, black, 170, 253);
            }

            if (!str_kolor_do_druku.Equals("POPRAWA") && !str_kolor_do_druku.Equals("REKLAMACJA"))
            {
                rysowanie.DrawString(str_kolor_do_druku, czcionka_15, black, 150, 300);
            }
            else
            {
                rysowanie.DrawString("", czcionka_15, black, 150, 300);
            }
            

            int int_sprawdz_przefarb = 0;
            int_sprawdz_przefarb = str_nr_koloru_an_farb_do_druku.IndexOf("PRZEFARB");
            if(int_sprawdz_przefarb >= 0)
            {
                str_nr_koloru_an_farb_do_druku = "";
            }
            int_sprawdz_przefarb = str_nr_koloru_an_farb_do_druku.IndexOf("POPRAWA");
            if (int_sprawdz_przefarb >= 0)
            {
                str_nr_koloru_an_farb_do_druku = "";
            }
            int_sprawdz_przefarb = str_nr_koloru_an_farb_do_druku.IndexOf("REKLAMACJA");
            if (int_sprawdz_przefarb >= 0)
            {
                str_nr_koloru_an_farb_do_druku = "";
            }
            rysowanie.DrawString(str_nr_koloru_an_farb_do_druku, czcionka_15, black, 180, 340);


           // int_pozycja_cena = str_nr_wzoru_na_etykiete.IndexOf("cena");
            if(flaga_cena == true)
            {
               // str_nr_wzoru_na_etykiete = str_nr_wzoru_na_etykiete.Substring(0, int_pozycja_cena-1);
                rysowanie.DrawString("CENA", czcionka_25, black, 155, 453);
            }
            

            if (flaga_dwa_wzory == false)
            {
                rysowanie.DrawString(str_nr_wzoru_na_etykiete, czcionka_15, black, 170, 377);
            }
            if (flaga_dwa_wzory == true)
            {
                rysowanie.DrawString(str_nr_wzoru_na_etykiete, czcionka_8_bold, black, 170, 377);
            }

            string str_temp_klient_do_druku_etykieta = "";

            str_temp_klient_do_druku_etykieta = str_klient_do_druku_etykieta;

            if (str_klient_do_druku_etykieta.Equals("MARCZAK"))
            {
                str_temp_klient_do_druku_etykieta = "";
            }
            if (str_klient_do_druku_etykieta.Equals("MIRAFO"))
            {
                str_temp_klient_do_druku_etykieta = "";
            }
            if (str_klient_do_druku_etykieta.Equals("RICH-S"))
            {
                str_temp_klient_do_druku_etykieta = "";
            }
            if (str_klient_do_druku_etykieta.Equals("SONTEX"))
            {
                str_temp_klient_do_druku_etykieta = "";
            }
            if (str_klient_do_druku_etykieta.Equals("TOMTEX/PDP"))
            {
                str_temp_klient_do_druku_etykieta = "LUBATEX";
            }


            if (!str_temp_klient_do_druku_etykieta.Equals("ANFARB"))
            {
                rysowanie.DrawString(str_temp_klient_do_druku_etykieta, czcionka_15, black, 130, 415);
            }

            pen_bialy.Width = 25;
            
            if (str_klient_do_druku_etykieta.Equals("Damaz"))
            {
                rysowanie.DrawLine(pen_bialy, 130, 425, 350, 425);
            }
             
            if (str_klient_do_druku_etykieta.Equals("DAMAZ"))
            {
                rysowanie.DrawLine(pen_bialy, 130, 425, 350, 425);
            }

            if (label_artykul_baza.Text.ToString().Equals("singiel # B 503/3L"))  ///drukowanie dla FABRICANT NR ARTYKULU singiel # B503/3L
            {
                str_temp_klient_do_druku_etykieta = "FABRICANT";
                rysowanie.DrawString(str_temp_klient_do_druku_etykieta, czcionka_15, black, 130, 415);
            }
            if (label_artykul_baza.Text.ToString().Equals("now by Hulsta # 156/21"))  ///drukowanie dla FABRICANT NR ARTYKULU singiel # B503/3L
            {
                str_klient_do_druku_edycja = "LUBATEX";
               // rysowanie.DrawString(str_temp_klient_do_druku_etykieta, czcionka_15, black, 130, 415);
            }

            try
            {
                int int_dllugosc_str = 0;
                if (flaga_edycja_metry_do_druku == false)
                {
                    int_dllugosc_str = str_klient_do_druku.Length;

                    if (int_dllugosc_str > 15)
                    {
                        str_klient_do_druku = str_klient_do_druku.Substring(0, 14);
                    }

                    rysowanie.DrawString(str_klient_do_druku, czcionka_15, black, 130, 415);
                }
                if (flaga_edycja_metry_do_druku == true)    ////////drukuj gdy szutka jest edytowana
                {
                    int_dllugosc_str = str_klient_do_druku_edycja.Length;

                    if (int_dllugosc_str > 15)
                    {
                        str_klient_do_druku_edycja = str_klient_do_druku_edycja.Substring(0, 14);
                    }

                    rysowanie.DrawString(str_klient_do_druku_edycja, czcionka_15, black, 130, 415);
                }
            }catch(Exception)
            {
                 rysowanie.DrawString("", czcionka_15, black, 130, 415);
            }
            

            
            string str_raport = "";
       

            try
            {

                polocz.Open();

                
                MySql.Data.MySqlClient.MySqlCommand raport = new MySql.Data.MySqlClient.MySqlCommand("SELECT  Raport FROM ETYKIETY_GRAM_SZER_TAB WHERE Nr_karta = \'" + str_sprawdz_nr_karta + "\' AND Nr_parti = \'" + str_nr_parti_do_zapisania + "\' AND Artykul = \'" + label_artykul_baza.Text.ToString() + "\' AND Data = \'" + str_data_parti + "\';", polocz);
                str_raport = raport.ExecuteScalar().ToString();


                polocz.Close();
            }
            catch (Exception)
            {
                polocz.Close();
                
                str_raport = "";
                
            }

            if (str_klient_do_druku_etykieta.Equals("MARCZAK") && (!str_raport.Equals("0") && !str_raport.Equals("")))
            {
                rysowanie.DrawString("Raport: "+str_raport+" cm", czcionka_15, black, 130, 450);
            }
            

               

        }

        private void dataGridView_sztuki_Click(object sender, EventArgs e)
        {
            try
            {
                int int_klikniety_wiersz = 0;
              
                bool flaga_usuwanie_wiersza = true;

                int_klikniety_wiersz = dataGridView_sztuki.CurrentCell.RowIndex;
                

                string str_ID_stabilizacja = dataGridView_sztuki.Rows[int_klikniety_wiersz].Cells[0].Value.ToString();

                int int_ilosc_wierszy = dataGridView_sztuki.RowCount;

                
                flaga_usuwanie_wiersza = true;
                Edycja_sztuki edycja_sztuki = new Edycja_sztuki(str_ID_stabilizacja, tab_wzory, flaga_usuwanie_wiersza, str_data_parti);
                edycja_sztuki.ShowDialog();

                wyswietl_sztuki_parti();

                if(edycja_sztuki.flaga_drukuj_etykiete == true)
                {
                    flaga_edycja_metry_do_druku = true;
                    str_ktora_sztuka_do_druku = dataGridView_sztuki.Rows[int_klikniety_wiersz].Cells[1].Value.ToString();
                    str_metry_do_druku_edycja = dataGridView_sztuki.Rows[int_klikniety_wiersz].Cells[2].Value.ToString();
                    str_kilogramy_do_druku = dataGridView_sztuki.Rows[int_klikniety_wiersz].Cells[3].Value.ToString();
                    str_nr_wzoru_na_etykiete = dataGridView_sztuki.Rows[int_klikniety_wiersz].Cells[5].Value.ToString();
                    str_klient_do_druku_edycja = dataGridView_sztuki.Rows[int_klikniety_wiersz].Cells[7].Value.ToString();

                    if(str_nr_wzoru_na_etykiete.Length > 18)
                    {
                        str_nr_wzoru_na_etykiete = str_nr_wzoru_na_etykiete.Substring(0, 18);
                    }

                    drukuj_etykieta();

                    flaga_edycja_metry_do_druku = false;
                    
                }
            }catch(Exception){

            }
        }

        private void button_wzory_Click(object sender, EventArgs e)
        {

            label_nr_wzoru_na_etykiete.Font = new Font(FontFamily.GenericSansSerif, 24, FontStyle.Bold);
            flaga_dwa_wzory = false;
            flaga_wzory = true;
            button_wzory.BackColor = Color.Green;
            button_wzory.ForeColor = Color.Black;

            button_kupony.BackColor = System.Drawing.SystemColors.ActiveBorder;
            button_kupony.ForeColor = System.Drawing.SystemColors.ButtonFace;

            label_nr_wzoru_na_etykiete.Text = "";
            label_kupony_klient.Text = "";

        }

        private void button_kupony_Click(object sender, EventArgs e)
        {
            label_nr_wzoru_na_etykiete.Font = new Font(FontFamily.GenericSansSerif, 24, FontStyle.Bold);
            flaga_dwa_wzory = false;
            flaga_wzory = false;
            button_kupony.BackColor = Color.Green;
            button_kupony.ForeColor = Color.Black;

            button_wzory.BackColor = System.Drawing.SystemColors.ActiveBorder;
            button_wzory.ForeColor = System.Drawing.SystemColors.ButtonFace;

            label_nr_wzoru_na_etykiete.Text = "KUPONY";

        }

        private void label_nr_wzoru_na_etykiete_Click(object sender, EventArgs e)
        {
            Zdjecie_pelny_ekran zdjecie_pelny_ekran = new Zdjecie_pelny_ekran(label_nr_wzoru_na_etykiete.Text.ToString());
            zdjecie_pelny_ekran.ShowDialog();
        }

        private void ustawieniaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Ustawienia ustawienia = new Ustawienia();
            ustawienia.ShowDialog();
        }

        private void button_nr_parti_do_gory_Click(object sender, EventArgs e)
        {
            try
            {

                int pozycja_scrolling = dataGridView_partie.FirstDisplayedScrollingRowIndex;
                if (pozycja_scrolling > 0)
                {

                    dataGridView_partie.FirstDisplayedScrollingRowIndex = pozycja_scrolling - 1;
                }
            }
            catch (Exception) { }
        }

        private void button_nr_parti_dol_Click(object sender, EventArgs e)
        {
            try
            {
                int ilosc_wierszy = dataGridView_partie.RowCount;

                int pozycja_scrolling = dataGridView_partie.FirstDisplayedScrollingRowIndex;

                if (pozycja_scrolling < ilosc_wierszy)
                {

                    dataGridView_partie.FirstDisplayedScrollingRowIndex = pozycja_scrolling + 1;
                }
            }
            catch (Exception) { }
        }

        private void button_do_gory_Click(object sender, EventArgs e)
        {
            try
            {

                int pozycja_scrolling = dataGridView_sztuki.FirstDisplayedScrollingRowIndex;
                if (pozycja_scrolling > 0)
                {

                    dataGridView_sztuki.FirstDisplayedScrollingRowIndex = pozycja_scrolling - 1;
                }
            }
            catch (Exception) { }

        }

        private void button_na_dol_Click(object sender, EventArgs e)
        {
            try
            {
                int ilosc_wierszy = dataGridView_sztuki.RowCount;

                int pozycja_scrolling = dataGridView_sztuki.FirstDisplayedScrollingRowIndex;

                if (pozycja_scrolling < ilosc_wierszy)
                {

                    dataGridView_sztuki.FirstDisplayedScrollingRowIndex = pozycja_scrolling + 1;
                }
            }
            catch (Exception) { }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Gramatura_szerokosc gramatura_szerokosc = new Gramatura_szerokosc(str_sprawdz_nr_karta, str_nr_parti_do_zapisania, label_artykul_baza.Text.ToString(), str_data_parti);
            gramatura_szerokosc.ShowDialog();

            if(gramatura_szerokosc.flaga_drukuj_kurczliwosci == true)
            {
                drukuj_kurczliwosci();
            }
        }

        private void button_uwagi_Click(object sender, EventArgs e)
        {
            
            flaga_uwagi = false;
            Uwagi_do_sztuki uwagi_do_sztuki = new Uwagi_do_sztuki(flaga_uwagi, "");
            uwagi_do_sztuki.ShowDialog();
         

        }

        private void button_dodaj_wzor_Click(object sender, EventArgs e)
        {
            Dodaj_wzor dodaj_wzor = new Dodaj_wzor(tab_wzory, label_nr_wzoru_na_etykiete.Text.ToString());
            dodaj_wzor.ShowDialog();

            if(dodaj_wzor.flaga_dodano_wzor == true)
            {
                label_nr_wzoru_na_etykiete.Font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold);
                //label_nr_wzoru_na_etykiete.Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
                label_nr_wzoru_na_etykiete.Text = dodaj_wzor.richTextBox1.Text.ToString();

                flaga_dwa_wzory = dodaj_wzor.flaga_dodano_wzor;
            }
        }

        private void button_zapamietaj_wzor_Click(object sender, EventArgs e)
        {
            if(flaga_zapamietaj_wzor == false)
            {
                button_zapamietaj_wzor.BackColor = Color.Green;
                flaga_zapamietaj_wzor = true;
                button_zapamietaj_wzor.Visible = false;
                button_zapamietaj_wzor_ok.Visible = true;
            }       
            
        }

        private void button_zapamietaj_wzor_ok_Click(object sender, EventArgs e)
        {
            if (flaga_zapamietaj_wzor == true)
            {
               
                flaga_zapamietaj_wzor = false;
                button_zapamietaj_wzor.Visible = true;
                button_zapamietaj_wzor_ok.Visible = false;

                button_zapamietaj_wzor.BackColor = Color.Green;

                int int_ilosc_wierszy = dataGridView_sztuki.RowCount;
                int int_ktora_sztuka = 0;

                if(int_ilosc_wierszy > 1)
                {
                    int_ktora_sztuka = Convert.ToInt32(dataGridView_sztuki.Rows[int_ilosc_wierszy-2].Cells[1].Value.ToString());
                    int_ktora_sztuka++;
                    button_zapamietaj_wzor.Text = Convert.ToString(int_ktora_sztuka);
                }
                else
                {
                    button_zapamietaj_wzor.Text = "1";
                }
      
            }
        }

        

        public void czysc_karte_obiegowa()
        {
            try
            {
                label_artykul_baza.Text = "";
                label_klient_baza.Text = "";
                label_nr_parti_baza.Text = "";
                label_ilosc_sztuk_baza.Text = "";
                label_waga_baza.Text = "";
                label_art_baza.Text = "";
                label_nr_art_baza.Text = "";
                label_szer_stab_baza.Text = "";
                label_data_przyjecia_baza.Text = "";
                str_kolor_do_druku = "";
                str_nr_koloru_an_farb_do_druku = "";
                label_data_stabilizacji_baza.Text = "";
            }catch(Exception){

            }
        }
        public void info_karta_obiegowa(int int_ID_karta_nr)
        {
            string str_zapytania_SQL = "";
            int int_pozycja_kratka = 0;


            dataSet_temp.Clear();
            dataGridView_temp.DataSource = dataSet_temp;

            try
            {
                str_zapytania_SQL = "SELECT `Ilosc`, `Waga`, `Artykul`, `Numer_art`, `Data_przyjecia`, `Kolor` FROM `KARTA_TAB` WHERE  `ID_Karta_nr` = \'" + int_ID_karta_nr + "\';";
              
                polocz.Open();


                MySqlCommand info_karta = new MySqlCommand(str_zapytania_SQL, polocz);
                MySqlDataAdapter adapter_info_karta = new MySqlDataAdapter(info_karta);
                adapter_info_karta.Fill(dataSet_temp, "Lista_info_karta");
                dataGridView_temp.DataSource = dataSet_temp.Tables["Lista_info_karta"];

                polocz.Close();

            }
            catch (Exception)
            {
                polocz.Close();

            }

            try
            {
                label_ilosc_sztuk_baza.Text = dataGridView_temp.Rows[0].Cells[0].Value.ToString();
            }catch(Exception){
                label_ilosc_sztuk_baza.Text = "";
            }
            try
            {
                label_waga_baza.Text = dataGridView_temp.Rows[0].Cells[1].Value.ToString();
            }
            catch (Exception)
            {
                label_waga_baza.Text = "";
            }
            try
            {
                label_art_baza.Text = dataGridView_temp.Rows[0].Cells[2].Value.ToString();
            }
            catch (Exception)
            {
                label_art_baza.Text = "";
            }
            try
            {
                label_nr_art_baza.Text = dataGridView_temp.Rows[0].Cells[3].Value.ToString();
                label_artykul_baza.Text = label_nr_art_baza.Text;

                int_pozycja_kratka = label_artykul_baza.Text.ToString().IndexOf("singiel # B 503/3L");
                if(int_pozycja_kratka >= 0)
                {
                    label_artykul_baza.Text = label_artykul_baza.Text.ToString().Substring(0, 18);
                }

                
            }
            catch (Exception)
            {
                label_nr_art_baza.Text = "";
            }
            try
            {
                label_data_przyjecia_baza.Text = dataGridView_temp.Rows[0].Cells[4].Value.ToString();
            }
            catch (Exception)
            {
                label_data_przyjecia_baza.Text = "";
            }
            try
            {
                str_kolor_do_druku = dataGridView_temp.Rows[0].Cells[5].Value.ToString();
            }
            catch (Exception)
            {
                str_kolor_do_druku = "";
            }


            /*
            try
            {

                polocz.Open();

                MySql.Data.MySqlClient.MySqlCommand ilosc = new MySql.Data.MySqlClient.MySqlCommand("SELECT `Ilosc` FROM `KARTA_TAB` WHERE  `ID_Karta_nr` = \'" + int_ID_karta_nr + "\';", polocz);
                label_ilosc_sztuk_baza.Text = Convert.ToString(ilosc.ExecuteScalar().ToString());

                MySql.Data.MySqlClient.MySqlCommand waga = new MySql.Data.MySqlClient.MySqlCommand("SELECT `Waga` FROM `KARTA_TAB` WHERE  `ID_Karta_nr` = \'" + int_ID_karta_nr + "\';", polocz);
                label_waga_baza.Text = Convert.ToString(waga.ExecuteScalar().ToString());

                MySql.Data.MySqlClient.MySqlCommand artykul = new MySql.Data.MySqlClient.MySqlCommand("SELECT `Artykul` FROM `KARTA_TAB` WHERE  `ID_Karta_nr` = \'" + int_ID_karta_nr + "\';", polocz);
                label_art_baza.Text = Convert.ToString(artykul.ExecuteScalar().ToString());
                

                MySql.Data.MySqlClient.MySqlCommand nr_artykul = new MySql.Data.MySqlClient.MySqlCommand("SELECT `Numer_art` FROM `KARTA_TAB` WHERE  `ID_Karta_nr` = \'" + int_ID_karta_nr + "\';", polocz);
                label_nr_art_baza.Text = Convert.ToString(nr_artykul.ExecuteScalar().ToString());
                label_artykul_baza.Text = label_nr_art_baza.Text;

                MySql.Data.MySqlClient.MySqlCommand data_przyjecia = new MySql.Data.MySqlClient.MySqlCommand("SELECT `Data_przyjecia` FROM `KARTA_TAB` WHERE  `ID_Karta_nr` = \'" + int_ID_karta_nr + "\';", polocz);
                label_data_przyjecia_baza.Text = Convert.ToString(data_przyjecia.ExecuteScalar().ToString()).Substring(0, 10);

                MySql.Data.MySqlClient.MySqlCommand kolor = new MySql.Data.MySqlClient.MySqlCommand("SELECT `Kolor` FROM `KARTA_TAB` WHERE  `ID_Karta_nr` = \'" + int_ID_karta_nr + "\';", polocz);
                str_kolor_do_druku = kolor.ExecuteScalar().ToString();

                //MySql.Data.MySqlClient.MySqlCommand kolor_anfarb = new MySql.Data.MySqlClient.MySqlCommand("SELECT `Kolor_anfarb` FROM `KARTA_TAB` WHERE  `ID_Karta_nr` = \'" + int_ID_karta_nr + "\';", polocz);
                //str_nr_koloru_an_farb_do_druku = kolor_anfarb.ExecuteScalar().ToString();


                polocz.Close();


            }
            catch (Exception)
            {
                polocz.Close();
            }
             */

            dataSet_temp.Clear();
            dataGridView_temp.DataSource = dataSet_temp;

            try
            {
                polocz_MSSQL.Open();

                SqlCommand szer_stab_MSSQL = new SqlCommand("SELECT szer_stabilizacji_cm, nr_kol_klienta FROM dbo.karty where dbo.karty.karta_nr = \'" + int_ID_karta_nr + "\';", polocz_MSSQL);
                SqlDataAdapter adapter_szer_kolor = new SqlDataAdapter(szer_stab_MSSQL);
                adapter_szer_kolor.Fill(dataSet_temp, "Lista_szer_kol");
                dataGridView_temp.DataSource = dataSet_temp.Tables["Lista_szer_kol"];

                polocz_MSSQL.Close();

            }catch(Exception){
                polocz_MSSQL.Close();
            }

            try
            {
                label_szer_stab_baza.Text = dataGridView_temp.Rows[0].Cells[0].Value.ToString();

            }catch(Exception){
                label_szer_stab_baza.Text = "";
            }
            try
            {
                str_nr_koloru_an_farb_do_druku = dataGridView_temp.Rows[0].Cells[1].Value.ToString();

            }
            catch (Exception)
            {
                str_nr_koloru_an_farb_do_druku = "";
            }

            /*
            try
            {
                polocz_MSSQL.Open();

                SqlCommand szer_stab_MSSQL = new SqlCommand("SELECT szer_stabilizacji_cm FROM dbo.karty where dbo.karty.karta_nr = \'" + int_ID_karta_nr + "\';", polocz_MSSQL);
                label_szer_stab_baza.Text = Convert.ToString(szer_stab_MSSQL.ExecuteScalar());
                // zmien_status_MSSQL.ExecuteNonQuery();

                SqlCommand kolor_klienta_MSSQL = new SqlCommand("SELECT nr_kol_klienta FROM dbo.karty where dbo.karty.karta_nr = \'" + int_ID_karta_nr + "\';", polocz_MSSQL);
                str_nr_koloru_an_farb_do_druku = Convert.ToString(kolor_klienta_MSSQL.ExecuteScalar());

               // SqlCommand kolor_klienta_Torusnet = new SqlCommand("SELECT nr_kol_anfarb FROM dbo.karty where dbo.karty.karta_nr = \'" + int_ID_karta_nr + "\';", polocz_MSSQL);
               // str_nr_koloru_Torusnet = Convert.ToString(kolor_klienta_Torusnet.ExecuteScalar());
               


                polocz_MSSQL.Close();
            }
            catch (Exception)
            {
                polocz_MSSQL.Close();
            }
             */

        }

        private void button_rozwin_Click(object sender, EventArgs e)
        {
            groupBox_karta_obieg.Visible = true;
            button_rozwin.Visible = false;
            button_zwin.Visible = true;
        }

        private void button_zwin_Click(object sender, EventArgs e)
        {
            groupBox_karta_obieg.Visible = false;
            button_rozwin.Visible = true;
            button_zwin.Visible = false;
        }

        /*
        private void zamknijPomiaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            foreach (var process in Process.GetProcessesByName("Md150e"))
            {
                process.Kill();
            }
        }
        */
        
        private void button_zamknij_Click(object sender, EventArgs e)
        {
            this.Close();
            /*
            foreach (var process in Process.GetProcessesByName("Md150e"))
            {
                process.Kill();
            }
             */

            
            //timer_zamykanie.Tick += new EventHandler(czysc_pomiary);
           // timer_zamykanie.Interval = 600;
           // timer_zamykanie.Start();
             

            //czysc_pomiar_metry();
            
        }
         

        /*
        public void czysc_pomiary(Object myObject, EventArgs myEventArgs)
        {
            try
            {

                StreamWriter pomiar_metry_czysc = new StreamWriter(path_pomiar_metry);
                pomiar_metry_czysc.Write("");

                pomiar_metry_czysc.Close();

                this.Close();

            }
            catch (Exception)
            {

            }
        }
        */
        private void button_lista_zapamietanych_parti_dol_Click(object sender, EventArgs e)
        {
            button_lista_zapamietanych_parti_dol.Visible = false;
            button_lista_zapamietanych_parti_gora.Visible = true;
            dataGridView_rozwijana_lista_parti.Visible = true;
        }

        private void button_lista_zapamietanych_parti_gora_Click(object sender, EventArgs e)
        {
            button_lista_zapamietanych_parti_dol.Visible = true;
            button_lista_zapamietanych_parti_gora.Visible = false;
            dataGridView_rozwijana_lista_parti.Visible = false;

        }

        private void dataGridView_rozwijana_lista_parti_Click(object sender, EventArgs e)
        {
            try
            {
                int int_klikniety_wiersz;
                int_klikniety_wiersz = dataGridView_rozwijana_lista_parti.CurrentCell.RowIndex;

                textBox_nr_parti.Text = dataGridView_rozwijana_lista_parti.Rows[int_klikniety_wiersz].Cells[0].Value.ToString();


                if (!(textBox_nr_parti.Text.ToString().Equals("")))
                {

                    szukaj_nr_parti();

                }
            }catch(Exception){

            }


        }

        private void button_drukuj_specyfikacje_Click(object sender, EventArgs e)
        {
            try
            {
                if (!str_klient_do_druku_etykieta.Equals(""))
                {
                    drukuj_specyfikacje();
                }
            }catch(Exception)
            {

            }
        }

        public void drukuj_specyfikacje()
        {
            
            try
            {

                printDialog_specyfikacja.Document = printDocument_specyfikacja;               
                printDocument_specyfikacja.DefaultPageSettings.Landscape = false;
                printDocument_specyfikacja.Print();


            }
            catch (Exception) { }
             
             
           /*                                                   
           printDocument_specyfikacja.DefaultPageSettings.Landscape = false;
           printPreviewDialog1.Document = printDocument_specyfikacja;
           printPreviewDialog1.ShowDialog();
            */
             
                                     
             
        }

        public void drukuj_kurczliwosci()
        {
            
            try
            {

                printDialog_kurczliwosci.Document = printDocument_kurczliwosci;
                //printDocument_etykieta.PrinterSettings.PrinterName = str_drukarka_do_etykiet;
                printDocument_kurczliwosci.DefaultPageSettings.Landscape = false;
                printDocument_kurczliwosci.Print();


            }
            catch (Exception) { }
             
             


            /*
           printDocument_kurczliwosci.DefaultPageSettings.Landscape = false;
           printPreviewDialog1.Document = printDocument_kurczliwosci;
           printPreviewDialog1.ShowDialog();
             */
             
            

        }

        private void printDocument_specyfikacja_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Graphics rysowanie = e.Graphics;
            Pen pioro = new Pen(System.Drawing.Color.Black);

            SolidBrush pedzel = new SolidBrush(Color.Black);
            Font czcionka_35 = new System.Drawing.Font("IDAutomationHC39M", 35, FontStyle.Bold);
            Font czcionka_30 = new System.Drawing.Font("IDAutomationHC39M", 30, FontStyle.Bold);
            Font czcionka_25 = new System.Drawing.Font("IDAutomationHC39M", 25, FontStyle.Bold);
            Font czcionka_26 = new System.Drawing.Font("IDAutomationHC39M", 26, FontStyle.Bold);
            Font czcionka_10 = new System.Drawing.Font("IDAutomationHC39M", 10);
            Font czcionka_10_bold = new System.Drawing.Font("IDAutomationHC39M", 10, FontStyle.Bold);
            Font czcionka_14 = new System.Drawing.Font("IDAutomationHC39M", 14);
            Font czcionka_14_bold = new System.Drawing.Font("IDAutomationHC39M", 14, FontStyle.Bold);           
            Font czcionka_15 = new System.Drawing.Font("IDAutomationHC39M", 15, FontStyle.Bold);
            Font czcionka_8 = new System.Drawing.Font("IDAutomationHC39M", 8);
            Font czcionka_8_bold = new System.Drawing.Font("IDAutomationHC39M", 8, FontStyle.Bold);
            Font czcionka_6_bold = new System.Drawing.Font("IDAutomationHC39M", 6, FontStyle.Bold);
            Font czcionka_6 = new System.Drawing.Font("IDAutomationHC39M", 6);
            Font czcionka_4 = new System.Drawing.Font("IDAutomationHC39M", 4);

            SolidBrush black = new SolidBrush(Color.Black);
            SolidBrush White = new SolidBrush(Color.White);
            Pen czarny = new Pen(black);
            Pen White_Pen = new Pen(White);


            Point punkty_1 = new Point(10, 10);
            Point punkty_2 = new Point(30, 30);
            Point[] kwadrat = { punkty_1, punkty_2 };

            White_Pen.Width = 22;

            rysowanie.DrawString(str_klient_do_druku_etykieta, czcionka_15, black, 10, 10);

            
            rysowanie.DrawRectangle(White_Pen, 190, 10, 100, 15); ///biały kwadrat
            rysowanie.DrawString(str_nr_parti_do_zapisania, czcionka_15, black, 190, 10);

            
            rysowanie.DrawRectangle(White_Pen, 290, 10, 50, 15);
            
            rysowanie.DrawString(pobierz_date_rozdzielona(), czcionka_10, black, 280, 10);

            rysowanie.DrawString(label_artykul_baza.Text.ToString(), czcionka_15, black, 10, 30);

            czarny.Width = 2;
            rysowanie.DrawLine(czarny, 10, 55, 350, 55);

            rysowanie.DrawString("Nr.", czcionka_10, black, 10, 60);
            rysowanie.DrawString("Metry", czcionka_10, black, 50, 60);
            rysowanie.DrawString("kg.", czcionka_10, black, 100, 60);
            rysowanie.DrawString("Uwagi", czcionka_10, black, 160, 60);
            rysowanie.DrawString("Wzór", czcionka_10, black, 250, 60);

            czarny.Width = 1;
            rysowanie.DrawLine(czarny, 10, 80, 350, 80);

            int int_ilosc_sztuk = dataGridView_sztuki.RowCount;
            int int_kolejny_wiersz = 85;

            int int_sprawdz_nowa_linia =0;

            int int_suma_metry = 0;

            double d_suma_kg = 0.0;

            string str_suma_kg = "0";
            string str_uwagi;
            string str_wzor; 

            for (int i = 0; i < int_ilosc_sztuk-1; i++ )
            {
                /*
                if(i == 29)
                {
                    e.HasMorePages = true;
                }
                 */
               


                if ((int_ilosc_sztuk - 1) < 14)
                {
                    if (i < 9)
                    {
                        rysowanie.DrawString(dataGridView_sztuki.Rows[i].Cells[1].Value.ToString(), czcionka_14, black, 18, int_kolejny_wiersz);
                    }
                    else
                    {
                        rysowanie.DrawString(dataGridView_sztuki.Rows[i].Cells[1].Value.ToString(), czcionka_14, black, 10, int_kolejny_wiersz);
                    }
                    rysowanie.DrawString(dataGridView_sztuki.Rows[i].Cells[2].Value.ToString(), czcionka_14_bold, black, 50, int_kolejny_wiersz);

                    try
                    {
                        int_suma_metry += Convert.ToInt32(dataGridView_sztuki.Rows[i].Cells[2].Value.ToString());
                    }
                    catch
                    {
                        int_suma_metry += 0;
                    }

                    if (!dataGridView_sztuki.Rows[i].Cells[3].Value.ToString().Equals("0"))
                    {
                        rysowanie.DrawString(dataGridView_sztuki.Rows[i].Cells[3].Value.ToString(), czcionka_14_bold, black, 100, int_kolejny_wiersz);
                    }

                    try
                    {
                        d_suma_kg += Convert.ToDouble(dataGridView_sztuki.Rows[i].Cells[3].Value.ToString().Replace(".", ","));
                    }
                    catch
                    {
                        d_suma_kg += 0;
                    }

                    str_uwagi = dataGridView_sztuki.Rows[i].Cells[4].Value.ToString();
                    int_sprawdz_nowa_linia = str_uwagi.IndexOf("\n");
                    if (int_sprawdz_nowa_linia < 0)
                    {
                        rysowanie.DrawString(str_uwagi, czcionka_10, black, 160, int_kolejny_wiersz);
                    }
                    if(int_sprawdz_nowa_linia > 0)
                    {
                        rysowanie.DrawString(str_uwagi, czcionka_6, black, 160, int_kolejny_wiersz);
                    }

                    try
                    {
                        str_wzor = dataGridView_sztuki.Rows[i].Cells[5].Value.ToString();
                        int_sprawdz_nowa_linia = str_uwagi.IndexOf("\n");
                        if (int_sprawdz_nowa_linia < 0)
                        {
                            rysowanie.DrawString(str_wzor, czcionka_10, black, 250, int_kolejny_wiersz);
                        }
                        if (int_sprawdz_nowa_linia > 0)
                        {
                            rysowanie.DrawString(str_wzor, czcionka_6, black, 250, int_kolejny_wiersz);
                        }

                    }catch(Exception){

                        str_wzor = "";

                    }

                    int_kolejny_wiersz += 30;
                }
                //////////////////powyżej 14 szt pomniejszona czcionka/////////////////
                if ((int_ilosc_sztuk - 1) >= 14 && (int_ilosc_sztuk - 1) < 22)
                {
                    if (i < 9)
                    {
                        rysowanie.DrawString(dataGridView_sztuki.Rows[i].Cells[1].Value.ToString(), czcionka_10, black, 18, int_kolejny_wiersz);
                    }
                    else
                    {
                        rysowanie.DrawString(dataGridView_sztuki.Rows[i].Cells[1].Value.ToString(), czcionka_10, black, 10, int_kolejny_wiersz);
                    }
                    rysowanie.DrawString(dataGridView_sztuki.Rows[i].Cells[2].Value.ToString(), czcionka_10_bold, black, 50, int_kolejny_wiersz);
                    try
                    {
                        int_suma_metry += Convert.ToInt32(dataGridView_sztuki.Rows[i].Cells[2].Value.ToString());
                    }
                    catch
                    {
                        int_suma_metry += 0;
                    }
                    if (!dataGridView_sztuki.Rows[i].Cells[3].Value.ToString().Equals("0"))
                    {
                        rysowanie.DrawString(dataGridView_sztuki.Rows[i].Cells[3].Value.ToString(), czcionka_10_bold, black, 100, int_kolejny_wiersz);
                    }

                    try
                    {
                        d_suma_kg += Convert.ToDouble(dataGridView_sztuki.Rows[i].Cells[3].Value.ToString().Replace(".", ","));
                    }
                    catch
                    {
                        d_suma_kg += 0;
                    }

                    str_uwagi = dataGridView_sztuki.Rows[i].Cells[4].Value.ToString();
                    int_sprawdz_nowa_linia = str_uwagi.IndexOf("\n");
                    if (int_sprawdz_nowa_linia < 0)
                    {
                        if(str_uwagi.Length > 12)
                        {
                          str_uwagi = str_uwagi.Substring(0, 12);
                        }
                        rysowanie.DrawString(str_uwagi, czcionka_10, black, 160, int_kolejny_wiersz);
                    }
                    if (int_sprawdz_nowa_linia > 0)
                    {
                        rysowanie.DrawString(str_uwagi, czcionka_6, black, 160, int_kolejny_wiersz);
                    }

                    try
                    {
                        str_wzor = dataGridView_sztuki.Rows[i].Cells[5].Value.ToString();
                        int_sprawdz_nowa_linia = str_uwagi.IndexOf("\n");
                        if (int_sprawdz_nowa_linia < 0)
                        {
                            rysowanie.DrawString(str_wzor, czcionka_10, black, 250, int_kolejny_wiersz);
                        }
                        if (int_sprawdz_nowa_linia > 0)
                        {
                            rysowanie.DrawString(str_wzor, czcionka_6, black, 250, int_kolejny_wiersz);
                        }

                    }
                    catch (Exception)
                    {

                        str_wzor = "";

                    }

                    int_kolejny_wiersz += 20;
                }

                //////////////////powyżej 21 szt pomniejszona czcionka/////////////////
                if ((int_ilosc_sztuk - 1) >= 22 && (int_ilosc_sztuk - 1) <= 28)
                {
                    if (i < 9)
                    {
                        rysowanie.DrawString(dataGridView_sztuki.Rows[i].Cells[1].Value.ToString(), czcionka_8, black, 18, int_kolejny_wiersz);
                    }
                    else
                    {
                        rysowanie.DrawString(dataGridView_sztuki.Rows[i].Cells[1].Value.ToString(), czcionka_8, black, 10, int_kolejny_wiersz);
                    }
                    rysowanie.DrawString(dataGridView_sztuki.Rows[i].Cells[2].Value.ToString(), czcionka_8_bold, black, 50, int_kolejny_wiersz);
                    try
                    {
                        int_suma_metry += Convert.ToInt32(dataGridView_sztuki.Rows[i].Cells[2].Value.ToString());
                    }
                    catch
                    {
                        int_suma_metry += 0;
                    }
                    if (!dataGridView_sztuki.Rows[i].Cells[3].Value.ToString().Equals("0"))
                    {
                        rysowanie.DrawString(dataGridView_sztuki.Rows[i].Cells[3].Value.ToString(), czcionka_8_bold, black, 100, int_kolejny_wiersz);
                    }

                    try
                    {
                        d_suma_kg += Convert.ToDouble(dataGridView_sztuki.Rows[i].Cells[3].Value.ToString().Replace(".", ","));
                    }
                    catch
                    {
                        d_suma_kg += 0;
                    }

                    str_uwagi = dataGridView_sztuki.Rows[i].Cells[4].Value.ToString();
                    int_sprawdz_nowa_linia = str_uwagi.IndexOf("\n");
                    if (int_sprawdz_nowa_linia < 0)
                    {
                        if (str_uwagi.Length > 15)
                        {
                            str_uwagi = str_uwagi.Substring(0, 15);
                        }
                        rysowanie.DrawString(str_uwagi, czcionka_8, black, 160, int_kolejny_wiersz);
                    }
                    if (int_sprawdz_nowa_linia > 0)
                    {
                        rysowanie.DrawString(str_uwagi, czcionka_4, black, 160, int_kolejny_wiersz);
                    }

                    try
                    {
                        str_wzor = dataGridView_sztuki.Rows[i].Cells[5].Value.ToString();
                        int_sprawdz_nowa_linia = str_uwagi.IndexOf("\n");
                        if (int_sprawdz_nowa_linia < 0)
                        {
                            rysowanie.DrawString(str_wzor, czcionka_8, black, 250, int_kolejny_wiersz);
                        }
                        if (int_sprawdz_nowa_linia > 0)
                        {
                            rysowanie.DrawString(str_wzor, czcionka_4, black, 250, int_kolejny_wiersz);
                        }

                    }
                    catch (Exception)
                    {

                        str_wzor = "";

                    }

                    int_kolejny_wiersz += 15;
                }

                //////////////////powyżej 28 szt pomniejszona czcionka/////////////////
                if ((int_ilosc_sztuk - 1) >= 29)
                {
                    if (i < 9)
                    {
                        rysowanie.DrawString(dataGridView_sztuki.Rows[i].Cells[1].Value.ToString(), czcionka_6, black, 18, int_kolejny_wiersz);
                    }
                    else
                    {
                        rysowanie.DrawString(dataGridView_sztuki.Rows[i].Cells[1].Value.ToString(), czcionka_6, black, 10, int_kolejny_wiersz);
                    }
                    rysowanie.DrawString(dataGridView_sztuki.Rows[i].Cells[2].Value.ToString(), czcionka_6_bold, black, 50, int_kolejny_wiersz);
                    try
                    {
                        int_suma_metry += Convert.ToInt32(dataGridView_sztuki.Rows[i].Cells[2].Value.ToString());
                    }
                    catch
                    {
                        int_suma_metry += 0;
                    }
                    if (!dataGridView_sztuki.Rows[i].Cells[3].Value.ToString().Equals("0"))
                    {
                        rysowanie.DrawString(dataGridView_sztuki.Rows[i].Cells[3].Value.ToString(), czcionka_6_bold, black, 100, int_kolejny_wiersz);
                    }

                    try
                    {
                        d_suma_kg += Convert.ToDouble(dataGridView_sztuki.Rows[i].Cells[3].Value.ToString().Replace(".", ","));
                    }
                    catch
                    {
                        d_suma_kg += 0;
                    }

                    str_uwagi = dataGridView_sztuki.Rows[i].Cells[4].Value.ToString();
                    int_sprawdz_nowa_linia = str_uwagi.IndexOf("\n");
                    if (int_sprawdz_nowa_linia < 0)
                    {
                        if (str_uwagi.Length > 20)
                        {
                            str_uwagi = str_uwagi.Substring(0, 20);
                        }
                        rysowanie.DrawString(str_uwagi, czcionka_6, black, 160, int_kolejny_wiersz);
                    }
                    if (int_sprawdz_nowa_linia > 0)
                    {
                        rysowanie.DrawString(str_uwagi, czcionka_4, black, 160, int_kolejny_wiersz);
                    }

                    try
                    {
                        str_wzor = dataGridView_sztuki.Rows[i].Cells[5].Value.ToString();
                        int_sprawdz_nowa_linia = str_uwagi.IndexOf("\n");
                        if (int_sprawdz_nowa_linia < 0)
                        {
                            rysowanie.DrawString(str_wzor, czcionka_6, black, 250, int_kolejny_wiersz);
                        }
                        if (int_sprawdz_nowa_linia > 0)
                        {
                            rysowanie.DrawString(str_wzor, czcionka_4, black, 250, int_kolejny_wiersz);
                        }

                    }
                    catch (Exception)
                    {

                        str_wzor = "";

                    }

                    int_kolejny_wiersz += 10;
                }
            }

            string str_gramatura = "";
            string str_szerokosc = "";
            string str_raport = "";
            string str_uwagi_ogolne = "";
            dataSet_temp.Clear();
            dataGridView_temp.DataSource = dataSet_temp;
            try
            {

                polocz.Open();

                MySqlCommand parametry_dzianiny = new MySqlCommand("SELECT  Gramatura, Szerokosc, Raport, Uwagi_ogolne FROM ETYKIETY_GRAM_SZER_TAB WHERE Nr_karta = \'" + str_sprawdz_nr_karta + "\' AND Nr_parti = \'" + str_nr_parti_do_zapisania + "\' AND Artykul = \'" + label_artykul_baza.Text.ToString() + "\' AND Data = \'" + str_data_parti + "\';", polocz);
                MySqlDataAdapter adapter_parametry_dzianiny = new MySqlDataAdapter(parametry_dzianiny);
                adapter_parametry_dzianiny.Fill(dataSet_temp, "Lista_parametry_dzianiny");
                dataGridView_temp.DataSource = dataSet_temp.Tables["Lista_parametry_dzianiny"];



                polocz.Close();
            }
            catch (Exception)
            {
                polocz.Close();               
            }
            try
            {

               // polocz.Open();

               // MySql.Data.MySqlClient.MySqlCommand gramatura = new MySql.Data.MySqlClient.MySqlCommand("SELECT  Gramatura FROM ETYKIETY_GRAM_SZER_TAB WHERE Nr_karta = \'" + str_sprawdz_nr_karta + "\' AND Nr_parti = \'" + str_nr_parti_do_zapisania + "\' AND Artykul = \'" + label_artykul_baza.Text.ToString() + "\' AND Data = \'" + str_data_parti + "\';", polocz);
                str_gramatura = dataGridView_temp.Rows[0].Cells[0].Value.ToString();

                //MySql.Data.MySqlClient.MySqlCommand szerokosc = new MySql.Data.MySqlClient.MySqlCommand("SELECT  Szerokosc FROM ETYKIETY_GRAM_SZER_TAB WHERE Nr_karta = \'" + str_sprawdz_nr_karta + "\' AND Nr_parti = \'" + str_nr_parti_do_zapisania + "\' AND Artykul = \'" + label_artykul_baza.Text.ToString() + "\' AND Data = \'" + str_data_parti + "\';", polocz);
                str_szerokosc = dataGridView_temp.Rows[0].Cells[1].Value.ToString();

                //MySql.Data.MySqlClient.MySqlCommand raport = new MySql.Data.MySqlClient.MySqlCommand("SELECT  Raport FROM ETYKIETY_GRAM_SZER_TAB WHERE Nr_karta = \'" + str_sprawdz_nr_karta + "\' AND Nr_parti = \'" + str_nr_parti_do_zapisania + "\' AND Artykul = \'" + label_artykul_baza.Text.ToString() + "\' AND Data = \'" + str_data_parti + "\';", polocz);
                str_raport = dataGridView_temp.Rows[0].Cells[2].Value.ToString();

                //MySql.Data.MySqlClient.MySqlCommand uwagi_ogolne = new MySql.Data.MySqlClient.MySqlCommand("SELECT  Uwagi_ogolne FROM ETYKIETY_GRAM_SZER_TAB WHERE Nr_karta = \'" + str_sprawdz_nr_karta + "\' AND Nr_parti = \'" + str_nr_parti_do_zapisania + "\' AND Artykul = \'" + label_artykul_baza.Text.ToString() + "\' AND Data = \'" + str_data_parti + "\';", polocz);
                str_uwagi_ogolne = dataGridView_temp.Rows[0].Cells[3].Value.ToString();



                //polocz.Close();
            }
            catch (Exception)
            {
               // polocz.Close();
                str_gramatura = "";
                str_szerokosc = "";
                str_raport = "";
                str_uwagi_ogolne = "";

            }

            rysowanie.DrawString(str_uwagi_ogolne, czcionka_14, black, 10, 460);

            White_Pen.Width = 50;           
            rysowanie.DrawLine(White_Pen, 10, 530, 350, 530);
           // rysowanie.DrawRectangle(White_Pen, 10, 520, 340, 80); ///biały kwadrat
                                                                  ///
            str_suma_kg = d_suma_kg.ToString("F2");

            if (!str_klient_do_druku_etykieta.Equals("ROKIET"))
            {
                rysowanie.DrawString("Gram: " + str_gramatura + " g/m2   Szer: " + str_szerokosc + " cm", czcionka_14_bold, black, 10, 500);
                if(!str_raport.Equals("0"))
                {
                    rysowanie.DrawString("Rap: " + str_raport + " cm   Metry: " +int_suma_metry+ " m  Kg: "+str_suma_kg+" kg" , czcionka_10_bold, black, 10, 520);
                }
                else
                {
                    rysowanie.DrawString("Metry: " + int_suma_metry + " m  Kg: " + str_suma_kg + " kg", czcionka_14_bold, black, 10, 520);
                
                }
            }
            if (str_klient_do_druku_etykieta.Equals("ROKIET"))
            {
                //rysowanie.DrawString("Gram: " + str_gramatura + " g/m2   Szer: " + str_szerokosc + " cm", czcionka_14_bold, black, 10, 500);
                rysowanie.DrawString("Metry razem: " + int_suma_metry + " m", czcionka_14_bold, black, 10, 520);
            }


        }

        private void printDocument_etykieta_Pakaita_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {

            Graphics rysowanie = e.Graphics;
            Pen pioro = new Pen(System.Drawing.Color.Black);



            SolidBrush pedzel = new SolidBrush(Color.Black);
            Font czcionka_35 = new System.Drawing.Font("IDAutomationHC39M", 35, FontStyle.Bold);
            Font czcionka_30 = new System.Drawing.Font("IDAutomationHC39M", 30, FontStyle.Bold);
            Font czcionka_25 = new System.Drawing.Font("IDAutomationHC39M", 25, FontStyle.Bold);
            Font czcionka_26 = new System.Drawing.Font("IDAutomationHC39M", 26, FontStyle.Bold);
            Font czcionka_10 = new System.Drawing.Font("IDAutomationHC39M", 10, FontStyle.Bold);
            Font czcionka_15 = new System.Drawing.Font("IDAutomationHC39M", 15, FontStyle.Bold);
            Font czcionka_8 = new System.Drawing.Font("IDAutomationHC39M", 8);
            Font czcionka_8_bold = new System.Drawing.Font("IDAutomationHC39M", 8, FontStyle.Bold);
            Font czcionka_6_bold = new System.Drawing.Font("IDAutomationHC39M", 6, FontStyle.Bold);
            Font czcionka_6 = new System.Drawing.Font("IDAutomationHC39M", 6);
            Font czcionka_4 = new System.Drawing.Font("IDAutomationHC39M", 4);

            SolidBrush black = new SolidBrush(Color.Black);
            SolidBrush White = new SolidBrush(Color.White);
            Pen czarny = new Pen(black);

                    
            rysowanie.DrawImage(System.Drawing.Image.FromFile("metka_pakaita.png"), 0, 0, 397, 525);

            rysowanie.DrawString(textBox_nr_parti.Text.ToString(), czcionka_25, black, 170, 100);

            int int_pozycja = 0;
            string str_artykul = label_artykul_baza.Text.ToString();
            int_pozycja = str_artykul.IndexOf("#");
            try
            {
                str_artykul = str_artykul.Substring(0, int_pozycja);
            }catch(Exception)
            {
                str_artykul = "";
            }

            rysowanie.DrawString(str_artykul, czcionka_25, black, 190, 150);

            string str_nr_parti = label_artykul_baza.Text.ToString();
            int_pozycja = str_nr_parti.IndexOf("(");
            try
            {
                str_nr_parti = str_nr_parti.Substring(int_pozycja, str_nr_parti.Length - int_pozycja);
            }catch(Exception){
                str_nr_parti = "";
            }
            int_pozycja = str_nr_parti.IndexOf(")");
            try
            {
                str_nr_parti = str_nr_parti.Substring(0, int_pozycja+1);
            }
            catch (Exception)
            {
                str_nr_parti = "";
            }
             
            
            rysowanie.DrawString(str_nr_parti+"-"+str_ktora_sztuka_do_druku, czcionka_25, black, 180, 200);
            //rysowanie.DrawString(str_nr_parti + "-", czcionka_25, black, 170, 200);


            if (str_metry_do_druku.Equals("0")) { str_metry_do_druku = ""; }
            rysowanie.DrawString(str_metry_do_druku, czcionka_25, black, 200, 305);

            if (str_kilogramy_do_druku.Equals("0")) { str_kilogramy_do_druku = ""; }
            rysowanie.DrawString(str_kilogramy_do_druku, czcionka_25, black, 200, 355);


            rysowanie.DrawString(label_nr_wzoru_na_etykiete.Text.ToString(), czcionka_15, black, 20, 420);
            
        }

        

        private void button_uwagi_ogolne_Click(object sender, EventArgs e)
        {           
            Uwagi_ogolne uwagi_ogolne = new Uwagi_ogolne(str_sprawdz_nr_karta, str_nr_parti_do_zapisania, label_artykul_baza.Text.ToString(), str_data_parti);
            uwagi_ogolne.ShowDialog();
        }

        private void printDocument_kurczliwosci_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {

            Graphics rysowanie = e.Graphics;
            Pen pioro = new Pen(System.Drawing.Color.Black);

            SolidBrush pedzel = new SolidBrush(Color.Black);
            Font czcionka_35 = new System.Drawing.Font("IDAutomationHC39M", 35, FontStyle.Bold);
            Font czcionka_30 = new System.Drawing.Font("IDAutomationHC39M", 30, FontStyle.Bold);
            Font czcionka_25 = new System.Drawing.Font("IDAutomationHC39M", 25, FontStyle.Bold);
            Font czcionka_26 = new System.Drawing.Font("IDAutomationHC39M", 26, FontStyle.Bold);
            Font czcionka_10 = new System.Drawing.Font("IDAutomationHC39M", 10);
            Font czcionka_10_bold = new System.Drawing.Font("IDAutomationHC39M", 10, FontStyle.Bold);
            Font czcionka_14 = new System.Drawing.Font("IDAutomationHC39M", 14);
            Font czcionka_14_bold = new System.Drawing.Font("IDAutomationHC39M", 14, FontStyle.Bold);
            Font czcionka_15 = new System.Drawing.Font("IDAutomationHC39M", 15, FontStyle.Bold);
            Font czcionka_8 = new System.Drawing.Font("IDAutomationHC39M", 8);
            Font czcionka_8_bold = new System.Drawing.Font("IDAutomationHC39M", 8, FontStyle.Bold);
            Font czcionka_6_bold = new System.Drawing.Font("IDAutomationHC39M", 6, FontStyle.Bold);
            Font czcionka_6 = new System.Drawing.Font("IDAutomationHC39M", 6);
            Font czcionka_4 = new System.Drawing.Font("IDAutomationHC39M", 4);

            SolidBrush black = new SolidBrush(Color.Black);
            SolidBrush White = new SolidBrush(Color.White);
            Pen czarny = new Pen(black);
            Pen White_Pen = new Pen(White);


            Point punkty_1 = new Point(10, 10);
            Point punkty_2 = new Point(30, 30);
            Point[] kwadrat = { punkty_1, punkty_2 };

            White_Pen.Width = 22;

            rysowanie.DrawString(str_klient_do_druku_etykieta, czcionka_15, black, 10, 10);


            rysowanie.DrawRectangle(White_Pen, 190, 10, 100, 15); ///biały kwadrat
            rysowanie.DrawString(str_nr_parti_do_zapisania, czcionka_15, black, 190, 10);


            rysowanie.DrawRectangle(White_Pen, 290, 10, 50, 15);

            rysowanie.DrawString(pobierz_date_rozdzielona(), czcionka_10, black, 280, 10);

            rysowanie.DrawString(label_artykul_baza.Text.ToString(), czcionka_15, black, 10, 30);

            string str_nazwa_artykul = "";

            try
            {
                polocz.Open();

                MySql.Data.MySqlClient.MySqlCommand gramatura = new MySql.Data.MySqlClient.MySqlCommand("SELECT Artykul FROM `KARTA_TAB` WHERE ID_Karta_nr = \'" + str_sprawdz_nr_karta + "\';", polocz);
                str_nazwa_artykul = Convert.ToString(gramatura.ExecuteScalar().ToString());

                polocz.Close();
            }catch(Exception)
            {
                polocz.Close();
                str_nazwa_artykul = "";
            }

            rysowanie.DrawString("Artykuł: "+str_nazwa_artykul, czcionka_15, black, 10, 50);

            czarny.Width = 2;
            rysowanie.DrawLine(czarny, 10, 75, 350, 75);

            

            

            string str_gramatura = "";
            string str_szerokosc = "";
            string str_raport = "";
            string str_uwagi_ogolne = "";

            try
            {

                polocz.Open();

                MySql.Data.MySqlClient.MySqlCommand gramatura = new MySql.Data.MySqlClient.MySqlCommand("SELECT  Gramatura FROM ETYKIETY_GRAM_SZER_TAB WHERE Nr_karta = \'" + str_sprawdz_nr_karta + "\' AND Nr_parti = \'" + str_nr_parti_do_zapisania + "\' AND Artykul = \'" + label_artykul_baza.Text.ToString() + "\' AND Data = \'" + str_data_parti + "\';", polocz);
                str_gramatura = Convert.ToString(gramatura.ExecuteScalar().ToString());

                MySql.Data.MySqlClient.MySqlCommand szerokosc = new MySql.Data.MySqlClient.MySqlCommand("SELECT  Szerokosc FROM ETYKIETY_GRAM_SZER_TAB WHERE Nr_karta = \'" + str_sprawdz_nr_karta + "\' AND Nr_parti = \'" + str_nr_parti_do_zapisania + "\' AND Artykul = \'" + label_artykul_baza.Text.ToString() + "\' AND Data = \'" + str_data_parti + "\';", polocz);
                str_szerokosc = szerokosc.ExecuteScalar().ToString();

                MySql.Data.MySqlClient.MySqlCommand raport = new MySql.Data.MySqlClient.MySqlCommand("SELECT  Raport FROM ETYKIETY_GRAM_SZER_TAB WHERE Nr_karta = \'" + str_sprawdz_nr_karta + "\' AND Nr_parti = \'" + str_nr_parti_do_zapisania + "\' AND Artykul = \'" + label_artykul_baza.Text.ToString() + "\' AND Data = \'" + str_data_parti + "\';", polocz);
                str_raport = raport.ExecuteScalar().ToString();

                MySql.Data.MySqlClient.MySqlCommand uwagi_ogolne = new MySql.Data.MySqlClient.MySqlCommand("SELECT  Uwagi_ogolne FROM ETYKIETY_GRAM_SZER_TAB WHERE Nr_karta = \'" + str_sprawdz_nr_karta + "\' AND Nr_parti = \'" + str_nr_parti_do_zapisania + "\' AND Artykul = \'" + label_artykul_baza.Text.ToString() + "\' AND Data = \'" + str_data_parti + "\';", polocz);
                str_uwagi_ogolne = uwagi_ogolne.ExecuteScalar().ToString();



                polocz.Close();
            }
            catch (Exception)
            {
                polocz.Close();
                str_gramatura = "";
                str_szerokosc = "";
                str_raport = "";
                str_uwagi_ogolne = "";

            }

            rysowanie.DrawString("Gram: " + str_gramatura + " g/m2", czcionka_25, black, 10, 150);
            rysowanie.DrawString("Szer: " + str_szerokosc + " cm", czcionka_25, black, 10, 200);
            rysowanie.DrawString("Rap: " + str_raport + " cm", czcionka_25, black, 10, 250);
           
           
            //rysowanie.DrawString("Gram: " + str_gramatura + " g/m2   Szer: " + str_szerokosc + " cm", czcionka_14_bold, black, 10, 500);
            


        }

        private void printDocument_etykieta_Torusnet_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Graphics rysowanie = e.Graphics;
            Pen pioro = new Pen(System.Drawing.Color.Black);



            SolidBrush pedzel = new SolidBrush(Color.Black);
            Font czcionka_35 = new System.Drawing.Font("IDAutomationHC39M", 35, FontStyle.Bold);
            Font czcionka_30 = new System.Drawing.Font("IDAutomationHC39M", 30, FontStyle.Bold);
            Font czcionka_25 = new System.Drawing.Font("IDAutomationHC39M", 25, FontStyle.Bold);
            Font czcionka_26 = new System.Drawing.Font("IDAutomationHC39M", 26, FontStyle.Bold);
            Font czcionka_10 = new System.Drawing.Font("IDAutomationHC39M", 10, FontStyle.Bold);
            Font czcionka_14 = new System.Drawing.Font("IDAutomationHC39M", 14, FontStyle.Bold);
            Font czcionka_12 = new System.Drawing.Font("IDAutomationHC39M", 12, FontStyle.Bold);
           
            Font czcionka_8 = new System.Drawing.Font("IDAutomationHC39M", 8);
            Font czcionka_8_bold = new System.Drawing.Font("IDAutomationHC39M", 8, FontStyle.Bold);
            Font czcionka_6_bold = new System.Drawing.Font("IDAutomationHC39M", 6, FontStyle.Bold);
            Font czcionka_6 = new System.Drawing.Font("IDAutomationHC39M", 6);
            Font czcionka_4 = new System.Drawing.Font("IDAutomationHC39M", 4);

            SolidBrush black = new SolidBrush(Color.Black);
            SolidBrush White = new SolidBrush(Color.White);
            Pen czarny = new Pen(black);


            rysowanie.DrawImage(System.Drawing.Image.FromFile("metka_torusnet.png"), 0, 0, 397, 525);


            rysowanie.DrawString(label_art_baza.Text.ToString(), czcionka_10, black, 200, 235);
            rysowanie.DrawString(str_sklad_dzianiny, czcionka_10, black, 200, 258);
            rysowanie.DrawString(str_kolor_do_druku, czcionka_14, black, 200, 280);
            rysowanie.DrawString(label_szer_stab_baza.Text.ToString()+" cm", czcionka_14, black, 200, 302);
            rysowanie.DrawString(textBox_nr_parti.Text.ToString(), czcionka_14, black, 200, 326);
            rysowanie.DrawString(str_ktora_sztuka_do_druku, czcionka_14, black, 200, 350);

            /*
            double d_kilogramy_brutto = 0.0;
            double d_kilogramy_netto = 0.0;
            string str_kilogramy_brutto = "0";

            str_kilogramy_do_druku = str_kilogramy_do_druku.Replace(".", ",");
            try
            {
                d_kilogramy_brutto = Convert.ToDouble(str_kilogramy_do_druku) + 0.5;
                //str_kilogramy_brutto = Convert.ToString(d_kilogramy_brutto);
                str_kilogramy_brutto = d_kilogramy_brutto.ToString("F2");
            }
            catch (Exception) { str_kilogramy_brutto = "0"; }
            
            rysowanie.DrawString(str_kilogramy_brutto + " kg", czcionka_14, black, 200, 373);
            try
            {
                
                d_kilogramy_netto = Convert.ToDouble(str_kilogramy_do_druku);
                str_kilogramy_do_druku = d_kilogramy_netto.ToString("F2");

            }catch(Exception){ str_kilogramy_do_druku = "0";}
            

            rysowanie.DrawString(str_kilogramy_do_druku + " kg", czcionka_14, black, 200, 395);
             */
            rysowanie.DrawString(str_metry_do_druku + " m", czcionka_14, black, 200, 416);


            
            
        }

        private void textBox_odczyt_metrow_TextChanged(object sender, EventArgs e)
        {

        }

        private void printDocument_etykieta_TOMTEX_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Graphics rysowanie = e.Graphics;
            Pen pioro = new Pen(System.Drawing.Color.Black);



            SolidBrush pedzel = new SolidBrush(Color.Black);
            Font czcionka_35 = new System.Drawing.Font("IDAutomationHC39M", 35, FontStyle.Bold);
            Font czcionka_30 = new System.Drawing.Font("IDAutomationHC39M", 30, FontStyle.Bold);
            Font czcionka_25 = new System.Drawing.Font("IDAutomationHC39M", 25, FontStyle.Bold);
            Font czcionka_26 = new System.Drawing.Font("IDAutomationHC39M", 26, FontStyle.Bold);
            Font czcionka_20 = new System.Drawing.Font("IDAutomationHC39M", 20, FontStyle.Bold);
            Font czcionka_18 = new System.Drawing.Font("IDAutomationHC39M", 18, FontStyle.Bold);
            Font czcionka_10 = new System.Drawing.Font("IDAutomationHC39M", 10, FontStyle.Bold);
            Font czcionka_15 = new System.Drawing.Font("IDAutomationHC39M", 15, FontStyle.Bold);
            Font czcionka_8 = new System.Drawing.Font("IDAutomationHC39M", 8);
            Font czcionka_8_bold = new System.Drawing.Font("IDAutomationHC39M", 8, FontStyle.Bold);
            Font czcionka_6_bold = new System.Drawing.Font("IDAutomationHC39M", 6, FontStyle.Bold);
            Font czcionka_6 = new System.Drawing.Font("IDAutomationHC39M", 6);
            Font czcionka_4 = new System.Drawing.Font("IDAutomationHC39M", 4);

            SolidBrush black = new SolidBrush(Color.Black);
            SolidBrush White = new SolidBrush(Color.White);
            Pen czarny = new Pen(black);

            string str_artykul = label_artykul_baza.Text.ToString();

            if (str_artykul.Equals("FROTTE"))
            {
                rysowanie.DrawImage(System.Drawing.Image.FromFile("metka_tomtex_bis.png"), 0, 0, 397, 525);
            }
            else
            {
                rysowanie.DrawImage(System.Drawing.Image.FromFile("metka_tomtex.png"), 0, 0, 397, 525);
            }

            rysowanie.DrawString(textBox_nr_parti.Text.ToString(), czcionka_15, black, 15, 15);

            int int_pozycja = 0;
            
            /*
            int_pozycja = str_artykul.IndexOf("#");
            try
            {
                str_artykul = str_artykul.Substring(0, int_pozycja);
            }
            catch (Exception)
            {
                str_artykul = "";
            }
             */
            if(str_artykul.Length > 17)
            {
                str_artykul = str_artykul.Substring(0, 17);
            }
            rysowanie.DrawString(str_artykul, czcionka_18, black, 60, 255);
            rysowanie.DrawString(str_sklad_dzianiny, czcionka_15, black, 60, 285);
            rysowanie.DrawString(str_gramatura_do_druku, czcionka_18, black, 60, 310);
            rysowanie.DrawString(str_szerokosc_do_druku, czcionka_18, black, 60, 340);

            
            //rysowanie.DrawString(str_nr_parti + "-", czcionka_25, black, 170, 200);


            if (str_metry_do_druku.Equals("0")) { str_metry_do_druku = ""; }
            rysowanie.DrawString(str_metry_do_druku, czcionka_25, black, 100, 450);

           // if (str_kilogramy_do_druku.Equals("0")) { str_kilogramy_do_druku = ""; }
           // rysowanie.DrawString(str_kilogramy_do_druku, czcionka_25, black, 100, 470);


           // rysowanie.DrawString(label_nr_wzoru_na_etykiete.Text.ToString(), czcionka_15, black, 20, 420);
        }

        private void printDocument_etykieta_Oganes_PES_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Graphics rysowanie = e.Graphics;
            Pen pioro = new Pen(System.Drawing.Color.Black);



            SolidBrush pedzel = new SolidBrush(Color.Black);
            Font czcionka_35 = new System.Drawing.Font("IDAutomationHC39M", 35, FontStyle.Bold);
            Font czcionka_30 = new System.Drawing.Font("IDAutomationHC39M", 30, FontStyle.Bold);
            Font czcionka_25 = new System.Drawing.Font("IDAutomationHC39M", 25, FontStyle.Bold);
            Font czcionka_26 = new System.Drawing.Font("IDAutomationHC39M", 26, FontStyle.Bold);
            Font czcionka_10 = new System.Drawing.Font("IDAutomationHC39M", 10, FontStyle.Bold);
            Font czcionka_14 = new System.Drawing.Font("IDAutomationHC39M", 14, FontStyle.Bold);
            Font czcionka_12 = new System.Drawing.Font("IDAutomationHC39M", 12, FontStyle.Bold);

            Font czcionka_8 = new System.Drawing.Font("IDAutomationHC39M", 8);
            Font czcionka_8_bold = new System.Drawing.Font("IDAutomationHC39M", 8, FontStyle.Bold);
            Font czcionka_6_bold = new System.Drawing.Font("IDAutomationHC39M", 6, FontStyle.Bold);
            Font czcionka_6 = new System.Drawing.Font("IDAutomationHC39M", 6);
            Font czcionka_4 = new System.Drawing.Font("IDAutomationHC39M", 4);

            SolidBrush black = new SolidBrush(Color.Black);
            SolidBrush White = new SolidBrush(Color.White);
            Pen czarny = new Pen(black);


            rysowanie.DrawImage(System.Drawing.Image.FromFile("metka_OGANES_PES.png"), 0, 0, 397, 525);


            //rysowanie.DrawString(label_art_baza.Text.ToString(), czcionka_10, black, 200, 235);
           // rysowanie.DrawString(str_sklad_dzianiny, czcionka_10, black, 200, 258);
           // rysowanie.DrawString(str_kolor_do_druku, czcionka_14, black, 200, 280);
           // rysowanie.DrawString(label_szer_stab_baza.Text.ToString() + " cm", czcionka_14, black, 200, 302);
           // rysowanie.DrawString(textBox_nr_parti.Text.ToString(), czcionka_14, black, 200, 326);
            rysowanie.DrawString(str_ktora_sztuka_do_druku, czcionka_30, black, 195, 132);

            /*
            double d_kilogramy_brutto = 0.0;
            double d_kilogramy_netto = 0.0;
            string str_kilogramy_brutto = "0";

            str_kilogramy_do_druku = str_kilogramy_do_druku.Replace(".", ",");
            try
            {
                d_kilogramy_brutto = Convert.ToDouble(str_kilogramy_do_druku) + 0.5;
                //str_kilogramy_brutto = Convert.ToString(d_kilogramy_brutto);
                str_kilogramy_brutto = d_kilogramy_brutto.ToString("F2");
            }
            catch (Exception) { str_kilogramy_brutto = "0"; }
            
            rysowanie.DrawString(str_kilogramy_brutto + " kg", czcionka_14, black, 200, 373);
            try
            {
                
                d_kilogramy_netto = Convert.ToDouble(str_kilogramy_do_druku);
                str_kilogramy_do_druku = d_kilogramy_netto.ToString("F2");

            }catch(Exception){ str_kilogramy_do_druku = "0";}
            

            rysowanie.DrawString(str_kilogramy_do_druku + " kg", czcionka_14, black, 200, 395);
             */
            rysowanie.DrawString(str_metry_do_druku, czcionka_30, black, 255, 191);



        }

        private void resetPomiarMetrówToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Ustawienia_serial_port ustawienia_serial_port = new Ustawienia_serial_port();
            ustawienia_serial_port.ShowDialog();
        }



        public void uruchamianie_watku(Object myObject, EventArgs myEventArgs)
        {

            uruchom_automatycznie = new Thread(new ThreadStart(this.watek_pobieranie_metrow));
            uruchom_automatycznie.Start();

        }
        private void watek_pobieranie_metrow()
        {
            if (!serialPort1.IsOpen)
            {
                try
                {
                    //odczyt_ustawien();
                    //serialPort1.PortName = str_port_Name;
                    //serialPort1.Open();
                    // sprawdz_port();
                    otwieranie_serialPort();
                }
                catch (Exception)
                {
                   // MessageBox.Show("Nie można połączyc z licznikiem.\n Pomiar nie będzie mozliwy !!!", "Uwaga !!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }


            delegate_odczyt_modbus odczytaj_modbus = new delegate_odczyt_modbus(this.odczyt_modbus);
           
            Invoke(odczytaj_modbus);
        }

        int value_temp = 0;
        int int_wartosc_70 = 0;
        double d_wartosc = 0.00;
        double procent = 0.00;
        double d_wartosc_temp = 0.00;

        private delegate void delegate_odczyt_modbus();
        public void odczyt_modbus()
        {
            try
            {

                ////////////Odczytu stanu licznika//////////////
                byte slaveAddress = 1;  //numer urządzenia
                byte functionCode = 3;   //Function
                ushort startAddress = 0;  //Starting Adress
                ushort numberOfPoints = 2;  //Quantity of Registers


                
                byte[] frame = this.REadHoldingRegister(slaveAddress, functionCode, startAddress, numberOfPoints);

                //byte[] frame = this.REadHoldingRegister_RAMKA_RECZNIE();

               // textBox1.Text = this.Display(frame);
                serialPort1.Write(frame, 0, frame.Length);
                Thread.Sleep(100);


                if (serialPort1.BytesToRead >= 5)
                {

                    frame = new byte[serialPort1.BytesToRead];

                    serialPort1.Read(frame, 0, frame.Length);

                   // textBox_wysylania.Text = this.Display(frame);
                    // textBox_wysylania.Text = Convert.ToString(serialPort1.ReadLine());
                    //////////dopisane /////////////////////////////
                    string str_wartows_hex = "";
                    str_wartows_hex = this.Display(frame);

                   // richTextBox_display_frame.Text += textBox_wysylania.Text.ToString() + "\n";

                  //  str_wartows_hex = str_wartows_hex.Substring(6, 4);
                    string str_wartosc_cyklu_70 = "";
                    

                    str_wartosc_cyklu_70 = str_wartows_hex.Substring(13, 1);

                    str_wartows_hex = str_wartows_hex.Substring(6, 4);

                    int value = Convert.ToInt32(str_wartows_hex, 16);

                    
                    if (str_wartosc_cyklu_70.Equals("0"))
                    {
                        value_temp = 0;
                    }

                    //////dodaje do 70 /////////////
                    if(str_wartosc_cyklu_70.Equals("1"))
                    {
                        value_temp = 64523;
                    }                   
                    ////////////dodaje do 130 //////////
                    if (str_wartosc_cyklu_70.Equals("2"))
                    {
                        value_temp = 129046;
                    }
                    ////////////dodaje do 200 //////////
                    if (str_wartosc_cyklu_70.Equals("3"))
                    {
                        value_temp = 193569;
                    }
                    ////////////dodaje do 270 //////////
                    if (str_wartosc_cyklu_70.Equals("4"))
                    {
                        value_temp = 258092;
                    }
                    if (str_wartosc_cyklu_70.Equals("5"))
                    {
                        value_temp = 322615;
                    }
                    if (str_wartosc_cyklu_70.Equals("6"))
                    {
                        value_temp = 387138;
                    }
                    if (str_wartosc_cyklu_70.Equals("7"))
                    {
                        value_temp = 451661;
                    }
                    if (str_wartosc_cyklu_70.Equals("8"))
                    {
                        value_temp = 516184;
                    }
                    if (str_wartosc_cyklu_70.Equals("9"))
                    {
                        value_temp = 580707;
                    }
                    if (str_wartosc_cyklu_70.Equals("A"))
                    {
                        value_temp = 645230;
                    }
                    if (str_wartosc_cyklu_70.Equals("B"))
                    {
                        value_temp = 709753;
                    }
                    if (str_wartosc_cyklu_70.Equals("C"))
                    {
                        value_temp = 774276;
                    }
                    if (str_wartosc_cyklu_70.Equals("D"))
                    {
                        value_temp = 838799;
                    }
                    if (str_wartosc_cyklu_70.Equals("E"))
                    {
                        value_temp = 903322;
                    }
                     
                    if (str_wartosc_cyklu_70.Equals("F"))
                    {
                        value = 0;
                    }



                        value += value_temp;

                    d_wartosc = Convert.ToDouble(value)/1000;
                    procent = 0.07;
                    d_wartosc_temp = 0.00;
                    


                  //  textBox_pH_wynik.Text = Convert.ToString(d_wartosc);
                    //////////////zapisywanie metrow do druku po rescie na maszynie //////////////////////
                    if(d_wartosc >= 0 && d_wartosc < 1 && flaga_zapisz_metry_do_druku == true)
                    {
                       
                        try
                        {
                            if (klawiatura_numeryczna.flaga_metry_automat == false)
                            {
                                str_ktora_sztuka_do_druku = "";
                                d_kilogramy = 0.0;
                                str_metry_do_druku_temp = textBox_odczyt_metrow.Text.ToString();
                                dodaj_sztuke_i_drukkuj_etykiete();
                                value_temp = 0;

                                Thread.Sleep(1000);
                            }

                            if (klawiatura_numeryczna.flaga_metry_automat == true)  ///włączona klawiatura
                            {
                                str_ktora_sztuka_do_druku = "";
                                d_kilogramy = 0.0;
                                klawiatura_numeryczna.textBox__wartosc_cyfrowa.Text = textBox_odczyt_metrow.Text.ToString();
                                value_temp = 0;
                                Thread.Sleep(1000);
                                klawiatura_numeryczna.flaga_metry_automat = false;
                            }
                           
                        }
                        catch (Exception)
                        {

                        }

                        flaga_zapisz_metry_do_druku = false;
                    }
                    if(d_wartosc > 1)
                    {
                        flaga_zapisz_metry_do_druku = true;
                    }

                    if (d_wartosc < 320)
                    {
                        ///dodaj 7 % /////////////
                        d_wartosc_temp = d_wartosc * procent;
                        d_wartosc = d_wartosc + d_wartosc_temp;
                    }
                    else
                    {
                        procent = 0.076;
                        ///dodaj 7,5 % /////////////
                        d_wartosc_temp = d_wartosc * procent;
                        d_wartosc = d_wartosc + d_wartosc_temp;
                    }

                    textBox_odczyt_metrow.Text = d_wartosc.ToString("F0");
                    int_wartosc_70 = Convert.ToInt32(textBox_odczyt_metrow.Text.ToString());
                    /*
                    if(!textBox_odczyt_metrow.Text.ToString().Equals(str_metry_temp_do_sprawdzenia_zmiany) && (!textBox_odczyt_metrow.Text.ToString().Equals("0")))
                    {
                       textBox_odczyt_metrow.Text = policz_metry(textBox_odczyt_metrow.Text.ToString());
                    }
                     */

                    

                    ////////////////////////////////////////////////
                }

            }
            catch (Exception ex)
            {
                /*
                if (klawiatura_numeryczna.flaga_metry_automat == true)
                {
                    textBox_odczyt_metrow.Text = "20";
                    
                    klawiatura_numeryczna.textBox__wartosc_cyfrowa.Text = textBox_odczyt_metrow.Text.ToString();
                }
                 */
                serialPort1.Close();


            }

        }
        private byte[] REadHoldingRegister(byte slaveAddress, byte functionCode, ushort startAddress, ushort numberOfPoints)
        {
            byte[] frame = new byte[8];  //Total 8 Bytes.
            frame[0] = slaveAddress;  //Slave Adress.
            frame[1] = functionCode;  //Function.
            frame[2] = (byte)(startAddress >> 8);  //Starting Address Hi.
            frame[3] = (byte)startAddress;    //Starting Address Lo.
            frame[4] = (byte)(numberOfPoints >> 8); //Quantity of Registers Hi
            frame[5] = (byte)numberOfPoints;  //Quantity of Registers Lo.
            byte[] crc = this.CalculateCRC(frame); //Call function Calculattion
            frame[6] = crc[0];
            frame[7] = crc[1];
            return frame;
        }
        private byte[] CalculateCRC(byte[] frame)
        {
            byte[] result = new byte[2];
            ushort CRCFull = 0xFFFF;
            char CRCLSB;
            for (int i = 0; i < frame.Length - 2; i++)
            {
                CRCFull = (ushort)(CRCFull ^ frame[i]);

                for (int j = 0; j < 8; j++)
                {
                    CRCLSB = (Char)(CRCFull & 0x0001);
                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);

                    if (CRCLSB == 1)
                    {
                        CRCFull = (ushort)(CRCFull ^ 0xA001);
                    }
                }
            }

            result[1] = (byte)((CRCFull >> 8) & 0xff);
            result[0] = (byte)(CRCFull & 0xff);
            return result;
        }
        private string Display(byte[] frame)
        {
            ////////dopisane /////////////////////////////
            int i = 0;

           // richTextBox_display_result.Text = "";
            // richTextBox_display_frame.Text = "";
           // richTextBox_display_result.Text += "\n";
            // richTextBox_display_frame.Text += "\n";
            //////////////////////////////////////////////


            string result = string.Empty;
            foreach (byte item in frame)
            {
                result += string.Format("{0:X2}", item);


                //////////////////dopisane /////////////////////////
               // richTextBox_display_result.Text += Convert.ToString(i) + "- " + item + "\n";
                // richTextBox_display_frame.Text += Convert.ToString(i) + "- " + frame + "\n";

                i++;
                /////////////////////////////////////////////////////
            }
            return result;
        }


        private void reset_metrow()
        {
            ////////////Reset licznika - COUNTER_RESET//////////////
            byte slaveAddress = 01;  //numer urządzenia
            byte functionCode = 05;   //Function
            ushort startAddress = 136;  //Starting Adress
            ushort numberOfPoints = 0;  //Quantity of Registers



            byte[] frame = this.REadHoldingRegister_Reset(slaveAddress, functionCode, startAddress, numberOfPoints);


            //byte[] frame = this.REadHoldingRegister(slaveAddress, functionCode, startAddress, numberOfPoints);
           // textBox1.Text = this.Display(frame);
            serialPort1.Write(frame, 0, frame.Length);
            Thread.Sleep(1000);
            if (serialPort1.BytesToRead >= 5)
            {
                frame = new byte[serialPort1.BytesToRead];
                int rs = serialPort1.Read(frame, 0, frame.Length);
               // textBox_wysylania.Text = this.Display(frame);

                //////////dopisane /////////////////////////////
                string str_wartows_hex = "";
                str_wartows_hex = this.Display(frame);

               // richTextBox_display_frame.Text += "" + textBox_wysylania.Text.ToString() + "\n";

               //  str_wartows_hex = str_wartows_hex.Substring(6, 4);

               // textBox_odczyt_metrow.Text = str_wartows_hex;

                // int value = Convert.ToInt32(str_wartows_hex, 16);

                // double d_wartosc = Convert.ToDouble(value) / 100;

                // textBox_pH_wynik.Text = Convert.ToString(d_wartosc);

                // textBox_pH_wynik.Text = d_wartosc.ToString("F2");

                ////////////////////////////////////////////////
            }

        }
        private byte[] REadHoldingRegister_Reset(byte slaveAddress, byte functionCode, ushort startAddress, ushort numberOfPoints)
        {
            byte[] frame = new byte[8];  //Total 8 Bytes.
            frame[0] = slaveAddress;  //Slave Adress.
            frame[1] = functionCode;  //Function.
            frame[2] = (byte)(19);  //Starting Address Hi.

            //frame[2] = (byte)(startAddress >> 8);  //Starting Address Hi.
            frame[3] = (byte)startAddress;    //Starting Address Lo.

            frame[4] = (byte)(255); //Quantity of Registers Hi
            //frame[4] = (byte)(numberOfPoints >> 8); //Quantity of Registers Hi
            frame[5] = (byte)numberOfPoints;  //Quantity of Registers Lo.

            byte[] crc = this.CalculateCRC(frame); //Call function Calculattion
            frame[6] = crc[0];
            frame[7] = crc[1];
            return frame;
        }

        private void button_reset_metrow_Click(object sender, EventArgs e)
        {
            label_poprzedni_pomiar_metrow.Text = "0";
            str_metry_do_druku = "0";
            textBox_odczyt_metrow.Text = "0";
            flaga_zapisz_metry_do_druku = false;
            reset_metrow();
        }

        private void printDocument_etykieta_Knittex_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Graphics rysowanie = e.Graphics;
            Pen pioro = new Pen(System.Drawing.Color.Black);



            SolidBrush pedzel = new SolidBrush(Color.Black);
            Font czcionka_35 = new System.Drawing.Font("IDAutomationHC39M", 35, FontStyle.Bold);
            Font czcionka_30 = new System.Drawing.Font("IDAutomationHC39M", 30, FontStyle.Bold);
            Font czcionka_25 = new System.Drawing.Font("IDAutomationHC39M", 25, FontStyle.Bold);
            Font czcionka_26 = new System.Drawing.Font("IDAutomationHC39M", 26, FontStyle.Bold);
            Font czcionka_10 = new System.Drawing.Font("IDAutomationHC39M", 10, FontStyle.Regular);
            Font czcionka_15 = new System.Drawing.Font("IDAutomationHC39M", 15, FontStyle.Bold);
            Font czcionka_8 = new System.Drawing.Font("IDAutomationHC39M", 8);
            Font czcionka_8_bold = new System.Drawing.Font("IDAutomationHC39M", 8, FontStyle.Bold);
            Font czcionka_6_bold = new System.Drawing.Font("IDAutomationHC39M", 6, FontStyle.Bold);
            Font czcionka_6 = new System.Drawing.Font("IDAutomationHC39M", 6);
            Font czcionka_4 = new System.Drawing.Font("IDAutomationHC39M", 4);

            SolidBrush black = new SolidBrush(Color.Black);
            SolidBrush White = new SolidBrush(Color.White);
            Pen czarny = new Pen(black);


            rysowanie.DrawImage(System.Drawing.Image.FromFile("metka_knittex.png"), 0, 0, 397, 525);

            

            rysowanie.DrawString(str_ktora_sztuka_do_druku, czcionka_10, black, 110, 155);
            rysowanie.DrawString(label_artykul_baza.Text.ToString(), czcionka_10, black, 110, 172);
            rysowanie.DrawString(label_szer_stab_baza.Text.ToString() + " cm", czcionka_10, black, 110, 188);

            if (!str_kolor_do_druku.Equals("POPRAWA"))
            {
                rysowanie.DrawString(str_kolor_do_druku, czcionka_10, black, 110, 205);
            }
            else
            {
                rysowanie.DrawString("", czcionka_10, black, 110, 205);
            }
            if (flaga_edycja_metry_do_druku == false)
            {
                rysowanie.DrawString(str_metry_do_druku + " m", czcionka_10, black, 110, 223);
            }
            if (flaga_edycja_metry_do_druku == true)
            {
                rysowanie.DrawString(str_metry_do_druku_edycja + " m", czcionka_10, black, 110, 223);
            }

            rysowanie.DrawString(textBox_nr_parti.Text.ToString(), czcionka_10, black, 110, 241);

            rysowanie.DrawString(pobierz_date(), czcionka_10, black, 90, 403);

            if (str_kilogramy_do_druku.Equals("0")) 
            { 
                str_kilogramy_do_druku = "";
            }
            else
            {
                rysowanie.DrawString(str_kilogramy_do_druku + " kg", czcionka_10, black, 110, 443);
            }
            

            

            // rysowanie.DrawString(str_artykul, czcionka_25, black, 190, 150);
            // rysowanie.DrawString(textBox_nr_parti.Text.ToString(), czcionka_25, black, 170, 100);
            /*
            int int_pozycja = 0;
            string str_artykul = label_artykul_baza.Text.ToString();
            int_pozycja = str_artykul.IndexOf("#");
            try
            {
                str_artykul = str_artykul.Substring(0, int_pozycja);
            }
            catch (Exception)
            {
                str_artykul = "";
            }

            rysowanie.DrawString(str_artykul, czcionka_25, black, 190, 150);

            string str_nr_parti = label_artykul_baza.Text.ToString();
            int_pozycja = str_nr_parti.IndexOf("(");
            try
            {
                str_nr_parti = str_nr_parti.Substring(int_pozycja, str_nr_parti.Length - int_pozycja);
            }
            catch (Exception)
            {
                str_nr_parti = "";
            }
            int_pozycja = str_nr_parti.IndexOf(")");
            try
            {
                str_nr_parti = str_nr_parti.Substring(0, int_pozycja + 1);
            }
            catch (Exception)
            {
                str_nr_parti = "";
            }


            rysowanie.DrawString(str_nr_parti + "-" + str_ktora_sztuka_do_druku, czcionka_25, black, 180, 200);
            //rysowanie.DrawString(str_nr_parti + "-", czcionka_25, black, 170, 200);


            if (str_metry_do_druku.Equals("0")) { str_metry_do_druku = ""; }
            rysowanie.DrawString(str_metry_do_druku, czcionka_25, black, 200, 305);

            if (str_kilogramy_do_druku.Equals("0")) { str_kilogramy_do_druku = ""; }
            rysowanie.DrawString(str_kilogramy_do_druku, czcionka_25, black, 200, 355);


            rysowanie.DrawString(label_nr_wzoru_na_etykiete.Text.ToString(), czcionka_15, black, 20, 420);
             */
        }

        private void button_stabilizacja_wstepna_Click(object sender, EventArgs e)
        {
            Stabilizacja_wstepna stabilizacja_wstepna = new Stabilizacja_wstepna(str_sprawdz_nr_karta, str_nr_parti_do_zapisania, label_artykul_baza.Text.ToString(), str_data_parti, str_klient_do_druku_etykieta);
            stabilizacja_wstepna.Show();
        }

        private void button_drukowanie_bez_drukowania_Click(object sender, EventArgs e)
        {
            
            if(flaga_drukowanie == true)
            {
                flaga_drukowanie = false;
                button_drukowanie_bez_drukowania.Visible = false;
                button_bez_drukowania.Visible = true;
            }
        }

        private void button_bez_drukowania_Click(object sender, EventArgs e)
        {
            if (flaga_drukowanie == false)
            {
                flaga_drukowanie = true;
                button_drukowanie_bez_drukowania.Visible = true;
                button_bez_drukowania.Visible = false;
            }

        }
        

        
    }
}
