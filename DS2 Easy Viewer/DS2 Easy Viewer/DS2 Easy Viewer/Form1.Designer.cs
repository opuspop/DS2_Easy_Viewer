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
            this.Select_Multi_btn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Select_Multi_btn
            // 
            this.Select_Multi_btn.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.Multiple_Select_2;
            this.Select_Multi_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Select_Multi_btn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Select_Multi_btn.Location = new System.Drawing.Point(6, 12);
            this.Select_Multi_btn.Name = "Select_Multi_btn";
            this.Select_Multi_btn.Size = new System.Drawing.Size(30, 456);
            this.Select_Multi_btn.TabIndex = 10;
            this.Select_Multi_btn.UseVisualStyleBackColor = true;
            this.Select_Multi_btn.Click += new System.EventHandler(this.Select_Multi_btn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.ClientSize = new System.Drawing.Size(1171, 715);
            this.Controls.Add(this.Select_Multi_btn);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Select_Multi_btn;
    }
}

