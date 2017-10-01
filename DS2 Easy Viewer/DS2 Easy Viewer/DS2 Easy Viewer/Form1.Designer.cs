namespace DS2_Easy_Viewer
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.capture_btn = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.GoToVideoTab_btn = new System.Windows.Forms.Button();
            this.ClearAll_btn = new System.Windows.Forms.Button();
            this.Select_Multi_btn = new System.Windows.Forms.Button();
            this.sauvegarde_btn = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // capture_btn
            // 
            this.capture_btn.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.capture;
            this.capture_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.capture_btn.FlatAppearance.BorderSize = 0;
            this.capture_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.capture_btn.Location = new System.Drawing.Point(1175, 235);
            this.capture_btn.Name = "capture_btn";
            this.capture_btn.Size = new System.Drawing.Size(216, 22);
            this.capture_btn.TabIndex = 15;
            this.capture_btn.UseVisualStyleBackColor = true;
            this.capture_btn.Click += new System.EventHandler(this.capture_btn_Click);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.Panel_On_2;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Location = new System.Drawing.Point(1175, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(216, 216);
            this.panel1.TabIndex = 14;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(5, 5);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(206, 206);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // GoToVideoTab_btn
            // 
            this.GoToVideoTab_btn.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.Video_tab_btn_2;
            this.GoToVideoTab_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.GoToVideoTab_btn.Enabled = false;
            this.GoToVideoTab_btn.FlatAppearance.BorderSize = 0;
            this.GoToVideoTab_btn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.GoToVideoTab_btn.Location = new System.Drawing.Point(9, 710);
            this.GoToVideoTab_btn.Name = "GoToVideoTab_btn";
            this.GoToVideoTab_btn.Size = new System.Drawing.Size(899, 22);
            this.GoToVideoTab_btn.TabIndex = 13;
            this.GoToVideoTab_btn.UseVisualStyleBackColor = true;
            // 
            // ClearAll_btn
            // 
            this.ClearAll_btn.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.ClearResetAll;
            this.ClearAll_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClearAll_btn.FlatAppearance.BorderSize = 0;
            this.ClearAll_btn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ClearAll_btn.Location = new System.Drawing.Point(914, 710);
            this.ClearAll_btn.Name = "ClearAll_btn";
            this.ClearAll_btn.Size = new System.Drawing.Size(239, 22);
            this.ClearAll_btn.TabIndex = 12;
            this.ClearAll_btn.UseVisualStyleBackColor = true;
            this.ClearAll_btn.Click += new System.EventHandler(this.ClearAll_btn_Click);
            // 
            // Select_Multi_btn
            // 
            this.Select_Multi_btn.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.Multiple_Select_1;
            this.Select_Multi_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Select_Multi_btn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Select_Multi_btn.Location = new System.Drawing.Point(9, 12);
            this.Select_Multi_btn.Name = "Select_Multi_btn";
            this.Select_Multi_btn.Size = new System.Drawing.Size(24, 691);
            this.Select_Multi_btn.TabIndex = 10;
            this.Select_Multi_btn.UseVisualStyleBackColor = true;
            this.Select_Multi_btn.Click += new System.EventHandler(this.Select_Multi_btn_Click);
            // 
            // sauvegarde_btn
            // 
            this.sauvegarde_btn.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.sauvegarde;
            this.sauvegarde_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.sauvegarde_btn.FlatAppearance.BorderSize = 0;
            this.sauvegarde_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sauvegarde_btn.Location = new System.Drawing.Point(1175, 263);
            this.sauvegarde_btn.Name = "sauvegarde_btn";
            this.sauvegarde_btn.Size = new System.Drawing.Size(216, 22);
            this.sauvegarde_btn.TabIndex = 16;
            this.sauvegarde_btn.UseVisualStyleBackColor = true;
            this.sauvegarde_btn.Click += new System.EventHandler(this.sauvegarde_btn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1165, 980);
            this.Controls.Add(this.sauvegarde_btn);
            this.Controls.Add(this.capture_btn);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.GoToVideoTab_btn);
            this.Controls.Add(this.ClearAll_btn);
            this.Controls.Add(this.Select_Multi_btn);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DS2 Easy Viewer";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Select_Multi_btn;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button ClearAll_btn;
        private System.Windows.Forms.Button GoToVideoTab_btn;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button capture_btn;
        private System.Windows.Forms.Button sauvegarde_btn;
    }
}

