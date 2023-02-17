using HtmlAgilityPack;
using Microsoft.VisualBasic.ApplicationServices;
using System.Data;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using static System.Formats.Asn1.AsnWriter;
using System.Configuration;
using System.Data.SqlClient;
using WebSiteScrapper.Data;
using WebSiteScrapper.Models;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using WebSiteScrapper.Classes;
using Microsoft.Identity.Client;
using System.Windows.Forms;

namespace WebSiteScrapper
{
    public partial class Form1 : Form
    {
        public Form self;
        public Thread t1;
        public Thread t2;
        public Hive hive;
        public bool finished = false;
        public string Message;
        public string Status;
        public Form1()
        {
            InitializeComponent();
            self = this;
        }

        private string url;
        private void button1_Click(object sender, EventArgs e)
        {
            if (txtUrl.Text.Trim() != "")
            {
                button1.Enabled = false;
                button2.Enabled = true;
                WebSiteScrapperContext _Context = new WebSiteScrapperContext(ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString);
                t1 = new Thread(DoWork);
                t1.Start();

                t2 = new Thread(UpdateControls);
                t2.Start();
            }
            else
            {
                MessageBox.Show("Please provide a url");
            }
            
                        
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (hive.paused == true)
            {
                MessageBox.Show("Resuming");
                button2.Text = "Pause";
                lblStatus.Text = "Resumed";
                hive.Start();
            }
            else
            {
                MessageBox.Show("Pausing");
                button2.Text = "Resume";
                lblStatus.Text = "Paused";
                hive.Pause();
            }

        }

       
        private void DoWork(object state)
        {

            //List<Urls> urls = scraper.GetAllUrlsFromSite_v2();
            //AddColumnsToDataGridView(CreateDataTable());
            WebSiteScrapperContext _Context = new WebSiteScrapperContext(ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString);
            SqlHandler sh = new SqlHandler(_Context);
            hive = new Hive(_Context, this);
            //sh.TruncateTable("Urls");
            //hive.ScanWebSite("https://theotokatosfc.gr");
            //hive.ScanWebSite("https://www.civiltech.gr/");
            string url = txtUrl.Text.Trim();

            if(url.StartsWith("http://") || url.StartsWith("https://"))
            {
                hive.ScanWebSite(url);
            }
            else
            {
                hive.ScanWebSite("https://"+url);
            }
                


            finished = true;           
            ToggleProcess();

        }

        public void UpdateControls()
        {
            WebSiteScrapperContext _Context = new WebSiteScrapperContext(ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString);
            SqlHandler sh = new SqlHandler(_Context);
            while (finished == false)
            {               
                this.SetlabelValue("Scrapping", sh.CalculateTotal().ToString(), sh.CalculateYetToVisit().ToString(), sh.CalculateVisited().ToString(), sh.maxYetToVisit.ToString());
            }

            this.SetlabelValue("finished", sh.CalculateTotal().ToString(), sh.CalculateYetToVisit().ToString(), sh.CalculateVisited().ToString(), sh.maxYetToVisit.ToString());
     


            this.SetDataTable();
        }

        public DataTable CreateDataTable()
        {
            // Create a new DataTable.
            DataTable table = new DataTable();

            // Add two columns to the DataTable.
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Title", typeof(string));
            table.Columns.Add("OriginalUrl", typeof(string));
            table.Columns.Add("Url", typeof(string));
            table.Columns.Add("ReferenceUrl", typeof(string));
            table.Columns.Add("Baseurl", typeof(string));                                  
            table.Columns.Add("visited", typeof(bool));
            table.Columns.Add("status", typeof(string));


            return table;
        }       
        
       
        public delegate void DelAddColumnsToDataGridView(DataTable dt);
        public void AddColumnsToDataGridView(DataTable dt)
        {
            if (this.InvokeRequired) this.Invoke(new DelAddColumnsToDataGridView(AddColumnsToDataGridView), dt);
            else
            {
                foreach (DataColumn dc in dt.Columns)
                {
                    dgrView.Columns.Add(dc.ColumnName, dc.ColumnName);
                }
            }

        }


        #region Deligates
        public delegate void DelSetLabelValue(string url, string total, string yetToVisit, string visited, string maxYetToVisit);
        public void SetlabelValue(string url, string total, string yetToVisit, string visited, string maxYetToVisit)
        {
            if (this.InvokeRequired) this.Invoke(new DelSetLabelValue(SetlabelValue), url, total, yetToVisit, visited, maxYetToVisit);
            else
            {
                this.lblUrl.Text = Message;
                this.lblTotal.Text = total;
                this.lblYetToVisit.Text = yetToVisit;
                this.lblVisited.Text = visited;
                this.lblMaxYetToVisit.Text = maxYetToVisit;
                this.lblStatus.Text = Status;
                
            }

        }

