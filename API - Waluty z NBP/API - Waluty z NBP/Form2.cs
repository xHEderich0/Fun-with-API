using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;

namespace API__Waluty_z_NBP
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        public void addToList(string curr)
        {
            bool flag = true;
            if (listBox1.Items.Count == 0)
                this.listBox1.Items.Add(curr);
            else
            {
                foreach (var element in listBox1.Items)
                {
                    if (curr == element.ToString())
                    {
                        flag = false;
                        break;
                    }
                }
                this.listBox1.Items.Add(curr);
                var elements = this.listBox1.Items.Cast<string>().Distinct().ToArray();
                this.listBox1.Items.Clear();
                foreach (var element in elements)
                {
                    this.listBox1.Items.Add(element);
                }
            }
            
            if(flag)
            {
                using (StreamWriter label = new StreamWriter(curr + ".txt", true))
                {
                    label.WriteLine("<|>   " + "Currency" + "   <|>     " + "Date" + "     <|>   " + "Avg. exchange rates" + "  <|> \n");
                    try
                    {
                    }
                    catch(Exception Error)
                    {
                        MessageBox.Show(Error.Message, "Error");
                    }
                }
            }

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            foreach(var element in listBox1.Items)
            {
                try
                {
                    string waluta = element.ToString();
                    string url = "http://api.nbp.pl/api/exchangerates/rates/a/" + waluta + "/?format=json";
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    var d = response.StatusCode;
                    string content = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    dynamic data = JObject.Parse(content);//var - kompilacja dynamic - runtime //dynamic bo typ jest definiowany podczas pracy programu
                    var effDate = data.rates[0].effectiveDate;
                    double mid = data.rates[0].mid;

                    using (StreamWriter saveScore = new StreamWriter(waluta + ".txt", true)) //jesli na nowym obiekcie wykonamy wszystkie akcje w klamrach, plik sie zamyka i zapisuje, wywalając go z pamieći RAM
                    {       
                        saveScore.WriteLine("<|      " + waluta + "       |   " + effDate + "   |           " + mid + "          |>");
                    }
                }
                catch(Exception Error)
                {
                    MessageBox.Show(Error.Message, "Error");
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double sum = 0.0;
            int amount = 0;
            string waluta = "";
            string effDate = ""; 
            try
            {   
                using (StreamReader sr = new StreamReader(textBox1.Text, true)) //jesli na nowym obiekcie wykonamy wszystkie akcje w klamrach, plik sie zamyka i zapisuje, wywalając go z pamieći RAM
                {
                    //string line;
                    for (var i = 0; i < 2; i++) //do pominiecia pierwszych linii, z ktorych nie mozna pobrac liczb
                    {
                        sr.ReadLine();
                    }
                    string line;

                    while ((line = sr.ReadLine()) != null)
                    {
                        if(!(line == ""))
                        {
                            string[] red = line.Split('|');
                            double result = double.Parse(red[3]);
                            effDate = Regex.Replace(red[2], @"\s+", "");
                            waluta = Regex.Replace(red[1], @"\s+", "");
                            sum += result;
                            amount++;
                        }
                    }
                }
                using (StreamWriter saveAvg = new StreamWriter(waluta + "Avg" + ".txt", true)) //jesli na nowym obiekcie wykonamy wszystkie akcje w klamrach, plik sie zamyka i zapisuje, wywalając go z pamieći RAM
                {
                    saveAvg.WriteLine((sum / amount + "  |  " + effDate).ToString());
                }
                textBox1.Text = "";
            }
            catch(Exception Error)
            {
                MessageBox.Show(Error.Message, "Error");
            }
        }
    }
}
