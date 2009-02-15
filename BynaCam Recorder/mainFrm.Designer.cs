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
            this.cb_vips = new System.Windows.Forms.CheckBox();
            this.cb_messages = new System.Windows.Forms.CheckBox();
            this.cb_pms = new System.Windows.Forms.CheckBox();
            this.cb_skills = new System.Windows.Forms.CheckBox();
            this.contextMenuStrip1.SuspendLayout();
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
            // cb_vips
            // 
            this.cb_vips.AutoSize = true;
            this.cb_vips.Location = new System.Drawing.Point(13, 13);
            this.cb_vips.Name = "cb_vips";
            this.cb_vips.Size = new System.Drawing.Size(75, 17);
            this.cb_vips.TabIndex = 1;
            this.cb_vips.Text = "Hide VIP\'s";
            this.cb_vips.UseVisualStyleBackColor = true;
            this.cb_vips.CheckedChanged += new System.EventHandler(this.cb_vips_CheckedChanged);
            // 
            // cb_messages
            // 
            this.cb_messages.AutoSize = true;
            this.cb_messages.Location = new System.Drawing.Point(13, 37);
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
            this.cb_pms.Location = new System.Drawing.Point(13, 61);
            this.cb_pms.Name = "cb_pms";
            this.cb_pms.Size = new System.Drawing.Size(133, 17);
            this.cb_pms.TabIndex = 3;
            this.cb_pms.Text = "Hide private messages";
            this.cb_pms.UseVisualStyleBackColor = true;
            this.cb_pms.CheckedChanged += new System.EventHandler(this.cb_pms_CheckedChanged);
            // 
            // cb_skills
            // 
            this.cb_skills.AutoSize = true;
            this.cb_skills.Location = new System.Drawing.Point(13, 85);
            this.cb_skills.Name = "cb_skills";
            this.cb_skills.Size = new System.Drawing.Size(73, 17);
            this.cb_skills.TabIndex = 4;
            this.cb_skills.Text = "Hide skills";
            this.cb_skills.UseVisualStyleBackColor = true;
            this.cb_skills.CheckedChanged += new System.EventHandler(this.cb_skills_CheckedChanged);
            // 
            // mainFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(195, 110);
            this.Controls.Add(this.cb_skills);
            this.Controls.Add(this.cb_pms);
            this.Controls.Add(this.cb_messages);
            this.Controls.Add(this.cb_vips);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "mainFrm";
            this.Text = "BynaCam v2 - Options";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.mainFrm_FormClosing);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem saveAndExitToolStripMenuItem;
        public System.Windows.Forms.NotifyIcon notifyIcon1;
        public System.Windows.Forms.CheckBox cb_vips;
        public System.Windows.Forms.CheckBox cb_messages;
        public System.Windows.Forms.CheckBox cb_pms;
        public System.Windows.Forms.CheckBox cb_skills;


    }
}

