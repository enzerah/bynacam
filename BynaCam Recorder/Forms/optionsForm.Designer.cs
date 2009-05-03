namespace BynaCam_Recorder
{
    partial class optionsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.tb_path = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cb_hideEq = new System.Windows.Forms.CheckBox();
            this.cb_hideVips = new System.Windows.Forms.CheckBox();
            this.cb_hideSkills = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cb_outpm = new System.Windows.Forms.CheckBox();
            this.cb_incomingprivate = new System.Windows.Forms.CheckBox();
            this.cb_default = new System.Windows.Forms.CheckBox();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.tb_path);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(259, 52);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Movies Folder";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(210, 17);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(41, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tb_path
            // 
            this.tb_path.Location = new System.Drawing.Point(6, 19);
            this.tb_path.Name = "tb_path";
            this.tb_path.ReadOnly = true;
            this.tb_path.Size = new System.Drawing.Size(198, 20);
            this.tb_path.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cb_hideEq);
            this.groupBox2.Controls.Add(this.cb_hideVips);
            this.groupBox2.Controls.Add(this.cb_hideSkills);
            this.groupBox2.Enabled = false;
            this.groupBox2.Location = new System.Drawing.Point(12, 70);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(259, 50);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Player";
            // 
            // cb_hideEq
            // 
            this.cb_hideEq.AutoSize = true;
            this.cb_hideEq.Location = new System.Drawing.Point(168, 19);
            this.cb_hideEq.Name = "cb_hideEq";
            this.cb_hideEq.Size = new System.Drawing.Size(66, 17);
            this.cb_hideEq.TabIndex = 2;
            this.cb_hideEq.Text = "Hide EQ";
            this.cb_hideEq.UseVisualStyleBackColor = true;
            // 
            // cb_hideVips
            // 
            this.cb_hideVips.AutoSize = true;
            this.cb_hideVips.Location = new System.Drawing.Point(87, 19);
            this.cb_hideVips.Name = "cb_hideVips";
            this.cb_hideVips.Size = new System.Drawing.Size(75, 17);
            this.cb_hideVips.TabIndex = 1;
            this.cb_hideVips.Text = "Hide VIP\'s";
            this.cb_hideVips.UseVisualStyleBackColor = true;
            // 
            // cb_hideSkills
            // 
            this.cb_hideSkills.AutoSize = true;
            this.cb_hideSkills.Location = new System.Drawing.Point(6, 19);
            this.cb_hideSkills.Name = "cb_hideSkills";
            this.cb_hideSkills.Size = new System.Drawing.Size(75, 17);
            this.cb_hideSkills.TabIndex = 0;
            this.cb_hideSkills.Text = "Hide Skills";
            this.cb_hideSkills.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cb_outpm);
            this.groupBox3.Controls.Add(this.cb_incomingprivate);
            this.groupBox3.Controls.Add(this.cb_default);
            this.groupBox3.Enabled = false;
            this.groupBox3.Location = new System.Drawing.Point(12, 126);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(204, 92);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Messages";
            // 
            // cb_outpm
            // 
            this.cb_outpm.AutoSize = true;
            this.cb_outpm.Location = new System.Drawing.Point(6, 65);
            this.cb_outpm.Name = "cb_outpm";
            this.cb_outpm.Size = new System.Drawing.Size(177, 17);
            this.cb_outpm.TabIndex = 3;
            this.cb_outpm.Text = "Hide outgoing private messages";
            this.cb_outpm.UseVisualStyleBackColor = true;
            // 
            // cb_incomingprivate
            // 
            this.cb_incomingprivate.AutoSize = true;
            this.cb_incomingprivate.Location = new System.Drawing.Point(6, 42);
            this.cb_incomingprivate.Name = "cb_incomingprivate";
            this.cb_incomingprivate.Size = new System.Drawing.Size(178, 17);
            this.cb_incomingprivate.TabIndex = 2;
            this.cb_incomingprivate.Text = "Hide incoming private messages";
            this.cb_incomingprivate.UseVisualStyleBackColor = true;
            // 
            // cb_default
            // 
            this.cb_default.AutoSize = true;
            this.cb_default.Location = new System.Drawing.Point(6, 19);
            this.cb_default.Name = "cb_default";
            this.cb_default.Size = new System.Drawing.Size(133, 17);
            this.cb_default.TabIndex = 0;
            this.cb_default.Text = "Hide default messages";
            this.cb_default.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(222, 135);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(49, 73);
            this.button2.TabIndex = 3;
            this.button2.Text = "Close";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // optionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(285, 225);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "optionsForm";
            this.Text = "Options";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.TextBox tb_path;
        public System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.GroupBox groupBox2;
        public System.Windows.Forms.GroupBox groupBox3;
        public System.Windows.Forms.Button button2;
        public System.Windows.Forms.Button button1;
        public System.Windows.Forms.CheckBox cb_hideEq;
        public System.Windows.Forms.CheckBox cb_hideVips;
        public System.Windows.Forms.CheckBox cb_hideSkills;
        public System.Windows.Forms.CheckBox cb_incomingprivate;
        public System.Windows.Forms.CheckBox cb_default;
        public System.Windows.Forms.CheckBox cb_outpm;
    }
}