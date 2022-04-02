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
    public partial class Form1 : Form
    {
        Form2 form2;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            form2 = new Form2();
            form2.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string waluta = textBox1.Text.ToLower();
                string url = "http://api.nbp.pl/api/exchangerates/rates/a/" + waluta + "/?format=json";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var d = response.StatusCode;
                string content = new StreamReader(response.GetResponseStream()).ReadToEnd();
                dynamic data = JObject.Parse(content);//var - kompilacja dynamic - runtime //dynamic bo typ jest definiowany podczas pracy programu
                var g = data.rates[0].mid;
                form2.addToList(waluta);
                textBox1.Text = "";
            }
            catch(Exception Error)
            {
                MessageBox.Show(Error.Message, "Error");
            }
        }
    }
}
