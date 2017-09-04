using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DS2_Easy_Viewer
{
    public partial class Form1 : Form
    {
        public static List<imageBox> imageBoxList = new List<imageBox>();
        public static int boiteSelectionnee;

        public Form1()
        {
            InitializeComponent();
            for (int i = 0; i < 10; i++)
            {
                imageBox box = new imageBox(this, i);
                imageBoxList.Add(box);
            }
            imageBoxList[boiteSelectionnee].panneauParametres.Visible = true;
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
                foreach (string files in openFileDialog1.FileNames)
                {
                    imageBoxList[increment].loadImage(files);
                    increment += 1;
                }


            }
        }

    }
    

    public partial class imageBox
    {

        public static bool DEBUG = true;

        public string image_Path; public string imageRenommee = "";
        TextBox chemin = new TextBox(); 
        PictureBox box = new PictureBox();
        Button envoyer_btn = new Button(); RadioButton Allsky_btn = new RadioButton(); RadioButton Panorama_btn = new RadioButton(); RadioButton Image_btn = new RadioButton();
        CheckBox ratio_btn = new CheckBox();
        Panel panneau = new Panel(); // Panneau de l'image
        public Panel panneauParametres = new System.Windows.Forms.Panel(); Panel panneauRotation = new System.Windows.Forms.Panel(); Panel panneauAzimuth = new System.Windows.Forms.Panel();
        Panel panneauElevation = new System.Windows.Forms.Panel(); Panel panneauWidth = new System.Windows.Forms.Panel(); Panel panneauHeight = new System.Windows.Forms.Panel();
        Panel panneauImage = new Panel();
        public TrackBar Slider_Rotation = new TrackBar(); TrackBar Slider_Azimuth = new TrackBar(); TrackBar Slider_Elevation = new TrackBar();  TrackBar Slider_Width = new TrackBar(); TrackBar Slider_Height = new TrackBar();
        Label slider_Rotation_lbl = new Label();  Label slider_Azimuth_lbl = new Label();  Label slider_Elevation_lbl = new Label();  Label slider_Width_lbl = new Label(); Label slider_Height_lbl = new Label();
        public TextBox slider_Rotation_txt = new TextBox(); TextBox slider_Azimuth_txt = new TextBox(); TextBox slider_Elevation_txt = new TextBox(); TextBox slider_Width_txt = new TextBox(); TextBox slider_Height_txt = new TextBox();
        private static int boxIndex;
        public static List<int> textAddParameters = new List<int> { 0, 0, 0, 90, 0, 0, 1, 1, 1 };  // crée une liste de listes des paramètres de text add 
        public static List<int> textLocateParameters = new List<int> { 0, 0, 90, 0, 180, 180 };     // crée une liste de listes des paramètres de text locate /// le dernier paramètre est l'opacite de textview
        // [0] RateTime     [1] Azimuth     [2] Elevation   [3] Rotation  [4] Width    [5] height
        private  List<TrackBar> listDeSliders = new List<TrackBar>();
        private  List<TextBox> listDeTextBox = new List<TextBox>();
        public static List<string> listeValeurDefautText = new List<string> { "0","0", "90", "0", "180", "180" };
        int count = 0;


        public imageBox(Form Form1, int index)
        {
            count += 1;
            initializationLayoutParameters(Form1, index);
            initializationImageBox(Form1, index);
         
            listDeSliders.Add(Slider_Rotation); listDeSliders.Add(Slider_Azimuth); listDeSliders.Add(Slider_Elevation); listDeSliders.Add(Slider_Width); listDeSliders.Add(Slider_Height);
            listDeTextBox.Add(slider_Rotation_txt); listDeTextBox.Add(slider_Azimuth_txt); listDeTextBox.Add(slider_Elevation_txt); listDeTextBox.Add(slider_Width_txt); listDeTextBox.Add(slider_Height_txt);
            setInitialParameterValues();  // set les textbox avec les valeurs par défaut de ListeValeurDefautTex
            //MessageBox.Show(count.ToString());
        }
        private void initializationLayoutParameters(Form Form1, int index)
        {
            // AJOUT DU PANNEAU DE PARAMETRES //
            panneauParametres.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.Params_Outline_2;
            panneauParametres.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            panneauParametres.Location = new System.Drawing.Point(910, 12);
            panneauParametres.Size = new System.Drawing.Size(246, 560);
            panneauParametres.TabIndex = 0;

            // AJOUT DES BOUTONS  ALLSKY 
            Allsky_btn.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.AllSky_btn_4;
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
            Image_btn.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.Image_btn_4;
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
            Panorama_btn.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.Pano_btn_4;
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
            panneauAzimuth.Controls.Add(slider_Azimuth_txt);

            // AJOUT DU PANNEAU DE Elevation //
            panneauElevation.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.sliderBox_2;
            panneauElevation.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            panneauElevation.Location = new System.Drawing.Point(8, 158);
            panneauElevation.Size = new System.Drawing.Size(230, 56);
            panneauElevation.TabIndex = 0;

            // AJOUT SLIDER Elevation //
            // 
            Slider_Elevation.Location = new System.Drawing.Point(3, 6);
            Slider_Elevation.Maximum = 90;
            Slider_Elevation.Minimum = -90;
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
            panneauElevation.Controls.Add(slider_Elevation_txt);

            // AJOUT DU PANNEAU DE Width //
            panneauWidth.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.sliderBox_2;
            panneauWidth.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            panneauWidth.Location = new System.Drawing.Point(8, 220);
            panneauWidth.Size = new System.Drawing.Size(230, 56);
            panneauWidth.TabIndex = 0;

            // AJOUT SLIDER Width //
            // 
            Slider_Width.Location = new System.Drawing.Point(3, 6);
            Slider_Width.Maximum = 360;
            Slider_Width.Minimum = 0;
            Slider_Width.Size = new System.Drawing.Size(224, 45);
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
            slider_Width_txt.Location = new System.Drawing.Point(198, 34);
            slider_Width_txt.Size = new System.Drawing.Size(20, 14);
            slider_Width_txt.TabIndex = 2;
            panneauWidth.Controls.Add(slider_Width_txt);

            // AJOUT DU PANNEAU DE Height //
            panneauHeight.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.sliderBox_2;
            panneauHeight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            panneauHeight.Location = new System.Drawing.Point(8, 282);
            panneauHeight.Size = new System.Drawing.Size(230, 56);
            panneauHeight.TabIndex = 0;

            // AJOUT SLIDER Height //
            // 
            Slider_Height.Location = new System.Drawing.Point(3, 6);
            Slider_Height.Maximum = 360;
            Slider_Height.Minimum = 0;
            Slider_Height.Size = new System.Drawing.Size(224, 45);
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
            slider_Height_txt.Location = new System.Drawing.Point(198, 34);
            slider_Height_txt.Size = new System.Drawing.Size(20, 14);
            slider_Height_txt.TabIndex = 2;
            panneauHeight.Controls.Add(slider_Height_txt);

            // AJOUT DU BOUTON GARDER RATIO
            ratio_btn.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.Ratio_2;
            ratio_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            ratio_btn.Location = new System.Drawing.Point(8, 340);
            ratio_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            ratio_btn.Size = new System.Drawing.Size(230, 20);
            ratio_btn.TabIndex = 0;
            ratio_btn.UseVisualStyleBackColor = true;
            ratio_btn.TabStop = false;
            ratio_btn.Text = "                       ";
            ratio_btn.Checked = false;
            panneauParametres.Controls.Add(ratio_btn);


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
            else
            {
                panneau.Location = new Point(50 + ((index - 5) * 170), 250);
            }
            boxIndex = index;
            panneau.Controls.Add(box);
            box.Click += new EventHandler(imageBox_Click);
            box.DoubleClick += new System.EventHandler(imageBox_DoubleClick);
            box.Location = new Point(13, 40);
            box.Size = new Size(140, 140);
            box.BorderStyle = BorderStyle.FixedSingle;
            envoyer_btn.Size = new Size(140, 22);
            envoyer_btn.Location = new Point(13, 185);
            envoyer_btn.BackgroundImage = DS2_Easy_Viewer.Properties.Resources.Envoyer_btn;
            envoyer_btn.Text = "";
            envoyer_btn.FlatStyle = FlatStyle.Popup;
            envoyer_btn.Click += new EventHandler(envoyer_Click);
            chemin.Text = "";
            chemin.Size = new Size(140, 22);
            chemin.BackColor = Color.FromArgb(40, 40, 40);
            chemin.ForeColor = Color.White;
            chemin.BorderStyle = BorderStyle.None;
            chemin.Location = new Point(13, 10);
            chemin.Font = new Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            panneau.Controls.Add(chemin);
            panneau.Controls.Add(envoyer_btn);
            Form1.Controls.Add(panneau);
        }
        private void imageBox_Click(object sender, EventArgs e)
        {
            if (imageRenommee != null)
            {
                foreach (imageBox boite in DS2_Easy_Viewer.Form1.imageBoxList)
                {
                    boite.panneau.BackgroundImage = DS2_Easy_Viewer.Properties.Resources.Panel_Off_2;
                }
                panneau.BackgroundImage = DS2_Easy_Viewer.Properties.Resources.Panel_On_2;
                DS2_Easy_Viewer.Form1.boiteSelectionnee = boxIndex;

                foreach (imageBox boite in Form1.imageBoxList)
                {
                    boite.panneauParametres.Visible = false;
                }
                panneauParametres.Visible = true;
            }

        }
        private void imageBox_DoubleClick(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.tif;*.tiff*,.tga...";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                image_Path = openFileDialog1.FileName;
                loadImage(openFileDialog1.FileName);
                setInitialParameterValues();
            }
        }
        private void setInitialParameterValues()
        {
            textLocateParameters.Clear();
            textLocateParameters = new List<int> { 0, 0, 90, 0, 180, 180 };
            foreach (TrackBar slider in listDeSliders )
            {
                slider.Value = textAddParameters[4];
            }
            int a = 0;
            foreach (TextBox text in listDeTextBox)
            {
                text.Text = listeValeurDefautText[a];
                a += 1;
            }
            //MessageBox.Show("ListDeTextBox Count = " + listDeTextBox.Count() + "\na = " + a );

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
            
            chemin.Text = imageRenommee;
            try
            {
                box.Image = Image.FromFile(imageRenommee);
                box.SizeMode = PictureBoxSizeMode.Zoom;
            }
            catch (Exception) { }
        }
        private void envoyer_Click(object sender, EventArgs e)
        {
            Byte[] commande;
            //fadeOutToutLeMonde();
            commande = Ds2Command("Text Add \"AllSky_" + boxIndex + "\" \"" + imageRenommee + "\"" + string.Join(" ", textAddParameters.ToArray()) + " \"");
            envoyerCommande(commande);
            Thread.Sleep(100);
            commande = Ds2Command("Text Locate \"AllSky_" + boxIndex + "\" " + string.Join(" ", textLocateParameters.ToArray()) + " \"");
            envoyerCommande(commande);
            Thread.Sleep(50);
            commande = Ds2Command("Text View \"AllSky_" + boxIndex + "\" 0 100 100 100 100");
            envoyerCommande(commande);
            Thread.Sleep(50);
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
                commande = Ds2Command("Text Locate \"AllSky_" + (boxIndex) + "\" " + string.Join(" ", textLocateParameters.ToArray()) + " \"");
                envoyerCommande(commande);
                slider_Rotation_txt.Text = Slider_Rotation.Value.ToString();
            }
            catch (Exception) {MessageBox.Show("la connexion avec Ds-master est impossible");}
        }
        private void slider_Azimuth_Scroll(object sender, EventArgs e)
        {
            try
            {
                Byte[] commande;
                textLocateParameters[1] = Slider_Azimuth.Value;
                commande = Ds2Command("Text Locate \"AllSky_" + (boxIndex) + "\" " + string.Join(" ", textLocateParameters.ToArray()) + " \"");
                envoyerCommande(commande);
                slider_Azimuth_txt.Text = Slider_Azimuth.Value.ToString();
            }
            catch (Exception) { MessageBox.Show("la connexion avec Ds-master est impossible"); }
        }
        private void slider_Elevation_Scroll(object sender, EventArgs e)
        {
            try
            {
                Byte[] commande;
                textLocateParameters[2] = Slider_Elevation.Value;
                commande = Ds2Command("Text Locate \"AllSky_" + (boxIndex) + "\" " + string.Join(" ", textLocateParameters.ToArray()) + " \"");
                envoyerCommande(commande);
                slider_Azimuth_txt.Text = Slider_Azimuth.Value.ToString();
            }
            catch (Exception) { MessageBox.Show("la connexion avec Ds-master est impossible"); }
        }
        private void slider_Width_Scroll(object sender, EventArgs e)
        {
            try
            {
                Byte[] commande;
                textLocateParameters[4] = Slider_Width.Value;
                commande = Ds2Command("Text Locate \"AllSky_" + (boxIndex) + "\" " + string.Join(" ", textLocateParameters.ToArray()) + " \"");
                envoyerCommande(commande);
                slider_Azimuth_txt.Text = Slider_Azimuth.Value.ToString();
            }
            catch (Exception) { MessageBox.Show("la connexion avec Ds-master est impossible"); }
        }
        private void slider_Height_Scroll(object sender, EventArgs e)
        {
            try
            {
                Byte[] commande;
                textLocateParameters[5] = Slider_Height.Value;
                commande = Ds2Command("Text Locate \"AllSky_" + (boxIndex) + "\" " + string.Join(" ", textLocateParameters.ToArray()) + " \"");
                envoyerCommande(commande);
                slider_Azimuth_txt.Text = Slider_Azimuth.Value.ToString();
            }
            catch (Exception) { MessageBox.Show("la connexion avec Ds-master est impossible"); }
        }

    }


}
