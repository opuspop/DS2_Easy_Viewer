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
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.ResetAll_btn = new System.Windows.Forms.Button();
            this.Select_Multi_btn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ResetAll_btn
            // 
            this.ResetAll_btn.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.Reset_Reset_All_2;
            this.ResetAll_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ResetAll_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ResetAll_btn.Location = new System.Drawing.Point(919, 710);
            this.ResetAll_btn.Name = "ResetAll_btn";
            this.ResetAll_btn.Size = new System.Drawing.Size(120, 22);
            this.ResetAll_btn.TabIndex = 11;
            this.ResetAll_btn.UseVisualStyleBackColor = true;
            this.ResetAll_btn.Click += new System.EventHandler(this.ResetAll_btn_Click);
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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.ClientSize = new System.Drawing.Size(1171, 744);
            this.Controls.Add(this.ResetAll_btn);
            this.Controls.Add(this.Select_Multi_btn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Select_Multi_btn;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button ResetAll_btn;
    }
}

