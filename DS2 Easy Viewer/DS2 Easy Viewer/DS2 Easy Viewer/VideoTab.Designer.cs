namespace DS2_Easy_Viewer
{
    partial class VideoTab
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
            this.GotToPhotoTab_btn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // GotToPhotoTab_btn
            // 
            this.GotToPhotoTab_btn.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.Video_tab_btn_2;
            this.GotToPhotoTab_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.GotToPhotoTab_btn.FlatAppearance.BorderSize = 0;
            this.GotToPhotoTab_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.GotToPhotoTab_btn.Location = new System.Drawing.Point(12, 330);
            this.GotToPhotoTab_btn.Name = "GotToPhotoTab_btn";
            this.GotToPhotoTab_btn.Size = new System.Drawing.Size(899, 22);
            this.GotToPhotoTab_btn.TabIndex = 14;
            this.GotToPhotoTab_btn.UseVisualStyleBackColor = true;
            this.GotToPhotoTab_btn.Click += new System.EventHandler(this.GotToPhotoTab_btn_Click);
            // 
            // VideoTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.ClientSize = new System.Drawing.Size(919, 358);
            this.Controls.Add(this.GotToPhotoTab_btn);
            this.Name = "VideoTab";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "VideoTab";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button GotToPhotoTab_btn;
    }
}