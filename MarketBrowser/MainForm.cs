using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MarketBrowser
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        string str = "";
        private void buttonOpenCSV_Click(object sender, EventArgs e)
        {
            StreamReader sr = new StreamReader(new FileStream("market.csv", FileMode.Open), Encoding.Default);

            var line = sr.ReadLine();
            var headers = line.Split(',');
            textBoxCSVViewer.Text = headers.ToString();

            foreach(string header in headers)
            {
                textBoxCSVViewer.Text += header + "\t";

            }
            /*
            while (sr.EndOfStream == false)
            {
                line = sr.ReadLine();
                var values = line.Split(',');
                str += values[4] + "\r\n";
            }

            textBoxCSVViewer.Text = str;

            */
            sr.Close();
        }
    }
}
