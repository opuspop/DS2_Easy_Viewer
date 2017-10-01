﻿using AxWMPLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WMPLib;
using System.Text.RegularExpressions;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using Accord.Video.FFMPEG;
using Accord.Video;

namespace DS2_Easy_Viewer
{
   
    public partial class Form1 : Form
    {
        public static List<imageBox> imageBoxList = new List<imageBox>();
        public static List<videoBox> videoBoxList = new List<videoBox>();
        public static int boiteSelectionnee;
        public static bool DEBUG = false;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }
        public Form1()
        {
            InitializeComponent();

            for (int i = 0; i < 15; i++)
            {
                imageBox box = new imageBox(this, i);
                imageBoxList.Add(box);
            }
            imageBoxList[boiteSelectionnee].panneauParametres.Visible = true;
            imageBoxList[0].imageBox_Click(this, EventArgs.Empty);

            videoBox videoB = new videoBox(this, 0);
            videoBoxList.Add(videoB);
            videoBoxList[0].wmPlayer.PlayStateChange += new AxWMPLib._WMPOCXEvents_PlayStateChangeEventHandler(playChange_event);
        }
        void playChange_event(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
           if (e.newState == 3)
            {
                //currentTime.Text = videoBoxList[0].wmPlayer.Ctlcontrols.currentPositionString;
            }
        }
        private void Select_Multi_btn_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Multiselect = true;
            openFileDialog1.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.tif;*.tiff*,.tga...";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                int increment = 0;
                try
                {
                    foreach (string files in openFileDialog1.FileNames)
                    {
                        imageBoxList[increment].loadImage(files);
                        increment += 1;
                    }


                }
                catch (Exception) { }
            }
        }
        private void ClearAll_btn_Click(object sender, EventArgs e)
        {
            try
            {
                imageBoxList[0].resetAll(); // demande à Ds-01 de faire un Reset Reset All de Ds2
                foreach (imageBox imgBox in imageBoxList)
                {
                    imgBox.remove_Click(this, EventArgs.Empty);

                }
                imageBoxList[0].imageBox_Click(this, EventArgs.Empty);
                foreach (videoBox vidBox in videoBoxList)
                {
                    vidBox.remove_Click(this, EventArgs.Empty);
                }
            }
            catch (Exception) { }
        }

        private void capture_btn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Fonction de capture d'images à venir");
        }

        private void sauvegarde_btn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Ça aussi ça s'en vient, patience!\nY'a pas assez d'heures dans une journée");
        }
    }
    public partial class imageBox
    {
        bool DEBUG = Form1.DEBUG;
        public string image_Path; public string imageRenommee = "";
        TextBox chemin = new TextBox(); 
        PictureBox box = new PictureBox();
        PictureBox imgSelect = new PictureBox();
        Label imgSelectLbl = new Label();
        public Button envoyer_btn = new Button(); Button Allsky_btn = new Button(); Button Panorama_btn = new Button(); Button Image_btn = new Button();
        Button ratio_btn = new Button();
        public Panel panneau = new Panel(); // Panneau de l'image
        public Panel panneauParametres = new System.Windows.Forms.Panel(); Panel panneauRotation = new System.Windows.Forms.Panel(); Panel panneauAzimuth = new System.Windows.Forms.Panel();
        Panel panneauElevation = new System.Windows.Forms.Panel(); Panel panneauWidth = new System.Windows.Forms.Panel(); Panel panneauHeight = new System.Windows.Forms.Panel();
        Panel panneauImage = new Panel();
        Button remove = new Button(); Button copieScript_btn = new Button();
        public TrackBar Slider_Rotation = new TrackBar(); TrackBar Slider_Azimuth = new TrackBar(); TrackBar Slider_Elevation = new TrackBar();  TrackBar Slider_Width = new TrackBar(); TrackBar Slider_Height = new TrackBar();
        Label slider_Rotation_lbl = new Label();  Label slider_Azimuth_lbl = new Label();  Label slider_Elevation_lbl = new Label();  Label slider_Width_lbl = new Label(); Label slider_Height_lbl = new Label();
        public TextBox slider_Rotation_txt = new TextBox(); TextBox slider_Azimuth_txt = new TextBox(); TextBox slider_Elevation_txt = new TextBox(); TextBox slider_Width_txt = new TextBox(); TextBox slider_Height_txt = new TextBox();
        Button resetRotation_btn = new Button(); Button resetAzimuth_btn = new Button(); Button resetElevation_btn = new Button(); Button resetWidth_btn = new Button(); Button resetHeight_btn = new Button();
        TextBox nomImage_txtBox = new TextBox(); ListBox scriptOutput_txtBox = new ListBox();
        private static int boxIndex;
        public  List<int> textAddParameters = new List<int> { 0, 0, 0, 90, 0, 0, 1, 1};  // crée une liste de listes des paramètres de text add 
        public  List<int> textLocateParameters = new List<int> { 0, 0, 90, 0, 180, 180 };     // crée une liste de listes des paramètres de text locate /// le dernier paramètre est l'opacite de textview
        // [0] RateTime     [1] Azimuth     [2] Elevation   [3] Rotation  [4] Width    [5] height
        private  List<TrackBar> listDeSliders = new List<TrackBar>();
        private  List<TextBox> listDeTextBox = new List<TextBox>();
        public static List<string> listeValeurDefautText = new List<string> { "0","0", "90", "0", "180", "180" };
        int count = 0; int modeDisplay = 0;
        double ratio = 1;
        public string nomImage = "";
        public static bool ratioOn = true; public bool surDome = false;
        public BackgroundWorker bw_loadImage = new BackgroundWorker();
        public imageBox(Form Form1, int index)
        {
            count += 1;
            initializationLayoutParameters(Form1, index);
            initializationImageBox(Form1, index);
         
            listDeSliders.Add(Slider_Azimuth); listDeSliders.Add(Slider_Elevation); listDeSliders.Add(Slider_Rotation); listDeSliders.Add(Slider_Width); listDeSliders.Add(Slider_Height);
            listDeTextBox.Add(slider_Azimuth_txt); listDeTextBox.Add(slider_Elevation_txt); listDeTextBox.Add(slider_Rotation_txt); listDeTextBox.Add(slider_Width_txt); listDeTextBox.Add(slider_Height_txt);
            setInitialParameterValues();  // set les textbox avec les valeurs par défaut de ListeValeurDefautTex
            bw_loadImage.WorkerSupportsCancellation = false;
            bw_loadImage.WorkerReportsProgress = false;
            bw_loadImage.DoWork += new DoWorkEventHandler(backgroundWorker_loadImage);

           
        }
        private void initializationLayoutParameters(Form Form1, int index)
        {
            // AJOUT DU PANNEAU DE PARAMETRES //
            panneauParametres.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.Params_Outline_2;
            panneauParametres.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            panneauParametres.Location = new System.Drawing.Point(910, 12);
            panneauParametres.Size = new System.Drawing.Size(246, 692);
            panneauParametres.TabIndex = 0;

            // AJOUT DU PICTUREBOX DE L'IMAGE SELECTIONNEE
            imgSelect.Location = new Point(8, 8);
            imgSelect.Size = new Size(115, 115);
            imgSelect.BorderStyle = BorderStyle.FixedSingle;
            panneauParametres.Controls.Add(imgSelect);

            // AJOUT DU NOM DE L'IMAGE SELECTIONNEE
            imgSelectLbl.AutoSize = false;
            imgSelectLbl.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            imgSelectLbl.ForeColor = System.Drawing.Color.White;
            imgSelectLbl.Location = new System.Drawing.Point(129, 12);
            imgSelectLbl.Size = new System.Drawing.Size(110, 13);
            imgSelectLbl.TabIndex = 3;
            imgSelectLbl.Text = " ";
            imgSelectLbl.RightToLeft = RightToLeft.Yes;
            panneauParametres.Controls.Add(imgSelectLbl);


            // AJOUT DES BOUTONS  ALLSKY 
            Allsky_btn.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.AllSky_btn_On;
            Allsky_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            Allsky_btn.Location = new System.Drawing.Point(129, 48);
            Allsky_btn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            Allsky_btn.Size = new System.Drawing.Size(110, 20);
            Allsky_btn.TabIndex = 0;
            Allsky_btn.UseVisualStyleBackColor = true;
            Allsky_btn.TabStop = false;
            Allsky_btn.Text = "";
            Allsky_btn.Click += new EventHandler(Allsky_btn_Click);
            panneauParametres.Controls.Add(Allsky_btn);

            // AJOUT DES BOUTONS  IMAGE
            Image_btn.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.Image_btn_Off;
            Image_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            Image_btn.Location = new System.Drawing.Point(129, 76);
            Image_btn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            Image_btn.Size = new System.Drawing.Size(110, 20);
            Image_btn.TabIndex = 0;
            Image_btn.UseVisualStyleBackColor = true;
            Image_btn.TabStop = false;
            Image_btn.Text = "  ";
            Image_btn.Click += new EventHandler(Image_btn_Click);
            panneauParametres.Controls.Add(Image_btn);
            
            // AJOUT DES BOUTONS  PANO
            Panorama_btn.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.Pano_btn_Off;
            Panorama_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            Panorama_btn.Location = new System.Drawing.Point(129, 104);
            Panorama_btn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            Panorama_btn.Size = new System.Drawing.Size(110, 20);
            Panorama_btn.TabIndex = 0;
            Panorama_btn.UseVisualStyleBackColor = true;
            Panorama_btn.TabStop = false;
            Panorama_btn.Text = "";
            Panorama_btn.Click += new EventHandler(Panorama_btn_Click);
            panneauParametres.Controls.Add(Panorama_btn);

            // AJOUT DU PANNEAU DE ROTATION //
            panneauRotation.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.sliderBox_2;
            panneauRotation.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            panneauRotation.Location = new System.Drawing.Point(8, 131);
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
            Slider_Rotation.Scroll += new System.EventHandler(Slider_Rotation_Scroll);
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
            slider_Rotation_txt.Text = "0";
            slider_Rotation_txt.KeyDown += new KeyEventHandler(enterRotationValue);
            // slider_Rotation_txt.KeyDown += enterRotationValue;
            panneauRotation.Controls.Add(slider_Rotation_txt);

            // AJOUT RESET ROTATION
            resetRotation_btn.Size = new Size(20, 20);
            resetRotation_btn.Location = new Point(105, 32);
            resetRotation_btn.BackgroundImage = Properties.Resources.Reset_btn_2;
            resetRotation_btn.FlatStyle = FlatStyle.Flat;
            resetRotation_btn.FlatAppearance.BorderSize = 0;
            resetRotation_btn.Click += new EventHandler(resetRotation_Click);
            panneauRotation.Controls.Add(resetRotation_btn);

            // AJOUT DU PANNEAU DE AZIMUTH //
            panneauAzimuth.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.sliderBox_2;
            panneauAzimuth.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            panneauAzimuth.Location = new System.Drawing.Point(8, 195);
            panneauAzimuth.Size = new System.Drawing.Size(230, 56);
            panneauAzimuth.TabIndex = 0;

            // AJOUT SLIDER AZIMUTH //
            // 
            Slider_Azimuth.Location = new System.Drawing.Point(3, 6);
            Slider_Azimuth.Maximum = 180;
            Slider_Azimuth.Minimum = -180;
            Slider_Azimuth.Size = new System.Drawing.Size(224, 45);
            Slider_Azimuth.TabIndex = 0;
            Slider_Azimuth.TickFrequency = 0;
            Slider_Azimuth.TickStyle = System.Windows.Forms.TickStyle.None;
            Slider_Azimuth.Scroll += new System.EventHandler(slider_Azimuth_Scroll);
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
            slider_Azimuth_txt.Text = "0";
            slider_Azimuth_txt.KeyDown += new KeyEventHandler(enterAzimuthValue);
            panneauAzimuth.Controls.Add(slider_Azimuth_txt);

            // AJOUT RESET AZIMUTH
            resetAzimuth_btn.Size = new Size(20, 20);
            resetAzimuth_btn.Location = new Point(105, 32);
            resetAzimuth_btn.BackgroundImage = Properties.Resources.Reset_btn_2;
            resetAzimuth_btn.FlatStyle = FlatStyle.Flat;
            resetAzimuth_btn.FlatAppearance.BorderSize = 0;
            resetAzimuth_btn.Click += new EventHandler(resetAzimuth_Click);
            panneauAzimuth.Controls.Add(resetAzimuth_btn);

            // AJOUT DU PANNEAU DE Elevation //
            panneauElevation.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.sliderBox_2;
            panneauElevation.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            panneauElevation.Location = new System.Drawing.Point(8, 259);
            panneauElevation.Size = new System.Drawing.Size(230, 56);
            panneauElevation.TabIndex = 0;

            // AJOUT SLIDER Elevation //
            // 
            Slider_Elevation.Location = new System.Drawing.Point(3, 6);
            Slider_Elevation.Maximum = 90;
            Slider_Elevation.Minimum = -90;
            Slider_Elevation.Value = 90;
            Slider_Elevation.Size = new System.Drawing.Size(224, 45);
            Slider_Elevation.TabIndex = 0;
            Slider_Elevation.TickFrequency = 0;
            Slider_Elevation.TickStyle = System.Windows.Forms.TickStyle.None;
            Slider_Elevation.Scroll += new System.EventHandler(slider_Elevation_Scroll);
            panneauElevation.Controls.Add(Slider_Elevation);

            //  AJOUT NOM Elevation SLIDER   //
            slider_Elevation_lbl.AutoSize = true;
            slider_Elevation_lbl.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            slider_Elevation_lbl.ForeColor = System.Drawing.Color.White;
            slider_Elevation_lbl.Location = new System.Drawing.Point(13, 35);
            slider_Elevation_lbl.Size = new System.Drawing.Size(46, 13);
            slider_Elevation_lbl.TabIndex = 3;
            slider_Elevation_lbl.Text = "Elevation";
            panneauElevation.Controls.Add(slider_Elevation_lbl);

            // AJOUT TEXTBOX VALEUR DE Elevation

            slider_Elevation_txt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            slider_Elevation_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            slider_Elevation_txt.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            slider_Elevation_txt.ForeColor = System.Drawing.Color.White;
            slider_Elevation_txt.Location = new System.Drawing.Point(198, 34);
            slider_Elevation_txt.Size = new System.Drawing.Size(20, 14);
            slider_Elevation_txt.TabIndex = 2;
            slider_Elevation_txt.Text = "90";
            slider_Elevation_txt.KeyDown += new KeyEventHandler(enterElevationValue);
            panneauElevation.Controls.Add(slider_Elevation_txt);

            // AJOUT RESET ELEVATION    
            resetElevation_btn.Size = new Size(20, 20);
            resetElevation_btn.Location = new Point(105, 32);
            resetElevation_btn.BackgroundImage = Properties.Resources.Reset_btn_2;
            resetElevation_btn.FlatStyle = FlatStyle.Flat;
            resetElevation_btn.FlatAppearance.BorderSize = 0;
            resetElevation_btn.Click += new EventHandler(resetElevation_Click);
            panneauElevation.Controls.Add(resetElevation_btn);

            // AJOUT DU PANNEAU DE Width //
            panneauWidth.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.sliderBox_2;
            panneauWidth.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            panneauWidth.Location = new System.Drawing.Point(8, 323);
            panneauWidth.Size = new System.Drawing.Size(206, 56);
            panneauWidth.TabIndex = 0;

            // AJOUT SLIDER Width //
            // 
            Slider_Width.Location = new System.Drawing.Point(3, 6);
            Slider_Width.Maximum = 360;
            Slider_Width.Minimum = 0;
            Slider_Width.Size = new System.Drawing.Size(200, 45);
            Slider_Width.TabIndex = 0;
            Slider_Width.TickFrequency = 0;
            Slider_Width.TickStyle = System.Windows.Forms.TickStyle.None;
            panneauWidth.Controls.Add(Slider_Width);

            //  AJOUT NOM Width SLIDER   //
            slider_Width_lbl.AutoSize = true;
            slider_Width_lbl.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            slider_Width_lbl.ForeColor = System.Drawing.Color.White;
            slider_Width_lbl.Location = new System.Drawing.Point(13, 35);
            slider_Width_lbl.Size = new System.Drawing.Size(46, 13);
            slider_Width_lbl.TabIndex = 3;
            slider_Width_lbl.Text = "Width";
            Slider_Width.Scroll += new System.EventHandler(slider_Width_Scroll);
            panneauWidth.Controls.Add(slider_Width_lbl);

            // AJOUT TEXTBOX VALEUR DE Width

            slider_Width_txt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            slider_Width_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            slider_Width_txt.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            slider_Width_txt.ForeColor = System.Drawing.Color.White;
            slider_Width_txt.Location = new System.Drawing.Point(174, 34);
            slider_Width_txt.Size = new System.Drawing.Size(20, 14);
            slider_Width_txt.TabIndex = 2;
            slider_Width_txt.Text = "180";
            slider_Width_txt.KeyDown += new KeyEventHandler(enterWidthValue);
            panneauWidth.Controls.Add(slider_Width_txt);

            // AJOUT RESET WIDTH   
            resetWidth_btn.Size = new Size(20, 20);
            resetWidth_btn.Location = new Point(93, 32);
            resetWidth_btn.BackgroundImage = Properties.Resources.Reset_btn_2;
            resetWidth_btn.FlatStyle = FlatStyle.Flat;
            resetWidth_btn.FlatAppearance.BorderSize = 0;
            resetWidth_btn.Click += new EventHandler(resetWidth_Click);
            panneauWidth.Controls.Add(resetWidth_btn);

            // AJOUT DU PANNEAU DE Height //
            panneauHeight.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.sliderBox_2;
            panneauHeight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            panneauHeight.Location = new System.Drawing.Point(8, 387);
            panneauHeight.Size = new System.Drawing.Size(206, 56);
            panneauHeight.TabIndex = 0;

            // AJOUT SLIDER Height //
            // 
            Slider_Height.Location = new System.Drawing.Point(3, 6);
            Slider_Height.Maximum = 360;
            Slider_Height.Minimum = 0;
            Slider_Height.Size = new System.Drawing.Size(200, 45);
            Slider_Height.TabIndex = 0;
            Slider_Height.TickFrequency = 0;
            Slider_Height.TickStyle = System.Windows.Forms.TickStyle.None;
            Slider_Height.Scroll += new System.EventHandler(slider_Height_Scroll);
            panneauHeight.Controls.Add(Slider_Height);

            //  AJOUT NOM Height SLIDER   //
            slider_Height_lbl.AutoSize = true;
            slider_Height_lbl.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            slider_Height_lbl.ForeColor = System.Drawing.Color.White;
            slider_Height_lbl.Location = new System.Drawing.Point(13, 35);
            slider_Height_lbl.Size = new System.Drawing.Size(46, 13);
            slider_Height_lbl.TabIndex = 3;
            slider_Height_lbl.Text = "Height";
            panneauHeight.Controls.Add(slider_Height_lbl);

            // AJOUT TEXTBOX VALEUR DE Height

            slider_Height_txt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            slider_Height_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            slider_Height_txt.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            slider_Height_txt.ForeColor = System.Drawing.Color.White;
            slider_Height_txt.Location = new System.Drawing.Point(174, 34);
            slider_Height_txt.Size = new System.Drawing.Size(20, 14);
            slider_Height_txt.TabIndex = 2;
            slider_Height_txt.Text = "180";
            slider_Height_txt.KeyDown += new KeyEventHandler(enterHeightValue);
            panneauHeight.Controls.Add(slider_Height_txt);

            // AJOUT RESET HEIGHT   
            resetHeight_btn.Size = new Size(20, 20);
            resetHeight_btn.Location = new Point(93, 32);
            resetHeight_btn.BackgroundImage = Properties.Resources.Reset_btn_2;
            resetHeight_btn.FlatStyle = FlatStyle.Flat;
            resetHeight_btn.FlatAppearance.BorderSize = 0;
            resetHeight_btn.Click += new EventHandler(resetHeight_Click);
            panneauHeight.Controls.Add(resetHeight_btn);

            // AJOUT DU BOUTON GARDER RATIO
            ratio_btn.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.Ratio_On;
            ratio_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            ratio_btn.Location = new System.Drawing.Point(218, 323);
            ratio_btn.Size = new Size(20, 120);
            ratio_btn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            ratio_btn.UseVisualStyleBackColor = true;
            ratio_btn.TabStop = false;
            ratio_btn.Text = " ";
            ratio_btn.Click += new System.EventHandler(ratio_btn_Click);
            panneauParametres.Controls.Add(ratio_btn);

            // AJOUT DU TEXTBOX POUR LE NOM DES IMAGES
            nomImage_txtBox.Size = new Size(230, 20);
            nomImage_txtBox.Location = new Point(8, 451);
            nomImage_txtBox.BackColor =  Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            nomImage_txtBox.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            nomImage_txtBox.ForeColor = System.Drawing.Color.White;
            nomImage_txtBox.BorderStyle = BorderStyle.FixedSingle;
            nomImage = "MonImage_" + (index + 1); // set la variable nomImage 
            setScript();
            nomImage_txtBox.KeyDown += new KeyEventHandler(changeNameVariable);
            nomImage_txtBox.Text = nomImage;

            panneauParametres.Controls.Add(nomImage_txtBox);

            // AJOUT DU TEXTBOX POUR LE SCRIPT
            scriptOutput_txtBox.Size = new Size(230, 172);
            scriptOutput_txtBox.Location = new Point(8, 479);
            scriptOutput_txtBox.BackColor = Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            scriptOutput_txtBox.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            scriptOutput_txtBox.ForeColor = System.Drawing.Color.White;
            scriptOutput_txtBox.BorderStyle = BorderStyle.FixedSingle;
            scriptOutput_txtBox.HorizontalScrollbar = true;
            scriptOutput_txtBox.SelectionMode = SelectionMode.MultiExtended;
            scriptOutput_txtBox.DoubleClick += new EventHandler(scriptOutput_DoubleClick);
            panneauParametres.Controls.Add(scriptOutput_txtBox);

            // AJOUT BOUTON COPIER
            copieScript_btn.Size = new Size(230,22);
            copieScript_btn.Location = new Point(8, 659);
            copieScript_btn.Click += new EventHandler(copieScript);
            copieScript_btn.BackgroundImage = Properties.Resources.CopieClipboard;
            copieScript_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            copieScript_btn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            panneauParametres.Controls.Add(copieScript_btn);
            

            panneauParametres.Controls.Add(panneauHeight);
            panneauParametres.Controls.Add(panneauWidth);
            panneauParametres.Controls.Add(panneauElevation);
            panneauParametres.Controls.Add(panneauRotation);
            panneauParametres.Controls.Add(panneauAzimuth);
            Slider_Azimuth.SendToBack();
            Slider_Rotation.SendToBack();
            Slider_Elevation.SendToBack();
            Slider_Width.SendToBack();
            Slider_Height.SendToBack();
            Form1.Controls.Add(panneauParametres);
            panneauParametres.Visible = false;
        }
        private void initializationImageBox(Form Form1, int index)
        {
            panneau.BackgroundImage = DS2_Easy_Viewer.Properties.Resources.Panel_Off_2;
            panneau.Size = new Size(166, 218);
            if (index < 5)
            {
                panneau.Location = new Point(50 + (index * 170), 12);
            }
            else if (index >= 5 & index < 10)
            {
                panneau.Location = new Point(50 + ((index - 5) * 170), 248);
            }
            else if (index >= 10 & index < 15)
            {
                panneau.Location = new Point(50 + ((index - 10) * 170), 485);
            }
            boxIndex = index;
            panneau.Controls.Add(box);
            box.Click += new EventHandler(imageBox_Click);
            box.DoubleClick += new System.EventHandler(imageBox_DoubleClick);
            box.Location = new Point(13, 40);
            box.Size = new Size(140, 140);
            box.BorderStyle = BorderStyle.FixedSingle;
            box.BackgroundImage = Properties.Resources.Double_Click;
            envoyer_btn.Size = new Size(140, 22);
            envoyer_btn.Location = new Point(13, 185);
            envoyer_btn.BackgroundImage = DS2_Easy_Viewer.Properties.Resources.Envoyer_btn;
            envoyer_btn.Text = "";
            envoyer_btn.FlatStyle = FlatStyle.Popup;
            envoyer_btn.Click += new EventHandler(envoyer_Click);
            chemin.Text = "";
            chemin.Size = new Size(110, 40);
            chemin.BackColor = Color.FromArgb(40, 40, 40);
            chemin.ForeColor = Color.White;
            chemin.BorderStyle = BorderStyle.None;
            chemin.Location = new Point(40, 10);
            chemin.RightToLeft = RightToLeft.Yes;
            chemin.Font = new Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            panneau.Controls.Add(chemin);
            panneau.Controls.Add(envoyer_btn);
            remove.Size = new Size(12,12);
            remove.Location = new Point(13, 12);
            remove.BackgroundImage = Properties.Resources.Remove_btn_3;
            remove.Text = "";
            remove.FlatStyle = FlatStyle.Flat;
            remove.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            remove.FlatAppearance.BorderSize = 0;
            remove.Click += new EventHandler(remove_Click);
            ToolTip toolTipRemove = new ToolTip();
            toolTipRemove.ShowAlways = true;
            toolTipRemove.SetToolTip(remove, "Text Remove ...");
            ToolTip toolTipView = new ToolTip();
            toolTipView.ShowAlways = true;
            toolTipView.SetToolTip(envoyer_btn, "Text View ...");
            panneau.Controls.Add(remove);
            Form1.Controls.Add(panneau);
    }
        public void imageBox_Click(object sender, EventArgs e)
        {
            if (imageRenommee != null)
            {
                foreach (imageBox boite in Form1.imageBoxList)
                {
                    boite.panneau.BackgroundImage = DS2_Easy_Viewer.Properties.Resources.Panel_Off_2;
                }
                panneau.BackgroundImage = DS2_Easy_Viewer.Properties.Resources.Panel_On_2;
                Form1.boiteSelectionnee = boxIndex;
                try
                {
                    Form1.videoBoxList[0].panneau.BackgroundImage = DS2_Easy_Viewer.Properties.Resources.Panel_Off_2;
                    Form1.videoBoxList[0].panneauParametres.Visible = false;
                } catch { }
                
                foreach (imageBox boite in Form1.imageBoxList)
                {
                    boite.panneauParametres.Visible = false;
                }
                panneauParametres.Visible = true;
            }
            nomImage = nomImage_txtBox.Text;

        }
        private void imageBox_DoubleClick(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.tif;*.tiff*,.tga...";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                image_Path = openFileDialog1.FileName;
                //bw_loadImage.RunWorkerAsync(openFileDialog1.FileName);
                loadImage(openFileDialog1.FileName);
                setInitialParameterValues();
                setScript();
            }
        }
        private void setInitialParameterValues()
        {
            textLocateParameters.Clear();
            textLocateParameters = new List<int> { 0, 0, 90, 0, 180, 180 };
            int a = 1;
            foreach (TrackBar slider in listDeSliders )
            {
                slider.Value = textLocateParameters[a];
                a += 1;
            }
            int b = 1;
            foreach (TextBox text in listDeTextBox)
            {
                text.Text = textLocateParameters[b].ToString();
                b += 1;
            }

        }
        private void changeModeImageSliderUpdate(int mode)
        {
            if (imageRenommee != "")
            {
                textLocateParameters.Clear();
                textAddParameters.Clear();
                ShellFile shellFile = ShellFile.FromFilePath(imageRenommee);
                Bitmap shellThumb = shellFile.Thumbnail.Bitmap;
                Image img = shellThumb;
                //Image img = Image.FromFile(imageRenommee);
                modeDisplay = mode;
                if (mode == 0) // Allsky
                {
                    textLocateParameters = new List<int> { 0, 0, 90, 0, 180, 180 };
                    textAddParameters = new List<int> { 0, 0, 0, 90, 0, 0, 1, 1 };
                    ratio = 1;
                }
                if (mode == 1) // Image
                {
                    ratio = (double)img.Height / img.Width;
                    int width = (int)(64 * ratio);
                    textLocateParameters = new List<int> { 0, 0, 30, 0, 64, width };
                    textAddParameters = new List<int> { 0, 0, 0, 30, 0, 0, 0, 0 };
                }
                if (mode == 2) // Panorama
                {
                    textLocateParameters = new List<int> { 0, 0, 0, 0, 180, 15 };
                    textAddParameters = new List<int> { 0, 0, 0, 30, 0, 1, 0, 0 };
                }

                int a = 1;
                foreach (TrackBar slider in listDeSliders)
                {
                    slider.Value = textLocateParameters[a];
                    a += 1;
                }
                int b = 1;
                foreach (TextBox text in listDeTextBox)
                {
                    text.Text = textLocateParameters[b].ToString();
                    b += 1;
                }
                setScript();
            }
           
            

        }
        private void backgroundWorker_loadImage(object sender, DoWorkEventArgs e)
        {
            string nomImage = (string)e.Argument;
            BackgroundWorker worker = sender as BackgroundWorker;
            string drivePrefix = nomImage.Substring(0, 1);
            if (DEBUG == true)
            {
                imageRenommee = nomImage;
            }
            else
            {
                imageRenommee = "\\\\Ds-Master\\" + drivePrefix + nomImage.Remove(0, 2);
            }
            try
            {
                chemin.Text = Path.GetFileName(nomImage);
                box.Image = Image.FromFile(imageRenommee);
                box.SizeMode = PictureBoxSizeMode.Zoom;
                imgSelect.Image = Image.FromFile(imageRenommee);
                imgSelect.SizeMode = PictureBoxSizeMode.Zoom;
                imgSelectLbl.Text = Path.GetFileName(nomImage);
                setScript();
                setInitialParameterValues();
                //MessageBox.Show("Text Add \"" + nomImage + "\"  \"" + imageRenommee + "\"  " + string.Join(" ", textAddParameters.ToArray()));
                textAddLocate();
            }
            catch (Exception) { }
        }
        public void loadImage(string filename)
        {
            string drivePrefix = filename.Substring(0, 1);
            if (DEBUG == true)
            {
                imageRenommee = filename;
            }
            else
            {
                imageRenommee = "\\\\Ds-Master\\" + drivePrefix + filename.Remove(0, 2);
            }
            try
            {
                chemin.Text = Path.GetFileName(filename);
                //box.Image = Image.FromFile(imageRenommee);
                ShellFile shellFile = ShellFile.FromFilePath(filename);
                Bitmap shellThumb = shellFile.Thumbnail.Bitmap;
                box.Image = shellThumb;
                box.SizeMode = PictureBoxSizeMode.Zoom;
                //imgSelect.Image = Image.FromFile(imageRenommee);
                imgSelect.Image = shellThumb;
                imgSelect.SizeMode = PictureBoxSizeMode.Zoom;
                imgSelectLbl.Text = Path.GetFileName(filename);
                setScript();
                setInitialParameterValues();
                //MessageBox.Show("Text Add \"" + nomImage + "\"  \"" + imageRenommee + "\"  " + string.Join(" ", textAddParameters.ToArray()));
                textAddLocate();
            }
            catch (Exception) { }
        }
        private void envoyer_Click(object sender, EventArgs e)
        {
            if (surDome == false)
            {
                /*
                foreach (imageBox boite in Form1.imageBoxList)
                {
                    boite.envoyer_btn.BackgroundImage = Properties.Resources.Envoyer_btn;
                    boite.surDome = false;
                }
                */
                eteindreReste();
                envoyer_btn.BackgroundImage = Properties.Resources.Envoyer_on;
                Byte[] commande;
                commande = Ds2Command("Text View \"" + nomImage + "\"  0 100 100 100 100");
                envoyerCommande(commande);
                surDome = true;
            }
            else if ( surDome == true)
            {
                envoyer_btn.BackgroundImage = Properties.Resources.Envoyer_btn;
                Byte[] commande;
                commande = Ds2Command("Text View \"" + nomImage + "\"  0 0 100 100 100");
                envoyerCommande(commande);
                surDome = false;
            }
        }
        private Byte[] Ds2Command(string command)
        {
            Byte[] sendBytes = Encoding.ASCII.GetBytes("\x02" + "DSTA00000000" + "\x07" + "DIRECT" + "\x07" + command + "\x03");
            return sendBytes;
        }
        private void envoyerCommande(Byte[] commande)
        {
            try
            {
                UdpClient udpClient = new UdpClient();
                //udpClient.Send(commande, commande.Length, "192.168.0.100", 2209);
                udpClient.Send(commande, commande.Length, "127.0.0.1", 2209);
            }
            catch (Exception)
            {

            }
        }
        private void Slider_Rotation_Scroll(object sender, EventArgs e)
        {
            try
            {
                Byte[] commande;
                textLocateParameters[3] = Slider_Rotation.Value;
                commande = Ds2Command("Text Locate \"" + nomImage + "\"  " + string.Join(" ", textLocateParameters.ToArray()) + " \"");
                envoyerCommande(commande);
                slider_Rotation_txt.Text = Slider_Rotation.Value.ToString();
                setScript();
            }
            catch (Exception) {MessageBox.Show("la valeur excède le maximum permis pour cet attribut");}
        }
        private void slider_Azimuth_Scroll(object sender, EventArgs e)
        {
            try
            {
                Byte[] commande;
                textLocateParameters[1] = Slider_Azimuth.Value;
                commande = Ds2Command("Text Locate \"" + nomImage + "\"  " + string.Join(" ", textLocateParameters.ToArray()) + " \"");
                envoyerCommande(commande);
                slider_Azimuth_txt.Text = Slider_Azimuth.Value.ToString();
                setScript();
            }
            catch (Exception) { MessageBox.Show("la valeur excède le maximum permis pour cet attribut"); }
        }
        private void slider_Elevation_Scroll(object sender, EventArgs e)
        {
            try
            {
                Byte[] commande;
                textLocateParameters[2] = Slider_Elevation.Value;
                commande = Ds2Command("Text Locate \"" + nomImage + "\"  " + string.Join(" ", textLocateParameters.ToArray()) + " \"");
                envoyerCommande(commande);
                slider_Elevation_txt.Text = Slider_Elevation.Value.ToString();
                setScript();
            }
            catch (Exception) { MessageBox.Show("la valeur excède le maximum permis pour cet attribut"); }
        }
        private void slider_Width_Scroll(object sender, EventArgs e)
        {
            try
            {
                Byte[] commande;
                textLocateParameters[4] = Slider_Width.Value;
                slider_Width_txt.Text = Slider_Width.Value.ToString();
                if (ratioOn)
                {
                    Slider_Height.Value = (int)Math.Round(Slider_Width.Value * ratio);
                    textLocateParameters[5] = Slider_Height.Value;
                    slider_Height_txt.Text = Slider_Height.Value.ToString();
                }
                commande = Ds2Command("Text Locate \"" + nomImage + "\"  " + string.Join(" ", textLocateParameters.ToArray()) + " \"");
                envoyerCommande(commande);
                setScript();
            }
            catch (Exception) { MessageBox.Show("la valeur excède le maximum permis pour cet attribut"); }
        }
        private void slider_Height_Scroll(object sender, EventArgs e)
        {
            try
            {
                Byte[] commande;
                textLocateParameters[5] = Slider_Height.Value;
                slider_Height_txt.Text = Slider_Height.Value.ToString();
                if (ratioOn) {
                    Slider_Width.Value = (int)Math.Round(Slider_Height.Value / ratio);
                    textLocateParameters[4] = Slider_Width.Value;
                    slider_Width_txt.Text = Slider_Width.Value.ToString();
                }
                commande = Ds2Command("Text Locate \"" + nomImage + "\"  " + string.Join(" ", textLocateParameters.ToArray()) + " \"");
                envoyerCommande(commande);
                setScript();
            }
            catch (Exception) { MessageBox.Show("la valeur excède le maximum permis pour cet attribut"); }
        }
        private void ratio_btn_Click(object sender, EventArgs e)
        {
            if (ratioOn == true)
            {
                ratioOn = false;
                ratio_btn.BackgroundImage = Properties.Resources.Ratio_Off;
            }
            else
            {
                ratioOn = true;
                ratio_btn.BackgroundImage = Properties.Resources.Ratio_On;
                ratio = (double)Slider_Height.Value/Slider_Width.Value;

            }
        }
        private void resetRotation_Click(object sender, EventArgs e)
        {
            try
            {
                Byte[] commande;
                textLocateParameters[3] = 0;
                commande = Ds2Command("Text Locate \"" + nomImage + "\"  " + string.Join(" ", textLocateParameters.ToArray()) + " \"");
                envoyerCommande(commande);
                slider_Rotation_txt.Text = textLocateParameters[3].ToString();
                Slider_Rotation.Value = textLocateParameters[3];
                setScript();
            }
            catch (Exception) { MessageBox.Show("la connexion avec Ds-master est impossible"); }
        }
        private void resetAzimuth_Click(object sender, EventArgs e)
        {
            try
            {
                Byte[] commande;
                textLocateParameters[1] = 0;
                commande = Ds2Command("Text Locate \"" + nomImage + "\"  " + string.Join(" ", textLocateParameters.ToArray()) + " \"");
                envoyerCommande(commande);
                slider_Azimuth_txt.Text = textLocateParameters[1].ToString();
                Slider_Azimuth.Value = textLocateParameters[1];
                setScript();
            }
            catch (Exception) { MessageBox.Show("la connexion avec Ds-master est impossible"); }
        }
        private void resetElevation_Click(object sender, EventArgs e)
        {
            try
            {
                Byte[] commande;
                if (modeDisplay == 0) { textLocateParameters[2] = 90; }
                if (modeDisplay == 1) { textLocateParameters[2] = 30; }
                commande = Ds2Command("Text Locate \"" + nomImage + "\"  " + string.Join(" ", textLocateParameters.ToArray()) + " \"");
                envoyerCommande(commande);
                slider_Elevation_txt.Text = textLocateParameters[2].ToString();
                Slider_Elevation.Value = textLocateParameters[2];
                setScript();
            }
            catch (Exception) { MessageBox.Show("la connexion avec Ds-master est impossible"); }
        }
        private void resetWidth_Click(object sender, EventArgs e)
        {
            try
            {
                Byte[] commande;
                if (modeDisplay == 0) { textLocateParameters[4] = 180; }
                if (modeDisplay == 1) { textLocateParameters[4] = 64; }
                commande = Ds2Command("Text Locate \"" + nomImage + "\"  " + string.Join(" ", textLocateParameters.ToArray()) + " \"");
                envoyerCommande(commande);
                slider_Width_txt.Text = textLocateParameters[4].ToString();
                Slider_Width.Value = textLocateParameters[4];
                setScript();
            }
            catch (Exception) { MessageBox.Show("la connexion avec Ds-master est impossible"); }
        }
        private void resetHeight_Click(object sender, EventArgs e)
        {
            try
            {
                ShellFile shellFile = ShellFile.FromFilePath(imageRenommee);
                Bitmap shellThumb = shellFile.Thumbnail.Bitmap;
                Image img = shellThumb;
                //Image img = Image.FromFile(imageRenommee);
                ratio = ratio = (double)img.Height / img.Width;
                Byte[] commande;
                if(modeDisplay == 0) { textLocateParameters[5] = 180; }
                if (modeDisplay == 1) { textLocateParameters[5] = (int)(64 * ratio); }
                commande = Ds2Command("Text Locate \"" + nomImage + "\"  " + string.Join(" ", textLocateParameters.ToArray()) + " \"");
                envoyerCommande(commande);
                slider_Height_txt.Text = textLocateParameters[5].ToString();
                Slider_Height.Value = textLocateParameters[5];
                setScript();
            }
            catch (Exception) { MessageBox.Show("la connexion avec Ds-master est impossible"); }
        }
        private void setScript()
        {
            scriptOutput_txtBox.Items.Clear();
            scriptOutput_txtBox.Items.Add("Text Add \"" + nomImage + "\"  \"" + imageRenommee + "\"  " + string.Join(" ", textAddParameters.ToArray()));
            scriptOutput_txtBox.Items.Add("+ .1");
            scriptOutput_txtBox.Items.Add("Text Locate \"" + nomImage + "\"  " + string.Join(" ", textLocateParameters.ToArray()));
            scriptOutput_txtBox.Items.Add("+ .1");
            scriptOutput_txtBox.Items.Add("Text View \"" + nomImage + "\"  1 100 100 100 100");
        }
        private void scriptOutput_DoubleClick(object sender, EventArgs e)
        {
          
        }
        private void copieScript(object sender, EventArgs e)
        {
            if (scriptOutput_txtBox.Items.Count >0)
                {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (object row in scriptOutput_txtBox.SelectedItems)
                    {
                        sb.Append(row.ToString());
                        sb.AppendLine();
                    }
                    sb.Remove(sb.Length - 1, 1); // Just to avoid copying last empty row
                    Clipboard.SetData(System.Windows.Forms.DataFormats.Text, sb.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Sélectionnez au moins une ligne de code");
            }

           
        }
        private void eteindreReste()
        {
            foreach (imageBox boite in Form1.imageBoxList)
            {
                boite.envoyer_btn.BackgroundImage = Properties.Resources.Envoyer_btn;
                boite.surDome = false;
                Byte[] commandeBoites;
                commandeBoites = Ds2Command("Text View \"" + boite.nomImage + "\"  0 0 100 100 100");  // Fermer éteindre les images des autres renderers
                envoyerCommande(commandeBoites);
            }
            foreach (videoBox boite in Form1.videoBoxList)
            {
                boite.envoyer_btn.BackgroundImage = Properties.Resources.Envoyer_btn;
                boite.surDome = false;
                Byte[] commandeBoites;
                commandeBoites = Ds2Command("Text View \"" + boite.nomImage + "\"  0 0 100 100 100");  // Fermer éteindre les images des autres renderers
                envoyerCommande(commandeBoites);
            }
            envoyer_btn.BackgroundImage = Properties.Resources.Envoyer_on;
        }
        private void textAddLocate()
        {
            Byte[] commande;
            commande = Ds2Command("Text Add \"" + nomImage + "\"  \"" + imageRenommee + "\"  " + string.Join(" ", textAddParameters.ToArray()));
            envoyerCommande(commande);
            Thread.Sleep(100);
            commande = Ds2Command("Text Locate \"" + nomImage + "\"  " + string.Join(" ", textLocateParameters.ToArray()));
            envoyerCommande(commande);
            Thread.Sleep(50);
        }
        private void textView()
        {
            Byte[] commande;
            commande = Ds2Command("Text View \"" + nomImage + "\"  0 100 100 100 100");
            envoyerCommande(commande);
        }
        private void Allsky_btn_Click(object sender, EventArgs e)
        {
            Allsky_btn.BackgroundImage = Properties.Resources.AllSky_btn_On;
            Image_btn.BackgroundImage = Properties.Resources.Image_btn_Off;
            Panorama_btn.BackgroundImage = Properties.Resources.Pano_btn_Off;
            changeModeImageSliderUpdate(0);
            eteindreReste();
            textAddLocate();
            textView();
        }
        private void Image_btn_Click(object sender, EventArgs e)
        {
            Allsky_btn.BackgroundImage = Properties.Resources.AllSky_btn_Off;
            Image_btn.BackgroundImage = Properties.Resources.Image_btn_On;
            Panorama_btn.BackgroundImage = Properties.Resources.Pano_btn_Off;
            changeModeImageSliderUpdate(1);
            eteindreReste();
            textAddLocate();
            textView();
        }
        private void Panorama_btn_Click(object sender, EventArgs e)
        {
            Allsky_btn.BackgroundImage = Properties.Resources.AllSky_btn_Off;
            Image_btn.BackgroundImage = Properties.Resources.Image_btn_Off;
            Panorama_btn.BackgroundImage = Properties.Resources.Pano_btn_On;
            changeModeImageSliderUpdate(2);
            eteindreReste();
            textAddLocate();
            textView();
        }
        private void enterRotationValue(object sender, KeyEventArgs e)
        {
            int integer;
            bool resultat = Int32.TryParse(slider_Rotation_txt.Text, out integer);
            if (e.KeyCode == Keys.Enter & slider_Rotation_txt.Text != "" & resultat )
            {
                if (integer >= -180 & integer <= 180)
                {
                    Slider_Rotation.Value = integer;
                    Slider_Rotation_Scroll(this, EventArgs.Empty);
                }
            }
        }
        private void enterAzimuthValue(object sender, KeyEventArgs e)
        {
            int integer;
            bool resultat = Int32.TryParse(slider_Azimuth_txt.Text, out integer);
            if (e.KeyCode == Keys.Enter & slider_Azimuth_txt.Text != "" & resultat)
            {
                if (integer >= -180 & integer <= 180)
                {
                    Slider_Azimuth.Value = integer;
                    slider_Azimuth_Scroll(this, EventArgs.Empty);
                }
            }
        }
        private void enterElevationValue(object sender, KeyEventArgs e)
        {
            int integer;
            bool resultat = Int32.TryParse(slider_Elevation_txt.Text, out integer);
            if (e.KeyCode == Keys.Enter & slider_Elevation_txt.Text != "" & resultat)
            {
                if (integer >= -90 & integer <= 90)
                {
                    Slider_Elevation.Value = integer;
                    slider_Elevation_Scroll(this, EventArgs.Empty);
                }
            }
        }
        private void enterWidthValue(object sender, KeyEventArgs e)
        {
            int integer;
            bool resultat = Int32.TryParse(slider_Width_txt.Text, out integer);
            if (e.KeyCode == Keys.Enter & slider_Width_txt.Text != "" & resultat)
            {
                if (integer >= 0 & integer <= 360)
                {
                    Slider_Width.Value = integer;
                    slider_Width_Scroll(this, EventArgs.Empty);
                }
            }
        }
        private void enterHeightValue(object sender, KeyEventArgs e)
        {
            int integer;
            bool resultat = Int32.TryParse(slider_Height_txt.Text, out integer);
            if (e.KeyCode == Keys.Enter & slider_Height_txt.Text != "" & resultat)
            {
                if (integer >= 0 & integer <= 360)
                {
                    Slider_Height.Value = integer;
                    slider_Height_Scroll(this, EventArgs.Empty);
                }
            }
        }
        private void changeNameVariable(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Byte[] commande;
                commande = Ds2Command("Text Remove \"" + nomImage + " \"");
                envoyerCommande(commande);
                string newName = nomImage_txtBox.Text;
                nomImage = newName;
                commande = Ds2Command("Text Add \"" + nomImage + "\"  \"" + imageRenommee + "\"" + string.Join(" ", textAddParameters.ToArray()) + " \"");
                envoyerCommande(commande);
                Thread.Sleep(100);
                commande = Ds2Command("Text Locate \"" + nomImage + "\"  " + string.Join(" ", textLocateParameters.ToArray()) + " \"");
                envoyerCommande(commande);
                Thread.Sleep(50);
                commande = Ds2Command("Text View \"" + nomImage + "\"  0 100 100 100 100");
                envoyerCommande(commande);
                Thread.Sleep(50);
                scriptOutput_txtBox.Items.Clear();
                scriptOutput_txtBox.Items.Add("Text Add \"" + nomImage + "\"  \"" + imageRenommee + "\"  " + string.Join(" ", textAddParameters.ToArray()));
                scriptOutput_txtBox.Items.Add("+ .1");
                scriptOutput_txtBox.Items.Add("Text Locate \"" + nomImage + "\"  " + string.Join(" ", textLocateParameters.ToArray()));
                scriptOutput_txtBox.Items.Add("+ .1");
                scriptOutput_txtBox.Items.Add("Text View \"" + nomImage + "\"  1 100 100 100 100");
            }
            
        }
        public void remove_Click(object sender, EventArgs e)
        {
            envoyer_btn.BackgroundImage = Properties.Resources.Envoyer_btn;
            Byte[] commande;
            commande = Ds2Command("Text Remove \"" + nomImage + " \"");
            envoyerCommande(commande);
            surDome = false;
            box.Image = null;
            imgSelect.Image = null;
            nomImage = nomImage_txtBox.Text;
            chemin.Text = "";
            imgSelectLbl.Text = "";            
        }
        public void resetAll()
        {
            Byte[] commande;
            commande = Ds2Command("Show Reset");
            envoyerCommande(commande);
        }
    }
    public partial class videoBox
    {
        public string image_Path; public string imageRenommee = "";
        TextBox chemin = new TextBox(); Panel marqueurs_Panel = new Panel();
        PictureBox box = new PictureBox(); TrackBar Volume_Slider = new TrackBar(); Panel Volume_Panneau = new Panel(); Label Volume_txt = new Label(); Label Volume_lvl = new Label(); public int VolumeLevel = 80;
        PictureBox imgSelect = new PictureBox();
        Label imgSelectLbl = new Label(); Label videoDureeTotale = new Label();
        public Button envoyer_btn = new Button(); Button Allsky_btn = new Button(); Button Panorama_btn = new Button(); Button Image_btn = new Button();
        Button ratio_btn = new Button(); Panel ControlesVideo_Panneau = new Panel();
        public Panel panneau = new Panel(); // Panneau de l'image
        public Panel panneauParametres = new System.Windows.Forms.Panel(); Panel panneauRotation = new System.Windows.Forms.Panel(); Panel panneauAzimuth = new System.Windows.Forms.Panel();
        Panel panneauElevation = new System.Windows.Forms.Panel(); Panel panneauWidth = new System.Windows.Forms.Panel(); Panel panneauHeight = new System.Windows.Forms.Panel();
        Panel panneauImage = new Panel(); 
        Button remove = new Button(); Button copieScript_btn = new Button();
        public TrackBar Slider_Rotation = new TrackBar(); TrackBar Slider_Azimuth = new TrackBar(); TrackBar Slider_Elevation = new TrackBar(); TrackBar Slider_Width = new TrackBar(); TrackBar Slider_Height = new TrackBar();
        Label slider_Rotation_lbl = new Label(); Label slider_Azimuth_lbl = new Label(); Label slider_Elevation_lbl = new Label(); Label slider_Width_lbl = new Label(); Label slider_Height_lbl = new Label();
        public TextBox slider_Rotation_txt = new TextBox(); TextBox slider_Azimuth_txt = new TextBox(); TextBox slider_Elevation_txt = new TextBox(); TextBox slider_Width_txt = new TextBox(); TextBox slider_Height_txt = new TextBox();
        Button resetRotation_btn = new Button(); Button resetAzimuth_btn = new Button(); Button resetElevation_btn = new Button(); Button resetWidth_btn = new Button(); Button resetHeight_btn = new Button();
        TextBox nomImage_txtBox = new TextBox(); ListBox scriptOutput_txtBox = new ListBox();
        private static int boxIndex; PictureBox curseur = new PictureBox();
        TextBox currentTimelineTime = new TextBox();
        Button videoPlay_btn = new Button(); Button videoPause = new Button(); Button videoNextMarker = new Button(); Button vieoPreviousMarker = new Button(); Button Marqueur = new Button();
        Panel timeLine = new Panel(); 
        public List<int> textAddParameters = new List<int> { 0, 0, 0, 90, 0, 0, 1, 1 };  // crée une liste de listes des paramètres de text add 
        public List<int> textLocateParameters = new List<int> { 0, 0, 90, 0, 180, 180 };     // crée une liste de listes des paramètres de text locate /// le dernier paramètre est l'opacite de textview
        // [0] RateTime     [1] Azimuth     [2] Elevation   [3] Rotation  [4] Width    [5] height
        private List<TrackBar> listDeSliders = new List<TrackBar>();
        private List<TextBox> listDeTextBox = new List<TextBox>();
        public static List<string> listeValeurDefautText = new List<string> { "0", "0", "90", "0", "180", "180" };
        int count = 0; int modeDisplay = 0;
        double ratio = 1;
        public string nomImage = ""; 
        public static bool ratioOn = true; public bool surDome = false;
        public string videoPath = "";
        public AxWindowsMediaPlayer wmPlayer = new AxWindowsMediaPlayer();
        public int x; int y; public double videoLength;
        public int videoState = 0; 
        Label videoSize_lbl = new Label(); Label videoLength_lbl = new Label(); Label videoWidth_lbl = new Label(); Label videoHeight_lbl = new Label(); Label videoBitRate_lbl = new Label();
        Label timelineStart = new Label(); Label info_lbl = new Label(); Label videoName_lbl = new Label(); Label frameRate_lbl = new Label();
        public double timelineMaxinSeconds; public double facteurConversionTimeline; public double frameRate;
        public bool play = false; Button loadNew_btn = new Button();
        Button videoGoToEnd_btn = new Button(); Button videoGoToStart_btn = new Button(); Button videoForwardOneFrame_btn = new Button(); Button videoBackOneFrame_btn = new Button(); Button marqueur_btn = new Button();
        Button videoGoToNextMarker_btn = new Button(); Button videoGoToPreviousMarker_btn = new Button(); Button loop_btn = new Button();
        List<Marqueur> Marqueurs_liste = new List<Marqueur>();
        int frames; public int videoWidth; public int videoHeight; public int actualFrame;
        TextBox tCode = new TextBox();
        Panel inOutRange_panel = new Panel();  int inX = 0; int outX = 800; Button in_btn = new Button(); Button out_btn = new Button();  double inLoop_timeDecimal; double outLoop_timeDecimal; int inFrame; int outFrame;
        int longueurVideoControlPannel = 800; // LONGUEUR DE LA TIMELINE
        bool loop = true; string dureeTotaleDuVideo; ToolTip outLoop_tooltip = new ToolTip(); ToolTip inLoop_toolTip = new ToolTip(); ShellProperty<UInt64?> duration; double durationVideoSecondes; double durationVideoMillis;
        VideoFileSource videoSource;

        private void initializationLayoutParameters(Form Form1, int index)
        {

            // AJOUT DU PANNEAU DE PARAMETRES //
            panneauParametres.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.Params_Outline_2;
            panneauParametres.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            panneauParametres.Location = new System.Drawing.Point(910, 12);
            panneauParametres.Size = new System.Drawing.Size(246, 692);
            panneauParametres.TabIndex = 0;

            // AJOUT DU PICTUREBOX DE L'IMAGE SELECTIONNEE
            imgSelect.Location = new Point(8, 8);
            imgSelect.Size = new Size(115, 115);
            imgSelect.BorderStyle = BorderStyle.FixedSingle;
            panneauParametres.Controls.Add(imgSelect);

            // AJOUT DU NOM DE L'IMAGE SELECTIONNEE
            imgSelectLbl.AutoSize = false;
            imgSelectLbl.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            imgSelectLbl.ForeColor = System.Drawing.Color.White;
            imgSelectLbl.Location = new System.Drawing.Point(129, 12);
            imgSelectLbl.Size = new System.Drawing.Size(110, 13);
            imgSelectLbl.TabIndex = 3;
            imgSelectLbl.Text = " ";
            imgSelectLbl.RightToLeft = RightToLeft.Yes;
            panneauParametres.Controls.Add(imgSelectLbl);


            // AJOUT DES BOUTONS  ALLSKY 
            Allsky_btn.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.AllSky_btn_On;
            Allsky_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            Allsky_btn.Location = new System.Drawing.Point(129, 48);
            Allsky_btn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            Allsky_btn.Size = new System.Drawing.Size(110, 20);
            Allsky_btn.TabIndex = 0;
            Allsky_btn.UseVisualStyleBackColor = true;
            Allsky_btn.TabStop = false;
            Allsky_btn.Text = "";
            Allsky_btn.Click += new EventHandler(Allsky_btn_Click);
            panneauParametres.Controls.Add(Allsky_btn);

            // AJOUT DES BOUTONS  IMAGE
            Image_btn.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.Image_btn_Off;
            Image_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            Image_btn.Location = new System.Drawing.Point(129, 76);
            Image_btn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            Image_btn.Size = new System.Drawing.Size(110, 20);
            Image_btn.TabIndex = 0;
            Image_btn.UseVisualStyleBackColor = true;
            Image_btn.TabStop = false;
            Image_btn.Text = "  ";
            Image_btn.Click += new EventHandler(Image_btn_Click);
            panneauParametres.Controls.Add(Image_btn);

            // AJOUT DES BOUTONS  PANO
            Panorama_btn.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.Pano_btn_Off;
            Panorama_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            Panorama_btn.Location = new System.Drawing.Point(129, 104);
            Panorama_btn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            Panorama_btn.Size = new System.Drawing.Size(110, 20);
            Panorama_btn.TabIndex = 0;
            Panorama_btn.UseVisualStyleBackColor = true;
            Panorama_btn.TabStop = false;
            Panorama_btn.Text = "";
            Panorama_btn.Click += new EventHandler(Panorama_btn_Click);
            panneauParametres.Controls.Add(Panorama_btn);

            // AJOUT DU PANNEAU DE ROTATION //
            panneauRotation.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.sliderBox_2;
            panneauRotation.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            panneauRotation.Location = new System.Drawing.Point(8, 131);
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
            Slider_Rotation.Scroll += new System.EventHandler(Slider_Rotation_Scroll);
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
            slider_Rotation_txt.Text = "0";
            slider_Rotation_txt.KeyDown += new KeyEventHandler(enterRotationValue);
            // slider_Rotation_txt.KeyDown += enterRotationValue;
            panneauRotation.Controls.Add(slider_Rotation_txt);

            // AJOUT RESET ROTATION
            resetRotation_btn.Size = new Size(20, 20);
            resetRotation_btn.Location = new Point(105, 32);
            resetRotation_btn.BackgroundImage = Properties.Resources.Reset_btn_2;
            resetRotation_btn.FlatStyle = FlatStyle.Flat;
            resetRotation_btn.FlatAppearance.BorderSize = 0;
            resetRotation_btn.Click += new EventHandler(resetRotation_Click);
            panneauRotation.Controls.Add(resetRotation_btn);

            // AJOUT DU PANNEAU DE AZIMUTH //
            panneauAzimuth.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.sliderBox_2;
            panneauAzimuth.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            panneauAzimuth.Location = new System.Drawing.Point(8, 195);
            panneauAzimuth.Size = new System.Drawing.Size(230, 56);
            panneauAzimuth.TabIndex = 0;

            // AJOUT SLIDER AZIMUTH //
            // 
            Slider_Azimuth.Location = new System.Drawing.Point(3, 6);
            Slider_Azimuth.Maximum = 180;
            Slider_Azimuth.Minimum = -180;
            Slider_Azimuth.Size = new System.Drawing.Size(224, 45);
            Slider_Azimuth.TabIndex = 0;
            Slider_Azimuth.TickFrequency = 0;
            Slider_Azimuth.TickStyle = System.Windows.Forms.TickStyle.None;
            Slider_Azimuth.Scroll += new System.EventHandler(slider_Azimuth_Scroll);
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
            slider_Azimuth_txt.Text = "0";
            slider_Azimuth_txt.KeyDown += new KeyEventHandler(enterAzimuthValue);
            panneauAzimuth.Controls.Add(slider_Azimuth_txt);

            // AJOUT RESET AZIMUTH
            resetAzimuth_btn.Size = new Size(20, 20);
            resetAzimuth_btn.Location = new Point(105, 32);
            resetAzimuth_btn.BackgroundImage = Properties.Resources.Reset_btn_2;
            resetAzimuth_btn.FlatStyle = FlatStyle.Flat;
            resetAzimuth_btn.FlatAppearance.BorderSize = 0;
            resetAzimuth_btn.Click += new EventHandler(resetAzimuth_Click);
            panneauAzimuth.Controls.Add(resetAzimuth_btn);

            // AJOUT DU PANNEAU DE Elevation //
            panneauElevation.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.sliderBox_2;
            panneauElevation.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            panneauElevation.Location = new System.Drawing.Point(8, 259);
            panneauElevation.Size = new System.Drawing.Size(230, 56);
            panneauElevation.TabIndex = 0;

            // AJOUT SLIDER Elevation //
            // 
            Slider_Elevation.Location = new System.Drawing.Point(3, 6);
            Slider_Elevation.Maximum = 90;
            Slider_Elevation.Minimum = -90;
            Slider_Elevation.Value = 90;
            Slider_Elevation.Size = new System.Drawing.Size(224, 45);
            Slider_Elevation.TabIndex = 0;
            Slider_Elevation.TickFrequency = 0;
            Slider_Elevation.TickStyle = System.Windows.Forms.TickStyle.None;
            Slider_Elevation.Scroll += new System.EventHandler(slider_Elevation_Scroll);
            panneauElevation.Controls.Add(Slider_Elevation);

            //  AJOUT NOM Elevation SLIDER   //
            slider_Elevation_lbl.AutoSize = true;
            slider_Elevation_lbl.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            slider_Elevation_lbl.ForeColor = System.Drawing.Color.White;
            slider_Elevation_lbl.Location = new System.Drawing.Point(13, 35);
            slider_Elevation_lbl.Size = new System.Drawing.Size(46, 13);
            slider_Elevation_lbl.TabIndex = 3;
            slider_Elevation_lbl.Text = "Elevation";
            panneauElevation.Controls.Add(slider_Elevation_lbl);

            // AJOUT TEXTBOX VALEUR DE Elevation

            slider_Elevation_txt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            slider_Elevation_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            slider_Elevation_txt.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            slider_Elevation_txt.ForeColor = System.Drawing.Color.White;
            slider_Elevation_txt.Location = new System.Drawing.Point(198, 34);
            slider_Elevation_txt.Size = new System.Drawing.Size(20, 14);
            slider_Elevation_txt.TabIndex = 2;
            slider_Elevation_txt.Text = "90";
            slider_Elevation_txt.KeyDown += new KeyEventHandler(enterElevationValue);
            panneauElevation.Controls.Add(slider_Elevation_txt);

            // AJOUT RESET ELEVATION    
            resetElevation_btn.Size = new Size(20, 20);
            resetElevation_btn.Location = new Point(105, 32);
            resetElevation_btn.BackgroundImage = Properties.Resources.Reset_btn_2;
            resetElevation_btn.FlatStyle = FlatStyle.Flat;
            resetElevation_btn.FlatAppearance.BorderSize = 0;
            resetElevation_btn.Click += new EventHandler(resetElevation_Click);
            panneauElevation.Controls.Add(resetElevation_btn);

            // AJOUT DU PANNEAU DE Width //
            panneauWidth.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.sliderBox_2;
            panneauWidth.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            panneauWidth.Location = new System.Drawing.Point(8, 323);
            panneauWidth.Size = new System.Drawing.Size(206, 56);
            panneauWidth.TabIndex = 0;

            // AJOUT SLIDER Width //
            // 
            Slider_Width.Location = new System.Drawing.Point(3, 6);
            Slider_Width.Maximum = 360;
            Slider_Width.Minimum = 0;
            Slider_Width.Size = new System.Drawing.Size(200, 45);
            Slider_Width.TabIndex = 0;
            Slider_Width.TickFrequency = 0;
            Slider_Width.TickStyle = System.Windows.Forms.TickStyle.None;
            panneauWidth.Controls.Add(Slider_Width);

            //  AJOUT NOM Width SLIDER   //
            slider_Width_lbl.AutoSize = true;
            slider_Width_lbl.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            slider_Width_lbl.ForeColor = System.Drawing.Color.White;
            slider_Width_lbl.Location = new System.Drawing.Point(13, 35);
            slider_Width_lbl.Size = new System.Drawing.Size(46, 13);
            slider_Width_lbl.TabIndex = 3;
            slider_Width_lbl.Text = "Width";
            Slider_Width.Scroll += new System.EventHandler(slider_Width_Scroll);
            panneauWidth.Controls.Add(slider_Width_lbl);

            // AJOUT TEXTBOX VALEUR DE Width

            slider_Width_txt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            slider_Width_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            slider_Width_txt.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            slider_Width_txt.ForeColor = System.Drawing.Color.White;
            slider_Width_txt.Location = new System.Drawing.Point(174, 34);
            slider_Width_txt.Size = new System.Drawing.Size(20, 14);
            slider_Width_txt.TabIndex = 2;
            slider_Width_txt.Text = "180";
            slider_Width_txt.KeyDown += new KeyEventHandler(enterWidthValue);
            panneauWidth.Controls.Add(slider_Width_txt);

            // AJOUT RESET WIDTH   
            resetWidth_btn.Size = new Size(20, 20);
            resetWidth_btn.Location = new Point(93, 32);
            resetWidth_btn.BackgroundImage = Properties.Resources.Reset_btn_2;
            resetWidth_btn.FlatStyle = FlatStyle.Flat;
            resetWidth_btn.FlatAppearance.BorderSize = 0;
            resetWidth_btn.Click += new EventHandler(resetWidth_Click);
            panneauWidth.Controls.Add(resetWidth_btn);

            // AJOUT DU PANNEAU DE Height //
            panneauHeight.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.sliderBox_2;
            panneauHeight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            panneauHeight.Location = new System.Drawing.Point(8, 387);
            panneauHeight.Size = new System.Drawing.Size(206, 56);
            panneauHeight.TabIndex = 0;

            // AJOUT SLIDER Height //
            // 
            Slider_Height.Location = new System.Drawing.Point(3, 6);
            Slider_Height.Maximum = 360;
            Slider_Height.Minimum = 0;
            Slider_Height.Size = new System.Drawing.Size(200, 45);
            Slider_Height.TabIndex = 0;
            Slider_Height.TickFrequency = 0;
            Slider_Height.TickStyle = System.Windows.Forms.TickStyle.None;
            Slider_Height.Scroll += new System.EventHandler(slider_Height_Scroll);
            panneauHeight.Controls.Add(Slider_Height);

            //  AJOUT NOM Height SLIDER   //
            slider_Height_lbl.AutoSize = true;
            slider_Height_lbl.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            slider_Height_lbl.ForeColor = System.Drawing.Color.White;
            slider_Height_lbl.Location = new System.Drawing.Point(13, 35);
            slider_Height_lbl.Size = new System.Drawing.Size(46, 13);
            slider_Height_lbl.TabIndex = 3;
            slider_Height_lbl.Text = "Height";
            panneauHeight.Controls.Add(slider_Height_lbl);

            // AJOUT TEXTBOX VALEUR DE Height

            slider_Height_txt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            slider_Height_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            slider_Height_txt.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            slider_Height_txt.ForeColor = System.Drawing.Color.White;
            slider_Height_txt.Location = new System.Drawing.Point(174, 34);
            slider_Height_txt.Size = new System.Drawing.Size(20, 14);
            slider_Height_txt.TabIndex = 2;
            slider_Height_txt.Text = "180";
            slider_Height_txt.KeyDown += new KeyEventHandler(enterHeightValue);
            panneauHeight.Controls.Add(slider_Height_txt);

            // AJOUT RESET HEIGHT   
            resetHeight_btn.Size = new Size(20, 20);
            resetHeight_btn.Location = new Point(93, 32);
            resetHeight_btn.BackgroundImage = Properties.Resources.Reset_btn_2;
            resetHeight_btn.FlatStyle = FlatStyle.Flat;
            resetHeight_btn.FlatAppearance.BorderSize = 0;
            resetHeight_btn.Click += new EventHandler(resetHeight_Click);
            panneauHeight.Controls.Add(resetHeight_btn);

            // AJOUT DU BOUTON GARDER RATIO
            ratio_btn.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.Ratio_On;
            ratio_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            ratio_btn.Location = new System.Drawing.Point(218, 323);
            ratio_btn.Size = new Size(20, 120);
            ratio_btn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            ratio_btn.UseVisualStyleBackColor = true;
            ratio_btn.TabStop = false;
            ratio_btn.Text = " ";
            ratio_btn.Click += new System.EventHandler(ratio_btn_Click);
            panneauParametres.Controls.Add(ratio_btn);

            // AJOUT DU TEXTBOX POUR LE NOM DES IMAGES
            nomImage_txtBox.Size = new Size(230, 20);
            nomImage_txtBox.Location = new Point(8, 451);
            nomImage_txtBox.BackColor = Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            nomImage_txtBox.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            nomImage_txtBox.ForeColor = System.Drawing.Color.White;
            nomImage_txtBox.BorderStyle = BorderStyle.FixedSingle;
            nomImage = "MonVideo_" + (index + 1); // set la variable nomImage 
            setScript();
            nomImage_txtBox.KeyDown += new KeyEventHandler(changeNameVariable);
            nomImage_txtBox.Text = nomImage;

            panneauParametres.Controls.Add(nomImage_txtBox);

            // AJOUT DU TEXTBOX POUR LE SCRIPT
            scriptOutput_txtBox.Size = new Size(230, 172);
            scriptOutput_txtBox.Location = new Point(8, 479);
            scriptOutput_txtBox.BackColor = Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            scriptOutput_txtBox.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            scriptOutput_txtBox.ForeColor = System.Drawing.Color.White;
            scriptOutput_txtBox.BorderStyle = BorderStyle.FixedSingle;
            scriptOutput_txtBox.HorizontalScrollbar = true;
            scriptOutput_txtBox.SelectionMode = SelectionMode.MultiExtended;
            scriptOutput_txtBox.DoubleClick += new EventHandler(scriptOutput_DoubleClick);
            panneauParametres.Controls.Add(scriptOutput_txtBox);

            // AJOUT BOUTON COPIER
            copieScript_btn.Size = new Size(230, 22);
            copieScript_btn.Location = new Point(8, 659);
            copieScript_btn.Click += new EventHandler(copieScript);
            copieScript_btn.BackgroundImage = Properties.Resources.CopieClipboard;
            copieScript_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            copieScript_btn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            panneauParametres.Controls.Add(copieScript_btn);
            

            // AJOUT PANNEAU CONTROLES VIDEO

            ControlesVideo_Panneau.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.VideoControl_2;
            ControlesVideo_Panneau.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            ControlesVideo_Panneau.Location = new System.Drawing.Point(546, 858);
            ControlesVideo_Panneau.Size = new System.Drawing.Size(390, 104);
            ControlesVideo_Panneau.TabIndex = 0;
            Form1.Controls.Add(ControlesVideo_Panneau);


            // AJOUT PANNEAU VOLUME

            Volume_Panneau.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.Volume_Panel_2;
            Volume_Panneau.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            Volume_Panneau.Location = new System.Drawing.Point(8, 40);
            Volume_Panneau.Size = new System.Drawing.Size(374, 56);
            Volume_Panneau.TabIndex = 0;
            ControlesVideo_Panneau.Controls.Add(Volume_Panneau);

            // AJOUT SLIDER VOLUME
            Volume_Slider.Size = new Size(366, 10);
            Volume_Slider.Location = new Point(4, 4);
            Volume_Slider.Maximum = 100;
            Volume_Slider.Minimum = 0;
            Volume_Slider.Value = 100;
            Volume_Slider.TabIndex = 0;
            Volume_Slider.TickFrequency = 0;
            Volume_Slider.TickStyle = System.Windows.Forms.TickStyle.None;
            Volume_Slider.Scroll += new System.EventHandler(Volume_Slider_Scroll);
            Volume_Panneau.Controls.Add(Volume_Slider);

            // AJOUT LABEL VOLUME

            Volume_txt.AutoSize = true;
            Volume_txt.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            Volume_txt.ForeColor = System.Drawing.Color.White;
            Volume_txt.Location = new System.Drawing.Point(14, 35);
            Volume_txt.Size = new System.Drawing.Size(46, 13);
            Volume_txt.TabIndex = 3;
            Volume_txt.Text = "Volume";
            Volume_Panneau.Controls.Add(Volume_txt);

            // AJOUT TEXTBOX VOLUME

            Volume_lvl.AutoSize = true;
            Volume_lvl.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            Volume_lvl.ForeColor = System.Drawing.Color.White;
            Volume_lvl.Location = new System.Drawing.Point(301, 34);
            Volume_lvl.Size = new System.Drawing.Size(20, 14);
            Volume_lvl.TabIndex = 2;
            Volume_lvl.Text = "100";
            Volume_Panneau.Controls.Add(Volume_lvl);


            marqueur_btn.BackgroundImage = Properties.Resources.Marqueur_5;
            marqueur_btn.BackgroundImageLayout = ImageLayout.Stretch;
            marqueur_btn.Size = new Size(37, 26);
            marqueur_btn.FlatStyle = FlatStyle.Flat;
            marqueur_btn.FlatAppearance.BorderSize = 0;
            marqueur_btn.Location = new Point(8, 8);
            marqueur_btn.Click += new EventHandler(marqueur_btn_Click);
            ControlesVideo_Panneau.Controls.Add(marqueur_btn);

            videoGoToStart_btn.BackgroundImage = Properties.Resources.GotoStart;
            videoGoToStart_btn.Size = new Size(37, 26);
            videoGoToStart_btn.FlatStyle = FlatStyle.Flat;
            videoGoToStart_btn.FlatAppearance.BorderSize = 0;
            videoGoToStart_btn.Location = new Point(50, 8);
            videoGoToStart_btn.Click += new EventHandler(videoGoToStart_btn_Click);
            ControlesVideo_Panneau.Controls.Add(videoGoToStart_btn);

            videoGoToPreviousMarker_btn.BackgroundImage = Properties.Resources.PreviousFrame_1;
            videoGoToPreviousMarker_btn.Size = new Size(37, 26);
            videoGoToPreviousMarker_btn.FlatStyle = FlatStyle.Flat;
            videoGoToPreviousMarker_btn.FlatAppearance.BorderSize = 0;
            videoGoToPreviousMarker_btn.Location = new Point(92, 8);
            videoGoToPreviousMarker_btn.Click += new EventHandler(videoGoToPreviousMarker_btn_Click);
            ControlesVideo_Panneau.Controls.Add(videoGoToPreviousMarker_btn);

            videoBackOneFrame_btn.BackgroundImage = Properties.Resources.lessOneFrame;
            videoBackOneFrame_btn.Size = new Size(37, 26);
            videoBackOneFrame_btn.FlatStyle = FlatStyle.Flat;
            videoBackOneFrame_btn.FlatAppearance.BorderSize = 0;
            videoBackOneFrame_btn.Location = new Point(134, 8);
            videoBackOneFrame_btn.Click += new EventHandler(videoBackOneFrame_btn_Click);
            ControlesVideo_Panneau.Controls.Add(videoBackOneFrame_btn);

            videoPlay_btn.BackgroundImage = Properties.Resources.Play_1;
            videoPlay_btn.Size = new Size(37, 26);
            videoPlay_btn.FlatStyle = FlatStyle.Flat;
            videoPlay_btn.FlatAppearance.BorderSize = 0;
            videoPlay_btn.Location = new Point(176, 8);
            videoPlay_btn.Click += new EventHandler(videoPlay_btn_Click);
            ControlesVideo_Panneau.Controls.Add(videoPlay_btn);

            videoForwardOneFrame_btn.BackgroundImage = Properties.Resources.plusOneFrame;
            videoForwardOneFrame_btn.Size = new Size(37, 26);
            videoForwardOneFrame_btn.FlatStyle = FlatStyle.Flat;
            videoForwardOneFrame_btn.FlatAppearance.BorderSize = 0;
            videoForwardOneFrame_btn.Location = new Point(218, 8);
            videoForwardOneFrame_btn.Click += new EventHandler(videoForwardOneFrame_btn_Click);
            ControlesVideo_Panneau.Controls.Add(videoForwardOneFrame_btn);

            videoGoToNextMarker_btn.BackgroundImage = Properties.Resources.NextFrame_1;
            videoGoToNextMarker_btn.Size = new Size(37, 26);
            videoGoToNextMarker_btn.FlatStyle = FlatStyle.Flat;
            videoGoToNextMarker_btn.FlatAppearance.BorderSize = 0;
            videoGoToNextMarker_btn.Location = new Point(260, 8);
            videoGoToNextMarker_btn.Click += new EventHandler(videoGoToNextMarker_btn_Click);
            ControlesVideo_Panneau.Controls.Add(videoGoToNextMarker_btn);

            videoGoToEnd_btn.BackgroundImage = Properties.Resources.GoToEnd;
            videoGoToEnd_btn.Size = new Size(37, 26);
            videoGoToEnd_btn.FlatStyle = FlatStyle.Flat;
            videoGoToEnd_btn.FlatAppearance.BorderSize = 0;
            videoGoToEnd_btn.Location = new Point(302, 8);
            videoGoToEnd_btn.Click += new EventHandler(videoGoToEnd_btn_Click);
            ControlesVideo_Panneau.Controls.Add(videoGoToEnd_btn);

            loop_btn.BackgroundImage = Properties.Resources.Loop_on;
            loop_btn.Size = new Size(37, 26);
            loop_btn.FlatStyle = FlatStyle.Flat;
            loop_btn.FlatAppearance.BorderSize = 0;
            loop_btn.Location = new Point(344, 8);
            loop_btn.Click += new EventHandler(loop_click);
            ControlesVideo_Panneau.Controls.Add(loop_btn);



            panneauParametres.Controls.Add(panneauHeight);
            panneauParametres.Controls.Add(panneauWidth);
            panneauParametres.Controls.Add(panneauElevation);
            panneauParametres.Controls.Add(panneauRotation);
            panneauParametres.Controls.Add(panneauAzimuth);
            Volume_Slider.SendToBack();
            Slider_Azimuth.SendToBack();
            Slider_Rotation.SendToBack();
            Slider_Elevation.SendToBack();
            Slider_Width.SendToBack();
            Slider_Height.SendToBack();
            Form1.Controls.Add(panneauParametres);

            panneauParametres.Visible = false;



        }
        private void initializationvideoBox(Form Form1, int index)
        {
            panneau.BackgroundImage = DS2_Easy_Viewer.Properties.Resources.Panel_Off_2;
            panneau.Size = new Size(166, 218);
            if (index < 5)
            {
                panneau.Location = new Point(160 + (index * 170), 747);
            }
            else if (index >= 5 & index < 10)
            {
                panneau.Location = new Point(50 + ((index - 5) * 170), 248);
            }
            else if (index >= 10 & index < 15)
            {
                panneau.Location = new Point(50 + ((index - 10) * 170), 485);
            }

            boxIndex = index;
            panneau.Controls.Add(box);

            box.Click += new EventHandler(videoBox_Click);
            box.DoubleClick += new System.EventHandler(videoBox_DoubleClick);
            box.Location = new Point(13, 40);
            box.Size = new Size(140, 140);
            box.BorderStyle = BorderStyle.FixedSingle;
            box.BackgroundImage = Properties.Resources.Double_Click;

            envoyer_btn.Size = new Size(140, 22);
            envoyer_btn.Location = new Point(13, 185);
            envoyer_btn.BackgroundImage = DS2_Easy_Viewer.Properties.Resources.Envoyer_btn;
            envoyer_btn.Text = "";
            envoyer_btn.FlatStyle = FlatStyle.Popup;
            envoyer_btn.Click += new EventHandler(envoyer_Click);
            panneau.Controls.Add(envoyer_btn);

            remove.Size = new Size(12, 12);
            remove.Location = new Point(13, 12);
            remove.BackgroundImage = Properties.Resources.Remove_btn_3;
            remove.Text = "";
            remove.FlatStyle = FlatStyle.Flat;
            remove.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            remove.FlatAppearance.BorderSize = 0;
            remove.Click += new EventHandler(remove_Click);
            panneau.Controls.Add(remove);

            

            loadNew_btn.Click += new EventHandler(videoBox_DoubleClick);
            ToolTip toolTipRemove = new ToolTip();
            toolTipRemove.ShowAlways = true;
            toolTipRemove.SetToolTip(remove, "Text Remove ...");
            ToolTip toolTipView = new ToolTip();
            toolTipView.ShowAlways = true;
            toolTipView.SetToolTip(envoyer_btn, "Text View ...");
            
            videoPause.BackgroundImage = Properties.Resources.Pause_1;
            videoPause.Size = new Size(37, 26);
            videoPause.FlatStyle = FlatStyle.Flat;
            videoPause.FlatAppearance.BorderSize = 0;
            videoPause.Location = new Point(727, 879);
            //videoPause.Click += new EventHandler(videoPause_btn_Click);
            //Form1.Controls.Add(videoPause);

            marqueurs_Panel.Size = new Size(longueurVideoControlPannel, 24);
            marqueurs_Panel.Location = new Point(343, 760);
            //Form1.Controls.Add(marqueurs_Panel);


            timeLine.BackgroundImage = Properties.Resources.Timeline_Progress;
            timeLine.Size = new Size(longueurVideoControlPannel, 24);
            timeLine.Location = new Point(343, 784);
            timeLine.MouseClick += new MouseEventHandler(timeline_Click);
            timeLine.Paint += new PaintEventHandler(timeLinePaintCursor);
            timeLine.MouseMove += new MouseEventHandler(timeline_Move);
            Form1.Controls.Add(currentTimelineTime);
            
            inOutRange_panel.Size = new Size(longueurVideoControlPannel, 11);
            inOutRange_panel.Location = new Point(343, 808);
            inOutRange_panel.Paint += new PaintEventHandler(inOutPaintCursor);
            Form1.Controls.Add(inOutRange_panel);

            in_btn.Size = new Size(7, 11);
            in_btn.Location = new Point (0,0);
            in_btn.BackgroundImage = Properties.Resources.In_2;
            in_btn.MouseMove += new MouseEventHandler(in_btn_Move);
            in_btn.MouseUp += new MouseEventHandler(in_btn_Up);
            in_btn.FlatStyle = FlatStyle.Flat;
            in_btn.FlatAppearance.BorderSize = 0;
            inOutRange_panel.Controls.Add(in_btn);

            
            inLoop_toolTip.ShowAlways = true;
            inLoop_toolTip.SetToolTip(in_btn, "00:00:00");

            Button resetInLoop = new Button();
            resetInLoop.Size = new Size(7, 7);
            resetInLoop.Location = new Point(inOutRange_panel.Left - 9, inOutRange_panel.Top + 2);
            resetInLoop.BackgroundImage = Properties.Resources.loopReset;
            resetInLoop.FlatStyle = FlatStyle.Flat;
            resetInLoop.FlatAppearance.BorderSize = 0;
            resetInLoop.MouseClick += new MouseEventHandler(inResetLoop);
            Form1.Controls.Add(resetInLoop);

            out_btn.Size = new Size(7, 11);
            out_btn.Location = new Point(longueurVideoControlPannel-7, 0);
            out_btn.BackgroundImage = Properties.Resources.Out_2;
            out_btn.MouseMove += new MouseEventHandler(out_btn_Move);
            out_btn.MouseUp += new MouseEventHandler(out_btn_Up);
            out_btn.FlatStyle = FlatStyle.Flat;
            out_btn.FlatAppearance.BorderSize = 0;
            inOutRange_panel.Controls.Add(out_btn);                   
           
            outLoop_tooltip.ShowAlways = true;
            outLoop_tooltip.SetToolTip(out_btn, "00:00:00");

            Button resetOutLoop = new Button();
            resetOutLoop.Size = new Size(7, 7);
            resetOutLoop.Location = new Point(inOutRange_panel.Right + 2, inOutRange_panel.Top + 2);
            resetOutLoop.BackgroundImage = Properties.Resources.loopReset;
            resetOutLoop.FlatStyle = FlatStyle.Flat;
            resetOutLoop.FlatAppearance.BorderSize = 0;
            resetOutLoop.MouseClick += new MouseEventHandler(outResetLoop);
            Form1.Controls.Add(resetOutLoop);

            //curseur.Size = new Size(1, 26);
            //curseur.Location = new Point(342, 762);
            //curseur.BackColor = Color.White;
            //Form1.Controls.Add(curseur);
            currentTimelineTime.Location = new Point(343, 858);
            currentTimelineTime.Text = "00:00:00";
            currentTimelineTime.AutoSize = true;
            currentTimelineTime.BorderStyle = BorderStyle.None;
            currentTimelineTime.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            currentTimelineTime.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            currentTimelineTime.ForeColor = System.Drawing.Color.White;
            Form1.Controls.Add(timeLine);

            tCode.Location = new Point(343, 888);
            tCode.Text = "00000";
            tCode.AutoSize = true;
            tCode.BorderStyle = BorderStyle.None;
            tCode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            tCode.Font = new System.Drawing.Font("Calibri", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tCode.ForeColor = System.Drawing.Color.White;
            tCode.KeyDown += new KeyEventHandler(enterTimeCodeValue);
            Form1.Controls.Add(tCode);


            ((System.ComponentModel.ISupportInitialize)(wmPlayer)).BeginInit();
            wmPlayer.Name = "wmPlayer";
            wmPlayer.Enabled = true;
            ((System.ComponentModel.ISupportInitialize)(wmPlayer)).EndInit();
            wmPlayer.PlayStateChange += new AxWMPLib._WMPOCXEvents_PlayStateChangeEventHandler(wmp_PlayStateChange);
            wmPlayer.OpenStateChange += new AxWMPLib._WMPOCXEvents_OpenStateChangeEventHandler(wmPlayer_OpenStateChange);
            wmPlayer.ClickEvent += new AxWMPLib._WMPOCXEvents_ClickEventHandler(wmPlayer_Click);
            wmPlayer.DoubleClickEvent += new AxWMPLib._WMPOCXEvents_DoubleClickEventHandler(wmPlayer_DoubleClick);
            // After initialization you can customize the Media Player
            wmPlayer.Size = new Size(140, 140);
            wmPlayer.Location = new Point(13, 40);
            //panneau.Controls.Add(wmPlayer);





            videoDureeTotale.Location = new Point(1090,747);
            videoDureeTotale.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            videoDureeTotale.ForeColor = System.Drawing.Color.White;
            videoDureeTotale.Font = new System.Drawing.Font("Calibri", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            videoDureeTotale.Text = "00:00:00";
            videoDureeTotale.RightToLeft = RightToLeft.Yes;
            videoDureeTotale.TextAlign = ContentAlignment.MiddleRight;
            Form1.Controls.Add(videoDureeTotale);           
       
            timelineStart.Location = new Point(339, 747);
            timelineStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            timelineStart.ForeColor = System.Drawing.Color.White;
            timelineStart.Font = new System.Drawing.Font("Calibri", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            timelineStart.Text = "00:00:00";
            Form1.Controls.Add(timelineStart);

            //info_lbl.Location = new Point(9,780);
            //info_lbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            //info_lbl.ForeColor = System.Drawing.Color.White;
            //info_lbl.Font = new System.Drawing.Font("Calibri", 8F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //info_lbl.Text = "Info";
            //Form1.Controls.Add(info_lbl);
            videoName_lbl.Location = new Point(9, 780);
            videoName_lbl.Size = new Size(140, 20);
            videoName_lbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            videoName_lbl.ForeColor = System.Drawing.Color.White;
            videoName_lbl.Font = new System.Drawing.Font("Calibri", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            videoName_lbl.Text = "";
            Form1.Controls.Add(videoName_lbl);

            videoSize_lbl.Location = new Point(9, 820);
            videoSize_lbl.AutoSize = true;
            videoSize_lbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            videoSize_lbl.ForeColor = System.Drawing.Color.White;
            videoSize_lbl.Font = new System.Drawing.Font("Calibri", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            videoSize_lbl.Text = "Taille: ";

            Form1.Controls.Add(videoSize_lbl);
            videoLength_lbl.Location = new Point(9, 840);
            videoLength_lbl.AutoSize = true;
            videoLength_lbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            videoLength_lbl.ForeColor = System.Drawing.Color.White;
            videoLength_lbl.Font = new System.Drawing.Font("Calibri", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            videoLength_lbl.Text = "Durée: ";
            Form1.Controls.Add(videoLength_lbl);

            videoWidth_lbl.Location = new Point(9, 860);
            videoWidth_lbl.AutoSize = true;
            videoWidth_lbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            videoWidth_lbl.ForeColor = System.Drawing.Color.White;
            videoWidth_lbl.Font = new System.Drawing.Font("Calibri", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            videoWidth_lbl.Text = "Largeur: ";
            Form1.Controls.Add(videoWidth_lbl);

            videoHeight_lbl.Location = new Point(9, 880);
            videoHeight_lbl.AutoSize = true;
            videoHeight_lbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            videoHeight_lbl.ForeColor = System.Drawing.Color.White;
            videoHeight_lbl.Font = new System.Drawing.Font("Calibri", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            videoHeight_lbl.Text = "Hauteur: ";
            Form1.Controls.Add(videoHeight_lbl);

            videoBitRate_lbl.Location = new Point(9, 900);
            videoBitRate_lbl.AutoSize = true;
            videoBitRate_lbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            videoBitRate_lbl.ForeColor = System.Drawing.Color.White;
            videoBitRate_lbl.Font = new System.Drawing.Font("Calibri", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            videoBitRate_lbl.Text = "Bitrate: ";
            Form1.Controls.Add(videoBitRate_lbl);

            frameRate_lbl.Location = new Point(9, 920);
            frameRate_lbl.AutoSize = true;
            frameRate_lbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            frameRate_lbl.ForeColor = System.Drawing.Color.White;
            frameRate_lbl.Font = new System.Drawing.Font("Calibri", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            frameRate_lbl.Text = "FrameRate: ";

            Form1.Controls.Add(frameRate_lbl);

            Form1.Controls.Add(info_lbl);
            Form1.Controls.Add(panneau);
            curseur.BringToFront();
        }
       
        public videoBox(Form Form1, int index)
        {

            count += 1;
            initializationLayoutParameters(Form1, index);
            initializationvideoBox(Form1, index);

            listDeSliders.Add(Slider_Azimuth); listDeSliders.Add(Slider_Elevation); listDeSliders.Add(Slider_Rotation); listDeSliders.Add(Slider_Width); listDeSliders.Add(Slider_Height);
            listDeTextBox.Add(slider_Azimuth_txt); listDeTextBox.Add(slider_Elevation_txt); listDeTextBox.Add(slider_Rotation_txt); listDeTextBox.Add(slider_Width_txt); listDeTextBox.Add(slider_Height_txt);
            setInitialParameterValues();  // set les textbox avec les valeurs par défaut de ListeValeurDefautTex
            

        }
        public void Volume_Slider_Scroll(object sender, EventArgs e)
        {
            VolumeLevel = Volume_Slider.Value;
            Volume_lvl.Text = Volume_Slider.Value.ToString();
            setScript();
            if (videoPath != "")
            {
                try
                {
                    Byte[] commande;
                    commande = Ds2Command("Text Volume \"" + nomImage + "\"  1 " + VolumeLevel);
                    envoyerCommande(commande);
                }
                catch (Exception) { MessageBox.Show("No video loaded DUDE! ;)"); }
            }
           
        }
        public void videoGoToPreviousMarker_btn_Click(object sender, EventArgs e) { }
        public void videoBackOneFrame_btn_Click(object sender, EventArgs e)
        {
            if (play == false)
            {
                double goTo = (actualFrame / frameRate) - (1 / frameRate);   // --->       frame suivant
                wmPlayer.Ctlcontrols.currentPosition = goTo;  // envoie le vidéo au nouveau frame
                UpdateLabel();
                Byte[] commande;
                commande = Ds2Command("Text GoTo \"" + nomImage + "\"  " + goTo);
                envoyerCommande(commande);
            }
           
        }
        public void videoGoToStart_btn_Click(object sender, EventArgs e)
        {
            wmPlayer.Ctlcontrols.currentPosition = 0;
            x = 0;
            timeLine.Invalidate();
            currentTimelineTime.Text = "00:00:00";
            tCode.Text = "0000";
            Byte[] commande;
            commande = Ds2Command("Text GoTo \"" + nomImage + "\"  " + 0.0);
            envoyerCommande(commande);
            if (play)
            {
                commande = Ds2Command("Text Play \"" + nomImage + "\"  ");
                envoyerCommande(commande);
            }
           
        }
        private void videoPlay_btn_Click(object sender, EventArgs e)
        {
            if (videoPath != "")
            {
                if (play == false)
                {
                    
                    wmPlayer.Ctlcontrols.play();
                    videoPlay_btn.BackgroundImage = Properties.Resources.Pause_2;
                    play = true;
                    try
                    {
                        Byte[] commande;
                        commande = Ds2Command("Text GoTo \"" + nomImage + "\"  " + wmPlayer.Ctlcontrols.currentPosition);
                        envoyerCommande(commande);
                        commande = Ds2Command("Text Play \"" + nomImage + "\"");
                        envoyerCommande(commande);
                    }
                    catch (Exception) { MessageBox.Show("Oups ça ben l'air que ça marche pas!!"); }
                    


                }
                else
                {
                    wmPlayer.Ctlcontrols.pause();
                    videoPlay_btn.BackgroundImage = Properties.Resources.Play_1;
                    play = false;
                    try
                    {
                        Byte[] commande;
                        commande = Ds2Command("Text Pause \"" + nomImage + "\"");
                        envoyerCommande(commande);
                    }
                    catch (Exception) { MessageBox.Show("Oups ça ben l'air que ça marche pas!!"); }
                }


            }
        }
        public void videoForwardOneFrame_btn_Click(object sender, EventArgs e)
        {
           
            if (play == false)
            {
                double goTo = (actualFrame / frameRate) + (1 / frameRate);   // --->       frame suivant
                wmPlayer.Ctlcontrols.currentPosition = goTo;  // envoie le vidéo au nouveau frame
                UpdateLabel();
                Byte[] commande;
                commande = Ds2Command("Text GoTo \"" + nomImage + "\"  " + goTo);
                envoyerCommande(commande);
            }
           
        }
        public void videoGoToNextMarker_btn_Click(object sender, EventArgs e) { }
        public void videoGoToEnd_btn_Click(object sender, EventArgs e)
        {
            wmPlayer.Ctlcontrols.currentPosition = durationVideoMillis;
            wmPlayer.Ctlcontrols.stop();
            x = (int)durationVideoMillis;
            timeLine.Invalidate();
            Byte[] commande;
            commande = Ds2Command("Text GoTo \"" + nomImage + "\"  " + durationVideoSecondes);
            envoyerCommande(commande);
        }
        public void loop_click(object sender, EventArgs e)
        {
            if (loop == true)
            {
                loop_btn.BackgroundImage = Properties.Resources.Loop;
                loop = false;
            }
            else
            {
                loop_btn.BackgroundImage = Properties.Resources.Loop_on;
                loop = true;
                wmPlayer.Ctlcontrols.currentPosition = 0;
            }
        }
        public void videoInfoUpdate()
        {
            {
                //videoLength.Text = wmPlayer.Ctlcontrols.currentPosition.ToString();
            }
        }
        void wmPlayer_OpenStateChange (object sender, _WMPOCXEvents_OpenStateChangeEvent e)
        {
            if (e.newState == 13)
            {
            }
        }
        void wmp_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            videoState = e.newState;
            if (e.newState == 3)
            {
                
                videoWidth_lbl.Text = "Largeur: " +  wmPlayer.currentMedia.imageSourceWidth.ToString();
                videoHeight_lbl.Text = "Hauteur: " + wmPlayer.currentMedia.imageSourceHeight.ToString();
                frameRate_lbl.Text = "FrameRate: " + frameRate + " fps";
                videoBitRate_lbl.Text = "Bitrate: " + GetBytesReadable(wmPlayer.network.bitRate);

                Thread t = new Thread(new ThreadStart(UpdateLabelThreadProc));
                t.Start();
            }
            if (e.newState == 8)
            {
                if (loop == true)
                {
                    
                    play = true;
                    
                    

                }
                if (loop == false)
                {
                    
                    play = false;
                }
                
            }
            
        }
        void wmPlayer_Click (object sender, AxWMPLib._WMPOCXEvents_ClickEvent e)
        {
            boxSelection();
        }
        void boxSelection()
        {
            if (imageRenommee != null)
            {
                foreach (videoBox boite in DS2_Easy_Viewer.Form1.videoBoxList)
                {
                    boite.panneau.BackgroundImage = DS2_Easy_Viewer.Properties.Resources.Panel_Off_2;
                }
                panneau.BackgroundImage = DS2_Easy_Viewer.Properties.Resources.Panel_On_2;

                foreach (videoBox boite in Form1.videoBoxList)
                {
                    boite.panneauParametres.Visible = false;
                }
                panneauParametres.Visible = true;
                try
                {
                    foreach (imageBox boite in Form1.imageBoxList)
                    {
                        boite.panneau.BackgroundImage = DS2_Easy_Viewer.Properties.Resources.Panel_Off_2;
                    }

                    Form1.boiteSelectionnee = 16;

                    Form1.videoBoxList[0].panneau.BackgroundImage = DS2_Easy_Viewer.Properties.Resources.Panel_On_2;
                    Form1.videoBoxList[0].panneauParametres.Visible = true;

                }
                catch { }

                foreach (imageBox boite in Form1.imageBoxList)
                {
                    boite.panneauParametres.Visible = false;
                }
                panneauParametres.Visible = true;


            }
            nomImage = nomImage_txtBox.Text;
        }
        void wmPlayer_DoubleClick (object sender, AxWMPLib._WMPOCXEvents_DoubleClickEvent e)
        {
            //MessageBox.Show("on se rend ici");
            
            videoBox_DoubleClick(this, EventArgs.Empty);
            return;
        }
        public void videoBox_Click(object sender, EventArgs e)
        {
            boxSelection();
        }
        private void videoBox_DoubleClick(object sender, EventArgs e)
        {
            boxSelection();
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Video File|*.mp4;*.mov;*.avi;*.wmv;*.m4v;";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                //wmPlayer.Ctlcontrols.stop();
                videoPlay_btn.BackgroundImage = Properties.Resources.Play_1;
                play = false;
                x = 0;
                timeLine.Invalidate();
                timeLine.BringToFront();
                inOutRange_panel.Invalidate();
                //wmPlayer.Show();
                // wmPlayer.settings.autoStart = false;
                videoPath = openFileDialog1.FileName;
                videoSource = new VideoFileSource(videoPath);
                videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
                box.BringToFront();
                Thread p = new Thread(new ThreadStart(FFMPEG_playFrames));
                videoSource.Start();
                p.Start();
                //this.wmPlayer.URL = videoPath;
                //wmPlayer.uiMode = "none";
                //wmPlayer.MaximumSize = new Size(140, 140);
                //wmPlayer.BringToFront();
                //wmPlayer.Ctlenabled = false;
                //wmPlayer.settings.volume = 0;
                //loadImage(videoPath);
                //videoName_lbl.Text = Path.GetFileName(videoPath);
                //ShellObject obj = ShellObject.FromParsingName(videoPath);
                //FileInfo fInfo = new FileInfo(videoPath);
                //videoSize_lbl.Text = "Taille: " + GetBytesReadable(fInfo.Length);
                //ShellProperty<UInt32?> bitrate = obj.Properties.GetProperty<UInt32?>("System.Video.TotalBitrate");
                //videoBitRate_lbl.Text = "Bitrate: " + GetBytesReadable(Convert.ToInt64(bitrate.Value)) + "/s";
                //ShellProperty<UInt32?> width = obj.Properties.GetProperty<UInt32?>("System.Video.FrameWidth");
                //videoWidth_lbl.Text = "Largeur: " + width.Value.ToString();
                //videoWidth = Convert.ToInt16(width.Value);
                //ShellProperty<UInt32?> height = obj.Properties.GetProperty<UInt32?>("System.Video.FrameHeight");
                //videoHeight_lbl.Text = "Hauteur: "+ height.Value.ToString();
                //videoHeight = Convert.ToInt16(height.Value);
                //ShellProperty<UInt32?> fps = obj.Properties.GetProperty<UInt32?>("System.Video.FrameRate");
                //setFPS((fps.Value / 1000).ToString());
                //frameRate_lbl.Text = "FrameRate: " + frameRate + " fps";

                //duration = obj.Properties.GetProperty<UInt64?>("System.Media.Duration");
                //durationVideoMillis = (double)duration.Value / 10000.0; // Ex: 134840    
                //durationVideoSecondes = (double)duration.Value/10000000.0; // Ex: 134,84     86,8(0)   125,41  14,9(0)
                ////MessageBox.Show(durationVideoSecondes.ToString());

                string end = formatTimeCode(durationVideoSecondes);
                videoLength_lbl.Text = "Durée: " + end + "  (" + frames + ")";
                videoDureeTotale.Text = end;
                outFrame = (int)(Math.Ceiling(durationVideoSecondes * frameRate));
                inX = 0;
                outX = longueurVideoControlPannel;
                outLoop_tooltip.SetToolTip(out_btn, end);
                facteurConversionTimeline = durationVideoSecondes / 798.0;
                //MessageBox.Show(facteurConversionTimeline.ToString());
                Thread t = new Thread(new ThreadStart(UpdateLabelThreadProc));
                t.Start();
                setInitialParameterValues();
                tCode.Text = "00000";
               
            }
        }

        private void FFMPEG_playFrames()
        {

        }

        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = eventArgs.Frame;
            Bitmap resized = new Bitmap(bitmap, new Size(bitmap.Width / 4, bitmap.Height / 4));
            box.Image = resized;
        }

        public string formatTimeCode(double duration)
        {
            frames = (int)Math.Ceiling(duration * frameRate);
            double decimale = (Convert.ToDouble(duration) % 1) * frameRate;
            //MessageBox.Show("duration : " + duration.ToString() + "\ndecimale: " + decimale.ToString());
            TimeSpan tm = TimeSpan.FromSeconds(duration);
            dureeTotaleDuVideo = string.Format("{1:D2}:{2:D2}:{3:D2}",
                                    tm.Hours,
                                    tm.Minutes,
                                    tm.Seconds,
                                    (int)decimale);
            return dureeTotaleDuVideo;
        }
        public string GetBytesReadable(long i)
        {
            // Get absolute value
            long absolute_i = (i < 0 ? -i : i);
            // Determine the suffix and readable value
            string suffix;
            double readable;
            if (absolute_i >= 0x1000000000000000) // Exabyte
            {
                suffix = "EB";
                readable = (i >> 50);
            }
            else if (absolute_i >= 0x4000000000000) // Petabyte
            {
                suffix = "PB";
                readable = (i >> 40);
            }
            else if (absolute_i >= 0x10000000000) // Terabyte
            {
                suffix = "TB";
                readable = (i >> 30);
            }
            else if (absolute_i >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                readable = (i >> 20);
            }
            else if (absolute_i >= 0x100000) // Megabyte
            {
                suffix = "MB";
                readable = (i >> 10);
            }
            else if (absolute_i >= 0x400) // Kilobyte
            {
                suffix = "KB";
                readable = i;
            }
            else
            {
                return i.ToString("0 B"); // Byte
            }
            // Divide by 1024 to get fractional value
            readable = (readable / 1024);
            // Return formatted number with suffix
            return readable.ToString("0.## ") + suffix;
        }
        public void setFPS(string tempString)
        {
            
            if (string.Compare("12", tempString) == 0) frameRate = 12.0;
            if (string.Compare("15", tempString) == 0) frameRate = 15.0;
            if (string.Compare("23", tempString) == 0) frameRate = 23.976;
            if (string.Compare("24", tempString) == 0) frameRate = 24.0;
            if (string.Compare("25", tempString) == 0) frameRate = 25.0;
            if (string.Compare("29", tempString) == 0) frameRate = 29.97;   // Parce que j'ai tout essayé pour convertir une string en double masi que c# est trop con!!!
            if (string.Compare("30", tempString) == 0) frameRate = 30.0;
            if (string.Compare("50", tempString) == 0) frameRate = 50.0;
            if (string.Compare("59", tempString) == 0) frameRate = 59.94;
            if (string.Compare("60", tempString) == 0) frameRate = 60.0;

        }
        public void UpdateLabelThreadProc()
        {
            if (!currentTimelineTime.IsHandleCreated)
            {
                try
                {
                    currentTimelineTime.CreateControl();
                }
                catch { }
                
            }
            if (!tCode.IsHandleCreated)
            {
                try
                {
                    tCode.CreateControl();
                }
                catch { }
            }
            while (videoState == 3)
            {
                try
                {
                    timeLine.Invalidate();
                    x = Convert.ToInt16(wmPlayer.Ctlcontrols.currentPosition * 800.0 / durationVideoSecondes);
                    //MessageBox.Show(x.ToString());
                    currentTimelineTime.Invoke(new MethodInvoker(UpdateLabel));
                    tCode.Invoke(new MethodInvoker(UpdateLabel));  
                    System.Threading.Thread.Sleep(50);
                }
                catch
                {
                    Console.Write("il y a encore une erreur");
                }
            }
        }
        private void UpdateTcode()
        {

        }
        private void UpdateLabel()
        {
           
            TimeSpan t = TimeSpan.FromMilliseconds(wmPlayer.Ctlcontrols.currentPosition * 1000);
            double frTemp = Convert.ToDouble(t.Milliseconds)*frameRate/1000.00;
            string answer = string.Format("{1:D2}:{2:D2}:{3:D2}fr",
                                    t.Hours,
                                    t.Minutes,
                                    t.Seconds,
                                    Convert.ToInt16(frTemp));
            currentTimelineTime.Text = answer;
            actualFrame = (int)(Math.Ceiling(wmPlayer.Ctlcontrols.currentPosition * frameRate));
            tCode.Text = actualFrame.ToString();
            if (actualFrame >= outFrame & loop == true)
            {

                //wmPlayer.Ctlcontrols.stop();
                try
                {
                    Byte[] commande;
                    commande = Ds2Command("Text GoTo \"" + nomImage + "\"  " + inLoop_timeDecimal);
                    envoyerCommande(commande);
                    commande = Ds2Command("Text Play \"" + nomImage + "\"");
                    envoyerCommande(commande);
                }
                catch (Exception) { }
                    wmPlayer.Ctlcontrols.currentPosition = inLoop_timeDecimal;
                wmPlayer.Ctlcontrols.play();
            }

        }
        private void timeLinePaintCursor(object sender, PaintEventArgs e)
        {
            Graphics g = timeLine.CreateGraphics();
            Pen p = new Pen(Color.White);
            TextureBrush tb = new TextureBrush(Properties.Resources.Timeline_Progress);
            TextureBrush tb2 = new TextureBrush(Properties.Resources.Timeline_Back);
            g.FillRectangle(tb, 0, 0, x, 26);
            g.FillRectangle(tb2, x, 0, 800 - x, 26);
            g.DrawRectangle(p, x, 0, 1, 26);
            
        }
        private void timeline_Click(object sender, MouseEventArgs e)
        {
            if (e.X > 0-1 & e.X < 799)
            {
                x = e.X;
                //curseur.Location = new Point(x, 0);
                timeLine.Invalidate();
                double dbl = Convert.ToDouble(x) * facteurConversionTimeline;
                wmPlayer.Ctlcontrols.currentPosition = dbl;
                UpdateLabel();
                if (play == true)
                {
                    try
                    {
                        Byte[] commande;
                        commande = Ds2Command("Text GoTo \"" + nomImage + "\"  " + wmPlayer.Ctlcontrols.currentPosition);
                        envoyerCommande(commande);
                        commande = Ds2Command("Text Play \"" + nomImage + "\"");
                        envoyerCommande(commande);
                    }
                    catch (Exception) { MessageBox.Show("Oups ça ben l'air que ça marche pas!!"); }
                   
                }
                else
                {
                    try
                    {

                        Byte[] commande;
                        commande = Ds2Command("Text GoTo \"" + nomImage + "\"  " + wmPlayer.Ctlcontrols.currentPosition);
                        envoyerCommande(commande);
                    }
                    catch (Exception) { MessageBox.Show("Oups ça ben l'air que ça marche pas!!"); }
                }

            }
        }
        private void timeline_Move(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (e.X > -1 & e.X < 799)
                {
                    x = e.X;
                    //curseur.Location = new Point(x, 0);
                    timeLine.Invalidate();
                    double dbl = Convert.ToDouble(x) * facteurConversionTimeline;
                    wmPlayer.Ctlcontrols.currentPosition = Convert.ToDouble(x) * facteurConversionTimeline;
                    UpdateLabel();
                    if (play == true)
                    {
                        try
                        {

                            Byte[] commande;
                            commande = Ds2Command("Text GoTo \"" + nomImage + "\"  " + wmPlayer.Ctlcontrols.currentPosition);
                            envoyerCommande(commande);
                            commande = Ds2Command("Text Play \"" + nomImage + "\"");
                            envoyerCommande(commande);
                        }
                        catch (Exception) { MessageBox.Show("Oups ça ben l'air que ça marche pas!!"); }
                    }
                }
               
            }
        }
        private void inOutPaintCursor(object sender, PaintEventArgs e)
        {
            using (Graphics g = e.Graphics)
            {
                Point pt1 = new Point(in_btn.Right, 5);
                Point pt2 = new Point(out_btn.Left, 5);
                using (Pen p = new Pen(Brushes.GreenYellow))
                {
                    g.DrawLine(p, pt1, pt2);
                }
            }

        }
        public void in_btn_Move(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (in_btn.Left + e.X >= 0 & in_btn.Right <= out_btn.Left)
                {

                    in_btn.Left += e.X;
                    inX = in_btn.Left += e.X;
                    inOutRange_panel.Invalidate();
                }
                else if (in_btn.Left + e.X > (inOutRange_panel.Left - 339))
                {
                    in_btn.Left = out_btn.Left - 7;
                    inX = in_btn.Left += e.X;
                    inOutRange_panel.Invalidate();
                }
            }
            
        }
        public void in_btn_Up(object sender, MouseEventArgs e)
        {
            if (in_btn.Right >= out_btn.Left)
            {
                in_btn.Left = out_btn.Left - 8;
            }
            double a = (double)inX * (double)durationVideoSecondes;
            double timeSec = a / longueurVideoControlPannel;
            string inLooping = formatTimeCode(timeSec);
            inLoop_timeDecimal = timeSec;
            inLoop_toolTip.SetToolTip(in_btn, inLooping);
            inFrame = (int)(Math.Ceiling(inLoop_timeDecimal * frameRate));
        }
        public void out_btn_Move(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (out_btn.Right + e.X <= (inOutRange_panel.Right - inOutRange_panel.Left) & out_btn.Left >= in_btn.Right)
                {
                    
                    out_btn.Left += e.X;
                    outX = out_btn.Left;
                    inOutRange_panel.Invalidate();
                }
            }
        }
        public void out_btn_Up(object sender, MouseEventArgs e)
        {
            if (in_btn.Right >= out_btn.Left)
            {
                out_btn.Left = in_btn.Right + 1;
            }
            double a = (double)outX * (double)durationVideoSecondes;
            double timeSec = a / longueurVideoControlPannel ;
            string outLooping = formatTimeCode(timeSec);
            outLoop_timeDecimal = timeSec;
            outLoop_tooltip.SetToolTip(out_btn, outLooping);
            outFrame = (int)(Math.Ceiling(outLoop_timeDecimal * frameRate));
        }
        public void inResetLoop(object sender, MouseEventArgs e)
        {
            inX = 0;
            in_btn.Left = 0;
            inOutRange_panel.Invalidate();
            inLoop_timeDecimal = inFrame = 0;
            inLoop_toolTip.SetToolTip(in_btn, "00:00:00");

        }
        public void outResetLoop(object sender, MouseEventArgs e)
        {
            outX = longueurVideoControlPannel;
            out_btn.Left = longueurVideoControlPannel - 7;
            inOutRange_panel.Invalidate();
            outLoop_timeDecimal = durationVideoSecondes;
            outLoop_tooltip.SetToolTip(out_btn, formatTimeCode(durationVideoSecondes));
            outFrame = outFrame = (int)(Math.Ceiling(durationVideoSecondes * frameRate));
        }
        private void setInitialParameterValues()
        {
            textLocateParameters.Clear();
            textLocateParameters = new List<int> { 0, 0, 90, 0, 180, 180 };
            int a = 1;
            foreach (TrackBar slider in listDeSliders)
            {
                slider.Value = textLocateParameters[a];
                a += 1;
            }
            int b = 1;
            foreach (TextBox text in listDeTextBox)
            {
                text.Text = textLocateParameters[b].ToString();
                b += 1;
            }
            VolumeLevel = 100;
            Volume_Slider.Value = VolumeLevel;
            Volume_lvl.Text = "100";
            setScript();

        }
        private void changeModeImageSliderUpdate(int mode)
        {
            if (imageRenommee != "")
            {
                textLocateParameters.Clear();
                textAddParameters.Clear();
                ShellFile shellFile = ShellFile.FromFilePath(imageRenommee);
                Bitmap shellThumb = shellFile.Thumbnail.Bitmap;
                Image img = shellThumb;
                //Image img = Image.FromFile(imageRenommee); // ne pas loader l'image au complet

                modeDisplay = mode;
                if (mode == 0) // Allsky
                {
                    textLocateParameters = new List<int> { 0, 0, 90, 0, 180, 180 };
                    textAddParameters = new List<int> { 0, 0, 0, 90, 0, 0, 1, 1 };
                    ratio = 1;
                }
                if (mode == 1) // Image
                {
                    ratio = Convert.ToDouble(videoHeight)/Convert.ToDouble(videoWidth);
                    int width = (int)(64 * ratio);
                    //MessageBox.Show(ratio.ToString());
                    textLocateParameters = new List<int> { 0, 0, 30, 0, 64, width };
                    textAddParameters = new List<int> { 0, 0, 0, 30, 0, 0, 0, 0 };
                }
                if (mode == 2) // Panorama
                {
                    textLocateParameters = new List<int> { 0, 0, 0, 0, 180, 15 };
                    textAddParameters = new List<int> { 0, 0, 0, 30, 0, 1, 0, 0 };
                }

                int a = 1;
                foreach (TrackBar slider in listDeSliders)
                {
                    slider.Value = textLocateParameters[a];
                    a += 1;
                }
                int b = 1;
                foreach (TextBox text in listDeTextBox)
                {
                    text.Text = textLocateParameters[b].ToString();
                    b += 1;
                }
                setScript();
            }



        }
        public void loadImage(string filename)
        {
            bool DEBUG = Form1.DEBUG;
            string drivePrefix = filename.Substring(0, 1);
            if (DEBUG == true)
            {
                imageRenommee = filename;
            }

            else
            {
                imageRenommee = "\\\\Ds-Master\\" + drivePrefix + filename.Remove(0, 2);
            }
            try
            {
               
                ShellFile shellFile = ShellFile.FromFilePath(filename);
                Bitmap shellThumb = shellFile.Thumbnail.ExtraLargeBitmap;
                imgSelect.Image = shellThumb;
                imgSelect.SizeMode = PictureBoxSizeMode.Zoom;
                imgSelectLbl.Text = Path.GetFileName(filename);
                setScript();
                setInitialParameterValues();
                textAddLocate();
            }
            catch (Exception) { }
        }
        private void envoyer_Click(object sender, EventArgs e)
        {
            if (surDome == false)
            {
               
                eteindreReste();
                envoyer_btn.BackgroundImage = Properties.Resources.Envoyer_on;
                Byte[] commande;
                commande = Ds2Command("Text View \"" + nomImage + "\"  0 100 100 100 100");
                envoyerCommande(commande);
                surDome = true;
            }
            else if (surDome == true)
            {
                envoyer_btn.BackgroundImage = Properties.Resources.Envoyer_btn;
                Byte[] commande;
                commande = Ds2Command("Text View \"" + nomImage + "\"  0 0 100 100 100");
                envoyerCommande(commande);
                surDome = false;
            }
        }
        private Byte[] Ds2Command(string command)
        {
            Byte[] sendBytes = Encoding.ASCII.GetBytes("\x02" + "DSTA00000000" + "\x07" + "DIRECT" + "\x07" + command + "\x03");
            return sendBytes;
        }
        private void envoyerCommande(Byte[] commande)
        {
            try
            {
                UdpClient udpClient = new UdpClient();
                //udpClient.Send(commande, commande.Length, "192.168.0.100", 2209);
                udpClient.Send(commande, commande.Length, "127.0.0.1", 2209);
            }
            catch (Exception)
            {

            }
        }
        private void Slider_Rotation_Scroll(object sender, EventArgs e)
        {
            try
            {
                Byte[] commande;
                textLocateParameters[3] = Slider_Rotation.Value;
                commande = Ds2Command("Text Locate \"" + nomImage + "\"  " + string.Join(" ", textLocateParameters.ToArray()) + " \"");
                envoyerCommande(commande);
                slider_Rotation_txt.Text = Slider_Rotation.Value.ToString();
                setScript();
            }
            catch (Exception) { MessageBox.Show("la valeur excède le maximum permis pour cet attribut"); }
        }
        private void slider_Azimuth_Scroll(object sender, EventArgs e)
        {
            try
            {
                Byte[] commande;
                textLocateParameters[1] = Slider_Azimuth.Value;
                commande = Ds2Command("Text Locate \"" + nomImage + "\"  " + string.Join(" ", textLocateParameters.ToArray()) + " \"");
                envoyerCommande(commande);
                slider_Azimuth_txt.Text = Slider_Azimuth.Value.ToString();
                setScript();
            }
            catch (Exception) { MessageBox.Show("la valeur excède le maximum permis pour cet attribut"); }
        }
        private void slider_Elevation_Scroll(object sender, EventArgs e)
        {
            try
            {
                Byte[] commande;
                textLocateParameters[2] = Slider_Elevation.Value;
                commande = Ds2Command("Text Locate \"" + nomImage + "\"  " + string.Join(" ", textLocateParameters.ToArray()) + " \"");
                envoyerCommande(commande);
                slider_Elevation_txt.Text = Slider_Elevation.Value.ToString();
                setScript();
            }
            catch (Exception) { MessageBox.Show("la valeur excède le maximum permis pour cet attribut"); }
        }
        private void slider_Width_Scroll(object sender, EventArgs e)
        {
            try
            {
                Byte[] commande;
                textLocateParameters[4] = Slider_Width.Value;
                slider_Width_txt.Text = Slider_Width.Value.ToString();
                if (ratioOn)
                {
                    Slider_Height.Value = (int)Math.Round(Slider_Width.Value * ratio);
                    textLocateParameters[5] = Slider_Height.Value;
                    slider_Height_txt.Text = Slider_Height.Value.ToString();
                }
                commande = Ds2Command("Text Locate \"" + nomImage + "\"  " + string.Join(" ", textLocateParameters.ToArray()) + " \"");
                envoyerCommande(commande);
                setScript();
            }
            catch (Exception) { MessageBox.Show("la valeur excède le maximum permis pour cet attribut"); }
        }
        private void slider_Height_Scroll(object sender, EventArgs e)
        {
            try
            {
                Byte[] commande;
                textLocateParameters[5] = Slider_Height.Value;
                slider_Height_txt.Text = Slider_Height.Value.ToString();
                if (ratioOn)
                {
                    Slider_Width.Value = (int)Math.Round(Slider_Height.Value / ratio);
                    textLocateParameters[4] = Slider_Width.Value;
                    slider_Width_txt.Text = Slider_Width.Value.ToString();
                }
                commande = Ds2Command("Text Locate \"" + nomImage + "\"  " + string.Join(" ", textLocateParameters.ToArray()) + " \"");
                envoyerCommande(commande);
                setScript();
            }
            catch (Exception) { MessageBox.Show("la valeur excède le maximum permis pour cet attribut"); }
        }
        private void ratio_btn_Click(object sender, EventArgs e)
        {
            if (ratioOn == true)
            {
                ratioOn = false;
                ratio_btn.BackgroundImage = Properties.Resources.Ratio_Off;
            }
            else
            {
                ratioOn = true;
                ratio_btn.BackgroundImage = Properties.Resources.Ratio_On;
                ratio = (double)Slider_Height.Value / Slider_Width.Value;

            }
        }
        private void resetRotation_Click(object sender, EventArgs e)
        {
            try
            {
                Byte[] commande;
                textLocateParameters[3] = 0;
                commande = Ds2Command("Text Locate \"" + nomImage + "\"  " + string.Join(" ", textLocateParameters.ToArray()) + " \"");
                envoyerCommande(commande);
                slider_Rotation_txt.Text = textLocateParameters[3].ToString();
                Slider_Rotation.Value = textLocateParameters[3];
                setScript();
            }
            catch (Exception) { MessageBox.Show("la connexion avec Ds-master est impossible"); }
        }
        private void resetAzimuth_Click(object sender, EventArgs e)
        {
            try
            {
                Byte[] commande;
                textLocateParameters[1] = 0;
                commande = Ds2Command("Text Locate \"" + nomImage + "\"  " + string.Join(" ", textLocateParameters.ToArray()) + " \"");
                envoyerCommande(commande);
                slider_Azimuth_txt.Text = textLocateParameters[1].ToString();
                Slider_Azimuth.Value = textLocateParameters[1];
                setScript();
            }
            catch (Exception) { MessageBox.Show("la connexion avec Ds-master est impossible"); }
        }
        private void resetElevation_Click(object sender, EventArgs e)
        {
            try
            {
                Byte[] commande;
                if (modeDisplay == 0) { textLocateParameters[2] = 90; }
                if (modeDisplay == 1) { textLocateParameters[2] = 30; }
                commande = Ds2Command("Text Locate \"" + nomImage + "\"  " + string.Join(" ", textLocateParameters.ToArray()) + " \"");
                envoyerCommande(commande);
                slider_Elevation_txt.Text = textLocateParameters[2].ToString();
                Slider_Elevation.Value = textLocateParameters[2];
                setScript();
            }
            catch (Exception) { MessageBox.Show("la connexion avec Ds-master est impossible"); }
        }
        private void resetWidth_Click(object sender, EventArgs e)
        {
            try
            {
                Byte[] commande;
                if (modeDisplay == 0) { textLocateParameters[4] = 180; }
                if (modeDisplay == 1) { textLocateParameters[4] = 64; }
                commande = Ds2Command("Text Locate \"" + nomImage + "\"  " + string.Join(" ", textLocateParameters.ToArray()) + " \"");
                envoyerCommande(commande);
                slider_Width_txt.Text = textLocateParameters[4].ToString();
                Slider_Width.Value = textLocateParameters[4];
                setScript();
            }
            catch (Exception) { MessageBox.Show("la connexion avec Ds-master est impossible"); }
        }
        private void resetHeight_Click(object sender, EventArgs e)
        {
            try
            {
                ShellFile shellFile = ShellFile.FromFilePath(imageRenommee);
                Bitmap shellThumb = shellFile.Thumbnail.Bitmap;
                Image img = shellThumb;
                //Image img = Image.FromFile(imageRenommee);
                ratio = Convert.ToDouble(videoHeight) / Convert.ToDouble(videoWidth);
                Byte[] commande;
                if (modeDisplay == 0) { textLocateParameters[5] = 180; }
                if (modeDisplay == 1) { textLocateParameters[5] = (int)(64 * ratio); }
                commande = Ds2Command("Text Locate \"" + nomImage + "\"  " + string.Join(" ", textLocateParameters.ToArray()) + " \"");
                envoyerCommande(commande);
                slider_Height_txt.Text = textLocateParameters[5].ToString();
                Slider_Height.Value = textLocateParameters[5];
                setScript();
            }
            catch (Exception) { MessageBox.Show("la connexion avec Ds-master est impossible"); }
        }
        private void setScript()
        {
            scriptOutput_txtBox.Items.Clear();
            scriptOutput_txtBox.Items.Add("Text Add \"" + nomImage + "\"  \"" + imageRenommee + "\"  " + string.Join(" ", textAddParameters.ToArray()));
            scriptOutput_txtBox.Items.Add("+ .5");
            scriptOutput_txtBox.Items.Add("Text Locate \"" + nomImage + "\"  " + string.Join(" ", textLocateParameters.ToArray()));
            scriptOutput_txtBox.Items.Add("+ .1");
            scriptOutput_txtBox.Items.Add("Text View \"" + nomImage + "\"  1 100 100 100 100");
            scriptOutput_txtBox.Items.Add("+ .1");
            scriptOutput_txtBox.Items.Add("Text Volume \"" + nomImage + "\"  1  " + VolumeLevel);
            scriptOutput_txtBox.Items.Add("+ .1");
            scriptOutput_txtBox.Items.Add("Text Play \"" + nomImage + "\"");
            
        }
        private void scriptOutput_DoubleClick(object sender, EventArgs e)
        {

        }
        private void copieScript(object sender, EventArgs e)
        {
            if (scriptOutput_txtBox.Items.Count > 0)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (object row in scriptOutput_txtBox.SelectedItems)
                    {
                        sb.Append(row.ToString());
                        sb.AppendLine();
                    }
                    sb.Remove(sb.Length - 1, 1); // Just to avoid copying last empty row
                    Clipboard.SetData(System.Windows.Forms.DataFormats.Text, sb.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Sélectionnez au moins une ligne de code");
            }


        }
        private void eteindreReste()
        {
            foreach (videoBox boite in Form1.videoBoxList)
            {
                boite.envoyer_btn.BackgroundImage = Properties.Resources.Envoyer_btn;
                boite.surDome = false;
                Byte[] commandeBoites;
                commandeBoites = Ds2Command("Text View \"" + boite.nomImage + "\"  0 0 100 100 100");  // Fermer éteindre les images des autres renderers
                envoyerCommande(commandeBoites);
            }
            envoyer_btn.BackgroundImage = Properties.Resources.Envoyer_on;
            foreach (imageBox boite in Form1.imageBoxList)
            {
                boite.envoyer_btn.BackgroundImage = Properties.Resources.Envoyer_btn;
                boite.surDome = false;
                Byte[] commandeBoites;
                commandeBoites = Ds2Command("Text View \"" + boite.nomImage + "\"  0 0 100 100 100");  // Fermer éteindre les images des autres renderers
                envoyerCommande(commandeBoites);
            }
            envoyer_btn.BackgroundImage = Properties.Resources.Envoyer_on;
        }
        private void textAddLocate()
        {
            Byte[] commande;
            commande = Ds2Command("Text Add \"" + nomImage + "\"  \"" + imageRenommee + "\"  " + string.Join(" ", textAddParameters.ToArray()));
            envoyerCommande(commande);
            Thread.Sleep(100);
            commande = Ds2Command("Text Locate \"" + nomImage + "\"  " + string.Join(" ", textLocateParameters.ToArray()));
            envoyerCommande(commande);
            Thread.Sleep(50);
        }
        private void textView()
        {
            Byte[] commande;
            commande = Ds2Command("Text View \"" + nomImage + "\"  0 100 100 100 100");
            envoyerCommande(commande);
        }
        private void Allsky_btn_Click(object sender, EventArgs e)
        {
            Allsky_btn.BackgroundImage = Properties.Resources.AllSky_btn_On;
            Image_btn.BackgroundImage = Properties.Resources.Image_btn_Off;
            Panorama_btn.BackgroundImage = Properties.Resources.Pano_btn_Off;
            changeModeImageSliderUpdate(0);
            eteindreReste();
            textAddLocate();
            textView();
        }
        private void Image_btn_Click(object sender, EventArgs e)
        {
            Allsky_btn.BackgroundImage = Properties.Resources.AllSky_btn_Off;
            Image_btn.BackgroundImage = Properties.Resources.Image_btn_On;
            Panorama_btn.BackgroundImage = Properties.Resources.Pano_btn_Off;
            changeModeImageSliderUpdate(1);
            eteindreReste();
            textAddLocate();
            textView();
        }
        private void Panorama_btn_Click(object sender, EventArgs e)
        {
            Allsky_btn.BackgroundImage = Properties.Resources.AllSky_btn_Off;
            Image_btn.BackgroundImage = Properties.Resources.Image_btn_Off;
            Panorama_btn.BackgroundImage = Properties.Resources.Pano_btn_On;
            changeModeImageSliderUpdate(2);
            eteindreReste();
            textAddLocate();
            textView();
        }
        private void enterTimeCodeValue(object sender, KeyEventArgs e)
        {
            //MessageBox.Show("ici");
            double dbl; double goTo;
            bool resultat = double.TryParse(tCode.Text, out dbl);
            if (e.KeyCode == Keys.Enter & tCode.Text != "" & resultat)
            {
                    
                    goTo = dbl / frameRate;
                    wmPlayer.Ctlcontrols.currentPosition = goTo;
                    Byte[] commande;
                    commande = Ds2Command("Text GoTo \"" + nomImage + "\"  " + goTo);
                    envoyerCommande(commande);
                    UpdateLabel(); 
            }
           
        }
        private void enterRotationValue(object sender, KeyEventArgs e)
        {
            int integer;
            bool resultat = Int32.TryParse(slider_Rotation_txt.Text, out integer);
            if (e.KeyCode == Keys.Enter & slider_Rotation_txt.Text != "" & resultat)
            {
                if (integer >= -180 & integer <= 180)
                {
                    Slider_Rotation.Value = integer;
                    Slider_Rotation_Scroll(this, EventArgs.Empty);
                }
            }
        }
        private void enterAzimuthValue(object sender, KeyEventArgs e)
        {
            int integer;
            bool resultat = Int32.TryParse(slider_Azimuth_txt.Text, out integer);
            if (e.KeyCode == Keys.Enter & slider_Azimuth_txt.Text != "" & resultat)
            {
                if (integer >= -180 & integer <= 180)
                {
                    Slider_Azimuth.Value = integer;
                    slider_Azimuth_Scroll(this, EventArgs.Empty);
                }
            }
        }
        private void enterElevationValue(object sender, KeyEventArgs e)
        {
            int integer;
            bool resultat = Int32.TryParse(slider_Elevation_txt.Text, out integer);
            if (e.KeyCode == Keys.Enter & slider_Elevation_txt.Text != "" & resultat)
            {
                if (integer >= -90 & integer <= 90)
                {
                    Slider_Elevation.Value = integer;
                    slider_Elevation_Scroll(this, EventArgs.Empty);
                }
            }
        }
        private void enterWidthValue(object sender, KeyEventArgs e)
        {
            int integer;
            bool resultat = Int32.TryParse(slider_Width_txt.Text, out integer);
            if (e.KeyCode == Keys.Enter & slider_Width_txt.Text != "" & resultat)
            {
                if (integer >= 0 & integer <= 360)
                {
                    Slider_Width.Value = integer;
                    slider_Width_Scroll(this, EventArgs.Empty);
                }
            }
        }
        private void enterHeightValue(object sender, KeyEventArgs e)
        {
            int integer;
            bool resultat = Int32.TryParse(slider_Height_txt.Text, out integer);
            if (e.KeyCode == Keys.Enter & slider_Height_txt.Text != "" & resultat)
            {
                if (integer >= 0 & integer <= 360)
                {
                    Slider_Height.Value = integer;
                    slider_Height_Scroll(this, EventArgs.Empty);
                }
            }
        }
        private void changeNameVariable(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Byte[] commande;
                commande = Ds2Command("Text Remove \"" + nomImage + " \"");
                envoyerCommande(commande);
                string newName = nomImage_txtBox.Text;
                nomImage = newName;
                commande = Ds2Command("Text Add \"" + nomImage + "\"  \"" + imageRenommee + "\"" + string.Join(" ", textAddParameters.ToArray()) + " \"");
                envoyerCommande(commande);
                Thread.Sleep(100);
                commande = Ds2Command("Text Locate \"" + nomImage + "\"  " + string.Join(" ", textLocateParameters.ToArray()) + " \"");
                envoyerCommande(commande);
                Thread.Sleep(50);
                commande = Ds2Command("Text View \"" + nomImage + "\"  0 100 100 100 100");
                envoyerCommande(commande);
                Thread.Sleep(50);
                commande = Ds2Command("Text Volume \"" + nomImage + "\"  1  " + VolumeLevel);
                envoyerCommande(commande);
                Thread.Sleep(50);
                scriptOutput_txtBox.Items.Clear();
                scriptOutput_txtBox.Items.Add("Text Add \"" + nomImage + "\"  \"" + imageRenommee + "\"  " + string.Join(" ", textAddParameters.ToArray()));
                scriptOutput_txtBox.Items.Add("+ .1");
                scriptOutput_txtBox.Items.Add("Text Locate \"" + nomImage + "\"  " + string.Join(" ", textLocateParameters.ToArray()));
                scriptOutput_txtBox.Items.Add("+ .1");
                scriptOutput_txtBox.Items.Add("Text View \"" + nomImage + "\"  1 100 100 100 100");
                scriptOutput_txtBox.Items.Add("+ .1");
                scriptOutput_txtBox.Items.Add("Text Volume \"" + nomImage + "\"  1  " + VolumeLevel);
                scriptOutput_txtBox.Items.Add("+ .1");
                scriptOutput_txtBox.Items.Add("Text Play \"" + nomImage + "\"");
            }

        }
        public void remove_Click(object sender, EventArgs e)
        {
            envoyer_btn.BackgroundImage = Properties.Resources.Envoyer_btn;
            wmPlayer.currentPlaylist.clear();
            wmPlayer.Hide();
            Byte[] commande;
            commande = Ds2Command("Text Remove \"" + nomImage + " \"");
            envoyerCommande(commande);
            surDome = false;
            box.Image = null;
            imgSelect.Image = null;
            nomImage = nomImage_txtBox.Text;
            chemin.Text = "";
            imgSelectLbl.Text = "";
        }
        public void resetAll()
        {
            Byte[] commande;
            commande = Ds2Command("Show Reset");
            envoyerCommande(commande);
        }
        public void marqueur_btn_Click(object sender, EventArgs e)
        {
            // Marqueur mark = new Marqueur(mkrIndex, wmPlayer.Ctlcontrols.currentPosition);
            //Marqueurs_liste.Add(mark);
            //Control ctrMarqueur = Marqueurs_liste[mkrIndex].Mark();
            //ctrMarqueur.MouseMove += new MouseEventHandler(mark.update);
            // marqueurs_Panel.Controls.Add(ctrMarqueur);
            //mkrIndex += 1;
            MessageBox.Show("Fonction de marqueurs à venir");
        }
    }

    public class Marqueur
    {
        int index; int posX;
        double time;
        public PictureBox marque = new PictureBox();
        public ToolTip tip = new ToolTip();
        Point mousePos;
        public  Marqueur( int _index, double _time)
        {
            index = _index;
            time = _time;
        }
       

        public Control Mark()
        {
            posX = (int)time;
            marque.Size = new Size(20, 20);
            marque.Location = new Point(posX, 0);
            marque.BackgroundImage = Properties.Resources.Marqueur;
            marque.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            //marque.MouseClick += new MouseEventHandler(marque_bouge);
            tip.ShowAlways = true;
            tip.SetToolTip(this.marque, posX.ToString() + "   "  + index);
            return marque;
        }
        public void update(object sender, MouseEventArgs e)
        {
            mousePos = e.Location;
            if (e.Button == MouseButtons.Left)
            {
                if (e.X > -1 & e.X < 799)
                {
                    //MessageBox.Show("on m'a cliqué dessus");
                    posX = e.X;

                    if (e.Button == MouseButtons.Left)
                    {
                        int dx = e.X - mousePos.X;
                        marque.Location = new Point(marque.Left, 0);
                        tip.SetToolTip(this.marque, posX.ToString());
                    }
                }
            }
        }
    }
    
}
