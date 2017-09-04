namespace Test_Paths
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
            this.button1 = new System.Windows.Forms.Button();
            this.box = new System.Windows.Forms.PictureBox();
            this.chemin = new System.Windows.Forms.Label();
            this.envoyer = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.box)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(165, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(107, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Load";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // box
            // 
            this.box.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.box.Location = new System.Drawing.Point(79, 81);
            this.box.Name = "box";
            this.box.Size = new System.Drawing.Size(107, 108);
            this.box.TabIndex = 1;
            this.box.TabStop = false;
            // 
            // chemin
            // 
            this.chemin.AutoSize = true;
            this.chemin.Location = new System.Drawing.Point(79, 78);
            this.chemin.Name = "chemin";
            this.chemin.Size = new System.Drawing.Size(0, 13);
            this.chemin.TabIndex = 2;
            // 
            // envoyer
            // 
            this.envoyer.Location = new System.Drawing.Point(79, 212);
            this.envoyer.Name = "envoyer";
            this.envoyer.Size = new System.Drawing.Size(107, 23);
            this.envoyer.TabIndex = 3;
            this.envoyer.Text = "envoyer";
            this.envoyer.UseVisualStyleBackColor = true;
            this.envoyer.Click += new System.EventHandler(this.envoyer_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(13, 14);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 4;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.envoyer);
            this.Controls.Add(this.chemin);
            this.Controls.Add(this.box);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.box)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox box;
        private System.Windows.Forms.Label chemin;
        private System.Windows.Forms.Button envoyer;
        private System.Windows.Forms.TextBox textBox1;
    }
}

