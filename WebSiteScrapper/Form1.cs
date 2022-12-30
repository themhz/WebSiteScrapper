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

namespace WebSiteScrapper
{
    public partial class Form1 : Form
    {
        public Form self;
        public Form1()
        {
            InitializeComponent();
            self = this;
        }

        private string url;
        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            ThreadPool.QueueUserWorkItem(DoWork);         
        }
                
        private void DoWork(object state)
        {

            
            url = "https://theotokatosfc.gr/";
            //url = "https://www.civiltech.gr/";              
            WebSiteScrapperContext _Context = new WebSiteScrapperContext(ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString);
            var scraper = new Scraper(url, this, _Context);            
            List<string> urls = scraper.GetAllUrlsFromSite_v2();

            DataTable dt = CreateDataTable(urls);
            SetDataTable(dt);
            button1.Enabled = true;

        }

        private DataTable CreateDataTable(List<string> urls)
        {
            // Create a new DataTable.
            DataTable table = new DataTable();

            // Add two columns to the DataTable.
            table.Columns.Add("Index", typeof(int));
            table.Columns.Add("hash", typeof(string));
            table.Columns.Add("url", typeof(string));            

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