using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
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
            // 방법 1
            /*
            string [] str = new string[12];
            int quotes, comma = 0;
            for(int i = 0; i < line.Length; i++)
            {
                quotes = 0;
                int j = 0;
                if(line[i] == '\"') // line을 하나씩 읽다가 quotes를 만나면 진입
                {
                    i++;
                    quotes++;
                    string str_temp = "";
                    for(j = 1; j < line.Length; j++)
                    {
                        str_temp += line[j]; // 첫 quotes 뒤는 같은 column
                        if(line[i + j] == '\"') // quotes를 한 번 더 만나면 종료
                            quotes++;
                        if(i + j == line.Length || quotes == 2)
                        {
                            break;
                        }
                    }
                    str[comma++] = str_temp;
                    i++;
                }
                else if(line[i] == ',') // comma를 만나면 진입
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
*/
            // 방법 2
            Regex parse = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))"); // parse할 문자열 정규식으로 정의
            string[] str = parse.Split(line); // 위의 정규식으로 line을 자름

            for(int i = 0; i < str.Length; i++)
            {
                str[i] = str[i].TrimStart('"');
                str[i] = str[i].TrimEnd('"');
            }
            return str;
        }

        Dictionary<string, int> headerIndexDict = new Dictionary<string, int>();

        private void SetHeaderList(string header)
        {
            var values = ParseCSVLine(header);
            listBoxHeaders.Items.Clear();

            for (int i = 0; i < values.Length; i++)
            {
                string column = values[i];
                listBoxHeaders.Items.Add(column);
                headerIndexDict[column] = i;
            }
        }

        private List<List<string>> MakeRowbasedDataStructure(string filename)
        {
            StreamReader sr = new StreamReader(new FileStream(filename, FileMode.Open), Encoding.Default);

            var line = sr.ReadLine();
            SetHeaderList(line);

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

        private void printRowData_rowbased(List<List<string>> data, TextBox textbox)
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
            textbox.Text = str;
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


        List<List<string>> data;
        private void buttonOpenCSV_Click(object sender, EventArgs e)
        {
            // 파일 탐색기 열어줌
            var FD = new OpenFileDialog();
            DialogResult dResult = FD.ShowDialog();

            if (dResult == DialogResult.OK)
            {
                string filename = FD.FileName;
                data = MakeRowbasedDataStructure(filename);
                printRowData_rowbased(data, textBoxCSVViewer);
            }

            //printColumnData_rowbased(data, 1);
            //List<List<string>> data = MakeColumnnarDataStructure();
            //printColumnData_columbased(data , 1);
            //printRowData_columbased(data);
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            string keyword = textBoxSearchKeyword.Text;

            if (keyword == "")
            {
                MessageBox.Show("검색 키워드를 입력하세요.", "확인");
                return;
            }

            if(listBoxHeaders.SelectedItem == null)
            {
                MessageBox.Show("검색대상 컬럼을 선택하세요.", "확인");
                return;
            }
            string column = listBoxHeaders.SelectedItem.ToString();
            if (column == "")
            {
                MessageBox.Show("검색대상 칼럼을 선택하세요.", "확인");
                return;
            }

            MessageBox.Show("키워드 = " + keyword + " 칼럼 = " + column, "확인");

            DoSearch(keyword, column);
        }

        private void DoSearch(string keyword, string column)
        {
            // 1. 대상 컬럼 확인
            if(headerIndexDict.ContainsKey(column) == false)
            {
                MessageBox.Show("검색대상 컬럼을 선택하세요", "확인");
                return;
            }

            int targetCoumnIndex = headerIndexDict[column];

            // 2. 대상 컬럼에서 키워드를 포함하는지 확인
            List<List<string>> resultData = new List<List<string>>();
            foreach(List<string> row in data)
            {
                string value = row[targetCoumnIndex];
                if(value.Contains(keyword))
                {
                    resultData.Add(row);
                }
            }

            printRowData_rowbased(resultData, textBoxSearchResult);
        }
    }
}
