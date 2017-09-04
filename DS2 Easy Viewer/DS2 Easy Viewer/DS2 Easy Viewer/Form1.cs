using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DS2_Easy_Viewer
{
    public partial class Form1 : Form
    {
        List<imageBox> imageBoxList = new List<imageBox>();

        public Form1()
        {
            InitializeComponent();
            imageBox box = new imageBox(this, 1);
            imageBoxList.Add(box);
        }
    }





    public class imageBox
    {
        public string image_Path;
        TextBox chemin = new TextBox(); 
        PictureBox box = new PictureBox();
        Button envoyer_btn = new Button(); RadioButton Allsky_btn = new RadioButton(); RadioButton Panorama_btn = new RadioButton(); RadioButton Image_btn = new RadioButton();
        Panel panneauParametres = new System.Windows.Forms.Panel(); Panel panneauRotation = new System.Windows.Forms.Panel(); Panel panneauAzimuth = new System.Windows.Forms.Panel();
        Panel panneauElevation = new System.Windows.Forms.Panel(); Panel panneauWidth = new System.Windows.Forms.Panel(); Panel panneauHeight = new System.Windows.Forms.Panel();
        Panel panneauImage = new Panel();
        TrackBar Slider_Rotation = new TrackBar(); TrackBar Slider_Azimuth = new TrackBar(); TrackBar Slider_Elevation = new TrackBar(); TrackBar Slider_Width = new TrackBar(); TrackBar Slider_Height = new TrackBar();
        Label slider_Rotation_lbl = new Label(); Label slider_Azimuth_lbl = new Label(); Label slider_Elevation_lbl = new Label(); Label slider_Width_lbl = new Label(); Label slider_Height_lbl = new Label();
        TextBox slider_Rotation_txt = new TextBox(); TextBox slider_Azimuth_txt = new TextBox(); TextBox slider_Elevation_txt = new TextBox(); TextBox slider_Width_txt = new TextBox(); TextBox slider_Height_txt = new TextBox();
        public int boxIndex;
        public List<int> textAddParameters = new List<int> { 0, 0, 0, 90, 0, 0, 1, 1, 1 };  // crée une liste de listes des paramètres de text add 
        public List<int> textLocateParameters = new List<int> { 0, 0, 90, 0, 180, 180 };     // crée une liste de listes des paramètres de text locate /// le dernier paramètre est l'opacite de textview
        // [0] RateTime     [1] Azimuth     [2] Elevation   [3] Rotation  [4] Width    [5] height

        public imageBox(Form Form1, int index)
        {
            initializationLayout(Form1, index);
            slider_Rotation_txt.Text = "0"; slider_Azimuth_txt.Text = "90"; slider_Azimuth_txt.Text = "0"; slider_Azimuth_txt.Text = "180"; slider_Azimuth_txt.Text = "180";
        }

        public void initializationLayout(Form Form1, int index)
        {
            // AJOUT DU PANNEAU DE PARAMETRES //
            panneauParametres.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.Params_Outline_2;
            panneauParametres.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            panneauParametres.Location = new System.Drawing.Point(910, 12);
            panneauParametres.Size = new System.Drawing.Size(246, 560);
            panneauParametres.TabIndex = 0;

            // AJOUT DES BOUTONS  ALLSKY 
            Allsky_btn.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.AllSky_btn_3;
            Allsky_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            Allsky_btn.Location = new System.Drawing.Point(8, 8);
            Allsky_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            Allsky_btn.Size = new System.Drawing.Size(75, 20);
            Allsky_btn.TabIndex = 0;
            Allsky_btn.UseVisualStyleBackColor = true;
            Allsky_btn.TabStop = false;
            Allsky_btn.Text = "                       ";
            Allsky_btn.Checked = true;
            panneauParametres.Controls.Add(Allsky_btn);

            // AJOUT DES BOUTONS  IMAGE
            Image_btn.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.Image_btn_3;
            Image_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            Image_btn.Location = new System.Drawing.Point(85, 8);
            Image_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            Image_btn.Size = new System.Drawing.Size(75, 20);
            Image_btn.TabIndex = 0;
            Image_btn.UseVisualStyleBackColor = true;
            Image_btn.TabStop = false;
            Image_btn.Text = "                       ";
            Image_btn.Checked = false;
            panneauParametres.Controls.Add(Image_btn);

            // AJOUT DES BOUTONS  PANO
            Panorama_btn.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.Pano_btn_3;
            Panorama_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            Panorama_btn.Location = new System.Drawing.Point(162, 8);
            Panorama_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            Panorama_btn.Size = new System.Drawing.Size(75, 20);
            Panorama_btn.TabIndex = 0;
            Panorama_btn.UseVisualStyleBackColor = true;
            Panorama_btn.TabStop = false;
            Panorama_btn.Text = "                       ";
            Panorama_btn.Checked = false;
            panneauParametres.Controls.Add(Panorama_btn);

            // AJOUT DU PANNEAU DE ROTATION //
            panneauRotation.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.sliderBox_2;
            panneauRotation.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            panneauRotation.Location = new System.Drawing.Point(8, 34);
            panneauRotation.Size = new System.Drawing.Size(230, 56);
            panneauRotation.TabIndex = 0;

            // AJOUT SLIDER ROTATION //
            // 
            Slider_Rotation.Location = new System.Drawing.Point(3, 6);
            Slider_Rotation.Maximum = 180;
            Slider_Rotation.Minimum = -180;
            Slider_Rotation.Size = new System.Drawing.Size(224, 45);
            Slider_Rotation.TabIndex = 0;
            Slider_Rotation.TickFrequency = 0;
            Slider_Rotation.TickStyle = System.Windows.Forms.TickStyle.None;
            panneauRotation.Controls.Add(Slider_Rotation);

            //  AJOUT NOM Rotation SLIDER   //
            slider_Rotation_lbl.AutoSize = true;
            slider_Rotation_lbl.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            slider_Rotation_lbl.ForeColor = System.Drawing.Color.White;
            slider_Rotation_lbl.Location = new System.Drawing.Point(13, 35);
            slider_Rotation_lbl.Size = new System.Drawing.Size(46, 13);
            slider_Rotation_lbl.TabIndex = 3;
            slider_Rotation_lbl.Text = "Rotation";
            panneauRotation.Controls.Add(slider_Rotation_lbl);

            // AJOUT TEXTBOX VALEUR DE ROTATION

            slider_Rotation_txt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            slider_Rotation_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            slider_Rotation_txt.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            slider_Rotation_txt.ForeColor = System.Drawing.Color.White;
            slider_Rotation_txt.Location = new System.Drawing.Point(198, 34);
            slider_Rotation_txt.Size = new System.Drawing.Size(20, 14);
            slider_Rotation_txt.TabIndex = 2;
            panneauRotation.Controls.Add(slider_Rotation_txt);

            
            

            // AJOUT DU PANNEAU DE AZIMUTH //
            panneauAzimuth.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.sliderBox_2;
            panneauAzimuth.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            panneauAzimuth.Location = new System.Drawing.Point(8, 96);
            panneauAzimuth.Size = new System.Drawing.Size(230, 56);
            panneauAzimuth.TabIndex = 0;

            // AJOUT SLIDER AZIMUTH //
            // 
            Slider_Azimuth.Location = new System.Drawing.Point(3, 6);
            Slider_Azimuth.Maximum = 90;
            Slider_Azimuth.Minimum = -90;
            Slider_Azimuth.Size = new System.Drawing.Size(224, 45);
            Slider_Azimuth.TabIndex = 0;
            Slider_Azimuth.TickFrequency = 0;
            Slider_Azimuth.TickStyle = System.Windows.Forms.TickStyle.None;
            panneauAzimuth.Controls.Add(Slider_Azimuth);

            //  AJOUT NOM AZIMUTH SLIDER   //
            slider_Azimuth_lbl.AutoSize = true;
            slider_Azimuth_lbl.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            slider_Azimuth_lbl.ForeColor = System.Drawing.Color.White;
            slider_Azimuth_lbl.Location = new System.Drawing.Point(13, 35);
            slider_Azimuth_lbl.Size = new System.Drawing.Size(46, 13);
            slider_Azimuth_lbl.TabIndex = 3;
            slider_Azimuth_lbl.Text = "Azimuth";
            panneauAzimuth.Controls.Add(slider_Azimuth_lbl);

            // AJOUT TEXTBOX VALEUR DE AZIMUTH

            slider_Azimuth_txt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            slider_Azimuth_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            slider_Azimuth_txt.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            slider_Azimuth_txt.ForeColor = System.Drawing.Color.White;
            slider_Azimuth_txt.Location = new System.Drawing.Point(198, 34);
            slider_Azimuth_txt.Size = new System.Drawing.Size(20, 14);
            slider_Azimuth_txt.TabIndex = 2;
            panneauAzimuth.Controls.Add(slider_Azimuth_txt);

            
            panneauParametres.Controls.Add(panneauRotation);
            panneauParametres.Controls.Add(panneauAzimuth);
            Slider_Azimuth.SendToBack();
            Slider_Rotation.SendToBack();


            Form1.Controls.Add(panneauParametres);

        }
    }


}
