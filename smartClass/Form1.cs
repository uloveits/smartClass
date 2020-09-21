using smartClass.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

using Excel = Microsoft.Office.Interop.Excel;

namespace smartClass
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

   
        private void importExcelBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            ofd.Filter = "工作表|*.xls";
            string filename = ofd.FileName;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //连接字符串
                string constr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filename + ";Extended Properties='Excel 12.0;HDR=Yes;IMEX=1;'";
                //创建一个数据链接
                OleDbConnection myConn = new OleDbConnection(constr);
                string strCom = " SELECT * FROM [Sheet1$] ";
                myConn.Open();

                //打开数据链接，得到一个数据集
                OleDbDataAdapter da = new OleDbDataAdapter(strCom, myConn);
                // DataSet对象
                DataSet ds = new DataSet();
                //得到自己的DataSet对象
                da.Fill(ds, "[Sheet1$]");
                //关闭此数据链接
                myConn.Close();
                this.importExcelData.DataSource = ds.Tables[0];
            }
        }

        private void exportBtn_Click(object sender, EventArgs e)
        {
            if(this.className.Text == "")
            {
                MessageBox.Show("请填写所在班级！");
                return;
            }
            if (ExportDataGridview(this.importExcelData, true))
                MessageBox.Show("导出成功，请记得保存!");
            else
                MessageBox.Show("导出未成功，请检查是否有错!");
        }

        public bool ExportDataGridview(DataGridView gridView, bool isShowExcle)//生成Excel
        {
            if (gridView.Rows.Count == 0)
            {
                MessageBox.Show("未导入原始课表数据");
         
                return false;
            }
            //建立Excel对象
            Excel.Application excel = new Excel.Application();
            excel.Application.Workbooks.Add(true);
            excel.Visible = isShowExcle;
            //教室 日期  节次 学科  任课教师工号 任课教师姓名
            string[] herder = new string[6] { "教室", "日期", "节次", "学科", "任课教师工号", "任课教师姓名"};
            //生成字段名称
            for (int i = 0; i < herder.Length; i++)
            {
                excel.Cells[1, i + 1] = herder[i];
            }
            //填充数据

            //21 22 23

            ArrayList rowList = new ArrayList();

            int idx = 1;
            for (int i = 2; i < gridView.ColumnCount; i++)
            {
                //处理数据              
                for (int j = 2; j < gridView.RowCount - 1; j++)
                {

                    string key = gridView[i, j].Value.ToString();

                    string _key = key.Replace("（", "(").Replace("）", ")");

                    string teacher = Regex.Replace(_key, @"(.*\()(.*)(\).*)", "$2"); ;



                    string course = _key.Replace("(" + teacher +")","");


                    DateTime dt = this.dateTime.Value;
                    if (6< idx && idx<=12)
                    {
                        dt = dt.AddDays(1);
                    }
                    else if (12 < idx && idx <= 18)
                    {
                        dt = dt.AddDays(2);

                    }
                    else if (18 < idx && idx <= 24)
                    {
                        dt = dt.AddDays(3);

                    }
                    else if (24 < idx && idx <= 30)
                    {
                        dt = dt.AddDays(4);

                    }
                    else if (30 < idx && idx <= 36)
                    {
                        dt = dt.AddDays(5);

                    }
                    idx++;




                    string[] _row = new string[6] 
                    {
                        this.className.Text,
                        dt.ToShortDateString(),
                        gridView[1, j].Value.ToString().Replace('/','-'),
                        course,
                        Resources.ResourceManager.GetString(teacher),
                        teacher
                    };        
                    rowList.Add(_row);               
                }
           
            }
                       
            for (int m=0;m< rowList.Count;m++)
            {
                string[] _row = (string[])rowList[m];

                for (int n = 0; n< _row.Length;n++)
                {
                    excel.Cells[m + 2, n + 1] = _row[n];                  
                }
            }
            return true;
        }

    }
}
