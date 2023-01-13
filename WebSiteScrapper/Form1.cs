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

namespace WebSiteScrapper
{
    public partial class Form1 : Form
    {
        public Form self;
        public Thread t1;
        public Thread t2;
        public Spider scraper;
        public bool finished = false;
        public Form1()
        {
            InitializeComponent();
            self = this;
        }

        private string url;
        private void button1_Click(object sender, EventArgs e)
        {
            //    button1.Enabled = false;
            //    button2.Enabled = true;
            //    WebSiteScrapperContext _Context = new WebSiteScrapperContext(ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString);
            //    url = "https://www.kipodomi-tools.gr/";

            //    scraper = new Spider(url, this, _Context);
            //    t1 = new Thread(DoWork);
            //    t1.Start();

            //    t2 = new Thread(SetlabelValue);
            //    t2.Start();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            //    if (scraper.paused == true)
            //    {
            //        MessageBox.Show("Resuming");
            //        button2.Text = "Pause";
            //        lblStatus.Text = "Resumed";
            //        scraper.Start();
            //    }
            //    else
            //    {                
            //        MessageBox.Show("Pausing");
            //        button2.Text = "Resume";
            //        lblStatus.Text = "Paused";
            //        scraper.Pause();
            //    }                            

        }

        private void SetlabelValue(object state)
        {
            var db = new SqlHandler(ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString);

            while(finished == false)
            {         
                System.Threading.Thread.Sleep(1000);
            }
            
        }
        //private void DoWork(object state)
        //{                        
            
        //    List<Urls> urls = scraper.GetAllUrlsFromSite_v2();
        //    finished = true;
        //    DataTable dt = CreateDataTable();
        //    foreach (Urls url in urls)
        //    {
        //        DataRow row = dt.NewRow();
        //        row[0] = url.Id;
        //        row[1] = url.Url;
        //        row[2] = url.Title;
        //        row[3] = url.Hash;
        //        dt.Rows.Add(row);
        //    }


        //    SetDataTable(dt);
        //    ToggleProcess();

        //}

        private DataTable CreateDataTable()
        {
            // Create a new DataTable.
            DataTable table = new DataTable();

            // Add two columns to the DataTable.
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Url", typeof(string));
            table.Columns.Add("Title", typeof(string));
            table.Columns.Add("Body", typeof(string));
            table.Columns.Add("Hash", typeof(string));

            return table;
        }        


        #region Deligates
        public delegate void DelSetLabelValue(string url, string total, string yetToVisit, string visited, string maxYetToVisit);
        public void SetlabelValue(string url, string total, string yetToVisit, string visited, string maxYetToVisit)
        {
            if (this.InvokeRequired) this.Invoke(new DelSetLabelValue(SetlabelValue), url, total, yetToVisit, visited, maxYetToVisit);
            else
            {
                this.lblUrl.Text = url;
                this.lblTotal.Text = total;
                this.lblYetToVisit.Text = yetToVisit;
                this.lblVisited.Text = visited;
                this.lblMaxYetToVisit.Text = maxYetToVisit;
            }

        }

        public delegate void DelSetLabelDataView(DataTable dt);
        public void SetDataTable(DataTable dt)
        {
            if (this.InvokeRequired) this.Invoke(new DelSetLabelDataView(SetDataTable), dt);
            else
            {
                dgrView.DataSource = dt;
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

       
    }
}