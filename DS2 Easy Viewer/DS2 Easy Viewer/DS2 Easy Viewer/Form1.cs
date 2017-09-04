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
        public int boxIndex;
        public List<int> textAddParameters = new List<int> { 0, 0, 0, 90, 0, 0, 1, 1, 1 };  // crée une liste de listes des paramètres de text add 
        public List<int> textLocateParameters = new List<int> { 0, 0, 90, 0, 180, 180 };     // crée une liste de listes des paramètres de text locate /// le dernier paramètre est l'opacite de textview
        // [0] RateTime     [1] Azimuth     [2] Elevation   [3] Rotation  [4] Width    [5] height

        public imageBox(Form Form1, int index)
        {
            initializationLayout(Form1);
        }

        public void initializationLayout(Form Form1)
        {
            // AJOUT DU PANNEAU DE PARAMETRES //
            panneauParametres.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.Params_Outline_2;
            panneauParametres.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            panneauParametres.Location = new System.Drawing.Point(910, 12);
            panneauParametres.Size = new System.Drawing.Size(246, 560);
            panneauParametres.TabIndex = 0;

            // AJOUT DES BOUTONS  ALLSKY 
            Allsky_btn.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.AllSky_btn;
            Allsky_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            Allsky_btn.Location = new System.Drawing.Point(6, 6);
            Allsky_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            Allsky_btn.Size = new System.Drawing.Size(76, 17);
            Allsky_btn.TabIndex = 0;
            Allsky_btn.UseVisualStyleBackColor = true;
            Allsky_btn.TabStop = false;
            Allsky_btn.Text = "                       ";
            Allsky_btn.Checked = true;
            panneauParametres.Controls.Add(Allsky_btn);

            // AJOUT DES BOUTONS  IMAGE
            Image_btn.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.Image_btn;
            Image_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            Image_btn.Location = new System.Drawing.Point(85, 6);
            Image_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            Image_btn.Size = new System.Drawing.Size(76, 17);
            Image_btn.TabIndex = 0;
            Image_btn.UseVisualStyleBackColor = true;
            Image_btn.TabStop = false;
            Image_btn.Text = "                       ";
            Image_btn.Checked = false;
            panneauParametres.Controls.Add(Image_btn);

            // AJOUT DES BOUTONS  Pano
            Panorama_btn.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.Pano_btn;
            Panorama_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            Panorama_btn.Location = new System.Drawing.Point(164, 6);
            Panorama_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            Panorama_btn.Size = new System.Drawing.Size(76, 17);
            Panorama_btn.TabIndex = 0;
            Panorama_btn.UseVisualStyleBackColor = true;
            Panorama_btn.TabStop = false;
            Panorama_btn.Text = "                       ";
            Panorama_btn.Checked = false;
            panneauParametres.Controls.Add(Panorama_btn);

            Form1.Controls.Add(panneauParametres);

        }
    }


}
