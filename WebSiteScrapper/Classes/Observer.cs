using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSiteScrapper.Classes
{
    internal class Observer
    {
        Form1 form;
        Spider spider;

        public Observer (Form1 _form,Spider _spider)
        {
            this.form = _form;  
            this.spider = _spider;
        }

        public void UpdateForm()
        {
            form.lblStatus.Text = "Active";
        }

    }
}
