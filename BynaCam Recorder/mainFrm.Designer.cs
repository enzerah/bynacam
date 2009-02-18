namespace BynaCam_Recorder
{
    partial class mainFrm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mainFrm));
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.saveAndExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cb_messages = new System.Windows.Forms.CheckBox();
            this.cb_pms = new System.Windows.Forms.CheckBox();
            this.cb_outpm = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ot_port = new System.Windows.Forms.NumericUpDown();
            this.ot_server = new System.Windows.Forms.TextBox();
            this.ot_enable = new System.Windows.Forms.CheckBox();
            this.btn_start = new System.Windows.Forms.Button();
            this.contextMenuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ot_port)).BeginInit();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "BynaCam - Recorder";
            this.notifyIcon1.Visible = true;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveAndExitToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(152, 26);
            // 
            // saveAndExitToolStripMenuItem
            // 
            this.saveAndExitToolStripMenuItem.Name = "saveAndExitToolStripMenuItem";
            this.saveAndExitToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.saveAndExitToolStripMenuItem.Text = "Save and Exit";
            this.saveAndExitToolStripMenuItem.Click += new System.EventHandler(this.saveAndExitToolStripMenuItem_Click);
            // 
            // cb_messages
            // 
            this.cb_messages.AutoSize = true;
            this.cb_messages.Location = new System.Drawing.Point(6, 19);
            this.cb_messages.Name = "cb_messages";
            this.cb_messages.Size = new System.Drawing.Size(174, 17);
            this.cb_messages.TabIndex = 2;
            this.cb_messages.Text = "Hide default channel messages";
            this.cb_messages.UseVisualStyleBackColor = true;
            this.cb_messages.CheckedChanged += new System.EventHandler(this.cb_messages_CheckedChanged);
            // 
            // cb_pms
            // 
            this.cb_pms.AutoSize = true;
            this.cb_pms.Location = new System.Drawing.Point(6, 42);
            this.cb_pms.Name = "cb_pms";
            this.cb_pms.Size = new System.Drawing.Size(178, 17);
            this.cb_pms.TabIndex = 3;
            this.cb_pms.Text = "Hide incoming private messages";
            this.cb_pms.UseVisualStyleBackColor = true;
            this.cb_pms.CheckedChanged += new System.EventHandler(this.cb_pms_CheckedChanged);
            // 
            // cb_outpm
            // 
            this.cb_outpm.AutoSize = true;
            this.cb_outpm.Location = new System.Drawing.Point(6, 65);
            this.cb_outpm.Name = "cb_outpm";
            this.cb_outpm.Size = new System.Drawing.Size(177, 17);
            this.cb_outpm.TabIndex = 4;
            this.cb_outpm.Text = "Hide outgoing private messages";
            this.cb_outpm.UseVisualStyleBackColor = true;
            this.cb_outpm.CheckedChanged += new System.EventHandler(this.cb_outpm_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cb_messages);
            this.groupBox1.Controls.Add(this.cb_pms);
            this.groupBox1.Controls.Add(this.cb_outpm);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(185, 94);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Messages";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.ot_port);
            this.groupBox2.Controls.Add(this.ot_server);
            this.groupBox2.Controls.Add(this.ot_enable);
            this.groupBox2.Location = new System.Drawing.Point(12, 112);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(185, 72);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Open Tibia";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.Location = new System.Drawing.Point(108, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(13, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = ":";
            // 
            // ot_port
            // 
            this.ot_port.Enabled = false;
            this.ot_port.Location = new System.Drawing.Point(127, 43);
            this.ot_port.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.ot_port.Minimum = new decimal(new int[] {
            80,
            0,
            0,
            0});
            this.ot_port.Name = "ot_port";
            this.ot_port.Size = new System.Drawing.Size(44, 20);
            this.ot_port.TabIndex = 2;
            this.ot_port.Value = new decimal(new int[] {
            7171,
            0,
            0,
            0});
            // 
            // ot_server
            // 
            this.ot_server.Enabled = false;
            this.ot_server.Location = new System.Drawing.Point(15, 43);
            this.ot_server.Name = "ot_server";
            this.ot_server.Size = new System.Drawing.Size(87, 20);
            this.ot_server.TabIndex = 1;
            // 
            // ot_enable
            // 
            this.ot_enable.AutoSize = true;
            this.ot_enable.Location = new System.Drawing.Point(6, 20);
            this.ot_enable.Name = "ot_enable";
            this.ot_enable.Size = new System.Drawing.Size(63, 17);
            this.ot_enable.TabIndex = 0;
            this.ot_enable.Text = "Use OT";
            this.ot_enable.UseVisualStyleBackColor = true;
            this.ot_enable.CheckedChanged += new System.EventHandler(this.ot_enable_CheckedChanged);
            // 
            // btn_start
            // 
            this.btn_start.Location = new System.Drawing.Point(12, 187);
            this.btn_start.Name = "btn_start";
            this.btn_start.Size = new System.Drawing.Size(185, 21);
            this.btn_start.TabIndex = 7;
            this.btn_start.Text = "Start Recording";
            this.btn_start.UseVisualStyleBackColor = true;
            this.btn_start.Click += new System.EventHandler(this.btn_start_Click);
            // 
            // mainFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(215, 213);
            this.Controls.Add(this.btn_start);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "mainFrm";
            this.Text = "BynaCam v2.4 - Options";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.mainFrm_FormClosing);
            this.contextMenuStrip1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ot_port)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem saveAndExitToolStripMenuItem;
        public System.Windows.Forms.NotifyIcon notifyIcon1;
        public System.Windows.Forms.CheckBox cb_messages;
        public System.Windows.Forms.CheckBox cb_pms;
        public System.Windows.Forms.CheckBox cb_outpm;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown ot_port;
        private System.Windows.Forms.TextBox ot_server;
        private System.Windows.Forms.CheckBox ot_enable;
        private System.Windows.Forms.Button btn_start;


    }
}

