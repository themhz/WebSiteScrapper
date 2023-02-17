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
            this.button2 = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgrView)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(477, 54);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(94, 29);
            this.button1.TabIndex = 0;
            this.button1.Text = "Crawl";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dgrView
            // 
            this.dgrView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgrView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgrView.Location = new System.Drawing.Point(2, 302);
            this.dgrView.Name = "dgrView";
            this.dgrView.RowHeadersWidth = 51;
            this.dgrView.RowTemplate.Height = 29;
            this.dgrView.Size = new System.Drawing.Size(1148, 666);
            this.dgrView.TabIndex = 1;
            // 
            // lblUrl
            // 
            this.lblUrl.AutoSize = true;
            this.lblUrl.Location = new System.Drawing.Point(133, 228);
            this.lblUrl.Name = "lblUrl";
            this.lblUrl.Size = new System.Drawing.Size(15, 20);
            this.lblUrl.TabIndex = 2;
            this.lblUrl.Text = "-";
            // 
            // lblVisited
            // 
            this.lblVisited.AutoSize = true;
            this.lblVisited.Location = new System.Drawing.Point(131, 152);
            this.lblVisited.Name = "lblVisited";
            this.lblVisited.Size = new System.Drawing.Size(15, 20);
            this.lblVisited.TabIndex = 3;
            this.lblVisited.Text = "-";
            // 
            // lblYetToVisit
            // 
            this.lblYetToVisit.AutoSize = true;
            this.lblYetToVisit.Location = new System.Drawing.Point(131, 122);
            this.lblYetToVisit.Name = "lblYetToVisit";
            this.lblYetToVisit.Size = new System.Drawing.Size(15, 20);
            this.lblYetToVisit.TabIndex = 4;
            this.lblYetToVisit.Text = "-";
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(131, 96);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(15, 20);
            this.lblTotal.TabIndex = 5;
            this.lblTotal.Text = "-";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(59, 152);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 20);
            this.label1.TabIndex = 6;
            this.label1.Text = "Visited";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(37, 126);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 20);
            this.label2.TabIndex = 7;
            this.label2.Text = "Yet to visit";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(69, 95);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 20);
            this.label3.TabIndex = 8;
            this.label3.Text = "Total";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 188);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(111, 20);
            this.label4.TabIndex = 10;
            this.label4.Text = "Max Yet To visit";
            // 
            // lblMaxYetToVisit
            // 
            this.lblMaxYetToVisit.AutoSize = true;
            this.lblMaxYetToVisit.Location = new System.Drawing.Point(133, 188);
            this.lblMaxYetToVisit.Name = "lblMaxYetToVisit";
            this.lblMaxYetToVisit.Size = new System.Drawing.Size(15, 20);
            this.lblMaxYetToVisit.TabIndex = 9;
            this.lblMaxYetToVisit.Text = "-";
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(577, 54);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(94, 29);
            this.button2.TabIndex = 11;
            this.button2.Text = "Pause";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(133, 262);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(15, 20);
            this.lblStatus.TabIndex = 12;
            this.lblStatus.Text = "-";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(677, 54);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(137, 29);
            this.button3.TabIndex = 13;
            this.button3.Text = "Get All Links";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // txtUrl
            // 
            this.txtUrl.Location = new System.Drawing.Point(131, 54);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(325, 27);
            this.txtUrl.TabIndex = 14;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(82, 57);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(28, 20);
            this.label5.TabIndex = 15;
            this.label5.Text = "Url";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(820, 54);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(176, 29);
            this.button4.TabIndex = 16;
            this.button4.Text = "Get Problematic Links";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(677, 96);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(137, 29);
            this.button5.TabIndex = 17;
            this.button5.Text = "Delete All Links";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1221, 972);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtUrl);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.button2);
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
            this.Text = "Crawler";
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
        private Button button2;
        public Label lblStatus;
        private Button button3;
        private TextBox txtUrl;
        private Label label5;
        private Button button4;
        private Button button5;
    }
}