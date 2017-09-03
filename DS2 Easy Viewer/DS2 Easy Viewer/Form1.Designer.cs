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
        public void InitializeComponent()
        {
            System.Windows.Forms.Label Rotation_Image_lbl;
            this.panel1 = new System.Windows.Forms.Panel();
            this.Rotation_Panel = new System.Windows.Forms.Panel();
            this.slider_rotation = new System.Windows.Forms.TrackBar();
            Rotation_Image_lbl = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.Rotation_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.slider_rotation)).BeginInit();
            this.SuspendLayout();
            // 
            // Rotation_Image_lbl
            // 
            Rotation_Image_lbl.AutoSize = true;
            Rotation_Image_lbl.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            Rotation_Image_lbl.ForeColor = System.Drawing.Color.White;
            Rotation_Image_lbl.Location = new System.Drawing.Point(13, 35);
            Rotation_Image_lbl.Name = "Rotation_Image_lbl";
            Rotation_Image_lbl.Size = new System.Drawing.Size(46, 13);
            Rotation_Image_lbl.TabIndex = 1;
            Rotation_Image_lbl.Text = "Rotation";
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.Params_Outline_2;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.panel1.Controls.Add(this.Rotation_Panel);
            this.panel1.Location = new System.Drawing.Point(909, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(246, 560);
            this.panel1.TabIndex = 0;
            // 
            // Rotation_Panel
            // 
            this.Rotation_Panel.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.sliderBox_2;
            this.Rotation_Panel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Rotation_Panel.Controls.Add(Rotation_Image_lbl);
            this.Rotation_Panel.Controls.Add(this.slider_rotation);
            this.Rotation_Panel.Location = new System.Drawing.Point(8, 11);
            this.Rotation_Panel.Name = "Rotation_Panel";
            this.Rotation_Panel.Size = new System.Drawing.Size(230, 56);
            this.Rotation_Panel.TabIndex = 1;
            // 
            // slider_rotation
            // 
            this.slider_rotation.Location = new System.Drawing.Point(82, 8);
            this.slider_rotation.Name = "slider_rotation";
            this.slider_rotation.Size = new System.Drawing.Size(104, 45);
            this.slider_rotation.TabIndex = 1;
            // 
            // Form1
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.ClientSize = new System.Drawing.Size(1166, 583);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "DS2 Easy View";
            this.panel1.ResumeLayout(false);
            this.Rotation_Panel.ResumeLayout(false);
            this.Rotation_Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.slider_rotation)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.Panel Rotation_Panel;
        public System.Windows.Forms.TrackBar slider_rotation;
    }
}