        public delegate void DelSetLabelDataView();
        public void SetDataTable()
        {
           

            if (this.InvokeRequired) this.Invoke(new DelSetLabelDataView(SetDataTable));
            else
            {
                WebSiteScrapperContext _Context = new WebSiteScrapperContext(ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString);
                DataTable dt = this.CreateDataTable();
                SqlHandler sh = new SqlHandler(_Context);
                List<Urls> urls = sh.GetAllDbUrls();

                foreach (Urls url in urls)
                {
                    DataRow row = dt.NewRow();
                    //table.Columns.Add("Id", typeof(int));
                    //table.Columns.Add("Title", typeof(string));
                    //table.Columns.Add("OriginalUrl", typeof(string));
                    //table.Columns.Add("Url", typeof(string));
                    //table.Columns.Add("ReferenceUrl", typeof(string));
                    //table.Columns.Add("Baseurl", typeof(string));
                    //table.Columns.Add("visited", typeof(bool));
                    //table.Columns.Add("status", typeof(string));
                    row[0] = url.Id;
                    row[1] = url.Title;
                    row[2] = url.OriginalUrl;
                    row[3] = url.Url;                    
                    row[4] = url.RefererUrl;
                    row[5] = url.Baseurl;
                    row[6] = url.Visited;
                    row[7] = url.Status;
                    dt.Rows.Add(row);
                }

                int selRow = 0, selCol = 0;

                if (dgrView.SelectedCells.Count > 0)
                {
                    selRow = dgrView.CurrentCell.RowIndex;
                    selCol = dgrView.CurrentCell.ColumnIndex;
                }

               
                dgrView.DataSource = dt;
                dgrView.Update();

                dgrView.Rows[selRow].Cells[selCol].Selected = true;
                dgrView.CurrentCell = dgrView.Rows[selRow].Cells[selCol];
                


            }

        }

        public void SetDataTableWithProblematicLinks()
        {
            if (this.InvokeRequired) this.Invoke(new DelSetLabelDataView(SetDataTable));
            else
            {
                WebSiteScrapperContext _Context = new WebSiteScrapperContext(ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString);
                DataTable dt = this.CreateDataTable();
                SqlHandler sh = new SqlHandler(_Context);
                List<Urls> urls = sh.GetAllDbUrls();

                foreach (Urls url in urls)
                {
                    DataRow row = dt.NewRow();
                    //table.Columns.Add("Id", typeof(int));
                    //table.Columns.Add("Title", typeof(string));
                    //table.Columns.Add("OriginalUrl", typeof(string));
                    //table.Columns.Add("Url", typeof(string));
                    //table.Columns.Add("ReferenceUrl", typeof(string));
                    //table.Columns.Add("Baseurl", typeof(string));
                    //table.Columns.Add("visited", typeof(bool));
                    //table.Columns.Add("status", typeof(string));
                    row[0] = url.Id;
                    row[1] = url.Title;
                    row[2] = url.OriginalUrl;
                    row[3] = url.Url;
                    row[4] = url.RefererUrl;
                    row[5] = url.Baseurl;
                    row[6] = url.Visited;
                    row[7] = url.Status;
                    dt.Rows.Add(row);
                }

                int selRow = 0, selCol = 0;

                if (dgrView.SelectedCells.Count > 0)
                {
                    selRow = dgrView.CurrentCell.RowIndex;
                    selCol = dgrView.CurrentCell.ColumnIndex;
                }


                dgrView.DataSource = dt;
                dgrView.Update();

                dgrView.Rows[selRow].Cells[selCol].Selected = true;
                dgrView.CurrentCell = dgrView.Rows[selRow].Cells[selCol];



            }
        }

        public void DeleteAllLinks()
        {
            WebSiteScrapperContext _Context = new WebSiteScrapperContext(ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString);            
            SqlHandler sh = new SqlHandler(_Context);
            sh.TruncateTable("Urls");
        }
        public delegate void DelAddRowToDataTable(DataRow dr);
        public void AddRowToDataTable(DataRow dr)
        {
            if (this.InvokeRequired) this.Invoke(new DelAddRowToDataTable(AddRowToDataTable), dr);
            else
            {                
                dgrView.Rows.Add(dr.ItemArray);                
                dgrView.Update();               
            }

        }

        public delegate void DelToggleProcess();
        public void ToggleProcess()
        {
            if (this.InvokeRequired) this.Invoke(new DelToggleProcess(ToggleProcess), null);
            else
            {
                if (button1.Enabled == false)
                {
                    button1.Enabled = true;
                }
                else
                {
                    button1.Enabled = false;
                }
            }

        }

        #endregion

        private void button3_Click(object sender, EventArgs e)
        {
            SetDataTable();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SetDataTableWithProblematicLinks();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DeleteAllLinks();
            MessageBox.Show("Links Deleted");
        }
    }
}