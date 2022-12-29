namespace WebSiteScrapper
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.dgrView = new System.Windows.Forms.DataGridView();
            this.lblUrl = new System.Windows.Forms.Label();
            this.lblVisited = new System.Windows.Forms.Label();
            this.lblYetToVisit = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblMaxYetToVisit = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgrView)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(16, 140);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(94, 29);
            this.button1.TabIndex = 0;
            this.button1.Text = "Scrap";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dgrView
            // 
            this.dgrView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgrView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgrView.Location = new System.Drawing.Point(2, 199);
            this.dgrView.Name = "dgrView";
            this.dgrView.RowHeadersWidth = 51;
            this.dgrView.RowTemplate.Height = 29;
            this.dgrView.Size = new System.Drawing.Size(1215, 435);
            this.dgrView.TabIndex = 1;
            // 
            // lblUrl
            // 
            this.lblUrl.AutoSize = true;
            this.lblUrl.Location = new System.Drawing.Point(132, 145);
            this.lblUrl.Name = "lblUrl";
            this.lblUrl.Size = new System.Drawing.Size(15, 20);
            this.lblUrl.TabIndex = 2;
            this.lblUrl.Text = "-";
            // 
            // lblVisited
            // 
            this.lblVisited.AutoSize = true;
            this.lblVisited.Location = new System.Drawing.Point(131, 69);
            this.lblVisited.Name = "lblVisited";
            this.lblVisited.Size = new System.Drawing.Size(15, 20);
            this.lblVisited.TabIndex = 3;
            this.lblVisited.Text = "-";
            // 
            // lblYetToVisit
            // 
            this.lblYetToVisit.AutoSize = true;
            this.lblYetToVisit.Location = new System.Drawing.Point(131, 39);
            this.lblYetToVisit.Name = "lblYetToVisit";
            this.lblYetToVisit.Size = new System.Drawing.Size(15, 20);
            this.lblYetToVisit.TabIndex = 4;
            this.lblYetToVisit.Text = "-";
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(131, 14);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(15, 20);
            this.lblTotal.TabIndex = 5;
            this.lblTotal.Text = "-";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(59, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 20);
            this.label1.TabIndex = 6;
            this.label1.Text = "Visited";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(36, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 20);
            this.label2.TabIndex = 7;
            this.label2.Text = "Yet to visit";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(69, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 20);
            this.label3.TabIndex = 8;
            this.label3.Text = "Total";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 105);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(111, 20);
            this.label4.TabIndex = 10;
            this.label4.Text = "Max Yet To visit";
            // 
            // lblMaxYetToVisit
            // 
            this.lblMaxYetToVisit.AutoSize = true;
            this.lblMaxYetToVisit.Location = new System.Drawing.Point(132, 105);
            this.lblMaxYetToVisit.Name = "lblMaxYetToVisit";
            this.lblMaxYetToVisit.Size = new System.Drawing.Size(15, 20);
            this.lblMaxYetToVisit.TabIndex = 9;
            this.lblMaxYetToVisit.Text = "-";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1220, 637);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblMaxYetToVisit);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.lblYetToVisit);
            this.Controls.Add(this.lblVisited);
            this.Controls.Add(this.lblUrl);
            this.Controls.Add(this.dgrView);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Scraper";
            ((System.ComponentModel.ISupportInitialize)(this.dgrView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button button1;
        private DataGridView dgrView;
        public Label lblUrl;
        private Label lblVisited;
        private Label lblYetToVisit;
        private Label lblTotal;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label lblMaxYetToVisit;
    }
}