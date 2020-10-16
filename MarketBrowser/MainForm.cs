using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        private List<List<string>> MakeColumnnarDataStructure()
        {
            StreamReader sr = new StreamReader(new FileStream("market.csv", FileMode.Open), Encoding.Default);


            var line = sr.ReadLine();
            var headers = line.Split(',');

            List<List<string>> data = new List<List<string>>();

            foreach(string header in headers)
            {
                List<string> list = new List<string>();
                list.Add(header);
                data.Add(list);
            }
            int row = 0;
            while (sr.EndOfStream == false)
            {
                line = sr.ReadLine();
                var values = line.Split(',');
                // | 우리 시장 | 5일장 | 대구시 | ...

                for (int i = 0; i < values.Length; i++)
                {
                    data[i].Add(values[i]);
                }
                //str += values[4] + "\r\n";
                if (row++ == 10)
                    break;
            }

            /*
            textBoxCSVViewer.Text = str;
            */
            sr.Close();

            return data;
        }

        private void printColumnData_columbased(List<List<string>> data, int columnIdx)
        {
            // Colum 단위로 뽑는 코드
            string str = "";
            textBoxCSVViewer.Text = "";
            foreach(string value in data[1]){ // UI 오버헤드가 크므로 웬만하면 한번에 일괄 처리 하는 것이 좋음.
                str += value + "\r\n";
                //textBoxCSVViewer.Text = value + "\r\n";// 업데이트를 매번 실행하여 느려짐
            }
            textBoxCSVViewer.Text = str;
        }

        private void printRowData_columbased(List<List<string>> data)
        {
            //Row 단위로 뽑는 코드
            string str = "";
            for(int row = 0; row < data[0].Count; row++)
            {
                foreach(List<string> list in data)
                {
                    str += list[row] +"\t";
                }
                str += "\r\n";
            }
            textBoxCSVViewer.Text = str;
        }

        private string[] ParseCSVLine(string line) // 과제 코드
        {
            //Regex parse = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            //string[] str = parse.Split(line);

            //for(int i = 0; i < str.Length; i++)
            //{
            //    str[i] = str[i].TrimStart('"');
            //    str[i] = str[i].TrimEnd('"');
            //}
            //char[] temp = new char[line.Length];
            string [] str = new string[12];
            int quotes, comma = 0;
            //for(int i = 0; i < line.Length; i++)
           // {
            //    temp[i] = line[i];
            //}
            for(int i = 0; i < line.Length; i++)
            {
                quotes = 0;
                int j = 0;
                if(line[i] == '\"')
                {
                    i++;
                    quotes++;
                    string str_temp = "";
                    for(j = 1; j < line.Length; j++)
                    {
                        str_temp += line[j];
                        if(line[i + j] == '\"')
                            quotes++;
                        if(i + j == line.Length || quotes == 2)
                        {
                            break;
                        }
                    }
                    str[comma++] = str_temp;
                    i++;
                }
                else if(line[i] == ',')
                {
                    string str_temp = "";
                    for(j = 0; j < line.Length; j++)
                    {
                        if(i + j == line.Length || line[i + j] == ',')
                        {
                            break;
                        }
                        str_temp += line[i];
                    }
                    str[comma++] = str_temp;
                }
                else
                {
                    string str_temp = "";
                    for(j = 0; j < line.Length; j++)
                    {
                        if(i + j == line.Length || line[i + j] == ',')
                        {
                            break;
                        }
                        str_temp += line[i + j];
                    }
                    str[comma++] = str_temp;
                }
                i += j;
            }

            return str;
         //   if (line[0] == '"')
         //       return line.Trim('"').Split(',');
                //return line.Trim('"').Split(new string[] {"\",\""}, StringSplitOptions.None);
         //   return line.Split(',');
        }
        private List<List<string>> MakeRowbasedDataStructure()
        {
            StreamReader sr = new StreamReader(new FileStream("market.csv", FileMode.Open), Encoding.Default);


            var line = sr.ReadLine();
            var headers = line.Split(',');

            List<List<string>> data = new List<List<string>>();

            //int row = 0;
            while (sr.EndOfStream == false)
            {
                line = sr.ReadLine();
                var values = ParseCSVLine(line);

                // "시장(구, 우리시장)"
                // 중간에 , 제대로 처리하기. (Parser)
                // parseCSVLine(string line) 함수 만들기

                data.Add(values.ToList());
                //if (row++ == 10)
                    //break;
            }

            /*
            textBoxCSVViewer.Text = str;
            */
            sr.Close();

            return data;
        }

        private void printRowData_rowbased(List<List<string>> data)
        {
            string str = "";
            foreach (List<string> rowlist in data)
            {
                foreach(string value in rowlist)
                {
                    str += value + "\t";
                }
                str += "\r\n";
            }
            textBoxCSVViewer.Text = str;
        }

        private void printColumnData_rowbased(List<List<string>> data, int colIdx)
        {
            string str = "";
            foreach(List<string> rowlist in data)
            {
                str += rowlist[colIdx] + "\r\n";
            }
            textBoxCSVViewer.Text = str;
        }


        private void buttonOpenCSV_Click(object sender, EventArgs e)
        {
            List<List<string>> data = MakeRowbasedDataStructure();
            printRowData_rowbased(data);
            //printColumnData_rowbased(data, 1);
            //List<List<string>> data = MakeColumnnarDataStructure();
            //printColumnData_columbased(data , 1);
            //printRowData_columbased(data);
        }
    }
}
