using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.Odbc;
using Excel = Microsoft.Office.Interop.Excel;
using MetroFramework.Forms;
using MetroFramework;

namespace Slowakei
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {

        static string oradb = "Driver={Microsoft ODBC for Oracle}; Server=LH6000; Uid=cpc; Pwd=sabrina;";
        string datum1;
        OdbcConnection connection = new OdbcConnection(oradb);
        int pfadlaenge;



        public Form1()
        {
            InitializeComponent();
            connection.Open();
        }














        private void button2_Click_1(object sender, EventArgs e)
        {
            SaveFileDialog sw = new SaveFileDialog();

            sw.Filter = "txt files (*.txt)|*.txt";
            sw.FilterIndex = 2;
            sw.RestoreDirectory = true;

            if (sw.ShowDialog() == DialogResult.OK)
            {
                System.IO.StreamWriter _fs = new System.IO.StreamWriter(sw.FileName);
                for (int i = 0; i < listBox3.Items.Count; i++)
                {
                    _fs.WriteLine(listBox3.Items[i].ToString());
                }
                _fs.Close();
            }
        }









        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

            OdbcCommand mycommand;



            string vgnummer;
            string posnummer;
            string selected;
            string kundeid = "";
            string name1 = "";
            string postext = "";
            string kurztext = "";
            string materialartid = "";
            string artikelart = "";
            string artikelid = "";
            string vorgangsart = "";

            selected = listBox3.SelectedItem.ToString();
            vgnummer = selected.Substring(0, 8);
            posnummer = selected.Substring(9, 2);

            mycommand = new OdbcCommand("SELECT a.Kundeid, a.Name1, b.Postext, c.artikelid, d.kurztext, d.materialartid, d.Artikelart, a.vorgangsart FROM VG a, vgpo b, vgpoar c, ar d WHERE b.VORGANGID = ? and b.POSID = ? and b.vorgangid = a.vorgangid and b.posid=c.posid and c.uposid='10' and c.firmaid=a.firmaid and c.vorgangid=a.vorgangid and c.artikelid=d.artikelid", connection);
            mycommand.Parameters.Add("@vorgangid", OdbcType.VarChar).Value = vgnummer;
            mycommand.Parameters.Add("@posid", OdbcType.VarChar).Value = posnummer;


            OdbcDataReader dbreader = mycommand.ExecuteReader();

            while (dbreader.Read())
            {
                kundeid = dbreader.GetString(0);
                name1 = dbreader.GetString(1);
                postext = dbreader.GetString(2);
                artikelid = dbreader.GetString(3);
                kurztext = dbreader.GetString(4);
                materialartid = dbreader.GetString(5);
                artikelart = dbreader.GetString(6);
                vorgangsart = dbreader.GetString(7);

            }
            dbreader.Close();




            metroTextBox1.Text = kundeid;
            metroTextBox2.Text = name1;
            textBox3.Text = artikelid;
            textBox4.Text = kurztext;
            textBox5.Text = materialartid;
            label15.Text = artikelart;
            metroTextBox3.Text = vorgangsart;


        }




        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Exception Occured while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }



        }


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            connection.Close();


        }











        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string pfad;
            OdbcCommand mycommand;


            listBox1.Items.Clear();
            listBox2.Items.Clear();
            listBox3.Items.Clear();


            datum1 = dateTimePicker1.Value.Day.ToString() + "." + dateTimePicker1.Value.Month.ToString() + "." + dateTimePicker1.Value.Year.ToString();



            mycommand = new OdbcCommand("SELECT DISTINCT" +
                               " vgpoarst.vorgangid," +
                               " vgpoarst.posid," +
                               " vgpoarst.prodc," +
                               " vgpoarst.prodd," +
                               " vgpoarst.prodd1," +
                                "vgpoarst.prodp, " +
                                "vgpoarst.prodpi " +
                                "FROM " +
                                     "vgpoarst " +
                                "INNER JOIN vgpoarstks ON vgpoarst.firmaid = vgpoarstks.firmaid " +
                                   "AND vgpoarst.vorgangid = vgpoarstks.vorgangid " +
                                   "AND vgpoarst.posid = vgpoarstks.posid " +
                                   "AND vgpoarst.uposid = vgpoarstks.uposid " +
                                   "AND vgpoarst.stueckid = vgpoarstks.stueckid " +
                                "INNER JOIN cpc.vgpoar ON cpc.vgpoar.firmaid = vgpoarst.firmaid " +
                                                        " AND cpc.vgpoar.vorgangid = vgpoarst.vorgangid " +
                                                         "AND cpc.vgpoar.posid = vgpoarst.posid " +
                                                         "AND cpc.vgpoar.uposid = vgpoarst.uposid " +
                                "INNER JOIN cpc.vgpo ON cpc.vgpo.firmaid = cpc.vgpoar.firmaid " +
                                                       "AND cpc.vgpo.vorgangid = cpc.vgpoar.vorgangid " +
                                                      "AND cpc.vgpo.posid = cpc.vgpoar.posid " +
                                "INNER JOIN cpc.vg ON cpc.vg.firmaid = cpc.vgpo.firmaid " +
                                                     "AND cpc.vg.vorgangid = cpc.vgpo.vorgangid " +
                            "WHERE " +
                                "(" +
                                 " vgpoarst.prodp = 'P' or " +
                                 " vgpoarst.prodp is null or " +
                                 " nvl(vgpoar.produktionf, 'Oelde') = 'Frohmasco'" +
                                  ") " +
                               " AND vgpoarstks.scandatum = ? " +
                               " AND vgpoarstks.kostenstelleid = '1100' " +
                                "AND (vgpoar.materialartid = 'Aluminium' or vgpoar.materialartid = 'Kunststoff') " +
                                "AND vg.vorgangsart <> 'Reklamation' " +
                            "ORDER BY " +
                            "vgpoarst.vorgangid", connection);
            mycommand.Parameters.Add("@datum1", OdbcType.VarChar).Value = datum1;

            OdbcDataReader dbreader = mycommand.ExecuteReader();

            folderBrowserDialog1.SelectedPath = "W:\\TCN";
            folderBrowserDialog1.ShowDialog();
            pfad = folderBrowserDialog1.SelectedPath;
            pfad = pfad + "\\";
            pfadlaenge = pfad.Length;

            OdbcCommand mycommand1;
            mycommand1 = new OdbcCommand("SELECT COUNT(VORGANGID) FROM VGPOARSTKS WHERE VORGANGID=? AND POSID=? AND KOSTENSTELLEID ='1310 " + "?" + "'", connection);
            mycommand1.Parameters.Add("@vorgangid", OdbcType.VarChar);
            mycommand1.Parameters.Add("@posid", OdbcType.VarChar);
            mycommand1.Parameters.Add("@ds", OdbcType.VarChar);


            while (dbreader.Read())
            {

                int a = 0;
                int b = 0;
                int n = 0;
                string ds = "";
                string file;
                string file2;
                string C = ""; //C
                string D = ""; //D
                string D1 = ""; //D1
                string P = ""; //P
                string PI = ""; //PI

                string[] found = { "" };

                a = dbreader.GetInt32(0);
                b = dbreader.GetInt32(1);

                try
                {
                    C = dbreader.GetString(2);
                }
                catch { }
                try
                {
                    D = dbreader.GetString(3);
                }
                catch { }
                try
                {
                    D1 = dbreader.GetString(4);
                }
                catch { }
                try
                {
                    P = dbreader.GetString(5);
                }
                catch { }
                try
                {
                    PI = dbreader.GetString(6);
                }
                catch { }


                while (n < 5)
                {

                    ds = "";

                    if (n == 0)
                    {
                        if (D == "P")
                        { ds = "D"; }
                        if (D == "D")
                        {
                            if (P == "D")
                            {
                                ds = "D";
                            }
                        }
                    }
                    if (n == 1)
                    {
                        if (C == "P")
                        { ds = "C"; }
                        if (C == "D")
                        {
                            if (P == "D")
                            {
                                ds = "C";
                            }
                        }
                    }
                    if (n == 2)
                    {
                        if (P == "P")
                        { ds = "P"; }
                        if (P == "D")
                        {
                            ds = "P";
                        }
                        if (P == null)
                        { ds = "P"; }
                    }
                    if (n == 3)
                    {
                        if (D1 == "P")
                        { ds = "D1"; }
                        if (D1 == "D")
                        {
                            if (P == "D")
                            {
                                ds = "D1";
                            }
                        }

                    }
                    if (n == 4)
                    {
                        if (PI == "P")
                        { ds = "PI"; }
                        if (PI == "D")
                        {
                            if (PI == "D")
                            {
                                ds = "PI";
                            }
                        }
                    }



                    if (ds.Length > 0)
                    {

                        if (b.ToString().Length > 2)
                        {

                            file = a.ToString() + b.ToString() + ds;
                        }
                        else
                        {
                            file = a.ToString() + "0" + b.ToString() + ds;
                        }
                        file2 = file + ".TCN";

                        listBox2.Items.Add(file2);

                        found = Directory.GetFiles(pfad, file + "*.tcn");

                        int Anzahl = found.Length;



                        for (int i = 0; i < Anzahl; i++)
                        {

                            string indata = found[i].Substring(pfadlaenge);
                            indata = indata.Trim(new Char[] { '*', '.', 't', 'c', 'n' });
                            string vorgangid = indata.Substring(0, 8);
                            string posnummer = indata.Substring(9, 2);
                            int Anzahl1 = 0;
                            object returnvalue;


                            mycommand1.Parameters["@vorgangid"].Value = vorgangid;
                            mycommand1.Parameters["@posid"].Value = posnummer;
                            mycommand1.Parameters["@ds"].Value = ds;

                            try
                            {

                                returnvalue = mycommand1.ExecuteScalar();
                                Anzahl1 = Convert.ToInt32(returnvalue);


                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            if (Anzahl1 > 0)
                            {

                                listBox1.Items.Add(found[i].Substring(pfadlaenge) + "                          Fertig: " + Anzahl1);
                            }
                            else
                            {
                                listBox1.Items.Add(found[i].Substring(pfadlaenge) + "                      ----nicht Fertig----");
                            }
                        }
                    }

                    n++;

                }













            }
            dbreader.Close();


            int List1anzahl;
            int List2anzahl;

            List1anzahl = listBox1.Items.Count;
            List2anzahl = listBox2.Items.Count;



            string[] stopWordArray = new string[listBox2.Items.Count];
            int itemCount = listBox2.Items.Count;
            for (int i = 0; i < itemCount; i++)
            {
                stopWordArray[i] = listBox2.Items[i].ToString();



            }




            for (int z = 0; z < List2anzahl; z++)
            {


                string myString = stopWordArray[z].ToString();
                int index = listBox1.FindString(myString, -1);
                if (index != -1)
                {

                }
                else
                    listBox3.Items.Add(myString);




            }






            metroLabel4.Text = "Gescannt: " + List2anzahl.ToString();
            metroLabel6.Text = "Fertige Programme: " + List1anzahl.ToString();
            metroLabel7.Text = "Fehlende Programme: " + listBox3.Items.Count.ToString();



        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            string directory1 = "K:\\MKM\\MKM01\\";
            try
            {
                int itemCount = listBox3.Items.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    string pfad = "";


                    pfad = directory1 + listBox3.Items[i].ToString().Substring(0, 12) + ".ncp";
                    try
                    {
                        File.SetLastWriteTime(pfad, DateTime.Now);
                    }
                    catch
                    {
                        listBox3.Items[i] = listBox3.Items[i].ToString() + "  Datei nicht vorhanden";
                    }

                    listBox3.Items[i] = listBox3.Items[i].ToString() + "  ✔";

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Exception Occured while releasing object " + ex.ToString());
            }

            MessageBox.Show("Alle Dateien wurden erfolgreich neu abgespeichert!");




        }

        private void button4_Click(object sender, EventArgs e)
        {
            int i = 0;
            int TCN = 0;
            int DROP = 0;
            int gesamt = 0;
            metroProgressBar1.Value = 0;

            object returnvalue1;
            OdbcCommand mycommand2;

            mycommand2 = new OdbcCommand("SELECT SCANDATUM " +
                                            "FROM VGPOARSTKS " +
                                            "WHERE VGPOARSTKS.VORGANGID    = ? " +
                                            "AND VGPOARSTKS.POSID          = ? " +
                                            "AND VGPOARSTKS.STUECKID       = '1' " +
                                            "AND VGPOARSTKS.UPOSID         = '10' " +
                                            "AND VGPOARSTKS.FIRMAID        = '1' " +
                                            "AND VGPOARSTKS.KOSTENSTELLEID = '9200'", connection);

            System.IO.DirectoryInfo ParentDirectory = new System.IO.DirectoryInfo(@"W:\TCN\"); //tcn pfad 


            gesamt = ParentDirectory.GetFiles().Length;
            metroProgressBar1.Minimum = 0;
            metroProgressBar1.Maximum = gesamt;

            mycommand2.Parameters.Add("@vorgangid", OdbcType.VarChar);
            mycommand2.Parameters.Add("@posid", OdbcType.VarChar);

            if (ParentDirectory.Exists == true)
            {

                foreach (System.IO.FileInfo f in ParentDirectory.GetFiles())
                {

                    returnvalue1 = null;
                    mycommand2.Parameters["@posid"].Value = f.Name.ToString().Substring(9, 2);
                    mycommand2.Parameters["@vorgangid"].Value = f.Name.ToString().Substring(0, 8);
                    try
                    {
                        returnvalue1 = mycommand2.ExecuteScalar();
                    }
                    catch
                    {
                        MetroMessageBox.Show(this, "Fehler bei SQL-Statement für Vorgang: " + f.Name.ToString().Substring(0, 8) + " Postion: " + f.Name.ToString().Substring(9, 2));
                    }

                    if (returnvalue1 != null)
                    {
                        if (returnvalue1 is DBNull)
                        {
                        }
                        else
                        {

                            DateTime Datum123 = Convert.ToDateTime(returnvalue1);
                            Datum123 = Datum123.AddDays(+7);
                            if (DateTime.Now > Datum123)
                            {

                                try
                                {
                                    if (metroCheckBox1.Checked == true)
                                    {
                                        MetroMessageBox.Show(this, "Datei: " + f.Directory.ToString() + "\\" + f.Name.ToString() + " wird gelöscht");
                                    }
                                    File.Delete(f.Directory.ToString() + "\\" + f.Name.ToString());
                                    TCN++;

                                }
                                catch
                                {
                                }








                            }
                            i++;
                            metroProgressBar1.PerformStep();
                        }

                    }



                }
                MetroMessageBox.Show(this, "Dateianzahl Ingesamt W:\\TCN: " + i.ToString() + "\ngelöscht W:\\TCN: " + TCN.ToString() + "\ngelöscht Dropbox: " + DROP.ToString());
            }


            else
            {
                MetroMessageBox.Show(this, "Das Verzeichnis ''TCN'' ist nicht erreichbar");
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {


            OdbcCommand mycommand;

            string path = "";

            folderBrowserDialog1.SelectedPath = "C:\\";
            folderBrowserDialog1.Description = "Speicherort auswählen";

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                path = folderBrowserDialog1.SelectedPath.ToString();

                int f = 0;

                Excel.Application xlApp;
                Excel.Workbook xlWorkBook;
                Excel.Worksheet xlWorkSheet;
                object misValue = System.Reflection.Missing.Value;

                xlApp = new Excel.Application();
                xlWorkBook = xlApp.Workbooks.Add(misValue);

                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
                xlWorkSheet.Cells[1, 1] = "Anzahl";
                xlWorkSheet.Cells[1, 2] = "Vorgang";
                xlWorkSheet.Cells[1, 3] = "Posid";
                xlWorkSheet.Cells[1, 4] = "Vorgangsart";
                xlWorkSheet.Cells[1, 5] = "ArtikelNr.";
                xlWorkSheet.Cells[1, 6] = "Beschreibung";
                xlWorkSheet.Cells[1, 7] = "Artikelart";
                xlWorkSheet.Cells[1, 8] = "Druckdatum";

                for (int i = 0; i < listBox2.Items.Count; i++)
                {
                    string vgnummer = "";
                    string posnummer = "";
                    string selected = "";


                    selected = listBox2.Items[i].ToString();
                    vgnummer = selected.Substring(0, 8);
                    posnummer = selected.Substring(9, 2);


                    mycommand = new OdbcCommand("SELECT a.Kundeid, a.Name1, b.Postext, c.artikelid, d.kurztext, d.materialartid, d.Artikelart, a.vorgangsart, b.anzahlbe FROM VG a, vgpo b, vgpoar c, ar d WHERE b.VORGANGID = ? and b.POSID = ? and b.vorgangid = a.vorgangid and b.posid=c.posid and c.uposid='10' and c.firmaid=a.firmaid and c.vorgangid=a.vorgangid and c.artikelid=d.artikelid order by b.vorgangid, b.posid", connection);
                    mycommand.Parameters.Add("@vorgangid", OdbcType.VarChar).Value = vgnummer;
                    mycommand.Parameters.Add("@posid", OdbcType.VarChar).Value = posnummer;

                    OdbcDataReader dbreader = mycommand.ExecuteReader();

                    f++;

                    while (dbreader.Read())
                    {
                        string kundeid = "";
                        string postext = "";
                        string kurztext = "";
                        string materialartid = "";
                        string artikelart = "";
                        string artikelid = "";
                        string vorgangsart = "";
                        string anzahl = "";


                        kundeid = dbreader.GetString(0);

                        postext = dbreader.GetString(2);
                        artikelid = dbreader.GetString(3);
                        kurztext = dbreader.GetString(4);
                        materialartid = dbreader.GetString(5);
                        artikelart = dbreader.GetString(6);
                        vorgangsart = dbreader.GetString(7);
                        anzahl = dbreader.GetString(8);
                        string zahl = f.ToString();

                        xlWorkSheet.Cells[f + 1, 1] = anzahl;
                        xlWorkSheet.Cells[f + 1, 2] = vgnummer;
                        xlWorkSheet.Cells[f + 1, 3] = posnummer;
                        xlWorkSheet.Cells[f + 1, 4] = vorgangsart;
                        xlWorkSheet.Cells[f + 1, 5] = artikelid;
                        xlWorkSheet.Cells[f + 1, 6] = postext;
                        xlWorkSheet.Cells[f + 1, 7] = artikelart;
                        xlWorkSheet.Cells[f + 1, 8] = datum1;








                    }
                    dbreader.Close();




                }
                xlWorkBook.SaveAs(path + "\\Slowakei.xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();

                releaseObject(xlWorkSheet);
                releaseObject(xlWorkBook);
                releaseObject(xlApp);

                MessageBox.Show("Excel Datei wurde erstellt, sie liegt unter " + path + "\\Slowakei.xls");
            }
            else
            {
                MessageBox.Show("Pfad ungültig");
            }







        }

    }


}